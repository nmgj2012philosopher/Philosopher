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
    class AssetManager
    {
        public enum Asset
        {
            SplashScreen
        }

        private static Dictionary<Asset, Texture2D> Assets;

        public static void LoadStaticAssets(Game1 parent)
        {
            Assets = new Dictionary<Asset, Texture2D>();
            Assets.Add(Asset.SplashScreen, parent.Content.Load<Texture2D>("splash"));
        }

        public static Texture2D GetAsset(Asset a)
        {
            return Assets[a];
        }
    }
}
