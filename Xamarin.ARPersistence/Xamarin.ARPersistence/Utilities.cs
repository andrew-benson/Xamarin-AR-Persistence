using System;
using System.Linq;
using ARKit;
using ImageIO;
using UIKit;

namespace ARPersistence.Utilities
{
    public static class ViewControllerUtilitiy
    {
        public static void ShowAlert(this UIViewController viewController)
        {

        }
    }

    public static class ARWorldMapUtility
    {
        public static SnapshotAnchor GetSnapshotAnchor(this ARWorldMap worldMap)
        {
            var snapshot = worldMap.Anchors.FirstOrDefault(anchor => anchor is SnapshotAnchor) as SnapshotAnchor;
            return snapshot;
        }
    }

    public static class OrientationUtility{

        public static CGImagePropertyOrientation CreateFromDeviceOrientation(UIDeviceOrientation cameraOrientation)
        {
            switch (cameraOrientation)
            {
                case UIDeviceOrientation.Portrait:
                    return CGImagePropertyOrientation.Right;
                case UIDeviceOrientation.PortraitUpsideDown:
                    return CGImagePropertyOrientation.Left;
                case UIDeviceOrientation.LandscapeLeft:
                    return CGImagePropertyOrientation.Up;
                case UIDeviceOrientation.LandscapeRight:
                    return CGImagePropertyOrientation.Down;
                default:
                    return CGImagePropertyOrientation.Right;
            }
        }
    }

}
