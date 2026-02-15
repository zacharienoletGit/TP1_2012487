using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("Le texte afficher")]
    private TextMeshProUGUI texteDirectives;

    public void ChangerTexte(string nouveauTexte)
    {
        texteDirectives.text = nouveauTexte;
    }
}
