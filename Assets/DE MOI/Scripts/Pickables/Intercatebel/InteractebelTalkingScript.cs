using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractebelTalkingScript : InteractebelOnject
{
    public string[] dialog;
    public string[] finaleDialog;

    public override bool select()
    {
        var inv = PlayerController.instance.inventar;
        if(inv.ContainsKey("Wood") && inv.ContainsKey("Food") && inv["Wood"] >= 100 && inv["Food"] >= 200)
        {
            MenuBehaviour.instance.showText(finaleDialog);
            MenuBehaviour.instance.playEnd();
            return true;
        }
        MenuBehaviour.instance.showText(dialog);
        return false;
    }
}
