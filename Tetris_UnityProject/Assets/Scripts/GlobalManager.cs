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
        _uiManger.DestoryItem += DestoryUI;
    }



    private void Start()
    {
        Init();
    }


    void Init()
    {
        Dictionary<EnumGameState, Istate> dic = new Dictionary<EnumGameState, Istate>();
        dic.Add(EnumGameState.Prepare, new StateGamePrepare());
        dic.Add(EnumGameState.Run, new StateGameRun());
        dic.Add(EnumGameState.Pause, new StateGamePause());
        dic.Add(EnumGameState.GameOver, new StateGameOver());
        _gameMachine.MachineInit(dic);
    }


    /// <summary>
    /// 实例化UI对象
    /// </summary>
    /// <param name="template"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    GameObject CreateUI(GameObject template, Transform container)
    {
        GameObject g = Instantiate(template, container);
        return g;
    }


    void DestoryUI(GameObject ui)
    {
        Destroy(ui);
    }
}
