using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.FSM;
using PM;

public class StateGameOver : StateBase
{
    GlobalManager _globalManager = GlobalManager.Instance;

    public override void StateEnter()
    {
        base.StateEnter();
    }
}
