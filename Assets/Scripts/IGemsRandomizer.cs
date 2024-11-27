using UnityEngine;
public interface IGemsRandomizer
{
    GlobalEnums.GemType SemiRandomGemTypeAtPosition(Vector2Int _Position);
}