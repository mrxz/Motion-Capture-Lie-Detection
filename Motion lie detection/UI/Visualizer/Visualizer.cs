﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.Data;
using System.Text;
using WinFormsGraphicsDevice;
using System.Windows.Forms;



namespace Motion_lie_detection
{

    public class Visualizer : GraphicsDeviceControl
    {
        Grid grid;

        Frame frame;

        public Frame Frame
        {
            get { return frame; }
            set
            {
                frame = value;
                Invalidate();
            }
        }
        BodyConfiguration bodyConfiguration;

        public BodyConfiguration BodyConfiguration
        {
            get { return bodyConfiguration; }

            set
            {
                bodyConfiguration = value;
                Invalidate();
            }
        }
        
        Color jointColor = Color.LightGray;
        Color boneColor = Color.LightGray;
        GeometricPrimitive sphere;
		CylinderPrimitive cylinder;

		Camera camera;

        public Visualizer()
        {
            this.MouseMove += visualizer_Drag;
            this.MouseUp += visualizer_StopDrag;
            this.MouseDown += visualizer_StartDrag;
            this.MouseWheel += visualizer_Zoom;
        }

        void Visualizer_Resize(object sender, EventArgs e)
        {

            camera.AspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }

		protected override void Initialize()
		{
            //create a camera
        	camera = new Camera (GraphicsDevice.Viewport.AspectRatio, Vector3.Forward);
            camera.LookAt = Vector3.Forward;
            camera.Zoom = 50;

            //create the sphere for drawing joints and the cylinder for the bones
            sphere = new SpherePrimitive(GraphicsDevice, 0.5f, 16);
            cylinder = new CylinderPrimitive(GraphicsDevice, 1, 0.5f, 16);

            //create the grid
            grid = new Grid(GraphicsDevice, -13);

            // listen for resizes to adjust aspect ratios
            this.Resize += Visualizer_Resize;
		}		

        protected override void Draw()
        {
            // Make sure a body configuration and joints are present.
            if (bodyConfiguration == null || frame.Joints == null)
            {
                // Not available so display the inactive/disabled color.
                GraphicsDevice.Clear(Color.DarkGray);
                return;
            }

            // Start the normal drawing procedure.
            GraphicsDevice.Clear(Color.Black);

            //draw the grid
            grid.Draw(GraphicsDevice, camera.ViewMatrix, camera.ProjectionMatrix);
            

			Vector3 Centre = ConvertRealWorldPoint (frame.Joints [3].Position.ToXNAVec());

            //jointId, joint, (x, y, z)
            Dictionary<int, Tuple<Joint, Vector3>> joints = new Dictionary<int, Tuple<Joint, Vector3>>();

            //Draw each joint
            foreach (Joint joint in frame.Joints)
            {
				var position = ConvertRealWorldPoint(joint.Position.ToXNAVec()) - Centre;
                joints.Add(joint.Id, Tuple.Create(joint, position));          
                drawJoint(position, 0.5f);               
            }

			Queue<BodyNode> q = new Queue<BodyNode>();
			q.Enqueue (bodyConfiguration.getRoot ());
			// BFS
			while (q.Count > 0) {
				BodyNode node = q.Dequeue ();
				foreach (BodyNode neighbour in node.getNeighbours()) {
					q.Enqueue (neighbour);
					drawBone (joints, bodyConfiguration, node.JointId, neighbour.JointId, 0);
				}
			}
		}
        //reset the visualizer
        public void Reset()
        {
            GraphicsDevice.Clear(Color.DarkGray);
            bodyConfiguration = null;
        }
        /**
         * Method for drawing a joint.
         * @param joints position in 3D
         * @param the contribution of this joint to the lie result          
         */
        private void drawJoint(Vector3 position, float amount)
        {
            jointColor = Color.Lerp(Color.LightGray, Color.Red, amount);
            sphere.Draw(Matrix.CreateTranslation(position), camera.ViewMatrix, camera.ProjectionMatrix, jointColor);  
        }

        /**
         * Method for drawing the connection between two joints.
         * @param joints Dictionary containing the joints by joint id with their position
         * @param configuration The bodyconfiguration to draw.
         * @param first The index of the first joint of the bone.
         * @param second The index of the second joint of the bone.
         * @param the contribution to the algorithm by this bone.
         */
		private void drawBone(Dictionary<int, Tuple<Joint, Vector3>> joints, BodyConfiguration configuration, int first, int second, float amount)
        {
			if (first== -1 || second == -1)
                return;

            Tuple<Joint, Vector3> firstJoint = joints[first];
			Tuple<Joint, Vector3> secondJoint = joints[second];


			Matrix initTrans = Matrix.CreateTranslation (0, 0.5f, 0);

			float length = Vector3.Distance (firstJoint.Item2, secondJoint.Item2);
			Matrix scale = Matrix.CreateScale (.5f, length, .5f);

			Vector3 dir = Vector3.Normalize (secondJoint.Item2 - firstJoint.Item2);
			Vector3 axis = Vector3.Normalize (Vector3.Cross (Vector3.UnitY, dir));
			Matrix rot = Matrix.Identity;
			rot.Up = dir;
			rot.Right = axis;
			rot.Forward = Vector3.Cross (dir, axis);

			Matrix trans = Matrix.CreateTranslation (firstJoint.Item2);

			Matrix world = initTrans * scale * rot * trans;
            boneColor =  Color.Lerp(Color.LightGray, Color.Red, amount);
			cylinder.Draw (world, camera.ViewMatrix, camera.ProjectionMatrix, boneColor);
        }

        #region events
        private static Vector3 ConvertRealWorldPoint(Vector3 position)
        {
			return new Vector3 (position.X, position.Z, position.Y) * 10;
        }

        //when the scroll wheal is used
        void visualizer_Zoom(object sender, MouseEventArgs e)
        {
            camera.Zoom -= e.Delta / 50;
        }

        int prevMouseX = -1;
        int prevMouseY = -1;
        
        //When the left mouse button is released, do the following:
        void visualizer_StopDrag(object sender, MouseEventArgs e)
        {
            prevMouseX = -1;
            prevMouseY = -1;
        }
        //When the left mouse button is pressed, do the following:
        void visualizer_StartDrag(object sender, MouseEventArgs e)
        {
            prevMouseX = e.X;
            prevMouseY = e.Y;
            this.Focus();
        }
        //When the left mouse button is being held down, do the following:
        void visualizer_Drag(object sender, MouseEventArgs e)
        {
            float dx = (prevMouseX - e.X) / 200f;
            float dy = (prevMouseY - e.Y) / 200f;
            visualizer_StartDrag(sender, e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    camera.Yaw += dx;
                    camera.Pitch += dy;
                    break;
                case MouseButtons.Right:
                    camera.MoveCameraRight(-dx*5);
                    camera.MoveCameraForward(dy*5);
                    Invalidate();
                    break;
                case MouseButtons.Middle:
                    camera.Zoom -= dy*20;
                    break;
            }
        }      
        #endregion
    }
}
