using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public enum Joueur { X, O }

    private Joueur joueurActuel = Joueur.X;
    private bool partieTerminee = false;
    private Cellule[] cellules;

    [SerializeField] private UIManager uiManager;

    [Header("Effet ligne gagnante")]
    [SerializeField] private Color couleurLigneGagnante = Color.yellow;
    [SerializeField] private float intensiteLueur = 2f;
    [SerializeField] private float dureePulse = 0.5f;

    private InputSystem_Actions controles;
    private bool peutJouer = false;

    private void Awake()
    {
        Instance = this;
        controles = new InputSystem_Actions();
        controles.Player.Tap.performed += OnTap;
        controles.Player.Enable();
    }

    /// <summary>
    /// Initialisation des cellules.
    /// </summary>
    public void InitialiserCellules()
    {
        cellules = FindObjectsOfType<Cellule>();
        foreach (var cellule in cellules)
        {
            cellule.ResetCell();
        }
        joueurActuel = Joueur.X;
        Debug.Log("Cellules rechargées : " + cellules.Length);
        StartCoroutine(ActiverJeuApresUnFrame());
    }

    /// <summary>
    /// Activer le jeu un peu après l'instanciation de la grille.
    /// </summary>
    /// <returns>Un IEnumerator</returns>
    private IEnumerator ActiverJeuApresUnFrame()
    {
        peutJouer = false;
        yield return null; // Attend une frame.
        peutJouer = true;
    }

    /// <summary>
    /// Méthode s'activant lorsque je tap.
    /// </summary>
    /// <param name="ctx">Le contexte qui a activé le tap.</param>
    private void OnTap(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!peutJouer) return;
        if (partieTerminee) return;
        if (cellules == null || cellules.Length == 0) return;

        Vector2 position = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Cellule cellule = hit.collider.GetComponent<Cellule>();
            if (cellule != null && cellule.Interactif)
                Jouer(cellule);
        }
    }

    /// <summary>
    /// Lorsque j'appuie sur la cellule.
    /// </summary>
    /// <param name="cellule">La cellule selectionnée</param>
    public void Jouer(Cellule cellule)
    {
        if (cellule.EstOccupee) return;

        cellule.SetSymbol(joueurActuel);

        int[] ligneGagnante = VerifierVictoire();
        if (ligneGagnante != null)
        {
            partieTerminee = true;
            uiManager?.ChangerTexte("Victoire  " + joueurActuel);
            StartCoroutine(AnimerLigneGagnante(ligneGagnante));
            return;
        }

        if (VerifierMatchNul())
        {
            partieTerminee = true;
            uiManager?.ChangerTexte("Match Nul");
            return;
        }

        joueurActuel = joueurActuel == Joueur.X ? Joueur.O : Joueur.X;
        uiManager?.ChangerTexte("Joueur " + joueurActuel);
    }

    /// <summary>
    /// Méthode verifiant le gagnant.
    /// </summary>
    /// <returns>Un tableau de int des index des cellules gagnantes.</returns>
    private int[] VerifierVictoire()
    {
        int[,] combinaisons =
        {
            {0,1,2},{3,4,5},{6,7,8},
            {0,3,6},{1,4,7},{2,5,8},
            {0,4,8},{2,4,6}
        };

        if (cellules == null || cellules.Length < 9) return null;

        for (int i = 0; i < 8; i++)
        {
            int idx1 = combinaisons[i, 0];
            int idx2 = combinaisons[i, 1];
            int idx3 = combinaisons[i, 2];

            Cellule c1 = cellules[idx1];
            Cellule c2 = cellules[idx2];
            Cellule c3 = cellules[idx3];

            if (c1.EstOccupee && c2.EstOccupee && c3.EstOccupee && c1.Joueur == joueurActuel && c2.Joueur == joueurActuel && c3.Joueur == joueurActuel)
            {
                return new int[] { idx1, idx2, idx3 }; // On retourne les 3 indexs gagnants  pour l'animation.
            }
        }

        return null;
    }

    /// <summary>
    /// Méthode pour animer la ligne gagnante.
    /// </summary>
    /// <param name="indices">Les index des cellules gagnantes.</param>
    /// <returns></returns>
    private IEnumerator AnimerLigneGagnante(int[] indices)
    {
        // Je désactive l'interaction.
        foreach (var cellule in cellules)
        {
            cellule.Interactif = false;
        }

        // Animation de pulse sur la ligne gagnante.
        float tempsEcoule = 0f;
        float cycles = 3f;

        while (tempsEcoule < dureePulse * cycles)
        {
            float t = Mathf.PingPong(tempsEcoule / dureePulse, 1f);
            float scale = Mathf.Lerp(1f, 1.3f, t);

            foreach (int idx in indices)
            {
                cellules[idx].AnimerVictoire(scale);
            }

            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // Je retoure à la taille normale.
        foreach (int idx in indices)
        {
            cellules[idx].AnimerVictoire(1f);
        }
    }

    /// <summary>
    /// Vérife si toute les cellules sont occupées.
    /// </summary>
    /// <returns>Un bool</returns>
    private bool VerifierMatchNul()
    {
        if (cellules == null) return false;
        foreach (Cellule cellule in cellules)
            if (!cellule.EstOccupee)
                return false;
        return true;
    }

    /// <summary>
    /// Pour recommencer la partie.
    /// </summary>
    public void ResetJeu()
    {
        joueurActuel = Joueur.X;
        partieTerminee = false;
        peutJouer = false;
        cellules = null;
        uiManager?.ChangerTexte("Placer la grille");
    }
}