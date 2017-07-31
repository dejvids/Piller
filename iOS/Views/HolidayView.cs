using System;
using UIKit;
using Piller.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Foundation;

namespace Piller.iOS.Views
{
    [MvxTabPresentation]
    [Register("HolidayView")]
    public class HolidayView:MvxViewController<HolidayViewModel>
    {
		UIBarButtonItem settingsButton;
        public HolidayView()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			this.settingsButton = new UIBarButtonItem() { Title = "Settings" };
            this.NavigationItem.LeftBarButtonItem = settingsButton;
        }
    }
}
