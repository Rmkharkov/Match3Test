using UnityEngine.Events;
public interface IGameBoardController
{
    UnityEvent<EBoardState> ChangedBoardState { get; }
}