using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GemsPool
{
    private readonly Dictionary<GlobalEnums.GemType, Stack<SC_Gem>> pool;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance;

    public GemsPool()
    {
        pool = new Dictionary<GlobalEnums.GemType, Stack<SC_Gem>>();
        for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        {
            pool.TryAdd((GlobalEnums.GemType)i, new Stack<SC_Gem>(GameVariables.maxPoolSize));
        }
    }

    public SC_Gem Get(GlobalEnums.GemType _GemType, Vector3 _Position)
    {
        var toReturn = pool[_GemType].Count > 0 ? pool[_GemType].Pop() : 
            Object.Instantiate(GemPrefabByType(_GemType), _Position, Quaternion.identity);
        toReturn.gameObject.SetActive(true);
        return toReturn;
    }

    public void Return(SC_Gem _Gem)
    {
        if (pool[_Gem.type].Count < GameVariables.maxPoolSize)
        {
            pool[_Gem.type].Push(_Gem);
            _Gem.gameObject.SetActive(false);
        }
        else
        {
            Object.Destroy(_Gem);
        }
    }

    private SC_Gem GemPrefabByType(GlobalEnums.GemType _GemType)
    {
        switch (_GemType)
        {
            case GlobalEnums.GemType.bomb:
                return GameVariables.bomb;
            default:
                return GameVariables.gems.FirstOrDefault(c => c.type == _GemType);
        }
    }
}
