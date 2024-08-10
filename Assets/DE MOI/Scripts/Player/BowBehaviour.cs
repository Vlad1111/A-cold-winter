using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowBehaviour : MonoBehaviour
{
    public LineRenderer string1, string2;
    public Transform hand;
    public float arrawForce;
    public Transform arraw;
    public Transform arrawPoint;

    private Animator animator;
    private Vector3 targetPoint;
    private Vector3 curentPoint;
    private bool isTargeting = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        curentPoint = targetPoint = (string1.transform.position + string1.transform.position) / 2;
        arraw.gameObject.SetActive(false);
    }

    public void start()
    {
        animator.SetBool("IsDrowing", true);
        //animator.Play("Drawing");
        isTargeting = true;
        arraw.gameObject.SetActive(true);
    }

    public void stop()
    {
        animator.SetBool("IsDrowing", false);
        isTargeting = false;
        arraw.gameObject.SetActive(false);
    }

    private Vector3 convertVector3(Vector3 v)
    {
        return new Vector3(v.z, v.y, v.x);
    }

    private void Update()
    {
        if (isTargeting)
            targetPoint = hand.position;
        else
            targetPoint = (string1.transform.position + string2.transform.position) / 2;
        curentPoint = Vector3.Lerp(curentPoint, targetPoint, Time.deltaTime * 30);
        string1.transform.rotation = Quaternion.identity;
        string2.transform.rotation = Quaternion.identity;
        Vector3 p1 = -string1.transform.position + curentPoint;
        Vector3 p2 = -string2.transform.position + curentPoint;
        //curentPoint = Vector3.Lerp(curentPoint, targetPoint, Time.deltaTime * 10);
        string1.SetPosition(1, p1);
        string2.SetPosition(1, p2);
        arraw.position = curentPoint;
        arraw.LookAt(arrawPoint);
        //Debug.Log(1 / Time.deltaTime);
    }

    internal void shoot()
    {
        Transform newA = Instantiate(arraw, arraw.parent);
        newA.localPosition = arraw.localPosition;
        newA.localRotation = arraw.localRotation;
        newA.localScale = arraw.localScale;
        newA.parent = null;
        ArrawBehaviour arrawBehaviour = newA.gameObject.AddComponent<ArrawBehaviour>();
        if (arrawBehaviour)
        {
            arrawBehaviour.force = arrawForce;
            arrawBehaviour.enabled = true;
            arrawBehaviour.shoot();
        }
    }
}
