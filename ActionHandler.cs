using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    private Action cur_action = null;

    private Player cur_player;
    public bool busy = false;

    private Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cur_action != null)
        {
            busy = true;
            var finished = cur_action.implement();
            if (finished)
            {
                cur_action = null;
                busy = false;
            }
        }
        
    }

    public bool SetAction(Move action)
    {
        // init the action
        Player c_player = action.m_FirstObject.GetComponent<Player>();
        BoardPiece c_boardpiece = action.m_SecondObject.GetComponent<BoardPiece>();
        GameBoard c_board = c_boardpiece.gameboard;

        if (!c_player.in_round) return false;

        // inner space
        bool is_legal = c_board.move_player(c_player.GetInnerPos, c_boardpiece.GetInnerPos, c_player);
        if (!is_legal) return false;
        
        c_player.GetInnerPos = c_boardpiece.GetInnerPos;

        // screen space
        action.m_Offset = new Vector3(action.m_SecondObject.transform.position.x - action.m_FirstObject.transform.position.x, 0, action.m_SecondObject.transform.position.z - action.m_FirstObject.transform.position.z).normalized;
        
        // set handler
        cur_action = action;
        busy = true;
        
        return true;
    }

    public bool SetAction(SetBarrier action)
    {
        // init the action
        GameObject BarrierSample = GameObject.Find("Barrier");
        BoardPiece c_boardpiece1 = action.m_FirstObject.GetComponent<BoardPiece>();
        BoardPiece c_boardpiece2 = action.m_SecondObject.GetComponent<BoardPiece>();
        GameBoard c_board = c_boardpiece1.gameboard;

        int Xoffset = Mathf.Abs(c_boardpiece1.GetInnerPos.Item1 - c_boardpiece2.GetInnerPos.Item1);
        int Yoffset = Mathf.Abs(c_boardpiece1.GetInnerPos.Item2 - c_boardpiece2.GetInnerPos.Item2);
        if (Xoffset + Yoffset != 1)
        {
            return false;
        }

        Debug.Log("processing BarrierCheck .......");
        if (!controller.BarrierCheck(c_boardpiece1.GetInnerPos, c_boardpiece2.GetInnerPos)) return false;
        Debug.Log("finish BarrierCheck");

        // init
        c_board.insert_2side_barrier(c_boardpiece1.GetInnerPos, c_boardpiece2.GetInnerPos);
        Vector3 loc = (action.m_FirstObject.transform.position + action.m_SecondObject.transform.position) / 2 + 1.0f * Vector3.up;
        if (Xoffset == 1)
        {
            GameObject barrier = Instantiate(BarrierSample, loc, Quaternion.AngleAxis(90, Vector3.up));
            barrier.GetComponent<Barrier>().GetInnerPos = new Vector4(c_boardpiece1.GetInnerPos.Item1, c_boardpiece1.GetInnerPos.Item2, c_boardpiece2.GetInnerPos.Item1,c_boardpiece2.GetInnerPos.Item2);
        }
        else
        {
            GameObject barrier = Instantiate(BarrierSample, loc, Quaternion.identity);
            barrier.GetComponent<Barrier>().GetInnerPos = new Vector4(c_boardpiece1.GetInnerPos.Item1, c_boardpiece1.GetInnerPos.Item2, c_boardpiece2.GetInnerPos.Item1,c_boardpiece2.GetInnerPos.Item2);
        }
        // set handler
        cur_action = action;
        busy = true;
        return true;
    }

    public void SetCurPlayer(Player player)
    {
        cur_player = player;
    }
}