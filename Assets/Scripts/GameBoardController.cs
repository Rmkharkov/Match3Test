using System;
using UnityEngine;
using UnityEngine.Events;
public class GameBoardController : PresentedSingleton<GameBoardController>, IGameBoardController
{
    private IGameBoardFiller BoardFiller => GameBoardFiller.Instance;
    public UnityEvent<EBoardState> ChangedBoardState { get; private set; } = new UnityEvent<EBoardState>();

    private void Start()
    {
        ChangedBoardState.Invoke(EBoardState.WaitForFill);
    }

    private void OnEnable() => SubscribeOnEvents();
    private void OnDisable() => UnSubscribeOnEvents();

    private void SubscribeOnEvents()
    {
        BoardFiller.BoardFillingFinishedEvent.AddListener(OnBoardFilled);
    }

    private void UnSubscribeOnEvents()
    {
        BoardFiller.BoardFillingFinishedEvent.RemoveListener(OnBoardFilled);
    }

    private void OnBoardFilled()
    {
        ChangedBoardState.Invoke(EBoardState.CheckingGemsMatch);
    }
}
