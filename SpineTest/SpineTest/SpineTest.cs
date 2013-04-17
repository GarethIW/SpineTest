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
using Spine;
using TiledLib;

namespace SpineTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpineTest : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map gameMap;
        Camera gameCamera;
        Hero gameHero;

        public SpineTest()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

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

            // TODO: use this.Content to load your game content here
            gameMap = Content.Load<Map>("map");
            
            gameHero = new Hero(new Vector2(1200, 1000));
            gameHero.LoadContent(Content, GraphicsDevice);

            gameCamera = new Camera(GraphicsDevice.Viewport, gameMap);
            gameCamera.Position = gameHero.Position;
            gameCamera.Target = gameHero.Position;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Left)) gameHero.MoveLeftRight(-1f);
            else if (ks.IsKeyDown(Keys.Right)) gameHero.MoveLeftRight(1f);

            if (ks.IsKeyDown(Keys.Up)) gameHero.Jump();
            if (ks.IsKeyDown(Keys.Down)) gameHero.Crouch();

            gameHero.Update(gameTime, gameCamera, gameMap);

            gameCamera.Target = gameHero.Position;
            gameCamera.Update(GraphicsDevice.Viewport.Bounds);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, gameCamera.CameraMatrix);
            gameMap.Draw(spriteBatch, gameCamera);
            spriteBatch.End();

            gameHero.Draw(GraphicsDevice, spriteBatch, gameCamera);

            base.Draw(gameTime);
        }
    }
}
