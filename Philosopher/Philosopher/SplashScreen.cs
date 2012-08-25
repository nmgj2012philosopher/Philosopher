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
    class SplashScreen : Screen
    {

        public override void Update(Game1 parent)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                parent.PopScreen();
        }

        public override void Render(Game1 parent, SpriteBatch sb)
        {
            sb.Draw(AssetManager.GetAsset(AssetManager.Asset.SplashScreen), Vector2.Zero, Color.White);
        }
    }
}
