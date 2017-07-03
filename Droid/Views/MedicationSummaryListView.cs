using Android.App;
using Android.OS;
using Piller.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Piller.Resources;
using MvvmCross.Binding.BindingContext;
using Android.Support.Design.Widget;
using Android.Views;
using System;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Binding.Droid.BindingContext;
using Android.Widget;
using MvvmCross.Droid.Support.V4;
using Android.Runtime;
using MvvmCross.Droid.Shared.Attributes;
using System.Collections;
using Piller.Data;
using System.Collections.Generic;

namespace Piller.Droid.Views
{
	[MvxFragment(typeof(RootViewModel), Resource.Id.content_frame, true)]
	[Register("piller.droid.views.MedicationSummaryListView")]
	public class MedicationSummaryListView : MvxFragment<MedicationSummaryListViewModel>
	{
        UpcomingListLayout upComingMedicines;
        UpcomingListLayout LaterMedicines;

        TextView upcomingPlaceholder;
        TextView laterPlaceholder;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
		{
			var ignore = base.OnCreateView(inflater, container, bundle);

            var view = this.BindingInflate(Resource.Layout.MedicationSummaryListView, null);
			((MvxCachingFragmentCompatActivity)Activity).SupportActionBar.Title = AppResources.MedicationSummaryListViewModel_Title;

            upComingMedicines = view.FindViewById<UpcomingListLayout>(Resource.Id.upcomingMedicines);
            LaterMedicines = view.FindViewById<UpcomingListLayout>(Resource.Id.latterMedicines);
            upcomingPlaceholder = view.FindViewById<TextView>(Resource.Id.empty2);

            laterPlaceholder = view.FindViewById<TextView>(Resource.Id.empty3);
            upComingMedicines.ItemTemplateId = Resource.Layout.medication_summary_item;
            LaterMedicines.ItemTemplateId = Resource.Layout.medication_summary_item;
            SetBinding();
            return view;
		}

        private void SetBinding()
		{
            var bindingSet = this.CreateBindingSet<MedicationSummaryListView, MedicationSummaryListViewModel>();
            bindingSet.Bind(upComingMedicines)
                .For(v => v.ItemsSource)
                .To(vm => vm.UpComingMedicines);
            bindingSet.Bind(upcomingPlaceholder)
                .For(v => v.Visibility)
                .To(vm => vm.UpComingMedicines)
                .WithConversion(new InlineValueConverter<List<MedicationDosage>, ViewStates>(m => m.Count > 0 ? ViewStates.Gone : ViewStates.Visible));
            bindingSet.Bind(LaterMedicines)
               .For(v => v.ItemsSource)
               .To(vm => vm.LaterMedicines);
            bindingSet.Bind(laterPlaceholder)
                .For(v => v.Visibility)
                .To(vm => vm.LaterMedicines)
                .WithConversion(new InlineValueConverter<List<MedicationDosage>, ViewStates>(m => m.Count > 0 ? ViewStates.Gone : ViewStates.Visible));
            bindingSet.Apply();
        }
	}
}