using UnityEngine;
namespace Core
{
    public class PresentedSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        private void Awake()
        {
            Instance = gameObject.GetComponent<T>();
        }
    }
}