using UnityEngine;

namespace TP2
{
    public class TaupeTarget : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] private int pointsDonnes = 1;
        [SerializeField] private float dureeDeVie = 3f;

        [Header("Audio")]
        [SerializeField] private AudioSource sourceAudio;
        [SerializeField] private AudioClip sonApparition;
        [SerializeField] private AudioClip sonImpact;

        [HideInInspector]
        public TargetSpawner spawner;

        private bool dejaDetruite = false;
        private Collider monCollider;
        private MeshRenderer monRenderer;

        private void Start()
        {
            gameObject.tag = "Target";

            monCollider = GetComponent<Collider>();
            monRenderer = GetComponent<MeshRenderer>();

            if (sourceAudio != null)
            {
                sourceAudio.spatialBlend = 1f;

                if (sonApparition != null)
                {
                    sourceAudio.PlayOneShot(sonApparition);
                }
            }

            Invoke(nameof(Expirer), dureeDeVie);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (dejaDetruite)
                return;

            if (!collision.gameObject.CompareTag("Hammer"))
                return;

            dejaDetruite = true;

            MarteauFeedback feedback = collision.gameObject.GetComponent<MarteauFeedback>();
            if (feedback != null)
            {
                feedback.VibrerImpact();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointsDonnes);
            }

            if (sourceAudio != null && sonImpact != null)
            {
                sourceAudio.PlayOneShot(sonImpact);
            }

            if (monCollider != null)
                monCollider.enabled = false;

            if (monRenderer != null)
                monRenderer.enabled = false;

            if (spawner != null)
            {
                spawner.NotifyTargetDestroyed();
            }

            float delaiDestruction = sonImpact != null ? sonImpact.length : 0f;
            Destroy(gameObject, delaiDestruction);
        }

        private void Expirer()
        {
            if (dejaDetruite)
                return;

            dejaDetruite = true;

            if (spawner != null)
            {
                spawner.NotifyTargetDestroyed();
            }

            Destroy(gameObject);
        }
    }
}