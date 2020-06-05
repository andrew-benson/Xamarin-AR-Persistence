using System;
using System.Diagnostics;
using ARKit;
using CoreGraphics;
using CoreImage;
using Foundation;
using OpenTK;
using UIKit;
using ARPersistence.Utilities;

namespace ARPersistence
{
    [Register("SnapshotAnchor")]
    public class SnapshotAnchor: ARAnchor
    {
        public NSData ImageData;

        protected SnapshotAnchor(IntPtr handle) : base(handle) {}

        public static SnapshotAnchor Create(ARSCNView capturingView)
        {
            try
            {
                var frame = capturingView.Session?.CurrentFrame;
                var camera = frame?.Camera;
                var capture = frame?.CapturedImage;

                if (frame == null || camera == null || capture == null)
                    return null;
                
                var image = CIImage.FromImageBuffer(capture);

                var orientation = OrientationUtility.CreateFromDeviceOrientation(UIDevice.CurrentDevice.Orientation);

                var context = new CIContext(new CIContextOptions { UseSoftwareRenderer = false });

                var data = context.GetJpegRepresentation(image.CreateByApplyingOrientation(orientation)
                    , CGColorSpace.CreateDeviceRGB()
                    , new CIImageRepresentationOptions { LossyCompressionQuality = 0.7f });

                return new SnapshotAnchor(data, camera.Transform);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to create image: " + ex.Message);
                return null;
            }
        }

        public SnapshotAnchor(NSData imageData, NMatrix4 transform) : base("snapshot", transform)
        {
            this.ImageData = imageData;            
        }

        [Export("initWithCoder:")]
        public SnapshotAnchor(NSCoder coder) : base(coder)
        {
            var imageBytes = coder.DecodeBytes("snap");
            ImageData = NSData.FromArray(imageBytes);
        }

        [Export("supportsSecureCoding")]
        public static bool SupportsSecureCoding => true;

        public override void EncodeTo(NSCoder encoder)
        {
            base.EncodeTo(encoder);
            encoder.Encode(ImageData.ToArray(), "snap");
        }
    }
}
