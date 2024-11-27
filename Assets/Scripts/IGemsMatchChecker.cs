using System.Collections.Generic;
using UnityEngine;
public interface IGemsMatchChecker
{
    List<Gem> MatchHorizontal(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType);
    List<Gem> MatchVertical(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType);
}