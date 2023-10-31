using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit
{
    public string UnitName;
    public int UnitID;
    private List<Player> players;
    private List<Player> m_KillList;
    private List<Player> m_BlockedList;
    public bool in_round;
    public int barrier_num;
    public int skill_num;

    public Unit(string name, int id, int barriernum)
    {
        UnitName = name;
        UnitID = id;
        in_round = false;
        barrier_num = barriernum;
        skill_num = 2;

        // Player Lists
        players = new List<Player>();
        m_KillList = new List<Player>();
        m_BlockedList = new List<Player>();
    }

    public void AddPlayerObject(GameObject gb)
    {
        players.Add(gb.GetComponent<Player>());
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public bool IsContainPlayer(int player_id)
    {
        foreach (Player player in players)
        {
            if (player.GetID() == player_id)
            {
                return true;
            }
        }
        return false;
    }

    public List<Player> GetPlayerList()
    {
        return players;
    }

    private Player GetPlayer(int id)
    {
        // inside ID
        return players[id];
    }

    public int GetPlayerNum()
    {
        return players.Count;
    }

    // Game Control Func
    public void InitPlayerRoute()
    {
        foreach (Player player in players)
            {
                player.RouteInit();
            }
    }

    public bool ReachTarget()
    {
        bool res = false;
        foreach (Player player in players)
        {
            res = res || player.ReachTarget();
        }
        return res;
    }

    // Killing function
    public void KillPlayer(Player player)
    {
        m_KillList.Add(player);
    }
    public void KillCheck((int, int) pos)
    {
        foreach (Player player in players)
        {
            player.KillCheck(pos);
        }
        foreach (Player player in m_BlockedList)
        {
            player.KillCheck(pos);
        }
    }

    public bool FinishDisappear()
    {
        if (m_KillList.Count > 0)
        {
            foreach (Player player in m_KillList)
            {
                if (player.FinishDisappearAnimation())
                {
                    m_KillList.Remove(player);
                }
            }
        }

        return m_KillList.Count == 0;
    }

    // Block functions
    public void BlockCheck()
    {
        foreach (Player player in m_BlockedList)
        {
            bool res = player.RelocateRouteSearch();
            if (res)
            {
                ActivatePlayer(player);
            }
        }
    }

    public void BlockPlayer(Player player)
    {
        players.Remove(player);
        m_BlockedList.Add(player);
    }

    public void ActivatePlayer(Player player)
    {
        m_BlockedList.Remove(player);
        player.SetActive();
        players.Add(player);
    }

    public int BlockPlayerCount()
    {
        return m_BlockedList.Count;
    }

    // Unit Round Control
    static public int StartRound(Unit unit)
    {
        unit.in_round = true;
        int playernum = unit.GetPlayerNum();
        if (playernum > 0)
        {
            for (int i = 0; i < unit.GetPlayerNum(); i++)
            {
                unit.GetPlayer(i).in_round = true;
            }
            return 1;  // normal mode 
        }
        else
        {
            if (unit.BlockPlayerCount() > 0)
            {
                return 2; // skill limited mode
            }
            else
            {
                return 0; // skip player (no left)
            }
        }
    }

    static public void EndRound(Unit unit)
    {
        unit.in_round = false;
        for (int i = 0; i < unit.GetPlayerNum(); i++)
        {
            unit.GetPlayer(i).in_round = false;
        }
    }

}

public class Controller : MonoBehaviour
{
    // Controller is to control the process of game
    public GameObject Game_Board;
    public GameObject CameraController;
    GameBoard gameboard;
    CameraController cameracontroller;
    ControllerUI controllerUI;

    // set game environment
    int player_num = 4;
    int barrier_num = 5;
    int unit_num = 4;

    Unit[] unitlist;

    // turn start -> choose a player -> choose which place to move -> do the move -> turn end
    StateManager statemanager;
    ActionHandler actionHandler;

    // state info para
    public int present_unit;

    // common data structure
    Color[] m_ColorList;

    // Start is called before the first frame update
    void Awake()
    {
        // start from the openning
        player_num = GameObject.Find("InitData").GetComponent<InitData>().playernum;
        barrier_num = GameObject.Find("InitData").GetComponent<InitData>().barriernum;
        unit_num = GameObject.Find("InitData").GetComponent<InitData>().unitnum;

        // public data creation
        m_ColorList = ColorListCreate();

        // do the initailization with the gameboard at the same time
        gameboard = Game_Board.GetComponent<GameBoard>();
        cameracontroller = CameraController.GetComponent<CameraController>();
        controllerUI = GameObject.Find("ControllerUI").GetComponent<ControllerUI>();
        actionHandler = GameObject.Find("ActionHandler").GetComponent<ActionHandler>();

        statemanager = new StateManager(actionHandler);
        statemanager.controller = this;

        (int, int)[] init_pos = new (int, int)[16]{((gameboard.gridx - 1)/2 , 0), ((gameboard.gridx - 1)/2 + 1 , 0), ((gameboard.gridx - 1)/2 - 1 , 0), ((gameboard.gridx - 1)/2 + 2 , 0),
                                                   (0 , (gameboard.gridy - 1)/2), (0 , (gameboard.gridy - 1)/2 + 1), (0 , (gameboard.gridy - 1)/2 - 1), (0 , (gameboard.gridy - 1)/2 + 2),
                                                   ((gameboard.gridx - 1)/2 , gameboard.gridy - 1), ((gameboard.gridx - 1)/2 + 1 , gameboard.gridy - 1), ((gameboard.gridx - 1)/2 - 1 , gameboard.gridy - 1), ((gameboard.gridx - 1)/2 + 2 , gameboard.gridy - 1),
                                                   (gameboard.gridx - 1 , (gameboard.gridy - 1)/2), (gameboard.gridx - 1 , (gameboard.gridy - 1)/2 + 1), (gameboard.gridx - 1 , (gameboard.gridy - 1)/2 - 1), (gameboard.gridx - 1 , (gameboard.gridy - 1)/2 + 2)};

        // first para: 0 is row, 1 is col; second para: num;
        (int, int)[] init_tar = new (int, int)[4]{(0, gameboard.gridy - 1), (1, gameboard.gridx - 1), (0, 0), (1, 0)};

        int[] init_camera_angle = new int[4]{ -90, 180, 90, 0 };

        // Initialization of every player;

        unitlist = new Unit[unit_num];
        Player[] playerlist = new Player[player_num * unit_num];

        int k = 0;
        if (unit_num == 4)
        {
            k = 1;
        }
        else if (unit_num == 2)
        {
            k = 2;
        }

        for (int u = 0; u < unit_num; u++)
        {
            // Unit Create
            Unit newunit = new Unit(u.ToString(), u, barrier_num);
            unitlist[u] = newunit;
            cameracontroller.CreatePlayerCamera(init_camera_angle[k*u]);
            (int, int)[] dir = DirectionCreate(u); 

            for (int i = 0; i < player_num; i++) 
            {
                int id = (u * player_num) + i;
                GameObject playerob = gameboard.create_player(init_pos[4*k*u + i], id);
                playerlist[id] = playerob.GetComponent<Player>();
                playerlist[id].SetID(id);
                playerlist[id].SetColor(m_ColorList[u]);
                playerlist[id].SetBlockedColor(m_ColorList[u+4]);
                playerlist[id].playername = id.ToString();
                playerlist[id].SetDirection(dir);

                playerlist[id].SetUnit(newunit);
                newunit.AddPlayer(playerlist[id]);


                // set target for player
                if (init_tar[k*u].Item1 == 0) // row target
                {
                    for (int j = 0; j < gameboard.gridx; j++){
                        playerlist[id].AddTarget((j, init_tar[k*u].Item2));
                    }
                }
                else                       // col target
                {
                    for (int j = 0; j < gameboard.gridy; j++){
                        playerlist[id].AddTarget((init_tar[k*u].Item2, j));
                    }
                }
            }
        }

        Unit.StartRound(unitlist[0]);
        present_unit = 0;
        // actionHandler.SetCurPlayer(playerlist[0]);
    }

    void Start()
    {
        for (int id = 0; id < unitlist.Length; id++)
        {
            unitlist[id].InitPlayerRoute();
        }
    }

    // Update is called once per frame
    void Update()
    {
        statemanager.Update();
    }

//  Barrier Setting Checker
    public bool BarrierCheck((int, int) from_pos, (int, int) to_pos)
    {
        if (gameboard.HasBarrier(from_pos, to_pos)) return false;
        foreach (Unit unit in unitlist){
            foreach (Player player in unit.GetPlayerList())
            {
                // Only check the player in the playerlist (Not in the block list)
                bool blocked = player.BarrierCheck(from_pos, to_pos);
                if (blocked)
                {
                    Debug.Log(player.playername.ToString() + " : Start Searching.......");
                    bool suc = player.reRouteSearch(from_pos, to_pos, false);
                    if (!suc)
                    {

                        Debug.Log(player.playername.ToString() + ": Searching Failed!");
                        return false;
                    }
                }
                Debug.Log(player.playername.ToString() + " : Barrier Check successfully!");
            }
        }
        Debug.Log("All Barrier Check successfully!");

        return true;
    }

    public bool DoubleBarrierCheck((int, int) from_pos, (int, int) to_pos)
    {
        if (gameboard.HasBarrier(from_pos, to_pos) || gameboard.HasBarrier(to_pos, from_pos)) return false;
        foreach (Unit unit in unitlist){
            foreach (Player player in unit.GetPlayerList())
            {
                // Only check the player in the playerlist (Not in the block list)
                bool blocked = player.DoubleBarrierCheck(from_pos, to_pos);
                if (blocked)
                {
                    Debug.Log(player.playername.ToString() + " : Start Searching.......");
                    bool suc = player.reRouteSearch(from_pos, to_pos, true);
                    if (!suc)
                    {
                        Debug.Log(player.playername.ToString() + ": Searching Failed!");
                        return false;
                    }
                }
                Debug.Log(player.playername.ToString() + " : Barrier Check successfully!");
            }
        }
        Debug.Log("All Barrier Check successfully!");

        return true;
    }


//  Round Control Function 

    public int StartRound()  // return info code
    {
        present_unit++;
        if (present_unit == unit_num)
        {
            present_unit = 0;
        }
        controllerUI.ActivateButtons();
        
        return Unit.StartRound(unitlist[present_unit]);
    }

    public void EndRound()
    {
        if (GetPresentUnit().ReachTarget())
        {
            controllerUI.EndGame();
            //EndGame();
        }

        controllerUI.ResetButtons();
        controllerUI.InactivateButtons();
        Unit.EndRound(unitlist[present_unit]);
    }

    void EndGame()
    {
        SceneManager.LoadScene("Openning");
    }


//  Functional Function & Interfaces
    public Unit GetPresentUnit()
    {
        return unitlist[present_unit];
    }

    public void ActivateCameraRotation()
    {
        cameracontroller.NextRound();
    }

    public bool IsCameraRotating()
    {
        return cameracontroller.GetState();
    }

    public void KillCheck((int, int) pos)
    {
        foreach (Unit unit in unitlist)
        {
            unit.KillCheck(pos);
        }
    }

    public void BlockCheck()
    {
        foreach (Unit unit in unitlist)
        {
            unit.BlockCheck();
        }
    }

    public bool FinishDisappear()
    {
        bool res = true;
        foreach (Unit unit in unitlist)
        {
            res = res & unit.FinishDisappear();
        }
        return res;
    }

// UI Event Function
    public void OnClickBarrier()
    {
        if (GetPresentUnit().barrier_num > 0)
        {
            statemanager.GetModeState = statemanager.GetModeState == 0 ? 1 : 0;
        }
        else
        {
            Debug.Log("No left barriers");
        }
        statemanager.ClearSelected();
    }

    public void OnClickSkill()
    {
        if (GetPresentUnit().skill_num > 0)
        {
            statemanager.GetModeState = statemanager.GetModeState == 0 ? 2 : 0;
        }
        else
        {
            Debug.Log("No left skills");
        }
        statemanager.ClearSelected();
    }

    public void OnClickCamera()
    {
        statemanager.ClearSelected();
    }

// Tool function
    private Color[] ColorListCreate()
    {
        string[] strcolors = new string[8];
        strcolors[0] = "#00ced15f"; // DarkTurquoise
        strcolors[1] = "#7fffaa5f";
        strcolors[2] = "#ffff005f";
        strcolors[3] = "#ffc0cb5f";

        strcolors[4] = "#4169e1af"; // Blue
        strcolors[5] = "#66cd00af"; // Chartreuse3
        strcolors[6] = "#eead0eaf"; // DarkGoldenrod2
        strcolors[7] = "#d02090af"; // VioletRed
        
        Color[] colors = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            Color color;
            ColorUtility.TryParseHtmlString(strcolors[i], out color);
            colors[i] = color;
        }
        return colors;
    }

    private (int, int)[]  DirectionCreate(int unitid)
    {
        switch (unitid)
        {
            case 0:
                return new (int, int)[4]{(0, 1), (-1, 0), (1, 0), (0, -1)};
            case 1:
                return new (int, int)[4]{(1, 0), (0, 1), (0, -1), (-1, 0)};
            case 2:
                return new (int, int)[4]{(0, -1), (1, 0), (-1, 0), (0, 1)};
            case 3:
                return new (int, int)[4]{(-1, 0), (0, -1), (0, 1), (1, 0)};
            default:
                Debug.Log("UnitID not found");
                return new (int, int)[4]{(-1, 0), (0, -1), (0, 1), (1, 0)};
        };
    }
}


