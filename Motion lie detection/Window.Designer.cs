using System;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
    public partial class Window
    {
		/**
		 * Universal margin used throughout the GUI.
		 */
		public static readonly int ControlMargin = 10;

		/**
		 * The main menu bar at the top of the window.
		 */
		private MainMenu mainMenu;

		/**
		 * The canvas in which the display is drawn.
		 */
		private Panel canvas;

		/**
		 * The timeline control that lets the user control the timeline.
		 */
		private Timeline timeline;

		/**
		 * Side panel
		 */
		private Panel sidePanel;

		private void InitializeComponent()
        {
            // 
            // Canvas
            // 
			this.canvas = new Panel();
            this.canvas.Name = "canvas";
            this.canvas.TabIndex = 0;
            this.canvas.BackColor = System.Drawing.SystemColors.ControlText;
            this.canvas.Text = "canvas";
            this.canvas.Paint += new PaintEventHandler(this.panel1_Paint);
			this.canvas.MouseMove += this.panel1_Drag;
			this.canvas.MouseDown += this.panel1_StartDrag;
			this.canvas.MouseUp += this.panel1_StopDrag;
			this.Controls.Add(this.canvas);

			//
			// Side panel
			//
			this.sidePanel = new Panel ();
			this.sidePanel.Name = "sidePanel";
			this.Controls.Add (this.sidePanel);

			//
			// Menu bar
			//
			mainMenu = new MainMenu();

			// File
			MenuItem File = mainMenu.MenuItems.Add("File");
			File.MenuItems.Add(new MenuItem("New"));
			File.MenuItems.Add(new MenuItem("Open"));
			File.MenuItems.Add(new MenuItem("Exit"));

			// Help
			MenuItem About = mainMenu.MenuItems.Add("Help");
			About.MenuItems.Add(new MenuItem("About"));

			this.Menu = mainMenu;

			//
			// Timeline
			//
			timeline = new Timeline ();
			this.Controls.Add (timeline);


			// 
			// Window
			// 
			this.Name = "Window";
			this.ClientSize = new Size(1200, 830);
			this.MinimumSize = new Size (800, 600);
			this.Text = "Motion Lie Detection";
			this.FormClosing += this.form_Closed;
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.KeyDown += this.keyDown;
			this.Resize += this.resize;


			// Note: we call resize once to make sure there's no difference between initial layout and resized layout.
			resize (null, null);
        }

		private void resize(Object sender, EventArgs e) {
			Size newSize = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);

			// Place and scale the sidebar.
			sidePanel.Location = new System.Drawing.Point (ControlMargin, ControlMargin);
			sidePanel.Width = 200;
			sidePanel.Height = newSize.Height - 3 * ControlMargin - 150;

			// Place and scale the canvas.
			canvas.Location = new System.Drawing.Point (sidePanel.Right + ControlMargin, ControlMargin);
			canvas.Width = newSize.Width - 3 * ControlMargin - 200 - 200;
			canvas.Height = newSize.Height - 3 * ControlMargin - 150;

			// Place and scale the timeline.
			timeline.Location = new System.Drawing.Point (ControlMargin, canvas.Bottom + ControlMargin);
			timeline.Width = newSize.Width - 2 * ControlMargin;
			timeline.Height = 150;
		}
    }
}
