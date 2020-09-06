using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.FSM
{
    /// <summary>
    /// 状态对象的基类
    /// </summary>
    public class StateBase : Istate
    {

        public virtual void StateEnter()
        {
        }

        public virtual void StateExit()
        {
        }

        public virtual void StateUpdate()
        {
        }
    }
}