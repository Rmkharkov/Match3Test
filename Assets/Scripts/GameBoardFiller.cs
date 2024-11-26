using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameBoardFiller : MonoBehaviour
{
    private GameBoardHolder BoardHolder => GameBoardHolder.Instance;
    private GemsRandomizer UsedGemsRandomizer => GemsRandomizer.Instance;
    private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
    [SerializeField] private GemsExistence gemsExistence;
    private IGemsExistence UsedGemsExistence => gemsExistence;
    
    private IEnumerator FillBoard()
    {
        for (int y = 0; y < BoardHolder.Height; y++)
        {
            for (int x = 0; x < BoardHolder.Width; x++)
            {
                Gem curGem = BoardHolder.GetGem(x,y);
                if (curGem == null)
                {
                    var pos = new Vector2Int(x, y);
                    var gemType = UsedGemsRandomizer.SemiRandomGemTypeAtPosition(new Vector2Int(x, y));
                    var gem = UsedGemsExistence.SpawnGem(pos, BoardHolder.Height, gemType, false);
                    BoardHolder.SetGem(x, y, gem);
                }
            }
            yield return new WaitForSeconds(1f/GameVariables.gemSpeed);
        }
    }
}
