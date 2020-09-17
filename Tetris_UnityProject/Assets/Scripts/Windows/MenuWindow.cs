﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;
using DG.Tweening;
using System;

public class MenuWindow : UIWindowBase
{
    public override WindowId WindowId => WindowId.Menu;

    [SerializeField]
    RectTransform _title;
    [SerializeField]
    RectTransform _downMenu;
    [SerializeField]
    PMButton _btnBack;


    float _dgTime = 0.5f;

    public override void ShowWindow(object args)
    {
        gameObject.SetActive(true);
        _btnBack.gameObject.SetActive(GlobalManager.Instance._gameMachine.CurrentState is StateGamePause);
        AnimationMove(false, ()=>
        {
            OnWindowShow?.Invoke(this);
        });
    }

    public override void HideWindows()
    {

        AnimationMove(true, () =>
        {
            gameObject.SetActive(false);
            OnWindowHide?.Invoke(this);
        });
    }



    /// <summary>
    /// 移动动画效果
    /// </summary>
    /// <param name="isMoveOut">是否处于移除状态</param>
    /// <param name="callback">动画结束后的回调</param>
    void AnimationMove(bool isMoveOut, Action callback)
    {
        float titlePos;
        float downMenuPos;
        if (isMoveOut)
        {
            titlePos = 180;
            downMenuPos = -210;
        }
        else
        {
            titlePos = -150;
            downMenuPos = 100;
        }
        _title.DOAnchorPos3DY(titlePos, _dgTime);
        _downMenu.DOAnchorPos3DY(downMenuPos, _dgTime).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }


    #region UI事件

    public void OnBtnStart()
    {
        Debug.Log("点击开始/重新开始按钮");
        GlobalManager.Instance._gameMachine.ChangeState(EnumGameState.Run);
    }

    public void OnBtnBack()
    {
        Debug.Log("点击返回按钮");
        GlobalManager.Instance._gameMachine.ChangeState(EnumGameState.Run);
    }

    public void OnToggleSound(bool b)
    {
        Debug.Log("点击声音开关按钮");
    }

    public void OnBtnRecord()
    {
        Debug.Log("点击计分按钮");
    }

    #endregion
}
