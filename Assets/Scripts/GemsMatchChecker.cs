using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GemsMatchChecker : MonoBoardSubscriber<GemsMatchChecker>, IGemsMatchChecker
{
    private GameBoardHolder GameBoard => GameBoardHolder.Instance;
    private Gem[,] AllGems => GameBoard.AllGems;

    public List<List<Gem>> CurrentMatches { get; private set; } = new List<List<Gem>>();

    protected override void OnChangedBoardState(EBoardState _State)
    {
        base.OnChangedBoardState(_State);
        if (_State == EBoardState.CheckingGemsMatch)
        {
            
        }
    }

    public List<Gem> MatchHorizontal(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType)
    {
        var toReturn = new List<Gem>();
        toReturn.Add(AllGems[_PositionToCheck.x, _PositionToCheck.y]);

        for (var i = _PositionToCheck.x - 1; i >= 0 && AllGems[i, _PositionToCheck.y] != null && AllGems[i, _PositionToCheck.y].Type == _StoneType; i--)
        {
            toReturn.Add(AllGems[i, _PositionToCheck.y]);
        }

        for (var i = _PositionToCheck.x + 1; i < GameBoard.Width && AllGems[i, _PositionToCheck.y] != null && AllGems[i, _PositionToCheck.y].Type == _StoneType; i++)
        {
            toReturn.Add(AllGems[i, _PositionToCheck.y]);
        }

        if (toReturn.Count < 3)
        {
            return null;
        }
        
        return toReturn;
    }

    public List<Gem> MatchVertical(Vector2Int _PositionToCheck, GlobalEnums.GemType _StoneType)
    {
        var toReturn = new List<Gem>();
        toReturn.Add(AllGems[_PositionToCheck.x, _PositionToCheck.y]);

        for (int i = _PositionToCheck.y - 1; i >= 0 && AllGems[_PositionToCheck.x, i] != null && AllGems[_PositionToCheck.x, i].Type == _StoneType; i--)
        {
            toReturn.Add(AllGems[_PositionToCheck.x, i]);
        }

        for (int i = _PositionToCheck.y + 1; i < GameBoard.Height && AllGems[_PositionToCheck.x, i] != null && AllGems[_PositionToCheck.x, i].Type == _StoneType; i++)
        {
            toReturn.Add(AllGems[_PositionToCheck.x, i]);
        }


        if (toReturn.Count < 3)
        {
            return null;
        }
        
        return toReturn;
    }

    private void FindAllMatches(Vector2Int _RequestedFrom)
    {
        CurrentMatches.Clear();

        for (int x = 0; x < GameBoard.Width; x++)
        for (int y = 0; y < GameBoard.Height; y++)
        {
            Gem currentGem = AllGems[x, y];
            if (currentGem != null && !CurrentMatches.Any(c => c.Contains(currentGem)))
            {
                var horizontal = MatchHorizontal(new Vector2Int(x, y), currentGem.Type);
                var vertical = MatchVertical(new Vector2Int(x, y), currentGem.Type);
                if (horizontal != null)
                {
                    CurrentMatches.Add(horizontal);
                }
                if (vertical != null)
                {
                    CurrentMatches.Add(vertical);
                }
            }
        }
    }
}