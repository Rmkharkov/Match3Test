using System.Collections.Generic;
using UnityEngine.Events;
namespace Gems
{
    public interface IGemsCombiner
    {
        UnityEvent<bool> MatchesDestroyFinishedSuccess { get; }
        UnityEvent<Gem> DestroyMatchedGem { get; }
        UnityEvent<Gem> SpawnedBombInsteadOfGem { get; }
        UnityEvent<List<List<Gem>>> MatchedBombsAndGems { get; }
        void DestroyGem(Gem _Gem);
    }
}