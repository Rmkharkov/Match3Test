using UnityEngine.Events;
public interface IGameBoardFiller
{
    UnityEvent BoardFillingFinishedEvent { get; }
}