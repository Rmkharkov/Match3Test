using System;
using System.Threading.Tasks;
using Core;
using Gems;
using UnityEngine.Events;
namespace Board
{
    public class GameBoardController : PresentedSingleton<GameBoardController>, IGameBoardController
    {
        private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
        private IGemsCombiner UsedGemsCombiner => GemsCombiner.Instance;
        private IGemsMoving UsedGemsMoving => GemsMoving.Instance;
        private IBombsMatching UsedBombsMatching => BombsMatching.Instance;
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
        public UnityEvent<EBoardState> ChangedBoardState { get; } = new UnityEvent<EBoardState>();

        private async void Start()
        {
            BoardHolder.SetupBoard(GameVariables.colsSize, GameVariables.rowsSize);
            await Task.Delay(TimeSpan.FromSeconds(GameVariables.delayBeforeStart));
            ChangedBoardState.Invoke(EBoardState.WaitForFill);
        }

        private void OnEnable() => SubscribeOnEvents();
        private void OnDisable() => UnSubscribeOnEvents();

        private void SubscribeOnEvents()
        {
            UsedGemsCombiner.MatchesDestroyFinishedSuccess.AddListener(OnDestroyMatchedGems);
            UsedGemsMoving.GemsFallAfterDestroyFinished.AddListener(OnFallAfterDestroyFinished);
            UsedBombsMatching.BombsExplodingFinished.AddListener(OnDestroyBombs);
        }

        private void UnSubscribeOnEvents()
        {
            UsedGemsCombiner.MatchesDestroyFinishedSuccess.RemoveListener(OnDestroyMatchedGems);
            UsedGemsMoving.GemsFallAfterDestroyFinished.RemoveListener(OnFallAfterDestroyFinished);
            UsedBombsMatching.BombsExplodingFinished.RemoveListener(OnDestroyBombs);
        }

        private void OnDestroyBombs()
        {
            OnDestroyMatchedGems(true);
        }

        private void OnDestroyMatchedGems(bool _WasntEmpty)
        {
            ChangedBoardState.Invoke(_WasntEmpty ? EBoardState.AfterDestroy : EBoardState.Default);
        }

        private void OnFallAfterDestroyFinished()
        {
            ChangedBoardState.Invoke(EBoardState.WaitForFill);
        }
    }
}
