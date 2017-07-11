﻿using System;
using Android.Runtime;
using Android.Widget;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.Views;
using Piller.Data;
using MvvmCross.Binding.Droid.BindingContext;
using Android.Content;
using Piller.Droid.BindingConverters;
using Android.Views;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using MvvmCross.Plugins.File;
using Services;
using MvvmCross.Platform;
using ReactiveUI;
using AndroidSwipeLayout;

namespace Piller.Droid.Views
{
    public class NearestListLayout : MvxLinearLayout
    {
        public NearestListLayout(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs, new NearestListAdapter(context))
        {
        }
    }
    public class NearestListAdapter : MvxAdapterWithChangedEvent
    {
        public ReactiveCommand<NotificationOccurrence, NotificationOccurrence> DeleteRequested { get; }
        private readonly ImageLoaderService imageLoader = Mvx.Resolve<ImageLoaderService>();

        public NearestListAdapter(Context context) : base(context)
        {
            this.DeleteRequested = ReactiveCommand.Create<NotificationOccurrence, NotificationOccurrence>(input => input);
        }



        protected override IMvxListItemView CreateBindableView(object dataContext, int templateId)
        {
            var view = base.CreateBindableView(dataContext, templateId) as MvxListItemView;

            var name = view.FindViewById<TextView>(Resource.Id.label_nearest_name);
            var dosage = view.FindViewById<TextView>(Resource.Id.label_nearest_dosage);
            var time = view.FindViewById<TextView>(Resource.Id.label_nearest_time);
            var bset = view.CreateBindingSet<MvxListItemView, NotificationOccurrence>();
            var thumbnail = view.FindViewById<ImageView>(Resource.Id.list_thumbnail);

            var del_nearest_not_button = view.FindViewById<ImageView>(Resource.Id.del_nearest_not_button);


            del_nearest_not_button.Click += (sender, e) => DeleteRequested.Execute((NotificationOccurrence)dataContext).Subscribe();

            bset.Bind(name)
                .To(x => x.Name);

            bset.Bind(dosage)
                .To(x => x.Dosage);

            bset.Bind(time)
                .To(x => x.OccurrenceDateTime)
                .WithConversion(new InlineValueConverter<DateTime, string>(dt => dt.ToShortTimeString()));


            bset.Bind(thumbnail)
                .To(x => x.ThumbnailImage)
                .For("Bitmap")
                .WithConversion(new InlineValueConverter<string, Bitmap>(file =>
             {
                 if (file != null)
                 {
                     byte[] array = imageLoader.LoadImage(file);
                     return BitmapFactory.DecodeByteArray(array, 0, array.Length);
                 }
                 else
                 {

                     return BitmapFactory.DecodeResource(this.Context.Resources, Resource.Drawable.pillThumb);
                 }
             }));



            bset.Apply();
          
            return view;
        }
    }
}
