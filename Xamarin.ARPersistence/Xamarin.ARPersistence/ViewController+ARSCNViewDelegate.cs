using System;
using ARKit;
using CoreFoundation;
using Foundation;
using SceneKit;
using UIKit;

namespace ARPersistence
{
    public partial class ViewController : IARSCNViewDelegate
    {
        [Export("renderer:didAddNode:forAnchor:")]
        public void DidAddNode(ISCNSceneRenderer renderer, SCNNode node, ARAnchor anchor)
        {
            if (anchor.Name != virtualObjectAnchorName)
                return;

            // save the reference to the virtual object anchor when the anchor is added from relocalizing
            if(objAnchor == null)
            {
                objAnchor = anchor;
            }

            node.AddChildNode(VirtualObject);
        }
    }
}
