using System;
using UIKit;
using Piller.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Foundation;
namespace Piller.iOS.Views
{
    [Register]
    [MvxModalPresentation]
    public class SettingsView:MvxTableViewController<SettingsViewModel>
    {
        public SettingsView()
        {
        }
    }
}
