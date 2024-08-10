using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBehaviour : MonoBehaviour
{
    public PickabelObject treeData;
    public bool isMakingContact = false;
    private float timeLastHit = 0;

    private void hit(Collider other)
    {
        if (Time.time - timeLastHit >= 1)
        {
            if (other.tag.Equals("Tree"))
            {
                PlayerController.instance.addToInventar(treeData.pick());
                PlayerController.instance.playSound(treeData.soundToPlayOnPick);
            }
            if (other.tag.Equals("Deer"))
            {
                DeerBehaviour db = null;
                Transform cur = other.transform;
                while (db == null)
                {
                    db = cur.GetComponent<DeerBehaviour>();
                    if (db)
                    {
                        db.getHit(75);
                        break;
                    }
                    if (cur.parent)
                        cur = cur.parent;
                    else break;
                }
            }
            timeLastHit = Time.time;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isMakingContact)
            return;
        hit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isMakingContact)
            return;
        hit(other);
    }

    private void OnTriggerExit(Collider other)
    {
        timeLastHit = 0;
    }
}
