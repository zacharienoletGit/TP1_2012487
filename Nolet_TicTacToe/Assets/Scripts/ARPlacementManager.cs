using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.InputSystem.HID.HID;

/// <summary>
/// Les données de mes symboles.
/// </summary>
[System.Serializable]
public class SymboleData
{
    public GameObject prefab;
    public Vector3 positionRelative;
    public Quaternion rotationRelative;
}

/// <summary>
/// Le gestionnaire de placement.
/// </summary>
public class ARPlacementManager : MonoBehaviour
{
    public static ARPlacementManager Instance;

    [Header("Prefabs et AR")]
    [SerializeField] private GameObject grillePrefab;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private UIManager ui;
    [SerializeField] private GameObject boutonRepositionner;
    [SerializeField] private GameObject boutonRecommencer;

    private List<ARRaycastHit> hits = new();
    private GameObject grilleInstance;
    private List<SymboleData> symbolesData = new();
    private List<GameObject> symbolesInstances = new();

    public bool GrillePlacee { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GrillePlacee)
        {
            boutonRecommencer.SetActive(true);
            boutonRepositionner.SetActive(true);
        }
        else 
        {
            boutonRecommencer.SetActive(false);
            boutonRepositionner.SetActive(false);
        }

        Vector2 touchPosition = Vector2.zero;
        bool hasTouch = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                hasTouch = true;
            }
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            touchPosition = Mouse.current.position.ReadValue();
            hasTouch = true;
        }

        if (hasTouch)
        {
            TenterPlacement(touchPosition);
        }
    }

    /// <summary>
    /// Tente le placement selon une position.
    /// </summary>
    /// <param name="screenPosition">La position à l'écran.</param>
    private void TenterPlacement(Vector2 screenPosition)
    {
        if (planeManager == null || raycastManager == null) return;

        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            PlacerGrille(hits[0].pose);
        }
    }
    
    /// <summary>
    /// Le placement de la grille.
    /// </summary>
    /// <param name="pose">Représentation d'une pose.</param>
    private void PlacerGrille(Pose pose)
    {
        Quaternion adjustedRotation = Quaternion.Euler(0, pose.rotation.eulerAngles.y, 0);

        grilleInstance = Instantiate(grillePrefab, pose.position, adjustedRotation);
        if (grilleInstance == null) return;

        var anchor = grilleInstance.AddComponent<ARAnchor>(); // Je vais chercher mon ancre.

        GrillePlacee = true;

        // Je désactive le plane manager pour ne plus détecter d'autres plans
        if (planeManager != null) planeManager.enabled = false;

        // Initialisation des cellules.
        if (GameController.Instance != null) GameController.Instance.InitialiserCellules();

        // Je change le texte UI
        ui?.ChangerTexte("Joueur X");

        // S'il y a lieu, je replace les symboles
        ReplacerSymboles();
    }
    
    /// <summary>
    /// Ajoute un symbole.
    /// </summary>
    /// <param name="prefab">Le symbole</param>
    /// <param name="worldPos">Sa position</param>
    /// <param name="worldRot">Sa position</param>
    /// <param name="effetPop">S'il a un effet pop.</param>
    public void AjouterSymbole(GameObject prefab, Vector3 worldPos, Quaternion worldRot, bool effetPop = true)
    {
        if (!GrillePlacee || grilleInstance == null) return;

        Vector3 relativePos = grilleInstance.transform.InverseTransformPoint(worldPos);
        Quaternion relativeRot = Quaternion.Inverse(grilleInstance.transform.rotation) * worldRot;

        // Sauvegarde le symbole dans mes données.
        symbolesData.Add(new SymboleData
        {
            prefab = prefab,
            positionRelative = relativePos,
            rotationRelative = relativeRot
        });

        // J'instancie le symbole sur la grille.
        GameObject instance = Instantiate(prefab, grilleInstance.transform);
        instance.transform.localPosition = relativePos;
        instance.transform.localRotation = relativeRot;

        if (effetPop)
        {
            instance.transform.localScale = Vector3.zero;
            StartCoroutine(AnimatePop(instance));
        }

        symbolesInstances.Add(instance);
    }

    /// <summary>
    /// La couroutine servant à l'animation
    /// </summary>
    /// <param name="obj">L'objet à animer.</param>
    /// <returns></returns>
    private IEnumerator AnimatePop(GameObject obj)
    {
        float duration = 0.2f;
        float time = 0f;
        Vector3 targetScale = new Vector3(0.3f,0.3f,0.3f);

        while (time < duration)
        {
            float t = time / duration;
            float scale = Mathf.SmoothStep(0f, 1.2f, t);
            obj.transform.localScale = targetScale * scale;
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale * 1.1f;
        yield return new WaitForSeconds(0.05f);
        obj.transform.localScale = targetScale;
    }

    /// <summary>
    /// Replacer les symboles selon les données.
    /// </summary>
    private void ReplacerSymboles()
    {
        foreach (var instance in symbolesInstances)
            if (instance != null)
                Destroy(instance);

        symbolesInstances.Clear();

        if (grilleInstance == null || symbolesData.Count == 0) return;

        foreach (var data in symbolesData)
        {
            GameObject instance = Instantiate(data.prefab, grilleInstance.transform);
            instance.transform.localPosition = data.positionRelative;
            instance.transform.localRotation = data.rotationRelative;

            symbolesInstances.Add(instance);
        }
    }

    /// <summary>
    /// Pour replaçer la grille.
    /// </summary>
    public void ResetPlacement()
    {
        if (grilleInstance != null) Destroy(grilleInstance);

        foreach (var instance in symbolesInstances)
            if (instance != null)
                Destroy(instance);

        symbolesInstances.Clear();
        GrillePlacee = false;

        if (planeManager != null) planeManager.enabled = true;

        ui?.ChangerTexte("Placer la grille");
    }

    /// <summary>
    /// Pour recommençer la partie.
    /// </summary>
    public void ResetComplet()
    {
        ResetPlacement();
        symbolesData.Clear();

        if (GameController.Instance != null)
            GameController.Instance.ResetJeu();
    }
}
