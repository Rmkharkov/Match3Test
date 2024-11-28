using UnityEngine.Events;
public interface IGemsCombiner
{
    UnityEvent<bool> MatchesDestroyFinishedSuccess { get; }
    UnityEvent<Gem> DestroyMatchedGem { get; }
    UnityEvent<Gem> SpawnedBombInsteadOfGem { get; }
    void DestroyGem(Gem _Gem);
}