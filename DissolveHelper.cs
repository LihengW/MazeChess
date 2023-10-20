using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveHelper : MonoBehaviour
{
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = transform.Find("Cylinder").GetComponent<Renderer>().materials[0];
    }

    // Update is called once per frame
    void Update()
    {
	    material.SetFloat("_DissolveThreshold", 0.5f);
    }
}
