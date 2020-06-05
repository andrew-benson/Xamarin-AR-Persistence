using ARKit;
using CoreFoundation;
using Foundation;
using SceneKit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UIKit;
using ARPersistence.Utilities;

namespace ARPersistence
{
    public partial class ViewController : UIViewController, IARSCNViewDelegate, IARSessionDelegate
    {
        public ViewController(IntPtr handle) : base(handle) {}
        string MapSaveURL => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/arworldmap";
        private ARAnchor objAnchor;
        private string virtualObjectAnchorName = "virtualObject";
        private bool isRelocalizingMap = false;

        private SCNNode VirtualObject
        {
            get
            {
                var sceneUrl = NSBundle.MainBundle.GetUrlForResource("cup", "scn", "cup");
                var referenceNode = SCNReferenceNode.CreateFromUrl(sceneUrl);
                referenceNode.Load();
                return referenceNode;
            }
        }

        private NSData DataFromFile
        {
            get
            {
                try
                {
                    return NSData.FromArray(File.ReadAllBytes(MapSaveURL));                        
                }
                catch
                {
                    return null;
                }
            }
        }

        private ARWorldTrackingConfiguration DefaultConfiguration => new ARWorldTrackingConfiguration
        {
            PlaneDetection = ARPlaneDetection.Horizontal,
            EnvironmentTexturing = AREnvironmentTexturing.Automatic,
        };

        // Lock the orientation of the app to the orientation in which it is launched
        public override bool ShouldAutorotate()
        {
            return false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Read in any already saved map to see if we can load one.
            if (DataFromFile != null)
            {
                _loadButton.Hidden = false;
            }           
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (ARWorldTrackingConfiguration.IsSupported == false)
            {               
                Debug.WriteLine("ARKit is not available on this device. For apps that require ARKit " +
                "for core functionality, use the `arkit` key in the key in the" +
                "`UIRequiredDeviceCapabilities` section of the Info.plist to prevent" +
                "the app from installing. (If the app can't be installed, this error" +
                "can't be triggered in a production scenario.)" +
                "In apps where AR is an additive feature, use `isSupported` to" + 
                " determine whether to show UI for launching AR experiences.");

                return;
            }

            StartAR();

            // Prevent the screen from being dimmed after a while as users will likely
            // have long periods of interaction without touching the screen or buttons.
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            // Pause the view's AR session.
            _sceneView.Session.Pause();
        }

        private void StartAR()
        {
            _sceneView.AutoenablesDefaultLighting = true;

            _sceneView.Delegate = this;

            _sceneView.Session = new ARSession();

            _sceneView.Session.Delegate = this;
                                
            _sceneView.Scene = SCNScene.Create();

            _sceneView.DebugOptions = ARSCNDebugOptions.ShowFeaturePoints;

            _sceneView.Session.Run(DefaultConfiguration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);    // Run the view's session
        }

        #region IBActions

        partial void ResetTracking()
        {
            _sceneView.Session.Run(DefaultConfiguration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);    // Run the view's session
            isRelocalizingMap = false;
            objAnchor = null;
        }

        partial void LoadExperience()
        {
            if (DataFromFile == null)
            {
                Debug.WriteLine("Map data should already be verified to exist before Load button is enabled.");
                return;
            }

            try
            {
                var data = DataFromFile;

                ARWorldMap worldMap = (ARWorldMap)NSKeyedUnarchiver.GetUnarchivedObject(typeof(ARWorldMap), data, out var err);

                if (err != null)
                {
                    Debug.WriteLine("No ARWorldMap in archive.");
                    return;
                }

                // Display the snapshot image stored in the world map to aid user in relocalizing.
                var snapshotData = worldMap.GetSnapshotAnchor()?.ImageData;

                using (var snapshot = UIImage.LoadFromData(snapshotData))
                {
                    if (snapshot != null)
                    {
                        _snapShotThumbnail.Image = snapshot;
                    }
                    else
                    {
                        Debug.WriteLine("No snapshot image in world map");
                    }
                }

                // Remove the snapshot anchor from the world map since we do not need it in the scene.
                worldMap.Anchors.ToList().RemoveAll(anchor => anchor is SnapshotAnchor);

                var configuration = new ARWorldTrackingConfiguration
                {
                    PlaneDetection = ARPlaneDetection.Horizontal,

                    //LightEstimationEnabled = true,
                    EnvironmentTexturing = AREnvironmentTexturing.Automatic,
                    InitialWorldMap = worldMap,
                };

                _sceneView.Session.Run(configuration, ARSessionRunOptions.ResetTracking | ARSessionRunOptions.RemoveExistingAnchors);    // Run the view's session
                isRelocalizingMap = true;
                objAnchor = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Can't unarchive ARWorldMap from file data: {ex.Message}");
                return;
            }

            Debug.WriteLine($"Success: Loaded world scene from {MapSaveURL}");
        }

        async partial void SaveExperience()
        {
            var worldMap = await _sceneView.Session.GetCurrentWorldMapAsync();

            if (worldMap != null)
            {
                try
                {
                    // Add a snapshot image indicating where the map was captured.
                    var snapshotAnchor = SnapshotAnchor.Create(_sceneView);

                    if(snapshotAnchor != null)
                    {
                        var imageView = UIImage.LoadFromData(snapshotAnchor.ImageData);
                        _snapShotThumbnail.Hidden = false;
                        _snapShotThumbnail.Image = imageView;
                        var anchList = worldMap.Anchors.ToList();
                        anchList.Add(snapshotAnchor);
                        worldMap.Anchors = anchList.ToArray();
                        Debug.WriteLine("Screenshot saved");
                    }
                    else
                    {
                        Debug.WriteLine("Snapshot anchor is null");
                    }
                }
                catch
                {
                    Debug.WriteLine("Can't take screenshot");
                    throw;
                }

                var data = NSKeyedArchiver.ArchivedDataWithRootObject(worldMap, true, out var err);

                if (err != null)
                {
                    Debug.WriteLine(err);
                    return;
                }

                try
                {
                    File.WriteAllBytes(MapSaveURL, data.ToArray());
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        _loadButton.Hidden = false;
                        _loadButton.Enabled = true;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Can't save map: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("Can't get current world map");
            }
        }

        partial void HandleSceneTap(UITapGestureRecognizer sender)
        {
            Debug.WriteLine("tapped");

            // Disable placing objects when the session is still relocalizing
            if (isRelocalizingMap && objAnchor == null)
                return;

            // Hit test to find a place for a virtual object.
            var hitTestResult = _sceneView.HitTest(sender.LocationInView(View)
                , ARHitTestResultType.ExistingPlaneUsingGeometry | ARHitTestResultType.EstimatedHorizontalPlane)?
                .FirstOrDefault();

            if(hitTestResult != null)
            {
                if (objAnchor != null)
                {
                    var existingAnchor = objAnchor;
                    _sceneView.Session.RemoveAnchor(existingAnchor);
                }

                objAnchor = new ARAnchor(virtualObjectAnchorName, hitTestResult.WorldTransform);
                _sceneView.Session.AddAnchor(objAnchor);
            }
        }
        #endregion
    }
}