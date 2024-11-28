using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Board;
using Core;
using UnityEngine;
using UnityEngine.Events;
namespace Gems
{
    public class GemsMoving : MonoBoardSubscriber<GemsMoving>, IGemsMoving
    {
        private IGemsInput UsedGemsInput => GemsInput.Instance;
        private IGameBoardFiller BoardFiller => GameBoardFiller.Instance;
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
        private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
        private IGemsMatchChecker MatchChecker => GemsMatchChecker.Instance;

        private int movesInProgress;
        private Vector2Int moveRequestedFrom;

        public UnityEvent<Vector2Int> GemsMovingFinished { get; } = new UnityEvent<Vector2Int>();
        public UnityEvent<List<Vector2Int>> MatchedBombs { get; } = new UnityEvent<List<Vector2Int>>();
        public UnityEvent GemsFallAfterDestroyFinished { get; } = new UnityEvent();

        private IEnumerator MoveGemTo(Gem _Gem, Vector2Int _TargetPosition)
        {
            movesInProgress++;
            var targetPosition = new Vector3(_TargetPosition.x, _TargetPosition.y);
            while (_Gem.GemObject != null && Vector3.Distance(_Gem.Transform.position, targetPosition) > 0.01f)
            {
                _Gem.Transform.position = Vector2.Lerp(_Gem.Transform.position, _TargetPosition, GameVariables.gemSpeed * Time.deltaTime);
                yield return null;
            }
        
            if (_Gem.GemObject != null)
            {
                _Gem.Transform.position = new Vector3(_TargetPosition.x, _TargetPosition.y, 0);
                BoardHolder.SetGem(_TargetPosition.x, _TargetPosition.y, _Gem);
            }
            movesInProgress--;
        }

        protected override async void SubscribeOnEvents()
        {
            base.SubscribeOnEvents();
            await Task.Yield();
            UsedGemsInput.SwitchGemsInputEvent.AddListener(OnGemsInput);
            BoardFiller.GemSpawnedAtEvent.AddListener(OnGemSpawned);
            BoardFiller.BoardFillingFinishedEvent.AddListener(WaitForFinishMoving);
        }

        protected override void UnSubscribeOnEvents()
        {
            base.UnSubscribeOnEvents();
            UsedGemsInput.SwitchGemsInputEvent.RemoveListener(OnGemsInput);
            BoardFiller.GemSpawnedAtEvent.RemoveListener(OnGemSpawned);
            BoardFiller.BoardFillingFinishedEvent.RemoveListener(WaitForFinishMoving);
        }

        protected override void OnChangedBoardState(EBoardState _State)
        {
            base.OnChangedBoardState(_State);
            if (_State == EBoardState.AfterDestroy)
            {
                StartCoroutine(DecreaseRowCo());
            }
        }

        private void OnGemSpawned(Gem _Gem, Vector2Int _TargetPosition)
        {
            StartCoroutine(MoveGemTo(_Gem, _TargetPosition));
        }

        private void OnGemsInput(Vector2 _PositionOne, Vector2 _PositionTwo)
        {
            StartCoroutine(SwitchGems(_PositionOne, _PositionTwo));
        }

        private async void WaitForFinishMoving()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            while (movesInProgress > 0)
            {
                await Task.Yield();
            }
        
            GemsMovingFinished.Invoke(moveRequestedFrom);
            ClearRequestedPosition();
        }

        private void ClearRequestedPosition()
        {
            moveRequestedFrom = -Vector2Int.one;
        }

        private void SetRequestedPosition(Vector2Int _Value)
        {
            moveRequestedFrom = _Value;
        }

        private IEnumerator SwitchGems(Vector2 _PositionOne, Vector2 _PositionTwo)
        {
            var positionOne = BoardPositionByInput(_PositionOne);
            var positionTwo = BoardPositionByInput(_PositionTwo);
            if (!IsSwitchPositionsGood(positionOne, positionTwo))
            {
                yield break;
            }
            SetRequestedPosition(positionTwo);
            var gem1 = GetGemByPosition(positionOne);
            var gem2 = GetGemByPosition(positionTwo);
            StartCoroutine(MoveGemTo(gem1, positionTwo));
            yield return MoveGemTo(gem2, positionOne);
            if (gem1.IsBomb && gem2.IsBomb)
            {
                MatchedBombs.Invoke(new List<Vector2Int>{
                    positionOne, positionTwo
                });
                yield break;
            }

            if (!AnyMatchOnSwitch(positionOne, gem2, positionTwo, gem1))
            {
                StartCoroutine(MoveGemTo(gem2, positionTwo));
                yield return MoveGemTo(gem1, positionOne);
            }

            WaitForFinishMoving();
        }

        private bool IsSwitchPositionsGood(Vector2Int _Position1, Vector2Int _Position2)
        {
            var totalPositionsGood = _Position1.x >= 0 && _Position1.y >= 0 && _Position2.x >= 0 && _Position2.y >= 0;
            var relatedPositionsGood = Mathf.Abs(_Position1.x - _Position2.x) + Mathf.Abs(_Position1.y - _Position2.y) == 1;
            return totalPositionsGood && relatedPositionsGood;
        }

        private bool AnyMatchOnSwitch(Vector2Int _Coordinate1, Gem _Gem1, Vector2Int _Coordinate2, Gem _Gem2)
        {
            return MatchChecker.AnyMatchAt(_Coordinate1, _Gem1.Type) || MatchChecker.AnyMatchAt(_Coordinate2, _Gem2.Type);
        }
    
        private IEnumerator DecreaseRowCo()
        {
            yield return new WaitForSeconds(.2f);

            int nullCounter = 0;
            for (int x = 0; x < BoardHolder.Width; x++)
            {
                for (int y = 0; y < BoardHolder.Height; y++)
                {
                    Gem curGem = BoardHolder.GetGem(x, y);
                    if (curGem == null)
                    {
                        nullCounter++;
                    }
                    else if (nullCounter > 0)
                    {
                        BoardHolder.SetGem(x, y, null);
                        BoardHolder.SetGem(x, y - nullCounter, curGem);
                        StartCoroutine(MoveGemTo(curGem, new Vector2Int(x, y - nullCounter)));
                    }
                }
                nullCounter = 0;
            }

            GemsFallAfterDestroyFinished.Invoke();
        }

        public static Vector2Int BoardPositionByInput(Vector2 _InputPosition)
        {
            var x = Mathf.FloorToInt(_InputPosition.x + 0.5f);
            var y = Mathf.FloorToInt(_InputPosition.y + 0.5f);
            var toReturn = new Vector2Int(x, y);
            return toReturn;
        }

        private Gem GetGemByPosition(Vector2Int _Position) => BoardHolder.AllGems[_Position.x, _Position.y];
    }
}
