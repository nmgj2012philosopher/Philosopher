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
        private float speed = 1, maxVelocity = 10;
        private float rotation = 0;
        private int lazerPower = 1000, lazerRechargeTime = 10; //how long can the lazer be fired?
        private Vector2 position;
        private Vector2 velocity;
        private MouseState previousMouseState;
        private Texture2D robotTexture = AssetManager.GetAsset(Asset.Robot);


        /// <summary>
        /// Creates a new robot. 
        /// </summary>
        /// <param name="spawnPos">The position to spawn the robot in.</param>
        public Robot(Vector2 spawnPos)
        {
            position = spawnPos;
        }
        public void Update(CaveTile[][][] map)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                velocity.Y -= speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                velocity.Y += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity.X -= speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity.X += speed;
            }



            rotation = (float)Math.Atan(Math.Sqrt(Math.Pow(Mouse.GetState().X - position.X, 2) + Math.Pow(Mouse.GetState().Y - position.Y, 2)));


            //Does lazer fire like a gun or like a water gun (constant stream as long as button is held down)

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //Fire lazer until power out
                if (lazerPower > 0)
                {
                    //Fire lazer
                    lazerPower--;
                }


            }
            else { lazerPower += lazerRechargeTime; }

            if (Mouse.GetState().LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                //Fire Lazer
            }
            //Set speed limit
            if (velocity.X > maxVelocity) { velocity.X = maxVelocity; }
            if (velocity.Y > maxVelocity) { velocity.Y = maxVelocity; }

            position += velocity;
            previousMouseState = Mouse.GetState();
        }

        public void Render(SpriteBatch sb)
        {
            sb.Draw(robotTexture, position, null, Color.White, rotation, new Vector2(robotTexture.Width / 2, robotTexture.Height / 2), 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}



