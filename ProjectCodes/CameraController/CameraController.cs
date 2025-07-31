using UnityEngine;
using YanGameFrameWork.Singleton;
using System.Collections;
using System;
using System.Collections.Generic;

namespace YanGameFrameWork.CameraController
{

    public enum MoveCurveType
    {
        Linear,
        SmoothStep,
        Custom
    }
    public class CameraController : Singleton<CameraController>
    {
        public Camera controlCamera;


        #region 拖拽相关参数
        public bool IsEnableDarg = true;

        [Header("拖动控制按键")]
        [Tooltip("选择使用左键进行拖动控制")]
        public bool useLeftMouseButton = false; // 左键拖动
        [Tooltip("选择使用右键进行拖动控制")]
        public bool useRightMouseButton = true; // 右键拖动

        public float dragSpeed = 2.0f;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;


        [Header("最小X")]
        public float minX = -10.0f;
        [Header("最大X")]
        public float maxX = 10.0f;
        [Header("最小Y")]
        public float minY = -10.0f;
        [Header("最大Y")]
        public float maxY = 10.0f;


        public List<Func<bool>> canDragEvents = new List<Func<bool>>();

        #endregion

        #region 缩放相关参数
        public bool IsEnableZoom = true;

        public float zoomSpeed = 2.0f;

        [Header("最小视野")]
        public float minFov = 15.0f;
        [Header("最大视野")]
        public float maxFov = 90.0f;

        #endregion


        #region 摄像机跟随相关参数
        public bool IsEnableFollow = true;
        public Transform followTarget;

        /// <summary>
        /// 摄像头移动的比例因子。
        /// 设置为1.0f时，摄像机移动的距离与玩家移动的距离相同。
        /// 设置为0.0f时，摄像机不移动。
        /// 设置为2.0f时，摄像机移动的距离是玩家移动的距离的2倍。
        /// </summary>
        public float moveFactor = 1.0f;


        /// <summary>
        /// 玩家上次已知的位置。
        /// </summary>
        private Vector3 _lastPlayerPosition;
        #endregion


        #region 摄像机注视相关参数
        public bool IsEnableLookAt = true;
        public Transform lookAtTarget;
        #endregion




        private bool isCinematicMode = false;

        void Start()
        {
            controlCamera ??= GetMainCamera();


            if (followTarget != null)
            {
                /// 初始化摄像头的偏移量和玩家的初始位置。
                _lastPlayerPosition = followTarget.position;
            }
        }


        void LateUpdate()
        {

            Drag();
            Zoom();
            Follow();
            LookAt();
        }

        private void LookAt()
        {
            if (!IsEnableLookAt)
                return;

            controlCamera.transform.LookAt(lookAtTarget.position);
        }

        private void Follow()
        {
            if (!IsEnableFollow || isCinematicMode)
                return;

            // print("player.position:" + player.position);
            // 计算玩家的移动距离
            Vector3 playerMovement = followTarget.position - _lastPlayerPosition;


            Vector3 newCameraPosition = controlCamera.transform.position + playerMovement * moveFactor;
            controlCamera.transform.position = newCameraPosition;
            _lastPlayerPosition = followTarget.position;
        }

        private void Drag()
        {
            if (!IsEnableDarg || isCinematicMode)
                return;

            // 检测左键或右键按下
            bool leftMouseDown = useLeftMouseButton && Input.GetMouseButtonDown(0);
            bool rightMouseDown = useRightMouseButton && Input.GetMouseButtonDown(1);

            if (leftMouseDown || rightMouseDown)
            {
                // 检查是否允许拖拽
                if (CanDrag() == false)
                {
                    return;
                }

                _dragOrigin = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
                return;
            }

            // 检测左键或右键松开
            bool leftMouseUp = useLeftMouseButton && Input.GetMouseButtonUp(0);
            bool rightMouseUp = useRightMouseButton && Input.GetMouseButtonUp(1);

            if (leftMouseUp || rightMouseUp)
            {
                _isDragging = false;
            }

            // 检测左键或右键按住并拖动
            bool leftMouseHeld = useLeftMouseButton && Input.GetMouseButton(0);
            bool rightMouseHeld = useRightMouseButton && Input.GetMouseButton(1);

            if (_isDragging && (leftMouseHeld || rightMouseHeld))
            {
                Vector3 currentPos = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 diff = _dragOrigin - currentPos;
                Vector3 newPosition = controlCamera.transform.position + diff;

                controlCamera.transform.position = ConstrainCameraPosition(newPosition);
                _dragOrigin = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                // TextEffectController.Instance.ShouldUpdateTargetPosition = true;
            }
        }


        public void RegisterCanDragEvent(Func<bool> canDragEvent)
        {
            canDragEvents.Add(canDragEvent);
        }


        /// <summary>
        /// 只要有一个事件返回false,则不可以拖拽
        /// </summary>
        /// <returns></returns>
        public bool CanDrag()
        {
            bool canDrag = true;
            foreach (var canDragEvent in canDragEvents)
            {
                if (canDragEvent() == false)
                {
                    canDrag = false;
                    break;
                }
            }
            return canDrag;
        }


        /// <summary>
        /// 限制摄像机位置在边界内
        /// </summary>
        /// <param name="newPosition">摄像机新位置</param>
        /// <returns></returns>
        private Vector3 ConstrainCameraPosition(Vector3 newPosition)
        {

            // 计算正交相机的视野宽高
            float camHeight = controlCamera.orthographicSize * 2f;
            float camWidth = camHeight * controlCamera.aspect;

            float halfCamWidth = camWidth / 2f;
            float halfCamHeight = camHeight / 2f;
            // 限制摄像机位置在边界内
            if ((newPosition.x - halfCamWidth) < minX)
            {
                newPosition.x = minX + halfCamWidth;
            }

            if ((newPosition.x + halfCamWidth) > maxX)
            {
                newPosition.x = maxX - halfCamWidth;
            }

            if ((newPosition.y - halfCamHeight) < minY)
            {
                newPosition.y = minY + halfCamHeight;
            }

            if ((newPosition.y + halfCamHeight) > maxY)
            {
                newPosition.y = maxY - halfCamHeight;
            }

            return newPosition;
        }

