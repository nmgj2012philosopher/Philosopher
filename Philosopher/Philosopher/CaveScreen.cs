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
        Crystal,
        OpenGrave,
        UpLadder,
        DownLadder,
        UpLadderExit,
        UpLadderEntrance,
        UpDownLadder,
        Shaft
    }

    public enum Direction
    {
        North,
        East,
        South,
        West,
        Up,
        Down
    }
    class CaveScreen : Screen
    {
        public const int MapWidth = 5;
        public const int MapHeight = 3;
        public const int MapDepth = 3;
        public const int TileHeight = 240;
        public const int TileWidth = 256;
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

            Vector3 currentPoint = new Vector3(currentLevel % 2 == 0 ? MapWidth - 1 : 0, 1, 0);
            robot = new Robot(new Vector2(currentPoint.X * TileWidth + (TileWidth / 2),
                currentPoint.Y * TileHeight + (TileHeight / 2)));

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

        private void DelveDeeper()
        {
            parent.PushScreen(new CaveScreen(parent, currentLevel + 1));
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
                            case CaveTile.Crystal:
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
                            case CaveTile.UpLadderExit:
                                DrawTile(Asset.UpLadderExitTile, x, y, sb); break;
                            case CaveTile.UpLadderEntrance:
                                DrawTile(Asset.UpLadderEntrance, x, y, sb); break;
                            case CaveTile.UpDownLadder:
                                DrawTile(Asset.UpDownLadder, x, y, sb); break;
                            case CaveTile.Shaft:
                                DrawTile(Asset.CaveShaft, x, y, sb); break;
                        }
                    }
                }
                if (z < currentHeight)
                    DrawAlphaMask(0x70, sb);

            }

            robot.Render( sb ); 
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

        public static CaveTile GetTile(CaveTile[][][] map, Vector3 pos)
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

        public static CaveTile GetTile(CaveTile[][][] map, Vector3 pos, Direction fromPos)
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

        public static Vector3 AddDirection(Vector3 pos, Direction dir, int distance = 1)
        {
            Vector3 newPos = new Vector3(pos.X, pos.Y, pos.Z);
            switch (dir)
            {
                case Direction.Up: newPos.Z += distance; break;
                case Direction.Down: newPos.Z -= distance; break;
                case Direction.East: newPos.X += distance; break;
                case Direction.West: newPos.X -= distance; break;
                case Direction.North: newPos.Y -= distance; break;
                case Direction.South: newPos.Y += distance; break;
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

            while (!pointStack.Peek().Equals(endPoint))
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
                        if (GetTile(map, AddDirection(pointStack.Peek(), Direction.Down)) == CaveTile.UpLadder)
                            SetTile(map, pointStack.Peek(), CaveTile.UpDownLadder);
                        else
                            SetTile(map, pointStack.Peek(), CaveTile.UpLadder);
                        SetTile(map, AddDirection(pointStack.Peek(), dir), CaveTile.DownLadder);
                    }
                    else if (dir == Direction.Down)
                    {
                        if (GetTile(map, AddDirection(pointStack.Peek(), Direction.Up)) == CaveTile.DownLadder)
                            SetTile(map, pointStack.Peek(), CaveTile.UpDownLadder);
                        else
                            SetTile(map, pointStack.Peek(), CaveTile.DownLadder);
                        SetTile(map, AddDirection(pointStack.Peek(), dir), CaveTile.UpLadder);
                    }

                    /*If the tile is not assigned at this point, give it some random graves.*/
                    else
                    {
                        int tileOffset = parent.rand.Next() % 5;
                        if (tileOffset == 4)
                            SetTile(map, pointStack.Peek(), CaveTile.Empty);
                        else
                            SetTile(map, pointStack.Peek(), CaveTile.Monster + tileOffset);
                    }
                }
                pointStack.Push(AddDirection(pointStack.Peek(), dir));

            }

            SetTile(map, endPoint, CaveTile.Exit);
            if (GetTile(map, AddDirection(endPoint, Direction.Up)) != CaveTile.None)
            {
                SetTile(map, endPoint, CaveTile.UpLadderExit);
                SetTile(map, AddDirection(endPoint, Direction.Up), CaveTile.UpDownLadder);
                SetTile(map, AddDirection(AddDirection(endPoint, Direction.Up), Direction.Up), CaveTile.DownLadder);
            }
            if (GetTile(map, AddDirection(endPoint, Direction.Up)) == CaveTile.DownLadder)
                SetTile(map, endPoint, CaveTile.UpLadderExit);

            if (GetTile(map, AddDirection(currentPoint, Direction.Up)) != CaveTile.None)
            {
                SetTile(map, AddDirection(currentPoint, Direction.Up), CaveTile.UpDownLadder);
                SetTile(map, currentPoint, CaveTile.UpLadderEntrance);
                SetTile(map, AddDirection(AddDirection(currentPoint, Direction.Up), Direction.Up), CaveTile.DownLadder);
            }

            map = UpDownAllLadders(map);

            if((currentLevel) % 5 == 0)
            for (int z = 0; z < MapHeight; z++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        if (GetTile(map, new Vector3(x, y, z)) == CaveTile.Empty)
                        {
                            SetTile(map, new Vector3(x, y, z), CaveTile.Shaft);
                            return map;
                        }
                    }
                }
            }

            return map;
        }

        private CaveTile[][][] UpDownAllLadders(CaveTile[][][] map)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (GetTile(map, new Vector3(x, y, 1)) == CaveTile.UpLadder)
                    {
                        SetTile(map, new Vector3(x, y, 0), CaveTile.UpLadder);
                        SetTile(map, new Vector3(x, y, 1), CaveTile.UpDownLadder);
                    }

                    if (GetTile(map, new Vector3(x, y, 0)) == CaveTile.UpLadder)
                    {
                        SetTile(map, new Vector3(x, y, 1), CaveTile.UpDownLadder);
                        SetTile(map, new Vector3(x, y, 2), CaveTile.DownLadder);
                    }
                }
            }

            return map;
        }

        private void SaveMap(CaveTile[][][] map, string filename = "")
        {
            FileStream fs;
            if (filename == "")
                fs = new FileStream("Content\\Maps\\cave" + currentLevel + ".map", FileMode.OpenOrCreate);
            else
                fs = new FileStream(filename, FileMode.OpenOrCreate);

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

        public override void GiveCommand(string command)
        {
            if (command.Equals("delve"))
            {
                if (currentHeight == 0)
                {
                    if (robot.GetPosition().Y > TileHeight &&
                        robot.GetPosition().Y < TileHeight * 2)
                    {
                        int exitStart = (currentLevel % 2 == 1 ? MapWidth - 1 : 0);
                        if (robot.GetPosition().X > exitStart &&
                            robot.GetPosition().X < exitStart + TileWidth)
                        {
                            DelveDeeper();
                        }
                    }
                }
            }
            switch (command)
            {
                case "mv u":
                case "move u":
                case "mv up":
                case "move up": 
                    currentHeight += (currentHeight == MapHeight - 1 ? 0 : 1);
                    break;

                case "mv d":
                case "move d":
                case "mv down":
                case "move down":
                    currentHeight -= (currentHeight == 0 ? 0 : 1);
                    break;

                case "open grave":
                case "open graves":
                    OpenGraves();
                    break;
            }
            robot.GiveCommand(command);
        }

        private void OpenGraves()
        {
            Vector3 truePosition = new Vector3(robot.GetPosition().X / TileWidth,
                robot.GetPosition().Y / TileHeight, currentHeight);
            CaveTile onTile = GetTile(map, truePosition);
            if (onTile >= CaveTile.Monster &&
                onTile <= CaveTile.Crystal)
            {
                SetTile(map, truePosition, CaveTile.OpenGrave);
            }

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
            fs.Close();

            return map;
        }

        public static bool CanOccupy(CaveTile[][][] map, Vector3 position)
        {
            if (position.Z > 0)
            {
                if (GetTile(map, position) == CaveTile.None)
                {
                    while (position.Z >= 0)
                    {
                        position.Z--;
                        if (GetTile(map, position) != CaveTile.None)
                            return true;
                    }
                    return false;
                }
            }

            if (GetTile(map, position) == CaveTile.None)
                return false;

            return true;
        }

        public override void Update(Game1 parent, KeyboardState prevState)
        {
            if (Game1.SOUNDS_ENABLED)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                    MediaPlayer.Play(AssetManager.GetSongAsset(AssetSong.Cave));
            }

            if (Util.SemiAutoKey(Keys.OemPeriod, prevState))
                currentHeight += (currentHeight == MapHeight - 1 ? 0 : 1);
            
            if (Util.SemiAutoKey(Keys.OemComma, prevState))
                currentHeight -= (currentHeight == 0 ? 0 : 1);

            if (Util.SemiAutoKey(Keys.Insert, prevState))
                SaveMap(map, "Content\\Maps\\DebugMap.map");

            if (Util.SemiAutoKey(Keys.Delete, prevState))
                map = LoadMap("Content\\Maps\\DebugMap.map");

            if(Util.SemiAutoKey(Keys.Home, prevState))
                map = GenerateMap();

            robot.Update(map);
        }
    }
}
