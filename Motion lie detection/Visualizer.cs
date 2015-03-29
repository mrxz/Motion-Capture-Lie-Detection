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
        KeyboardState keyboardState;
        BasicEffect basicEffect;

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
			new GraphicsDeviceManager (this);
            keyboardState = new KeyboardState();
        }

		protected override void Initialize ()
		{
			// FIXME: This code doesn't work for some reason, the time_Tick method is never invoked.
			var timer = new Timer();
			timer.Interval = 1000 / 60;
			timer.Tick += new EventHandler(this.timer_Tick);
			timer.Start();

            eye = new Vector3(0, 0, -50);
            focus = new Vector3(0, 0, 0);
            up = Vector3.Up;

			base.Initialize ();

			// HACK: Call time_Tick method once to get something on the screen.
			timer_Tick (null, null);
		}

        protected override void LoadContent()
        {
			graphics = this.GraphicsDevice;
            primitive = new SpherePrimitive(graphics, 0.5f, 16);
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = Matrix.CreateLookAt(eye,focus, up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
			base.LoadContent();
        }

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
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (frame.Joints == null)
                return;

            //update the view matrix
            Matrix view = Matrix.CreateLookAt(eye, focus, up);
            basicEffect.View = Matrix.CreateLookAt(eye, focus, up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);

            //draw a red sphere at the center of the world.
            Vector3 position = Vector3.Zero;
            Matrix world = Matrix.CreateTranslation(position);
            primitive.Draw(world, view, projection, Color.Red);

            //find the average position
            Vector3 AveragePosition = Vector3.Zero;
            foreach (Joint joint in frame.Joints)
            {               
                float temp = AveragePosition.Y;
                AveragePosition.Y = AveragePosition.Z;
                AveragePosition.Z = temp;
                AveragePosition += joint.Position;
                
            }
            AveragePosition /= frame.Joints.Count;
            AveragePosition *= 10;

            //jointId, joint, (x, y, z)
            Dictionary<int, Tuple<Joint, Vector3>> joints = new Dictionary<int, Tuple<Joint, Vector3>>();

            //Draw each joint
            foreach ( Joint joint in frame.Joints)
            {
                position = ConvertRealWorldPoint(joint.Position);
                position = Vector3.Transform(position, Matrix.CreateTranslation(-AveragePosition));
                joints.Add(joint.Id, Tuple.Create(joint, position));
                world = Matrix.CreateTranslation(position);
                primitive.Draw(world, view, projection, DrawColor);                
            }

            BodyConfiguration bodyConfiguration = recording.BodyConfiguration;
            foreach (Tuple<BodyPart, BodyPart> connection in bodyConfiguration.GetConnections())
                drawLine(joints, bodyConfiguration, connection.Item1, connection.Item2);
            
			base.Draw(gameTime);
		}

        private void drawLine(Dictionary<int, Tuple<Joint, Vector3>> joints, BodyConfiguration configuration, BodyPart first, BodyPart second)
        {
            int one = configuration.GetJointFor(first);
            int two = configuration.GetJointFor(second);
            if (one == -1 || two == -1)
                return;

            Tuple<Joint, Vector3> firstJoint = joints[one];
            Tuple<Joint, Vector3> secondJoint = joints[two];
            
            //Draw the lines between the joints
            basicEffect.CurrentTechnique.Passes[0].Apply();
            var vertices = new[] { new VertexPositionColor(firstJoint.Item2, Color.White),  new VertexPositionColor(secondJoint.Item2, Color.White) };
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                            
        }

        private Vector3 ConvertRealWorldPoint(Vector3 position)
        {
            var returnVector = new Vector3();
            returnVector.X = position.X ;
            returnVector.Y = position.Z ;
            returnVector.Z = position.Y;
            return returnVector * 10;
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
