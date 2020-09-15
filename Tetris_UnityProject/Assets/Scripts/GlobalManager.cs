using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.FSM;
using PM;

public class GlobalManager : MonoBehaviour
{

    private static GlobalManager _instance;

    public ChessBoard _chessBoard;
    public GameMachine _gameMachine;
    public UIManagerBase _uiManger;

    public static GlobalManager Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        _uiManger.InitManager("Prefabs/Windows");
        _gameMachine = new GameMachine();

        _uiManger.CreateItem += CreateUI;
    }



    private void Start()
    {
        Init();
    }


    void Init()
    {
        Dictionary<int, Istate> dic = new Dictionary<int, Istate>();
        dic.Add((int)EnumGameState.Prepare, new StateGamePrepare());
        _gameMachine.MachineInit(dic);
    }



    GameObject CreateUI(GameObject template, Transform container)
    {
        GameObject g = Instantiate(template, container);
        return g;
    }
}
