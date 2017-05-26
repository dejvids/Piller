﻿﻿using System;
using MvvmCross.Core.ViewModels;
using RxUI = ReactiveUI;
using System.Reactive;
using Piller.Data;
using Piller.Services;
using MvvmCross.Platform;
using Acr.UserDialogs;
using Piller.Resources;
using ReactiveUI;
using MvvmCross.Plugins.Messenger;
using MvvmCross.Plugins.PictureChooser;
using System.IO;
using MvvmCross.Plugins.File;
using Services;

namespace Piller.ViewModels
{
    public class MedicationDosageViewModel : MvxViewModel
    {
		IMvxPictureChooserTask PictureChooser = Mvx.Resolve<IMvxPictureChooserTask>();
		private IPermanentStorageService storage = Mvx.Resolve<IPermanentStorageService>();
		private readonly ImageLoaderService imageLoader = Mvx.Resolve<ImageLoaderService>();
        private readonly INotificationService notifications = Mvx.Resolve<INotificationService>();

        public ReactiveCommand<Unit, Stream> TakePhotoCommand { get; set; }

		private byte[] _bytes;
		public byte[] Bytes
		{
			get { return _bytes; }
			set { _bytes = value; RaisePropertyChanged(() => Bytes); }
		}

		private void OnPicture(Stream pictureStream)
		{
			var memoryStream = new MemoryStream();
			pictureStream.CopyTo(memoryStream);
			Bytes = memoryStream.ToArray();
		}

        //identyfikator rekordu, uzywany w trybie edycji
        private int? id;
        public int? Id { 
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        string medicationName;
        public string MedicationName
        {
            get { return medicationName; }
            set { this.SetProperty(ref medicationName, value); }
        }


        string medicationDosage;
        public string MedicationDosage
        {
            get { return medicationDosage; }
            set { this.SetProperty(ref medicationDosage, value); }
        }


        private bool monday;
        public bool Monday
        {
            get { return monday; }
            set { this.SetProperty(ref monday, value); }
        }

        private bool tuesday;
        public bool Tuesday
        {
            get { return tuesday; }
            set { this.SetProperty(ref tuesday, value); }
        }

        private bool wednesday;
        public bool Wednesday
        {
            get { return wednesday; }
            set { this.SetProperty(ref wednesday, value); }
        }

        private bool thursday;
        public bool Thurdsday
        {
            get { return thursday; }
            set { this.SetProperty(ref thursday, value); }
        }

        private bool friday;
        public bool Friday
        {
            get { return friday; }
            set { this.SetProperty(ref friday, value); }
        }

        private bool saturday;
        public bool Saturday
        {
            get { return saturday; }
            set { this.SetProperty(ref saturday, value); }
        }

        private bool sunday;
        public bool Sunday
        {
            get { return sunday; }
            set { this.SetProperty(ref sunday, value); }
        }


        private RxUI.ReactiveList<TimeSpan> dosageHours;
        public RxUI.ReactiveList<TimeSpan> DosageHours
        {
            get { return this.dosageHours; }
            set { SetProperty(ref dosageHours, value); }
        }

        public RxUI.ReactiveCommand<Unit, bool> Save { get; private set; }
        public RxUI.ReactiveCommand<MedicationDosage, bool> Delete { get; set; }    
        public RxUI.ReactiveCommand SelectAllDays { get; set; }

        public MedicationDosageViewModel()
        {
            this.DosageHours = new RxUI.ReactiveList<TimeSpan>();

			var canSave = this.WhenAny(
				vm => vm.MedicationName,
				vm => vm.MedicationDosage,
				vm => vm.Monday,
				vm => vm.Tuesday,
				vm => vm.Wednesday,
				vm => vm.Thurdsday,
				vm => vm.Friday,
				vm => vm.Saturday,
				vm => vm.Sunday,
				vm => vm.DosageHours.Count,
				(n, d, m, t, w, th, f, sa, su, h) =>

				!String.IsNullOrWhiteSpace(n.Value) &&
				!String.IsNullOrWhiteSpace(d.Value) &&
				(m.Value | t.Value | w.Value | th.Value | f.Value | sa.Value | su.Value) &&
				h.Value > 0);
            
			this.TakePhotoCommand = ReactiveCommand.CreateFromTask(() => PictureChooser.TakePicture(100, 90));
			this.TakePhotoCommand.Subscribe(x =>
			{
				this.OnPicture(x);
			});

            this.Save = RxUI.ReactiveCommand.CreateFromTask<Unit, bool>(async _ =>
            {

				var dataRecord = new MedicationDosage
				{
					Id = this.Id,
					Name = this.MedicationName,
					Dosage = this.MedicationDosage,

                    Days =
                        (this.Monday ? DaysOfWeek.Monday : DaysOfWeek.None)
                        | (this.Tuesday ? DaysOfWeek.Tuesday : DaysOfWeek.None)
                        | (this.Wednesday ? DaysOfWeek.Wednesday : DaysOfWeek.None)
                        | (this.Thurdsday ? DaysOfWeek.Thursday : DaysOfWeek.None)
                        | (this.Friday ? DaysOfWeek.Friday : DaysOfWeek.None)
                        | (this.Saturday ? DaysOfWeek.Saturday : DaysOfWeek.None)
                        | (this.Sunday ? DaysOfWeek.Sunday : DaysOfWeek.None),
                    DosageHours = this.DosageHours
                };

                if (this.Bytes != null)
                {
                    dataRecord.ImageName = $"image_{medicationName}";
                    dataRecord.ThumbnailName = $"thumbnail_{medicationName}";
                    imageLoader.SaveImage(this.Bytes, dataRecord.ImageName);
                    imageLoader.SaveImage(this.Bytes, dataRecord.ThumbnailName, 30);
                }

				await this.storage.SaveAsync<MedicationDosage>(dataRecord);
                //var notification = new CoreNotification(dataRecord.Id.Value, dataRecord.Name, "Rano i wieczorem", new RepeatPattern() { DayOfWeek = dataRecord.Days, Interval = RepetitionInterval.None, RepetitionFrequency = 1 });
                await this.notifications.ScheduleNotification(dataRecord);

                return true;
            }, canSave);


            var canDelete = this.WhenAny(x => x.Id, id => id.Value.HasValue);
            this.Delete = RxUI.ReactiveCommand.CreateFromTask<Data.MedicationDosage, bool>(async _ =>
               {
                   if (this.Id.HasValue)
                   {
                       await this.storage.DeleteByKeyAsync<MedicationDosage>(this.Id.Value);
                       return true;
                   }
                   return false;
             }, canDelete);

            this.SelectAllDays = RxUI.ReactiveCommand.Create(() => { Monday = true; Tuesday = true; Wednesday = true; Thurdsday = true; Friday = true; Saturday = true; Sunday = true; });

            //save sie udal, albo nie - tu dosatniemy rezultat komendy. Jak sie udal, to zamykamy ViewModel
            this.Save
                .Subscribe(result =>
                {
                    if (result)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new DataChangedMessage(this));
                        this.Close(this);
                    }
                });

