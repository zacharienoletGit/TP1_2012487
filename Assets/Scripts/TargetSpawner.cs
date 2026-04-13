using TP2;
using UnityEngine;

namespace TP2
{
    /// <summary>
    /// Gère l'apparition des cibles dans la scène.
    /// Le script fait apparaître des taupes à différents points de spawn
    /// pendant que la partie est active.
    /// </summary>
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField, Tooltip("Prefab de la cible à instancier.")]
        private GameObject prefabCible;

        [SerializeField, Tooltip("Liste des emplacements possibles pour faire apparaître une cible.")]
        private Transform[] pointsSpawn;

        [SerializeField, Tooltip("Parent utilisé pour garder la hiérarchie propre dans Unity.")]
        private Transform conteneurCibles;

        [Header("Réglages du spawn")]
        [SerializeField, Tooltip("Délai avant le tout premier spawn.")]
        private float delaiPremierSpawn = 1f;

        [SerializeField, Tooltip("Temps entre chaque tentative de spawn.")]
        private float intervalleSpawn = 2f;

        [SerializeField, Tooltip("Nombre maximal de cibles actives en même temps.")]
        private int maxCiblesActives = 3;

        /// <summary>
        /// Nombre de cibles actuellement présentes dans la scène
        /// </summary>
        private int nbCiblesActives;

        /// <summary>
        /// Permet d'éviter de démarrer le système plusieurs fois
        /// </summary>
        private bool spawnDemarre;

        private void Update()
        {
            if (!spawnDemarre)
            {
                spawnDemarre = true;
                InvokeRepeating(nameof(FaireSpawnCible), delaiPremierSpawn, intervalleSpawn);
            }
        }

        /// <summary>
        /// Fait apparaître une nouvelle cible si les conditions sont respectées
        /// </summary>
        private void FaireSpawnCible()
        {
            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying())
                return;

            if (prefabCible == null || pointsSpawn == null || pointsSpawn.Length == 0)
                return;

            if (nbCiblesActives >= maxCiblesActives)
                return;

            int indexAleatoire = Random.Range(0, pointsSpawn.Length);
            Transform pointChoisi = pointsSpawn[indexAleatoire];

            GameObject nouvelleCible = Instantiate(
                prefabCible,
                pointChoisi.position,
                pointChoisi.rotation,
                conteneurCibles
            );

            TaupeTarget scriptCible = nouvelleCible.GetComponent<TaupeTarget>();
            if (scriptCible != null)
            {
                scriptCible.spawner = this;
            }

            nbCiblesActives++;
        }

        /// <summary>
        /// Appelé par une cible quand elle disparaît
        /// </summary>
        public void NotifyTargetDestroyed()
        {
            nbCiblesActives--;

            if (nbCiblesActives < 0)
                nbCiblesActives = 0;
        }
    }
}