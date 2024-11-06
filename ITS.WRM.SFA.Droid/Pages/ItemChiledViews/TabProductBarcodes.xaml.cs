using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabProductBarcodes : ContentPage
    {
        ObservableCollection<BarcodeItems> ObservalBarcodes = new ObservableCollection<BarcodeItems>();
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        public TabProductBarcodes(Item ItemDetails)
        {
            InitializeComponent();
            InitiallizeControllers();
            LoadBarcode(ItemDetails);
            lblProduct.Text = ItemDetails.Name;
        }
        private async void LoadBarcode(Item item)
        {
            try
            {
                if (item.BarcodeDetails == null)
                {
                    return;
                }
                foreach (BarcodeDetails dtl in item.BarcodeDetails)
                {
                    BarcodeItems bcItem = new BarcodeItems() { Code = dtl.Barcode, MesurementUnit = dtl.MeasurementUnit, CreateOn = dtl.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss"), UpdateOn = dtl.UpdatedOn.ToString("dd/MM/yyyy HH:mm:ss") };
                    ObservalBarcodes.Add(bcItem);
                }
                BarcodeListView.ItemsSource = ObservalBarcodes;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblBarcode.Text = ResourcesRest.TabProductBarcodesLblBarcode;
            lblMeasuremantUnit.Text = ResourcesRest.TabProductBarcodeLblMeasurmentUnit;
            lblCreateOn.Text = ResourcesRest.TabProductBarcodeLblCreatedOn;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }
    }
}