public class MouseSelect
{
private
    Ray ray;
    RaycastHit hit;

public
    GameObject HitObj;

    public GameObject GetObject()
    {
        if (Input.GetMouseButtonDown(0)) {
            ray = Camera.allCameras[0].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                HitObj = hit.collider.gameObject;
                if (HitObj.layer == 5)
                {
                    Debug.Log("Clicked UI");
                    return null;
                }
                if (HitObj.layer == 7)
                {
                    // player
                    Player player = HitObj.GetComponent<Player>();
                    if (player.GetStatus() == 1) // Active Player
                    {
                        Debug.Log("Clicked Player");
                        if (!player.selected)
                        {
                            player.Selected();
                        }
                        else
                        {
                            player.Deselected();
                        }
                    }
                    else if (player.GetStatus() == 2) // Blocked Player
                    {
                        Debug.Log("Clicked Blocked Player");
                        return null;
                    }
                }
                else if (HitObj.layer == 8)
                {
                    // board
                    Debug.Log("Clicked Board");
                    if (!HitObj.GetComponent<BoardPiece>().selected)
                    {
                        HitObj.GetComponent<BoardPiece>().Selected();
                    }
                    else
                    {
                        HitObj.GetComponent<BoardPiece>().Deselected();
                    }
                }
                else if (HitObj.layer == 9)
                {
                    Debug.Log("Clicked Barrier");
                }
                return HitObj;
            }  
        }

