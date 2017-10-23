using System;
using Cirrious.FluentLayouts.Touch;
using UIKit;

namespace Piller.iOS.Common.Dialog
{
    public class PhotoCell : UITableViewCell
    {
		public UILabel TitleLabel { get; private set; }
        public UIImageView PhotoView { get; private set; }

        public PhotoCell(string cellKey) : base(UITableViewCellStyle.Default, cellKey)
        {
			this.TitleLabel = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false, Lines = 0 };
            this.PhotoView = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = false };

            this.ContentView.AddSubviews(this.TitleLabel, this.PhotoView);

            this.ContentView.AddConstraints(
                this.TitleLabel.AtTopOf(this.ContentView).Plus(10),
                this.TitleLabel.AtLeftOf(this.ContentView).Plus(15),
                this.TitleLabel.WithSameRight(this.ContentView).Minus(15),
                this.PhotoView.Below(this.TitleLabel),
                this.PhotoView.WithSameCenterX(this.ContentView),
                this.PhotoView.AtBottomOf(this.ContentView).Minus(10),
                this.PhotoView.Width().EqualTo(192),
                this.PhotoView.Height().EqualTo(192));
		}
    }
}
