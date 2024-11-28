using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class GemsRandomizer : Instantiable<GemsRandomizer>, IGemsRandomizer
{
    private IGemsMatchChecker MatchChecker => GemsMatchChecker.Instance;

    public GlobalEnums.GemType SemiRandomGemTypeAtPosition(Vector2Int _Position)
    {
        List<GlobalEnums.GemType> possibleGems = NecessaryGemTypesAt(new Vector2Int(_Position.x, _Position.y));
        return possibleGems[Random.Range(0, possibleGems.Count)];
    }

    private List<GlobalEnums.GemType> NecessaryGemTypesAt(Vector2Int _PositionToCheck)
    {
        List<GlobalEnums.GemType> toReturn = new List<GlobalEnums.GemType>();
        for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        {
            GlobalEnums.GemType gemType = (GlobalEnums.GemType)i;

            var horizontal = MatchChecker.MatchHorizontal(_PositionToCheck, gemType);
            var vertical = MatchChecker.MatchVertical(_PositionToCheck, gemType);
            if (horizontal == null && vertical == null)
            {
                toReturn.Add(gemType);
            }
        }

        if (toReturn.Count == 0)
        {
            for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
            {
                GlobalEnums.GemType setType = (GlobalEnums.GemType)i;
                toReturn.Add(setType);
            }
        }

        return toReturn;
    }
}
