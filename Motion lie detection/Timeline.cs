using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
	/**
	 * Special control that displays the timeline of a recording.
	 */
    public class Timeline : Control
    {
        /**
         * Margin that is applied once the number of frames in the recording exceed the capacity of the timeline.
         */
        public static readonly double FramesMargin = 0.3;

        /**
		 * The recording to visualize.
		 */
        private Recording recording;

        /**
		 * The LieResult to visualize.
		 */
        private LieResult lieresult;

        /**
         * Special markpoint that represents no selected/hovered markpoint.
         */
        private static readonly MarkPoint NONE = new MarkPoint(-1, null, -1);

        /**
         * The current frame the timeline is on.
         */
        private double currentFrame;

        /**
         * The current mark point that is hovered over.
         */
        private MarkPoint currentMarkPoint = NONE;

        /**
         * Previous update time.
         */
        private double previouseUpdateTime = -1.0;
        /**
         * Playback speed.
         */
        private float playBackSpeed = 1.0f;

        /**
         * The number of frames that are expected/available.
         */
        private int numberOfFrames;

        /**
         * Bool that shows if stepping mode is on
         */
        private bool playing = true;
        /**
         * Flag indicating that the timeline is at the end of a recording.
         */
        public bool atEnd = true;
        /**
         * Boolean if the player is looping.
         */
        private bool looping = false;

        /**
         * Selection segment.
         */
        private bool selection;
        private int selectionStart;
        private int selectionEnd;

        public Timeline()
            : this(1000) // FIXME: DEBUG VALUE
        {
        }

        public Timeline(int numberOfFrames)
        {
            // Note: set the style to prevent flickering of the contorl.
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);

            this.numberOfFrames = numberOfFrames;
        }

        public List<double> trafficclassif;
        public List<Tuple<double, double>> meanclassif;
        
        /**
         * Methods for handling events
         * @param e EventArgs object
         */
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Recording == null)
                return;

            // Two things to do:
            //  1) Check for hovering over markpoints.
            {
                // Set the currently hovered markpoint to none.
                currentMarkPoint = NONE;

                // Check if it was above one of the markpoints.
                foreach (MarkPoint mark in Recording.MarkPoints)
                {
                    float markPos = position(mark.Frameid);

                    if (e.Y >= 5 && e.Y <= 15 && Math.Abs(e.X - markPos) <= 5)
                    {
                        currentMarkPoint = mark;
                        break;
                    }
                }
            }

            //  2) Check for dragging the mouse over the timeline.
            {
                // If the mouse is down, move the currentFrame line.
                if (e.Button == MouseButtons.Left)
                {
                    CurrentPos = frameId((float)e.X);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            int frame = frameId((float)e.X);
            if (currentMarkPoint.Id != -1)
                frame = currentMarkPoint.Frameid;

            // Check for the right button.
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (selection)
                {
                    selection = false;
                    selectionStart = -1;
                    selectionEnd = -1;
                    return;
                }

                if (Math.Abs(selectionStart - frame) > 5)
                {
                    selection = true;
                    selectionEnd = frame;
                    // Swap if needed.
                    if (selectionEnd < selectionStart)
                    {
                        var tmp = selectionEnd;
                        selectionEnd = selectionStart;
                        selectionStart = tmp;
                    }
                }
                else
                {
                    selection = true;
                    selectionEnd = -2;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int frame = frameId((float)e.X);
            if (currentMarkPoint.Id != -1)
                frame = currentMarkPoint.Frameid;

            // Check for the right button.
            if (!selection && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                selection = false;
                selectionStart = frame;
                selectionEnd = -1;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int frame = frameId((float)e.X);
            if (currentMarkPoint.Id != -1)
                frame = currentMarkPoint.Frameid;

            // Update the current position to the frameId corresponding with the clicked point.
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                CurrentPos = frame;
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Clear the client rectangle
            pe.Graphics.FillRectangle(Brushes.White, ClientRectangle);
            pe.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
            pe.Graphics.FillRectangle(Brushes.LightGray, 0, 0, Width, 20);
            pe.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - 1, 19);
            pe.Graphics.FillRectangle(Brushes.LightGray, 0, Height - 20, Width, 20);
            pe.Graphics.DrawRectangle(Pens.Black, 0, Height - 20, Width - 1, 19);
            if (Recording == null)
                return;

            // FIXME: Perhaps check the cliprectangle to only draw what is necessary?
            paintHeader(pe.Graphics);
            paintBody(pe.Graphics);
            paintFooter(pe.Graphics);
        }

        /**
         * Methods for drawing the parts of the timeline
         * @param g Graphics object needed for drawing.
         */
        protected void paintHeader(Graphics g)
        {
            // Draw the markers.
            foreach (MarkPoint mark in Recording.MarkPoints)
            {
                float markPos = position(mark.Frameid);

                Brush colorBrush = Brushes.Orange;
                if (mark.Id == currentMarkPoint.Id)
                    colorBrush = Brushes.Red;
                g.FillEllipse(colorBrush, (int)markPos - 5, 5, 10, 10);
                g.DrawEllipse(Pens.Black, (int)markPos - 5, 5, 10, 10);
            }

            //if (meanclassif != null && meanclassif.Count >= 29)
            //{
            //    Brush meanbrush = Brushes.Orange;
            //    var trueTot = 1.0;
            //    var falseTot = 1.0;
            //    for (int i = 0; i < 5; i++)
            //    {
            //        double ptruth = meanclassif[24 + i].Item1;
            //        double plie = meanclassif[24 + i].Item2;

            //        trueTot *= ptruth;
            //        falseTot *= plie;
            //    }
            //    double result = trueTot / (trueTot + falseTot);

            //    if (result < 0.5f)
            //        meanbrush = Brushes.Red;
            //    else if (result > 0.5f)
            //        meanbrush = Brushes.Green;
                
            //    g.FillEllipse(meanbrush, Width - 20, 5, 10, 10);
            //    g.DrawString("Mean: " + result.ToString(), new Font("Arial", 10.0f), Brushes.Black, Width - 200, 5);
            //}
        }

        protected void paintBody(Graphics g)
        {
            // Draw the invalid area after the available frames.
            if (numberOfFrames > Recording.FrameCount)
            {
                float unavailablePos = position(Recording.FrameCount);
                g.FillRectangle(Brushes.DarkGray, (int)unavailablePos, 20, Width - (int)unavailablePos - 1, Height - 40);
            }

            if (LieResult != null)
            {
                float start = position(LieResult.Start);
                float end = position(LieResult.End);
                g.FillRectangle(Brushes.LightBlue, (int)start, 20, end - start, Height - 40);
            }

            // Draw the markpoint lines.
            Pen markPen = new Pen(Color.Gray);
            markPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            foreach (MarkPoint mark in Recording.MarkPoints)
            {
                float markPos = position(mark.Frameid);
                g.DrawLine(markPen, (int)markPos, 20, (int)markPos, Height - 20);
            }

            // Draw the selection.
            {
                float startPos = position(selectionStart);
                int endFrame = selectionEnd;
                if (selectionEnd == -2)
                    endFrame = Recording.FrameCount;
                if (!selection && MouseButtons == MouseButtons.Right)
                    endFrame = frameId((float)PointToClient(MousePosition).X);
                float endPos = position(endFrame);
                // Swap if needed
                if (endPos < startPos)
                {
                    var tmp = endPos;
                    endPos = startPos;
                    startPos = tmp;
                }

                g.DrawLine(Pens.Blue, (int)startPos, 20, (int)startPos, Height - 20);

                if (selection || MouseButtons == MouseButtons.Right)
                {
                    Brush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));
                    g.FillRectangle(brush, (int)startPos, 20, (int)endPos - (int)startPos, Height - 40);

                    g.DrawLine(Pens.Blue, (int)endPos, 20, (int)endPos, Height - 20);
                }
            }

            // Draw the red currentFrame line
            {
                float timePos = position(CurrentPos);
                g.DrawLine(Pens.Red, (int)timePos, 20, (int)timePos, Height - 20);
            }
        }

        protected void paintFooter(Graphics g)
        {
            //if(trafficclassif != null && recording != null && CurrentPos < lieresult.End){
            //    Brush trafficbrush = Brushes.Orange;
            //    if(trafficclassif[0]<0.5f)
            //        trafficbrush = Brushes.Red;
            //    if (trafficclassif[0] > 0.5f)
            //        trafficbrush = Brushes.Green;
            //    g.FillEllipse(trafficbrush, 250, Height - 20, 10, 10);

            //    g.DrawString("Truth likeliness: " + trafficclassif[0].ToString(), new Font("Arial", 10.0f), Brushes.Black, 400, Height - 20);
            //}

            // Draw the time.
            int seconds = (int)(currentFrame / Recording.FrameRate);
            String time = String.Format("{0:D2}:{1:D2}:{2:D2} ({3})", seconds / 3600, (seconds % 3600) / 60, seconds % 60, (int)currentFrame);
            g.DrawString(time, new Font("Arial", 10.0f), Brushes.Black, 10, Height - 20);

            // Draw the markpoint description
            if (currentMarkPoint.Id != -1)
            {
                String markpoint = String.Format("{0} @{1}", currentMarkPoint.Description, currentMarkPoint.Frameid);
                g.DrawString(markpoint, new Font("Arial", 10.0f), Brushes.Black, Width / 2, Height - 20);
            }
        }

        private double totalSeconds()
        {
            DateTime Jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = DateTime.UtcNow - Jan1970;
            return span.TotalSeconds;
        }

        /**
         * Method for updating the timeline to incorporate new frames.
         * @param margin The margin to take the new amount of numberOfFrames.
         */
        public new void Update()
        {
            double nowUpdateTime = totalSeconds();

            // Determine the delta between the updates.
            if (previouseUpdateTime == -1)
            {
                // FIXME: At this rate the first update will be a dummy.
                // Perhaps let the timeline prepare in case of play/start?
                previouseUpdateTime = nowUpdateTime;
                return;
            }

            // Check if the timeline is playing.
            if (playing)
            {
                // TODO: Special method to snap to the end in case it's a live recording.
                if (atEnd)
                {
                    currentFrame = recording.FrameCount - 1;
                }
                else
                {
                    // Determine the time delta since the last update.
                    double deltaSeconds = nowUpdateTime - previouseUpdateTime;
                    double newCurrentFrame = (float)(currentFrame + (deltaSeconds * Recording.FrameRate) * playBackSpeed);
                    setCurrentFrame(newCurrentFrame);
                }
            }

            // Now check for looping within the selection.
            if (selection && looping)
            {
                if (currentFrame > SelectionEnd)
                {
                    double newCurrentFrame = currentFrame - SelectionEnd + SelectionStart;
                    setCurrentFrame(newCurrentFrame);
                }
            }

            // Regardless if we're playing or not, update the previous update time.
            // Note: this prevents large time delta's to occur due to pausing/playing.
            previouseUpdateTime = nowUpdateTime;

            // -------
            // Check if the number of frames has exceeded the capacity.
            if (recording.FrameCount > numberOfFrames)
            {
                numberOfFrames = (int)(recording.FrameCount * (1.0 + FramesMargin));
            }

            if (recording != null && CurrentPos < lieresult.End)
            {
                trafficclassif = Classification.ClassifyParts(recording.ClassificationConfiguration, lieresult, CurrentPos);
            }

            if (CurrentPos == lieresult.End)
            {
                meanclassif = Classification.ClassifyMeansBoth(recording.ClassificationConfiguration, lieresult);
            }

            // Let the timeline be redrawn and update the control.
            this.Invalidate();
            base.Update();
        }

        /**
         * Method for converting from the integer frameId to the (pixel) position on the control.
         * @param frameId The frame id to compute the position for.
         * @return The x position corresponding to the frame id. 
         */
        private float position(int frameId)
        {
            return (float)frameId * ((float)Width / (float)numberOfFrames);
        }

        /**
         * Method for converting from the (pixel) position on the control to the frame id.
         * @param position The x position to compute the frame id for.
         * @return The frame id corresponding to the position.
         */
        private int frameId(float position)
        {
            return (int)(position * (float)numberOfFrames / Width);
        }

        private void setCurrentFrame(double value)
        {
            // Initially set the current frame to the dictated value.
            currentFrame = value;
            atEnd = false;

            // Now clamp the value between 0 and the frame count.
            if (currentFrame < 0.0)
                currentFrame = 0.0;
            else if (currentFrame >= Recording.FrameCount - 1)
            {
                currentFrame = Recording.FrameCount - 1;
                // Note: only set AtEnd if the playing speed is above 1.00 (and actually playing)
                atEnd = playBackSpeed >= 1.0 && playing;
            }

        }

        /**
         * Property that gives the current frameid selected in the timeline
         * Or sets current id to given value and makes sure the set value is limited between min and max frames
         * @param value Int frameid to set
         */
        public int CurrentPos
        {
            get
            {
                return (int)currentFrame;
            }
            set
            {
                if (Recording != null)
                    setCurrentFrame(value);
                else
                    currentFrame = 0;
                this.Invalidate();
            }
        }
        
        /**
         * Property that gives the current Frame selected in the timeline
         */
        public Frame CurrentFrame
        {
            get { return recording.GetFrame(CurrentPos); }
        }

        /**
         * Property that gives the current BodyConfiguration in the timeline
         */
        public BodyConfiguration BodyConfiguration
        {
            get{ return (recording != null) ? recording.BodyConfiguration: null;}
        }
        
        /**
         * Property that gets or sets recording from the timeline
         */
        public Recording Recording
        {
            get { return recording; }
            set { recording = value; CurrentPos = 0; }
        }

        public LieResult LieResult
        {
            get { return lieresult; }
            set { lieresult = value;}
        }

        public float PlayBackSpeed
        {
            get { return playBackSpeed; }
            set { 
                playBackSpeed = value;
                // The playback will only stay at the end if the playback speed is greater than or equal to one.
                if (atEnd && playBackSpeed < 1.0)
                    atEnd = false;
            }
        }

        public bool Playing
        {
            get { return playing; }
            set {
                playing = value;
                atEnd &= playing;
            }
        }

        public bool Looping
        {
            get { return looping; }
            set
            {
                looping = value;
                //atEnd &= playing;
            }
        }

        public int SelectionStart
        {
            get { return selection ? Math.Max(selectionStart, 0) : 0; }
        }

        public int SelectionEnd
        {
            get
            {
                if (selection && selectionEnd != -2)
                    return Math.Min(selectionEnd, lieresult.End);
                else
                    return lieresult.End;
            }
        }
    }
}

