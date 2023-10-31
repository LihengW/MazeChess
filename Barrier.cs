using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Vector4 inner_pos;
    // Start is called before the first frame update
    void Awake()
    {
        inner_pos = new Vector4(-1.0f, -1.0f, -1.0f, -1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(inner_pos);
    }

    public Vector4 GetInnerPos
    {
        get { return inner_pos; }
        set {inner_pos = value; }
    }
}
