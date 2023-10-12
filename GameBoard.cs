using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    // prerequest of making up of gameboard
    public GameObject boardpiece;
    public GameObject playerunit;

    // bisac attr of gameboard
    public int gridx = 9;
    public int gridy = 9;
    public GameObject[,] gameboard;

    public GameObject[] barriers;

    private int[,] innerboard;
    private bool[, , ,] innerbarrier;
    public float distance = 2.05F;  

    void Awake()
    {
        gameboard = new GameObject[gridx, gridy];
        innerboard = new int[gridx, gridy];
        innerbarrier = new bool[gridx, gridy, gridx, gridy];

        for (int x = 0; x < gridx; x++) {
            for (int y = 0; y < gridy; y++) {
                innerboard[x, y] = -1;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // create the board & pos_mat
        for (int x = 0; x < gridx; x++) {
            for (int y = 0; y < gridy; y++) {
                gameboard[x, y] = Instantiate(boardpiece, new Vector3(x * distance, 0, y * distance), Quaternion.AngleAxis(90, Vector3.left));
                gameboard[x, y].GetComponent<BoardPiece>().gameboard = this;
                gameboard[x, y].GetComponent<BoardPiece>().SetInnerPos((x, y));
                SetGameObjectLayer(gameboard[x, y],  "Board");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public GameObject create_player((int, int) pos, int ID) {
        // player ID
        innerboard[pos.Item1, pos.Item2] = ID;
        (float, float) world_pos = board_to_world(pos);
        GameObject player = Instantiate(playerunit, new Vector3(world_pos.Item1, 0.0f, world_pos.Item2), Quaternion.identity);
        player.GetComponent<Player>().GetInnerPos = pos;
        player.GetComponent<Player>().SetBoard(this);
        SetGameObjectLayer(player, "Player");

        return player;
    }

    public bool move_player((int, int) from_pos, (int, int) to_pos, Player player) {
        // check legality
        if (innerboard[to_pos.Item1, to_pos.Item2] != -1) return false; // no player in the position
        if (Math.Abs(to_pos.Item1 - from_pos.Item1) + Math.Abs(to_pos.Item2 - from_pos.Item2) > player.move_ability) return false; // over the moving distance
        if (innerbarrier[from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2]) return false; // no barrier in the way

        int id = innerboard[from_pos.Item1, from_pos.Item2];
        innerboard[from_pos.Item1, from_pos.Item2] = -1;
        innerboard[to_pos.Item1, to_pos.Item2] = id;
        return true;
    }

    public bool insert_1side_barrier((int, int) from_pos, (int, int) to_pos)
    {
        if (innerbarrier[from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2])
        {
            return false;
        }
        else
        {
            innerbarrier[from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2] = true;
            return true;
        }
    }

    public bool insert_2side_barrier((int, int) from_pos, (int, int) to_pos)
    {
        bool res1 = insert_1side_barrier(from_pos, to_pos);
        bool res2 = insert_1side_barrier(to_pos, from_pos);
        return res1 && res2;
    }


    public (float, float) board_to_world((int, int) board_pos) {
        (float, float) world_pos = (board_pos.Item1 * distance, board_pos.Item2 * distance);
        return world_pos;
    }

    public int GetInnerPos((int, int) pos)
    {
        return innerboard[pos.Item1, pos.Item2];
    }

    public bool HasBarrier((int, int) from_pos, (int, int) to_pos)
    {
        return innerbarrier[from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2];
    }
    
    private void SetGameObjectLayer(GameObject go, string layerName) {
        int layer = LayerMask.NameToLayer(layerName);
        go.layer = layer;
    }

    public void _SetInnerBarrier((int, int) from_pos, (int, int) to_pos, bool value)
    {
        // dangerous function and only for dfs search for route
        // do not use this one in other place
        innerbarrier[from_pos.Item1, from_pos.Item2, to_pos.Item1, to_pos.Item2] = value;
    }

    public bool isInsideBoard((int, int) pos)
    {
        return pos.Item1 >= 0 && pos.Item1 < gridx && pos.Item2 >= 0 && pos.Item2 < gridy;
    }
}


