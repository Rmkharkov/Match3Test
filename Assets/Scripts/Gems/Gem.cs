using UnityEngine;
namespace Gems
{
    public class Gem
    {
        public GlobalEnums.GemType Type { get; private set; }
        public GameObject GemObject { get; private set; }
        public bool IsBomb { get; private set; }
        public Transform Transform => GemObject?.transform;

        public Gem(GlobalEnums.GemType _Type, GameObject _GameObject, bool _IsBomb)
        {
            Type = _Type;
            GemObject = _GameObject;
            IsBomb = _IsBomb;
        }
    }
}