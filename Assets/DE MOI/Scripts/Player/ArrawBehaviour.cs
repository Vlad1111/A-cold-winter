using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrawBehaviour : MonoBehaviour
{
    public float force;
    private Rigidbody rb;

    private float destroyIn = 20;
    private bool stillFlying = true;

    private void verify(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            Destroy(gameObject, 0.5f);
        }
        else if (other.tag.Equals("Tree") || other.tag.Equals("Deer"))
        {
            if (other.tag.Equals("Deer"))
            {
                DeerBehaviour db = null;
                Transform cur = other.transform;
                while (db == null)
                {
                    db = cur.GetComponent<DeerBehaviour>();
                    if (db)
                    {
                        db.getHit(30);
                        break;
                    }
                    if (cur.parent)
                        cur = cur.parent;
                    else break;
                }
            }
            transform.position -= rb.velocity / 80;
            transform.parent = other.transform;
            stillFlying = false;
            Destroy(rb);
            Destroy(this);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        verify(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        verify(other);
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void shoot()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
                rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        rb.velocity += transform.forward * force;
        stillFlying = true;
    }
    public void Update()
    {
        if (stillFlying)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            destroyIn -= Time.deltaTime;
            if (destroyIn < 0)
                Destroy(gameObject, 0);
        }
    }
}
