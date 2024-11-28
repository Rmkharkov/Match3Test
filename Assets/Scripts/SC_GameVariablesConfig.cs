using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SC_GameVariablesConfig", menuName = "Configs/SC_GameVariablesConfig", order = 0)]
public class SC_GameVariablesConfig : ScriptableObjectSingleton<SC_GameVariablesConfig>
{
    public GameObject bgTilePrefabs;
    [SerializeField] private GemData[] gems;
    [SerializeField] private GemData[] bombs;
    [SerializeField] private GemData[] effects;
    public float bonusAmount = 0.5f;
    public float bombChance = 2f;
    public int dropHeight = 0;
    public float gemSpeed;
    public float scoreSpeed = 5;
    public int maxPoolSize = 100;
    public int scoreByGem = 10;
    public float delayBeforeStart = 0.1f;
    public float delayBeforeDestroyBombNeighbors = 0.6f;
    public float delayBeforeDestroyBombAfterNeighbors = 1f;
    
    [HideInInspector]
    public int rowsSize = 7;
    [HideInInspector]
    public int colsSize = 7;

    public GameObject GemPrefabByType(GlobalEnums.GemType _GemType, bool _IsBomb)
    {
        if (_IsBomb)
        {
            return bombs.FirstOrDefault(_C => _C.GemType == _GemType).Prefab;
        }
        return gems.FirstOrDefault(_C => _C.GemType == _GemType).Prefab;
    }

    public GameObject EffectPrefabByType(GlobalEnums.GemType _GemType)
    {
        return effects.FirstOrDefault(_C => _C.GemType == _GemType).Prefab;
    }
    
    [System.Serializable]
    private struct GemData
    {
        [SerializeField] private GlobalEnums.GemType gemType;
        public GlobalEnums.GemType GemType => gemType;
        [SerializeField] private GameObject prefab;
        public GameObject Prefab => prefab;
    }
}