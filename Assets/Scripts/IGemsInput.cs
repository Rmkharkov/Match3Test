using UnityEngine;
using UnityEngine.Events;
public interface IGemsInput
{
    void OnGemStartInput(GameObject _GemObject);
    UnityEvent<Vector2, Vector2> SwitchGemsInputEvent { get; }
}