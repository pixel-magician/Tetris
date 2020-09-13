using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChessBoard : MonoBehaviour
{
    [Tooltip("棋盘尺寸，X代表行数，Y代表列数")]
    [SerializeField]
    Vector2Int _size = new Vector2Int(20, 10);
    [SerializeField]
    Transform _container;
    [SerializeField]
    GameObject _row_Template;
    [SerializeField]
    GameObject _cell_Template;


    float _dgTime = 0.5f;
    RectTransform rect;
    bool _isPause = false;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CreateBG(_size);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPause) return;
    }



    /// <summary>
    /// 创建背景棋盘格
    /// </summary>
    public void CreateBG(Vector2Int chessboard_Size)
    {
        GameObject row_Template = Fill_Container(chessboard_Size.y, _row_Template.transform, _cell_Template);
        Fill_Container(chessboard_Size.x, _container, row_Template);
    }

    /// <summary>
    /// 打开棋盘，棋盘运行，进入可操作状态
    /// </summary>
    public void Open()
    {
        rect.DOAnchorPos3DY(50, _dgTime);
        rect.DOSizeDelta(new Vector2(800, 1600), _dgTime);
    }


    /// <summary>
    /// 关闭棋盘，进入不可操作状态
    /// </summary>
    public void Close()
    {
        rect.DOAnchorPos3DY(350, _dgTime);
        rect.DOSizeDelta(new Vector2(600, 1200), _dgTime);
    }








    /// <summary>
    /// 填充容器
    /// 用child对象作为模板填充容器
    /// </summary>
    /// <param name="cellnum">要填充的子节点的数量</param>
    /// <param name="container">待填充的父容器</param>
    /// <param name="child">子节点对象</param>
    /// <returns></returns>
    GameObject Fill_Container(int cellnum, Transform container, GameObject child)
    {
        string nameTemp = child.name;
        for (int i = 0; i < cellnum; i++)
        {
            GameObject g = Instantiate(child, container.transform);
            g.SetActive(true);
            g.name = string.Format("{0}_{1}", nameTemp, i);
        }
        return container.gameObject;
    }
}
