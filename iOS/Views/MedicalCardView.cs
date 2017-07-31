using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Piller.ViewModels;
using UIKit;

namespace Piller.iOS.Views
{
    [MvxTabPresentation]
    [Register("MedicalCardView")]

    public class MedicalCardView : MvxTableViewController<MedicalCardViewModel>
    {
        UIBarButtonItem addButton;
        UIBarButtonItem settingsButton;
        public MedicalCardView()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.addButton = new UIBarButtonItem(UIKit.UIBarButtonSystemItem.Add);
            this.settingsButton = new UIBarButtonItem() { Title = "Settings" };
            this.NavigationItem.RightBarButtonItem = addButton;
            this.NavigationItem.LeftBarButtonItem = settingsButton;

            var bindingSet = this.CreateBindingSet<MedicalCardView, MedicalCardViewModel>();
            bindingSet.Bind(addButton).To(vm => vm.AddNew);
            bindingSet.Apply();

        }
    }
}