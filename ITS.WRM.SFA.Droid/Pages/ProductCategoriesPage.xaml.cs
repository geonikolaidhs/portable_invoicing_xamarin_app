using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs.Forms.Controls;

using ImageButton = XLabs.Forms.Controls.ImageButton;

namespace ITS.WRM.SFA.Droid.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductCategoriesPage : ContentPage, INotifyPropertyChanged
    {

        ObservableCollection<ItemPresent> ObservalItems = new ObservableCollection<ItemPresent>();
        Guid? SelectedCategory = App.SFASettings.CategoryNode;
        private Guid? ParentSelectedCategory;
        private List<ItemCategory> AllCategories = new List<ItemCategory>();
        private string strNoResultsFoundMessage;
        private string msgItemNotIncludeInThisCategory;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        Guid CategoryItem = App.SFASettings.CategoryNode;
        private ItemPresent _CurrentSelectedItem;
        private string strMsgTypeLoadAll, strMsgLoad, strMsgYes, strMsgNo;
        private enumModel.SearchCriteria QueryType;
        private static bool EventRegistered = false;
        protected GridLength FirstColumn = new GridLength(30, GridUnitType.Star);
        protected GridLength SecondColumn = new GridLength(70, GridUnitType.Star);
        bool showStock = true;



        public ProductCategoriesPage()
        {
            InitializeComponent();
            InitiallizeControllers();
            BindingContext = this;
            Title = "user: " + App.UserName;
            ItemListView.IsVisible = true;
            btnCategoryParth.FontSize = 16;
            btnCategoryParth.Padding = 0;
            btnCategoryParth.Margin = 0;
            AllCategories = App.AllCategories;
            LoadViewCategories(false);
            SearchHeader.Text = ResourcesRest.Results;
            PageGrid.ColumnDefinitions[0].Width = FirstColumn;
            PageGrid.ColumnDefinitions[1].Width = SecondColumn;
        }


        /// <summary>
        /// MESSAGING CENTER
        /// </summary> 
        #region 

        protected override void OnDisappearing()
        {
            UnregisterFromEvents();
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            RegisterOnEvents();
            base.OnAppearing();
        }

        private void UnregisterFromEvents()
        {
            MessagingCenter.Unsubscribe<App, string>(this, EventNames.SCAN_EVENT);
            EventRegistered = false;
        }

        private void RegisterOnEvents()
        {
            if (!EventRegistered)
            {
                if (!DependencyService.Get<IBlueTooth>().Connected())
                {
                    DependencyService.Get<IBlueTooth>().Start(App.SFASettings.BlueToothScanner, eBlueToothDevice.SCANNER);
                }
                if (!EventRegistered)
                {
                    MessagingCenter.Subscribe<App, string>(this, EventNames.SCAN_EVENT, (sender, arg) =>
                    {
                        OnScan(arg);
                    });
                    EventRegistered = true;
                }
                EventRegistered = true;
            }
        }
        #endregion
        ///------------END-------------///

        /// <summary>
        /// ITEM CATEGORY EVENTS & NAVIGATION
        /// </summary> 
        #region 
        void OnBackCategory(object sender, EventArgs e)
        {
            LoadViewCategories(false, true);
        }

        async void LoadViewCategories(bool categoryTapped = false, bool backTapped = false)
        {
            try
            {
                if (AllCategories == null)
                {
                    return;
                }

                List<ItemCategory> viewList = new List<ItemCategory>();
                ItemCategory selected;
                CategoryList.ItemsSource = null;
                selected = AllCategories.Where(x => x.Oid.Equals(SelectedCategory)).FirstOrDefault();
                if (backTapped)
                {
                    if (SelectedCategory != null && SelectedCategory != Guid.Empty && SelectedCategory != new Guid("980D5D85-F033-425E-BBD2-055780805F75"))
                    {
                        Clear();
                        SelectedCategory = selected.ParentOid;
                        LoadViewCategories(false, false);
                        return;
                    }
                }
                if (SelectedCategory == null || SelectedCategory == Guid.Empty)
                {
                    viewList = AllCategories.Where(x => x.ParentOid == null)?.ToList() ?? new List<ItemCategory>();
                    btnCategoryParth.Text = ResourcesRest.All_ItemCategories;
                }
                else
                {
                    viewList = AllCategories.Where(x => x.ParentOid == SelectedCategory)?.ToList() ?? new List<ItemCategory>();
                    btnCategoryParth.Text = selected.FullDescription;
                }
                if (viewList.Count == 0)
                {
                    viewList.Add(new ItemCategory() { Oid = new Guid("980D5D85-F033-425E-BBD2-055780805F75"), Description = ResourcesRest.SubCategoriesNotFound });
                }
                CategoryList.ItemsSource = viewList;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        public async void OnCategoryTap(object objectList, ItemTappedEventArgs e)
        {
            try
            {
                ListView listView = objectList as ListView;
                ItemCategory TappedCategory = (ItemCategory)listView.SelectedItem;
                if (TappedCategory != null && TappedCategory.Oid != null && TappedCategory?.Oid != new Guid("980D5D85-F033-425E-BBD2-055780805F75"))
                {
                    SelectedCategory = TappedCategory.Oid;
                    ParentSelectedCategory = TappedCategory.ParentOid;
                    btnCategoryParth.Text = TappedCategory.Description;
                    Clear();
                    LoadViewCategories(true);
                }
                else
                {
                    var viewList = CategoryList.ItemsSource;
                    CategoryList.ItemsSource = null;
                    CategoryList.ItemsSource = viewList;
                    CategoryList.BackgroundColor = Color.FromHex("#474a4f");
                    btnCategoryParth.Text = btnCategoryParth.Text;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        #endregion
        ///------------END-------------///


        /// <summary>
        /// SEARCH EVENTS & DOCUMENT FUNCTIONS
        /// </summary> 
        #region 
        private void Clear()
        {
            BarcodeInput.Text = string.Empty;
            DescriptionInput.Text = string.Empty;
            ObservalItems.Clear();
            SearchHeader.Text = ResourcesRest.SearchResults;
        }

        protected async void btnSearchItem(object sender, EventArgs e)
        {
            try
            {
                await Search();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }
        private void GetSearchCriteria(out string codeSearch, out string nameSearch, out DateTime DateInsert, out DateTime DateUpdate)
        {
            bool IsActiveDateInsert = switcDateFromActive.IsToggled ? true : false;
            bool IsActiveUpdatedDAte = switchUpdatedActive.IsToggled ? true : false;
            DateUpdate = pckDateUpdated.Date;
            DateInsert = pckDateInsertFrom.Date;
            codeSearch = BarcodeInput.Text;
            nameSearch = DescriptionInput.Text;
            if (IsActiveDateInsert && IsActiveUpdatedDAte)
            {
                QueryType = enumModel.SearchCriteria.IsActiveDateFromAndIsActiveUpdatedDate;
            }
            else if (IsActiveUpdatedDAte == true && IsActiveDateInsert == false)
            {
                QueryType = enumModel.SearchCriteria.IsActiveUpdatedFrom;
            }
            else if (IsActiveDateInsert == true && IsActiveUpdatedDAte == false)
            {
                QueryType = enumModel.SearchCriteria.IsActiveDateFrom;
            }
            else if (!IsActiveUpdatedDAte && !IsActiveDateInsert)
            {
                QueryType = enumModel.SearchCriteria.NotActiveDates;
            }
        }

        private void AfterSearchActions(List<ItemPresent> ItemList, int resultCount)
        {
            try
            {
                ItemList.ForEach(x => x.ShowStock = showStock);
                ObservalItems.Clear();
                ObservalItems = (ItemList == null || resultCount == 0) == true ? new ObservableCollection<ItemPresent>() { new ItemPresent() { Name = strNoResultsFoundMessage } } : new ObservableCollection<ItemPresent>(ItemList);
                fillNumberOfItems();
                ItemListView.ItemsSource = ObservalItems;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                });
            }
            UserDialogs.Instance.HideLoading();
        }

        private async Task<int> Search(bool fromScanner = false)
        {
            try
            {
                List<ItemPresent> ItemList = new List<ItemPresent>();
                string codeSearch = string.Empty;
                string nameSearch = string.Empty;
                DateTime DateInsert = DateTime.Now;
                DateTime UpdatedDate = DateTime.Now;
                bool SearchAllcategories = switchSearchAll.IsToggled ? true : false;
                bool IsActiveItem = switchActiveItem.IsToggled ? true : false;
                bool onlyItemsWithStock = switchOnlyStockItems.IsToggled ? true : false;
                int resultsCount = 0;
                GetSearchCriteria(out codeSearch, out nameSearch, out DateInsert, out UpdatedDate);
                if (fromScanner && !string.IsNullOrEmpty(codeSearch))
                {
                    ObservalItems.Clear();
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        ItemList = App.DbLayer.SearchItems(Guid.Empty, codeSearch, true, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, true, out resultsCount);
                    });
                    AfterSearchActions(ItemList, resultsCount);
                }
                else
                {
                    if (SelectedCategory == null || SelectedCategory == Guid.Empty || SearchAllcategories)
                    {
                        if ((codeSearch?.Count() ?? 0) < 2 && (nameSearch?.Count() ?? 0) < 2 && !switchOnlyStockItems.IsToggled)
                        {
                            await DisplayAlert(strMsgLoad, ResourcesRest.InsertAtLeastTwoCharacters, strMsgOk);
                            return 0;
                        }
                    }
                    if (string.IsNullOrEmpty(codeSearch) && string.IsNullOrEmpty(nameSearch))
                    {
                        string question = !switchOnlyStockItems.IsToggled ? ResourcesRest.LoadAllCategoryItems : ResourcesRest.LoadAllStockItems;
                        var answer = await DisplayAlert(strMsgLoad, question, strMsgYes, strMsgNo);
                        if (answer == true)
                        {
                            ObservalItems.Clear();
                            UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);

                            await Task.Run(() =>
                            {
                                ItemList = App.DbLayer.SearchItems(SelectedCategory, "", false, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, true, out resultsCount);
                            });
                            AfterSearchActions(ItemList, resultsCount);
                        }
                    }
                    else if (!string.IsNullOrEmpty(codeSearch))
                    {
                        UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                        BarcodeParseResult barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode<BarcodeType>(App.BarcodeTypes,
                                                                 codeSearch,
                                                                 App.OwnerApplicationSettings.PadBarcodes,
                                                                 App.OwnerApplicationSettings.BarcodeLength,
                                                                 App.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                                 );

                        if (barcodeParseResult != null && (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY || barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE))
                        {
                            codeSearch = barcodeParseResult.DecodedCode;
                        }
                        ObservalItems.Clear();

                        await Task.Run(() =>
                        {
                            ItemList = App.DbLayer.SearchItems(SelectedCategory, codeSearch, true, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, true, out resultsCount);

                        });
                        AfterSearchActions(ItemList, resultsCount);
                    }
                    else if (!string.IsNullOrEmpty(nameSearch))
                    {
                        ObservalItems.Clear();
                        UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                        await Task.Run(() =>
                        {
                            ItemList = App.DbLayer.SearchItems(SelectedCategory, nameSearch, false, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, true, out resultsCount);
                        });
                        AfterSearchActions(ItemList, resultsCount);
                    }
                }
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
            return 1;
        }


        private void fillNumberOfItems()
        {
            SearchHeader.Text = string.Format("{0} : {1}", ResourcesRest.Results, ObservalItems.Count);
        }

        protected void OnScan(string code)
        {
            try
            {
                OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
                string paddedCode = (ownAppSet.PadBarcodes) ? code.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : code;
                string codeSearch = string.Empty;
                string nameSearch = string.Empty;
                DateTime DateInsert = DateTime.Now;
                DateTime UpdatedDate = DateTime.Now;
                bool SearchAllcategories = switchSearchAll.IsToggled ? true : false;
                bool IsActiveItem = switchActiveItem.IsToggled ? true : false;
                bool onlyItemsWithStock = switchOnlyStockItems.IsToggled ? true : false;
                GetSearchCriteria(out codeSearch, out nameSearch, out DateInsert, out UpdatedDate);

                ItemDetail item = App.DbLayer.GetItemByScanner(SelectedCategory, paddedCode, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, true);
                if (item == null)
                {
                    throw new Exception(strNoResultsFoundMessage);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    _CurrentSelectedItem = item.ConvertToItemPresent();
                    _CurrentSelectedItem.ShowStock = showStock;
                    ObservalItems.Clear();
                    ObservalItems.Add(_CurrentSelectedItem);
                    fillNumberOfItems();
                    ItemListView.ItemsSource = ObservalItems;
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
            UserDialogs.Instance.HideLoading();
        }


        protected async void SearchBarcodeByKeyboard(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(BarcodeInput.Text) || string.IsNullOrWhiteSpace(BarcodeInput.Text) || BarcodeInput.Text.Length < 2)
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.InsertAtLeastTwoCharacters, strMsgOk);

                OnScan(BarcodeInput.Text);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        protected async void SearchDescriptionByKeyboard(object sender, EventArgs e)
        {
            try
            {
                await Search();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        #endregion
        ///-----END-------------///


        /// <summary>
        /// NAVIGATION
        /// </summary> 
        #region 
        void OnCheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxFilterPanel.Checked)
            {
                PageGrid.ColumnDefinitions[0].Width = FirstColumn;
                PageGrid.ColumnDefinitions[1].Width = SecondColumn;
                FilterPanelStack.IsVisible = true;
            }
            else
            {
                PageGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                PageGrid.ColumnDefinitions[1].Width = new GridLength(100, GridUnitType.Star);
                FilterPanelStack.IsVisible = false;
            }
        }

        private async Task NavigateToProductDetails(Guid SelectedItemOid)
        {
            await Task.Run(() =>
            {
                IReadOnlyList<Page> stack = Navigation.NavigationStack;
                if (stack[stack.Count - 1].GetType() != typeof(ProductDetails))
                {
                    Item CurrentItem = DependencyService.Get<ICrossPlatformMethods>().GetItem(SelectedItemOid);
                    if (CurrentItem == null)
                    {
                        return;
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushModalAsync(new ProductDetails(CurrentItem), true);
                    });
                }
            });
        }

        async void OnMore(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                MenuItem item = (MenuItem)sender;
                Guid SelectedItemOid = ObservalItems.Where(x => x.Oid.Equals(item.CommandParameter)).First().Oid;
                if (SelectedItemOid == Guid.Empty || SelectedItemOid == null)
                {
                    return;
                }
                else
                {
                    await NavigateToProductDetails(SelectedItemOid);
                }
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }
        #endregion
        ///-----END-------------///


        /// <summary>
        /// FOCUS EVENTS
        /// </summary> 
        #region 

        protected void OnDescriptionInputFocused(object sender, EventArgs e)
        {
            DescriptionInput.Text = string.Empty;
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnDescriptionInputUnFocused(object sender, EventArgs e)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void OnBarcodeInputFocused(object sender, EventArgs e)
        {
            BarcodeInput.Text = string.Empty;
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnBarcodeInputUnFocused(object sender, EventArgs e)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        #endregion
        ///-----END-------------///


        private void InitiallizeControllers()
        {
            ResourcesRest.Culture = ResourcesRest.Culture =  App.Languageinfo(App.SFASettings.Language ?? "en-GB");
            BarcodeInput.Placeholder = ResourcesRest.ProductPageSearchByCodeOrBaercode;
            DescriptionInput.Placeholder = ResourcesRest.SearchByDescription;
            strNoResultsFoundMessage = ResourcesRest.NoResultsFoundMessage;
            BarcodeInput.Placeholder = ResourcesRest.ProductPageSearchBarByBarcode;
            strMsgTypeAlert = ResourcesRest.MsgTypeAlert;
            strMsgOk = ResourcesRest.MsgBtnOk;
            SearchHeader.Text = ResourcesRest.Results;
            strMsgFailure = ResourcesRest.sendExceptionMsg;
            Header.Text = ResourcesRest.NewOrderTabInsertItemPage;
            lblSwitch.Text = ResourcesRest.IsActiveItem;
            lblDateInserFrom.Text = ResourcesRest.InsertedFroDatem;
            lblDateUpdatedOn.Text = ResourcesRest.DateUpdated;
            strMsgLoad = ResourcesRest.LoadAllItems;
            strMsgTypeLoadAll = ResourcesRest.msgLoadAllItems;
            strMsgYes = ResourcesRest.MsgYes;
            strMsgNo = ResourcesRest.MsgNo;
            msgItemNotIncludeInThisCategory = ResourcesRest.msgItemNotIncludeInThisCategoryItem;
            lblSearchAll.Text = ResourcesRest.SearchAllCategories;
            lblOnlyStockItems.Text = ResourcesRest.OnlyItemsWithStock;
            lblDescription.Text = ResourcesRest.Description;
            lblCode.Text = ResourcesRest.Code;
            lblStock.Text = ResourcesRest.Stock;
            switchOnlyStockItems.IsToggled = false;
        }


    }
}