using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    /// <summary>
    /// Socket协议
    /// </summary>
    public enum SocketType
    {
        /// <summary>
        /// 使用Tcp协议
        /// </summary>
        Tcp,
        /// <summary>
        /// 使用Udp协议
        /// 最大数据包长度65000字节
        /// </summary>
        Udp,
        /// <summary>
        /// 所用类型协议都启用
        /// </summary>
        All,

    }
}