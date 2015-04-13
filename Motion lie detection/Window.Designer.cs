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
            Settings.MenuItems.Add(new MenuItem("Start endless dummy stream", new EventHandler(startEndlessDummyStream)));
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
            timeline.LieResult = LieResult;
            timeline.CurrentPos = 0;
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
                    if(timeline.Recording == null)
                        return;

                    // Seek the best fitting markpoint.
                    MarkPoint markpoint = null;
                    int now = timeline.CurrentPos;
                    foreach (MarkPoint point in timeline.Recording.MarkPoints)
                    {
                        if (point.Frameid < now)
                            markpoint = point;
                        else
                            break;
                    }
                    if (markpoint != null)
                        timeline.CurrentPos = markpoint.Frameid;
                    else
                        timeline.CurrentPos = 0;
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
                    if(timeline.Recording == null)
                        return;

                    // Seek the best fitting markpoint.
                    MarkPoint markpoint = null;
                    int now = timeline.CurrentPos;
                    foreach (MarkPoint point in timeline.Recording.MarkPoints)
                    {
                        if (point.Frameid > now)
                        {
                            markpoint = point;
                            break;
                        }
                    }
                    if(markpoint != null)
                        timeline.CurrentPos = markpoint.Frameid;
                    else
                        timeline.CurrentPos = timeline.Recording.FrameCount;
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
                    timeline.Looping = loop.Checked;
                };
                this.Controls.Add(loop);

                speed = new TextBox();
                speed.Text = "1.00";
                speed.Top = 15;
                speed.Height = 20;
                speed.LostFocus += (obj, e) =>
                {
                    // Try to convert the text to a float.
                    float playSpeed;
                    // Behold! This nice way C# allows you to specifiy the float number format.
                    if (float.TryParse(speed.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out playSpeed))
                        timeline.PlayBackSpeed = playSpeed;
                };
                speed.KeyPress += (obj, e) =>
                {
                    if (e.KeyChar != 13)
                        return;

                    // Try to convert the text to a float.
                    float playSpeed;
                    // Behold! This nice way C# allows you to specifiy the float number format.
                    if (float.TryParse(speed.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out playSpeed))
                    {
                        // Note: it's possible for parsing an empty string as a speed, we 
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

        public class LeftSidePanel : Panel
        {
            private Timeline timeline;

            private Panel light;
            private Label titleLabel;
            private Label absoluteMovement;

            private List<Tuple<double, double>> results = null;
            public Chart Chart;

            public LeftSidePanel(Timeline timeline)
            {
                this.timeline = timeline;

                light = new Light();
                light.Paint += (obj, e) => {
                    Graphics g = e.Graphics;

                    // Determine the color
                    Brush meanbrush = Brushes.Orange;
                    if (results != null)
                    {
                        var trueTot = 1.0;
                        var falseTot = 1.0;
                        for (int i = 0; i < 5; i++)
                        {
                            double ptruth = results[24 + i].Item1;
                            double plie = results[24 + i].Item2;

                            trueTot *= ptruth;
                            falseTot *= plie;
                        }

                        double result = trueTot / (trueTot + falseTot);
                        if (result < 0.5f)
                            meanbrush = Brushes.Red;
                        else if (result > 0.5f)
                            meanbrush = Brushes.Green;
                    }

                    // Draw the ellipse
                    Rectangle draw = ((Panel)obj).ClientRectangle;
                    draw.Inflate(-20, -20);
                    if (draw.Height > draw.Width)
                    {
                        int diff = draw.Height - draw.Width;
                        draw.Height = draw.Width;
                        draw.Y += diff/2;
                    }
                    else
                    {
                        int diff = draw.Width - draw.Height;
                        draw.Width = draw.Height;
                        draw.X += diff / 2;
                    }
                    g.FillEllipse(meanbrush, draw);
                };
                this.Controls.Add(light);

                titleLabel = new Label();
                titleLabel.Text = "Frame dgiference";
                titleLabel.TextAlign = ContentAlignment.MiddleCenter;
                titleLabel.Font = new Font(titleLabel.Font.FontFamily, titleLabel.Font.Size, FontStyle.Bold);
                //this.Controls.Add(titleLabel);

                absoluteMovement = new Label();
                absoluteMovement.Text = "";
                absoluteMovement.TextAlign = ContentAlignment.MiddleLeft;
                this.Controls.Add(absoluteMovement);

                Chart = new Chart();

                //Chart Settings 
                // Populating the data arrays.
                this.Chart.Series.Clear();

                this.Chart.Palette = ChartColorPalette.Pastel;

                // Set chart title.
                this.Chart.Titles.Add("Frame difference");
                
                // add a chart legend. neat right.
                this.Chart.Legends.Add("chart legend");
                Chart.ResetAutoValues();
                Chart.ChartAreas.Add(new ChartArea());
                Chart.ChartAreas[0].AxisY.Maximum = 0.02;
                Chart.ChartAreas[0].AxisY.Minimum = 0;
                Chart.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                Chart.ChartAreas[0].AxisY.Title = "difference";
                Chart.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.Controls.Add(Chart);

                this.Resize += resize;
            }

            public new void Update()
            {
               

          

           
                String text = "";
                if (timeline.Recording != null)
                {
                    
                    }
                    // update the traffic light
                    List<double> movement = timeline.LieResult.ComputeAbsoluteMovements(timeline.SelectionStart, timeline.SelectionEnd);
                  
                    

                    if (movement != null)
                    {
                        BodyConfiguration bodyConfiguration = timeline.Recording.BodyConfiguration;
                        int index = bodyConfiguration.Size;

     
                        var abs = movement[index++];

                        text += String.Format("Abs. movement: {0:0.000}\n", abs);

              
                        foreach (BodyNode node in timeline.Recording.ClassificationConfiguration.Rootnodes)
                        {
                            var value = movement[index++];
           
                            text += String.Format("Limb {0}: {1:0.000}\n", node.getName(), value);
            
                        }

                        this.results = Classification.ClassifyBoth(timeline.Recording.ClassificationConfiguration, movement);
                        light.Invalidate();
                    }
                
            
                absoluteMovement.Text = text;
                absoluteMovement.Invalidate();
                base.Update();
            }

            private void resize(Object sender, EventArgs e)
            {
                Size newSize = new Size(ClientRectangle.Width, ClientRectangle.Height);

                light.Left = 0;
                light.Top = 0;
                light.Width = newSize.Width / 2;
                light.Height = newSize.Height / 2;
                light.Invalidate();

                absoluteMovement.Left = newSize.Width / 2;
                absoluteMovement.Top = 0;
                absoluteMovement.Width = newSize.Width / 2;
                absoluteMovement.Height = newSize.Height / 2;

                Chart.Left = 0;
                Chart.Top = newSize.Height / 2 + 20;
                Chart.Width = newSize.Width;
                Chart.Height = newSize.Height / 2 - 20;
            }

            public class Light : Panel
            {
                public Light()
                {
                    // Note: set the style to prevent flickering of the contorl.
                    SetStyle(ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.UserPaint |
                        ControlStyles.AllPaintingInWmPaint, true);
                }
            }
        }

		public class RightSidePanel : Panel
		{
            private Timeline timeline;

            private Label title;
			private ListBox markpointBox;
			private Button addButton;
			private Button removeButton;

            /**
             * Special text box that is normally hidden, but shows to let the user edit the description of a markpoint.
             */
            private TextBox editBox;

			public RightSidePanel(Timeline timeline)
			{
                this.timeline = timeline;

                title = new Label();
                title.Text = "Markpoints";
                title.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(title);

                markpointBox = new ListBox();
                markpointBox.KeyPress += (obj, e) =>
                {
                    if(e.KeyChar == 13)
                        CreateEditBox();
                };
                markpointBox.KeyDown += (obj, e) =>
                {
                    if (e.KeyCode == Keys.F2)
                        CreateEditBox();
                };
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

                    // Insert the markpoint with an automatic description.
                    int id = timeline.Recording.MarkpointId;
                    MarkPoint newPoint = new MarkPoint(id, "Markpoint #" + (id + 1), timeline.CurrentPos);
                    timeline.Recording.AddMarkPoint(newPoint);
                    int index = timeline.Recording.MarkPoints.IndexOf(newPoint);
                    markpointBox.Items.Insert(index, newPoint);
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

                // Create the edit box.
                editBox = new TextBox();
                editBox.Left = 0;
                editBox.Top = 0;
                editBox.Size = new Size(0,0);
                editBox.Hide();
                markpointBox.Controls.Add(editBox);
                editBox.Text = "";
                editBox.BackColor = Color.Beige;
                editBox.ForeColor = Color.Blue;
                editBox.BorderStyle = BorderStyle.FixedSingle;
                editBox.KeyPress += (obj, e) =>
                {
                    if (e.KeyChar == 13)
                    {
                        int index = markpointBox.SelectedIndex;
                        MarkPoint markPoint = (MarkPoint)markpointBox.Items[index];
                        markPoint.Description = editBox.Text;
                        editBox.Hide();

                        markpointBox.DisplayMember = DateTime.UtcNow.ToString();
                    }

                };
                editBox.LostFocus += (obj, e) =>
                {
                    int index = markpointBox.SelectedIndex;
                    MarkPoint markPoint = (MarkPoint)markpointBox.Items[index];
                    markPoint.Description = editBox.Text;
                    editBox.Hide();

                    markpointBox.DisplayMember = DateTime.UtcNow.ToString();
                };

				this.Resize += resize;
			}

            private void CreateEditBox()
            {
                int itemSelected = markpointBox.SelectedIndex ;
                Rectangle r = markpointBox.GetItemRectangle(itemSelected);
                string itemText = ((MarkPoint)markpointBox.Items[itemSelected]).Description;

                editBox.Left = r.X;// + delta;
                editBox.Top = r.Y;// + delta;
                editBox.Width = r.Width - 10;
                editBox.Height = r.Height;// - delta;
                editBox.Show();

                markpointBox.Controls.Add(editBox);
                editBox.Text = itemText ;
                editBox.Focus();
                editBox.SelectAll();
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
