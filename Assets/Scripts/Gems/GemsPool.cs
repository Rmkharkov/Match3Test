using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gems
{
    public class GemsPool
    {
        private readonly Dictionary<GlobalEnums.GemType, Stack<Gem>> pool;
        private readonly Dictionary<GlobalEnums.GemType, Stack<Gem>> poolBombs;
        private readonly List<Gem> live = new List<Gem>();
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();

        public GemsPool()
        {
            pool = new Dictionary<GlobalEnums.GemType, Stack<Gem>>();
            poolBombs = new Dictionary<GlobalEnums.GemType, Stack<Gem>>();
            for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
            {
                pool.TryAdd((GlobalEnums.GemType)i, new Stack<Gem>(GameVariables.maxPoolSize));
                poolBombs.TryAdd((GlobalEnums.GemType)i, new Stack<Gem>(GameVariables.maxPoolSize));
            }
        }

        public Gem Get(GlobalEnums.GemType _GemType, bool _IsBomb, Vector3 _Position)
        {
            var toReturn = !_IsBomb ? 
                pool[_GemType].Count > 0 ? pool[_GemType].Pop() : null : 
                poolBombs[_GemType].Count > 0 ? poolBombs[_GemType].Pop() : null;
        
            if (toReturn == null || live.Contains(toReturn))
            {
                var spawnGem = Object.Instantiate(GemPrefabByType(_GemType, _IsBomb));
                toReturn = new Gem(_GemType, spawnGem, _IsBomb);
            }
        
            toReturn.Transform.position = _Position;
            toReturn.GemObject.SetActive(true);
            live.Add(toReturn);
            return toReturn;
        }

        public void Return(Gem _Gem)
        {
            if (live.Contains(_Gem))
            {
                live.Remove(_Gem);
            }
            if (_Gem.IsBomb && poolBombs[_Gem.Type].Count < GameVariables.maxPoolSize)
            {
                poolBombs[_Gem.Type].Push(_Gem);
                _Gem.GemObject.SetActive(false);
            }
            else if (!_Gem.IsBomb && pool[_Gem.Type].Count < GameVariables.maxPoolSize)
            {
                pool[_Gem.Type].Push(_Gem);
                _Gem.GemObject.SetActive(false);
            }
            else
            {
                Object.Destroy(_Gem.GemObject);
            }
        }

        private GameObject GemPrefabByType(GlobalEnums.GemType _GemType, bool _IsBomb)
        {
            return GameVariables.GemPrefabByType(_GemType, _IsBomb);
        }
    }
}
