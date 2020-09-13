using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;


namespace PM
{
    /*
     * 以将物体设置到非渲染的layout层，代替直接设置Active。用以节省性能，也不用改变物体坐标.
     */
    /// <summary>
    /// 资源管理器，对象池
    /// </summary>
    public class PoolsManager : MonoBehaviour
    {
        static PoolsManager _instance = null;


        [SerializeField]
        [Tooltip("对象池中每个容器容纳对象的个数")]
        private int _countMax = 100;


        /// <summary>
        /// 容器池
        /// </summary>
        Dictionary<string, Transform> _containers = new Dictionary<string, Transform>();
        /// <summary>
        /// 预设池
        /// </summary>
        Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        /// <summary>
        /// 对象池
        /// </summary>
        Dictionary<string, Queue<PoolsData>> _pools = new Dictionary<string, Queue<PoolsData>>();
        /// <summary>
        /// 待删除物体的集合
        /// </summary>
        Queue<PoolsData> _deletes = new Queue<PoolsData>();



        /// <summary>
        /// 对象池中每个容器容纳对象的个数
        /// </summary>
        public int CountMax { get => _countMax; set => _countMax = value; }
        /// <summary>
        /// 对象池实例
        /// </summary>
        public static PoolsManager Instance { get => _instance; set => _instance = value; }



        public virtual void Awake()
        {
            Init();
            _instance = this;
        }



        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            _prefabs.Clear();
            _pools.Clear();
            //开始协程，自动清理池中“溢出”的对象
            StartCoroutine(IEScavenger());
        }



        /// <summary>
        /// 添加预设对象，从Resources中添加
        /// </summary>
        /// <param name="path">待添加预设的路径</param>
        public virtual void Add_Prefabs(string path)
        {
            GameObject[] gameObjects = Resources.LoadAll<GameObject>(path);
            Add_Prefabs(gameObjects);
        }
        /// <summary>
        /// 添加预设对象，从AssetBundle中添加
        /// </summary>
        /// <param name="assetBundle"></param>
        public virtual void Add_Prefabs(AssetBundle assetBundle)
        {
            GameObject[] gameObjects = assetBundle.LoadAllAssets<GameObject>();
            Add_Prefabs(gameObjects);
        }
        /// <summary>
        /// 添加预设对象，指定数组
        /// </summary>
        /// <param name="gameObjects">待添加的预设数组</param>
        public void Add_Prefabs(GameObject[] gameObjects)
        {
            foreach (var item in gameObjects)
            {
                _prefabs[item.name] = item;
            }
        }



        /// <summary>
        /// 将对象放入池
        /// 将对象存入指定的Key中，未指定时读取对面名称作为Key
        /// 释放前自动执行IRelease接口（不是必须，不继承IRelease接口也能正常执行。但是强烈建议继承IRelease接口）。
        /// </summary>
        /// <param name="g">回收的对象</param>
        /// <param name="key">回收对象将要存入的Key值</param>
        public void Push(GameObject g, string key = null)
        {
            //释放内存
            g.GetComponent<IRelease>()?.Release();
            PoolsData data = new PoolsData(g);


            string gName = g.name;
            if (key != null) gName = key;
            if (_pools.ContainsKey(gName))
            {
                //到达缓存上限，将多余对象踢出
                if (_pools[gName].Count >= CountMax) _deletes.Enqueue(_pools[gName].Dequeue());
                _pools[gName].Enqueue(data);
            }
            else
            {
                _pools.Add(gName, new Queue<PoolsData>());
                _pools[gName].Enqueue(data);
            }
            //设置父节点
            Transform parent = _containers.ContainsKey(gName) ? _containers[gName] : null;
            if (parent == null)
            {
                GameObject g2 = new GameObject(gName);
                g2.transform.SetParent(_instance.transform);
                parent = g2.transform;
                _containers.Add(gName, parent);
            }
            g.transform.SetParent(parent);
        }
        /// <summary>
        /// 从池中取出对象
        /// </summary>
        /// <param name="name">预设名称</param>
        /// <param name="pTransform">获取后设置父节点</param>
        /// <param name="isInPrefabs">标记，当池中没有所需对象时，是否从预设中实例化</param>
        /// <returns></returns>
        public GameObject Pull(string name, Transform pTransform = null, bool isInPrefabs = true)
        {
            GameObject instance = null;
            //从对象池中取对象
            if (_pools.ContainsKey(name))
            {
                Debug.Log(_pools[name].Count);
                if (_pools[name].Count >= 1)
                {
                    var v = _pools[name].Dequeue();
                    instance = v?.Target;
                }
            }
            //从预设池中实例化新对象
            else if (isInPrefabs && _prefabs.ContainsKey(name))
            {
                GameObject g = Instantiate(_prefabs[name], transform);
                g.name = name;
                instance = g;
            }
            instance?.transform?.SetParent(pTransform);
            return instance;

        }

        /// <summary>
        /// 对溢出的对象进行清理
        /// </summary>
        /// <returns></returns>
        IEnumerator IEScavenger()
        {
            while (true)
            {
                yield return null;
                if (_deletes.Count > 0)
                    _deletes.Dequeue().DestoryItem();
            }
        }





        class PoolsData
        {
            private GameObject _target;
            /// <summary>
            /// 对象物体
            /// </summary>
            public GameObject Target
            {
                get
                {
                    _target.SetActive(true);
                    return _target;
                }
                set
                {
                    value.SetActive(false);
                    _target = value;
                }
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="target">目标物体</param>
            /// <param name="toLayout">给目标设定新层</param>
            public PoolsData(GameObject target)
            {
                Target = target;
            }

            /// <summary>
            /// 销毁对象
            /// </summary>
            public void DestoryItem()
            {
                Destroy(_target);
                _target = null;
            }


        }
    }
}