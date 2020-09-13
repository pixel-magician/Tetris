using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PM
{
    /// <summary>
    /// 网络管理
    /// </summary>
    public class WWW_Manager : MonoBehaviour
    {

        public static WWW_Manager Instance = null;
        /// <summary>
        /// 服务器地址
        /// </summary>
        [SerializeField]
        public string _serverURL = "";

        /// <summary>
        /// 超时时间
        /// </summary>
        [SerializeField]
        float _outTime = 5;

        void Awake()
        {
            Instance = this;
        }




        /// <summary>
        /// 获取网络管理实例
        /// </summary>
        /// <returns></returns>
        public static WWW_Manager GetInstance()
        {
            if (Instance != null)
            {
                return Instance;
            }
            return null;
        }

        /// <summary>
        /// 获取通讯对象
        /// </summary>
        /// <typeparam name="T">对象包含的数据类型</typeparam>
        /// <returns></returns>
        public static WWW_Net<T> GetWWW<T>(string apiPath)
        {
            WWW_Net<T> www = new WWW_Net<T>(apiPath);

            www.AddHeaders("Content-Type", "application/json");
            //TODO::这里添加需要的请求头
            return www;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="www"></param>
        public static void Send<T>(WWW_Net<T> www)
        {
            Instance.StartCoroutine(www.IESend(Instance._serverURL, Instance._outTime));
        }






    }
}