﻿using System.Collections.Generic;
using UnityEngine;
public class GemsRandomizer : Instantiable<GemsRandomizer>
{
    public GlobalEnums.GemType SemiRandomGemTypeAtPosition(Vector2Int _Position)
    {
        List<GlobalEnums.GemType> possibleGems = NecessaryGemTypesAt(new Vector2Int(_Position.x, _Position.y));
        return possibleGems[Random.Range(0, possibleGems.Count)];
    }

    private List<GlobalEnums.GemType> NecessaryGemTypesAt(Vector2Int _PositionToCheck)
    {
        List<GlobalEnums.GemType> toReturn = new List<GlobalEnums.GemType>();
        // for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        // {
        //     GlobalEnums.GemType gemType = (GlobalEnums.GemType)i;
        //     if (gemType == GlobalEnums.GemType.bomb) continue;
        //     
        //     var horizontal = MatchHorizontal(_PositionToCheck, gemType);
        //     var vertical = MatchVertical(_PositionToCheck, gemType);
        //     if (horizontal == null && vertical == null)
        //     {
        //         toReturn.Add(gemType);
        //     }
        // }
        //
        // if (toReturn.Count == 0)
        // {
        //     for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        //     {
        //         GlobalEnums.GemType setType = (GlobalEnums.GemType)i;
        //         if (setType != GlobalEnums.GemType.bomb)
        //         {
        //             toReturn.Add(setType);
        //         }
        //     }
        // }

        return toReturn;
    }
}