        return null;      
    }
}

public class StateManager
{
    // State 0 means in player operation
    // State 1 means in implementing action
    // State 2 means in Round Intervals (do Camera Rotation & Round End Check)
    private int State = 0;

    // ModeState 0 means in moving player
    // ModeState 1 means in setting barriers
    // ModeState 2 means in destroying barriers

    private int ModeState = 0;

    // EndState 0 means Checking the End Part of previous Round
    // EndState 1 means doing Kill Check & Animate the KILL
    // EndState 2 means doing the Camera Rotating
    // EndState 3 means Starting the New Round
    private int EndState = 0;

    private bool Special_Skill = false;

    public Controller controller;

    List<GameObject> SelectedList;

    MouseSelect mouseselect;
    ActionHandler handler;
    public Action cur_action;

    // special State var
    private bool CameraActivated;
    private (int, int) MovedPosition;

    public StateManager(ActionHandler o_handler)
    {
        mouseselect = new MouseSelect();
        SelectedList = new List<GameObject>();
        cur_action = null;
        handler = o_handler;
        
        CameraActivated = false;
        MovedPosition = (-1, -1);
    }

    public void Update()
    {
        // Player Operation Time
        if (State == 0)
        {
            // receive Click Action when Player can move
            GameObject clicked = mouseselect.GetObject();
            if (clicked != null)
            {
                SelectedList.Add(clicked);
            }

            if (SelectedList.Count == 1)
            {
                if (ModeState == 2 && SelectedList[0].layer == 9)
                {
                    
                    DestroyBarrier new_action = new DestroyBarrier(SelectedList[0], Special_Skill);
                    bool res = handler.SetAction(new_action);
                    if (res)
                    {
                        State = 1;
                        cur_action = new_action;
                    }
                    ClearSelected();
                }
            }
            else if (SelectedList.Count == 2)
            {
                if (SelectedList[0].layer == 7 && SelectedList[1].layer == 8)
                {
                    // reasonable action
                    if (ModeState == 0)
                    {
                        Move new_action = new Move(SelectedList[0], SelectedList[1]);
                        bool res = handler.SetAction(new_action);
                        if (res) 
                        { 
                            State = 1;
                            cur_action = new_action;
                            MovedPosition = SelectedList[1].GetComponent<BoardPiece>().GetInnerPos;
                        }
                    }
                    else if (ModeState == 1)
                    {
                        // Set Barrier Code;
                    }
                }
                else if (SelectedList[0].layer == 8 && SelectedList[1].layer == 8)
                {
                    // reasonable action
                    if (ModeState == 1)
                    {
                        Debug.Log("Creating SetBarrier Action!");
                        SetBarrier new_action = new SetBarrier(SelectedList[0], SelectedList[1]);
                        bool res = handler.SetAction(new_action);
                        if (res) 
                        { 
                            State = 1;
                            cur_action = new_action;
                        }
                    }
                }
                else if (SelectedList[0].layer == 8 && SelectedList[1].layer == 7)
                {
                    if (ModeState == 1)
                    {
                        // Set Barrier Code;
                    }
                }
                else
                {
                    // pass
                }

                ClearSelected();
            }
            else if (SelectedList.Count >= 2)
            {
                // Invalid Selection
                ClearSelected();
            }
        }
        // System Operation Time
        else if (State == 1)
        {
            if (handler.busy)
            {
                //handler.Update();
            }

            else
            {
                cur_action.handled = true;
                State = 2;
            }
        } 
        // Round Check/Pushing Time
        else if (State == 2)
        {
            if (EndState == 0)
            {
                Debug.Log("Pushing to the Next Round...");
                if (Special_Skill) Special_Skill = false;
                controller.EndRound();
                EndState++;
            }
            else if (EndState == 1)
            {
                Debug.Log("Doing the Kill & Blocking Check...");
                if (MovedPosition != (-1, -1))
                {
                    controller.BlockCheck();
                    controller.KillCheck(MovedPosition);
                }

                MovedPosition = (-1, -1);
                
                if (controller.FinishDisappear())
                {
                    EndState++;
                }
            }
            else if (EndState == 2)
            {
                if (!CameraActivated)
                {
                    Debug.Log("Rotating the Camera...");
                    controller.ActivateCameraRotation();
                    CameraActivated = true;
                }
                else
                {
                    if (!controller.IsCameraRotating()) // the Camera is not moving
                    {
                        Debug.Log("Camera Rotated");
                        CameraActivated = false;
                        EndState++;
                    }
                }
            }
            else if (EndState == 3)
            {
                Debug.Log("New Round Starting !");
                int res = controller.StartRound();
                // End State Reset
                EndState = 0;
                // Action State Reset
                if (res == 1)
                {
                    State = 0;
                    ModeState = 0;
                }
                else if (res == 0)
                {
                    // Directly End the Round
                    State = 2;
                    ModeState = 0;
                }
                else if (res == 2)
                {
                    // special skill mode
                    State = 0;
                    ModeState = 0;
                    Special_Skill = true;
                }
            }
        }
    }
    public int GetState
    {
        get { return State; }
        set {State = value; }
    }

