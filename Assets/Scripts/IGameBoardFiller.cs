using UnityEngine;
using UnityEngine.Events;
public interface IGameBoardFiller
{
    UnityEvent BoardFillingFinishedEvent { get; }
    UnityEvent<Gem, Vector2Int> GemSpawnedAtEvent { get; }
}