using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody rg;
    public float force;
    public Vector3 aa;
    void Start()
    {
        rg=GetComponent<Rigidbody>();
       rg.AddForce(Vector3.right * force * -1, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        aa = transform.forward;
        /*Vector3 f = transform.forward * force * Input.GetAxis("Vertical");
        rg.AddForce(f);*/
    }
}
