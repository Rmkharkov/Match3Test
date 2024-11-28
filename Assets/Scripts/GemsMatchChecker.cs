using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class GemsMatchChecker : MonoBoardSubscriber<GemsMatchChecker>, IGemsMatchChecker
{
    private IGameBoardHolder GameBoard => GameBoardHolder.Instance;
    private IGemsMoving UsedGemsMoving => GemsMoving.Instance;
    private Gem[,] AllGems => GameBoard.AllGems;

    public List<List<Gem>> CurrentMatches { get; } = new List<List<Gem>>();
    public UnityEvent<Vector2Int> MatchingFinishedEvent { get; } = new UnityEvent<Vector2Int>();

    protected override async void SubscribeOnEvents()
    {
        base.SubscribeOnEvents();
        await Task.Yield();
        UsedGemsMoving.GemsMovingFinished.AddListener(OnGemsFinishedMoving);
    }

    protected override void UnSubscribeOnEvents()
    {
        base.UnSubscribeOnEvents();
        UsedGemsMoving.GemsMovingFinished.RemoveListener(OnGemsFinishedMoving);
    }

    private void OnGemsFinishedMoving(Vector2Int _RequestedFrom)
    {
        FindAllMatches(_RequestedFrom);
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

    public bool AnyMatchAt(Vector2Int _Coordinate, GlobalEnums.GemType _StoneType)
    {
        return MatchHorizontal(_Coordinate, _StoneType) != null || MatchVertical(_Coordinate, _StoneType) != null;
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
        
        MatchingFinishedEvent.Invoke(_RequestedFrom);
    }
}