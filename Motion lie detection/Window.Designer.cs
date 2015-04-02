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
		public static readonly int Margin = 10;

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

		private void InitializeComponent()
        {
            // 
            // panel1
            // 
			this.canvas = new Panel();
            this.canvas.Name = "panel1";
            this.canvas.TabIndex = 0;
            this.canvas.BackColor = System.Drawing.SystemColors.ControlText;
            this.canvas.Text = "panel1";
            this.canvas.Paint += new PaintEventHandler(this.panel1_Paint);
			this.canvas.MouseMove += this.panel1_Drag;
			this.canvas.MouseDown += this.panel1_StartDrag;
			this.canvas.MouseUp += this.panel1_StopDrag;

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
			this.Controls.Add(this.canvas);
			this.Text = "Window";
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

			// Place and scale the canvas.
			canvas.Location = new System.Drawing.Point (Margin, Margin);
			canvas.Width = newSize.Width - 2 * Margin;
			canvas.Height = newSize.Height - 3 * Margin - 150;

			// Place and scale the timeline.
			timeline.Location = new System.Drawing.Point (Margin, canvas.Bottom + Margin);
			timeline.Width = newSize.Width - 2 * Margin;
			timeline.Height = 150;
		}
    }
}
