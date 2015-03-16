using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Motion_lie_detection
{
    class Camera
    {
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private Vector3 eye;
        private Vector3 up;
        private Vector3 focus;
        float deltaH = 0, deltaV = 0;
        const float SENS = 0.02f;//sensitivity of the mouse

        //Sets up the camera settings
        public Camera(Vector3 camEye, Vector3 camFocus, Vector3 camUp, float aspectRatio = 4.0f / 3.0f)
        {
            this.up = camUp; //Tell the camera which side is up
            this.eye = camEye; //The position of the camera
            this.focus = camFocus; //The gaze direction
            this.updateViewMatrix();
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 300.0f); //Determines the field of view of the camera
        }

        private void updateViewMatrix()
        {
            Vector3 toFocus = focus - eye; //gives the vector from the camera to the point at which the camera is looking
            toFocus = Vector3.Transform(toFocus, Matrix.CreateFromAxisAngle(up, deltaH * SENS)); //rotate horizontally 
            focus = eye + toFocus;//recalculate the focus vector
            toFocus = focus - eye;
            Vector3 axis = Vector3.Cross(up, toFocus);//calculate the axis for the vertical rotation
            axis.Normalize();
            toFocus = Vector3.Transform(toFocus, Matrix.CreateFromAxisAngle(axis, deltaV * SENS));//rotate vertically
            focus = eye + toFocus;

            viewMatrix = Matrix.CreateLookAt(eye, focus, up);//create the view Matrix
            deltaH = 0;
            deltaV = 0;
        }

        #region Properties


        public Matrix ViewMatrix
        {
            get { return this.viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return this.projectionMatrix; }
        }

        public Vector3 Eye
        {
            get { return this.eye; }
            set { this.eye = value; this.updateViewMatrix(); }
        }

        public Vector3 Focus
        {
            get { return this.focus; }
            set { this.focus = value; this.updateViewMatrix(); }
        }
        public float DeltaH
        {
            get { return this.deltaH; }
            set { this.deltaH = value; this.updateViewMatrix(); }
        }
        public float DeltaV
        {
            get { return this.deltaV; }
            set { this.deltaV = value; this.updateViewMatrix(); }
        }

        #endregion
    }
}
