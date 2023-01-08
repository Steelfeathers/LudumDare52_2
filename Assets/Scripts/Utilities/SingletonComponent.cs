using UnityEngine;

namespace FirebirdGames.Utilities
{
    public class SingletonComponent<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this as T)
            {
                GameObject.Destroy(this);
                return;
            }

            Instance = this as T;
        }
    }
}
