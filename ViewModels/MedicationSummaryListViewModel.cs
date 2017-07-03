using System.Reactive;
using MvvmCross.Core.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using Piller.Data;
using Piller.Services;
using MvvmCross.Platform;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;
using System.Linq;
using System;
using Cheesebaron.MvxPlugins.Settings.Interfaces;
using Newtonsoft.Json;

namespace Piller.ViewModels
{
    public class MedicationSummaryListViewModel : MvxViewModel
	{
		private IPermanentStorageService storage = Mvx.Resolve<IPermanentStorageService>();
        private ISettings settings = Mvx.Resolve<ISettings>();
        private SettingsData settingsData;
        MvxSubscriptionToken dataChangedSubscriptionToken;
        MvxSubscriptionToken settingsChangedSubscriptionToken;

        private List<MedicationDosage> medicationList;

        private double upComingHour;
        public double Upcoming
        {
            get { return upComingHour; }
            set { SetProperty(ref upComingHour, value);RaisePropertyChanged(nameof(UpComingMedicines));RaisePropertyChanged(nameof(LaterMedicines)); }
        }

        public List<MedicationDosage> MedicationList
        {
            get { return medicationList; }
            set { SetProperty(ref medicationList, value);  RaisePropertyChanged(nameof(UpComingMedicines)); RaisePropertyChanged(nameof(LaterMedicines)); }
        }

        public List<MedicationDosage> UpComingMedicines
        {
            get
            {
                return MedicationList
                    .Where(m => m.DosageHours.Any(h=>(h.TotalMinutes - (DateTime.Now.TimeOfDay).TotalMinutes)>0 
                    && (h.TotalMinutes - (DateTime.Now.TimeOfDay).TotalMinutes) <Upcoming)
                   // &&m.Days.HasFlag((DaysOfWeek)DateTime.Now.DayOfWeek)
                    )
                    .ToList();
            }
        }

        public List<MedicationDosage> LaterMedicines
        {
            get
            {
                return MedicationList
                    .Where(m => m.DosageHours.Any(h =>(h.TotalMinutes - (DateTime.Now.TimeOfDay).TotalMinutes) > Upcoming)
                  //  && m.Days.HasFlag((DaysOfWeek)DateTime.Now.DayOfWeek)
                    )
                    .ToList();
            }
        }

        public ReactiveCommand<Unit, bool> AddNew { get; }
		public ReactiveCommand<Data.MedicationDosage, Unit> Edit { get; }


        public MedicationSummaryListViewModel()
        {
           
            dataChangedSubscriptionToken = Mvx.Resolve<IMvxMessenger>().Subscribe<DataChangedMessage>(async mesg => await Init());
            settingsChangedSubscriptionToken = Mvx.Resolve<IMvxMessenger>().Subscribe<SettingsChangeMessage>(async mesg => await Init());
        }
        public async Task Init()
        {

            var items = await storage.List<MedicationDosage>();
            if (items != null)
                MedicationList = new List<MedicationDosage>(items);
            else
                MedicationList = new List<MedicationDosage>();
            readSettings();
            Upcoming = TimeSpan.FromHours(settingsData.Interval).TotalMinutes;
        }
        private void readSettings()
        {
            var data = settings.GetValue<string>(SettingsData.Key);
            if (String.IsNullOrEmpty(data))
            {
                settingsData = new SettingsData();
                return;
            }
            try
            {
                settingsData = JsonConvert.DeserializeObject<SettingsData>(data);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                settingsData = new SettingsData();
            }

        }


    }
}