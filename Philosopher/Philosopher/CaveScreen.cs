﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Philosopher
{
    public enum CaveTile
    {
        None,      /*None is different from empty. None means nothing,*/
        Empty,     /*Empty means no graves.*/
        Entrance,
        Exit,
        Monster,
        Artifact,
        Metal,
        UpLadder,
        DownLadder
    }
    class CaveScreen : Screen
    {
        private const int MapWidth = 5;
        private const int MapHeight = 3;
        private const int MapDepth = 3;
        private const int TileHeight = 256;
        private const int TileWidth = 240;
        private int currentLevel = 0;
        private Game1 parent;
        private CaveTile[][][] map;
        private Robot robot;
        public CaveScreen(Game1 parent)
        {
            this.parent = parent;
            map = GenerateMap();
        }

        public override void Render(Game1 parent, SpriteBatch sb)
        {
        }

        private CaveTile[][][] MakeNoneMap()
        {
            CaveTile[][][] tiles = new CaveTile[MapWidth][][];
            for (int x = 0; x < MapWidth; x++)
            {
                tiles[x] = new CaveTile[MapHeight][];
                for (int y = 0; y < MapHeight; y++)
                {
                    tiles[x][y] = new CaveTile[MapDepth];
                    for (int z = 0; z < MapDepth; z++)
                        tiles[x][y][z] = CaveTile.None;
                }
            }
            return tiles;
        }

        private CaveTile GetTile(CaveTile[][][] map, Vector3 pos)
        {
            return map[(int)pos.X][(int)pos.Y][(int)pos.Z];
        }

        private void SetTile(CaveTile[][][] map, Vector3 pos, CaveTile tile)
        {
            map[(int)pos.X][(int)pos.Y][(int)pos.Z] = tile;
        }

        private enum Direction
        {
            North,
            East,
            South,
            West,
            Up,
            Down
        }

        private Direction FindNextDir(CaveTile[][][] map, Vector3 pos)
        {
            List<Direction> dirs = new List<Direction>();
            dirs.Add(Direction.North);
            dirs.Add(Direction.East);
            dirs.Add(Direction.South);
            dirs.Add(Direction.West);
            dirs.Add(Direction.Up);
            dirs.Add(Direction.Down);

            if (pos.X == 0) dirs.Remove(Direction.West);
            if (pos.X >= MapWidth - 1) dirs.Remove(Direction.East);
            if (pos.Y == 0) dirs.Remove(Direction.North);
            if (pos.Y >= MapHeight - 1) dirs.Remove(Direction.South);
            if (pos.Z == 0) dirs.Remove(Direction.Down);
            if (pos.Z >= MapDepth - 1) dirs.Remove(Direction.Up);

            return dirs[parent.rand.Next() % dirs.Count];
        }

        private CaveTile[][][] GenerateMap()
        {
            CaveTile[][][] map = MakeNoneMap();

            Vector3 currentPoint = new Vector3(currentLevel % 2 == 0 ? MapWidth : 0, 1, 0);
            Vector3 endPoint = new Vector3(currentLevel % 2 == 0 ? 0 : MapWidth, 1, 0);

            Stack<Vector3> pointStack = new Stack<Vector3>();
            pointStack.Push(currentPoint);

            while (pointStack.Count > 0 && !pointStack.Peek().Equals(endPoint))
            {
                Direction nextDir = FindNextDir(map, pointStack.Peek());

                /*If the tile is not assigned at this point, give it some random graves.*/
                if (GetTile(map, pointStack.Peek()) == CaveTile.None)
                {
                    SetTile(map, pointStack.Peek(), CaveTile.Monster + (parent.rand.Next() % 3));
                }
            }

            return map;
        }

        public override void Update(Game1 parent)
        {
            
        }
    }
}