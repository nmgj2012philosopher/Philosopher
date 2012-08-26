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
        private float speed = 1.5F, maxVelocity = 10;
        private float rotation = 0;
        private int lazerPower = 1000, lazerRechargeTime = 10; //how long can the lazer be fired?
        private Vector2 position;

        public Vector2 GetPosition()
        {
            return position;
        }

        private Vector2 velocity;
        private MouseState previousMouseState;
        private Texture2D robotTexture = AssetManager.GetAsset(Asset.Robot);
        
        private bool moving = false;
        private Vector3 target;

        /// <summary>
        /// Creates a new robot. 
        /// </summary>
        /// <param name="spawnPos">The position to spawn the robot in.</param>
        public Robot(Vector2 spawnPos)
        {
            position = spawnPos;
        }

        public void GiveCommand(string command)
        {
            if (!moving)
            {
                SetTarget(command);
                
            }
        }

        private void SetTarget(string command)
        {

            switch (command)
            {
                case "mv n":
                case "move n":
                case "mv north":
                case "move north": GetTarget(Direction.North); break;

                case "mv s":
                case "move s":
                case "mv south":
                case "move south": GetTarget(Direction.South); break;

                case "mv e":
                case "move e":
                case "mv east":
                case "move east": GetTarget(Direction.East); break;

                case "mv w":
                case "move w":
                case "mv west":
                case "move west": GetTarget(Direction.West); break;
            }

        }

        private void GetTarget(Direction direction)
        {
            moving = true;
            target = CaveScreen.AddDirection( new Vector3( position.X / (float)CaveScreen.TileWidth, position.Y / (float)CaveScreen.TileHeight, 0), direction );
            target.X = (float)(target.X *CaveScreen.TileWidth);
            target.Y = (float)(target.Y *CaveScreen.TileHeight);

        }




        public void Update(CaveTile[][][] map)
        {
            if (moving)
            {
                if (Util.Distance(position.X, position.Y, target.X, target.Y) > 5)
                {
                    if (target.X > position.X)
                        position.X += speed;
                    if (target.X < position.X)
                        position.X -= speed;
                    if (target.Y < position.Y)
                        position.Y -= speed;
                    if (target.Y > position.Y)
                        position.Y += speed;
                }
                else
                    moving = false;
            }
            
            /*

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

            */



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



