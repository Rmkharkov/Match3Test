﻿using Core;
using Gems;
using UnityEngine;
namespace Board
{
    public class GameBoardHolder : Instantiable<GameBoardHolder>, IGameBoardHolder
    {
#region Variables

        public int Height { get; private set; }

        public int Width { get; private set; }

        public Gem[,] AllGems { get; private set; }

  #endregion

        public void SetupBoard(int _Width, int _Height)
        {
            Width = _Width;
            Height = _Height;
            AllGems = new Gem[Width, Height];
        }

        public void SetGem(int _X, int _Y, Gem _Gem)
        {
            AllGems[_X, _Y] = _Gem;
        }

        public Gem GetGem(int _X, int _Y)
        {
            return AllGems[_X, _Y];
        }

        public void RemoveGemAt(Vector2 _Position)
        {
            var coords = GemsMoving.BoardPositionByInput(_Position);
            SetGem(coords.x, coords.y, null);
        }
    }
}
