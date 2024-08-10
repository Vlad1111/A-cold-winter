using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickabelObjectScript : InteractebelOnject
{
    public PickabelObject pickData;

    public override bool select()
    {
        PlayerController.instance.addToInventar(pickData.pick());
        PlayerController.instance.activatePickUpAction();
        PlayerController.instance.playSound(pickData.soundToPlayOnPick);
        Destroy(gameObject);
        return true;
    }
}
