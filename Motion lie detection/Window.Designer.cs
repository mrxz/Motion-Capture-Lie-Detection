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
		 * Side panels
		 */
		private Panel leftSidePanel;
		private Panel rightSidePanel;

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
			this.leftSidePanel = new Panel ();
			this.leftSidePanel.Name = "leftSidePanel";
			this.Controls.Add (this.leftSidePanel);

			this.rightSidePanel = new RightSidePanel ();
			this.rightSidePanel.Name = "rightSidePanel";
			this.Controls.Add (this.rightSidePanel);

			//
			// Menu bar
			//
			mainMenu = new MainMenu();

			// File
			MenuItem File = mainMenu.MenuItems.Add("File");
			File.MenuItems.Add(new MenuItem("Start listening", new EventHandler(startListener), Shortcut.CtrlShiftO));
			File.MenuItems.Add(new MenuItem("Open recording file", new EventHandler(openFile), Shortcut.CtrlO));
			File.MenuItems.Add(new MenuItem("Save recording", new EventHandler(saveFile), Shortcut.CtrlS));
			File.MenuItems.Add(new MenuItem("Close recording", new EventHandler(closeRecording), Shortcut.CtrlW));
			File.MenuItems.Add(new MenuItem("Exit", new EventHandler(exit)));

			// Markpoint.
			MenuItem Mark = mainMenu.MenuItems.Add("Markpoint");
			Mark.MenuItems.Add(new MenuItem("Add markpoint"));
			Mark.MenuItems.Add(new MenuItem("Remove markpoint"));
			Mark.MenuItems.Add(new MenuItem("Next markpoint"));
			Mark.MenuItems.Add(new MenuItem("Previous markpoint"));

			// Settings
			MenuItem Settings = mainMenu.MenuItems.Add("Settings");
			Settings.MenuItems.Add(new MenuItem("Something"));
			Settings.MenuItems.Add(new MenuItem("Start dummy stream", new EventHandler(startDummyStream)));
			Settings.MenuItems.Add(new MenuItem("TODO"));

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

		public class ConnectForm : Form {

			private TextBox input;

			public ConnectForm() {
				Size size = new Size(300, 130);
				this.Size = size;
				this.MinimumSize = size;
				this.MaximumSize = size;
				this.Text = "Start listener";

				Label text = new Label();
				text.Text = "Enter host and port:";
				text.Width = ClientRectangle.Width - 2 * ControlMargin;
				text.Left = ControlMargin;
				text.TextAlign = ContentAlignment.MiddleCenter;
				this.Controls.Add(text);

				input = new TextBox();
				input.Text = "localhost:9763";
				input.Width = ClientRectangle.Width - 2 * ControlMargin;
				input.Left = ControlMargin;
				input.Top = text.Bottom + ControlMargin;
				this.Controls.Add(input);

				Button ok = new Button();
				ok.Text = "Ok";
				ok.Top = input.Bottom + ControlMargin;
				ok.Left = ControlMargin;
				ok.Click += (object sender, EventArgs e) => this.DialogResult = DialogResult.OK;
				this.Controls.Add(ok);

				Button cancel = new Button();
				cancel.Text = "Cancel";
				cancel.Top = input.Bottom + ControlMargin;
				cancel.Left = ok.Right + ControlMargin;
				cancel.Click += (object sender, EventArgs e) => this.DialogResult = DialogResult.Cancel;
				this.Controls.Add(cancel);

				this.CenterToParent();
			}

			public String Host {
				get { 
					return input.Text.Split (':') [0];
				}
			}

			public int Port {
				get {
					return int.Parse(input.Text.Split (':') [1]);
				}
			}

		}

		private void startListener(object sender, EventArgs e) {
			// Ask the user for the host + port.
			ConnectForm connectForm = new ConnectForm ();
			if (connectForm.ShowDialog (this) == DialogResult.Cancel)
				return;

			SuitController controller = new XSensController(connectForm.Host, connectForm.Port);
			controller.Calibrate ();
			controller.Connect();

			RecordingProvider provider = new SuitRecordingProvider (controller);
			provider.Init ();
			Recording recording = new Recording (provider, new FixedBodyConfiguration());
			recording.Update ();

			this.Recording = recording;
			this.Text = "Motion Lie Detection - " + connectForm.Host + ":" + connectForm.Port;
		}

		private void openFile(object sender, EventArgs e) {
			// Show the file open dialog
			OpenFileDialog dialog = new OpenFileDialog ();
			dialog.DefaultExt = "mvnx";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
			DialogResult result = dialog.ShowDialog ();
			if (result == DialogResult.Cancel)
				return;

			RecordingProvider provider = new FileRecordingProvider (dialog.FileName);
			provider.Init ();
			Recording recording = new Recording (provider);
			recording.Update ();

			this.Recording = recording;
			this.Text = "Motion Lie Detection - " + dialog.SafeFileName;
		}

		private void saveFile(object sender, EventArgs e) 
		{
			SaveFileDialog dialog = new SaveFileDialog ();
			dialog.DefaultExt = "mvnx";
			DialogResult result = dialog.ShowDialog ();
			if (result == DialogResult.Cancel)
				return;

			// TODO: Perhaps notify the user if the recording hasn't ended or something?

			RecordingSaver saver = new MVNXSaver (dialog.FileName); // FIXME: Hard-coded implementation for Recording saver.
			saver.saveToFile (this.recording);
		}

		private void closeRecording(object sender, EventArgs e) {
			// FIXME: Close the recording (and provider) correctly.
			this.Recording = null;
			this.Text = "Motion Lie Detection";
		}

		private void exit(object sender, EventArgs e) {
			// TODO: Cleanup
			Environment.Exit (0);
		}

		private void startDummyStream(object sender, EventArgs e) {
			DummyStream stream = new DummyStream ();
			stream.Start (); 
		}

		private void resize(Object sender, EventArgs e) {
			Size newSize = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);

			// Place and scale the left sidebar.
			leftSidePanel.Location = new System.Drawing.Point (ControlMargin, ControlMargin);
			leftSidePanel.Width = 200;
			leftSidePanel.Height = newSize.Height - 3 * ControlMargin - 150;

			// Place and scale the canvas.
			canvas.Location = new System.Drawing.Point (leftSidePanel.Right + ControlMargin, ControlMargin);
			canvas.Width = newSize.Width - 4 * ControlMargin - 200 - 200;
			canvas.Height = newSize.Height - 3 * ControlMargin - 150;

			// Place and scale the right sidebar.
			rightSidePanel.Width = 200;
			rightSidePanel.Left = canvas.Right + ControlMargin;
			rightSidePanel.Top = ControlMargin;
			rightSidePanel.Height = newSize.Height - 3 * ControlMargin - 150;

			// Place and scale the timeline.
			timeline.Location = new System.Drawing.Point (ControlMargin, canvas.Bottom + ControlMargin);
			timeline.Width = newSize.Width - 2 * ControlMargin;
			timeline.Height = 150;
		}

		public class RightSidePanel : Panel
		{
			private ListBox markpointBox;
			private Button addButton;
			private Button removeButton;

			public RightSidePanel()
			{
				markpointBox = new ListBox ();
				this.Controls.Add (markpointBox);

				addButton = new Button ();
				addButton.Text = "Add";
				this.Controls.Add (addButton);

				removeButton = new Button ();
				removeButton.Text = "Remove";
				this.Controls.Add (removeButton);

				this.Resize += resize;
			}

			private void resize(Object sender, EventArgs e) {
				Size newSize = new Size (ClientRectangle.Width, ClientRectangle.Height);

				markpointBox.Left = 0;
				markpointBox.Top = 0;
				markpointBox.Width = newSize.Width;
				markpointBox.Height = newSize.Height - 20 - ControlMargin;

				addButton.Top = markpointBox.Bottom + ControlMargin;
				addButton.Left = ControlMargin;
				addButton.Width = (newSize.Width - 3 * ControlMargin) / 2;
				addButton.Height = 20;

				removeButton.Top = markpointBox.Bottom + ControlMargin;
				removeButton.Left = addButton.Right + ControlMargin;
				removeButton.Width = (newSize.Width - 3 * ControlMargin) / 2;
				removeButton.Height = 20;
			}
		}
    }
}
