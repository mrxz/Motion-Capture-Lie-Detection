using System;
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
    class Grid
    {
        private BasicEffect basicEffect;
        private Vector3 startPoint = new Vector3(0, 0, 0);
        private Vector3 endPoint = new Vector3(0, 0, -50);
        private Vector3[] points;

        public Grid(GraphicsDevice GraphicsDevice, float groundLevel)
        {
            Init(GraphicsDevice);
            points = new Vector3[200];
            Fill(groundLevel);
            
        }

        public void Fill(float groundLevel)
        {
            int begin = -100;
            int step = 10;
            int current = begin;
            int i = 0;
            while(current <= -begin)
            {
                points[i] = new Vector3(begin, groundLevel, current);
                points[i+1] = new Vector3(-begin, groundLevel, current);
                i += 2;
                current += step;
            }
            current = begin;
            while (current <= -begin + step)
            {
                points[i] = new Vector3(current, groundLevel, begin);
                points[i + 1] = new Vector3(current, groundLevel, -begin);
                i += 2;
                current += step;
            }
            current = begin;
            
            
        }

        public void Init(GraphicsDevice GraphicsDevice)
        {
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = Matrix.CreateLookAt(new Vector3(50, 50, 50), new Vector3(0, 0, 0), Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
        }

        // Inside your Game.Draw method
        public void Draw(GraphicsDevice GraphicsDevice, Matrix view, Matrix projection)
        {
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.CurrentTechnique.Passes[0].Apply();
            Vector3 vec1, vec2;
            int i=0;
            while(points[i+2] != Vector3.Zero)
            {
                vec1 = points[i];
                vec2 = points[i + 1];
                
                var vertices = new[] { new VertexPositionColor(vec1, Color.White), new VertexPositionColor(vec2, Color.White) };
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
                i+=2;
            }
        }

    }
}
