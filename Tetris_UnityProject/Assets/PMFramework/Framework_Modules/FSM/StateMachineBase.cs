using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PM.FSM
{
    /// <summary>
    /// 状态机对象的基类
    /// </summary>
    public class StateMachineBase : IStateMachine
    {
        Istate _currentState;
        public virtual Istate CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState?.StateExit();
                _currentState = value;
                _currentState.StateEnter();
            }
        }

        Dictionary<int, Istate> _states = new Dictionary<int, Istate>();

        public virtual void ChangeState(int stateIndex)
        {
            CurrentState = _states[stateIndex];
        }

        public virtual void MachineInit(Dictionary<int, Istate> states)
        {
            _states = states;
            ChangeState(0);
        }

        public virtual void MachineUpdate()
        {
            CurrentState.StateUpdate();
        }
    }
}



