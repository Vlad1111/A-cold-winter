using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractebelTradeScript : InteractebelOnject
{
    private HunterBehaviou hb;
    public override bool select()
    {
        if (hb == null)
            hb = GetComponent<HunterBehaviou>();
        if (hb == null)
            return true;
        PlayerController.instance.startTrading(hb);
        return false;
    }
}
