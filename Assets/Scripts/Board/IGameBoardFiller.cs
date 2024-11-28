using Gems;
using UnityEngine;
using UnityEngine.Events;
namespace Board
{
    public interface IGameBoardFiller
    {
        UnityEvent BoardFillingFinishedEvent { get; }
        UnityEvent<Gem, Vector2Int> GemSpawnedAtEvent { get; }
    }
}