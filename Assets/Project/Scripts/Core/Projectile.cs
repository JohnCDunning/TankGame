using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float _MovementSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1))
        {

            Vector3 incomingVec = hit.point - transform.position;
            transform.forward = Vector3.Reflect(incomingVec, hit.normal);
        }

        rb.velocity = transform.forward * _MovementSpeed;

    
    }
 
}
