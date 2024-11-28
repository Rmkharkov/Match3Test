using UnityEngine;
using UnityEngine.Events;
public interface IGemsMoving
{
    UnityEvent<Vector2Int> GemsMovingFinished { get; }
    UnityEvent GemsFallAfterDestroyFinished { get; }
}