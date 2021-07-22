using System;
using OpenCvSharp;
using YoonFactory.Image;

namespace YoonFactory.CV
{
    public class FrameArgs : EventArgs
    {
        public CVImage Frame { get; set; }
        public FrameArgs(Mat pMat)
        {
            this.Frame = new CVImage(pMat);
        }

        public FrameArgs(YoonImage pImage)
        {
            this.Frame = new CVImage(pImage);
        }
    }

    public delegate void RecieveFrameCallback(object sender, FrameArgs e);
}
