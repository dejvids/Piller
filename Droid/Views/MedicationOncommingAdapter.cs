using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.Views;
using Piller.Data;
using Services;
using MvvmCross.Platform;
using MvvmCross.Plugins.File;
using Android.Graphics;
using Piller.Droid.BindingConverters;

namespace Piller.Droid.Views
{
    public class MedicationOncommingAdapter:MvxAdapterWithChangedEvent
    {
        private readonly IMvxFileStore fileStore = Mvx.Resolve<IMvxFileStore>();
        private readonly ImageLoaderService imageLoader = Mvx.Resolve<ImageLoaderService>();
        public MedicationOncommingAdapter(Context context):base(context)
        {

        }
        protected override IMvxListItemView CreateBindableView(object dataContext, int templateId)
        {
            var view = base.CreateBindableView(dataContext, templateId) as MvxListItemView;
            var name = view.FindViewById<TextView>(Resource.Id.label_medication_name);
            var time = view.FindViewById<TextView>(Resource.Id.label_medication_time);
            var medication = dataContext as MedicationDosage;

            name.Text = $"{medication.Name} ({medication.Dosage})";
            time.Text = new DosageHoursConverter().Convert(medication.DosageHours, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture).ToString();
            if (medication?.ThumbnailName != null)
            {
                var thumbnail = view.FindViewById<ImageView>(Resource.Id.list_thumbnail);
                byte[] array = imageLoader.LoadImage(medication.ThumbnailName);
                thumbnail.SetImageBitmap(BitmapFactory.DecodeByteArray(array, 0, array.Length));
            }
            else
            {
                var thumbnail = view.FindViewById<ImageView>(Resource.Id.list_thumbnail);
                thumbnail.SetImageBitmap(BitmapFactory.DecodeResource(this.Context.Resources, Resource.Drawable.pill64x64));
            }
            return view;
        }
    }
}