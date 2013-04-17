using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiledLib;

namespace SpineTest
{
    class Hero
    {
        public Vector2 Position;
        public Vector2 Speed;

        Vector2 gravity = new Vector2(0f, 0.25f);

        Rectangle collisionRect = new Rectangle(0, 0, 75, 150);

        Texture2D blankTex;

        SkeletonRenderer skeletonRenderer;
        Skeleton skeleton;
        Animation walkAnimation;
        Animation jumpAnimation;
        Animation crawlAnimation;
        float animTime;

        int faceDir = 1;

        bool walking = false;
        bool jumping = false;
        bool crouching = false;

        public Hero(Vector2 spawnPosition)
        {
            Position = spawnPosition;
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            blankTex = content.Load<Texture2D>("blank");

            skeletonRenderer = new SkeletonRenderer(graphicsDevice);
            Atlas atlas = new Atlas(graphicsDevice, Path.Combine(content.RootDirectory, "spineboy.atlas"));
            SkeletonJson json = new SkeletonJson(atlas);
            skeleton = new Skeleton(json.readSkeletonData("spineboy", File.ReadAllText(Path.Combine(content.RootDirectory, "spineboy.json"))));
            skeleton.SetSkin("default");
            skeleton.SetSlotsToBindPose();
            walkAnimation = skeleton.Data.FindAnimation("walk");
            jumpAnimation = skeleton.Data.FindAnimation("jump");
            crawlAnimation = skeleton.Data.FindAnimation("crawl");

            skeleton.RootBone.X = Position.X;
            skeleton.RootBone.Y = Position.Y;
            skeleton.UpdateWorldTransform();
        }

        public void Update(GameTime gameTime, Camera gameCamera, Map gameMap)
        {
            if (!walking && !jumping && !crouching)
            {
                skeleton.SetToBindPose();
            }

            if (walking && !jumping)
            {
                animTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;
                if (!crouching)
                {
                    walkAnimation.Mix(skeleton, animTime, true, 0.3f);
                }
                else
                {
                    crawlAnimation.Mix(skeleton, animTime, true, 0.5f);
                }
            }

            if (jumping)
            {
                animTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;
                jumpAnimation.Mix(skeleton, animTime, false, 0.5f);
            }

            if (crouching && !jumping)
            {
                collisionRect.Width = 120;
                collisionRect.Height = 96;

                if (!walking)
                {
                    animTime = 0;
                    crawlAnimation.Mix(skeleton, animTime, false, 0.5f);
                }
            }
            else
            {
                collisionRect.Width = 75;
                collisionRect.Height = 150;
            }

            Speed += gravity;
            CheckCollision(gameMap);
            Position += Speed;
            collisionRect.Location = new Point((int)Position.X - (collisionRect.Width / 2), (int)Position.Y - (collisionRect.Height));

            skeleton.RootBone.X = Position.X;
            skeleton.RootBone.Y = Position.Y;

            if (faceDir == -1) skeleton.FlipX = true; else skeleton.FlipX = false;

            skeleton.UpdateWorldTransform();

            walking = false;
            Speed.X = 0f;
        }

        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Camera gameCamera)
        {
            skeletonRenderer.Begin(gameCamera.CameraMatrix);
            skeletonRenderer.Draw(skeleton);
            skeletonRenderer.End();

            // Draw collision box
            //spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, gameCamera.CameraMatrix);
            //spriteBatch.Draw(blankTex, collisionRect, Color.White * 0.3f);
            //spriteBatch.End();
        }


        public void MoveLeftRight(float dir)
        {
            if (dir > 0) faceDir = 1; else faceDir = -1;

            Speed.X = dir * 4f;
            walking = true;
            
        }

        public void Jump()
        {
            if (!jumping && !crouching)
            {
                jumping = true;
                animTime = 0;
                Speed.Y = -9f;
            }
        }

        public void Crouch()
        {
            crouching = true;
        }

        void CheckCollision(Map gameMap)
        {
            collisionRect.Offset(new Point((int)Speed.X, (int)Speed.Y));

            if (Speed.Y > 0f)
                if (CheckCollisionBottom(gameMap))
                {
                    Speed.Y = 0f;
                    jumping = false;
                }

            if (Speed.Y < 0f)
                if (CheckCollisionTop(gameMap)) Speed.Y = 0f;

            if (Speed.X > 0f)
                if (CheckCollisionRight(gameMap)) Speed.X = 0f;
            if (Speed.X < 0f)
                if (CheckCollisionLeft(gameMap)) Speed.X = 0f;


            bool collided = false;
            for (int y = -1; y > -15; y--)
            {
                collisionRect.Offset(0, -1);
                if (CheckCollisionTop(gameMap)) collided = true;
            }
            if (!collided) crouching = false;

        }

        bool CheckCollisionTop(Map gameMap)
        {
            for (float x = collisionRect.Left + 3; x <= collisionRect.Right-2; x += (collisionRect.Right-2 - collisionRect.Left+3) / 4f)
            {
                Vector2 checkPos = new Vector2(x, collisionRect.Top);
                if (gameMap.CheckTileCollision(checkPos)) return true;
            }

            return false;
        }
        bool CheckCollisionBottom(Map gameMap)
        {
            for (float x = collisionRect.Left + 3; x <= collisionRect.Right-2; x += ((collisionRect.Right-2) - (collisionRect.Left+3)) / 10f)
            {
                Vector2 checkPos = new Vector2(x, collisionRect.Bottom);
                if (gameMap.CheckTileCollision(checkPos)) return true;
            }

            return false;
        }
        bool CheckCollisionRight(Map gameMap)
        {
            for (float y = collisionRect.Top; y < collisionRect.Bottom; y += (collisionRect.Bottom - collisionRect.Top) / 10f)
            {
                Vector2 checkPos = new Vector2(collisionRect.Right, y);
                if (gameMap.CheckTileCollision(checkPos)) return true;
            }

            return false;
        }
        bool CheckCollisionLeft(Map gameMap)
        {
            for (float y = collisionRect.Top; y < collisionRect.Bottom; y += (collisionRect.Bottom - collisionRect.Top) / 10f)
            {
                Vector2 checkPos = new Vector2(collisionRect.Left, y);
                if (gameMap.CheckTileCollision(checkPos)) return true;
            }

            return false;
        }
    }
}
