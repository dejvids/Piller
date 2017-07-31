using System;
using MvvmCross.iOS.Views;
using Piller.ViewModels;
using Piller.iOS.Common;
using Piller.Resources;
using MvvmCross.Binding.BindingContext;
using UIKit;
using Piller.iOS.Common.Dialog;
using System.Collections.Generic;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Foundation;

namespace Piller.iOS.Views
{
    [MvxChildPresentation]
    [Register("MedicationDosageView")]
    public class MedicationDosageView : MvxTableViewController<MedicationDosageViewModel>
    {

        public MedicationDosageView() : base(UITableViewStyle.Grouped)
        {
            
        }
        ElementTableSource tableSource;
        SingleLineEditElement drugNameElement;
        SingleLineEditElement dosageElement;
        UIBarButtonItem saveButton;

        public override void ViewDidLoad()
        {
          
            base.ViewDidLoad();
            this.Title = "Nowy lek";

            tableSource = new ElementTableSource(createEditForm(), this.TableView);
            this.TableView.RowHeight = UITableView.AutomaticDimension;
            this.TableView.EstimatedRowHeight = 44f;
            this.TableView.Source = tableSource;

            this.saveButton = new UIBarButtonItem (UIKit.UIBarButtonSystemItem.Save);
            this.NavigationItem.RightBarButtonItem = saveButton;

            /*
            setBindings();

            */
        }

        private FormDefinition createEditForm()
        {
            drugNameElement = new SingleLineEditElement { Title = AppResources.MedicationDosageView_MedicationName };
            var drugEntrySection = new Section(String.Empty, new List<Element> { drugNameElement });
            
            dosageElement = new SingleLineEditElement { Title = "Dawka" };
            var dosageEntrySection = new Section(String.Empty, new List<Element> { dosageElement });

            var rootElement = new FormDefinition(new List<Section> { drugEntrySection,dosageEntrySection });
       
            return rootElement;
        }
        /*
        private void setBindings()
        {
  
            var bindingSet = this.CreateBindingSet<MedicationDosageView, MedicationDosageViewModel>();

            bindingSet.Bind(drugNameElement)
                      .For(element => element.Value)
                      .To(viewModel => viewModel.MedicationName)
                      .TwoWay();

            bindingSet.Bind (this.saveButton).To (vm => vm.Save);
            
            bindingSet.Apply();
        }

        */
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.tableSource.Dispose();
        }
    }
}
