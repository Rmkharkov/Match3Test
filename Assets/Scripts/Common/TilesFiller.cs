using System.Threading.Tasks;
using Board;
using UnityEngine;
namespace Common
{
    public class TilesFiller : MonoBehaviour
    {
        [SerializeField]
        private Transform tilesParent;

        private IGameBoardHolder BoardHolder => GameBoardHolder.Instance;
        private SC_GameVariablesConfig GameVariables => SC_GameVariablesConfig.Instance();
    
        private async void Start()
        {
            await Task.Yield();
            FillTiles();
        }

        private void FillTiles()
        {
            for (int y = 0; y < BoardHolder.Height; y++)
            for (int x = 0; x < BoardHolder.Width; x++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(GameVariables.bgTilePrefabs, pos, Quaternion.identity);
                bgTile.transform.SetParent(tilesParent);
                bgTile.name = "BG Tile - " + x + ", " + y;
            }
        }
    }
}
