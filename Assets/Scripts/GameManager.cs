using UnityEngine;
using UnityEngine.UI;

namespace TP2
{
    /// <summary>
    /// Gère la logique générale du jeu :
    /// menu, partie, fin de partie, score et minuterie.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Instance globale du GameManager
        /// </summary>
        public static GameManager Instance;

        public enum GameState
        {
            Menu,
            Playing,
            GameOver
        }

        [Header("État du jeu")]
        [SerializeField]
        private GameState state = GameState.Menu;

        [Header("Canvases")]
        [SerializeField, Tooltip("Canvas affiché au menu principal.")]
        private GameObject canvasMenu;

        [SerializeField, Tooltip("Canvas affiché pendant la partie.")]
        private GameObject canvasHUD;

        [SerializeField, Tooltip("Canvas affiché à la fin de la partie.")]
        private GameObject canvasGameOver;

        [Header("Textes UI")]
        [SerializeField, Tooltip("Texte du score affiché en jeu.")]
        private Text texteScore;

        [SerializeField, Tooltip("Texte du temps restant.")]
        private Text texteTimer;

        [SerializeField, Tooltip("Texte affiché à la fin avec le score final.")]
        private Text texteScoreFinal;

        [Header("Gameplay")]
        [SerializeField, Tooltip("Durée totale d'une partie en secondes.")]
        private float dureePartie = 60f;

        [Header("Références de scène")]
        [SerializeField, Tooltip("Position où replacer le marteau quand la partie commence.")]
        private Transform pointSpawnMarteau;

        [SerializeField, Tooltip("Référence vers le marteau du joueur.")]
        private GameObject marteau;

        [SerializeField, Tooltip("Parent contenant toutes les cibles actives.")]
        private Transform conteneurCibles;

        /// <summary>
        /// Score actuel du joueur
        /// </summary>
        private int scoreActuel;

        /// <summary>
        /// Temps restant avant la fin de la partie
        /// </summary>
        private float tempsRestant;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ChangerEtat(GameState.Menu);
            MettreAJourHUD();
        }

        private void Update()
        {
            if (state != GameState.Playing)
                return;

            tempsRestant -= Time.deltaTime;

            if (tempsRestant < 0f)
                tempsRestant = 0f;

            MettreAJourHUD();

            if (tempsRestant <= 0f)
            {
                FinDePartie();
            }
        }

        /// <summary>
        /// Vérifie si une partie est actuellement en cours
        /// </summary>
        public bool IsPlaying()
        {
            return state == GameState.Playing;
        }

        /// <summary>
        /// Démarre une nouvelle partie
        /// </summary>
        public void StartGame()
        {
            scoreActuel = 0;
            tempsRestant = dureePartie;

            EffacerCibles();
            ReplacerMarteau();

            ChangerEtat(GameState.Playing);
            MettreAJourHUD();
        }

        /// <summary>
        /// Redémarre la partie
        /// </summary>
        public void RestartGame()
        {
            StartGame();
        }

        /// <summary>
        /// Retourne au menu principal
        /// </summary>
        public void ReturnToMenu()
        {
            EffacerCibles();
            ReplacerMarteau();
            ChangerEtat(GameState.Menu);
        }

        /// <summary>
        /// Ajoute des points au score
        /// </summary>
        public void AddScore(int points)
        {
            if (state != GameState.Playing)
                return;

            scoreActuel += points;
            MettreAJourHUD();
        }

        /// <summary>
        /// Termine la partie et affiche l'écran de fin
        /// </summary>
        private void FinDePartie()
        {
            ChangerEtat(GameState.GameOver);

            if (texteScoreFinal != null)
            {
                texteScoreFinal.text = "Final score: " + scoreActuel;
            }
        }

        /// <summary>
        /// Change l'état du jeu et active les bons canvases
        /// </summary>
        public void ChangerEtat(GameState nouvelEtat)
        {
            state = nouvelEtat;

            if (canvasMenu != null) canvasMenu.SetActive(state == GameState.Menu);
            if (canvasHUD != null) canvasHUD.SetActive(state == GameState.Playing);
            if (canvasGameOver != null) canvasGameOver.SetActive(state == GameState.GameOver);
        }

        /// <summary>
        /// Met à jour les textes de l'interface
        /// </summary>
        private void MettreAJourHUD()
        {
            if (texteScore != null)
            {
                texteScore.text = "Score: " + scoreActuel;
            }

            if (texteTimer != null)
            {
                texteTimer.text = "Time: " + Mathf.CeilToInt(tempsRestant);
            }
        }

        /// <summary>
        /// Supprime toutes les cibles encore présentes
        /// </summary>
        private void EffacerCibles()
        {
            if (conteneurCibles == null)
                return;

            for (int i = conteneurCibles.childCount - 1; i >= 0; i--)
            {
                Destroy(conteneurCibles.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Replace le marteau à son point de départ
        /// </summary>
        private void ReplacerMarteau()
        {
            if (marteau == null || pointSpawnMarteau == null)
                return;

            marteau.transform.position = pointSpawnMarteau.position;
            marteau.transform.rotation = pointSpawnMarteau.rotation;

            Rigidbody rb = marteau.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}