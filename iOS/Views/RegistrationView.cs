using System;
using MvvmCross.iOS.Views;
using Piller.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using MvvmCross.iOS.Views.Presenters.Attributes;
using UIKit;

namespace Piller.iOS.Views
{
    [MvxTabPresentation]
    [Register("RegistrationView")]
    public class RegistrationView : MvxTableViewController<RegistrationViewModel>
    {
		UIBarButtonItem settingsButton;
        public RegistrationView ()
        {
			this.TableView.Source = new MedicationSummaryListViewSource(this.TableView);

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.settingsButton = new UIBarButtonItem() { Title = "Settings" };
            this.NavigationItem.LeftBarButtonItem = settingsButton;
        }
       
    }
}
