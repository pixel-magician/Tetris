using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PM.FSM
{

    /// <summary>
    /// 状态机对象的基类
    /// 泛型类
    /// </summary>
    public class StateMachineBase<T> : IStateMachine<T>
    {
        Istate _currentState;
        T _currentStateIndex;
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

        Dictionary<T, Istate> _states = new Dictionary<T, Istate>();

        public virtual void ChangeState(T stateIndex)
        {
            CurrentState = _states[stateIndex];
            _currentStateIndex = stateIndex;
        }

        public virtual void MachineInit(Dictionary<T, Istate> states)
        {
            _states = states;
            ChangeState(default(T));
        }

        public virtual void MachineUpdate()
        {
            CurrentState.StateUpdate();
        }

    }
}



