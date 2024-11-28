using UnityEngine.Events;
namespace Gems
{
    public interface IBombsMatching
    {
        UnityEvent BombsExplodingFinished { get; }
    }
}