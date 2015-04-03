using System;
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
		 * Special markpoint that represents no selected/hovered markpoint.
		 */
		private static readonly MarkPoint NONE = new MarkPoint(-1, null, -1);

		/**
		 * The current frame the timeline is on.
		 */
		private int currentFrame;
		/**
		 * The current mark point that is hovered over.
		 */
		private MarkPoint currentMarkPoint = NONE;

		/**
		 * The number of frames that are expected/available.
		 */
		private int numberOfFrames;

		public Timeline() : this(1000) // FIXME: DEBUG VALUE
		{
		}

		public Timeline (int numberOfFrames)
		{
			// Note: set the style to prevent flickering of the contorl.
			SetStyle(ControlStyles.OptimizedDoubleBuffer | 
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint, true);

			this.numberOfFrames = numberOfFrames;
		}

		protected override void OnMouseMove (MouseEventArgs e)
		{
			if (Recording == null)
				return;

			// Two things to do:
			//  1) Check for hovering over markpoints.
			{
				// Set the currently hovered markpoint to none.
				currentMarkPoint = NONE;

				// Check if it was above one of the markpoints.
				foreach (MarkPoint mark in Recording.MarkPoints) {
					float markPos = position (mark.Frameid);
					if (e.Y >= 5 && e.Y <= 15 && Math.Abs (e.X - markPos) <= 5) {
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
					CurrentPos = frameId ((float)e.X);
				}
			}
		}

		protected override void OnMouseClick (MouseEventArgs e)
		{
			// Update the current position to the frameId corresponding with the clicked point.
			int frame = frameId ((float)e.X);
			CurrentPos = frame;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			// Clear the client rectangle
			pe.Graphics.FillRectangle (Brushes.White, ClientRectangle);
			pe.Graphics.DrawRectangle (Pens.Black, 0, 0, Width - 1, Height - 1);
			pe.Graphics.FillRectangle (Brushes.LightGray, 0, 0, Width, 20);
			pe.Graphics.DrawRectangle (Pens.Black, 0, 0, Width - 1, 19);
			pe.Graphics.FillRectangle (Brushes.LightGray, 0, Height - 20, Width, 20);
			pe.Graphics.DrawRectangle (Pens.Black, 0, Height - 20, Width - 1, 19);
			if (Recording == null)
				return;

			// FIXME: Perhaps check the cliprectangle to only draw what is necessary?
			paintHeader (pe.Graphics);
			paintBody (pe.Graphics);
			paintFooter (pe.Graphics);
		}

		protected void paintHeader(Graphics g) {
			// Draw the markers.
			foreach (MarkPoint mark in Recording.MarkPoints) {
				float markPos = position (mark.Frameid);

				Brush colorBrush = Brushes.Orange;
				if (mark.Id == currentMarkPoint.Id)
					colorBrush = Brushes.Red;
				g.FillEllipse (colorBrush, (int)markPos - 5, 5, 10, 10); 
				g.DrawEllipse (Pens.Black, (int)markPos - 5, 5, 10, 10); 
			}
		}

		protected void paintBody(Graphics g) {
			// Draw the invalid area after the available frames.
			if (numberOfFrames > Recording.FrameCount) {
				float unavailablePos = position (Recording.FrameCount);
				g.FillRectangle (Brushes.DarkGray, (int)unavailablePos, 20, Width - (int)unavailablePos - 1, Height - 40);
			}

			// Draw the markpoint lines.
			Pen markPen = new Pen (Color.Gray);
			markPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			foreach (MarkPoint mark in Recording.MarkPoints) {
				float markPos = position (mark.Frameid);
				g.DrawLine (markPen, (int)markPos, 20, (int)markPos, Height - 20);
			}

			// Draw the red currentFrame line
			float timePos = position (CurrentPos);
			g.DrawLine (Pens.Red, (int)timePos, 20, (int)timePos, Height - 20);
		}

		protected void paintFooter(Graphics g) {
			// Draw the time.
			int seconds = currentFrame / Recording.FrameRate;
			String time = String.Format ("{0:D2}:{1:D2}:{2:D2} ({3})", seconds / 3600, (seconds % 3600) / 60, seconds % 60, currentFrame);
			g.DrawString (time, new Font ("Arial", 10.0f), Brushes.Black, 10, Height - 20);

			// Draw the markpoint description
			if(currentMarkPoint.Id != -1) {
				String markpoint = String.Format ("{0} @{1}", currentMarkPoint.Description, currentMarkPoint.Frameid);
				g.DrawString (markpoint, new Font ("Arial", 10.0f), Brushes.Black, Width / 2, Height - 20);
			}
		}

		/**
		 * Method for updating the timeline to incorporate new frames.
		 * @param margin The margin to take the new amount of numberOfFrames.
		 */
		public void Update(float margin) {
			// FIXME: Did do we why no need this, perhaps?
		}

		/**
		 * Method for converting from the integer frameId to the (pixel) position on the control.
		 * @param frameId The frame id to compute the position for.
		 * @return The x position corresponding to the frame id. 
		 */
		private float position(int frameId) {
			return (float)frameId * ((float)Width / (float)numberOfFrames);
		}

		/**
		 * Method for converting from the (pixel) position on the control to the frame id.
		 * @param position The x position to compute the frame id for.
		 * @return The frame id corresponding to the position.
		 */
		private int frameId(float position) {
			return (int)(position * (float)numberOfFrames / Width);
		}

		public int CurrentPos { 
			get {
				return currentFrame;
			}
			set { 
				if(Recording != null)
					currentFrame = Math.Max (Math.Min (value, Recording.FrameCount), 0);
				else
					currentFrame = 0;
				this.Invalidate ();
			}
		}

		public Recording Recording { get; set; }
	}
}

