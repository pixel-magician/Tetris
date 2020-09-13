using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
//using Newtonsoft.Json;
using LitJsonExt;
using System;
using System.IO;


namespace PM
{
    /// <summary>
    /// 通讯对象,http
    /// </summary>
    /// <typeparam name="T">对象包含的数据类型</typeparam>
    public class WWW_Net<T>
    {
        public delegate void Request_OutTime();
        public delegate void Request_OK(T result);
        public delegate void Request_Failed(string result);
        public delegate void Request_Error(string result);

        /// <summary>
        /// 事件：请求成功
        /// </summary>
        public event Request_OK Event_RequestOK;
        /// <summary>
        /// 事件：请求超时
        /// </summary>
        public event Request_OutTime Event_OutTime;
        /// <summary>
        /// 请求失败，网络层
        /// </summary>
        public event Request_Failed Event_Failed;
        /// <summary>
        /// 请求错误，数据层
        /// </summary>
        public event Request_Error Event_Error;



        bool _multipart = false;


        /*  
         *  事件:
         *  超时事件
         *  网络错误事件
         *  请求失败事件
         *  返回请求结果事件
         * 
         */

        /// <summary>
        /// get参数
        /// </summary>
        Dictionary<string, object> _param = new Dictionary<string, object>();
        /// <summary>
        /// post参数
        /// </summary>
        Dictionary<string, object> _body = new Dictionary<string, object>();
        /// <summary>
        /// 请求头
        /// </summary>
        Dictionary<string, string> headers = new Dictionary<string, string>();
        /// <summary>
        /// 请求结果,仅包含数据部分
        /// </summary>
        string _result = "";

        WWWForm _form = new WWWForm();


        string _url = "";

        //string _urlHead = "http://";
        WWW _www;
        ///// <summary>
        ///// URL地址头部，http/https协议
        ///// </summary>
        //public string UrlHead
        //{
        //    get
        //    {
        //        return _urlHead;
        //    }

        //    set
        //    {
        //        _urlHead = value;
        //    }
        //}

        public WWW_Net()
        {

        }
        public WWW_Net(string apiPath)
        {
            _url = apiPath;
        }





        /// <summary>
        /// 添加post参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="json"></param>
        public void AddParam_Post(string key, object o)
        {
            _body[key] = o;
            _form.AddField(key, o.ToString());
        }
        public void AddParam_Post(string key, byte[] bytes)
        {
            _multipart = true;
            _form.AddBinaryData(key, bytes);
        }

        /// <summary>
        /// 添加get参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        public void AddParam_Get(string key, object o)
        {
            _param.Add(key, o);
        }
        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeaders(string key, string value)
        {
            //headers.Add(key, value);
            if (headers.ContainsKey(key))
            {
                headers[key] = value;
            }
            else
            {
                headers.Add(key, value);
            }




            //if (_form.headers.ContainsKey(key))
            //{
            //    _form.headers[key] = value;
            //}
            //else
            //{
            //    _form.headers.Add(key, value);
            //}
        }

        /// <summary>
        /// 获取post参数
        /// </summary>
        /// <returns></returns>
        byte[] GetBody()
        {
            //if (_form.data.Length <= 0)
            //    return null;
            //else
            //    return _form.data;



            if (_body.Keys.Count <= 0)
            {
                return null;
            }
            string json = JsonMapper.ToJson(_body);
            //string json = JsonConvert.SerializeObject(_body);
            byte[] bytes = Encoding.Default.GetBytes(json);
            return bytes;
        }
        /// <summary>
        /// 获取URL，包含Get参数
        /// </summary>
        /// <returns></returns>
        string GetURL()
        {
            StringBuilder sb = new StringBuilder(_url);
            if (_param.Count <= 0)
            {
                return sb.ToString();
            }
            if (_param != null && _param.Count > 0)
            {
                sb.Append("?");

                int count = 0;
                foreach (KeyValuePair<string, object> entry in _param)
                {
                    sb.Append(entry.Key).Append("=").Append(WWW.EscapeURL(entry.Value.ToString()));
                    ++count;
                    if (count < _param.Count)
                        sb.Append("&");
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 获取WWW对象
        /// </summary>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        public WWW GetWWW(string serverPath)
        {
            string url = serverPath + GetURL();
            byte[] bytes = GetBody();

            if (_multipart)
            {
                foreach (var item in headers)
                {
                    if (item.Key == "Content-Type")
                    {
                        continue;
                    }
                    _form.AddField(item.Key, item.Value);
                }
                _www = new WWW(url, _form);
            }
            else
                _www = new WWW(url, bytes, headers);


            //if (_form.data.Length > 0)
            //    _www = new WWW(url, _form);
            //else
            //    _www = new WWW(url);


            return _www;
        }

        public virtual IEnumerator IESend(string serverPath, float outTime)
        {
            WWW www = GetWWW(serverPath);
            //Debug.Log(www.url);
            float startRequestTime = Time.time;
            //做超时判断
            while (!www.isDone)
            {
                yield return new WaitForEndOfFrame();
                if (Time.time - startRequestTime >= outTime)
                {
                    if (Event_OutTime != null)
                    {
                        Debug.Log(www.url);
                        Event_OutTime();
                    }
                    www.Dispose();
                    yield break;
                }
            }
            //请求失败的处理,一般是网络层的失败
            //退出函数
            if (!string.IsNullOrEmpty(www.error))
            {
                if (Event_Failed != null)
                {
                    Debug.Log(www.error.ToString() + "," + www.url);
                    Event_Failed(www.error/* + ":" + www.text*/);
                    //Event_Failed(www.error + ":" + www.url);
                }
                yield break;
            }
            Debug.Log(www.url);
            //Dictionary<string, object> jd = JsonConvert.DeserializeObject<Dictionary<string, object>>(www.text);
            LitJsonExt.JsonData jd = LitJsonExt.JsonMapper.ToObject(www.text);
            Debug.Log(www.text);

            if (jd.ContainsKey("errorMsg"))
            {
                if (Event_Error != null)
                {
                    //Debug.Log(jd["errorMsg"].ToString() + "," + www.url);
                    Event_Error(jd["errorMsg"].ToString());
                }
                yield break;
            }
            if (jd.ContainsKey("size"))
            {
                string result = LitJsonExt.JsonMapper.ToJson(jd["size"]);
                //string result = JsonConvert.SerializeObject(jd["size"]);
            }
            if (jd.ContainsKey("data"))
            {
                _result = LitJsonExt.JsonMapper.ToJson(jd["data"]);
            }
            else
            {
                _result = null;
            }
            if (Event_RequestOK != null)
            {
                Event_RequestOK(GetResult());
            }
        }

        public T GetResult()
        {
            T t = default(T);
            if (_result != null)
            {
                Type type = typeof(T);
                //类型为基本类型时直接转换
                if (type.IsPrimitive)
                {
                    t = (T)Convert.ChangeType(_result, typeof(T));
                }
                //类型为字符串是返回字符串
                else if (type.Equals(typeof(string)))
                {
                    t = (T)Convert.ChangeType(_result.Replace("\"", ""), typeof(T));
                }
                //类型为自定义Class时序列号json对象
                else
                {
                    t = JsonMapper.ToObject<T>(_result);
                }
            }
            return t;
        }




    }
}