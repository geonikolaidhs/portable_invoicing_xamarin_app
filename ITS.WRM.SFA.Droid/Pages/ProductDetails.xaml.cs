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
    public partial class ProductDetails : TabbedPage
    {

        Item ItemDetails = new Item();

        private string TabDetails;
        private string TabBarcodes;
        private string TabCategories;
        private string TabMotherCodes;
        private string TabLinkedItems;
        private string TabLinkedTo;
        private string TabExtraInfo;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        private string TabPrices;

        public ProductDetails()
        {
            InitializeComponent();
            InitiallizeControllers();
        }
        public ProductDetails(Item Item)
        {
            InitializeComponent();
            LoadItemDetails(Item.Oid);
            this.BindingContext = ItemDetails;
            InitiallizeControllers();

            Children.Add(new TabProductMainDetails(ItemDetails) { Title = TabDetails });
            Children.Add(new TabItemPrice(ItemDetails) { Title = TabPrices });
            Children.Add(new TabProductBarcodes(ItemDetails) { Title = TabBarcodes });
            Children.Add(new TabProductCategories(ItemDetails) { Title = TabCategories });
            Children.Add(new TabProductLinkedItems(ItemDetails) { Title = TabLinkedItems });
        }
        private async void LoadItemDetails(Guid Oid)
        {
            try
            {
                ItemDetails = DependencyService.Get<ICrossPlatformMethods>().GetItemDetails(Oid);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            TabDetails = ResourcesRest.ProductDetailsTabDetails;
            TabBarcodes = ResourcesRest.ProductDetailsTabBarcodes;
            TabCategories = ResourcesRest.ProductDetailsCategories;
            TabMotherCodes = ResourcesRest.ProductDetailsTabMotherCodes;
            TabLinkedItems = ResourcesRest.ProductDetailsTabLikedItems;
            TabLinkedTo = ResourcesRest.ProductDetailsTabLinkedTo;
            TabExtraInfo = ResourcesRest.ProductDetailsTabItemExtraInfo;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
            TabPrices = ResourcesRest.Prices;
        }


    }
}