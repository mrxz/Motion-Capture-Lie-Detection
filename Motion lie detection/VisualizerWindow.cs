using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Motion_lie_detection
{
	using GdiColor = System.Drawing.Color;
	using XnaColor = Microsoft.Xna.Framework.Color;

	public partial class VisualizerWindow : Form
	{
		public VisualizerWindow(Recording recording)
		{
			InitializeComponents ();
		}
	}

}
