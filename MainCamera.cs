using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    private Camera m_MainCamera;
    private Transform m_Transform;
    
    // state 0 means in Init position; state 1 in Setting position;
    private int m_CameraState;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = this.gameObject.GetComponent<Camera>();
        m_Transform = this.gameObject.GetComponent<Transform>();
        m_CameraState = 0;
        m_Transform.position = MainCameraTransform.InitPos;
        m_Transform.Rotate(MainCameraTransform.InitRot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBarrier()
    {
        if (m_CameraState == 0)
        {
            m_Transform.position = MainCameraTransform.SettingPos;
            m_Transform.Rotate(-1.0f * MainCameraTransform.InitRot);
            m_Transform.Rotate(MainCameraTransform.SettingRot);
            m_CameraState = 1;           
        }
        else if (m_CameraState == 1)
        {
            m_Transform.position = MainCameraTransform.InitPos;
            m_Transform.Rotate(-1.0f * MainCameraTransform.SettingRot);
            m_Transform.Rotate(MainCameraTransform.InitRot);
            m_CameraState = 0;       
        }
    }
}


public struct MainCameraTransform
{
    public static Vector3 InitPos = new Vector3(7.37f, 5.1f, -11.68f);
    public static Vector3 InitRot = new Vector3(7.38f, 4.9f, 0.0f);
    public static Vector3 SettingPos = new Vector3(9.0f, 12.0f, -4.0f);
    public static Vector3 SettingRot = new Vector3(50.0f, 0.0f, 0.0f);
}
