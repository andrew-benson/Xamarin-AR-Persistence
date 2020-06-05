using System;
using System.Linq;
using ARKit;
using CoreFoundation;
using Foundation;
using SceneKit;
using UIKit;

namespace ARPersistence
{
    public partial class ViewController
    {
        [Export("renderer:updateAtTime:")]
        public void Update(ISCNSceneRenderer renderer, double timeInSeconds)
        {
            using (var currentFrame = _sceneView?.Session?.CurrentFrame)
            {
                if (currentFrame == null || currentFrame?.Camera == null)
                {
                    return;
                }
            }
        }

        [Export("session:didUpdateFrame:")]
        public void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            // I've added this as frame.Camera get accessor returns null at the time of execution on line 51.
            // Storing it will prevent the value from changing mid-method
            var camera = frame.Camera;

            if (frame == null || camera == null)
            {
                return;
            }

            switch (frame.WorldMappingStatus)
            {
                case ARWorldMappingStatus.Extending:
                case ARWorldMappingStatus.Mapped:
                    _saveButton.Enabled = objAnchor != null && frame.Anchors.Contains(objAnchor);
                    break;
                default:
                    _saveButton.Enabled = false;
                    break;
            }

            _statusLabel.Text =
                $"Mapping: {frame.WorldMappingStatus}" +
                Environment.NewLine +
                $"Tracking: {camera.TrackingState}";

            UpdateSessionInfoLabel(frame, camera.TrackingState, camera.TrackingStateReason);
        }

        [Export("session:cameraDidChangeTrackingState:")]
        public void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            UpdateSessionInfoLabel(session.CurrentFrame, camera.TrackingState, camera.TrackingStateReason);
        }

        private void UpdateSessionInfoLabel(ARFrame currentFrame, ARTrackingState trackingState, ARTrackingStateReason trackingStateReason)
        {
            string message = "";

            _snapShotThumbnail.Hidden = true;

            switch (trackingState)
            {
                case ARTrackingState.Normal:
                    switch (currentFrame.WorldMappingStatus)
                    {
                        case ARWorldMappingStatus.Mapped:
                        case ARWorldMappingStatus.Extending:
                            if (currentFrame.Anchors.Any(anchor => anchor.Name == virtualObjectAnchorName))
                            {
                                message = "Tap 'Save Experience' to save the current map.";
                            }
                            else
                            {
                                message = "Tap on the screen to place an object.";
                            }
                            break;

                        default:
                            if (DataFromFile == null)
                            {
                                message = "Move around to map the environment.";
                            }
                            else if (isRelocalizingMap == false)
                            {
                                message = "Move around to map the environment or tap 'Load Experience' to load a saved experience.";
                            }
                            break;
                    }
                    break;
                case ARTrackingState.Limited:
                    if (trackingStateReason == ARTrackingStateReason.Relocalizing && isRelocalizingMap)
                    {
                        message = "Move your device to the location shown in the image.";
                        _snapShotThumbnail.Hidden = false;
                    }
                    break;
                default:
                    message = trackingState.ToString();
                    break;
            }

            _sessionInfoLabel.Text = message;
            _sessionInfoView.Hidden = message == string.Empty;
        }

        [Export("sessionWasInterrupted:")]
        public void WasInterrupted(ARSession session)
        {
            _sessionInfoLabel.Text = "Session was interrupted";
        }

        [Export("sessionInterruptionEnded:")]
        public void InterruptionEnded(ARSession session)
        {
            _sessionInfoLabel.Text = "Session interrupted ended";
        }

        [Export("session:didFailWithError:")]
        public void DidFail(ARSession session, NSError error)
        {
            _sessionInfoLabel.Text = $"Session failed: {error.LocalizedDescription}";

            var messages = new[]
            {
                error.LocalizedDescription,
                error.LocalizedFailureReason,
                error.LocalizedRecoverySuggestion
            };

            var errorMessage = string.Join(Environment.NewLine, messages);

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {

                var alertController = UIAlertController.Create("The AR session failed", errorMessage, UIAlertControllerStyle.Alert);

                var restartAction = UIAlertAction.Create("Restart Session"
                    , UIAlertActionStyle.Default
                    , (UIAlertAction) => { alertController.DismissViewController(true, null); });

                alertController.AddAction(restartAction);
            });
        }
    }
}
