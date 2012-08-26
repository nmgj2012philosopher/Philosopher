using System;
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
    public enum Asset
    {
        SplashScreen,
        ClosedGraveTile,
        OpenGraveTile,
        EmptyCaveTile,
        UpLadderTile,
        DownLadderTile,
        UpLadderExitTile,
        CaveEntranceTile,
        CaveExitTile,
        Mask,
        NullCaveTile,
        CaveEntrance,
        CaveExit,
        CaveShaft,
        UpLadderEntrance,
        Robot,
        UpDownLadder
    }

    public enum AssetSong
    {
        Intro,
        Surface,
        Cave
    }

    class AssetManager
    {
        private static Dictionary<Asset, Texture2D> Assets;
        private static Dictionary<AssetSong, Song> Songs;

        public static void LoadStaticAssets(Game1 parent)
        {
            Assets = new Dictionary<Asset, Texture2D>();
            Assets.Add(Asset.SplashScreen, parent.Content.Load<Texture2D>("splash"));
            Assets.Add(Asset.OpenGraveTile, parent.Content.Load<Texture2D>("OpenGraves"));
            Assets.Add(Asset.ClosedGraveTile, parent.Content.Load<Texture2D>("ClosedGraves"));
            Assets.Add(Asset.Mask, parent.Content.Load<Texture2D>("mask"));
            Assets.Add(Asset.UpLadderTile, parent.Content.Load<Texture2D>("UpLadder"));
            Assets.Add(Asset.DownLadderTile, parent.Content.Load<Texture2D>("DownLadder"));
            Assets.Add(Asset.NullCaveTile, parent.Content.Load<Texture2D>("Null"));
            Assets.Add(Asset.EmptyCaveTile, parent.Content.Load<Texture2D>("Empty"));
            Assets.Add(Asset.CaveEntrance, parent.Content.Load<Texture2D>("Entrance"));
            Assets.Add(Asset.CaveExit, parent.Content.Load<Texture2D>("Exit"));
            Assets.Add(Asset.UpLadderExitTile, parent.Content.Load<Texture2D>("UpLadderExit"));
            Assets.Add(Asset.UpLadderEntrance, parent.Content.Load<Texture2D>("UpLadderEntrance"));
            Assets.Add(Asset.CaveShaft, parent.Content.Load<Texture2D>("Shaft"));
            Assets.Add(Asset.Robot, parent.Content.Load<Texture2D>("Robot"));
            Assets.Add(Asset.UpDownLadder, parent.Content.Load<Texture2D>("UpDownLadder"));

            Songs = new Dictionary<AssetSong, Song>();
            Songs.Add(AssetSong.Intro, parent.Content.Load<Song>("Intro"));
            Songs.Add(AssetSong.Surface, parent.Content.Load<Song>("Surface"));
            Songs.Add(AssetSong.Cave, parent.Content.Load<Song>("Cave"));
        }

        public static Texture2D GetAsset(Asset a)
        {
            return Assets[a];
        }

        public static Song GetSongAsset(AssetSong a)
        {
            return Songs[a];
        }
    }
}
