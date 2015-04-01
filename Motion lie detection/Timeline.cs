using System;
using System.Drawing;
using System.Windows.Forms;

namespace Motion_lie_detection
{
	public class Timeline : Control
	{
		private int currentPos;
		private MarkPoint currentMarkPoint;

		public Timeline ()
		{
			// FIXME: Prevent flickering
			SetStyle(ControlStyles.OptimizedDoubleBuffer | 
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint, true);
		}

		protected override void OnMouseMove (MouseEventArgs e)
		{
			// Check if it was above one of the markpoints.
			foreach (MarkPoint mark in Recording.MarkPoints) {
				float markPos = position (mark.Frameid);
				if (e.Y >= 5 && e.Y <= 15 && Math.Abs (e.X - markPos) <= 5) {
					currentMarkPoint = mark;
					return;
				}
			}

			// FIXME: Special unselected markpoint
			currentMarkPoint = new MarkPoint (-1, null, -1);
		}

		protected override void OnMouseClick (MouseEventArgs e)
		{
			// Update the current position to the frameId corresponding with the clicked point.
			int frame = frameId ((float)e.X);
			CurrentPos = frame;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Console.WriteLine ("HERE");
			pe.Graphics.FillRectangle (Brushes.White, ClientRectangle);
			pe.Graphics.DrawRectangle (Pens.Black, 0, 0, Width - 1, Height - 1);
			if (Recording == null) {
				return;
			}

			// FIXME: Perhaps check the cliprectangle to only draw what is necessary?
			paintHeader (pe.Graphics);
			paintBody (pe.Graphics);
			paintFooter (pe.Graphics);
		}

		protected void paintHeader(Graphics g) {
			g.FillRectangle (Brushes.LightGray, 0, 0, Width, 20);
			g.DrawRectangle (Pens.Black, 0, 0, Width - 1, 19);

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
			int length = Recording.FrameCount;

			float timePos = position (CurrentPos);
			g.DrawLine (Pens.Red, (int)timePos, 20, (int)timePos, Height - 20);
		}

		protected void paintFooter(Graphics g) {
			g.FillRectangle (Brushes.LightGray, 0, Height - 20, Width, 20);
			g.DrawRectangle (Pens.Black, 0, Height - 20, Width - 1, 19);

			// Draw the time.
			int seconds = currentPos / Recording.FrameRate;
			String time = String.Format ("{0:D2}:{1:D2}:{2:D2} ({3})", seconds / 3600, (seconds % 3600) / 60, seconds % 60, currentPos);
			g.DrawString (time, new Font ("Arial", 10.0f), Brushes.Black, 10, Height - 20);

			// Draw the markpoint description
			if(currentMarkPoint.Id != -1) {
				String markpoint = String.Format ("{0} @{1}", currentMarkPoint.Description, currentMarkPoint.Frameid);
				g.DrawString (markpoint, new Font ("Arial", 10.0f), Brushes.Black, Width / 2, Height - 20);
			}
		}

		private float position(int frameId) {
			return (float)frameId * ((float)Width / (float)Recording.FrameCount);
		}

		private int frameId(float position) {
			return (int)(position * (float)Recording.FrameCount / Width);
		}

		public int CurrentPos { 
			get {
				return currentPos;
			}
			set { 
				currentPos = Math.Max (Math.Min (value, Recording.FrameCount), 0);
				this.Invalidate ();
			}
		}

		public Recording Recording { get; set; }
	}
}

