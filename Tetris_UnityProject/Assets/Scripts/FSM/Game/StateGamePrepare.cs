using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.FSM;
using PM;

/// <summary>
/// 游戏预备状态
/// </summary>
public class StateGamePrepare : StateBase
{
    GlobalManager _globalManager = GlobalManager.Instance;
    public override void StateEnter()
    {
        base.StateEnter();
        _globalManager._chessBoard.Close();
        _globalManager._chessBoard.Release();
        _globalManager._uiManger.NextWindow(WindowId.Menu);
    }
}
