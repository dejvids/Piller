using System;
using UIKit;
using MvvmCross.iOS.Views;
using Piller.ViewModels;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Piller.iOS.Views
{
    [MvxRootPresentationAttribute]
    public class RootView:MvxTabBarViewController<RootViewModel> 
    {
       
        public new RootViewModel ViewModel
        {
            get { return (RootViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
        public RootView(IntPtr handle) :base(handle)
        {
          
        }
        public RootView()
        {
            _constructed = true;
            ViewDidLoad();
        }

        private int _createdSoFarCount = 0;
        private bool _constructed;

        private UIViewController CreateTabFor(string title, string imageName, IMvxViewModel viewModel)
        {
            var controller = new UINavigationController();
            controller.NavigationBar.TintColor = UIColor.Black;
            var screen = this.CreateViewControllerFor(viewModel) as UIViewController;
            SetTitleAndTabBarItem(screen, title, imageName);
            controller.PushViewController(screen, false);
            return controller;
        }

        private  void SetTitleAndTabBarItem(UIViewController screen, string title, string imageName)
        {
            screen.Title = title;
            screen.TabBarItem = new UITabBarItem(title, UIImage.FromBundle(imageName), _createdSoFarCount);
            _createdSoFarCount++;
        }
        public override void ViewDidLoad()
        {
            if (!_constructed)
                return;
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;
			
            /*
            */
            RegistrationViewModel registrationViewModel = (RegistrationViewModel)Mvx.IocConstruct(typeof(RegistrationViewModel));
            MedicalCardViewModel medicalCardViewModel = (MedicalCardViewModel)Mvx.IocConstruct(typeof(MedicalCardViewModel));
            HolidayViewModel holidayViewModel = (HolidayViewModel)Mvx.IocConstruct(typeof(HolidayViewModel));

       /*
            UINavigationController medicalCardView = new UINavigationController(new MedicalCardView(){ Title = "Karta medyczna" });
            UINavigationController holidayView = new UINavigationController(new HolidayView(){Title="Wakacje"});
            UINavigationController upcomingMedicationsview = new UINavigationController(new MedicationSummaryListView(){Title="Najbliższe"});

            upcomingMedicationsview.TabBarItem = new UITabBarItem("Najbliższe",UIImage.FromBundle("recent"), 0);
            medicalCardView.TabBarItem = new UITabBarItem("Karta medyczna",UIImage.FromBundle("hospital"),1);
            holidayView.TabBarItem = new UITabBarItem("Wakacje",UIImage.FromBundle("holiday"),2);
            */
            var viewControllerList = new UIViewController[]
            {
                CreateTabFor("Najbliższe","recent",registrationViewModel),
                CreateTabFor("Karta medyczna","hospital",medicalCardViewModel),
                CreateTabFor("Wakacje","holiday",holidayViewModel)
            
            };
            /*
            var viewControllerList = new UIViewController[]
            {
                
                upcomingMedicationsview,
                medicalCardView,
                holidayView

            };
            */
            ViewControllers = viewControllerList;
           // this.ViewModel.ShowMedicalCardView.Execute().Subscribe();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
           // 
            //if (ViewModel != null && _isPresentedFirstTime)
            //{
                //_isPresentedFirstTime = false;
               // ViewModel.ShowMedicalCardView.Execute();
            //}
        }

        public bool ShowView(IMvxIosView view)
        {
            if (TryShowViewInCurrentTab(view))
                return true;

            return false;
        }

        private bool TryShowViewInCurrentTab(IMvxIosView view)
        {
            var navigationController = (UINavigationController)this.SelectedViewController;
            navigationController.PushViewController((UIViewController)view, true);
            return true;
        }
    }      

}
