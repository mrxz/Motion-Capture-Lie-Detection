﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_lie_detection
{
    public class LieResult
    {
        private Recording recording;
        private int framestart;
        private int framend;
        private List<Frame> filteredFrames;
        private List<List<float>> frameDifferences;
        private float meandiff;

        public LieResult(Recording recording, int framestart, Frame start)
        {
            this.recording = recording;
            this.filteredFrames = new List<Frame>();
            this.frameDifferences = new List<List<float>>();
            this.framestart = (framestart > 0) ? framestart : 0;
            if (Frame.IsEmpty(start)) 
            {
                this.framend = framestart - 1;
            }
            else
            {                
                this.filteredFrames.Add(start);
                this.framend = framestart;
            }
        }

        public static LieResult Empty(Recording recording)
        {
            return new LieResult(recording, -1, Frame.Empty);
        }

        public void AddDiff(int next, List<float> diff)
        {
            if (diff == null)            
                framestart = framend = next;
            else
            {
                framend = next;
                frameDifferences.Add(diff);
                float n = frameDifferences.Count;
                meandiff = (n - 1) / n * meandiff + diff[diff.Count -1] / n;
            }
        }
        //TODO: check setting of star and and, I believe i is done twice now
        public void AddFrame(Frame frame)
        {
            filteredFrames.Add(frame);
            framend++;
        }

        public int NextFrameId
        {
            get { return (framend + 1 < recording.FrameCount) ? framend + 1 : -1; }
        }

        public Frame LastFrame
        {
            get { return (filteredFrames.Count > 0) ? filteredFrames[filteredFrames.Count -1] : Frame.Empty; }
        }
    }
}
