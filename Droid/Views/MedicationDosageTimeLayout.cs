using System;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Binding.Droid.BindingContext;

namespace Piller.Droid.Views
{
        
    public class MedicationDosageTimeLayout : MvxLinearLayout
    {
        public MedicationDosageTimeLayout(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs, new MedicationDosageTimeListAdapter(context))
        {
        }
    }

    class UpcomingListLayout:MvxLinearLayout
    {
        public UpcomingListLayout(Android.Content.Context context, Android.Util.IAttributeSet attrs):base(context,attrs,new MedicationOncommingAdapter(context))
        {

        }
    }
  

}
