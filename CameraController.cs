using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private GameObject m_ShowingCamera;
    private GameObject m_MainCamera;
    private GameObject m_TopCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = GameObject.Find("MainCamera");
        m_TopCamera = GameObject.Find("TopCamera");
        m_ShowingCamera = m_MainCamera;
        m_MainCamera.SetActive(true);
        m_TopCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBarrier()
    {
        if (m_ShowingCamera == m_MainCamera)
        {
            m_MainCamera.SetActive(false);
            m_TopCamera.SetActive(true);
            m_ShowingCamera = m_TopCamera;
        }
    }

    public void OnClickCamera()
    {
        if (m_ShowingCamera == m_MainCamera)
        {
            m_MainCamera.SetActive(false);
            m_TopCamera.SetActive(true);
            m_ShowingCamera = m_TopCamera;
        }

        else if (m_ShowingCamera == m_TopCamera)
        {
            m_TopCamera.SetActive(false);
            m_MainCamera.SetActive(true);
            m_ShowingCamera = m_MainCamera;
        }

        else
        {
            Debug.Log("Camera Error!");
        }
    }
    
}
