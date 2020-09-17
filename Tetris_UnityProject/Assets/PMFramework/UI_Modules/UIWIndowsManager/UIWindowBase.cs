using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PM
{
    public class UIWindowBase : MonoBehaviour, IUIWindow
    {
        /// <summary>
        /// 窗口Id
        /// </summary>
        public virtual WindowId WindowId { get { return WindowId.None; } }
        /// <summary>
        /// 该窗口的管理者
        /// </summary>
        public virtual UIManagerBase UIManager { set; get; }

        /// <summary>
        /// 窗口是否加入导航中
        /// </summary>
        public virtual bool IsAddNavigation { get { return true; } }

        public Action<IUIWindow> OnWindowShow { get; set; }
        public Action<IUIWindow> OnWindowHide { get; set; }



        /// <summary>
        /// 隐藏窗口
        /// </summary> 
        public virtual void HideWindows()
        {
            Debug.Log("隐藏：" + WindowId);
            gameObject.SetActive(false);
            OnWindowHide?.Invoke(this);
        }

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="args"></param>
        public virtual void InitWindow(object args = null)
        {

        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="args"></param>
        public virtual void ShowWindow(object args)
        {
            Debug.Log("显示窗口：" + WindowId);
            gameObject.SetActive(true);
            OnWindowShow?.Invoke(this);
        }
    }
}