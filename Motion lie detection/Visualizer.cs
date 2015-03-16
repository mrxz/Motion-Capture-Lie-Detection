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


namespace Motion_lie_detection

{
    public class Visualizer : Game
    {
        BodyModel bodyModel;
        Camera camera;
        TrafficLight trafficLight;
        Color DrawColor = Color.Green;
        Frame frame;
        GeometricPrimitive primitive;

        public Visualizer()
        {
            primitive = new SpherePrimitive(GraphicsDevice);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -20), new Vector3(0, 0, 100), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 100);

            
            foreach ( Joint joint in frame.Joints)
            {
                Vector3 position = ConvertRealWorldPoint(joint.Position);
                Matrix world = Matrix.CreateTranslation(position);
                primitive.Draw(world, view, projection, DrawColor);                
            }
            base.Draw(gameTime);
        }

        public void DrawFrame(Frame frame)
        {
            this.frame = frame;

        }
        private Vector3 ConvertRealWorldPoint(Vector3 position)
        {
            var returnVector = new Vector3();
            returnVector.X = position.X * 10;
            returnVector.Y = position.Y * 10;
            returnVector.Z = position.Z;
            return returnVector;
        }
    }
}
