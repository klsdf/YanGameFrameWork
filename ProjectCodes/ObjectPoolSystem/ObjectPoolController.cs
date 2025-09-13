/****************************************************************************
 * Author: 闫辰祥
 * Date: 2025-04-07
 * Description: 对象池系统，负责管理对象池。包括创建，释放，获取，统计等。
 *
 ****************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using YanGameFrameWork.Singleton;
namespace YanGameFrameWork.ObjectPoolSystem
{
    public class ObjectPoolController : Singleton<ObjectPoolController>
    {
        #region 私有字段
        private Dictionary<GameObject, ObjectPoolBase> _objectPoolsNew;
        private Transform _poolContainer;
        
        public bool _enableDebugPanel = true; // 是否启用调试面板功能
        
        // 帧率计算
        private float _fpsUpdateInterval = 0.5f; // 更新间隔
        private float _accum = 0.0f; // 累积时间
        private int _frames = 0; // 帧数计数
        private float _timeleft; // 剩余时间
        private float _fps = 0.0f; // 当前FPS
        #endregion

        #region 属性
        public Transform PoolContainer
        {
            get
            {
                _poolContainer ??= new GameObject("ObjectPool").transform;
                return _poolContainer;
            }
        }
        #endregion

        #region Unity生命周期
        protected override void Awake()
        {
            base.Awake();
            // YanGF.Debug.Log(nameof(ObjectPoolController), "初始化对象池");
            _objectPoolsNew = new Dictionary<GameObject, ObjectPoolBase>();
            
            // 初始化帧率计算
            _timeleft = _fpsUpdateInterval;
        }

        private void Update()
        {
            // 计算帧率
            CalculateFPS();

            if (!_enableDebugPanel)
                return;

            // 按F1键切换调试面板显示
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _enableDebugPanel = !_enableDebugPanel;
            }
            
            // 按ESC键关闭调试面板
            if (Input.GetKeyDown(KeyCode.Escape) && _enableDebugPanel)
            {
                _enableDebugPanel = false;
            }
        }

        /// <summary>
        /// 计算FPS
        /// </summary>
        private void CalculateFPS()
        {
            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            ++_frames;

            if (_timeleft <= 0.0)
            {
                _fps = _accum / _frames;
                _timeleft = _fpsUpdateInterval;
                _accum = 0.0f;
                _frames = 0;
            }
        }
        #endregion

        #region 对象池核心功能
        /// <summary>
        /// 注册对象池，并传入对象池的的对象
        /// </summary>
        /// <param name="prefab">预制体</param>
        public void RegisterPool(GameObject prefab)
        {
            if (!_objectPoolsNew.ContainsKey(prefab))
            {
                _objectPoolsNew[prefab] = new ObjectPoolBase(prefab);
            }
        }

        /// <summary>
        /// 获取对象池中的对象，需要传入注册时的预制体
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象</returns>
        public GameObject GetItem(GameObject prefab)
        {
            if (_objectPoolsNew.TryGetValue(prefab, out var pool))
            {
                return pool.GetItem();
            }

            // 如果对象池不存在，自动创建
            RegisterPool(prefab);
            return _objectPoolsNew[prefab].GetItem();
        }

        /// <summary>
        /// 返回对象池中的对象，需要传入注册时的预制体
        /// </summary>
        /// <param name="item">对象</param>
        /// <param name="prefab">预制体</param>
        public void ReturnItem(GameObject item, GameObject prefab)
        {
            if (_objectPoolsNew.TryGetValue(prefab, out var pool))
            {
                pool.ReturnItem(item);
            }
            else
            {
                YanGF.Debug.LogError(nameof(ObjectPoolController), $"返回对象池错误：未找到预制体 {prefab.name} 对应的对象池");
            }
        }
        #endregion

        #region 统计功能
        /// <summary>
        /// 获取目标对象池中的对象总数
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象总数</returns>
        public int GetTotalCount(GameObject prefab)
        {
            return _objectPoolsNew.TryGetValue(prefab, out var pool) ? pool.CountAll : 0;
        }

        /// <summary>
        /// 获取目标对象池中的对象活跃数量
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>对象活跃数量</returns>
        public int GetActiveCount(GameObject prefab)
        {
            return _objectPoolsNew.TryGetValue(prefab, out var pool) ? pool.ActiveCount : 0;
        }

        /// <summary>
        /// 批量统计方法，可以传入多个预制体，来统计多个对象池中的对象总数
        /// </summary>
        /// <param name="prefabs">预制体列表</param>
        /// <returns>对象总数</returns>
        public int GetTotalCount(IEnumerable<GameObject> prefabs)
        {
            int total = 0;
            foreach (var prefab in prefabs)
            {
                total += GetTotalCount(prefab);
            }
            return total;
        }

        /// <summary>
        /// 批量统计方法，可以传入多个预制体，来统计多个对象池中的对象活跃数量
        /// </summary>
        /// <param name="prefabs">预制体列表</param>
        /// <returns>对象活跃数量</returns>
        public int GetActiveCount(IEnumerable<GameObject> prefabs)
        {
            int total = 0;
            foreach (var prefab in prefabs)
            {
                total += GetActiveCount(prefab);
            }
            return total;
        }
        #endregion

        #region 调试面板GUI
        private void OnGUI()
        {
            if (!_enableDebugPanel)
                return;

            // 设置GUI样式
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 16;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.white;

            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 14;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.yellow;

            GUIStyle contentStyle = new GUIStyle(GUI.skin.label);
            contentStyle.fontSize = 12;
            contentStyle.normal.textColor = Color.white;

            // 计算面板大小和位置
            float panelWidth = 400f;
            float panelHeight = 350f; // 增加高度以容纳帧率信息
            float startX = Screen.width - panelWidth - 10f;
            float startY = 10f;

            // 绘制背景
            GUI.Box(new Rect(startX, startY, panelWidth, panelHeight), "");

            // 标题
            GUI.Label(new Rect(startX + 10, startY + 10, panelWidth - 20, 25), "对象池状态监控", titleStyle);

            // 关闭按钮
            if (GUI.Button(new Rect(startX + panelWidth - 60, startY + 10, 50, 25), "关闭"))
            {
                _enableDebugPanel = false;
            }

            float currentY = startY + 40;

            // 帧率显示
            Color fpsColor = _fps >= 30 ? Color.green : (_fps >= 15 ? Color.yellow : Color.red);
            GUIStyle fpsStyle = new GUIStyle(contentStyle);
            fpsStyle.normal.textColor = fpsColor;
            fpsStyle.fontStyle = FontStyle.Bold;
            
            GUI.Label(new Rect(startX + 10, currentY, panelWidth - 20, 20), $"FPS: {_fps:F1}", fpsStyle);
            currentY += 25;

            // 总对象池数量
            GUI.Label(new Rect(startX + 10, currentY, panelWidth - 20, 20), $"对象池总数: {_objectPoolsNew.Count}", headerStyle);
            currentY += 25;

            // 如果没有对象池，显示提示
            if (_objectPoolsNew.Count == 0)
            {
                GUI.Label(new Rect(startX + 10, currentY, panelWidth - 20, 20), "暂无对象池", contentStyle);
                return;
            }

            // 绘制每个对象池的信息
            foreach (var kvp in _objectPoolsNew)
            {
                GameObject prefab = kvp.Key;
                ObjectPoolBase pool = kvp.Value;

                // 对象池名称
                string poolName = prefab != null ? prefab.name : "Unknown";
                GUI.Label(new Rect(startX + 10, currentY, panelWidth - 20, 20), $"• {poolName}", headerStyle);
                currentY += 20;

                // 详细信息
                GUI.Label(new Rect(startX + 20, currentY, panelWidth - 30, 20), 
                    $"  总数: {pool.CountAll} | 活跃: {pool.ActiveCount} | 空闲: {pool.CountAll - pool.ActiveCount}", contentStyle);
                currentY += 18;

                // 如果面板高度不够，停止绘制
                if (currentY > startY + panelHeight - 30)
                {
                    GUI.Label(new Rect(startX + 10, currentY, panelWidth - 20, 20), "...", contentStyle);
                    break;
                }
            }

            // 底部提示
            GUI.Label(new Rect(startX + 10, startY + panelHeight - 25, panelWidth - 20, 20), 
                "按F1键切换显示 | 按ESC键关闭", contentStyle);
        }
        #endregion

    }

}