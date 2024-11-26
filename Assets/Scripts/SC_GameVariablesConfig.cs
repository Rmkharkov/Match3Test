using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SC_GameVariablesConfig", menuName = "Configs/SC_GameVariablesConfig", order = 0)]
public class SC_GameVariablesConfig : ScriptableObjectSingleton<SC_GameVariablesConfig>
{
    public GameObject bgTilePrefabs;
    public SC_Gem bomb;
    [FormerlySerializedAs("gems")] public SC_Gem[] gemsOld;
    [SerializeField] private GemData[] gems;
    [SerializeField] private GemData[] bombs;
    public float bonusAmount = 0.5f;
    public float bombChance = 2f;
    public int dropHeight = 0;
    public float gemSpeed;
    public float scoreSpeed = 5;
    public int maxPoolSize = 100;
    
    [HideInInspector]
    public int rowsSize = 7;
    [HideInInspector]
    public int colsSize = 7;

    public GameObject GemPrefabByType(GlobalEnums.GemType _GemType, bool _IsBomb)
    {
        if (_IsBomb)
        {
            return bombs.FirstOrDefault(c => c.GemType == _GemType).Prefab;
        }
        return gems.FirstOrDefault(c => c.GemType == _GemType).Prefab;
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