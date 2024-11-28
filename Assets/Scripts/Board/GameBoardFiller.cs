using System.Collections;
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
            for (int y = 0; y < BoardHolder.Height; y++)
            {
                for (int x = 0; x < BoardHolder.Width; x++)
                {
                    Gem curGem = BoardHolder.GetGem(x,y);
                    if (curGem == null)
                    {
                        var pos = new Vector2Int(x, y);
                        var gemType = UsedGemsRandomizer.SemiRandomGemTypeAtPosition(new Vector2Int(x, y));
                        var gem = UsedGemsExistence.SpawnGem(pos, BoardHolder.Height, gemType, false);
                        BoardHolder.SetGem(x, y, gem);
                        GemSpawnedAtEvent.Invoke(gem, pos);
                    }
                }
                yield return new WaitForSeconds(1f/GameVariables.gemSpeed);
            }
        
            BoardFillingFinishedEvent.Invoke();
        }
    }
}
