using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class GemsPool
{
    private readonly Dictionary<GlobalEnums.GemType, Stack<SC_Gem>> poolOld;
    private readonly Dictionary<GlobalEnums.GemType, Stack<Gem>> pool;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();

    public GemsPool()
    {
        pool = new Dictionary<GlobalEnums.GemType, Stack<Gem>>();
        poolOld = new Dictionary<GlobalEnums.GemType, Stack<SC_Gem>>();
        for (var i = 0; i < Enum.GetNames(typeof(GlobalEnums.GemType)).Length; i++)
        {
            poolOld.TryAdd((GlobalEnums.GemType)i, new Stack<SC_Gem>(GameVariables.maxPoolSize));
            pool.TryAdd((GlobalEnums.GemType)i, new Stack<Gem>(GameVariables.maxPoolSize));
        }
    }

    public Gem Get(GlobalEnums.GemType _GemType, bool _IsBomb, Vector3 _Position)
    {
        var toReturn = pool[_GemType].Count > 0 ? pool[_GemType].Pop() : null;

        if (toReturn == null)
        {
            var spawnGem = Object.Instantiate(GemPrefabByType(_GemType, _IsBomb));
            toReturn = new Gem(_GemType, spawnGem, _IsBomb);
        }
        
        toReturn.Transform.position = _Position;
        toReturn.GemObject.SetActive(true);
        return toReturn;
    }

    public SC_Gem GetOld(GlobalEnums.GemType _GemType, Vector3 _Position)
    {
        var toReturn = poolOld[_GemType].Count > 0 ? poolOld[_GemType].Pop() : 
            Object.Instantiate(GemPrefabByTypeOld(_GemType));
        
        toReturn.transform.position = _Position;
        toReturn.gameObject.SetActive(true);
        toReturn.isMatch = false;
        return toReturn;
    }

    public void Return(Gem _Gem)
    {
        if (pool[_Gem.Type].Count < GameVariables.maxPoolSize)
        {
            pool[_Gem.Type].Push(_Gem);
            _Gem.GemObject.SetActive(false);
        }
        else
        {
            Object.Destroy(_Gem.GemObject);
        }
    }

    public void ReturnOld(SC_Gem _Gem)
    {
        if (poolOld[_Gem.type].Count < GameVariables.maxPoolSize)
        {
            poolOld[_Gem.type].Push(_Gem);
            _Gem.gameObject.SetActive(false);
        }
        else
        {
            Object.Destroy(_Gem);
        }
    }

    private SC_Gem GemPrefabByTypeOld(GlobalEnums.GemType _GemType)
    {
        switch (_GemType)
        {
            case GlobalEnums.GemType.bomb:
                return GameVariables.bomb;
            default:
                return GameVariables.gemsOld.FirstOrDefault(c => c.type == _GemType);
        }
    }

    private GameObject GemPrefabByType(GlobalEnums.GemType _GemType, bool _IsBomb)
    {
        return GameVariables.GemPrefabByType(_GemType, _IsBomb);
    }
}
