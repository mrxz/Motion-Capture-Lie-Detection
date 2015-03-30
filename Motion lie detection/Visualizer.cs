using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using System.Data;
using System.Text;




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
        //Camera camera;
        TrafficLight trafficLight;
        Color DrawColor = Color.Green;
        GeometricPrimitive primitive;
		GraphicsDevice graphics;
        KeyboardState keyboardState;
        BasicEffect basicEffect;

		private Vector3 position;
		private Vector3 target;
		public Matrix viewMatrix, projectionMatrix;

		Matrix World = Matrix.Identity;

		Camera camera ;
		

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


			//camera = new Camera (GraphicsDevice.Viewport.AspectRatio, Matrix.CreateLookAt (new Vector3 (0, 0, -50), new Vector3 (0, 0, 0), Vector3.Up));

			camera = new Camera (GraphicsDevice.Viewport.AspectRatio, Vector3.Left);
			camera.Zoom = 10;
			base.Initialize ();

		
		}

        protected override void LoadContent()
        {
			graphics = this.GraphicsDevice;

            primitive = new SpherePrimitive(graphics, 0.5f, 16);
            basicEffect = new BasicEffect(GraphicsDevice);
			basicEffect.View = camera.ViewMatrix;
			basicEffect.Projection = camera.ProjectionMatrix;
            //basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
			base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
		

			var state = Keyboard.GetState ();

			if (state.IsKeyDown (Keys.Left)) {
				camera.Yaw += 0.1f;
			}
			if (state.IsKeyDown (Keys.Right)) {
				camera.Yaw -= 0.1f;
			}
			if (state.IsKeyDown (Keys.Up)) {
				camera.Pitch += 0.1f;
			}
			if (state.IsKeyDown (Keys.Down)) {
				camera.Pitch -= 0.1f;
			}

			if (state.IsKeyDown (Keys.OemPlus)) {
				camera.Zoom += 1;
			}
			if (state.IsKeyDown (Keys.OemMinus)) {
				camera.Zoom -= 1;
			}
            
            base.Update(gameTime);
        }

		public void Update ()
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


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (frame.Joints == null)
                return;

            //update the view matrix
           /* Matrix view = Matrix.CreateLookAt(eye, focus, up);
            basicEffect.View = Matrix.CreateLookAt(eye, focus, up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
*/

			basicEffect.View = camera.ViewMatrix;
			basicEffect.Projection = camera.ProjectionMatrix;
            //draw a red sphere at the center of the world.
	
            Vector3 position = Vector3.Zero;
            Matrix world = Matrix.CreateTranslation(position);
			primitive.Draw(world, camera.ViewMatrix, camera.ProjectionMatrix, Color.Red);

            //find the average position
			Vector3 Centre;
           /* foreach (Joint joint in frame.Joints)
            {               
                float temp = AveragePosition.Y;
                AveragePosition.Y = AveragePosition.Z;
                AveragePosition.Z = temp;
                AveragePosition += joint.Position;
                
            }*/

			Centre = ConvertRealWorldPoint (frame.Joints [3].Position);

            //jointId, joint, (x, y, z)
            Dictionary<int, Tuple<Joint, Vector3>> joints = new Dictionary<int, Tuple<Joint, Vector3>>();

            //Draw each joint
            foreach ( Joint joint in frame.Joints)
            {
				position = ConvertRealWorldPoint(joint.Position) - Centre;
                //position = Vector3.Transform(position, Matrix.CreateTranslation(-AveragePosition));
                joints.Add(joint.Id, Tuple.Create(joint, position));
                world = Matrix.CreateTranslation(position);
				primitive.Draw(world, basicEffect.View, basicEffect.Projection, DrawColor);                
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
			return new Vector3 (position.X, position.Z, position.Y) * 10;
        }

    }
}
