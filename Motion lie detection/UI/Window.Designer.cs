using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
		 MainMenu mainMenu;

        /**
         * Panel containing the playback controls.
         */
        PlaybackPanel playbackPanel;

		/**
		 * The timeline control that lets the user control the timeline.
		 */
        Timeline timeline;

		/**
         * the visualizer in which our subject is shown. and any other visualizations
         */
        Visualizer visualizer;

        /**
		 * Side panels
		 */
        LeftSidePanel leftSidePanel;
        RightSidePanel rightSidePanel;

		private void InitializeComponent()
        {
            this.visualizer = new Visualizer();
            this.visualizer.Name = "visualizer";
            this.visualizer.TabIndex = 0;
            this.Controls.Add(this.visualizer);
            
			//
			// Timeline
			//
            timeline = new Timeline();
            this.Controls.Add(timeline);

            //
            // Playback controls
            //
            playbackPanel = new PlaybackPanel(timeline);
            this.Controls.Add(playbackPanel);

            //
            // Side panels
            //
            this.leftSidePanel = new LeftSidePanel(timeline);
            this.leftSidePanel.Name = "leftSidePanel";
            this.Controls.Add(this.leftSidePanel);

            this.rightSidePanel = new RightSidePanel(timeline);
            this.rightSidePanel.Name = "rightSidePanel";
            this.Controls.Add(this.rightSidePanel);

            // 
			// Window
			// 
			this.Name = "Window";
			this.ClientSize = new Size(1200, 830);
            this.MinimumSize = new Size(800, 600);
			this.Text = "Motion Lie Detection";
			this.FormClosing += this.form_Closed;
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.KeyDown += this.keyDown;
			this.Resize += this.resize;

            //
            // Menu bar
            //
            mainMenu = new MainMenu();
            this.Menu = mainMenu;

            // File
            MenuItem File = mainMenu.MenuItems.Add("File");
            File.MenuItems.Add(new MenuItem("Start listening", new EventHandler(startListener), Shortcut.CtrlShiftO));
            File.MenuItems.Add(new MenuItem("Stop listener", new EventHandler(stopListener), Shortcut.CtrlShiftS));
            File.MenuItems.Add(new MenuItem("Open recording file", new EventHandler(openFile), Shortcut.CtrlO));
            File.MenuItems.Add(new MenuItem("Save recording", new EventHandler(saveFile), Shortcut.CtrlS));
            File.MenuItems.Add(new MenuItem("Close recording", new EventHandler(closeRecording), Shortcut.CtrlW));
            File.MenuItems.Add(new MenuItem("Exit", new EventHandler(exit)));

            // Markpoint.
            MenuItem Mark = mainMenu.MenuItems.Add("Markpoint");
            Mark.MenuItems.Add(new MenuItem("Add markpoint", rightSidePanel.AddMarkpoint));
            Mark.MenuItems.Add(new MenuItem("Next markpoint", playbackPanel.NextMarkpoint));
            Mark.MenuItems.Add(new MenuItem("Previous markpoint", playbackPanel.PreviousMarkpoint));

            // Demo
            MenuItem Settings = mainMenu.MenuItems.Add("Test");
            Settings.MenuItems.Add(new MenuItem("Start dummy stream", new EventHandler(startDummyStream)));
            Settings.MenuItems.Add(new MenuItem("Start endless dummy stream", new EventHandler(startEndlessDummyStream)));


			// Note: we call resize once to make sure there's no difference between initial layout and resized layout.
            resize(null, null);
        }

        /**
         * Special form that can be used as dialog to ask the user for a text value.
         */
        public class InputForm : Form
        {

			private TextBox input;

            public InputForm(String title, String prompt, String placeholder)
            {
				Size size = new Size(300, 130);

				this.MinimumSize = size;
				this.MaximumSize = size;
				this.Text = title;

				Label text = new Label();
				text.Text = prompt;
				text.Width = ClientRectangle.Width - 2 * ControlMargin;
				text.Left = ControlMargin;
				text.TextAlign = ContentAlignment.MiddleCenter;
				this.Controls.Add(text);

				input = new TextBox();
				input.Text = placeholder;
				input.Width = ClientRectangle.Width - 2 * ControlMargin;
				input.Left = ControlMargin;
				input.Top = text.Bottom + ControlMargin;
				this.Controls.Add(input);

				Button ok = new Button();
				ok.Text = "Ok";
				ok.Top = input.Bottom + ControlMargin;
				ok.Left = ControlMargin;
				ok.Click += (object sender, EventArgs e) => this.DialogResult = DialogResult.OK;
                input.KeyPress += (obj, e) =>
                {
                    if (e.KeyChar == (char)13)
                        ok.PerformClick();
                };
                this.Controls.Add(ok);

				Button cancel = new Button();
				cancel.Text = "Cancel";
				cancel.Top = input.Bottom + ControlMargin;
				cancel.Left = ok.Right + ControlMargin;
				cancel.Click += (object sender, EventArgs e) => this.DialogResult = DialogResult.Cancel;
				this.Controls.Add(cancel);

				this.CenterToParent();
			}
            
            public String Value
            {
                get { return input.Text;  }
            }

        }

        public class ConnectForm : InputForm
        {
            public ConnectForm()
                : base("Start listener", "Enter host and port:", "localhost:9763")
            { }

            public String Host
            {
                get
                {
                    return Value.Split(':')[0];
				}
			}

            public int Port
            {
                get
                {
                    return int.Parse(Value.Split(':')[1]);
				}
			}

		}

        private void startListener(object sender, EventArgs e)
        {
			// Ask the user for the host + port.
            ConnectForm connectForm = new ConnectForm();
            if (connectForm.ShowDialog(this) == DialogResult.Cancel)
				return;

			SuitController controller = new XSensController(connectForm.Host, connectForm.Port);
            controller.Calibrate();
			controller.Connect();

            RecordingProvider provider = new SuitRecordingProvider(controller);
            provider.Init();
            Recording recording = new Recording(provider, new FixedBodyConfiguration());
            
            recording.Update();

			this.Recording = recording;
            leftSidePanel.Chart.Series.Clear();
			this.Text = "Motion Lie Detection - " + connectForm.Host + ":" + connectForm.Port;
		}
        
        private void stopListener(object sender, EventArgs e)
        {
            
        }

        private void openFile(object sender, EventArgs e)
        {
			// Show the file open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MVNX File Format|*.mvnx|Binary MoCap Format|*.bmocap";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
            DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

            // In case a recording is still open, close it.
            if (this.Recording != null)
                closeRecording(sender, e);

            RecordingProvider provider = FileRecordingProvider.AppropriateReader(dialog.FileName);
            provider.Init();
            Recording recording = new Recording(provider);
            recording.Update();

			this.Recording = recording;
            leftSidePanel.Chart.Series.Clear();
			this.Text = "Motion Lie Detection - " + dialog.SafeFileName;
		}

		private void saveFile(object sender, EventArgs e) 
		{
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "MVNX File Format|*.mvnx;|Binary MoCap Format|*.bmocap";
            DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

			// TODO: Perhaps notify the user if the recording hasn't ended or something?

            RecordingSaver saver = RecordingSaver.AppropriateSaver(dialog.FileName);
            saver.saveToFile(this.recording);
		}

        private void closeRecording(object sender, EventArgs e)
        {
            visualizer.Reset();

            //Set recording
            this.Recording = null;

            //Set Lieresult
            this.LieResult = new LieResult(0);

            this.context = new AlgorithmContext();

            //Set Timeline
            timeline.Reset(this.LieResult);
            timeline.Invalidate();
            visualizer.Invalidate();

            // Set the markpoint panel.
            rightSidePanel.Reset();
            leftSidePanel.Chart.Series.Clear();

			this.Text = "Motion Lie Detection";
		}

        private void exit(object sender, EventArgs e)
        {
			// TODO: Cleanup
            Environment.Exit(0);
		}

        private void startDummyStream(object sender, EventArgs e)
        {
            DummyStream stream = new DummyStream();
            stream.Start(false);
		}

        private void startEndlessDummyStream(object sender, EventArgs e)
        {
            DummyStream stream = new DummyStream();
            stream.Start(true);
        }

        private void resize(Object sender, EventArgs e)
        {
			Size newSize = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);

			// Place and scale the left sidebar.
            leftSidePanel.Location = new System.Drawing.Point(ControlMargin, ControlMargin);
            leftSidePanel.Width = 400 ;
			leftSidePanel.Height = newSize.Height - 4 * ControlMargin - 150 - 50;

            // Place the visualizer
            visualizer.Location = new System.Drawing.Point(leftSidePanel.Right + ControlMargin, ControlMargin);
            visualizer.Width = newSize.Width - 4 * ControlMargin - 400 - 200;
            visualizer.Height = newSize.Height - 4 * ControlMargin - 150 - 40;

			// Place and scale the right sidebar.
			rightSidePanel.Width = 200;
            rightSidePanel.Left = visualizer.Right + ControlMargin;
			rightSidePanel.Top = ControlMargin;
			rightSidePanel.Height = newSize.Height - 4 * ControlMargin - 150 - 40;

            // Place and scale the playback controls.
            playbackPanel.Location = new System.Drawing.Point(ControlMargin, visualizer.Bottom + ControlMargin);
            playbackPanel.Width = newSize.Width - 2 * ControlMargin;
            playbackPanel.Height = 40;

			// Place and scale the timeline.
            timeline.Location = new System.Drawing.Point(ControlMargin, playbackPanel.Bottom + ControlMargin);
			timeline.Width = newSize.Width - 2 * ControlMargin;
			timeline.Height = 150;
		}
    }
}
