using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Gems;
using UnityEngine;
using UnityEngine.Events;
namespace Board
{
    public class GameBoardFiller : MonoBoardSubscriber<GameBoardFiller>, IGameBoardFiller
    {
        private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
        private IGemsRandomizer UsedGemsRandomizer => GemsRandomizer.Instance;
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
        private IGemsExistence UsedGemsExistence => GemsExistence.Instance;

        public UnityEvent BoardFillingFinishedEvent { get; } = new UnityEvent();
        public UnityEvent<Gem, Vector2Int> GemSpawnedAtEvent { get; } = new UnityEvent<Gem, Vector2Int>();

        protected override void OnChangedBoardState(EBoardState _State)
        {
            base.OnChangedBoardState(_State);
            if (_State == EBoardState.WaitForFill)
            {
                StartCoroutine(FillBoard());
            }
        }

        private IEnumerator FillBoard()
        {
            var coroutines = new List<Coroutine>();
            for (int x = 0; x < BoardHolder.Width; x++)
            {
                coroutines.Add(StartCoroutine(FillColumn(x)));
            }
            
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }

            BoardFillingFinishedEvent.Invoke();
        }

        private IEnumerator FillColumn(int _ID)
        {
            var gemsCounter = 0;
            for (int y = 0; y < BoardHolder.Height; y++)
            {
                Gem curGem = BoardHolder.GetGem(_ID, y);
                if (curGem == null)
                {
                    var pos = new Vector2Int(_ID, y);
                    var gemType = UsedGemsRandomizer.SemiRandomGemTypeAtPosition(new Vector2Int(_ID, y));
                    var gem = UsedGemsExistence.SpawnGem(pos, BoardHolder.Height, gemType, false);
                    BoardHolder.SetGem(_ID, y, gem);
                    GemSpawnedAtEvent.Invoke(gem, pos);
                    var delayInSec = GameVariables.baseFallDelay - GameVariables.decFallDelay * gemsCounter;
                    yield return new WaitForSecondsRealtime(delayInSec);
                    gemsCounter++;
                }
            }
        }
    }
}
