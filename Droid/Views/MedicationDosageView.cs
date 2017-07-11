using Android.App;
using Android.OS;
using Piller.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Piller.Resources;
using MvvmCross.Binding.BindingContext;
using Android.Views;
using Android.Graphics;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Widget;
using System;
using System.Windows.Input;
using System.Reactive;
using MvvmCross.Binding.Droid.Views;
using Android.Opengl;
using Piller.Droid.BindingConverters;
using MvvmCross.Platform.Converters;
using System.Globalization;
using MvvmCross.Plugins.PictureChooser.Droid;
using System.Reactive.Linq;
using Android.Support.Design.Widget;
using System.Collections;
using System.Collections.Generic;
using ZXing.Mobile;

namespace Piller.Droid.Views
{
    [Activity]
    public class MedicationDosageView : MvxCachingFragmentCompatActivity<MedicationDosageViewModel>
    {
        EditText nameText;
        EditText dosageText;
        LinearLayout takePicutre;
        ImageView picture;
        TextView deleteBtn;
        TextView daysSelector;

        TextView timeSelector;
		TextView fromDate;
		TextView toDate;

        //TextView daysOfWeek;
		ImageButton clearFrom;
		ImageButton clearTo;

        //MedicationDosageTimeLayout hoursList;

        FloatingActionButton barScan;

        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MedicationDosageView);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = AppResources.MedicationDosageViewModel_Title;
			nameText = FindViewById<EditText>(Resource.Id.NameEditText);
			fromDate = FindViewById<TextView>(Resource.Id.odKiedy);
			toDate = FindViewById<TextView>(Resource.Id.doKiedy);
		    clearFrom = FindViewById<ImageButton>(Resource.Id.clearFrom);
			clearTo = FindViewById<ImageButton>(Resource.Id.clearTo);
            dosageText = FindViewById<EditText>(Resource.Id.DosageEditText);

            takePicutre = FindViewById<LinearLayout>(Resource.Id.take_photo);
            picture = FindViewById<ImageView>(Resource.Id.photo);

            deleteBtn = FindViewById<TextView>(Resource.Id.deleteBtn);

            timeSelector = FindViewById<TextView>(Resource.Id.timeSelector);
            daysSelector = FindViewById<TextView>(Resource.Id.daySelector);


            var timeSelectorDialog = new OrdinationTimesSheet(this, this.ViewModel);

			timeSelector.Click += (o, e) => timeSelectorDialog.Show();


			
            barScan = FindViewById<FloatingActionButton>(Resource.Id.barScan);

            MobileBarcodeScanner.Initialize(Application);


            barScan.Click += async (sender, e) =>
            {
                // Initialize the scanner first so it can track the current context
                var scanner = new MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
                {
                    ViewModel.SetMedicinesName(result.Text);
                }
                   
            };
            OrdiniationSchemeDialog ordinationSchemeDialog = new OrdiniationSchemeDialog(this, this.ViewModel);
			daysSelector.Click += (o, e) => ordinationSchemeDialog.Show();

            DeleteDialog deleteDialog = new DeleteDialog(this);
 

            View deleteView = LayoutInflater.Inflate(Resource.Layout.delete_dialog, null);


		  deleteDialog.SetContentView(deleteView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

          
          
			fromDate.Click += (o,e) =>{
					DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
					{
					fromDate.Text = time.ToShortDateString();
					});
					frag.minDate = DateTime.Now.Date;

					frag.Show(FragmentManager, DatePickerFragment.TAG);

			};

