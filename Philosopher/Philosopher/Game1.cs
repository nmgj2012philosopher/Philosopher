using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Philosopher
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public const bool SOUNDS_ENABLED = true;
        public const int SCREENWIDTH = 1280;
        public const int SCREENHEIGHT = 720;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Stack<Screen> screenStack;
        public Random rand;
        private KeyboardState prevKeyState;
        private SpriteFont DefaultFont;

        private StringBuilder commandBuilder;
        private string readyCommand = "";
        private string previousCommand = "";

        public void PushScreen(Screen s)
        {
            screenStack.Push(s);
        }
        public Screen PeekScreen()
        {
            return screenStack.Peek();
        }
        public Screen PopScreen()
        {
            Screen s = screenStack.Pop();
            if (screenStack.Peek() is SurfaceScreen)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(AssetManager.GetSongAsset(AssetSong.Surface));
            }
            return s;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rand = new Random();
            commandBuilder = new StringBuilder();
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            screenStack = new Stack<Screen>();
            screenStack.Push(new SurfaceScreen());
            screenStack.Push(new CaveScreen(this, 0));
            screenStack.Push(new SplashScreen());

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            prevKeyState = Keyboard.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadStaticAssets(this);

            DefaultFont = Content.Load<SpriteFont>("Default");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("Content\\Maps");
            foreach (FileInfo f in dirInfo.GetFiles())
                File.Delete(f.FullName);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            screenStack.Peek().Update(this, prevKeyState);

            for (int i = 0; i < 26; i++)
            {
                if (Util.SemiAutoKey((Keys)(Keys.A + i), prevKeyState))
                    commandBuilder.Append((char)(65 + i));
            }
            if (Util.SemiAutoKey(Keys.Enter, prevKeyState))
            {
                readyCommand = commandBuilder.ToString();
                commandBuilder.Clear();
            }
            if (Util.SemiAutoKey(Keys.Back, prevKeyState))
            {
                if (commandBuilder.Length > 0)
                    commandBuilder.Remove(commandBuilder.Length - 1, 1);
            }
            if(Util.SemiAutoKey(Keys.Space, prevKeyState))
                commandBuilder.Append(' ');

            if (Util.SemiAutoKey(Keys.PageDown, prevKeyState))
                PopScreen();

            if (Util.SemiAutoKey(Keys.Up, prevKeyState))
            {
                commandBuilder.Clear();
                commandBuilder = new StringBuilder(previousCommand);
            }

            if (!readyCommand.Equals(""))
            {
                screenStack.Peek().GiveCommand(readyCommand.ToLower());
                previousCommand = readyCommand;
                readyCommand = "";
            }

            prevKeyState = Keyboard.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            screenStack.Peek().Render(this, spriteBatch);

            spriteBatch.DrawString(DefaultFont, "> " + commandBuilder.ToString(), new Vector2(10, SCREENHEIGHT - 30), Color.Lime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}