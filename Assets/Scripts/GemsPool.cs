using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GemsPool
{
    private readonly Dictionary<GlobalEnums.GemType, Stack<Gem>> pool;
    private readonly Dictionary<GlobalEnums.GemType, Stack<Gem>> poolBombs;
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
        // var toReturn = !_IsBomb ? 
        //     pool[_GemType].Count > 0 ? pool[_GemType].Pop() : null : 
        //     poolBombs[_GemType].Count > 0 ? poolBombs[_GemType].Pop() : null;
        //
        // if (toReturn == null)
        // {
            var spawnGem = Object.Instantiate(GemPrefabByType(_GemType, _IsBomb));
            var toReturn = new Gem(_GemType, spawnGem, _IsBomb);
        // }
        
        toReturn.Transform.position = _Position;
        toReturn.GemObject.SetActive(true);
        return toReturn;
    }

    public void Return(Gem _Gem)
    {
        Object.Destroy(_Gem.GemObject);
        return;
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
