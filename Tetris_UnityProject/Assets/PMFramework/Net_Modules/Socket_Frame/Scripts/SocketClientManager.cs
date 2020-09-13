#if !UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM.SocketManage;

namespace PM
{
    public class SocketClientManager : MonoBehaviour
    {
        public delegate void ReceivePacket(SocketPacket packet);
        public event ReceivePacket Evenet_ReceivePacket;
        public event ReceivePacket Evenet_ReceiveTcp;
        public event ReceivePacket Evenet_ReceiveUdp;

        Dictionary<int, ReceivePacket> _msgIdEvent = new Dictionary<int, ReceivePacket>();



        //static SocketClientManager _instance = null;
        /// <summary>
        /// 服务器的IP
        /// </summary>
        [SerializeField]
        string _serverIP = "127.0.0.1";
        /// <summary>
        /// 服务器端口号
        /// </summary>
        [SerializeField]
        int _serverPort;

        TcpClient _tcpClient = new TcpClient();
        UdpClient _udpClient = new UdpClient();

        Queue<SocketPacket> _queueUdp = new Queue<SocketPacket>();
        Queue<SocketPacket> _queueTcp = new Queue<SocketPacket>();



        private void Awake()
        {
            //_instance = this;
            _udpClient.EventReceivePacket += (packet) => { _queueUdp.Enqueue(packet); };
            _tcpClient.EventReceivePacket += (packet) => { _queueTcp.Enqueue(packet); };
            _udpClient = new UdpClient();
            _tcpClient = new TcpClient();
        }

        private void Update()
        {
            if (_queueUdp.Count > 0)
            {
                SocketPacket packet = _queueUdp.Dequeue();
                Evenet_ReceivePacket?.Invoke(packet);
                Evenet_ReceiveUdp?.Invoke(packet);
                DoMsgIdEvent(packet);
            }
            if (_queueTcp.Count > 0)
            {
                SocketPacket packet = _queueUdp.Dequeue();
                Evenet_ReceivePacket?.Invoke(packet);
                Evenet_ReceiveTcp?.Invoke(packet);
                DoMsgIdEvent(packet);
            }
        }


        //public static SocketClientManager GetInstance()
        //{
        //    if (_instance == null)
        //    {
        //        GameObject prefab = Resources.Load<GameObject>("SocketClient");
        //        GameObject g = Instantiate(prefab);
        //        g.name = prefab.name;

        //    }
        //    return _instance;
        //}


        /// <summary>
        /// 开启Socket线程
        /// </summary>
        /// <param name="socketType"></param>
        public void StartSocket(SocketType socketType)
        {
            switch (socketType)
            {
                case SocketType.Tcp:
                    _tcpClient.Init(_serverIP, _serverPort);
                    break;
                case SocketType.Udp:
                    _udpClient.Init(_serverIP, _serverPort);
                    break;
                case SocketType.All:
                    _tcpClient.Init(_serverIP, _serverPort);
                    _udpClient.Init(_serverIP, _serverPort);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 开启Socket线程，并指定服务器地址
        /// </summary>
        /// <param name="socketType"></param>
        /// <param name="serverIP"></param>
        /// <param name="serverPort"></param>
        public void StartSocket(SocketType socketType, string serverIP, int serverPort)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            StartSocket(socketType);
        }
        /// <summary>
        /// 添加数据包到发送队列
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="socketType"></param>
        public void AddPacket(SocketPacket packet, SocketType socketType)
        {
            switch (socketType)
            {
                case SocketType.Tcp:
                    _tcpClient.AddPacket(packet);
                    break;
                case SocketType.Udp:
                    _udpClient.AddPacket(packet);
                    break;
                case SocketType.All:
                    _tcpClient.AddPacket(packet);
                    _udpClient.AddPacket(packet);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 触发对应MsgId的回调
        /// </summary>
        /// <param name="packet"></param>
        void DoMsgIdEvent(SocketPacket packet)
        {
            if (_msgIdEvent.ContainsKey(packet.MsgId))
            {
                _msgIdEvent[packet.MsgId]?.Invoke(packet);
            }
        }


        /// <summary>
        /// 注册一个对应msgId的回调
        /// 当管理器取得msgId的数据包时执行回调
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="callback"></param>
        public void AddEvent(int msgId, ReceivePacket callback)
        {
            if (_msgIdEvent.ContainsKey(msgId))
            {
                _msgIdEvent[msgId] += callback;
            }
            else
            {
                _msgIdEvent.Add(msgId, callback);
            }
        }
        /// <summary>
        /// 移除一个已注册回调
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="callback"></param>
        public void RemoveEvent(int msgId, ReceivePacket callback)
        {
            if (_msgIdEvent.ContainsKey(msgId))
            {
                _msgIdEvent[msgId] -= callback;
            }
        }
        /// <summary>
        /// 移除对应id绑定的所有回调
        /// </summary>
        /// <param name="msgId"></param>
        public void RemoveEvent(int msgId)
        {
            if (_msgIdEvent.ContainsKey(msgId))
            {
                _msgIdEvent.Remove(msgId);
            }
        }
    }
}
#endif