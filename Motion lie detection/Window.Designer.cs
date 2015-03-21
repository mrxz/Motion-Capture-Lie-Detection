using System;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
    public partial class Window
    {
		private Panel panel1;

		private void InitializeComponent()
        {
            // 
            // panel1
            // 
			this.panel1 = new Panel();
            this.panel1.Name = "panel1";
            this.panel1.Location = new System.Drawing.Point(16, 24);
            this.panel1.TabIndex = 0;
			this.panel1.Size = new Size(1180, 576);
            this.panel1.BackColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Text = "panel1";
            this.panel1.Paint += new PaintEventHandler(this.panel1_Paint);
			this.panel1.Click += this.panel1_Click;
			this.panel1.MouseMove += this.panel1_Drag;
			this.panel1.MouseDown += this.panel1_StartDrag;
			this.panel1.MouseUp += this.panel1_StopDrag;

			// 
            // Window
            // 
            this.Name = "Window";
			this.ClientSize = new Size(1200, 630);
            this.Controls.Add(this.panel1);
            this.Text = "Window";
			this.FormClosing += this.form_Closed;
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.KeyDown += this.keyDown;
        }
    }
}
