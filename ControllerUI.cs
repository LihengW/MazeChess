using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour
{
    
    private Controller m_Controller;

    public GameObject UICanvas;

    private Text RoundText; 
    private Text BarrierNum;
    private Text EndText;

    private string[] colorlist;

    string RoundDisplay;

    void Awake()
    {
        colorlist = new string[4];
        colorlist[0] = "#00ced1ff"; // cyan
        colorlist[1] = "#7fffaaff"; // green
        colorlist[2] = "#ffff00ff"; // yellow
        colorlist[3] = "#ffc0cbff"; // pink
    }


    // Start is called before the first frame update
    void Start()
    {
        m_Controller = this.transform.parent.gameObject.GetComponent<Controller>();
        RoundText = GameObject.Find("RoundText").GetComponent<Text>();
        BarrierNum = GameObject.Find("BarrierNum").GetComponent<Text>();
        EndText = GameObject.Find("EndText").GetComponent<Text>();
        EndText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        Unit preunit = m_Controller.GetPresentUnit();
        RoundDisplay = "Round of " + "<color="+ colorlist[preunit.UnitID] + ">" + preunit.UnitName + "</color>";
        RoundText.text = RoundDisplay;

        BarrierNum.text = "Barrier : " + preunit.barrier_num;
    }

    public void OnClickBarrier()
    {
        m_Controller.OnClickBarrier();
    }

    public void OnClickSkill()
    {
        Debug.Log("Click Skill");
    }

    public void OnClickCamera()
    {
        m_Controller.OnClickCamera();
    }

    public void EndGame()
    {
        Unit preunit = m_Controller.GetPresentUnit();
        EndText.text = "<color="+ colorlist[preunit.UnitID] + ">" + preunit.UnitName + "</color> WINS!";
    }
}
