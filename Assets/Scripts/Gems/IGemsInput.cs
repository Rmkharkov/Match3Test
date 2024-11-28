using UnityEngine;
using UnityEngine.Events;
namespace Gems
{
    public interface IGemsInput
    {
        void OnGemStartInput(GameObject _GemObject);
        UnityEvent<Vector2, Vector2> SwitchGemsInputEvent { get; }
    }
}