            this.Save.ThrownExceptions.Subscribe(ex =>
            {
                UserDialogs.Instance.ShowError(AppResources.MedicationDosageView_SaveError);
                // show nice message to the user

            });



            this.Delete
                .Subscribe(result =>
                {
                    if (result)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new DataChangedMessage(this));
                        this.Close(this);
                    }
                });
        }



        public async void Init(MedicationDosageNavigation nav)
        {
            if (nav.MedicationDosageId != MedicationDosageNavigation.NewRecord)
            {
                Data.MedicationDosage item = await storage.GetAsync<Data.MedicationDosage>(nav.MedicationDosageId);
                Id = item.Id;
                MedicationName = item.Name;
                MedicationDosage = item.Dosage;
                Monday = item.Days.HasFlag(DaysOfWeek.Monday);
                Tuesday = item.Days.HasFlag(DaysOfWeek.Tuesday);
                Wednesday = item.Days.HasFlag(DaysOfWeek.Wednesday);
                Thurdsday = item.Days.HasFlag(DaysOfWeek.Thursday);
                Friday = item.Days.HasFlag(DaysOfWeek.Friday);
                Saturday = item.Days.HasFlag(DaysOfWeek.Saturday);
                Sunday = item.Days.HasFlag(DaysOfWeek.Sunday);
                DosageHours = new RxUI.ReactiveList<TimeSpan>(item.DosageHours);

                if (!string.IsNullOrEmpty(item.ImageName))
				    Bytes = imageLoader.LoadImage(item.ImageName);
            }
      
        }
    }
}
