using UnityEngine;
public interface IGameBoardHolder
{
    public int Height { get; }
    public int Width { get; }
    public Gem[,] AllGems { get; }
    void SetupBoard(int _Width, int _Height);
    void SetGem(int _X, int _Y, Gem _Gem);
    void RemoveGemAt(Vector2 _Position);
    Gem GetGem(int _X, int _Y);
}