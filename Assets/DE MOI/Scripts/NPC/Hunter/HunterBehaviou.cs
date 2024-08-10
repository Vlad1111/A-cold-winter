using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterBehaviou : DeerBehaviour
{
    public Dictionary<string, int> inventory;
    protected override void initializ()
    {
        inventory = pickabelAfterDeath.pick();
        base.initializ();
    }

    public void trade(Dictionary<string, int> toAdd)
    {
        foreach(string key in toAdd.Keys)
        {
            if (inventory.ContainsKey(key))
                inventory[key] += toAdd[key];
            else
                inventory.Add(key, toAdd[key]);
        }
    }

    protected override void kill()
    {
        for(int i=0;i< pickabelAfterDeath.data.Length; i++)
        {
            pickabelAfterDeath.data[i].range = 0;
            if (inventory.ContainsKey(pickabelAfterDeath.data[i].objectID))
                pickabelAfterDeath.data[i].value = inventory[pickabelAfterDeath.data[i].objectID];
            else
                pickabelAfterDeath.data[i].value = 0;
        }
        base.kill();

        PlayerController.instance.nrHunterKilled++;
    }
}
