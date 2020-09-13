using UnityEngine;
using System;


namespace PM
{
    class SceneAsync
    {
        public string Name { set; get; }
        public AsyncOperation Async { set; get; }
        public ScenesManager.DelegateNoArge CallBack { set; get; }
        public Action<float> Progress { set; get; }

        float _progress = 0;

        /// <summary>
        /// 加载完毕返回true
        /// 未完成返回false
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            //进度为100%表示完成加载，激活场景
            if (_progress == 1)
            {
                if (CallBack != null)
                {
                    CallBack();
                    //执行后清空回调，保证回调只执行一次
                    CallBack = null;
                    Progress = null;
                }
                return true;
            }
            //进度不足，获取新的进度值
            _progress = Async.progress;
            //触发进度事件
            if (Progress != null)
            {
                Progress(_progress);
            }
            //判断异步是否完成
            if (Async.progress < 0.9f)
            {
                return false;
            }
            else
            {
                _progress = 1;
                Async.allowSceneActivation = true;
                if (Progress != null)
                {
                    Progress(_progress);
                }
            }

            return false;
        }
        /// <summary>
        /// 激活场景
        /// </summary>
        public void Active()
        {
            Async.allowSceneActivation = true;
        }
    }
}