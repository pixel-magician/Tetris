using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PM
{
    /// <summary>
    /// 场景管理对象
    /// </summary>
    public class ScenesManager : MonoBehaviour
    {


        static ScenesManager _instance = null;

        /// <summary>
        /// 一个无参的委托类型
        /// </summary>
        public delegate void DelegateNoArge();
        /// <summary>
        /// 一个浮点参数的委托
        /// </summary>
        /// <param name="progress"></param>
        public delegate void DelegateFloatArge(float progress);
        /// <summary>
        /// 字符类型的委托
        /// </summary>
        /// <param name="content"></param>
        public delegate void DelegateTextArge(string content);

        /// <summary>
        /// 场景加载失败事件
        /// </summary>
        public event DelegateTextArge EventLoadFailed;





        /// <summary>
        /// 场景名列表
        /// </summary>
        public List<string> SceneNames = new List<string>();
        /// <summary>
        /// 场景加载的记录缓存
        /// </summary>
        [SerializeField]
        List<string> _catchSceneName = new List<string>();

        /// <summary>
        /// 异步场景缓存
        /// </summary>
        Dictionary<string, SceneAsync> _asynScenes = new Dictionary<string, SceneAsync>();


        private void Awake()
        {
            _instance = this;
            //StartCoroutine(IEUpdate());
        }
        private void LateUpdate()
        {
            List<string> removeList = new List<string>();
            foreach (var item in _asynScenes)
            {
                //item.Value.Update();
                if (item.Value.Update())
                {
                    //_asynScenes.Remove(item.Key);
                    removeList.Add(item.Key);
                }
            }
            foreach (var item in removeList)
            {
                _asynScenes.Remove(item);
            }
        }


        //IEnumerator IEUpdate()
        //{
        //    while (true)
        //    {

        //        yield return new WaitForEndOfFrame();
        //        //yield return new WaitForSeconds(0.5f);
        //    }
        //}





        /// <summary>
        /// 获取管理器实例
        /// </summary>
        /// <returns></returns>
        public static ScenesManager GetInstance()
        {
            return _instance;
        }






        /// <summary>
        /// 初始化管理器
        /// </summary>
        public virtual void InitManager()
        {

            //获取 构建设置 中的场景名集合，后续可能会动态下载新场景文件，所以用场景名列表维护
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                SceneNames.Add(SceneManager.GetSceneAt(i).name);
            }
        }

        /// <summary>
        /// 关闭管理器并且释放资源
        /// </summary>
        public virtual void OnClose()
        {
            _asynScenes.Clear();
            _catchSceneName.Clear();
        }


        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">加载的场景名</param>
        /// <param name="callback">加载完成后的回调</param>
        public virtual bool LoadScene(string sceneName, DelegateNoArge callback = null)
        {
            SceneManager.LoadScene(sceneName);
            if (callback != null)
            {
                callback();
            }
            _catchSceneName.Add(sceneName);
            return true;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="allowSceneActivation">场景加载完毕时是否允许自动激活场景,为false时将会把异步加载的场景加入异步场景缓存，但是不激活。后期动过ActiveScene()激活该场景</param>
        /// <param name="callback">异步加载完成，且allowSceneActivation为true时执行</param>
        /// <param name="isAddCatch">是否将本次加载的场景记录到缓存（该缓存用于返回上一场景）</param>
        public virtual bool LoadSceneAsyn(string sceneName, Action<float> progress = null, Action callback = null, bool isAddCatch = true)
        {
            try
            {
                AsyncOperation async;
                async = SceneManager.LoadSceneAsync(sceneName);
                Debug.Log("加载场景：" + sceneName);
                async.allowSceneActivation = false;
                async.completed += (obj) => { callback?.Invoke(); };


                SceneAsync sceneAsync = new SceneAsync()
                {
                    Async = async,
                    //CallBack = callback,
                    Name = sceneName,
                    Progress = progress,
                };
                if (_asynScenes.ContainsKey(sceneName))
                {
                    _asynScenes[sceneName] = sceneAsync;
                }
                else
                {
                    _asynScenes.Add(sceneName, sceneAsync);
                }
                if (isAddCatch)
                {
                    _catchSceneName.Add(sceneName);
                }
                else
                {
                    _catchSceneName.RemoveAt(_catchSceneName.Count - 1);
                }
            }
            catch
            {

                EventLoadFailed?.Invoke("场景加载错误：" + sceneName);
                return false;
            }
            return true;
        }



        /// <summary>
        /// 激活异步加载的场景
        /// </summary>
        /// <param name="sceneName"></param>
        public virtual void ActiveScene(string sceneName)
        {
            if (_asynScenes.ContainsKey(sceneName))
            {
                _asynScenes[sceneName].Active();
            }
        }

        /// <summary>
        /// 返回当前激活的场景对象
        /// </summary>
        /// <returns></returns>
        public virtual Scene GetActiveScene()
        {
            return SceneManager.GetActiveScene();
        }
        /// <summary>
        /// 顺序加载下一个场景
        /// 预定的列表
        /// </summary>
        /// <param name="callback"></param>
        public virtual bool NextScene(DelegateNoArge callback = null)
        {
            string sceneName = GetNextSceneName();
            if (sceneName == null)
            {
                return false;
            }
            return LoadScene(sceneName, callback);
        }
        /// <summary>
        /// 顺序加载上一个场景
        /// </summary>
        /// <param name="callback"></param>
        public virtual bool PreviousScene(DelegateNoArge callback = null)
        {
            string sceneName = GetPreviousSceneName();
            if (sceneName == null)
            {
                return false;
            }
            return LoadScene(sceneName, callback);
        }

        /// <summary>
        /// 异步加载下一个场景
        /// 预定的列表
        /// </summary>
        /// <param name="allowSceneActivation"></param>
        /// <param name="progress"></param>
        /// <param name="callback"></param>
        public virtual bool NextSceneAsync(Action<float> progress = null, Action callback = null)
        {
            string sceneName = GetNextSceneName();
            if (sceneName == null)
            {
                Debug.Log("场景名为空");
                return false;
            }

            return LoadSceneAsyn(sceneName, progress, callback);
        }
        /// <summary>
        /// 异步加载上一个场景
        /// </summary>
        /// <param name="allowSceneActivation"></param>
        /// <param name="progress"></param>
        /// <param name="callback"></param>
        public virtual bool PreviousSceneAsync(Action<float> progress = null, Action callback = null)
        {
            string sceneName = GetPreviousSceneName();
            if (sceneName == null)
            {
                Debug.Log("场景名为空");
                return false;
            }
            //Debug.Log("跳转场景：" + sceneName);
            return LoadSceneAsyn(sceneName, progress, callback);
        }

        /// <summary>
        /// 返回上一个场景
        /// </summary>
        /// <param name="callback"></param>
        public virtual bool BackScene(DelegateNoArge callback = null)
        {
            string sceneName = GetBackSceneName();
            if (sceneName == null)
            {
                return false;
            }
            return LoadScene(sceneName, callback);
        }
        /// <summary>
        /// 异步返回上一个场景
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="callback"></param>
        public virtual bool BackSceneAsync(Action<float> progress = null, Action callback = null)
        {
            string sceneName = GetBackSceneName();
            if (sceneName == null)
            {
                Debug.Log("场景名为空");
                return false;
            }
            //Debug.Log("跳转场景：" + sceneName);
            return LoadSceneAsyn(sceneName, progress, callback, false);
        }

        /// <summary>
        /// 清空场景名缓存
        /// </summary>
        public virtual void ClearSceneNames()
        {
            _catchSceneName.Clear();
        }



        /// <summary>
        /// 顺序获取下一个场景的名字
        /// </summary>
        /// <returns></returns>
        string GetNextSceneName()
        {
            string sceneName = GetActiveScene().name;
            int index = SceneNames.IndexOf(sceneName);
            if (index >= SceneNames.Count - 1 || index == -1)
            {
                sceneName = null;
            }
            else
            {
                sceneName = SceneNames[index + 1];
            }
            return sceneName;
        }

        /// <summary>
        /// 顺序获取上一个场景名
        /// </summary>
        /// <returns></returns>
        string GetPreviousSceneName()
        {
            string sceneName = GetActiveScene().name;
            int index = SceneNames.IndexOf(sceneName);
            if (index <= 0)
            {
                sceneName = null;
            }
            else
            {
                sceneName = SceneNames[index - 1];
            }
            return sceneName;
        }
        /// <summary>
        /// 获取需要返回的场景名称
        /// </summary>
        /// <returns></returns>
        string GetBackSceneName()
        {
            string sceneName = GetActiveScene().name;
            int index = _catchSceneName.LastIndexOf(sceneName);
            if (index <= 0)
            {
                sceneName = null;
            }
            else
            {
                sceneName = _catchSceneName[index - 1];
            }
            return sceneName;
        }
    }


}