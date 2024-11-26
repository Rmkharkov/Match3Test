using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard
{
    #region Variables

    private int height = 0;
    public int Height { get { return height; } }

    private int width = 0;
    public int Width { get { return width; } }
  
    private SC_Gem[,] allGems;
    public SC_Gem[,] AllGems { get { return allGems; } }

    private int score = 0;
    public int Score 
    {
        get { return score; }
        set { score = value; }
    }

    private List<List<SC_Gem>> currentMatches = new List<List<SC_Gem>>();
    public List<List<SC_Gem>> CurrentMatches { get { return currentMatches; } }
    #endregion

    public GameBoard(int _Width, int _Height)
    {
        height = _Height;
        width = _Width;
        allGems = new SC_Gem[width, height];
    }

    public List<GlobalEnums.GemType> NecessaryGemTypesAt(Vector2Int _PositionToCheck)
    {
        List<GlobalEnums.GemType> toReturn = new List<GlobalEnums.GemType>();
        for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        {
            GlobalEnums.GemType gemType = (GlobalEnums.GemType)i;
            if (gemType == GlobalEnums.GemType.bomb) continue;
            
            var horizontal = MatchHorizontal(_PositionToCheck, gemType);
            var vertical = MatchVertical(_PositionToCheck, gemType);
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
                if (setType != GlobalEnums.GemType.bomb)
                {
                    toReturn.Add(setType);
                }
            }
        }

        return toReturn;
    }

    private List<SC_Gem> MatchHorizontal(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType)
    {
        var toReturn = new List<SC_Gem>();
        toReturn.Add(allGems[_PositionToCheck.x, _PositionToCheck.y]);

        for (var i = _PositionToCheck.x - 1; i >= 0 && allGems[i, _PositionToCheck.y] != null && allGems[i, _PositionToCheck.y].type == _StoneType; i--)
        {
            toReturn.Add(allGems[i, _PositionToCheck.y]);
        }

        for (var i = _PositionToCheck.x + 1; i < Width && allGems[i, _PositionToCheck.y] != null && allGems[i, _PositionToCheck.y].type == _StoneType; i++)
        {
            toReturn.Add(allGems[i, _PositionToCheck.y]);
        }

        if (toReturn.Count < 3)
        {
            return null;
        }
        
        return toReturn;
    }

    private List<SC_Gem> MatchVertical(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType)
    {
        var toReturn = new List<SC_Gem>();
        toReturn.Add(allGems[_PositionToCheck.x, _PositionToCheck.y]);

        for (int i = _PositionToCheck.y - 1; i >= 0 && allGems[_PositionToCheck.x, i] != null && allGems[_PositionToCheck.x, i].type == _StoneType; i--)
        {
            toReturn.Add(allGems[_PositionToCheck.x, i]);
        }

        for (int i = _PositionToCheck.y + 1; i < Height && allGems[_PositionToCheck.x, i] != null && allGems[_PositionToCheck.x, i].type == _StoneType; i++)
        {
            toReturn.Add(allGems[_PositionToCheck.x, i]);
        }


        if (toReturn.Count < 3)
        {
            return null;
        }
        
        return toReturn;
    }

    public void SetGem(int _X, int _Y, SC_Gem _Gem)
    {
        allGems[_X, _Y] = _Gem;
    }
    public SC_Gem GetGem(int _X,int _Y)
    {
       return allGems[_X, _Y];
    }

    public void FindAllMatches(Vector2Int _RequestedFrom)
    {
        currentMatches.Clear();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                SC_Gem currentGem = allGems[x, y];
                if (currentGem != null && !currentMatches.Any(c => c.Contains(currentGem)) && currentGem.type != GlobalEnums.GemType.bomb)
                {
                    var horizontal = MatchHorizontal(new Vector2Int(x, y), currentGem.type);
                    var vertical = MatchVertical(new Vector2Int(x, y), currentGem.type);
                    if (horizontal != null)
                    {
                        currentMatches.Add(horizontal);
                    }
                    if (vertical != null)
                    {
                        currentMatches.Add(vertical);
                    }
                }
            }
        
        currentMatches.ForEach(c => c.ForEach(g => g.isMatch = true));
    }

    public void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            // SC_Gem gem = currentMatches[i];
            // int x = gem.posIndex.x;
            // int y = gem.posIndex.y;
            //
            // if (gem.posIndex.x > 0)
            // {
            //     if (allGems[x - 1, y] != null && allGems[x - 1, y].type == GlobalEnums.GemType.bomb)
            //         MarkBombArea(new Vector2Int(x - 1, y), allGems[x - 1, y].blastSize);
            // }
            //
            // if (gem.posIndex.x + 1 < width)
            // {
            //     if (allGems[x + 1, y] != null && allGems[x + 1, y].type == GlobalEnums.GemType.bomb)
            //         MarkBombArea(new Vector2Int(x + 1, y), allGems[x + 1, y].blastSize);
            // }
            //
            // if (gem.posIndex.y > 0)
            // {
            //     if (allGems[x, y - 1] != null && allGems[x, y - 1].type == GlobalEnums.GemType.bomb)
            //         MarkBombArea(new Vector2Int(x, y - 1), allGems[x, y - 1].blastSize);
            // }
            //
            // if (gem.posIndex.y + 1 < height)
            // {
            //     if (allGems[x, y + 1] != null && allGems[x, y + 1].type == GlobalEnums.GemType.bomb)
            //         MarkBombArea(new Vector2Int(x, y + 1), allGems[x, y + 1].blastSize);
            // }
        }
    }

    private void MarkBombArea(Vector2Int bombPos, int _BlastSize)
    {
        string _print = "";
        for (int x = bombPos.x - _BlastSize; x <= bombPos.x + _BlastSize; x++)
        {
            for (int y = bombPos.y - _BlastSize; y <= bombPos.y + _BlastSize; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (allGems[x, y] != null)
                    {
                        _print += "(" + x + "," + y + ")" + System.Environment.NewLine;
                        allGems[x, y].isMatch = true;
                        // currentMatches.Add(allGems[x, y]);
                    }
                }
            }
        }
        currentMatches = currentMatches.Distinct().ToList();
    }
}

