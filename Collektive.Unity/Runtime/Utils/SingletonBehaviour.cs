using UnityEngine;

namespace Collektive.Unity.Utils
{
    /// <summary>
    /// Singleton monobehaviour. Allows only one intance in the scene and it becomes DontDestroyOnLoad
    /// </summary>
    public abstract class SingletonBehaviour<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Object.FindAnyObjectByType(typeof(T));
                        if (_instance == null)
                        {
                            var singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                    return _instance;
                }
            }
        }
    }
}
