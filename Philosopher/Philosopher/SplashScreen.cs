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
        private bool firstRun = false;
        public override void Update(Game1 parent, KeyboardState prevState)
        {
            if (!firstRun)
            {
                firstRun = true;
                if (Game1.SOUNDS_ENABLED) 
                    MediaPlayer.Play(AssetManager.GetSongAsset(AssetSong.Intro));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if(Game1.SOUNDS_ENABLED)
                    MediaPlayer.Stop();

                parent.PopScreen();
            }
        }

        public override void Render(Game1 parent, SpriteBatch sb)
        {
            sb.Draw(AssetManager.GetAsset(Asset.SplashScreen), Vector2.Zero, Color.White);
        }

        public override void GiveCommand(string command)
        {

        }
    }
}
