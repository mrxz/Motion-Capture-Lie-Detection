using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;


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


    public class Visualizer : Game
    {
        BodyModel bodyModel;
        Camera camera;
        TrafficLight trafficLight;
        Color DrawColor = Color.Green;
        GeometricPrimitive primitive;
		GraphicsDevice graphics;

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
			new GraphicsDeviceManager (this);
        }

		protected override void Initialize ()
		{
			// FIXME: This code doesn't work for some reason, the time_Tick method is never invoked.
			var timer = new Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += new EventHandler(this.timer_Tick);
			timer.Start();
			base.Initialize ();

			// HACK: Call time_Tick method once to get something on the screen.
			timer_Tick (null, null);
		}

        protected override void LoadContent()
        {
			graphics = this.GraphicsDevice;
			primitive = new SpherePrimitive(graphics);
			base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
			// HACK: This makes it work sort of, but relies on update frequency of the visualization :/
			//timer_Tick (null, null);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (frame.Joints == null)
                return;

            Matrix view = Matrix.CreateLookAt(new Vector3(-10, -15, -15), new Vector3(0, 0, 100), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1.0f, 100);

            
            foreach ( Joint joint in frame.Joints)
            {
                Vector3 position = ConvertRealWorldPoint(joint.Position);
                Matrix world = Matrix.CreateTranslation(position);
                primitive.Draw(world, view, projection, DrawColor);                
            }
			base.Draw(gameTime);
		}

        private Vector3 ConvertRealWorldPoint(Vector3 position)
        {
            var returnVector = new Vector3();
            returnVector.X = position.X * 10;
            returnVector.Y = position.Y * 10;
            returnVector.Z = position.Z;
            return returnVector;
        }

        public void timer_Tick(Object source, EventArgs e)
        {
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
