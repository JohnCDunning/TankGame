using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float _MovementSpeed;
    public float _ReflectionsRemaining = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PhotonNetwork.isMasterClient)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1))
            {
                Hit(hit.point, hit.normal);

            }

            rb.velocity = transform.forward * _MovementSpeed;
        }
    }
    void Hit(Vector3 hit,Vector3 hitNormal)
    {
        if (_ReflectionsRemaining > 0)
        {
            Vector3 incomingVec = hit - transform.position;
            transform.forward = Vector3.Reflect(incomingVec, hitNormal);
            _ReflectionsRemaining--;
            return;
        }
        if (PhotonNetwork.isMasterClient)
        //- The projectile will explode now
            PhotonNetwork.Destroy(gameObject);
    }
 
}
