using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM
{
    /// <summary>
    /// UI窗口接口
    /// </summary>
    public interface IUIWindow
    {
        /// <summary>
        /// 窗口已显示
        /// </summary>
        Action OnWindowShow { set; get; }
        /// <summary>
        /// 窗口已隐藏
        /// </summary>
        Action OnWindowHide { set; get; }

        /// <summary>
        /// 该窗口的管理者
        /// </summary>
        UIManagerBase UIManager { set; get; }

        /// <summary>
        /// 当前窗口是否加入导航列表
        /// 为false时，该窗口独立
        /// </summary>
        bool IsAddNavigation { get; }

        /// <summary>
        /// 窗口Id，只读
        /// </summary>
        WindowId WindowId { get; }
        /// <summary>
        /// 初始化UI窗口
        /// </summary>
        /// <param name="args">参数</param>
        void InitWindow(object args = null);
        /// <summary>
        /// 显示窗口
        /// 生命周期在Awake之后，Start之前
        /// </summary>
        /// <param name="args">参数</param>
        void ShowWindow(object args = null);//这里为了适应给窗口传递多个参数的情况
        /// <summary>
        /// 隐藏窗口
        /// </summary>
        void HideWindows();



    }
}