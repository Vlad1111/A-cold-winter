using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    public bool isMoving;
    public bool loop;
    public bool isStartingFromZero = true;
    public bool setFromTheStart = false;
    public float endTime;

    public AnimationCurve px;
    public AnimationCurve py;
    public AnimationCurve pz;

    public Vector3 pTOffset;
    public Vector3 pPOffset;
    public Vector3 pTMultiply;
    public Vector3 pPMultyply;

    public AnimationCurve rx;
    public AnimationCurve ry;
    public AnimationCurve rz;

    public Vector3 rTOffset;
    public Vector3 rPOffset;
    public Vector3 rTMultiply;
    public Vector3 rPMultyply;

    private Vector3 originalP;
    private Vector3 originalR;

    public float time = 0;
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        originalP = transform.localPosition;
        originalR = transform.localRotation.eulerAngles;
        time = Time.time;

        if (endTime == 0)
            endTime = 1000;

        if (isStartingFromZero)
            time = 0;

        if (setFromTheStart)
            setData();

        //Debug.Log(originalR);
    }

    public void resetMovement(bool goBackwords = false, bool useCurentPosition = true)
    {
        if (useCurentPosition)
        {
            originalP = transform.localPosition;
            originalR = transform.localRotation.eulerAngles;
        }
        if (goBackwords)
        {
            direction = -1;
            time = endTime;
        }
        else
        {
            direction = 1;
            time = 0;
        }
        isMoving = true;
    }

    public void setData()
    {
        float p_x = px.Evaluate(time * pTMultiply.x + pTOffset.x) * pPMultyply.x;
        float p_y = py.Evaluate(time * pTMultiply.y + pTOffset.y) * pPMultyply.y;
        float p_z = pz.Evaluate(time * pTMultiply.z + pTOffset.z) * pPMultyply.z;

        transform.localPosition = new Vector3(originalP.x + p_x + pPOffset.x,
            originalP.y + p_y + pPOffset.y, originalP.z + p_z + pPOffset.z);

        float r_x = rx.Evaluate(time * rTMultiply.x + rTOffset.x) * rPMultyply.x;
        float r_y = ry.Evaluate(time * rTMultiply.y + rTOffset.y) * rPMultyply.y;
        float r_z = rz.Evaluate(time * rTMultiply.z + rTOffset.z) * rPMultyply.z;

        transform.localRotation = Quaternion.Euler(originalR.x + r_x + rPOffset.x,
            originalR.y + r_y + rPOffset.y, originalR.z + r_z + rPOffset.z);
    }

    void Update()
    {
        if (isMoving)
        {
            setData();

            time += Time.deltaTime * direction;
            if (!loop && (time >= endTime || time < 0))
                isMoving = false;
        }
    }
}
