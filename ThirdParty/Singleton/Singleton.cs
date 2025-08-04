namespace FrameWork
{
    /// <summary>
    /// 缓存单例,避免每次调用SingletonObjects.GetInstance时的消耗
    /// </summary>
    public sealed class Singleton<T> where T : class
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (SingletonObjects.TryGetInstanceContainer<T>(out var container))
                    {
                        container.OnDispose += () =>
                        {
                            Singleton<T>._instance = null;
                        };
                        _instance = container.Instance as T;
                    }
                }

                return _instance;
            }
        }
    }
}
