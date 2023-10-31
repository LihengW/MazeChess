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
    private Text SkillNum;
    private Text EndText;

    private NWButton BarrierButton;
    private NWButton SkillButton;    
    private NWButton CameraButton;
    private NWButton ResetButton;

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
        SkillNum = GameObject.Find("SkillNum").GetComponent<Text>();
        EndText = GameObject.Find("EndText").GetComponent<Text>();
        EndText.text = "";

        BarrierButton = new NWButton(GameObject.Find("BarrierButton"));
        SkillButton = new NWButton(GameObject.Find("SkillButton"));
        CameraButton = new NWButton(GameObject.Find("CameraButton"));
        ResetButton = new NWButton(GameObject.Find("ResetButton"));

    }

    // Update is called once per frame
    void Update()
    {
        Unit preunit = m_Controller.GetPresentUnit();
        RoundDisplay = "Round of " + "<color="+ colorlist[preunit.UnitID] + ">" + preunit.UnitName + "</color>";
        RoundText.text = RoundDisplay;

        BarrierNum.text = "Barrier : " + preunit.barrier_num;
        SkillNum.text = "Skill : " + preunit.skill_num;
    }

    public void OnClickBarrier()
    {
        m_Controller.OnClickBarrier();
        BarrierButton.OnClickedButton();
        if (BarrierButton.Selected)
        {
            SkillButton.InactivateButton();
        }
        else
        {
            SkillButton.ActivateButton();
        }
    }

    public void OnClickSkill()
    {
        m_Controller.OnClickSkill();
        SkillButton.OnClickedButton();
        if (SkillButton.Selected)
        {
            BarrierButton.InactivateButton();
        }
        else
        {
            BarrierButton.ActivateButton();
        }
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

    public void InactivateButtons()
    {
        BarrierButton.InactivateButton();
        SkillButton.InactivateButton();
        CameraButton.InactivateButton();
        ResetButton.InactivateButton();
    }

    public void ActivateButtons()
    {
        BarrierButton.ActivateButton();
        SkillButton.ActivateButton();
        CameraButton.ActivateButton();
        ResetButton.ActivateButton();
    }

    public void ResetButtons()
    {
        BarrierButton.Reset();
        SkillButton.Reset();
        CameraButton.Reset();
        ResetButton.Reset();
    }
}


public class NWButton
{
    public string name;
    private GameObject buttonObj;
    private Button buttonComp;
    private Image imageComp;
    private Text textComp;
    public bool Selected;

    private Color SelectedColor;
    private Color NormalColor;
    private Color SelectedTextColor;
    private Color NormalTextColor;

    public NWButton(GameObject buttonObj)
    {
        buttonComp = buttonObj.GetComponent<Button>();
        imageComp = buttonObj.GetComponent<Image>();
        textComp = buttonObj.transform.Find("Text").GetComponent<Text>();
        name = textComp.text;
        Selected = false;

        NormalColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        SelectedColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        NormalTextColor = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        SelectedTextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        imageComp.color = NormalColor;
        textComp.color = NormalTextColor;
    }

    public void ActivateButton()
    {
        buttonComp.interactable = true;
    }

    public void InactivateButton()
    {
        buttonComp.interactable = false;
    }

    public void OnClickedButton()
    {
        Debug.Log("Select " + name);
        if (Selected)
        {
            imageComp.color = NormalColor;
            textComp.color = NormalTextColor;
            Selected = false;
        }
        else
        {
            imageComp.color = SelectedColor;
            textComp.color = SelectedTextColor;
            Selected = true;
        }
    }

    public void Reset()
    {
        if (Selected)
        {
            imageComp.color = NormalColor;
            textComp.color = NormalTextColor;
            Selected = false;
        }
    }



} 
