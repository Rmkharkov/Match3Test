using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_GameLogic : MonoBehaviour
{
    private Dictionary<string, GameObject> unityObjects;
    private int score = 0;
    private float displayScore = 0;
    private GameBoard gameBoard;
    private GemsPool gemsPool;
    private GlobalEnums.GameState currentState = GlobalEnums.GameState.move;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
    public GlobalEnums.GameState CurrentState { get { return currentState; } }

    #region MonoBehaviour
    private void Awake()
    {
        gemsPool = new GemsPool();
        Init();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        displayScore = Mathf.Lerp(displayScore, gameBoard.Score, GameVariables.scoreSpeed * Time.deltaTime);
        unityObjects["Txt_Score"].GetComponent<TMPro.TextMeshProUGUI>().text = displayScore.ToString("0");
    }
    #endregion

    #region Logic
    private void Init()
    {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] _obj = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach (GameObject g in _obj)
            unityObjects.Add(g.name,g);

        gameBoard = new GameBoard(7, 7);
        Setup();
    }
    private void Setup()
    {
        for (int y = 0; y < gameBoard.Height; y++)
            for (int x = 0; x < gameBoard.Width; x++)
            {
                Vector2 _pos = new Vector2(x, y);
                GameObject _bgTile = Instantiate(GameVariables.bgTilePrefabs, _pos, Quaternion.identity);
                _bgTile.transform.SetParent(unityObjects["GemsHolder"].transform);
                _bgTile.name = "BG Tile - " + x + ", " + y;

                GlobalEnums.GemType gemType = SemiRandomGemTypeAtPosition(new Vector2Int(x, y));
                SpawnGem(new Vector2Int(x, y), gemType);
            }
        
        FindAllMatches(-Vector2Int.one);
        DestroyMatches();
    }

    private GlobalEnums.GemType SemiRandomGemTypeAtPosition(Vector2Int _Position)
    {
        List<GlobalEnums.GemType> possibleGems = gameBoard.NecessaryGemTypesAt(new Vector2Int(_Position.x, _Position.y));
        return possibleGems[Random.Range(0, possibleGems.Count)];
    }
    
    private void StartGame()
    {
        unityObjects["Txt_Score"].GetComponent<TextMeshProUGUI>().text = score.ToString("0");
    }
    private void SpawnGem(Vector2Int _Position, GlobalEnums.GemType _GemTypeToSpawn, bool _FromTop = true)
    {
        SC_Gem _gem = gemsPool.GetOld(_GemTypeToSpawn, new Vector3(_Position.x, _FromTop ? gameBoard.Height : _Position.y, 0f));
        _gem.transform.SetParent(unityObjects["GemsHolder"].transform);
        _gem.name = "Gem - " + _Position.x + ", " + _Position.y;
        gameBoard.SetGem(_Position.x,_Position.y, _gem);
        _gem.SetupGem(this,_Position);
    }
    public void SetGem(int _X,int _Y, SC_Gem _Gem)
    {
        gameBoard.SetGem(_X,_Y, _Gem);
    }
    public SC_Gem GetGem(int _X, int _Y)
    {
        return gameBoard.GetGem(_X, _Y);
    }
    public void SetState(GlobalEnums.GameState _CurrentState)
    {
        currentState = _CurrentState;
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < gameBoard.CurrentMatches.Count; i++)
        {
            if (gameBoard.CurrentMatches[i] != null)
            {
                gameBoard.CurrentMatches[i].ForEach(ScoreCheck);
            }
        }

        foreach (var gem in gameBoard.AllGems)
        {
            if (gem.isMatch)
            {
                DestroyMatchedGem(gem);
            }
        }
        
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;
        for (int x = 0; x < gameBoard.Width; x++)
        {
            for (int y = 0; y < gameBoard.Height; y++)
            {
                SC_Gem _curGem = gameBoard.GetGem(x, y);
                if (_curGem == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    _curGem.posIndex.y -= nullCounter;
                    SetGem(x, y - nullCounter, _curGem);
                    SetGem(x, y, null);
                }
            }
            nullCounter = 0;
        }

        StartCoroutine(FilledBoardCo());
    }

    private void ScoreCheck(SC_Gem gemToCheck)
    {
        gameBoard.Score += gemToCheck.scoreValue;
    }
    
    private void DestroyMatchedGem(SC_Gem _Gem)
    {
        Instantiate(_Gem.destroyEffect, _Gem.transform.position, Quaternion.identity);

        gemsPool.ReturnOld(_Gem);
        SetGem(_Gem.posIndex.x, _Gem.posIndex.y, null);
    }

    private IEnumerator FilledBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        yield return RefillBoard();
        yield return new WaitForSeconds(0.5f);
        CheckAllDefaultsAfterEvents(-Vector2Int.one);
        if (gameBoard.CurrentMatches.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            currentState = GlobalEnums.GameState.move;
        }
    }
    private IEnumerator RefillBoard()
    {
        for (int y = 0; y < gameBoard.Height; y++)
        {
            for (int x = 0; x < gameBoard.Width; x++)
            {
                SC_Gem _curGem = gameBoard.GetGem(x,y);
                if (_curGem == null)
                {
                    SpawnGem(new Vector2Int(x, y), SemiRandomGemTypeAtPosition(new Vector2Int(x, y)));
                }
            }
            yield return new WaitForSeconds(1f/GameVariables.gemSpeed);
        }
    }

    private void CheckAllDefaultsAfterEvents(Vector2Int _RequestedFrom)
    {
        gameBoard.FindAllMatches(_RequestedFrom);
        CheckMatchesToSpawnBombs(_RequestedFrom);
        gameBoard.CheckForBombs();
    }

    private void CheckMatchesToSpawnBombs(Vector2Int _RequestedFrom)
    {
        var matches = gameBoard.CurrentMatches;
        foreach (var gems in matches)
        {
            if (gems.Count < 4) continue;
            var position = _RequestedFrom != -Vector2Int.one ? _RequestedFrom : gems[UnityEngine.Random.Range(0, gems.Count)].posIndex;
            ReplaceGemWithBombAt(position);
        }
    }

    private void ReplaceGemWithBombAt(Vector2Int _Position)
    {
        var gem = gameBoard.GetGem(_Position.x, _Position.y);
        if (gem != null)
        {
            gemsPool.ReturnOld(gem);
        }
        SpawnGem(_Position, GlobalEnums.GemType.bomb, false);
    }
    public void FindAllMatches(Vector2Int _RequestedFrom)
    {
        CheckAllDefaultsAfterEvents(_RequestedFrom);
    }

    #endregion
}
