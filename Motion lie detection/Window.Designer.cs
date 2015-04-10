using System;
using System.Collections.Generic;
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
        Panel leftSidePanel;
        RightSidePanel rightSidePanel;

		private void InitializeComponent()
        {


            this.visualizer = new Visualizer();
            this.visualizer.Name = "visualizer";
            this.visualizer.TabIndex = 0;
            this.Controls.Add(this.visualizer);

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
            this.leftSidePanel = new Panel();
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
			this.Text = "Motion Lie Detection - " + connectForm.Host + ":" + connectForm.Port;
		}

        private void openFile(object sender, EventArgs e)
        {
			// Show the file open dialog
            OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "mvnx";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
            DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

            RecordingProvider provider = new FileRecordingProvider(dialog.FileName);
            provider.Init();
            Recording recording = new Recording(provider);
            recording.Update();

			this.Recording = recording;
			this.Text = "Motion Lie Detection - " + dialog.SafeFileName;
		}

		private void saveFile(object sender, EventArgs e) 
		{
            SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = "mvnx";
            DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.Cancel)
				return;

			// TODO: Perhaps notify the user if the recording hasn't ended or something?

            RecordingSaver saver = new MVNXSaver(dialog.FileName); // FIXME: Hard-coded implementation for Recording saver.
            saver.saveToFile(this.recording);
		}

        private void closeRecording(object sender, EventArgs e)
        {
            //// FIXME: Close the recording (and provider) correctly.
            //Recording = null;
            //LieResult = new LieResult();
            //timeline = new Timeline();
            //timeline.LieResult = LieResult;

            visualizer.Reset();

            //Set recording
            this.Recording = null;

            //Set Lieresult
            this.LieResult = new LieResult(0);

            this.context = new AlgorithmContext();

            //Set Timeline
            timeline.LieResult = LieResult;
            timeline.CurrentPos = 0;
            timeline.Invalidate();
            visualizer.Invalidate();

            // Set the markpoint panel.
            rightSidePanel.Reset();

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
            stream.Start();
		}

        private void resize(Object sender, EventArgs e)
        {
			Size newSize = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);

			// Place and scale the left sidebar.
            leftSidePanel.Location = new System.Drawing.Point(ControlMargin, ControlMargin);
			leftSidePanel.Width = 200;
			leftSidePanel.Height = newSize.Height - 4 * ControlMargin - 150 - 50;

            // Place the visualizer
            visualizer.Location = new System.Drawing.Point(leftSidePanel.Right + ControlMargin, ControlMargin);
            visualizer.Width = newSize.Width - 4 * ControlMargin - 200 - 200;
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

        public class PlaybackPanel : Panel
        {
            /**
             * The timeline control that is influenced by these playback controls.
             */
            private Timeline timeline;

            /**
             * The different playback buttons.
             */
            private Button toStart;
            private Button toPrevMarker;
            private Button pause;
            private Button toNextMarker;
            private Button toEnd;

            private CheckBox loop;
            private TextBox speed;

            public PlaybackPanel(Timeline timeline)
            {
                this.timeline = timeline;

                toStart = new Button();
                toStart.Text = "|<<";
                toStart.Click += (obj, e) =>
                {
                    // Simply reset the timeline to the initial point.
                    timeline.CurrentPos = 0;
                };
                this.Controls.Add(toStart);

                toPrevMarker = new Button();
                toPrevMarker.Text = "<<";
                toPrevMarker.Click += (obj, e) =>
                {
                    // TODO: Somehow determine the previous mark point and move the timeline.
                    // Note: probably best to move to the start if no markpoints are present between start and now.
                };
                this.Controls.Add(toPrevMarker);

                pause = new Button();
                pause.Text = "||";
                pause.Click += ((obj, e) => { 
                    // Simply invert the playing status of the timeline.
                    timeline.Playing = !timeline.Playing;
                    ((Button)obj).Text = timeline.Playing ? "||" : ">";
                });
                this.Controls.Add(pause);

                toNextMarker = new Button();
                toNextMarker.Text = ">>";
                toNextMarker.Click += (obj, e) =>
                {
                    // TODO: Somehow determine the next mark point and move the timeline.
                    // Note: probably best to move to the end if no markpoints are present between now and end.
                };
                this.Controls.Add(toNextMarker);

                toEnd = new Button();
                toEnd.Text = ">>|";
                toEnd.Click += (obj, e) =>
                {
                    // FIXME: Somehow signal the timeline that the timeline should follow the end.
                    if(timeline.Recording != null)
                      timeline.CurrentPos = timeline.Recording.FrameCount;
                };
                this.Controls.Add(toEnd);

                loop = new CheckBox();
                loop.Text = "loop";
                loop.CheckedChanged += (obj, e) =>
                {

                };
                this.Controls.Add(loop);

                speed = new TextBox();
                speed.Text = "1.00";
                speed.Top = 15;
                speed.Height = 20;
                speed.TextChanged += (obj, e) =>
                {
                    // Try to convert the text to a float.
                    float playSpeed;
                    // Behold! This nice way C# allows you to specifiy the float number format.
                    if (float.TryParse(speed.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out playSpeed))
                    {
                        timeline.PlayBackSpeed = playSpeed;
                    }
                    //float playSpeed = Float.speed.Text
                };
                this.Controls.Add(speed);
            }

            protected override void OnResize(EventArgs eventargs)
            {
                Size newSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

                int controlsWidth = newSize.Width / 2;
                int buttonWidth = controlsWidth / 5;
                toStart.Left = controlsWidth / 2;
                toStart.Top = 0;
                toStart.Width = buttonWidth;
                toStart.Height = newSize.Height;

                toPrevMarker.Left = controlsWidth / 2 + buttonWidth;
                toPrevMarker.Top = 0;
                toPrevMarker.Width = buttonWidth;
                toPrevMarker.Height = newSize.Height;

                pause.Left = controlsWidth / 2 + 2 * buttonWidth;
                pause.Top = 0;
                pause.Width = buttonWidth;
                pause.Height = newSize.Height;

                toNextMarker.Left = controlsWidth / 2 + 3 * buttonWidth;
                toNextMarker.Top = 0;
                toNextMarker.Width = buttonWidth;
                toNextMarker.Height = newSize.Height;

                toEnd.Left = controlsWidth / 2 + 4 * buttonWidth;
                toEnd.Top = 0;
                toEnd.Width = buttonWidth;
                toEnd.Height = newSize.Height;

                loop.Left = toEnd.Right + ControlMargin;
                loop.Top = 0;
                loop.Width = buttonWidth;
                loop.Height = newSize.Height;

                speed.Left = newSize.Width - buttonWidth;
                speed.Top = 15;
                speed.Width = buttonWidth;
            }
        }

		public class RightSidePanel : Panel
		{
            private Timeline timeline;

            private Label title;
			private ListBox markpointBox;
			private Button addButton;
			private Button removeButton;

			public RightSidePanel(Timeline timeline)
			{
                this.timeline = timeline;

                title = new Label();
                title.Text = "Markpoints";
                title.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(title);

                markpointBox = new ListBox();
                markpointBox.MouseDoubleClick += (obj, e) =>
                {
                    int index = markpointBox.IndexFromPoint(e.Location);
                    if (index != System.Windows.Forms.ListBox.NoMatches)
                    {
                        // Jump to the markpoint on the timeline.
                        MarkPoint markpoint = (MarkPoint)markpointBox.Items[index];

                        timeline.CurrentPos = markpoint.Frameid;
                    }
                };
                this.Controls.Add(markpointBox);

                addButton = new Button();
				addButton.Text = "Add";
                addButton.Click += (obj, e) =>
                {
                    if (timeline.Recording == null)
                        return;

                    // Show dialog for description.
                    InputForm dialog = new InputForm("Add markpoint", "Enter description:", "");
                    if (dialog.ShowDialog(this) == DialogResult.Cancel)
                        return;

                    MarkPoint newPoint = new MarkPoint(timeline.Recording.MarkPoints.Count, dialog.Value, timeline.CurrentPos);
                    timeline.Recording.AddMarkPoint(newPoint);
                    markpointBox.Items.Add(newPoint);
                };
                this.Controls.Add(addButton);

                removeButton = new Button();
				removeButton.Text = "Remove";
                removeButton.Click += (obj, e) =>
                {
                    if(timeline.Recording == null)
                        return;

                    // Get the selected items.
                    MarkPoint markPoint = (MarkPoint)markpointBox.SelectedItem;
                    timeline.Recording.RemoveMarkPoint(markPoint);
                    markpointBox.Items.Remove(markPoint);
                };
                this.Controls.Add(removeButton);

				this.Resize += resize;
			}
            
            private void resize(Object sender, EventArgs e)
            {
                Size newSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

                title.Left = 0;
                title.Top = 0;
                title.Width = newSize.Width;
                title.Height = 20;

				markpointBox.Left = 0;
				markpointBox.Top = 20 + ControlMargin;
				markpointBox.Width = newSize.Width;
				markpointBox.Height = newSize.Height - 40 - 2 * ControlMargin;

				addButton.Top = markpointBox.Bottom + ControlMargin;
				addButton.Left = ControlMargin;
				addButton.Width = (newSize.Width - 3 * ControlMargin) / 2;
				addButton.Height = 20;

				removeButton.Top = markpointBox.Bottom + ControlMargin;
				removeButton.Left = addButton.Right + ControlMargin;
				removeButton.Width = (newSize.Width - 3 * ControlMargin) / 2;
				removeButton.Height = 20;
			}

            public void Reset() {
                // Clear out the markpoints.
                markpointBox.Items.Clear();
            }
		}
    }
}
