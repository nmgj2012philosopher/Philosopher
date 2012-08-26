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
    class SurfaceScreen : Screen
    {
        private Game1 parent;
        public override void Update(Game1 parent, KeyboardState prevKeys)
        {
            this.parent = parent;
        }

        public override void Render(Game1 parent, SpriteBatch sb)
        {

        }

        private void EnterCave()
        {
            if(Game1.SOUNDS_ENABLED)
                MediaPlayer.Stop();
            parent.PushScreen(new CaveScreen(parent, 0));
        }

        public override void GiveCommand(string command)
        {
            if (command.Equals("enter cave"))
                EnterCave();
        }
    }
}
