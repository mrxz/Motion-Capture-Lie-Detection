using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Timers;



namespace Motion_lie_detection
{
    /*
     * ONZE SUPER COOOLE MEGA TODO LIJST!!!!!
     * 
     *  MUST:
     * - Add camera controllzzz
     * - Basix GUI die loaden en saven ondersteunt 
     * - Draw de verbindingen tussen de joints waar ze verbonden zijn
     * 
     * COULD:
     * maak de spheres kleiner want ze zijn zo groot nu.
     * mooie kleurtjes!!!
     */


	public class Visualizer : GraphicsDeviceControl
    {
        BodyModel bodyModel;
        Camera camera;
        TrafficLight trafficLight;
        Color DrawColor = Color.Green;
        GeometricPrimitive primitive;
		GraphicsDevice graphics;
        KeyboardState keyboardState;

        Vector3 eye, focus, up;

        /**
		 * The recording to visualize.
		 */
        private readonly Recording recording;
        /**
         * The frame that is being drawn.
         */
        private Frame frame = new Frame();

        /**
         * Simple playback control variables.
         */
        private int currentFrameID = 0;
        private bool forward = true;
        private bool stepMode = false;


        public Visualizer(Recording recording)
        {
            this.recording = recording;
			//new GraphicsDeviceManager (this);
            keyboardState = new KeyboardState();
        }

		protected override void Initialize ()
		{
			// FIXME: This code doesn't work for some reason, the time_Tick method is never invoked.
			var timer = new Timer();
			timer.Interval = 1000 / 60;
			//timer.Elapsed
			timer.Start();
			timer.Enabled = true;

            eye = new Vector3(0, 0, -50);
            focus = new Vector3(0, 0, 0);
            up = Vector3.Up;

			//base.Initialize ();

			// HACK: Call time_Tick method once to get something on the screen.
			timer_Tick (null, null);

			graphics = this.GraphicsDevice;
			primitive = new SpherePrimitive(graphics);
			//base.LoadContent();
		}



		/*
        protected override void Update(GameTime gameTime)
        {
			// HACK: This makes it work sort of, but relies on update frequency of the visualization :/
			//timer_Tick (null, null);
            float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds * 60.0f;
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                this.Exit();
            
            
            //rotate around the visualized data
            float deltaAngle = 0.0f;
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                deltaAngle += 0.05f;
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                deltaAngle -= 0.05f;
            if (deltaAngle != 0)
                eye = Vector3.Transform(eye, Matrix.CreateRotationY(deltaAngle));

            //zoom controls
            float zoom = 0.00f;
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                zoom += 1f;
            if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                zoom -= 1f;
            eye.Z += zoom;
            
            base.Update(gameTime);
        }*/

        protected override void Draw(	)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (frame.Joints == null)
                return;

            Matrix view = Matrix.CreateLookAt(eye, focus, up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 100);

            //draw a red sphere at the center of the world.
            Vector3 position = Vector3.Zero;
            Matrix world = Matrix.CreateTranslation(position);
            primitive.Draw(world, view, projection, Color.Red);

            //find the average position
            Vector3 AveragePosition = Vector3.Zero;
            foreach (Joint joint in frame.Joints)
            {
                AveragePosition += joint.Position;
            }
            AveragePosition /= frame.Joints.Count;
            AveragePosition.X *= 10;
            AveragePosition.Z *= 10;

            foreach ( Joint joint in frame.Joints)
            {
                position = ConvertRealWorldPoint(joint.Position);
                position = Vector3.Transform(position, Matrix.CreateTranslation(-AveragePosition));
                world = Matrix.CreateTranslation(position);              
                primitive.Draw(world, view, projection, DrawColor);                
            }
            
			//base.Draw(gameTime);
		}

        private Vector3 ConvertRealWorldPoint(Vector3 position)
        {
            var returnVector = new Vector3();
            returnVector.X = position.X * 10;
            returnVector.Z = position.Y * 10;
			returnVector.Y = position.Z * 10;
            return returnVector;
        }

        public void timer_Tick(Object source, EventArgs e)
        {
			Console.WriteLine ("he");
            recording.Update();
            if (!stepMode)
            {
                if (forward)
                {
                    currentFrameID++;
                    currentFrameID = Math.Min(currentFrameID, recording.LastFrame());
                }
                else
                {
                    currentFrameID--;
                    currentFrameID = Math.Max(currentFrameID, 0);
                }
            }

            frame = recording.GetFrame(currentFrameID);
        }
    }
}
