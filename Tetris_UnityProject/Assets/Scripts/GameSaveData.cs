using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJsonExt;
using System.Security.Permissions;

public class GameSaveData
{
    /// <summary>
    /// 最高分
    /// </summary>
    public int BestScore { set; get; }
    /// <summary>
    /// 总游戏次数
    /// </summary>
    public int RunCount { set; get; }






    public string ToJson()
    {
        return JsonMapper.ToJson(this);
    }


    public static GameSaveData LoadJson(string json)
    {
        return JsonMapper.ToObject<GameSaveData>(json);
    }

}
