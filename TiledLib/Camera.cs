using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace TiledLib
{
    public class Camera
    {
        public Vector2 Position;
        public int Width;
        public int Height;
        public Vector2 Target;
        public Rectangle ClampRect;
        public float Rotation;
        public float RotationTarget;

        public Matrix CameraMatrix;

        float Speed = 0.2f;

        /// <summary>
        /// Initialise the camera, using the game map to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="map">Game Map</param>
        public Camera(Viewport vp, Map map)
        {
            Position = new Vector2(0, 0);
            Width = vp.Width;
            Height = vp.Height;

            ClampRect = new Rectangle(0,0, map.Width * map.TileWidth, map.Height * map.TileHeight);

            if (map.Properties.Contains("CameraBoundsLeft"))
                ClampRect.X = Convert.ToInt32(map.Properties["CameraBoundsLeft"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsTop"))
                ClampRect.Y = Convert.ToInt32(map.Properties["CameraBoundsTop"]) * map.TileHeight;
            if (map.Properties.Contains("CameraBoundsWidth"))
                ClampRect.Width = Convert.ToInt32(map.Properties["CameraBoundsWidth"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsHeight"))
                ClampRect.Height = Convert.ToInt32(map.Properties["CameraBoundsHeight"]) * map.TileHeight;

            // Set initial position and target
            Position.X = ClampRect.X;
            Position.Y = ClampRect.Y;
            Target = new Vector2(ClampRect.X, ClampRect.Y);
        }
        public Camera(RenderTarget2D vp, Map map)
        {
            Position = new Vector2(0, 0);
            Width = vp.Width;
            Height = vp.Height;

            ClampRect = new Rectangle(0, 0, map.Width * map.TileWidth, map.Height * map.TileHeight);

            if (map.Properties.Contains("CameraBoundsLeft"))
                ClampRect.X = Convert.ToInt32(map.Properties["CameraBoundsLeft"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsTop"))
                ClampRect.Y = Convert.ToInt32(map.Properties["CameraBoundsTop"]) * map.TileHeight;
            if (map.Properties.Contains("CameraBoundsWidth"))
                ClampRect.Width = Convert.ToInt32(map.Properties["CameraBoundsWidth"]) * map.TileWidth;
            if (map.Properties.Contains("CameraBoundsHeight"))
                ClampRect.Height = Convert.ToInt32(map.Properties["CameraBoundsHeight"]) * map.TileHeight;

            // Set initial position and target
            Position.X = ClampRect.X;
            Position.Y = ClampRect.Y;
            Target = new Vector2(ClampRect.X, ClampRect.Y);
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        /// 
        public void Update(Rectangle bounds)
        {
            Update(Speed, bounds);
        }
        public void Update(float speed, Rectangle bounds)
        {
            Width = bounds.Width;
            Height = bounds.Height;

            // Clamp target to map/camera bounds
            //Target.X = MathHelper.Clamp(Target.X, ClampRect.X, ClampRect.Width - Width);
            //Target.Y = MathHelper.Clamp(Target.Y, ClampRect.Y, ClampRect.Height - Height);

            //Position.X = MathHelper.Clamp(Position.X, ClampRect.X, ClampRect.Width - Width);
            //Position.Y = MathHelper.Clamp(Position.Y, ClampRect.Y, ClampRect.Height - Height);
            
            // Move camera toward target
            Position = Vector2.Lerp(Position, Target, speed * 0.5f);

            Rotation = TurnToFace(Vector2.Zero, AngleToVector(RotationTarget, 1f), Rotation, 0.02f);

            CameraMatrix = Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0) * Matrix.CreateScale(1f) * Matrix.CreateRotationZ(-Rotation) * Matrix.CreateTranslation(Width/2, Height-(bounds.Height/3),0);
            //CameraMatrix *= Matrix.CreateRotationZ(Rotation);
        }

        public static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         C 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // S--------
            //     x
            // 
            // where S is the position of the spot light, C is the position of the cat,
            // and "o" is the angle that the spot light should be facing in order to 
            // point at the cat. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public static Vector2 AngleToVector(float angle, float length)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(angle) * length;
            direction.Y = (float)Math.Sin(angle) * length;
            return direction;
        }
    }
}
