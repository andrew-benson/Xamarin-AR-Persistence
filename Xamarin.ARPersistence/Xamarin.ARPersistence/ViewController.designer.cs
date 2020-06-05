// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ARPersistence
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIButton _loadButton { get; set; }

		[Outlet]
		UIKit.UIButton _saveButton { get; set; }

		[Outlet]
		ARKit.ARSCNView _sceneView { get; set; }

		[Outlet]
		UIKit.UILabel _sessionInfoLabel { get; set; }

		[Outlet]
		UIKit.UIVisualEffectView _sessionInfoView { get; set; }

		[Outlet]
		UIKit.UIImageView _snapShotThumbnail { get; set; }

		[Outlet]
		UIKit.UILabel _statusLabel { get; set; }

		[Action ("HandleSceneTap:")]
		partial void HandleSceneTap (UIKit.UITapGestureRecognizer sender);

		[Action ("LoadExperience")]
		partial void LoadExperience ();

		[Action ("ResetTracking")]
		partial void ResetTracking ();

		[Action ("SaveExperience")]
		partial void SaveExperience ();
		
		void ReleaseDesignerOutlets ()
		{
			if (_loadButton != null) {
				_loadButton.Dispose ();
				_loadButton = null;
			}

			if (_saveButton != null) {
				_saveButton.Dispose ();
				_saveButton = null;
			}

			if (_sceneView != null) {
				_sceneView.Dispose ();
				_sceneView = null;
			}

			if (_sessionInfoLabel != null) {
				_sessionInfoLabel.Dispose ();
				_sessionInfoLabel = null;
			}

			if (_sessionInfoView != null) {
				_sessionInfoView.Dispose ();
				_sessionInfoView = null;
			}

			if (_snapShotThumbnail != null) {
				_snapShotThumbnail.Dispose ();
				_snapShotThumbnail = null;
			}

			if (_statusLabel != null) {
				_statusLabel.Dispose ();
				_statusLabel = null;
			}
		}
	}
}
