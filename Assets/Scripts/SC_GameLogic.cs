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
    private SC_GameVariablesConfig gameVariables;
    public GlobalEnums.GameState CurrentState { get { return currentState; } }

    #region MonoBehaviour
    private void Awake()
    {
        gameVariables = SC_GameVariablesConfig.Instance;
        gemsPool = new GemsPool();
        Init();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        displayScore = Mathf.Lerp(displayScore, gameBoard.Score, gameVariables.scoreSpeed * Time.deltaTime);
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
        for (int x = 0; x < gameBoard.Width; x++)
            for (int y = 0; y < gameBoard.Height; y++)
            {
                Vector2 _pos = new Vector2(x, y);
                GameObject _bgTile = Instantiate(gameVariables.bgTilePrefabs, _pos, Quaternion.identity);
                _bgTile.transform.SetParent(unityObjects["GemsHolder"].transform);
                _bgTile.name = "BG Tile - " + x + ", " + y;
                
                SpawnGem(new Vector2Int(x, y), SemiRandomGemTypeAtPosition(new Vector2Int(x, y)));
            }
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
    private void SpawnGem(Vector2Int _Position, GlobalEnums.GemType _GemTypeToSpawn)
    {
        if (Random.Range(0, 100f) < gameVariables.bombChance)
            _GemTypeToSpawn = GlobalEnums.GemType.bomb;

        SC_Gem _gem = gemsPool.Get(_GemTypeToSpawn, new Vector3(_Position.x, gameBoard.Height, 0f));
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
            if (gameBoard.CurrentMatches[i] != null)
            {
                ScoreCheck(gameBoard.CurrentMatches[i]);
                DestroyMatchedGemsAt(gameBoard.CurrentMatches[i].posIndex);
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

    public void ScoreCheck(SC_Gem gemToCheck)
    {
        gameBoard.Score += gemToCheck.scoreValue;
    }
    private void DestroyMatchedGemsAt(Vector2Int _Pos)
    {
        SC_Gem _curGem = gameBoard.GetGem(_Pos.x,_Pos.y);
        if (_curGem != null)
        {
            Instantiate(_curGem.destroyEffect, new Vector2(_Pos.x, _Pos.y), Quaternion.identity);

            gemsPool.Return(_curGem);
            SetGem(_Pos.x,_Pos.y, null);
        }
    }

    private IEnumerator FilledBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        yield return RefillBoard();
        yield return new WaitForSeconds(0.5f);
        gameBoard.FindAllMatches();
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
            yield return new WaitForSeconds(1f/gameVariables.gemSpeed);
        }
        CheckMisplacedGems();
    }
    private void CheckMisplacedGems()
    {
        List<SC_Gem> foundGems = new List<SC_Gem>();
        foundGems.AddRange(FindObjectsOfType<SC_Gem>());
        for (int x = 0; x < gameBoard.Width; x++)
        {
            for (int y = 0; y < gameBoard.Height; y++)
            {
                SC_Gem _curGem = gameBoard.GetGem(x, y);
                if (foundGems.Contains(_curGem))
                    foundGems.Remove(_curGem);
            }
        }

        foreach (SC_Gem g in foundGems)
            gemsPool.Return(g);
    }
    public void FindAllMatches()
    {
        gameBoard.FindAllMatches();
    }

    #endregion
}
