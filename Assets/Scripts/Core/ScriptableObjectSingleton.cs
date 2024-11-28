using UnityEngine;
namespace Core
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T instance;

        public static T Instance()
        {
            if (instance == null)
            {
                instance = Resources.Load<T>(typeof(T).ToString());
            }
            
            return instance;
        }
    }
}