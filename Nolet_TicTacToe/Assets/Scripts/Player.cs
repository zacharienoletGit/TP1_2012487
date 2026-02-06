using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [SerializeField, Tooltip("Distance des rayons maximum")]
    private float distance = 10;
    
    [SerializeField, Tooltip("Le prefab de la grille.")]
    private GameObject grille;

    [SerializeField]
    private ARRaycastManager aRRaycastManager;

    private InputSystem_Actions controles;

    private bool debutPartie = true;

    [SerializeField, Tooltip("Le texte afficher")]
    private TextMeshProUGUI texteDirectives;

    private void Awake()
    {
        controles = new InputSystem_Actions();

        controles.Player.Tap.performed += ctx => PlacerGrilleOnTap(ctx);
        controles.Player.Enable();

        texteDirectives.text = "Joueur X";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlacerGrilleOnTap(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {

        if (!debutPartie) return;

        if (controles.Player.Tap.enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                Instantiate(grille, hit.point, rotation);

                debutPartie = false;
            }
        }
    }



}
