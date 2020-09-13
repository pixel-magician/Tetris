using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;
using DG.Tweening;

public class MenuWindow : UIWindowBase
{
    public override WindowId WindowId => WindowId.Menu;

    [SerializeField]
    RectTransform _title;
    [SerializeField]
    RectTransform _downMenu;


    float _dgTime = 0.5f;

    public override void ShowWindow(object args)
    {
        gameObject.SetActive(true);
        _title.DOAnchorPos3DY(-150, _dgTime);
        _downMenu.DOAnchorPos3DY(100, _dgTime).OnComplete(() =>
        {
            OnWindowShow?.Invoke();
        });
    }

    public override void HideWindows()
    {
        gameObject.SetActive(false);
        _title.DOAnchorPos3DY(180, _dgTime);
        _downMenu.DOAnchorPos3DY(-210, _dgTime).OnComplete(() =>
        {
            OnWindowHide?.Invoke();
        });
    }



    #region UI事件

    public void OnBtnStart()
    {
        Debug.Log("点击开始/重新开始按钮");
    }

    public void OnBtnBack()
    {
        Debug.Log("点击返回按钮");
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
