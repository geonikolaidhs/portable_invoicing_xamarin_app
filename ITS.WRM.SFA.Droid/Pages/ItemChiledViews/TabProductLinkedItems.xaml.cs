using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Model;
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
    public partial class TabProductLinkedItems : ContentPage
    {
        ObservableCollection<LinkedItemsDetails> ObservalLinkedItems = new ObservableCollection<LinkedItemsDetails>();
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        Item ItemDetails = new Item();
        public TabProductLinkedItems(Item item)
        {
            InitializeComponent();
            ItemDetails = item;
            InitiallizeControllers();
            lblProduct.Text = ItemDetails.Name;
            LoadLinkedItems(ItemDetails);
        }
        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            lblCode.Text = ResourcesRest.Code;
            lblDescription.Text = ResourcesRest.CustomerDetailLblCompanyName;
            lblQtyFactor.Text = ResourcesRest.TabProductLinkedItemsLblQtyFactor;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }
        private async void LoadLinkedItems(Item item)
        {
            try
            {
                if (item.LinkedItemsDetails == null)
                {
                    return;
                }
                ObservalLinkedItems = new ObservableCollection<LinkedItemsDetails>(item.LinkedItemsDetails);
                LinkedItemsListView.ItemsSource = ObservalLinkedItems;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
    }
}