using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
	public class FileRecordingProvider : RecordingProvider
	{
		public override int GetFrameRate ()
		{
			throw new NotImplementedException ();
		}

		public override BodyConfiguration GetBodyConfiguration ()
		{
			return null;
		}

		public override List<Frame> GetNewFrames()
		{
			return null;
		}
	}
}
