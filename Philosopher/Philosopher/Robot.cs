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
    class Robot
    {
        private Vector2 position;
        /// <summary>
        /// Creates a new robot. 
        /// </summary>
        /// <param name="spawnPos">The position to spawn the robot in.</param>
        public Robot(Vector2 spawnPos)
        {
            position = spawnPos;
        }
        public void Update()
        {
        }

        public void Render()
        {

        }
    }
}
