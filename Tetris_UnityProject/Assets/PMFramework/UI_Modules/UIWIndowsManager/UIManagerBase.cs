using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PM
{
    /// <summary>
    /// 基类，UI管理器
    /// </summary>
    public class UIManagerBase : MonoBehaviour
    {

        /// <summary>
        /// 创建物体
        /// 将创建物体的权限开放给外部，可适配对象池、Resources.Load等多种实例化方法
        /// </summary>
        public Func<GameObject, Transform, GameObject> CreateItem { set; get; }
        /// <summary>
        /// 销毁物体
        /// 将销毁物体的权限开放给外部，可兼容对象池
        /// </summary>
        public Action<GameObject> DestoryItem { set; get; }


        /// <summary>
        /// UI窗体的容器
        /// </summary>
        [SerializeField]
        [Tooltip("UI窗体的容器")]
        Transform _container;
        /// <summary>
        /// 让UI可交互的组件
        /// </summary>
        [SerializeField]
        [Tooltip("让UI可交互的组件 GraphicRaycaster")]
        GraphicRaycaster _graphicRaycaster;


        /// <summary>
        /// 导航列表
        /// </summary>
        List<IUIWindow> _navigationList = new List<IUIWindow>();
        /// <summary>
        /// 预设集合
        /// </summary>
        Dictionary<WindowId, GameObject> _prefabs = new Dictionary<WindowId, GameObject>();


        /// <summary>
        /// 当前焦点的窗体
        /// </summary>
        public virtual IUIWindow Current
        {
            get { return _navigationList?.Last(); }
        }

        /// <summary>
        /// 是否允许交互
        /// </summary>
        public virtual bool Interactivity
        {
            set { _graphicRaycaster.enabled = value; }
            get { return _graphicRaycaster.enabled; }
        }







        /// <summary>
        /// 初始化窗口管理
        /// </summary>
        /// <param name="prefabPath">预设的路径</param>
        public virtual void InitManager(string prefabsPath)
        {
            ClearWindow();
            //读取窗体所需的预设，并保存引用
            GameObject[] gameObjects = Resources.LoadAll<GameObject>(prefabsPath);
            foreach (var item in gameObjects)
            {
                var v = item.GetComponent<IUIWindow>();
                if (v == null) continue;
                _prefabs.Add(v.WindowId, item);
            }
        }
        /// <summary>
        /// 清除所有创建并销毁对象,
        /// 清空导航列表
        /// </summary>
        public virtual void ClearWindow()
        {
            for (int i = _navigationList.Count - 1; i >= 0; i--)
            {
                var window = _navigationList[i];
                if (window != null) DestoryItem?.Invoke((window as UIWindowBase).gameObject);
            }
            _navigationList.Clear();
        }

        /// <summary>
        /// 打开指定UI界面
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        IUIWindow OpenWindow(WindowId windowId, object args)
        {
            IUIWindow window;
            if (_prefabs.ContainsKey(windowId))
            {
                //每次都重新实例化,支持多个相同窗口
                GameObject g = CreateItem?.Invoke(_prefabs[windowId], _container);
                window = g.GetComponent<IUIWindow>();
            }
            else
            {
                throw new System.Exception("指定UI窗口不存在:" + windowId);
            }
            if (window == null) throw new System.Exception("指定UI窗口实例化为空:" + windowId);
            window.UIManager = this;
            window.ShowWindow(args);
            return window;
        }

        /// <summary>
        /// 切换到下一个窗口
        /// </summary>
        /// <param name="windowId">下一个窗口对象的Id</param>
        /// <param name="windowAction">对上一窗口的操作，默认为隐藏。仅对加到导航列表的窗口有效</param>
        /// <param name="clearNavigation">清除导航列表，默认不清除</param>
        /// <param name="args">窗口参数</param> 
        public virtual void NextWindow(WindowId windowId, object args = null, bool clearNavigation = false, WindowActionState windowAction = WindowActionState.Hide, Action callback = null)
        {
            if (clearNavigation) ClearWindow();
            var oldWindow = _navigationList.Count > 0 ? _navigationList.Last() : null;
            if (_navigationList.Count > 0)
            {
                WindowAction(oldWindow, windowAction, (w) =>
                {
                    //考虑到上一窗口的隐藏可能不是立即执行（协程，动画等）,将显示下一个窗口的逻辑放到回调里执行
                    var window = OpenWindow(windowId, args);
                    if (window.IsAddNavigation) _navigationList.Add(window);
                    callback?.Invoke();
                    w.OnWindowShow = null;
                    w.OnWindowHide = null;
                });
            }
            else
            {
                var window = OpenWindow(windowId, args);
                if (window.IsAddNavigation) _navigationList.Add(window);
                callback?.Invoke();
            }

            //return _navigationList.Last();
        }

        /// <summary>
        /// 返回到上一个窗口
        /// </summary>
        public virtual void BackWindow(Action callback = null)
        {
            if (_navigationList.Count > 0)
            {
                var temp = _navigationList.Last();
                _navigationList.RemoveAt(_navigationList.Count - 1);
                DestoryItem?.Invoke((temp as UIWindowBase).gameObject);
            }
            callback?.Invoke();
            if (_navigationList.Count > 0) _navigationList.Last().ShowWindow();
        }

        /// <summary>
        /// 返回到指定窗口
        /// </summary>
        /// <param name="window"></param>
        /// <param name="callback"></param>
        public virtual void BackWindow(IUIWindow window, Action callback = null)
        {
            int index = _navigationList.LastIndexOf(window);
            if (index < 0) throw new Exception("导航中没有记录该窗口");
            for (int i = 0; i < _navigationList.Count - index; i++)
            {
                var temp = _navigationList.Last();
                _navigationList.RemoveAt(_navigationList.Count - 1);
                DestoryItem?.Invoke((temp as UIWindowBase).gameObject);
            }
            callback?.Invoke();
            if (_navigationList.Count > 0) _navigationList.Last().ShowWindow();
        }

        /// <summary>
        /// 对窗口的操作
        /// </summary>
        /// <param name="window"></param>
        /// <param name="windowAction"></param>
        IUIWindow WindowAction(IUIWindow window, WindowActionState windowAction, Action<IUIWindow> action)
        {
            IUIWindow w = window;
            switch (windowAction)
            {
                case WindowActionState.None:
                    action?.Invoke(w);
                    break;
                case WindowActionState.Show://这一步判断貌似多余，以后再整理
                    w.OnWindowShow += action;
                    w?.ShowWindow();
                    break;
                case WindowActionState.Hide:
                    w.OnWindowHide += action;
                    w?.HideWindows();
                    break;
                case WindowActionState.Destory:
                    DestoryItem?.Invoke((w as UIWindowBase).gameObject);
                    action?.Invoke(w);
                    break;
                default:
                    break;
            }
            return window;
        }

        /// <summary>
        /// 关闭管理器
        /// </summary>
        public virtual void Close()
        {
            _prefabs.Clear();
            ClearWindow();
        }




        /// <summary>
        /// 窗口操作
        /// </summary>
        public enum WindowActionState
        {
            /// <summary>
            /// 无操作
            /// </summary>
            None,
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 隐藏
            /// </summary>
            Hide,
            /// <summary>
            /// 销毁
            /// </summary>
            Destory,
        }
    }
}