        private void Zoom()
        {

            if (!IsEnableZoom || isCinematicMode)
                return;

            // 检测鼠标滚轮滚动
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                // 使用正交相机大小来实现缩放效果
                float orthographicSize = controlCamera.orthographicSize;
                orthographicSize -= scroll * zoomSpeed * 5f; // 增大缩放系数使效果更明显
                orthographicSize = Mathf.Clamp(orthographicSize, 2f, 20f); // 设置合适的缩放范围
                controlCamera.orthographicSize = orthographicSize;
                controlCamera.transform.position = ConstrainCameraPosition(controlCamera.transform.position);
                // TextEffectController.Instance.ShouldUpdateTargetPosition = true;
            }

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
            Gizmos.DrawWireCube(center, size);
        }

        private Camera GetMainCamera()
        {
            // 方式1：通过标签查找
            Camera cam = Camera.main;

            // 方式2：如果方式1失败，通过标签查找
            if (cam == null)
            {
                cam = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
            }

            // 方式3：如果方式2失败，查找所有摄像机
            if (cam == null)
            {
                cam = GameObject.FindObjectOfType<Camera>();
            }

            if (cam == null)
            {
                YanGF.Debug.LogError(nameof(CameraController), "场景中没有找到任何可用的摄像机");
            }

            return cam;
        }




        ///////////////////////////////公共方法/////////////////////////////////

        /// <summary>
        /// 设置摄像机的正交大小
        /// </summary>
        /// <param name="size"></param>
        public void SetCameraOrthographicSize(float size)
        {
            controlCamera.orthographicSize = size;
        }

        /// <summary>
        /// 设置摄像机的位置
        /// </summary>
        /// <param name="position"></param>
        public void SetCameraPosition(Vector3 position)
        {
            controlCamera.transform.position = position;
        }

        /// <summary>
        /// 使摄像机震动。
        /// </summary>
        /// <param name="duration">震动持续时间。</param>
        /// <param name="magnitude">震动幅度。</param>
        public void ShakeCamera(float duration, float magnitude)
        {
            StartCoroutine(Shake(duration, magnitude));
        }

        /// <summary>
        /// 摄像机震动的协程
        /// </summary>
        /// <param name="duration">震动持续时间</param>
        /// <param name="magnitude">震动幅度</param>
        /// <returns></returns>
        private IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPosition = controlCamera.transform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                controlCamera.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }

            controlCamera.transform.localPosition = originalPosition;
        }


        /// <summary>
        /// 移动摄像机到目标位置
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="duration">移动时间</param>
        /// <param name="curveType">移动曲线类型</param>
        /// <param name="onComplete">移动完成回调</param>
        public void MoveToTarget(Transform target, float duration, MoveCurveType curveType, Action onComplete = null)
        {
            EnterCinematicMode();
            StartCoroutine(MoveToTargetCoroutine(target, duration, curveType, onComplete));
        }





        private IEnumerator MoveToTargetCoroutine(Transform targetTransform, float duration, MoveCurveType curveType, Action onComplete = null)
        {
            Vector3 startPosition = controlCamera.transform.position;
            float startZ = startPosition.z; // 保持初始Z轴位置
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                switch (curveType)
                {
                    case MoveCurveType.Linear:
                        t = t; // Linear interpolation
                        break;
                    case MoveCurveType.SmoothStep:
                        t = Mathf.SmoothStep(0f, 1f, t);
                        break;
                }

                Vector3 newPosition = Vector3.Lerp(startPosition, targetTransform.position, t);
                newPosition.z = startZ; // 保持Z轴不变
                controlCamera.transform.position = newPosition;
                yield return null;
            }

            controlCamera.transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, startZ); // 确保最终位置的Z轴不变
            onComplete?.Invoke();
        }


        /// <summary>
        /// 进入摄像机的电影模式
        /// </summary>
        public void EnterCinematicMode()
        {
            isCinematicMode = true;
        }


        /// <summary>
        /// 退出摄像机的电影模式
        /// </summary>
        public void ExitCinematicMode()
        {
            isCinematicMode = false;
        }


        /// <summary>
        /// 缩放摄像机到目标大小
        /// </summary>
        /// <param name="targetSize">目标大小</param>
        /// <param name="duration">缩放时间</param>
        /// <param name="onComplete">缩放完成回调</param>


        // 兼容老接口
        public void ZoomToTarget(float targetSize, float duration, Action onComplete = null)
        {
            StartCoroutine(ZoomToTargetCoroutine(targetSize, duration, onComplete));
        }

        private IEnumerator ZoomToTargetCoroutine(float targetSize, float duration, Action onComplete)
        {
            if (controlCamera == null)
            {
                Debug.LogError("controlCamera未设置！");
                yield break;
            }

            float startSize = controlCamera.orthographic ? controlCamera.orthographicSize : controlCamera.fieldOfView;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // 可根据curveType自定义插值方式，这里暂用线性插值
                if (controlCamera.orthographic)
                    controlCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                else
                    controlCamera.fieldOfView = Mathf.Lerp(startSize, targetSize, t);

                yield return null;
            }

            if (controlCamera.orthographic)
                controlCamera.orthographicSize = targetSize;
            else
                controlCamera.fieldOfView = targetSize;

            onComplete?.Invoke();
        }
    }

}
