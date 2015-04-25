using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
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
            toPrevMarker.Click += PreviousMarkpoint;
            this.Controls.Add(toPrevMarker);

            pause = new Button();
            pause.Text = "||";
            pause.Click += ((obj, e) =>
            {
                // Simply invert the playing status of the timeline.
                timeline.Playing = !timeline.Playing;
                ((Button)obj).Text = timeline.Playing ? "||" : ">";
            });
            this.Controls.Add(pause);

            toNextMarker = new Button();
            toNextMarker.Text = ">>";
            toNextMarker.Click += NextMarkpoint;
            this.Controls.Add(toNextMarker);

            toEnd = new Button();
            toEnd.Text = ">>|";
            toEnd.Click += (obj, e) =>
            {
                // FIXME: Somehow signal the timeline that the timeline should follow the end.
                if (timeline.Recording != null)
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

        public void PreviousMarkpoint(object sender, EventArgs e)
        {
            if (timeline.Recording == null)
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
        }

        public void NextMarkpoint(object sender, EventArgs e)
        {
            if (timeline.Recording == null)
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
            if (markpoint != null)
                timeline.CurrentPos = markpoint.Frameid;
            else
                timeline.CurrentPos = timeline.Recording.FrameCount;
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

            loop.Left = toEnd.Right + Window.ControlMargin;
            loop.Top = 0;
            loop.Width = buttonWidth;
            loop.Height = newSize.Height;

            speed.Left = newSize.Width - buttonWidth;
            speed.Top = 15;
            speed.Width = buttonWidth;
        }
    }
}
