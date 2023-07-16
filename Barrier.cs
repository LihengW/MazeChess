using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private Vector4 inner_pos;
    // Start is called before the first frame update
    void Start()
    {
        inner_pos = new Vector4();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector4 GetInnerPos
    {
        get { return inner_pos; }
        set {inner_pos = value; }
    }
}
