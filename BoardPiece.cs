using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    private (int, int) inner_pos;
    public GameBoard gameboard;
    private Animator m_SurfAnimator;

    // Selection Animation
    private GameObject SelectedSurf;
    public bool selected;


    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        SelectedSurf = transform.Find("SelectedSurface").gameObject;
        m_SurfAnimator = SelectedSurf.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInnerPos((int, int) pos)
    {
        inner_pos = pos;
    }

    public (int, int) GetInnerPos
    {
        get { return inner_pos; }
        set {inner_pos = value; }
    }

    public int GetInnerPosX()
    {
        return inner_pos.Item1;
    }

    public int GetInnerPosY()
    {
        return inner_pos.Item2;
    }

    public void Selected()
    {
        m_SurfAnimator.SetTrigger("Expand");

        if (m_SurfAnimator.GetCurrentAnimatorStateInfo(0).IsName("SelectedSurfaceOff"))
        {
            m_SurfAnimator.ResetTrigger("Shrink");
        }

        selected = true;
    }

    public void Deselected()
    {
        m_SurfAnimator.SetTrigger("Shrink");
        
        if (m_SurfAnimator.GetCurrentAnimatorStateInfo(0).IsName("SelectedSurfaceOn"))
        {
            m_SurfAnimator.ResetTrigger("Expand");
        }

        selected = false;
    }

    public void ChangeColor(Color color)
    {
        gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", color);
    }

    public void ResetColor()
    {
        gameObject.GetComponent<Renderer>().materials[0].SetColor("_Color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
    }
}
