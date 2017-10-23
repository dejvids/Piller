using System;
using Foundation;
using ReactiveUI;
using UIKit;

namespace Piller.iOS.Common.Dialog
{
    public class PhotoElement : Element
    {
		const string CellKey = "SinglelineEdit";
        UIImage photo;
        public event EventHandler ValueChanged;
        public UIImage Photo
        {
            get { return photo; }
			set
			{
                this.RaiseAndSetIfChanged(ref photo, value);
				this.ValueChanged?.Invoke(this, EventArgs.Empty);
			}
        }

		public string Title { get; set; }
        public PhotoElement()
        {
            photo = UIImage.FromBundle("thumbnail");
        }
		protected override UITableViewCell GetCell(UITableView tv, NSIndexPath indexPath)
		{
            var cell = this.CurrentAttachedCell as PhotoCell;

			if (cell == null)
			{
                cell = new PhotoCell(CellKey);

                /*
                cell.Photo.Events().EditingChanged.Subscribe(x =>
				{
					var textView = cell.Input;
					this.Value = textView.Text == "" ? null : textView.Text;
				}).AddTo(Subscriptions);
				*/

			}


			cell.TitleLabel.Text = this.Title;
            cell.PhotoView.Image = this.Photo;


			
			cell.BackgroundColor = this.BackgroundColor;

			cell.TitleLabel.TextColor = this.CaptionColor;
			return cell;
		}


    }
}
