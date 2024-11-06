using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
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
using XLabs.Forms.Controls;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabItemPrice : ContentPage
    {
        ObservableCollection<PriceCatalogPresentation> ObservablePriceCatalog = new ObservableCollection<PriceCatalogPresentation>();
        public TabItemPrice(Item item)
        {
            InitializeComponent();
            InitiallizeController();
            lblProduct.Text = item.Name;
            LoadData(item);
            var mycheckbox = new CheckBox();

        }
        private void LoadData(Item item)
        {
            try
            {
                foreach (PriceCatalogDetail detail in item.PriceCatalogDetails)
                {
                    ObservablePriceCatalog.Add(new PriceCatalogPresentation()
                    {
                        PriceCatalogDescription = detail.PriceCatalog.Description,
                        CreatedOnDate = new DateTime(detail.CreatedOnTicks),
                        UpdatedOnDate = new DateTime(detail.UpdatedOnTicks),
                        Value = ResourcesRest.Price + ": " + detail.Value.ToString("C" + DependencyService.Get<ICrossPlatformMethods>().GetOwnerApplicationSettings()?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                        IsActiveHeader = detail.IsActive == true ? ResourcesRest.IsActive : ResourcesRest.Inactive,
                        VatIncludedHeader = detail.VATIncluded == true ? ResourcesRest.WithVat : ResourcesRest.WithoutVat,
                        Createheader = ResourcesRest.CreatedOn + ": " + new DateTime(detail.CreatedOnTicks).ToString(),
                        UpdateHeader = ResourcesRest.UpdatedOn + ": " + new DateTime(detail.UpdatedOnTicks),

                    });

                }
                PriceCatalogListView.ItemsSource = ObservablePriceCatalog;

            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }
        private void InitiallizeController()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");

            //lblCatalog.Text = ResourcesRest.PriceCatalog;
            //lblBarcode.Text = ResourcesRest.CodeBarcode;
            //lblCreated.Text = ResourcesRest.CreatedOn;
            //lblUpdated.Text = ResourcesRest.UpdatedOn;
            //lblPrice.Text = ResourcesRest.DefaultValue;
            //lblDiscount.Text = ResourcesRest.Discount;
            //lblDMarkup.Text = ResourcesRest.Markup;
            //lblVat.Text = ResourcesRest.IsVat;
            //lblActive.Text = ResourcesRest.Active;
        }
    }
}