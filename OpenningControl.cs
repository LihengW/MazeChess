using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenningControl : MonoBehaviour
{
    public GameObject OpeningUI;
    public GameObject InitData;

    private InitData m_InitData;

    private Dropdown BarrierDropdown;
    private Dropdown ChessDropdown;
    private Dropdown ModeDropdown;

    // Start is called before the first frame update
    void Start()
    {
        m_InitData = InitData.GetComponent<InitData>();
        BarrierDropdown = GameObject.Find("BarrierDrop").GetComponent<Dropdown>();
        ChessDropdown = GameObject.Find("ChessDrop").GetComponent<Dropdown>();
        ModeDropdown = GameObject.Find("ModeDrop").GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void ToMaingame()
    {
        Debug.Log(BarrierDropdown.value);
        m_InitData.barriernum = BarrierDropdown.value + 5;
        m_InitData.playernum = ChessDropdown.value + 1;
        m_InitData.mode = ModeDropdown.value;
        SceneManager.LoadScene("MainGame");
    }

    public void OnClickPlayer()
    {
        OpeningUI.GetComponent<Animator>().SetTrigger("ToSecond");
    }
    
}
