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

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabProductCategories : ContentPage
    {
        ObservableCollection<CategoryNodePath> ObservalCategoryPathList = new ObservableCollection<CategoryNodePath>();
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;

        public TabProductCategories(Item ItemDetails)
        {
            InitializeComponent();
            lblProduct.Text = ItemDetails.Name;
            LoadCategoryPath(ItemDetails);
            InitiallizeControllers();
        }
        private async void LoadCategoryPath(Item ItemDetails)
        {
            try
            {
                List<ItemAnalyticTree> ListItemAnalyticalTree = ItemDetails.ItemAnalyticTrees;
                List<CategoryNodePath> CategoryPath = new List<CategoryNodePath>();
                DatabaseLayer DBLayer = DependencyService.Get<ICrossPlatformMethods>().GetDataBaseLayer();
                foreach (ItemAnalyticTree itemAnTree in ListItemAnalyticalTree)
                {
                    ItemCategory itemCategory = itemAnTree.GetItemCategory(itemAnTree.NodeOid, DBLayer);
                    string strNode = itemCategory?.Description ?? "";
                    string strPath = itemCategory?.CategoryPath(DBLayer) ?? "";
                    CategoryPath.Add(new CategoryNodePath() { Root = itemAnTree.GetCategoryNode(DBLayer)?.Description ?? "", Path = strPath, node = strNode });
                }

                foreach (CategoryNodePath itemCategoryPath in CategoryPath)
                {
                    ObservalCategoryPathList.Add(itemCategoryPath);
                }
                CategoryPathListView.ItemsSource = ObservalCategoryPathList;
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
            lblRoot.Text = ResourcesRest.TabProductCategoryLblRoot;
            lblPath.Text = ResourcesRest.TabProductCategoryLblPath;
            lblNode.Text = ResourcesRest.TabProductCategoryLblNode;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            strMsgOk = ResourcesRest.MsgBtnOk;
        }
    }
}