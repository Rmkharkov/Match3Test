using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IGemsMoving
{
    UnityEvent<Vector2Int> GemsMovingFinished { get; }
    UnityEvent GemsFallAfterDestroyFinished { get; }
    UnityEvent<List<Vector2Int>> MatchedBombs { get; }
}