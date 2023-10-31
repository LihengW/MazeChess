using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CameraController : MonoBehaviour
{

    private GameObject m_ShowingCamera;
    private GameObject m_MainCamera;
    private GameObject m_TopCamera;
    private GameObject m_FixedCamera;

    // Initialize the Camera
    public float Board_radius = 8.2f;
    public float camera_distance = 12.0f;
    public Vector2 MainCamera_angles;

    private int Angleid;
    private ArrayList defaultCameraAngles;

    private float sphere_r;

    bool m_Rotating;
    float sum_rotate_angle;
    public float rotate_frq = 0.8f;
    private int round_rotation_degree = 90;

    void Awake()
    {
        defaultCameraAngles = new ArrayList();
        m_Rotating = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = GameObject.Find("MainCamera");
        m_TopCamera = GameObject.Find("TopCamera");
        m_FixedCamera = GameObject.Find("FixedCamera");

        m_ShowingCamera = m_MainCamera;
        m_MainCamera.SetActive(true);
        m_TopCamera.SetActive(false);
        m_FixedCamera.SetActive(false);

        sphere_r = Board_radius * Mathf.Sqrt(2) + camera_distance;
        Angleid = 0;

        if (GameObject.Find("InitData").GetComponent<InitData>().unitnum == 4)
        {
            round_rotation_degree = 90;
        }
        else if (GameObject.Find("InitData").GetComponent<InitData>().unitnum == 2)
        {
            round_rotation_degree = 180;
        }

        MainCamera_angles = (Vector2)defaultCameraAngles[Angleid];
        MainCameraCreate();
    }

    // called per 0.2s
    void Update()
    {
        if (!m_Rotating)
        {
            MainCameraMove();
        }
        else
        {
            CameraRotate();
        }        
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

    public void OnClickResetCamera()
    {
        if (m_ShowingCamera == m_MainCamera)
        {
            m_MainCamera.GetComponent<Transform>().position = m_FixedCamera.GetComponent<Transform>().position;
            m_MainCamera.GetComponent<Transform>().rotation = m_FixedCamera.GetComponent<Transform>().rotation;
        }
    }

    public void CreatePlayerCamera(float angle)
    {
        defaultCameraAngles.Add(new Vector2(angle, 30));
    }

    public bool GetState()
    {
        return m_Rotating;
    }

    public void NextRound()
    {
        // reset camera pos
        m_MainCamera.GetComponent<Transform>().position = m_FixedCamera.GetComponent<Transform>().position;
        m_MainCamera.GetComponent<Transform>().rotation = m_FixedCamera.GetComponent<Transform>().rotation;
        // set status
        m_Rotating = true;
    }

    private void CameraRotate()
    {
        if (sum_rotate_angle < round_rotation_degree)
        {
            m_MainCamera.transform.RotateAround(new Vector3(Board_radius, 0, Board_radius), Vector3.up, rotate_frq);
            m_TopCamera.transform.RotateAround(new Vector3(Board_radius, 0, Board_radius), Vector3.up, rotate_frq);
            sum_rotate_angle += rotate_frq;
        }
        else
        {
            m_FixedCamera.GetComponent<Transform>().position = m_MainCamera.GetComponent<Transform>().position;
            m_FixedCamera.GetComponent<Transform>().rotation = m_MainCamera.GetComponent<Transform>().rotation;

            sum_rotate_angle = 0.0f;
            m_Rotating = false;
        }
    }

    private void MainCameraCreate()
    {
        float y = sphere_r * Mathf.Cos(Mathf.PI * (90.0f - MainCamera_angles.y) / 180.0f);
        float d = sphere_r * Mathf.Sin(Mathf.PI * (90.0f - MainCamera_angles.y) / 180.0f);
        float x = d * Mathf.Cos(Mathf.PI * MainCamera_angles.x / 180.0f) + Board_radius;
        float z = d * Mathf.Sin(Mathf.PI * MainCamera_angles.x / 180.0f) + Board_radius;

        m_MainCamera.GetComponent<Transform>().position = new Vector3(x, y, z);
        m_FixedCamera.GetComponent<Transform>().position = new Vector3(x, y, z);
    }

    private void MainCameraMove()
    {
        if (m_ShowingCamera == m_MainCamera && Input.GetAxis("Fire3") == 1)
        {
            float mouse_x = Input.GetAxis("Mouse X");
            float mouse_y = Input.GetAxis("Mouse Y");

            if (m_MainCamera.transform.position.y < 0.3f && mouse_y < 0.0f) mouse_y = 0.0f;

            MainCameraPosUpdate(2.0f*mouse_x, 2.0f*mouse_y);
        }
    }

    private void MainCameraPosUpdate(float xoffset, float yoffset)
    {

        m_MainCamera.transform.RotateAround(new Vector3(Board_radius, 0, Board_radius), m_FixedCamera.transform.right, yoffset);
        m_MainCamera.transform.RotateAround(new Vector3(Board_radius, 0, Board_radius), m_FixedCamera.transform.up, xoffset);
    }
    
}
