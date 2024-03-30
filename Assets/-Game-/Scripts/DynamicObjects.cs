using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjects : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    public void Hit(RaycastHit hit)
    {
        rb.AddForce(-hit.normal * 100f);
    }
}
