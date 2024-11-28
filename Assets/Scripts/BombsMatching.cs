using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BombsMatching : MonoBoardSubscriber<BombsMatching>, IBombsMatching
{
    private IGemsMoving UsedGemsMoving => GemsMoving.Instance;
    private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
    private IGemsCombiner UsedGemsCombiner => GemsCombiner.Instance;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
    
    public UnityEvent BombsExplodingFinished { get; } = new UnityEvent();

    protected override void SubscribeOnEvents()
    {
        base.SubscribeOnEvents();
        UsedGemsMoving.MatchedBombs.AddListener(OnMatchingBombs);
        UsedGemsCombiner.MatchedBombsAndGems.AddListener(OnRegularMatchingWithBomb);
    }

    protected override void UnSubscribeOnEvents()
    {
        base.UnSubscribeOnEvents();
        UsedGemsMoving.MatchedBombs.RemoveListener(OnMatchingBombs);
        UsedGemsCombiner.MatchedBombsAndGems.RemoveListener(OnRegularMatchingWithBomb);
    }

    private void OnMatchingBombs(List<Vector2Int> _Coordinates)
    {
        var affectedCoords = CoordsToExplodeFromBombs(_Coordinates);
        StartCoroutine(BombsDestroyFlow(affectedCoords));
    }

    private void OnRegularMatchingWithBomb(List<List<Gem>> _MatchedBombsAndGems)
    {
        var affectedCoords = new Dictionary<Vector2Int, bool>();
        var bombsCoords = new List<Vector2Int>();
        foreach (var matchedBombsAndGem in _MatchedBombsAndGems)
        {
            foreach (var gem in matchedBombsAndGem)
            {
                var coords = GemsMoving.BoardPositionByInput(gem.Transform.position);
                affectedCoords.TryAdd(coords, gem.IsBomb);
                if (gem.IsBomb)
                {
                    bombsCoords.Add(coords);
                }
            }
        }
        
        foreach (var coordToExplodeFromBomb in CoordsToExplodeFromBombs(bombsCoords))
        {
            if (!affectedCoords.ContainsKey(coordToExplodeFromBomb.Key))
            {
                affectedCoords.Add(coordToExplodeFromBomb.Key, coordToExplodeFromBomb.Value);
            }
        }
        
        StartCoroutine(BombsDestroyFlow(affectedCoords));
    }

    private IEnumerator BombsDestroyFlow(Dictionary<Vector2Int, bool> _ToExplode)
    {
        yield return new WaitForSeconds(GameVariables.delayBeforeDestroyBombNeighbors);
        foreach (var affected in _ToExplode)
        {
            if (!affected.Value)
            {
                UsedGemsCombiner.DestroyGem(BoardHolder.GetGem(affected.Key.x, affected.Key.y));
            }
        }
        yield return new WaitForSeconds(GameVariables.delayBeforeDestroyBombAfterNeighbors);
        foreach (var affected in _ToExplode)
        {
            if (affected.Value)
            {
                UsedGemsCombiner.DestroyGem(BoardHolder.GetGem(affected.Key.x, affected.Key.y));
            }
        }
        
        BombsExplodingFinished.Invoke();
    }

    private Dictionary<Vector2Int, bool> CoordsToExplodeFromBombs(List<Vector2Int> _BombsCoords)
    {
        var toReturn = new Dictionary<Vector2Int, bool>();
        foreach (var bombCoord in _BombsCoords)
        {
            var coords = GetSurroundingCoordinates(bombCoord);
            coords.ForEach(_C => toReturn.TryAdd(_C, _BombsCoords.Contains(_C) || BoardHolder.GetGem(_C.x, _C.y).IsBomb));
        }

        return toReturn;
    }
    
    private List<Vector2Int> GetSurroundingCoordinates(Vector2Int _Center)
    {
        var toReturn = new List<Vector2Int>();

        for (var x = 0; x < BoardHolder.Width; x++)
        {
            for (var y = 0; y < BoardHolder.Height; y++)
            {
                var distance = Mathf.Abs(x - _Center.x) + Mathf.Abs(y - _Center.y);

                if (distance is > 0 and <= 2)
                {
                    toReturn.Add(new Vector2Int(x, y));
                }
            }
        }

        return toReturn;
    }
}