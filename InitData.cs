using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitData : MonoBehaviour
{
    
    public static InitData Instance { get; private set; }
    public int param { get; set; } = 0;

    public int unitnum;
    public int playernum;
    public int barriernum;
    public int mode;
 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick2Player()
    {
        unitnum = 2;
    }

    public void OnClick4Player()
    {
        unitnum = 4;
    }
}
