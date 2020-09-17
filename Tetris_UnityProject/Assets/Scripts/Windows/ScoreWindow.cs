using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PM;
using DG.Tweening;
using TMPro;
using System;

public class ScoreWindow : UIWindowBase
{
    public override WindowId WindowId => WindowId.Score;

    [SerializeField]
    TextMeshProUGUI _score;
    [SerializeField]
    TextMeshProUGUI _bestScore;
    [SerializeField]
    PMButton _btnBack;


    float _dgTime = 0.5f;

    public override void ShowWindow(object args)
    {
        gameObject.SetActive(true);
        AnimationMove(false, () =>
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
        float scorePos;
        float btnPos;
        if (isMoveOut)
        {
            scorePos = 300;
            btnPos = 200;
        }
        else
        {
            scorePos = 0;
            btnPos = -50;
        }
        _score.transform.parent.GetComponent<RectTransform>().DOAnchorPosX(scorePos, _dgTime);
        _bestScore.transform.parent.GetComponent<RectTransform>().DOAnchorPosX(-scorePos, _dgTime);
        _btnBack.GetComponent<RectTransform>().DOAnchorPosX(btnPos, _dgTime).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }



    #region UI事件

    public void OnBtnBack()
    {
        Debug.Log("点击返回按钮");
        GlobalManager.Instance._gameMachine.ChangeState(EnumGameState.Pause);
    }


    #endregion
}
