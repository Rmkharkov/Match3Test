using UnityEngine;
namespace Gems
{
    public interface IGemsRandomizer
    {
        GlobalEnums.GemType SemiRandomGemTypeAtPosition(Vector2Int _Position);
    }
}