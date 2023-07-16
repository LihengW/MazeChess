using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playername = "default";
    public bool in_round;

    public int move_ability = 1;
    private int id;
    private List<(int, int)> target= new List<(int, int)>();
    private (int, int) inner_pos;

    // For Barrier Detection
    private List<(int, int)> route;
    private HashSet<(int, int, int, int)> routeset = new HashSet<(int, int, int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void SetID(int id) {this.id = id;}

    public int GetID() {return id;}

    public (int, int) GetInnerPos
    {
        get { return inner_pos; }
        set {inner_pos = value; }
    }

    public void AddTarget((int, int) pos)
    {
        target.Add(pos);
    }

    public bool ReachTarget()
    {
        return target.Contains(inner_pos);
    }

    public bool CheckNewBarrier((int, int, int, int) barrierpos)
    {
        return true;
    }
}
