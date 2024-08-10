using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PickabelObject", order = 1)]
public class PickabelObject : ScriptableObject
{
    [System.Serializable]
    public class __pickabel_data
    {
        public string objectID;
        public int value;
        public int range;
    }

    public string onScreenName;
    public __pickabel_data[] data;
    public Vector2Int minMaxTotalData;
    public string soundToPlayOnPick;

    public Dictionary<string, int> pick()
    {
        Dictionary<string, int> rez = new Dictionary<string, int>();

        for(int i = 0; i < data.Length; i++)
        {
            string key = data[i].objectID;
            int val = data[i].value;
            val += data[i].range - (int)(Random.value * 2 * data[i].range);
            if (val < 0)
                continue;
            rez.Add(key, val);
        }

        return rez;
    }
}
