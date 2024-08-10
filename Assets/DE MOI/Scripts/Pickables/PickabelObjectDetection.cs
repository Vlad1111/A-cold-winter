using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickabelObjectDetection : MonoBehaviour
{
    public Text centerText;
    public InteractebelOnject pickabel = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Deer")
        {
            Transform cur = other.transform;
            while (cur.tag != "DeerMain" && cur.parent)
                cur = cur.parent;
            PickabelObjectScript pickabel_ = cur.GetComponent<PickabelObjectScript>();
            pickabel = pickabel_;
            if (pickabel)
            {
                centerText.text = "Press [E] to pick up " + pickabel_.pickData.onScreenName;
            }
            else
            {
                pickabel = cur.GetComponent<InteractebelOnject>();
                if (pickabel)
                {
                    centerText.text = "Press [E] to trade";
                }
            }
        }
        else if (other.tag == "Wife")
        {
            pickabel = other.transform.GetComponent<InteractebelOnject>();
            centerText.text = "Press [E] to speak with your wife";
        }
        else if (other.tag == "Intractable")
        {
            pickabel = other.transform.GetComponent<InteractebelOnject>();
            centerText.text = "Press [E] to interact";
        }
        else
        {
            PickabelObjectScript pickabel_ = other.transform.GetComponent<PickabelObjectScript>();
            pickabel = pickabel_;
            if (pickabel)
            {
                centerText.text = "Press [E] to pick up " + pickabel_.pickData.onScreenName;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pickabel && other.tag != "Deer")
            if (pickabel.transform != other.transform)
                return;
        centerText.text = "";
    }

    public void pick()
    {
        if (pickabel)
        {
            if (pickabel.select())
                centerText.text = "";
        }
    }
}
