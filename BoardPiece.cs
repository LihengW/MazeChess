using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    private (int, int) inner_pos;
    public GameBoard gameboard;


    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInnerPos((int, int) pos)
    {
        inner_pos = pos;
    }

    public (int, int) GetInnerPos
    {
        get { return inner_pos; }
        set {inner_pos = value; }
    }

    public int GetInnerPosX()
    {
        return inner_pos.Item1;
    }

    public int GetInnerPosY()
    {
        return inner_pos.Item2;
    }
}
