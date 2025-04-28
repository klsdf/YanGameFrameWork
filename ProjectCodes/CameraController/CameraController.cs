
using UnityEngine;
using YanGameFrameWork.Singleton;
namespace YanGameFrameWork.CameraController
{
    public class CameraController : Singleton<CameraController>
    {
        public Camera controlCamera;

        public bool IsEnableDarg = true;
        public bool IsEnableZoom = true;


        public float dragSpeed = 2.0f;
        private Vector3 _dragOrigin;
        private bool _isDragging = false;
        public float zoomSpeed = 2.0f;

        [Header("最小视野")]
        public float minFov = 15.0f;
        [Header("最大视野")]
        public float maxFov = 90.0f;
        [Header("最小X")]
        public float minX = -10.0f;
        [Header("最大X")]
        public float maxX = 10.0f;
        [Header("最小Y")]
        public float minY = -10.0f;
        [Header("最大Y")]
        public float maxY = 10.0f;


        void Start()
        {
            controlCamera ??= GetMainCamera();
        }

        void Update()
        {
            Drag();
            Zoom();
        }

        private void Drag()
        {
            if (!IsEnableDarg)
                return;

            // 检测鼠标右键按下
            if (Input.GetMouseButtonDown(1))
            {
                _dragOrigin = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                _isDragging = true;
                return;
            }

            // 检测鼠标右键松开
            if (Input.GetMouseButtonUp(1))
            {
                _isDragging = false;
            }

            // 检测鼠标右键按住并拖动
            if (_isDragging)
            {
                Vector3 currentPos = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 diff = _dragOrigin - currentPos;
                Vector3 newPosition = controlCamera.transform.position + diff;

                // 限制摄像机位置在边界内
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                controlCamera.transform.position = newPosition;
                _dragOrigin = controlCamera.ScreenToWorldPoint(Input.mousePosition);
                // TextEffectController.Instance.ShouldUpdateTargetPosition = true;
            }
        }


        private void Zoom()
        {

            if (!IsEnableZoom)
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
                // TextEffectController.Instance.ShouldUpdateTargetPosition = true;
            }

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float screenX = Screen.width;
            float screenY = Screen.height;


            //把screenX/2,screenY/2转为世界坐标
            // Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenX/2, screenY/2, 0));


            Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(maxX - minX, maxY - minY, 0));
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




    }

}