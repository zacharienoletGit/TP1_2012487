using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace TP2
{
    public class MarteauFeedback : MonoBehaviour
    {
        [SerializeField] private HapticImpulsePlayer mainGauche;
        [SerializeField] private HapticImpulsePlayer mainDroite;

        public void VibrerGrab()
        {
            if (mainDroite != null)
                mainDroite.SendHapticImpulse(0.4f, 0.1f);

            if (mainGauche != null)
                mainGauche.SendHapticImpulse(0.4f, 0.1f);
        }

        public void VibrerImpact()
        {
            if (mainDroite != null)
                mainDroite.SendHapticImpulse(0.8f, 0.2f);

            if (mainGauche != null)
                mainGauche.SendHapticImpulse(0.8f, 0.2f);
        }
    }
}