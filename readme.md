Prompt 1 :
    private InputSystem_Actions controles; pk pas un simple input système

Prompt 2 :
    private InputSystem_Actions controles; il me faut tu un using

Prompt 3 :
    je n'ai pas InputSystem_Actions

Prompt 4 :
    private void Awake()
    {
        controles = new InputSystem_Actions();
        controles.Player.Tap.canceled += Tap_canceled;
        controles.Player.SelectionCouleur.performed += SelectionCouleur_performed;
        controles.Player.LongTap.performed += LongTap_performed;
    } 
    qu'est-ce qui faut faire dans le inputsystem pour reagir au tap


RENDU A PLACER GRILLE