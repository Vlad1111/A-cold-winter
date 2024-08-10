using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopyArmatureComponents : MonoBehaviour
{
    public Transform toCopy;
    public bool update = false;
    void Start()
    {
        
    }
    public static void deleteGameObject(Object obj)
    {
        //if (Application.isEditor)
        //    DestroyImmediate(obj);
        //else
            Destroy(obj);
    }
    public static void copyColidersComponents(Transform curent, Transform target)
    {
        Collider[] coliders = curent.GetComponents<Collider>();
        foreach(var c in coliders)
        {
            deleteGameObject(c);
        }
        CapsuleCollider[] capCol = target.GetComponents<CapsuleCollider>();
        foreach (var c in capCol)
        {
            CapsuleCollider cap = curent.gameObject.AddComponent<CapsuleCollider>();
            cap.direction = c.direction;
            cap.radius = c.radius;
            cap.height = c.height;
            cap.center = c.center;
            cap.isTrigger = c.isTrigger;
        }
        SphereCollider[] sphCol = target.GetComponents<SphereCollider>();
        foreach (var c in sphCol)
        {
            SphereCollider cap = curent.gameObject.AddComponent<SphereCollider>();
            cap.radius = c.radius;
            cap.center = c.center;
            cap.isTrigger = c.isTrigger;
        }
    }
    public static void copyRigidBodyComponent(Transform curent, Transform target)
    {
        foreach (Transform child in curent)
        {
            Joint[] joints = child.GetComponents<Joint>();
            foreach (var c in joints)
                deleteGameObject(c);
        }
        Rigidbody rb = curent.GetComponent<Rigidbody>();
        Rigidbody trb = target.GetComponent<Rigidbody>();
        if (trb)
        {
            if (rb == null)
                rb = curent.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = trb.isKinematic;
            rb.mass = trb.mass;
            rb.useGravity = trb.useGravity;
            rb.angularDrag = trb.angularDrag;
            rb.drag = trb.drag;
            rb.interpolation = trb.interpolation;
            rb.constraints = trb.constraints;
            rb.collisionDetectionMode = trb.collisionDetectionMode;
        }
        else if(rb)
            deleteGameObject(rb);
    }

    public static void copyJointComponent(Transform curent, Transform target)
    {
        HingeJoint rb = curent.GetComponent<HingeJoint>();
        HingeJoint trb = target.GetComponent<HingeJoint>();
        if (trb)
        {
            if (rb == null)
                rb = curent.gameObject.AddComponent<HingeJoint>();
            rb.connectedBody = curent.parent.GetComponent<Rigidbody>();
            rb.limits = trb.limits;
            rb.axis = trb.axis;
        }
        else if (rb)
            deleteGameObject(rb);
    }

    public static void copyData(Transform curent, Transform target, bool updateCollider = false)
    {
        if(updateCollider)
            copyColidersComponents(curent, target);
        copyRigidBodyComponent(curent, target);
        copyJointComponent(curent, target);
        for (int i = 0; i < target.childCount; i++)
            copyData(curent.GetChild(i), target.GetChild(i));
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            update = false;
            copyData(transform, toCopy);
        }
    }
}
