using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playername = "default";
    public bool in_round;

    public int move_ability = 1;
    public int barrier_num = 5;
    private int id;
    private List<(int, int)> target = new List<(int, int)>();
    private (int, int) inner_pos;
    private GameBoard gameboard;

    // For Barrier Detection
    private (int, int)[] route;
    private HashSet<string> routeset = new HashSet<string>();
    private HashSet<(int, int)> visited = new HashSet<(int, int)>();

    private (int, int)[] direction;

    // Unit
    private Unit  m_Unit;


    // Animation
    private Animator m_Animator;
    public bool selected;
    private IEnumerator coroutine;
    private Color m_Color;
    private float m_EmssionRate;
    private float m_LightIntensity;

    // Start is called before the first frame update
    void Awake()
    {
        m_EmssionRate = 0.1f;
        m_LightIntensity = 1.0f;
        direction = new (int, int)[4]{(0, 1), (0, -1), (1, 0), (-1, 0)};
        m_Animator = transform.GetComponent<Animator>();
        selected = false;
    }

    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutine != null)
        {
            StartCoroutine(coroutine);
        }
    }

    public void SetID(int id) {this.id = id;}

    public int GetID() {return id;}

    public void SetBoard(GameBoard board)
    {
        this.gameboard = board;
    }

    public void SetUnit(Unit unit)
    {
        m_Unit = unit;
    }

    public Unit GetUnit()
    {
        return m_Unit;
    }

    public void SetColor(Color color)
    {
        m_Color = color;
        transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_Color", color);
        transform.Find("PlayerLight").GetComponent<Light>().color = color;
        color = color * m_EmssionRate;
        transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", color);
    }

    public void Selected()
    {
        m_Animator.SetTrigger("Select");

        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIDLE"))
        {
            m_Animator.ResetTrigger("Deselect");
        }

        selected = true;

        coroutine = SelectAnimation();
    }

    private IEnumerator SelectAnimation()
    {
        while (true)
        {
            if (!selected)
            {
                yield return StartCoroutine(DeselectAnimation());
            }

            if (transform.position.y > 0.3f)
            {
                yield break;
            }

            transform.position = transform.position + new Vector3(0, 0.00005f, 0);
            m_EmssionRate += 0.0002f;
            m_LightIntensity += 0.00015f;
            transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", m_Color * m_EmssionRate);
            transform.Find("PlayerLight").GetComponent<Light>().intensity = m_LightIntensity;
            yield return new WaitForFixedUpdate();
        }
    }

    public void Deselected()
    {
        m_Animator.SetTrigger("Deselect");
        
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Selected"))
        {
            m_Animator.ResetTrigger("Select");
        }

        selected = false;

        coroutine = DeselectAnimation();
    }

    private IEnumerator DeselectAnimation()
    {
        while (true)
        {
            if (selected)
            {
                yield return StartCoroutine(SelectAnimation());
            }
            if (transform.position.y < 0.0f)
            {
                m_EmssionRate = 0.1f;
                m_LightIntensity = 1.0f;
                yield break;
            }
            transform.position = transform.position - new Vector3(0, 0.00005f, 0);
            m_EmssionRate -= 0.0002f;
            m_LightIntensity -= 0.00015f;
            transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", m_Color * m_EmssionRate);
            transform.Find("PlayerLight").GetComponent<Light>().intensity = m_LightIntensity;
            yield return new WaitForFixedUpdate();
        }
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
        var pos = (from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2).ToString();

        if (routeset.Contains(pos)) return true;
        
        return false;
    }

    public bool DoubleBarrierCheck((int, int) from_pos, (int, int) to_pos)
    {
        var pos1 = (from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2).ToString();
        var pos2 = (to_pos.Item1, to_pos.Item2, from_pos.Item1, from_pos.Item2).ToString();

        if (routeset.Contains(pos1) || routeset.Contains(pos2)) return true;
        
        return false;
    }

    public void RouteInit()
    {
        Stack<(int, int)> trace = new Stack<(int, int)>();
        trace.Push(inner_pos);
        bool res = searchroute(inner_pos, trace);
        string routeinfo = playername.ToString();
        for (int i = 0; i < route.Length - 1; i++)
        {
            routeset.Add((route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2).ToString());
            routeinfo += " " + route[i].ToString() + " ";
        }
        visited.Clear();

        routeinfo += " " + route[route.Length - 1].ToString() + " ";
        Debug.Log(routeinfo);
        Debug.Log("Route Initialized!");
    }

    public bool reRouteSearch((int, int) from_pos, (int, int) to_pos, bool doubleside)
    {
        // fake setting
        gameboard._SetInnerBarrier(from_pos, to_pos, true);
        if (doubleside) gameboard._SetInnerBarrier(to_pos, from_pos, true);

        Debug.Log("Seting Barrier" + from_pos.ToString() + " to " + to_pos.ToString());
        // search for each target
        Stack<(int, int)> trace = new Stack<(int, int)>();
        trace.Push(inner_pos);
        bool res = searchroute(inner_pos, trace);
        
        // cancel fake setting 
        gameboard._SetInnerBarrier(from_pos, to_pos, false);
        if (doubleside) gameboard._SetInnerBarrier(to_pos, from_pos, false);
        
        visited.Clear();
        
        if (res)
        {
            routeset.Clear();
            string routeinfo = playername.ToString();
            string routesetinfo = playername.ToString();
            for (int i = 0; i < route.Length - 1; i++)
            {
                routeset.Add((route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2).ToString());
                routeinfo += " " + route[i].ToString() + " ";
                routesetinfo += " " + (route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2).ToString() + " ";
            }
            routeinfo += " " + route[route.Length - 1].ToString() + " ";
            Debug.Log(routeinfo);
            Debug.Log(routesetinfo);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool searchroute((int, int) player_pos, Stack<(int, int)> trace)
    {
        //Debug.Log(playername.ToString() + " : searching in " + player_pos.ToString() + " .......");
        bool res = false;
        
        if (target.Contains(player_pos)) 
        {
            route = new (int, int)[trace.Count];
            trace.CopyTo(route, 0);
            Array.Reverse(route);
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
                    //Debug.Log("Forwarding to" + next_pos.ToString() + " .......");
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

                    //Debug.Log("Forwarding to" + next_pos.ToString() + "Failed :" + failcode.ToString());
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
