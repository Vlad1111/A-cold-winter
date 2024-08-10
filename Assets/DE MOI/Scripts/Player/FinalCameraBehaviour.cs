using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCameraBehaviour : MonoBehaviour
{
    private Transform NextTarget = null;
    private float position = 0;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private void Awake()
    {
        transform.parent = null;
        NextTarget = GameObject.FindGameObjectWithTag("FinaleCameraTrail").transform;
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void Update()
    {
        if (NextTarget == null)
            return;

        Vector3 newPoz = lastPosition * (1 - position) + NextTarget.position * position;
        transform.position = newPoz;
        transform.rotation = Quaternion.Lerp(lastRotation, NextTarget.rotation, position);
        position += Time.deltaTime / 3;
        if(position > 1)
        {
            if (NextTarget.childCount > 0)
            {
                NextTarget = NextTarget.GetChild(0);
                lastPosition = newPoz;
                lastRotation = transform.rotation;
                position = 0;
            }
            else
                position = 1;
        }
        Debug.Log(position + " " + NextTarget);
    }
}
