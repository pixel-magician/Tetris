using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.FSM;
using PM;

public class StateGameRun : StateBase
{
    GlobalManager _globalManager = GlobalManager.Instance;

    public override void StateEnter()
    {
        base.StateEnter();
        _globalManager._chessBoard.Open();
        _globalManager._uiManger.NextWindow(WindowId.Score);
    }
}