			toDate.Click += (o,e) => {
					DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
					{
					toDate.Text = time.ToShortDateString();
					});
				if (!string.IsNullOrEmpty(fromDate.Text))
				{
					frag.minDate = DateTime.Parse(fromDate.Text);
				}
				else
				{
					frag.minDate = DateTime.Now.Date;
				}
					frag.Show(FragmentManager, DatePickerFragment.TAG);

			};

			clearFrom.Click += (o,e) => {
				fromDate.Text = "";
			};

			clearTo.Click += (o,e) => {
				toDate.Text = "";
			};

			fromDate.TextChanged += (o,e) => {
				if (string.IsNullOrEmpty(fromDate.Text))
					clearFrom.Visibility=ViewStates.Invisible;
				else
					clearFrom.Visibility=ViewStates.Visible;
			};

			toDate.TextChanged += (o,e) => {
				if (string.IsNullOrEmpty(toDate.Text))
					clearTo.Visibility=ViewStates.Invisible;
				else
					clearTo.Visibility=ViewStates.Visible;
			};

            //deleteDialog.Create();
            deleteBtn.Click += (o, e) => deleteDialog.Show();
            deleteDialog.Accept.Subscribe(x =>
            {
                if(((ICommand)ViewModel.Delete).CanExecute(null))
                    ViewModel.Delete.Execute().Subscribe();
            });
            deleteDialog.Cancel.Subscribe(x => deleteDialog.Dismiss());
            
            SetBinding();
		}



		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			this.MenuInflater.Inflate(Resource.Menu.dosagemenu, menu);
			var saveItem = menu.FindItem(Resource.Id.action_save);

			this.ViewModel.Save.CanExecute.Subscribe(canExecute =>
			{
				saveItem.SetEnabled(canExecute);
			});

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
            //sprawdzamy, czy przycisk ma id zdefiniowane dla Save, i czy Save mozna wywolac (to na przyszlosc, gdy bedzie walidacja)
            // jak tak - odpalamy komendę. To dziwne Subscribe na końcu do wymóg ReactiveUI7
            if (item.ItemId == Resource.Id.action_save && ((ICommand)this.ViewModel.Save).CanExecute(null))
            {
                this.ViewModel.Save.Execute().Catch<bool, Exception>(ex =>
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    return Observable.Empty<bool>();
                }).Subscribe(_ => {
                    System.Diagnostics.Debug.WriteLine($"Save invoked {_}");
                });

                return true;
            }

            return base.OnOptionsItemSelected(item);
		}

        private void SetBinding()
        {
            var bindingSet = this.CreateBindingSet<MedicationDosageView, MedicationDosageViewModel>();

			//sposob na bezposrednie sluchanie observable. W momencie, gdy CanExecute sie zmieni wykona sie kod z Subscribe
			this.ViewModel.Delete.CanExecute.Subscribe(canExecute => deleteBtn.Visibility = canExecute ? ViewStates.Visible : ViewStates.Gone);

			bindingSet.Bind(this.SupportActionBar)
					  .To(x => x.MedicationName)
					  .For(v => v.Title)
					  .WithConversion(new InlineValueConverter<string, string>(medicationName =>
					  {
						  if (string.IsNullOrEmpty(medicationName))
							  return this.ViewModel.Id.HasValue ? "" : AppResources.MedicationDosageViewModel_Title;
						  return medicationName;
					  }));

			bindingSet.Bind(nameText)
					  .To(x => x.MedicationName);

			bindingSet.Bind(fromDate)
			          .To(x => x.StartDate)
			          .Mode(MvvmCross.Binding.MvxBindingMode.TwoWay);


			bindingSet.Bind(toDate)
			          .To(x => x.EndDate)
			          .Mode(MvvmCross.Binding.MvxBindingMode.TwoWay);

            bindingSet.Bind(picture)
                      .To(x => x.Bytes)
                      .For("Bitmap")
                      .WithConversion(new MvxInMemoryImageValueConverter());

			bindingSet.Bind(picture)
                      .To(x => x.Bytes)
                      .For(v => v.Visibility)
			          .WithConversion(new InlineValueConverter<byte[], ViewStates>((byte[] arg) => arg == null ? ViewStates.Gone : ViewStates.Visible)) ;

            bindingSet.Bind(takePicutre)
                .For(nameof(View.Click))
                .To(vm => vm.TakePhotoCommand);
            bindingSet.Bind(dosageText)
                .To(vm => vm.MedicationDosage);
            bindingSet.Bind(daysSelector)
                .To(vm => vm.DaysLabel);

            bindingSet.Bind(timeSelector)
                      .WithConversion(new InlineValueConverter<string, string>((arg) => Humanizer.CasingExtensions.ApplyCase(arg, Humanizer.LetterCasing.Sentence)))
                      .To(vm => vm.HoursLabel);

            bindingSet.Apply();

            //Bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
		}

	}
}
