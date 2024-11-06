using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabProductMainDetails : ContentPage
    {
        Item ItemDetails = new Item();
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;

        public TabProductMainDetails(Item Item)
        {
            InitializeComponent();
            InitiallizeControllers();
            ItemDetails = Item;
            FillData(ItemDetails);
            this.BindingContext = ItemDetails;
            lblDescription.Text = Item.Name;
        }

        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture = DependencyService.Get<ICrossPlatformMethods>().Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblCode.Text = ResourcesRest.Code + " : ";
            lblDefaultBarcode.Text = ResourcesRest.TabProductMainDetailsLblDefaultBarcode + " : ";
            lblVatCategory.Text = ResourcesRest.TabProductMainDetailsLblVatCategory + " : ";
            lblInsertedDate.Text = ResourcesRest.TabProductMainDetailsLblInsertedDate + " : ";
            lblPoints.Text = ResourcesRest.TabProductMainDetailsLblPoints + " : ";
            lblPackingQty.Text = ResourcesRest.TabProductMainDetailsLblPackingQty + " : ";
            lblStock.Text = ResourcesRest.Stock + " : ";
            lblDefaultSupplier.Text = ResourcesRest.TabProductMainDetailsLblDefaultSupplier + " : ";
            lblOrderQty.Text = ResourcesRest.TabProductMainDetailsLblOrderQty + " : ";
            lblItemName.Text = ResourcesRest.Description + " : ";
            lblMeasurementUnit.Text = ResourcesRest.MeasurementUnit + " : ";
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }
        private async void FillData(Item itemObj)
        {
            try
            {
                Guid CurrentItemOid = itemObj.Oid;
                Supplier SuplierItem = itemObj.GetDefaultSupplier(App.DbLayer);
                string SuplierDescription = SuplierItem == null ? "" : SuplierItem.CompanyName;
                string measurementUnitDescription = App.DbLayer.GetBarcodeById(itemObj.DefaultBarcodeOid)?.MeasurementUnit(App.DbLayer, App.Owner)?.Description;
                codeValue.Text = itemObj.Code;
                ItemNameValue.Text = itemObj.Name;
                BarcodeValue.Text = itemObj.DefaultBarcode.Code;
                VatCategoryValue.Text = itemObj.VatCategory.Description;
                Stock.Text = itemObj.Stock.ToString();
                PackingQtyValue.Text = itemObj.PackingQty.ToString();
                OrderQtyValue.Text = itemObj.OrderQty.ToString();
                InsertDateValue.Text = new DateTime(itemObj.CreatedOnTicks).ToString("dd/MM/yyyy");
                PointsValue.Text = itemObj.Points.ToString();
                MeasurementValue.Text = measurementUnitDescription;
                DefaultSuplierValue.Text = SuplierDescription;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
    }
}