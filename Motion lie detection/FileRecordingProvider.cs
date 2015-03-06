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

		public override BodyConfiguration getBodyConfiguration ()
		{
			return null;
		}
	}
}
