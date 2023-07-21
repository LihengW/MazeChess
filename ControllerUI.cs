using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour
{
    
    private Controller m_Controller;

    public GameObject UICanvas;

    private Text RoundText; 

    string RoundDisplay;

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = this.transform.parent.gameObject.GetComponent<Controller>();
        RoundText = GameObject.Find("RoundText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Player preplayer = m_Controller.GetPresentPlayer();
        RoundDisplay = "Round of Player " + preplayer.playername;
        RoundText.text = RoundDisplay;
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
}
