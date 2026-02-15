using UnityEngine;
/// <summary>
/// Classe pour mes cellules
/// </summary>
public class Cellule : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefabX;
    [SerializeField] private GameObject prefabO;

    [Header("Ajustements")]
    [SerializeField] private float hauteurSymbole = 0.03f;
    [SerializeField] private bool regarderCamera = true;

    [Header("Effet visuel")]
    [SerializeField] private float scaleSelection = 1.1f;

    [Header("Effet gagnant")]
    [SerializeField] private Material materialLueur;
    [SerializeField] private Color couleurVictoire = new Color(1f, 0.84f, 0f); // Or

    private Vector3 scaleInitial;
    private Renderer rend;
    private Material materialOriginal;
    private bool estEnModeVictoire = false;

    public bool EstOccupee { get; private set; }
    public GameController.Joueur Joueur { get; private set; }
    public bool Interactif { get; set; } = true;

    private void Awake()
    {
        // JE sauvegarde le scale initiale.
        scaleInitial = transform.localScale;

        // Je vais chercher le rendu  de la cellule
        rend = GetComponent<Renderer>();

        // Si un rendu existe, je sauvegarde son matériau
        if (rend != null)
        {
            materialOriginal = rend.material;
        }
    }

    /// <summary>
    /// Fonction s'appliquant lorsque j'appuie sur une cellul
    /// </summary>
    public void OnCelluleTapped()
    {
        if (!Interactif || EstOccupee) return;

        Selectionner();

        if (GameController.Instance != null)
            GameController.Instance.Jouer(this);
    }

    /// <summary>
    /// Méthode me permettant de réglé le symbole avant de l'instancier..
    /// </summary>
    /// <param name="joueur">Le joueur actuel.</param>
    public void SetSymbol(GameController.Joueur joueur)
    {
        if (EstOccupee || !Interactif) return;

        EstOccupee = true;
        Joueur = joueur;

        GameObject prefab = joueur == GameController.Joueur.X ? prefabX : prefabO;
        if (prefab == null) return;

        Vector3 position = transform.position + Vector3.up * hauteurSymbole;
        Quaternion rotation = Quaternion.identity;


        ///ChatGPT
        if (joueur == GameController.Joueur.X)
        {
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (regarderCamera && Camera.main != null)
        {
            Vector3 dir = Camera.main.transform.position - position;
            dir.y = 0;
            if (dir != Vector3.zero)
                rotation = Quaternion.LookRotation(dir);
        }

        ARPlacementManager placement = ARPlacementManager.Instance;
        if (placement != null)
        {
            placement.AjouterSymbole(prefab, position, rotation, true);
        }

        Deselectionner();
    }

    /// <summary>
    /// Méthode quand je selectionne une cellule.
    /// </summary>
    public void Selectionner()
    {
        if (!estEnModeVictoire)
            transform.localScale = scaleInitial * scaleSelection;
    }

    /// <summary>
    /// Méthode quand je déselectionne une cellule.
    /// </summary>
    public void Deselectionner()
    {
        if (!estEnModeVictoire)
            transform.localScale = scaleInitial;
    }
    
    /// <summary>
    /// Méthode qui anime la ligne gagnante (couleur et grossissement).
    /// </summary>
    /// <param name="scaleMultiplier">Le multiplicateur de grossisement.</param>
    public void AnimerVictoire(float scaleMultiplier)
    {
        estEnModeVictoire = true;
        transform.localScale = scaleInitial * scaleMultiplier;

        // Changer la couleur pour mettre en évidence CHATGPT
        if (rend != null && materialLueur != null)
        {
            if (scaleMultiplier > 1.15f)
            {
                rend.material = materialLueur;
                rend.material.color = couleurVictoire;
                rend.material.SetColor("_EmissionColor", couleurVictoire * 2f);
            }
            else if (scaleMultiplier <= 1.05f)
            {
                rend.material = materialOriginal;
            }
        }
    }

    /// <summary>
    /// Effacement des données de la cellules.
    /// </summary>
    public void ResetCell()
    {
        EstOccupee = false;
        Interactif = true;
        Joueur = default;
        estEnModeVictoire = false;

        Deselectionner();

        if (rend != null && materialOriginal != null)
        {
            rend.material = materialOriginal;
        }
    }
}