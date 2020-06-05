// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using Foundation;
using UIKit;

namespace ARPersistence
{
	public partial class RoundedButton : UIButton
	{
		public RoundedButton (IntPtr handle) : base (handle)
		{
            SetUp();
        }

        void SetUp()
        {
            BackgroundColor = TintColor;
            Layer.CornerRadius = 8f;
            ClipsToBounds = true;
            SetTitleColor(UIColor.White, UIControlState.Normal);
            TitleLabel.Font = UIFont.BoldSystemFontOfSize(17f);
        }

        public override bool Enabled
        {
            get => base.Enabled;
            set {
                base.Enabled = value;
                BackgroundColor = Enabled ? TintColor : UIColor.Gray;
            }
        }
    }
}
