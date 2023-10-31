using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playername = "default";
    public int move_ability = 1;
    public int barrier_num = 5;
    private int id;
    private List<(int, int)> target = new List<(int, int)>();
    private GameBoard gameboard;

    // Status
    public bool in_round;
    private (int, int) inner_pos;
    private int m_Status;  // 0 is dead, 1 is active, 2 is blocked; 

    // For Route Searching...
    static private int HashCode;
    private (int, int)[] route;
    private HashSet<int> route_pos_set = new HashSet<int>(); // record postion
    private HashSet<(int, int)> route_pos2pos_set = new HashSet<(int, int)>(); // record action (from pos1 to pos2)
    private HashSet<(int, int)> visited = new HashSet<(int, int)>();
    private (int, int)[] direction;

    // Unit
    private Unit  m_Unit;


    // Animation
    private Animator m_Animator;
    public bool selected;
    private IEnumerator coroutine;
    private Color m_Color;
    private Color m_BlockedColor;
    private float m_EmssionRate;
    private float m_LightIntensity;
    private float m_ColorLerpRate;


    // Start is called before the first frame update
    void Awake()
    {
        m_EmssionRate = 0.1f;
        m_LightIntensity = 1.0f;
        direction = new (int, int)[4]{(0, 1), (0, -1), (1, 0), (-1, 0)};
        m_Animator = transform.GetComponent<Animator>();
        selected = false;
        route = new (int, int)[4];
        m_Status = 1;
        m_ColorLerpRate = 0.0f;
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

    // Static funciton
    public static void SetHashCode(int code)
    {
        HashCode = code;
    }
    public static int PostoHashPos((int, int) pos)
    {
        return pos.Item1 * HashCode + pos.Item2;
    }
    public static (int, int) HashPostoPos(int HashPos)
    {
        return (HashPos / HashCode, HashPos % HashCode);
    }
    public static (int, int) Pos2toHashPos2((int, int) pos1, (int, int) pos2)
    {
        return (PostoHashPos(pos1), PostoHashPos(pos2));
    }
    public static (int, int, int, int) Pos2toHashPos2((int, int) HashPos)
    {
        (int, int) pos1 = HashPostoPos(HashPos.Item1);
        (int, int) pos2 = HashPostoPos(HashPos.Item2);
        return (pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2);
    }
    
    private static (int, int) PositionAdd((int, int) pos1, (int, int) pos2)
    {
        return (pos1.Item1 + pos2.Item1, pos1.Item2 + pos2.Item2);
    }

    // Member Function
    public void SetID(int id) {this.id = id;}

    public int GetID() {return id;}

    public void SetBoard(GameBoard board)
    {
        this.gameboard = board;
    }

    public void SetDirection((int, int)[] dir)
    {
        direction = dir;
    }

    public int GetStatus() {return m_Status;}

    public void SetActive() {m_Status = 1;}
    public void SetBlocked() {m_Status = 2;}
    public void SetDead() {m_Status = 0;}

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
        var main = transform.Find("Particle").GetComponent<ParticleSystem>().main;
        main.startColor = color;
        color = color * m_EmssionRate;
        transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_EmissionColor", color);
    }

    public void SetBlockedColor(Color color)
    {
        m_BlockedColor = color;
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
                coroutine = null;
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
                coroutine = null;
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

    private void Blocked()
    {
        SetBlocked();
        coroutine = BlockAnimation();
    }

    private IEnumerator BlockAnimation()
    {
        while (true)
        {
            if (m_ColorLerpRate >= 1.0f)
            {
                m_ColorLerpRate = 1.0f;
                coroutine = null;
                yield break;
            }
            m_ColorLerpRate += 0.0002f;
            transform.Find("Cylinder").GetComponent<Renderer>().materials[1].SetColor("_Color", (m_Color * (1.0f - m_ColorLerpRate) + m_BlockedColor * m_ColorLerpRate));
            yield return new WaitForFixedUpdate();
        }
    }

    public void Disappear()
    {
        m_Animator.SetTrigger("Vanish");
        m_Unit.GetPlayerList().Remove(this);
        m_Unit.KillPlayer(this);
    }

    public bool FinishDisappearAnimation()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            GameObject.Destroy(gameObject, 1.0f);
            return true;
        }
        else
        {
            return false;
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
        return route_pos2pos_set.Contains(Pos2toHashPos2(from_pos, to_pos));
    }

    public bool DoubleBarrierCheck((int, int) from_pos, (int, int) to_pos)
    {
        return route_pos2pos_set.Contains(Pos2toHashPos2(from_pos, to_pos)) || route_pos2pos_set.Contains(Pos2toHashPos2(to_pos, from_pos));
    }

    public void RouteInit()
    {
        Stack<(int, int)> trace = new Stack<(int, int)>();
        trace.Push(inner_pos);
        bool res = searchroute(inner_pos, trace);
        UpdateRouteRecord();

        visited.Clear();
        Debug.Log("Route Initialized!");
    }


    public bool RelocateRouteSearch()
    {
        Stack<(int, int)> trace = new Stack<(int, int)>();
        trace.Push(inner_pos);
        bool res = searchroute(inner_pos, trace);

        visited.Clear();

        if (res)
        {
            UpdateRouteRecord();
            return true;
        }
        else
        {
            return false;
        }

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
            UpdateRouteRecord();
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
                    if (m_Status == 1 && (gameboard.GetInnerPos(next_pos) == -1 || m_Unit.IsContainPlayer(gameboard.GetInnerPos(next_pos)))) // Active Check 
                    {
                        trace.Push(next_pos);
                        res = res || searchroute(next_pos, trace);
                        if (res) return true;
                        trace.Pop();
                    }
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

    private void UpdateRouteRecord()
    {
        route_pos2pos_set.Clear();
        route_pos_set.Clear();

        string routeinfo = playername.ToString();
        string routesetinfo = playername.ToString();
        for (int i = 0; i < route.Length - 1; i++)
        {
            route_pos_set.Add(PostoHashPos(route[i]));
            route_pos2pos_set.Add(Pos2toHashPos2(route[i], route[i+1]));
            routeinfo += " " + route[i].ToString() + " ";
            routesetinfo += " " + (route[i].Item1, route[i].Item2, route[i+1].Item1, route[i+1].Item2).ToString() + " ";
        }
        route_pos_set.Add(PostoHashPos(route[route.Length - 1]));
        routeinfo += " " + route[route.Length - 1].ToString() + " ";
        Debug.Log(routeinfo);
        Debug.Log(routesetinfo);
    }

    public void KillCheck((int, int) pos)
    {
        if (m_Status == 1)
        {
            if (route_pos_set.Contains(PostoHashPos(pos)))
            {
                Stack<(int, int)> trace = new Stack<(int, int)>();
                trace.Push(inner_pos);
                bool res = searchroute(inner_pos, trace);
                visited.Clear();
                if (res)
                {
                    UpdateRouteRecord();
                }
                else
                {
                    // Chess are Killed (Nowhere to Escape!)
                    if (CheckBox())
                    {
                        SetDead();
                        m_Unit.GetPlayerList().Remove(this);
                        Disappear();
                    }
                    else
                    {
                        Blocked();
                    }
                }
            }
        }
        else if (m_Status == 2)
        {
            // if blocked check if the player is dead
            if (CheckBox())
            {
                SetDead();
                m_Unit.GetPlayerList().Remove(this);
                Disappear();
            }
        }
        else
        {
            Debug.Log("Unknown Status!");
        }
    }

    private bool CheckBox()
    {
        // check for real box
        foreach (var dir in direction)
        {
            (int, int) pos;
            var next_pos = inner_pos;
            do
            {
                pos = next_pos;
                next_pos = PositionAdd(pos, dir);
                if (!gameboard.isInsideBoard(next_pos)) 
                {
                    return false; // boundary box
                }
            } while (!gameboard.HasBarrier(pos, next_pos) && (gameboard.GetInnerPos(next_pos) == -1 || m_Unit.IsContainPlayer(gameboard.GetInnerPos(next_pos))));
        }

        return true; // real box
    }
}
