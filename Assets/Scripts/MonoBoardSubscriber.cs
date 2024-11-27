﻿using UnityEngine;
public class MonoBoardSubscriber<T> : PresentedSingleton<T> where T : MonoBehaviour
{
    private IGameBoardController BoardController => GameBoardController.Instance;

    private void OnEnable() => SubscribeOnEvents();

    private void OnDisable() => UnSubscribeOnEvents();

    protected virtual void SubscribeOnEvents()
    {
        BoardController.ChangedBoardState.AddListener(OnChangedBoardState);
    }

    protected virtual void UnSubscribeOnEvents()
    {
        BoardController.ChangedBoardState.RemoveListener(OnChangedBoardState);
    }

    protected virtual void OnChangedBoardState(EBoardState _State) { }
}