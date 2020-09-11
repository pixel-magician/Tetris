using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.FSM
{
    /// <summary>
    /// 状态机管理
    /// </summary>
    public class StateMachineManagerBase : MonoBehaviour
    {
        public List<StateMachineBase> StateMachines { set; get; } = new List<StateMachineBase>();



        private void Update()
        {
            foreach (var item in StateMachines)
            {
                item.MachineUpdate();
            }
        }
    }
}