    public int GetModeState
    {
        get { return ModeState; }
        set {ModeState = value; }
    }

    public void ClearSelected()
    {
        for (int i = 0; i < SelectedList.Count; i++)
        {
            if (SelectedList[i].layer == 7)
            {
                SelectedList[i].GetComponent<Player>().Deselected();
            }
            else if (SelectedList[i].layer == 8)
            {
                SelectedList[i].GetComponent<BoardPiece>().Deselected();
            }
        }
        SelectedList.Clear();
    }

}

public class Action
{ 
    public GameObject m_FirstObject;
    public GameObject m_SecondObject;

    public bool handled = false;

    public Action()
    {
        ;
    }

    public Action(GameObject player)
    {
        m_FirstObject = player;
        m_SecondObject = null;
    }

    public virtual void init()
    {
        ;
    }

    public virtual bool implement()
    {
        return true;
    }
}

public class Move : Action
{
public
    Vector3 m_Offset;
    
    public Move()
    {
        ;
    }

    public Move(GameObject player, GameObject target)
    {
        m_FirstObject = player;
        m_SecondObject = target;
    }

    public override bool implement() // return true if the implement is finished
    {
        m_FirstObject.transform.position += m_Offset * Time.deltaTime * 2.0f;
        Vector2 dis = new Vector2(m_SecondObject.transform.position.x - m_FirstObject.transform.position.x, m_SecondObject.transform.position.z - m_FirstObject.transform.position.z);
        if (dis.magnitude < 0.1f)
        {
            m_FirstObject.transform.position = new Vector3(m_SecondObject.transform.position.x, m_FirstObject.transform.position.y, m_SecondObject.transform.position.z);
            return true;
        }
        else
        {
            return false;
        }
    }

}

public class SetBarrier : Action
{

    public GameObject BarrierSample;

    public GameObject NewBarrier;

    public float m_Height;
    
    public SetBarrier()
    {
        m_Height = 1.0f;
    }
    public SetBarrier(GameObject block1, GameObject block2)
    {
        m_Height = 1.0f;
        m_FirstObject = block1;
        m_SecondObject = block2;
    }

    public override bool implement() // return true if the implement is finished
    {
        if (m_Height < 0.0f)
        {
            return true;
        }
        else
        {
            m_Height = m_Height - 0.01f;
            NewBarrier.transform.position = NewBarrier.transform.position - Vector3.up * 0.01f;
            return false;
        }
    }
}

public class DestroyBarrier : Action
{   
    public bool use_skillnum;
    public DestroyBarrier()
    {
        ;
    }
    public DestroyBarrier(GameObject barrier, bool Special_Skill)
    {
        m_FirstObject = barrier;
        m_SecondObject = null;
        use_skillnum = !Special_Skill;
    }

    public override bool implement() // return true if the implement is finished
    {
        if (m_FirstObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BarrierVanished"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

