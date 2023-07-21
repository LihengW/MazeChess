using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playername = "default";
    public bool in_round;

    public int move_ability = 1;
    private int id;
    private List<(int, int)> target = new List<(int, int)>();
    private (int, int) inner_pos;

    private GameBoard gameboard;

    // For Barrier Detection
    private (int, int)[] route;
    private HashSet<(int, int, int, int)> routeset = new HashSet<(int, int, int, int)>();
    private HashSet<(int, int)> visited = new HashSet<(int, int)>();

    private (int, int)[] direction;

    // Start is called before the first frame update
    void Awake()
    {
        direction = new (int, int)[4]{(0, 1), (0, -1), (1, 0), (-1, 0)};
    }

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

    public void SetBoard(GameBoard board)
    {
        this.gameboard = board;
    }
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

    public bool BarrierCheck((int, int) from_pos, (int, int) to_pos)
    {
        var pos = (from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2);

        if (routeset.Contains(pos)) return false;
        
        return true;
    }

    public void RouteInit()
    {
        Stack<(int, int)> trace = new Stack<(int, int)>();
        bool res = searchroute(inner_pos, trace);
        for (int i = 0; i < route.Length - 1; i++)
        {
            routeset.Add((route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2));
        }
        visited.Clear();
        Debug.Log("Route Initialized!");
        Debug.Log(this.route);
    }

    public bool reRouteSearch((int, int) from_pos, (int, int) to_pos)
    {
        // fake setting
        gameboard._SetInnerBarrier(from_pos, to_pos, true);
        // search for each target
        Stack<(int, int)> trace = new Stack<(int, int)>();
        bool res = searchroute(inner_pos, trace);
        gameboard._SetInnerBarrier(from_pos, to_pos, false);
        visited.Clear();
        
        if (res)
        {
            routeset.Clear();
            for (int i = 0; i < route.Length - 1; i++)
            {
                routeset.Add((route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2));
            }
            Debug.Log(routeset);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool searchroute((int, int) player_pos, Stack<(int, int)> trace)
    {
        Debug.Log("searching in " + player_pos.ToString() + " .......");
        bool res = false;
        
        if (target.Contains(player_pos)) 
        {
            route = new (int, int)[trace.Count];
            trace.CopyTo(route, 0);
            return true;
        }
        else
        {
            visited.Add(player_pos);
            foreach (var dir in direction)
            {
                var next_pos = PositionAdd(player_pos, dir);
                if (gameboard.isInsideBoard(next_pos) && !visited.Contains(next_pos) && !gameboard.HasBarrier(player_pos, next_pos))
                {
                    Debug.Log("Forwarding to" + next_pos.ToString() + " .......");
                    trace.Push(next_pos);
                    res = res || searchroute(next_pos, trace);
                    if (res) return true;
                    trace.Pop();
                }
                else
                {
                    int failcode = 0;
                    if (!gameboard.isInsideBoard(next_pos)) failcode += 1;
                    else if (visited.Contains(next_pos)) failcode += 10;
                    else if (gameboard.HasBarrier(player_pos, next_pos)) failcode += 100;
                    else failcode += 1000;

                    Debug.Log("Forwarding to" + next_pos.ToString() + "Failed :" + failcode.ToString());
                }
            }
            return res;
        }
    }


    static (int, int) PositionAdd((int, int) pos1, (int, int) pos2)
    {
        return (pos1.Item1 + pos2.Item1, pos1.Item2 + pos2.Item2);
    }
}
