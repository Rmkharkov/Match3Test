using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class GemsCombiner : MonoBoardSubscriber<GemsCombiner>, IGemsCombiner
{
    private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
    private IGemsExistence UsedGemsExistence => GemsExistence.Instance;
    private IGemsMatchChecker MatchChecker => GemsMatchChecker.Instance;
    private List<List<Gem>> CurrentMatches => MatchChecker.CurrentMatches;

    public UnityEvent<bool> MatchesDestroyFinishedSuccess { get; } = new UnityEvent<bool>();
    public UnityEvent<Gem> DestroyMatchedGem { get; } = new UnityEvent<Gem>();
    public UnityEvent<Gem> SpawnedBombInsteadOfGem { get; } = new UnityEvent<Gem>();

    protected override async void SubscribeOnEvents()
    {
        base.SubscribeOnEvents();
        await Task.Yield();
        MatchChecker.MatchingFinishedEvent.AddListener(OnMatchingFinished);
    }

    protected override void UnSubscribeOnEvents()
    {
        base.UnSubscribeOnEvents();
        MatchChecker.MatchingFinishedEvent.RemoveListener(OnMatchingFinished);
    }

    private void OnMatchingFinished(Vector2Int _RequestedFrom)
    {
        CheckMatchesToSpawnBombs(_RequestedFrom);
        var empty = CurrentMatches.Count == 0;
        if (!empty)
        {
            DestroyMatches();
        }
        MatchesDestroyFinishedSuccess.Invoke(!empty);
    }
    
    private void CheckMatchesToSpawnBombs(Vector2Int _RequestedFrom)
    {
        var matches = CurrentMatches;
        foreach (var gems in matches)
        {
            if (gems.Count < 4) continue;
            var position = _RequestedFrom;
            if (_RequestedFrom == -Vector2Int.one)
            {
                var gem = gems[UnityEngine.Random.Range(0, gems.Count)];
                position = GemsMoving.BoardPositionByInput(gem.Transform.position);
            }
            ReplaceGemWithBombAt(position);
        }
    }

    private void DestroyMatches()
    {
        var matches = CurrentMatches;

        foreach (List<Gem> match in matches)
        {
            foreach (Gem gem in match)
            {
                if (gem == null) continue;
                BoardHolder.RemoveGemAt(gem.Transform.position);
                DestroyMatchedGem.Invoke(gem);
                UsedGemsExistence.RemoveGem(gem);
            }
        }
    }

    private void ReplaceGemWithBombAt(Vector2Int _Position)
    {
        var gem = BoardHolder.GetGem(_Position.x, _Position.y);
        if (gem != null)
        {
            BoardHolder.RemoveGemAt(gem.Transform.position);
            SpawnedBombInsteadOfGem.Invoke(gem);
            UsedGemsExistence.RemoveGem(gem);
        }
        UsedGemsExistence.SpawnGem(_Position, _Position.y, GlobalEnums.GemType.bomb, true);
    }
}
