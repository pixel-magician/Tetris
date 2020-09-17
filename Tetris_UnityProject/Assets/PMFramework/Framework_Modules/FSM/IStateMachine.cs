using System.Collections.Generic;
using System.Globalization;

namespace PM.FSM
{
    /// <summary>
    /// 状态机接口
    /// 所有状态机对象需要实现该接口
    /// </summary>
    /// <typeparam name="TEnum">当前状态机所包含所有状态的枚举类型</typeparam>
    public interface IStateMachine<T>
    {
        /// <summary>
        /// 该状态机当前所处的状态对象
        /// </summary>
        Istate CurrentState { set; get; }


        /// <summary>
        /// 初始化状态机
        /// </summary>
        /// <param name="states"></param>
        void MachineInit(Dictionary<T, Istate> states);

        /// <summary>
        /// 状态机处于某一状态时的逻辑更新
        /// </summary>
        void MachineUpdate();

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateIndex">状态的索引值</param>
        void ChangeState(T stateIndex);

    }
}