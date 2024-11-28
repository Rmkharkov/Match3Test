using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Gems
{
    public interface IGemsMatchChecker
    {
        List<Gem> MatchHorizontal(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType);
        List<Gem> MatchVertical(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType);
        bool AnyMatchAt(Vector2Int _Coordinate, GlobalEnums.GemType _StoneType);
        UnityEvent<Vector2Int> MatchingFinishedEvent { get; }
        List<List<Gem>> CurrentMatches { get; }
    }
}