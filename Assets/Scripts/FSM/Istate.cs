
namespace PM.FSM
{
    /// <summary>
    /// 状态机的状态接口
    /// 状态对象需要实现该接口
    /// </summary>
    public interface Istate
    {

        /// <summary>
        /// 进入状态时执行
        /// </summary>
        void StateEnter();

        /// <summary>
        /// 处于当前状态时的逻辑更新
        /// </summary>
        void StateUpdate();

        /// <summary>
        /// 离开当前状态时执行
        /// </summary>
        void StateExit();

    }
}
