using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractebelOnject : MonoBehaviour
{
    public virtual bool select()
    {
        Debug.Log("Was selected");
        return true;
    }
}
