using System.Collections.Generic;
using Board;
using Core;
using UnityEngine;
using UnityEngine.Events;
namespace Gems
{
    public class GemsCombiner : MonoBoardSubscriber<GemsCombiner>, IGemsCombiner
    {
        private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
        private IGemsExistence UsedGemsExistence => GemsExistence.Instance;
        private IGemsMatchChecker MatchChecker => GemsMatchChecker.Instance;
        private List<List<Gem>> CurrentMatches => MatchChecker.CurrentMatches;

        public UnityEvent<bool> MatchesDestroyFinishedSuccess { get; } = new UnityEvent<bool>();
        public UnityEvent<Gem> DestroyMatchedGem { get; } = new UnityEvent<Gem>();
        public UnityEvent<Gem> SpawnedBombInsteadOfGem { get; } = new UnityEvent<Gem>();
        public UnityEvent<List<List<Gem>>> MatchedBombsAndGems { get; } = new UnityEvent<List<List<Gem>>>();

        protected override void SubscribeOnEvents()
        {
            base.SubscribeOnEvents();
            MatchChecker.MatchingFinishedEvent.AddListener(OnMatchingFinished);
        }

        protected override void UnSubscribeOnEvents()
        {
            base.UnSubscribeOnEvents();
            MatchChecker.MatchingFinishedEvent.RemoveListener(OnMatchingFinished);
        }

        private void OnMatchingFinished(Vector2Int _RequestedFrom)
        {
            var empty = CurrentMatches.Count == 0;
            if (!empty)
            {
                var anyBombInMatch = IsBombsInMatch();
                CheckMatchesToSpawnBombs(_RequestedFrom);
                DestroyMatches();
                if (anyBombInMatch)
                {
                    MatchedBombsAndGems.Invoke(CurrentMatches);
                    return;
                }
            }
            MatchesDestroyFinishedSuccess.Invoke(!empty);
        }

        private bool IsBombsInMatch()
        {
            foreach (var currentMatch in CurrentMatches)
            {
                for (var i = currentMatch.Count - 1; i >= 0; i--)
                {
                    if (currentMatch[i].IsBomb)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckMatchesToSpawnBombs(Vector2Int _RequestedFrom)
        {
            var matches = CurrentMatches;
            foreach (var gems in matches)
            {
                var normalGemsInMatch = GemsInMatchExcludeBombs(gems);
                if (normalGemsInMatch.Count < 4) continue;
                var position = _RequestedFrom;
                if (_RequestedFrom == -Vector2Int.one)
                {
                    var gem = normalGemsInMatch[UnityEngine.Random.Range(0, normalGemsInMatch.Count)];
                    position = GemsMoving.BoardPositionByInput(gem.Transform.position);
                }
                RemoveGemFromAllMatches(BoardHolder.GetGem(position.x, position.y));
                ReplaceGemWithBombAt(position);
            }
        }

        private List<Gem> GemsInMatchExcludeBombs(List<Gem> _GemsMatch)
        {
            var toReturn = new List<Gem>();
            _GemsMatch.ForEach(_G =>
            {
                if (!_G.IsBomb)
                {
                    toReturn.Add(_G);
                }
                else if (toReturn.Count < 4)
                {
                    toReturn.Clear();
                }
            });

            return toReturn;
        }

        private void RemoveGemFromAllMatches(Gem _Gem)
        {
            CurrentMatches.ForEach(_C =>
            {
                if (_C.Contains(_Gem))
                {
                    _C.Remove(_Gem);
                }
            });
        }

        private void DestroyMatches()
        {
            var matches = CurrentMatches;

            foreach (List<Gem> match in matches)
            {
                foreach (Gem gem in match)
                {
                    if (!gem.IsBomb)
                    {
                        DestroyGem(gem);
                    }
                }
            }
        }

        public void DestroyGem(Gem _Gem)
        {
            if (_Gem == null) return;

            BoardHolder.RemoveGemAt(_Gem.Transform.position);
            DestroyMatchedGem.Invoke(_Gem);
            UsedGemsExistence.RemoveGem(_Gem);
        }

        private void ReplaceGemWithBombAt(Vector2Int _Position)
        {
            var gem = BoardHolder.GetGem(_Position.x, _Position.y);
            if (gem == null) return;
        
            var gemType = gem.Type;
            BoardHolder.RemoveGemAt(gem.Transform.position);
            SpawnedBombInsteadOfGem.Invoke(gem);
            UsedGemsExistence.RemoveGem(gem);

            var newGem = UsedGemsExistence.SpawnGem(_Position, _Position.y, gemType, true);
            BoardHolder.SetGem(_Position.x, _Position.y, newGem);
        }
    }
}
