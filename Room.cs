using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject gameboard;

    private GameBoard m_GameBoard;

    // center offset
    private Vector3 m_posoffset = new Vector3(10, 0.5f,8);

    // Start is called before the first frame update
    void Start()
    {
        m_GameBoard = gameboard.GetComponent<GameBoard>();
        transform.position = new Vector3(m_GameBoard.gridx * m_GameBoard.distance / 2, 0, m_GameBoard.gridy * m_GameBoard.distance / 2) - m_posoffset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
