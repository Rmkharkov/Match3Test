using UnityEngine;

[CreateAssetMenu(fileName = "SC_GameVariablesConfig", menuName = "Configs/SC_GameVariablesConfig", order = 0)]
public class SC_GameVariablesConfig : ScriptableObject
{
    public GameObject bgTilePrefabs;
    public SC_Gem bomb;
    public SC_Gem[] gems;
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
        
        #region Singleton
        
    public static SC_GameVariablesConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SC_GameVariablesConfig>("SC_GameVariablesConfig");
            }

            return instance;
        }
    }
    private static SC_GameVariablesConfig instance;
        
        #endregion
}