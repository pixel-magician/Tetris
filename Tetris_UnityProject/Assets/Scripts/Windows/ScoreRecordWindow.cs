using PM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreRecordWindow : UIWindowBase
{
    public override WindowId WindowId => WindowId.ScoreRecord;

    public override bool IsAddNavigation => false;

    [SerializeField]
    TextMeshProUGUI _score;
    [SerializeField]
    TextMeshProUGUI _bestScore;
    [SerializeField]
    TextMeshProUGUI _runCount;




    public override void ShowWindow(object args)
    {
        base.ShowWindow(args);
    }



    public override void HideWindows()
    {
        base.HideWindows();
    }


    #region UI事件

    public void OnBtnBack()
    {

    }

    #endregion
}
