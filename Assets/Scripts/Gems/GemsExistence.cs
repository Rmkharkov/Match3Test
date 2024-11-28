using Core;
using UnityEngine;
namespace Gems
{
    public class GemsExistence : PresentedSingleton<GemsExistence>, IGemsExistence
    {
        [SerializeField] private Transform gemsParent;

        private GemsPool gemsPool;

        private void Start()
        {
            gemsPool = new GemsPool();
        }

        public Gem SpawnGem(Vector2Int _Position, int _StartYPosition, GlobalEnums.GemType _GemTypeToSpawn, bool _IsBomb)
        {
            Gem gem = gemsPool.Get(_GemTypeToSpawn, _IsBomb, new Vector3(_Position.x, _StartYPosition, 0f));
            gem.Transform.SetParent(gemsParent);
            gem.GemObject.name = "Gem - " + _Position.x + ", " + _Position.y;

            return gem;
        }

        public void RemoveGem(Gem _Gem)
        {
            gemsPool.Return(_Gem);
        }
    }
}
