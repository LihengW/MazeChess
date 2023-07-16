using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerUI : MonoBehaviour
{
    
    private Controller m_Controller;

    string RoundDisplay;

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = this.transform.parent.gameObject.GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        Player preplayer = m_Controller.GetPresentPlayer();
        RoundDisplay = "Round of Player " + preplayer.playername;
        Debug.Log(RoundDisplay);  // need to become a UI object
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
