using System;
using System.Collections.Generic;
using System.IO;
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
        Null,      /*Null means NOT FOUND IN MAP*/
        None,      /*None is different from empty. None means empty space,*/
        Empty,     /*Empty means no graves.*/
        Entrance,
        Exit,
        Monster,
        Artifact,
        Metal,
        OpenGrave,
        UpLadder,
        DownLadder
    }
    class CaveScreen : Screen
    {
        private const int MapWidth = 5;
        private const int MapHeight = 3;
        private const int MapDepth = 3;
        private const int TileHeight = 240;
        private const int TileWidth = 256;
        private int currentLevel = 0;
        private int currentHeight = 0;
        private Game1 parent;
        private CaveTile[][][] map;
        private Robot robot;
        public CaveScreen(Game1 parent, int depth)
        {
            this.parent = parent;
            currentLevel = depth;
            if (File.Exists("Content\\Maps\\cave" + currentLevel + ".map"))
            {
                map = LoadMap("Content\\Maps\\cave" + currentLevel + ".map");
            }
            else
            {
                map = GenerateMap();
                SaveMap(map);
            }
        }

        /// <summary>
        /// Draws the tile at the selected tile position on the map, NOT PIXEL POSITION.
        /// </summary>
        /// <param name="tile">The tile to draw</param>
        /// <param name="x">the TILE POSITION to draw the tile at</param>
        /// <param name="y">the TILE POSITION to draw the tile at</param>
        private void DrawTile(Asset tile, int x, int y, SpriteBatch sb)
        {
            sb.Draw(AssetManager.GetAsset(tile),
                    new Vector2(x * TileWidth, y * TileHeight),
                    Color.White);
        }

        private void DrawAlphaMask(byte alphaLvl, SpriteBatch sb)
        {
            sb.Draw(AssetManager.GetAsset(Asset.Mask), new Rectangle(0, 0, Game1.SCREENWIDTH, Game1.SCREENHEIGHT),
                Color.FromNonPremultiplied(0, 0, 0, alphaLvl));
        }

        public override void Render(Game1 parent, SpriteBatch sb)
        {
            for (int z = 0; z < currentHeight + 1; z++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        switch (GetTile(map, new Vector3(x, y, z)))
                        {
                            case CaveTile.OpenGrave:
                                DrawTile(Asset.OpenGraveTile, x, y, sb); break;
                            case CaveTile.Monster:
                            case CaveTile.Metal:
                            case CaveTile.Artifact:
                                DrawTile(Asset.ClosedGraveTile, x, y, sb); break;
                            case CaveTile.UpLadder:
                                DrawTile(Asset.UpLadderTile, x, y, sb); break;
                            case CaveTile.DownLadder:
                                DrawTile(Asset.DownLadderTile, x, y, sb); break;
                            case CaveTile.Empty:
                                DrawTile(Asset.EmptyCaveTile, x, y, sb); break;
                            case CaveTile.Null:
                                DrawTile(Asset.NullCaveTile, x, y, sb); break;
                            case CaveTile.Entrance:
                                DrawTile(Asset.CaveEntrance, x, y, sb); break;
                            case CaveTile.Exit:
                                DrawTile(Asset.CaveExit, x, y, sb); break;
                        }
                    }
                }
                if (z < MapHeight)
                    DrawAlphaMask(0x40, sb);
            }
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
            try
            {
                return map[(int)pos.X][(int)pos.Y][(int)pos.Z];
            }
            catch
            {
                return CaveTile.Null;
            }
        }

        private CaveTile GetTile(CaveTile[][][] map, Vector3 pos, Direction fromPos)
        {
            pos = AddDirection(pos, fromPos);
            try
            {
                return map[(int)pos.X][(int)pos.Y][(int)pos.Z];
            }
            catch
            {
                return CaveTile.Null;
            }
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

            for (int i = 0; i < dirs.Count; i++)
            {
                if (GetTile(map, AddDirection(pos, dirs[i])) != CaveTile.None)
                    dirs.RemoveAt(i);
            }

            return dirs[parent.rand.Next() % dirs.Count];
        }

        private Vector3 AddDirection(Vector3 pos, Direction dir)
        {
            Vector3 newPos = new Vector3(pos.X, pos.Y, pos.Z);
            switch (dir)
            {
                case Direction.Up: newPos.Z++; break;
                case Direction.Down: newPos.Z--; break;
                case Direction.East: newPos.X++; break;
                case Direction.West: newPos.X--; break;
                case Direction.North: newPos.Y--; break;
                case Direction.South: newPos.Y++; break;
            }
            return newPos;
        }

        private bool CanMove(CaveTile[][][] map, Vector3 pos)
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

            for (int i = 0; i < dirs.Count; i++)
            {
                if (GetTile(map, AddDirection(pos, dirs[i])) != CaveTile.None)
                    dirs.RemoveAt(i);
            }
            return dirs.Count > 0;
        }

        private CaveTile[][][] GenerateMap()
        {
            CaveTile[][][] map = MakeNoneMap();

            Vector3 currentPoint = new Vector3(currentLevel % 2 == 0 ? MapWidth - 1 : 0, 1, 0);
            Vector3 endPoint = new Vector3(currentLevel % 2 == 0 ? 0 : MapWidth - 1, 1, 0);
            int wrongDirs = 0;
            Stack<Vector3> pointStack = new Stack<Vector3>();
            pointStack.Push(currentPoint);

            SetTile(map, currentPoint, CaveTile.Entrance);

            while (pointStack.Count > 0 && !pointStack.Peek().Equals(endPoint))
            {
                if(!CanMove(map, pointStack.Peek())) pointStack.Pop();

                Direction dir = FindNextDir(map, pointStack.Peek());
                while (GetTile(map, pointStack.Peek(), dir) != CaveTile.None)
                {
                    wrongDirs++;
                    if (wrongDirs > 50)
                    {
                        return GenerateMap();
                    }
                    dir = FindNextDir(map, pointStack.Peek());
                }

                if (GetTile(map, pointStack.Peek()) == CaveTile.None)
                {
                    if (dir == Direction.Up)
                    {
                        SetTile(map, pointStack.Peek(), CaveTile.UpLadder);
                        SetTile(map, AddDirection(pointStack.Peek(), dir), CaveTile.DownLadder);
                    }
                    else if (dir == Direction.Down)
                    {
                        SetTile(map, pointStack.Peek(), CaveTile.DownLadder);
                        SetTile(map, AddDirection(pointStack.Peek(), dir), CaveTile.UpLadder);
                    }

                    /*If the tile is not assigned at this point, give it some random graves.*/
                    else
                    {
                        int tileOffset = parent.rand.Next() % 4;
                        if (tileOffset == 3)
                            SetTile(map, pointStack.Peek(), CaveTile.Empty);
                        else
                            SetTile(map, pointStack.Peek(), CaveTile.Monster + tileOffset);
                    }
                }
                pointStack.Push(AddDirection(pointStack.Peek(), dir));

            }

            SetTile(map, endPoint, CaveTile.Exit);
            if (GetTile(map, AddDirection(endPoint, Direction.Up)) == CaveTile.DownLadder)
                SetTile(map, AddDirection(endPoint, Direction.Up), CaveTile.None);

            return map;
        }

        private void SaveMap(CaveTile[][][] map)
        {
            FileStream fs = new FileStream("Content\\Maps\\cave" + currentLevel + ".map", FileMode.OpenOrCreate);

            for (int x = 0; x < map.Length; x++)
            {
                for (int y = 0; y < map[x].Length; y++)
                {
                    for (int z = 0; z < map[x][y].Length; z++)
                    {
                        fs.WriteByte((byte)map[x][y][z]);
                    }
                }
            }
            fs.Close();
        }

        private CaveTile[][][] LoadMap(string filename)
        {
            CaveTile[][][] map = MakeNoneMap();
            FileStream fs = new FileStream(filename, FileMode.Open);

            for (int x = 0; x < map.Length; x++)
            {
                for (int y = 0; y < map[x].Length; y++)
                {
                    for (int z = 0; z < map[x][y].Length; z++)
                    {
                        map[x][y][z] = (CaveTile)fs.ReadByte();
                    }
                }
            }

            return map;
        }

        public override void Update(Game1 parent, KeyboardState prevState)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemPeriod) &&
                !prevState.IsKeyDown(Keys.OemPeriod))
                currentHeight += (currentHeight == MapHeight - 1 ? 0 : 1);
            
            if (Keyboard.GetState().IsKeyDown(Keys.OemComma) &&
                !prevState.IsKeyDown(Keys.OemComma))
                currentHeight -= (currentHeight == 0 ? 0 : 1);
        }
    }
}
