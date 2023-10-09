using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitData : MonoBehaviour
{
    
    public static InitData Instance { get; private set; }
    public int param { get; set; } = 0;

    public int playernum = 2;
    public int barriernum = 5;
 
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
        playernum = 2;
        barriernum = 8;
    }

    public void OnClick4Player()
    {
        playernum = 4;
        barriernum = 5;
    }
}
