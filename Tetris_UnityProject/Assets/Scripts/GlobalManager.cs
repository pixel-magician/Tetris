using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.FSM;
using PM;
using System.IO;
using LitJson;

public class GlobalManager : MonoBehaviour
{

    private static GlobalManager _instance;

    public ChessBoard _chessBoard;
    public GameMachine _gameMachine;
    public UIManagerBase _uiManger;

    public GameSaveData _gameSaveData;

    public static GlobalManager Instance { get => _instance; }




    [SerializeField]
    string _savePath;

    private void Awake()
    {
        _instance = this;
        
    }



    private void Start()
    {
        Init();
    }



    public void SaveData()
    {
        WriteDataFile(_savePath, _gameSaveData);
    }



    void Init()
    {
        _uiManger.InitManager("Prefabs/Windows");
        _gameMachine = new GameMachine();

        _uiManger.CreateItem += CreateUI;
        _uiManger.DestoryItem += DestoryUI;

        Dictionary<EnumGameState, Istate> dic = new Dictionary<EnumGameState, Istate>();
        dic.Add(EnumGameState.Prepare, new StateGamePrepare());
        dic.Add(EnumGameState.Run, new StateGameRun());
        dic.Add(EnumGameState.Pause, new StateGamePause());
        dic.Add(EnumGameState.GameOver, new StateGameOver());
        _gameMachine.MachineInit(dic);
        _gameSaveData = ReadDataFile<GameSaveData>(_savePath);
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


    T ReadDataFile<T>(string path)
    {
        string json = File.ReadAllText(path);
        T t = JsonMapper.ToObject<T>(json);
        return t;
    }

    void WriteDataFile(string path, object o)
    {
        string json = JsonMapper.ToJson(o);
        File.WriteAllText(path, json);
    }
}
