using UnityEngine;
public interface IGemsExistence
{
    Gem SpawnGem(Vector2Int _Position, int _StartYPosition, GlobalEnums.GemType _GemTypeToSpawn, bool _IsBomb);
    void RemoveGem(Gem _Gem);
}