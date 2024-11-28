using UnityEngine.Events;
namespace Board
{
    public interface IGameBoardController
    {
        UnityEvent<EBoardState> ChangedBoardState { get; }
    }
}