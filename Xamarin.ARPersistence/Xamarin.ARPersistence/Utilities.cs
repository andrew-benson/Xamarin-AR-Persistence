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

    public static class ARCameraUtility
    {
        public static string GetLocalisedFeedback(this ARCamera camera)
        {
            if(camera == null)
                return "Initializing AR session.";

            switch (camera.TrackingState)
            {
                case ARTrackingState.Normal:
                    return "Move around to map the environment.";
                case ARTrackingState.NotAvailable:
                    return "Tracking unavailable.";
                case ARTrackingState.Limited:
                    switch (camera.TrackingStateReason)
                    {
                        case ARTrackingStateReason.ExcessiveMotion:
                            return "Move the device more slowly.";
                        case ARTrackingStateReason.InsufficientFeatures:
                            return "Point the device at an area with visible surface detail, or improve lighting conditions.";
                        case ARTrackingStateReason.Relocalizing:
                            return "Resuming session — move to where you were when the session was interrupted.";
                        case ARTrackingStateReason.Initializing:
                            return "Initializing AR session.";
                        default:
                            return string.Empty;
                    }
                default:
                    return string.Empty;
            }
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
