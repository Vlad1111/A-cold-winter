using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractebelAnimationActivationScript : InteractebelOnject
{
    private bool direction = false;
    public override bool select()
    {
        PlatformBehaviour pb = GetComponent<PlatformBehaviour>();
        pb.resetMovement(direction, false);
        direction = !direction;
        return false;
    }
}
