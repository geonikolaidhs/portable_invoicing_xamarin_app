using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Devices;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Interface;
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
    public partial class DocumentProductCategoriesPage : ContentPage
    {
        ObservableCollection<ItemPresent> ObservalItems = new ObservableCollection<ItemPresent>();
        Guid? SelectedCategory = App.SFASettings.CategoryNode;
        private Guid? ParentSelectedCategory;
        private List<ItemCategory> AllCategories = new List<ItemCategory>();
        private string strNoResultsFoundMessage;
        DocumentHeader _CurrentDocumentHeader = new DocumentHeader();
        private string msgItemNotIncludeInThisCategory;
        private string strMsgTypeAlert, strMsgFailure, strMsgOk;
        Guid CategoryItem = App.SFASettings.CategoryNode;
        Image cart = new Image();
        private ItemPresent _CurrentSelectedItem;
        private List<ItemDetail> ItemsToAdd = new List<ItemDetail>();
        private string strMsgTypeLoadAll, strMsgLoad, strMsgYes, strMsgNo;
        private enumModel.SearchCriteria QueryType;
        private static bool EventRegistered = false;
        protected GridLength FirstColumn = new GridLength(30, GridUnitType.Star);
        protected GridLength SecondColumn = new GridLength(70, GridUnitType.Star);
        private static List<char> Numeric = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };
        bool showStock = false;
        public double OverlayHeight = Screen.Height;
        public double OverlayWidth = Screen.Width;
        private static object locker = new object();
        private static decimal MaxQty = 9999;
        Color CheckBoxTextColor = Color.WhiteSmoke;
        bool DeletePreviousQty = true;
        bool DeletePreviousPackingQty = true;
        private eFocusedEntry FocusedEntry = eFocusedEntry.NONE;

        public DocumentProductCategoriesPage(DocumentHeader DocumentHeader)
        {
            OverlayHeight = Screen.Height;
            OverlayWidth = Screen.Width;
            InitializeComponent();
            overlay.IsVisible = false;
            InitiallizeControllers(DocumentHeader);
            BindingContext = this;
            Title = "user: " + App.UserName;
            ItemListView.IsVisible = true;
            btnCategoryParth.FontSize = 16;
            btnCategoryParth.Padding = 0;
            btnCategoryParth.Margin = 0;
            AllCategories = App.AllCategories;
            LoadViewCategories(false);
            this._CurrentDocumentHeader = DocumentHeader;
            cart.Source = ImageSource.FromResource("cart");
            SearchHeader.Text = ResourcesRest.Results;
            PageGrid.ColumnDefinitions[0].Width = FirstColumn;
            PageGrid.ColumnDefinitions[1].Width = SecondColumn;
            Xamarin.Forms.MessagingCenter.Send<App, List<ItemDetail>>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT, ItemsToAdd);
            Xamarin.Forms.MessagingCenter.Send<App, DocumentHeader>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_EVENT, _CurrentDocumentHeader);

        }

        /// <summary>
        /// MESSAGING CENTER
        /// </summary> 
        #region     

        protected override void OnDisappearing()
        {
            if (App.SkipAsking == false)
            {
                UnregisterFromEvents();
                base.OnDisappearing();
            }
        }

        protected override void OnAppearing()
        {
            App.SkipAsking = false;
            RegisterOnEvents();
            if (ObservalItems != null)
            {
                ObservalItems.Clear();
                RefreshItemListView();
            }
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
                App.LogError(ex);
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
        private void Clear()
        {
            BarcodeInput.Text = string.Empty;
            DescriptionInput.Text = string.Empty;
            ObservalItems.Clear();
            SearchHeader.Text = ResourcesRest.SearchResults;
        }

        protected void ShowListToAdd(object sender, EventArgs e)
        {
            try
            {
                ObservalItems.Clear();
                foreach (ItemDetail itm in ItemsToAdd.OrderBy(x => x.UpdatedOnTicks))
                {
                    ItemPresent present = itm.ConvertToItemPresent();
                    present.QtyOnCurrentDocument = ItemHelper.GetCurrentDocumentItemQuantity(present.Oid, _CurrentDocumentHeader?.DocumentDetails ?? new List<DocumentDetail>());
                    present.QtyOnTemporaryList = present.Qty;
                    present.ShowStock = showStock;
                    ObservalItems.Add(present);
                }
                ItemListView.ItemsSource = ObservalItems.OrderByDescending(x => x.UpdatedOnTicks);
                fillNumberOfItems();
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
        protected async void btnSearchItem(object sender, EventArgs e)
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
        private void GetSearchCriteria(out string codeSearch, out string nameSearch, out DateTime DateInsert, out DateTime DateUpdate)
        {
            bool IsActiveDateInsert = switcDateFromActive.Checked ? true : false;
            bool IsActiveUpdatedDAte = switchUpdatedActive.Checked ? true : false;
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
                ObservalItems.Clear();
                if (ItemList == null || resultCount == 0)
                {
                    ObservalItems = new ObservableCollection<ItemPresent>() { new ItemPresent() { Name = strNoResultsFoundMessage } };
                }
                else
                {
                    if (!_CurrentDocumentHeader.IsSynchronized)
                    {
                        List<Guid> itemOids = new List<Guid>();
                        itemOids.AddRange(ItemsToAdd.Select(x => x.ItemOid)?.ToList() ?? new List<Guid>());
                        itemOids.AddRange(_CurrentDocumentHeader?.DocumentDetails.Select(x => x.ItemOid).Where(z => !itemOids.Contains(z)) ?? new List<Guid>());
                        foreach (Guid ItemOid in itemOids)
                        {
                            ItemPresent present = ItemList.Where(x => x.Oid == ItemOid).FirstOrDefault();
                            if (present != null)
                            {
                                decimal tempQty = ItemHelper.GetItemQuantityFromTemporaryList(present.Oid, ItemsToAdd ?? new List<ItemDetail>());
                                present.Qty = tempQty;
                                if (showStock)
                                {
                                    decimal documentQty = ItemHelper.GetCurrentDocumentItemQuantity(present.Oid, _CurrentDocumentHeader?.DocumentDetails ?? new List<DocumentDetail>());
                                    present.QtyOnTemporaryList = tempQty;
                                    present.QtyOnCurrentDocument = documentQty;
                                    if (present.Stock == (double)present.QtyOnCurrentDocument)
                                        ItemList.Remove(present);
                                }
                            }
                        }
                    }
                    ObservalItems = new ObservableCollection<ItemPresent>(ItemList);
                }
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

        private async Task<int> Search()
        {
            try
            {
                List<ItemPresent> ItemList = new List<ItemPresent>();
                string codeSearch = string.Empty;
                string nameSearch = string.Empty;
                DateTime DateInsert = DateTime.Now;
                DateTime UpdatedDate = DateTime.Now;
                bool SearchAllcategories = switchSearchAll.Checked ? true : false;
                bool IsActiveItem = switchActiveItem.Checked ? true : false;
                bool onlyItemsWithStock = switchOnlyStockItems.Checked ? true : false;
                int resultsCount = 0;
                GetSearchCriteria(out codeSearch, out nameSearch, out DateInsert, out UpdatedDate);
                if (SelectedCategory == null || SelectedCategory == Guid.Empty || SearchAllcategories)
                {
                    if ((codeSearch?.Count() ?? 0) < 2 && (nameSearch?.Count() ?? 0) < 2 && !switchOnlyStockItems.Checked)
                    {
                        await DisplayAlert(strMsgLoad, ResourcesRest.InsertAtLeastTwoCharacters, strMsgOk);
                        return 0;
                    }
                }
                if (string.IsNullOrEmpty(codeSearch) && string.IsNullOrEmpty(nameSearch))
                {
                    string question = !switchOnlyStockItems.Checked ? ResourcesRest.LoadAllCategoryItems : ResourcesRest.LoadAllStockItems;
                    var answer = await DisplayAlert(strMsgLoad, question, strMsgYes, strMsgNo);
                    if (answer == true)
                    {
                        ObservalItems.Clear();
                        UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);

                        await Task.Run(() =>
                        {
                            ItemList = App.DbLayer.SearchItems(SelectedCategory, "", false, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, showStock, out resultsCount);
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
                        ItemList = App.DbLayer.SearchItems(SelectedCategory, codeSearch, true, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, showStock, out resultsCount);
                    });
                    AfterSearchActions(ItemList, resultsCount);
                }
                else if (!string.IsNullOrEmpty(nameSearch))
                {
                    ObservalItems.Clear();
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SearchingDataPleaseWait, MaskType.Black);
                    await Task.Run(() =>
                    {
                        ItemList = App.DbLayer.SearchItems(SelectedCategory, nameSearch, false, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, showStock, out resultsCount);
                    });
                    AfterSearchActions(ItemList, resultsCount);
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

        private PriceCatalogDetail GetPriceCatalogDetail(Guid itemOid, Guid barcodeOid, string itemCode)
        {
            PriceCatalogDetail priceCatalogDetail = null;
            EffectivePriceCatalogPolicy EffectivePriceCatalogPolicy = _CurrentDocumentHeader.EffectivePriceCatalogPolicy();
            priceCatalogDetail = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(App.DbLayer, EffectivePriceCatalogPolicy, itemOid, itemCode, barcodeOid);
            return priceCatalogDetail;
        }

        async void addToBasket(object sender, EventArgs e)
        {
            try
            {
                if (_CurrentDocumentHeader.IsSynchronized)
                {
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    return;
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.LoadingDataPleaseWait, MaskType.Black);
                Button btn = (Button)sender;
                Guid ItemOid;
                Guid.TryParse(btn.CommandParameter.ToString(), out ItemOid);
                PriceCatalogDetail priceCatalogDetail = null;
                _CurrentSelectedItem = ObservalItems.Where(x => x.Oid == ItemOid).FirstOrDefault();
                if (_CurrentSelectedItem == null)
                {
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert(strMsgTypeAlert, strMsgFailure + "Item Not Fount", strMsgOk);
                }
                await Task.Run(() =>
                {
                    priceCatalogDetail = GetPriceCatalogDetail(_CurrentSelectedItem.Oid, _CurrentSelectedItem.BarcodeOid, _CurrentSelectedItem.Itemcode);
                });
                bool documentUsePrices = _CurrentDocumentHeader.DocumentType?.UsesPrices ?? true;
                bool documentAllowZeroPrices = _CurrentDocumentHeader.DocumentType?.AllowItemZeroPrices ?? false;
                if (priceCatalogDetail == null && documentUsePrices)
                {
                    throw new Exception(ResourcesRest.PriceCatalogDetailNotFound);
                }
                if (priceCatalogDetail.Value <= 0 && !documentAllowZeroPrices)
                {
                    throw new Exception(ResourcesRest.ItemsWithZeroPricesAreNotAllowed);
                }
                if (priceCatalogDetail != null)
                {
                    _CurrentSelectedItem.UnitPriceWithVat = priceCatalogDetail.GetUnitPriceWithVat();
                    _CurrentSelectedItem.UnitPriceWithoutVat = priceCatalogDetail.GetUnitPriceWithoutVat();
                }
                else
                {
                    _CurrentSelectedItem.UnitPriceWithVat = 0m;
                    _CurrentSelectedItem.UnitPriceWithoutVat = 0m;
                }
                PrepareQtypopup(_CurrentSelectedItem);
                UserDialogs.Instance.HideLoading();
                overlay.IsVisible = true;
                DeletePreviousQty = true;

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                });
                UserDialogs.Instance.HideLoading();
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void PrepareQtypopup(ItemPresent present)
        {

            decimal decQty = 0;
            lblSelectedItem.Text = present?.Name ?? "";
            lblUnitPriceWithoutVat.Text = ResourcesRest.UnitPriceWithoutvat + " : " + present.UnitPriceWithoutVat.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            lblUnitPriceWithVat.Text = ResourcesRest.UnitPrice + " " + ResourcesRest.WithVat + " : " + present.UnitPriceWithVat.ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"));

            if (present.Qty == 0)
            {
                Qty.Text = "1";
                decQty = 1;
            }
            else
            {
                Qty.Text = present?.Qty.ToString() ?? "1";
            }
            Qty.Text.TryParse(out decQty);
            lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + ((present?.UnitPriceWithoutVat ?? 0) * decQty).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"));
            lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + ((present?.UnitPriceWithVat ?? 0) * decQty).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"));

            if (!ItemHelper.HasPackingMeasurementunit(_CurrentDocumentHeader.DocumentType, present.PackingMeasurementUnit, present.MeasurementUnit, present.PackingMeasurementUnitRelationFactor))
            {
                present.PackingMeasurementUnitOid = present.MeasurementUnitOid;
                present.PackingMeasurementUnit = present.MeasurementUnit;
                present.PackingMeasurementUnitRelationFactor = 1;
            }

            PackingQty.Text = ItemHelper.GetPackingQuantity(decQty, present.PackingMeasurementUnitRelationFactor).ToString();
            lblUnit.Text = present.MeasurementUnit.Description;
            lblPackingUnit.Text = present.PackingMeasurementUnit?.Description ?? present.MeasurementUnit?.Description;
            FocusedEntry = eFocusedEntry.QTY;
            Qty.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        void CloseDialog(object sender, EventArgs e)
        {
            try
            {
                overlay.IsVisible = false;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        void DeleteFromList(object sender, EventArgs e)
        {
            try
            {
                ItemsToAdd.RemoveAll(x => x.ItemOid == _CurrentSelectedItem.Oid);
                _CurrentSelectedItem.Qty = 0;
                ItemPresent remove = ObservalItems.Where(x => x.Oid == _CurrentSelectedItem.Oid).FirstOrDefault();
                if (remove != null)
                {
                    ObservalItems.Remove(remove);
                }
                remove.QtyOnCurrentDocument = ItemHelper.GetCurrentDocumentItemQuantity(remove.Oid, _CurrentDocumentHeader?.DocumentDetails ?? new List<DocumentDetail>());
                remove.QtyOnTemporaryList = 0;
                ItemListView.ItemsSource = ObservalItems.ToList();
                _CurrentSelectedItem = null;
                overlay.IsVisible = false;
                fillNumberOfItems();
                Xamarin.Forms.MessagingCenter.Send<App, List<ItemDetail>>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT, ItemsToAdd);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        protected async void AddQty(object sender, EventArgs e)
        {
            try
            {
                Entry focusedEntry = new Entry();
                if (FocusedEntry == eFocusedEntry.QTY)
                {
                    DeletePreviousQty = false;
                    focusedEntry = Qty;
                }
                else if (FocusedEntry == eFocusedEntry.PACKINGQTY)
                {
                    DeletePreviousPackingQty = false;
                    focusedEntry = PackingQty;
                }

                decimal AddedQty;
                focusedEntry.Text.TryParse(out AddedQty);
                if (AddedQty + 1 > MaxQty)
                    return;

                focusedEntry.Text = (AddedQty + 1).ToString();
                decimal TotalQty = 0;
                focusedEntry.Text.TryParse(out TotalQty);
                lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + (TotalQty * (_CurrentSelectedItem?.UnitPriceWithoutVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + (TotalQty * (_CurrentSelectedItem?.UnitPriceWithVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }
        }

        protected async void RemoveQty(object sender, EventArgs e)
        {
            try
            {
                Entry focusedEntry = new Entry();
                if (FocusedEntry == eFocusedEntry.QTY)
                {
                    focusedEntry = Qty;
                }
                else if (FocusedEntry == eFocusedEntry.PACKINGQTY)
                {
                    focusedEntry = PackingQty;
                }

                double RemoveQty;
                focusedEntry.Text.TryParse(out RemoveQty);
                if (RemoveQty == 1 || RemoveQty < 0.0001)
                {
                    return;
                }
                if (RemoveQty - 1 < 0.0001)
                {
                    return;
                }
                focusedEntry.Text = (RemoveQty - 1).ToString();
                decimal TotalQty = 0;
                focusedEntry.Text.TryParse(out TotalQty);
                lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + (TotalQty * (_CurrentSelectedItem?.UnitPriceWithoutVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + (TotalQty * (_CurrentSelectedItem?.UnitPriceWithVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
            }

        }

        async void AddToTemporaryDetails(object sender, EventArgs e)
        {
            try
            {
                if (_CurrentDocumentHeader.IsSynchronized)
                {
                    await DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    return;
                }
                UserDialogs.Instance.ShowLoading(ResourcesRest.SavingDocumentsPleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    if (_CurrentSelectedItem == null)
                    {
                        overlay.IsVisible = false;
                        Qty.Text = "1";
                        return;
                    }
                    decimal decQty = 0;
                    if (!Qty.Text.TryParse(out decQty) || decQty == 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(strMsgTypeAlert, ResourcesRest.InvalidQuantity, strMsgOk);
                        });
                        return;
                    }
                    _CurrentSelectedItem.Qty = decQty;
                    if (string.IsNullOrEmpty(Qty.Text) || string.IsNullOrWhiteSpace(Qty.Text))
                    {
                        decQty = 1;
                    }
                    else
                    {
                        string stringQty = (_CurrentSelectedItem.SupportDecimal || !Qty.Text.Contains(",")) ? Qty.Text : Qty.Text.Split(",")[0];
                        stringQty.TryParse(out decQty);
                        _CurrentSelectedItem.Qty = decQty;
                    }
                    Item addItem = App.DbLayer.GetItem(_CurrentSelectedItem.Oid);
                    double availiableStock;
                    if (!ItemHelper.CheckItemStockBeforeInsertToDocument(addItem, _CurrentDocumentHeader, decQty, out availiableStock))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(strMsgTypeAlert, ResourcesRest.OutOfStock + ResourcesRest.AvailiableStock + " " + availiableStock, strMsgOk);
                        });
                        return;
                    }
                    Barcode addBarcode = App.DbLayer.GetBarcodeById(_CurrentSelectedItem.BarcodeOid);
                    MeasurementUnit mu = App.MeasurementUnits.Where(x => x.Oid == _CurrentSelectedItem.MeasurementUnitOid).FirstOrDefault();
                    if (ItemsToAdd.Where(x => x.ItemOid == _CurrentSelectedItem.Oid).Count() == 1)
                    {
                        ItemDetail itemDtl = ItemsToAdd.Where(x => x.ItemOid == _CurrentSelectedItem.Oid).FirstOrDefault();
                        itemDtl.Itemcode = _CurrentSelectedItem.Itemcode;
                        itemDtl.Barcodecode = _CurrentSelectedItem.BarcodeCode;
                        itemDtl.TotalValueWithoutVat = _CurrentSelectedItem.Qty * _CurrentSelectedItem.UnitPriceWithoutVat;
                        itemDtl.TotalValueWithVat = _CurrentSelectedItem.Qty * _CurrentSelectedItem.UnitPriceWithVat;
                        itemDtl.UnitPriceWithVat = _CurrentSelectedItem.UnitPriceWithVat;
                        itemDtl.UnitPriceWithoutVat = _CurrentSelectedItem.UnitPriceWithoutVat;
                        itemDtl.TotalQty = _CurrentSelectedItem.Qty;
                        itemDtl.Item = addItem;
                        itemDtl.Barcode = addBarcode;
                        itemDtl.MeasurementUnit = mu;
                        itemDtl.MeasurementUnitOid = mu?.Oid ?? Guid.Empty;
                        itemDtl.PackingMeasurementUnitOid = _CurrentSelectedItem.PackingMeasurementUnitOid;
                        itemDtl.PackingMeasurementUnit = _CurrentSelectedItem.PackingMeasurementUnit;
                        itemDtl.PackingMeasurementUnitRelationFactor = _CurrentSelectedItem.PackingMeasurementUnitRelationFactor;
                        itemDtl.UpdatedOnTicks = DateTime.Now.Ticks;
                    }
                    else
                    {
                        ItemsToAdd.Add(new ItemDetail()
                        {
                            ItemOid = _CurrentSelectedItem.Oid,
                            BarcodeOid = _CurrentSelectedItem.BarcodeOid,
                            TotalQty = _CurrentSelectedItem.Qty,
                            TotalValueWithoutVat = _CurrentSelectedItem.UnitPriceWithoutVat * _CurrentSelectedItem.Qty,
                            TotalValueWithVat = _CurrentSelectedItem.UnitPriceWithVat * _CurrentSelectedItem.Qty,
                            UnitPriceWithoutVat = _CurrentSelectedItem.UnitPriceWithoutVat,
                            UnitPriceWithVat = _CurrentSelectedItem.UnitPriceWithVat,
                            Barcodecode = _CurrentSelectedItem.BarcodeCode,
                            Itemcode = _CurrentSelectedItem.Itemcode,
                            Item = addItem,
                            Barcode = addBarcode,
                            MeasurementUnit = mu,
                            MeasurementUnitOid = mu?.Oid ?? Guid.Empty,
                            PackingMeasurementUnitOid = _CurrentSelectedItem.PackingMeasurementUnitOid,
                            PackingMeasurementUnit = _CurrentSelectedItem.PackingMeasurementUnit,
                            PackingMeasurementUnitRelationFactor = _CurrentSelectedItem.PackingMeasurementUnitRelationFactor,
                            UpdatedOnTicks = _CurrentDocumentHeader.UpdatedOnTicks
                        });
                    }
                    _CurrentSelectedItem.QtyOnTemporaryList = _CurrentSelectedItem.Qty;
                    _CurrentSelectedItem.QtyOnCurrentDocument = ItemHelper.GetCurrentDocumentItemQuantity(_CurrentSelectedItem.Oid, _CurrentDocumentHeader?.DocumentDetails ?? new List<DocumentDetail>());

                    RefreshItemListView();
                    Xamarin.Forms.MessagingCenter.Send<App, List<ItemDetail>>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT, ItemsToAdd);
                    Xamarin.Forms.MessagingCenter.Send<App, DocumentHeader>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_EVENT, _CurrentDocumentHeader);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ItemListView.ItemsSource = ObservalItems.OrderByDescending(x => x.UpdatedOnTicks).ToList();
                        overlay.IsVisible = false;
                        Qty.Text = "1";
                        _CurrentSelectedItem = null;
                        fillNumberOfItems();
                    });
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    _CurrentSelectedItem = null;
                    DisplayAlert(strMsgTypeAlert, strMsgFailure + ex.Message, strMsgOk);
                    UserDialogs.Instance.HideLoading();
                    overlay.IsVisible = false;
                    Qty.Text = "1";
                });
            }
            finally
            {
                Xamarin.Forms.MessagingCenter.Send<App, List<ItemDetail>>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_DETAILS_EVENT, ItemsToAdd);
                Xamarin.Forms.MessagingCenter.Send<App, DocumentHeader>((App)Xamarin.Forms.Application.Current, EventNames.UPDATE_CURRENT_DOCUMENT_EVENT, _CurrentDocumentHeader);
            }
        }

        private void fillNumberOfItems()
        {
            SearchHeader.Text = string.Format("{0} : {1}, {2} ({3}), {4} ({5}), {6}, ({7})",
                ResourcesRest.Results,
                ObservalItems.Count,
                ResourcesRest.ItemsToAdd,
                ItemsToAdd.Count(),
                ResourcesRest.OrderDocDetailLblTotal + " " + ResourcesRest.WithVat,
                ItemsToAdd.Sum(x => x.TotalValueWithVat).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")),
                ResourcesRest.OrderDocDetailLblTotal + " " + ResourcesRest.WithoutVat,
                ItemsToAdd.Sum(x => x.TotalValueWithoutVat).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR"))
                );
        }

        protected async void AddListToDocumentHeader(object sender, EventArgs e)
        {
            try
            {
                if (ItemsToAdd.Count > 0)
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SavingDocumentsPleaseWait, MaskType.Black);
                    string failMessage = string.Empty;
                    await Task.Run(async () =>
                    {
                        failMessage = await InsertTemporaryDetailsInDocumentHeader();
                    });
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ItemsToAdd.Clear();
                        fillNumberOfItems();
                        foreach (ItemPresent itm in ObservalItems.Where(x => x.Qty > 0))
                        {
                            itm.Qty = 0;
                        }
                        ObservalItems.ToList();
                        ItemListView.ItemsSource = ObservalItems;
                        RefreshItemListView();
                        if (!string.IsNullOrEmpty(failMessage))
                        {
                            DisplayAlert(strMsgTypeAlert, failMessage, strMsgOk);
                        }
                    });
                }
                else
                {
                    UserDialogs.Instance.HideLoading();
                    return;
                }
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
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                });
            }
        }

        private async Task<string> InsertTemporaryDetailsInDocumentHeader()
        {
            int curentLine = 0;
            int count = ItemsToAdd.Count();
            StringBuilder sb = new StringBuilder();
            List<string> fails = new List<string>();
            foreach (ItemDetail currentItem in ItemsToAdd)
            {
                curentLine++;
                try
                {
                    CreateDocumentDetail(currentItem);
                }
                catch (Exception ex)
                {
                    sb.Append("Product with code : " + currentItem.Itemcode + " Fail To Import with error :  " + ex.Message);
                    sb.AppendLine();
                    continue;
                }
            }
            await Task.Delay(1);
            App.DbLayer.UpdateDocumentHeader(_CurrentDocumentHeader);
            return sb.ToString();
        }

        private void CreateDocumentDetail(ItemDetail item)
        {
            if (item.Item == null)
            {
                item.Item = App.DbLayer.GetItem(item.ItemOid);
            }
            DocumentDetail documentDetail = DocumentHelper.CreateNewDocumentDetail(ref _CurrentDocumentHeader, item.Item, item.Barcode, item.BarcodeOid, item.TotalQty, App.DbLayer);
            documentDetail.Item = item.Item;
            documentDetail.Barcode = item.Barcode;
            documentDetail.BarcodeOid = item.BarcodeOid;
            DocumentHelper.AddItem(ref _CurrentDocumentHeader, documentDetail, App.DbLayer, true, true);
        }

        protected void OnScan(string code)
        {
            try
            {
                if (_CurrentDocumentHeader.IsSynchronized)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert(strMsgTypeAlert, ResourcesRest.DocumentAlreadySend, strMsgOk);
                    });
                    return;
                }
                bool showItemPopup = switchDefaultQty.IsToggled ? false : true;
                if (showItemPopup)
                {
                    UserDialogs.Instance.ShowLoading(ResourcesRest.SavingDocumentsPleaseWait, MaskType.Black);
                }
                OwnerApplicationSettings ownAppSet = App.OwnerApplicationSettings;
                string paddedCode = (ownAppSet.PadBarcodes) ? code.PadLeft(ownAppSet.BarcodeLength, ownAppSet.BarcodePaddingCharacter[0]) : code;
                string codeSearch = string.Empty;
                string nameSearch = string.Empty;
                DateTime DateInsert = DateTime.Now;
                DateTime UpdatedDate = DateTime.Now;
                bool SearchAllcategories = switchSearchAll.Checked ? true : false;
                bool IsActiveItem = switchActiveItem.Checked ? true : false;
                bool onlyItemsWithStock = switchOnlyStockItems.Checked ? true : false;
                decimal wheightedQuantity = 0;
                GetSearchCriteria(out codeSearch, out nameSearch, out DateInsert, out UpdatedDate);

                BarcodeParseResult barcodeParseResult = CustomBarcodeHelper.ParseCustomBarcode<BarcodeType>(App.BarcodeTypes,
                                                              paddedCode,
                                                              App.OwnerApplicationSettings.PadBarcodes,
                                                              App.OwnerApplicationSettings.BarcodeLength,
                                                              App.OwnerApplicationSettings.BarcodePaddingCharacter.First()
                                                              );

                if (barcodeParseResult != null && (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY || barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE))
                {
                    paddedCode = barcodeParseResult.DecodedCode;
                }
                ObservalItems.Clear();
                ItemDetail item = App.DbLayer.GetItemByScanner(SelectedCategory, paddedCode, IsActiveItem, SearchAllcategories, DateInsert, UpdatedDate, QueryType, onlyItemsWithStock, showStock);

                if (item == null)
                {
                    throw new Exception(strNoResultsFoundMessage);
                }
                PriceCatalogDetail priceCatalogDetail = null;
                priceCatalogDetail = GetPriceCatalogDetail(item.ItemOid, item.BarcodeOid, item.Itemcode);
                bool documentUsePrices = _CurrentDocumentHeader.DocumentType?.UsesPrices ?? true;
                bool documentAllowZeroPrices = _CurrentDocumentHeader.DocumentType?.UsesPrices ?? false;
                if (priceCatalogDetail == null && documentUsePrices)
                {
                    throw new Exception(ResourcesRest.PriceCatalogDetailNotFound);
                }
                if (priceCatalogDetail.Value <= 0 && !documentAllowZeroPrices)
                {
                    throw new Exception(ResourcesRest.ItemsWithZeroPricesAreNotAllowed);
                }

                decimal defaultQty = 1;
                txtDefaultQty.Text.TryParse(out defaultQty);

                if (barcodeParseResult != null && (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY || barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE))
                {
                    if (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_QUANTITY)
                    {
                        wheightedQuantity = DocumentHelper.RoundWheightValue(barcodeParseResult.Quantity);
                    }
                    if (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.ITEM_CODE_VALUE)
                    {
                        if (priceCatalogDetail != null && priceCatalogDetail.Value > 0)
                        {
                            wheightedQuantity = DocumentHelper.RoundWheightValue(barcodeParseResult.CodeValue / priceCatalogDetail.Value);
                        }
                        else
                        {
                            wheightedQuantity = defaultQty;
                        }
                    }
                    defaultQty = wheightedQuantity;
                }

                decimal TotalWithoutVat = 0;
                decimal TotalWithVat = 0;
                item.UnitPriceWithoutVat = priceCatalogDetail?.GetUnitPriceWithoutVat() ?? 0;
                item.UnitPriceWithVat = priceCatalogDetail?.GetUnitPriceWithVat() ?? 0;

                TotalWithoutVat = DocumentHelper.RoundValue((defaultQty * item.UnitPriceWithoutVat), App.OwnerApplicationSettings);
                TotalWithVat = DocumentHelper.RoundValue((defaultQty * item.UnitPriceWithVat), App.OwnerApplicationSettings);
                ItemDetail existingLine = ItemsToAdd.Where(x => x.ItemOid == item.ItemOid).FirstOrDefault();
                if (ItemsToAdd.Where(x => x.ItemOid == item.ItemOid).Count() == 1)
                {
                    if (showItemPopup)
                    {
                        item.TotalQty = existingLine.TotalQty;
                        item.TotalValueWithoutVat = existingLine.TotalValueWithoutVat;
                        item.TotalValueWithVat = existingLine.TotalValueWithVat;
                    }
                    else
                    {
                        existingLine.TotalQty = existingLine.TotalQty + defaultQty;
                        existingLine.TotalValueWithoutVat = existingLine.TotalValueWithoutVat + TotalWithoutVat;
                        existingLine.TotalValueWithVat = existingLine.TotalValueWithVat + TotalWithVat;
                        existingLine.UpdatedOnTicks = DateTime.Now.Ticks;
                        item.TotalQty = existingLine.TotalQty;
                        item.TotalValueWithoutVat = existingLine.TotalValueWithoutVat;
                        item.TotalValueWithVat = existingLine.TotalValueWithVat;
                        item.UpdatedOnTicks = DateTime.Now.Ticks;
                    }
                }
                else if (!showItemPopup)
                {
                    double availiableStock;
                    if (!ItemHelper.CheckItemStockBeforeInsertToDocument(item.Item, _CurrentDocumentHeader, defaultQty, out availiableStock))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(strMsgTypeAlert, ResourcesRest.OutOfStock + ResourcesRest.AvailiableStock + " " + availiableStock, strMsgOk);
                        });
                        return;
                    }
                    item.TotalQty = defaultQty;
                    item.TotalValueWithoutVat = TotalWithoutVat;
                    item.TotalValueWithVat = TotalWithVat;
                    item.UpdatedOnTicks = DateTime.Now.Ticks;
                    ItemsToAdd.Add(item);
                }
                UserDialogs.Instance.HideLoading();
                Device.BeginInvokeOnMainThread(() =>
                {
                    _CurrentSelectedItem = item.ConvertToItemPresent();
                    if (showItemPopup)
                    {
                        PrepareQtypopup(_CurrentSelectedItem);
                        overlay.IsVisible = true;
                        DeletePreviousQty = true;
                    }
                    if (ObservalItems.Count() > 0 && _CurrentSelectedItem != null)
                    {
                        ItemPresent remove = ObservalItems.Where(x => x.Oid == _CurrentSelectedItem.Oid).FirstOrDefault();
                        if (remove != null)
                        {
                            ObservalItems.Remove(remove);
                        }
                    }
                    ObservalItems.Add(_CurrentSelectedItem);
                    fillNumberOfItems();
                    ItemListView.ItemsSource = ObservalItems.OrderByDescending(x => x.UpdatedOnTicks);
                    _CurrentSelectedItem.QtyOnTemporaryList = ItemHelper.GetItemQuantityFromTemporaryList(_CurrentSelectedItem.Oid, ItemsToAdd);
                    _CurrentSelectedItem.QtyOnCurrentDocument = ItemHelper.GetCurrentDocumentItemQuantity(_CurrentSelectedItem.Oid, _CurrentDocumentHeader.DocumentDetails);
                    RefreshItemListView();
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

        private void RefreshItemListView()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ObservalItems == null)
                {
                    ObservalItems = new ObservableCollection<ItemPresent>();
                }
                ItemListView.ItemsSource = ObservalItems.OrderByDescending(x => x.UpdatedOnTicks);
                ItemListView.BeginRefresh();
                ItemListView.IsRefreshing = true;
                ItemListView.EndRefresh();
                ItemListView.IsRefreshing = false;
            });
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
                FilterPanelStack.IsVisible = false;
                PageGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                PageGrid.ColumnDefinitions[1].Width = new GridLength(100, GridUnitType.Star);
            }
        }
        private async Task NavigateToProductDetails(Guid SelectedItemOid)
        {
            App.SkipAsking = true;
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
                Guid SelectedItemOid = ObservalItems.Where(x => x.Oid.Equals(item.CommandParameter)).FirstOrDefault()?.Oid ?? Guid.Empty;
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
            BarcodeInput.Text = string.Empty;
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
            DescriptionInput.Text = string.Empty;
            BarcodeInput.Text = string.Empty;
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnBarcodeInputUnFocused(object sender, EventArgs e)
        {
            SearchBar item = (SearchBar)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void QtyFocused(object sender, EventArgs e)
        {
            DeletePreviousQty = true;
            FocusedEntry = eFocusedEntry.QTY;
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
            PackingQty.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void QtyUnFocused(object sender, EventArgs e)
        {
            //Entry item = (Entry)sender;
            //item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }


        protected void PackingQtyFocused(object sender, EventArgs e)
        {
            DeletePreviousPackingQty = true;
            FocusedEntry = eFocusedEntry.PACKINGQTY;
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
            Qty.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void PackingQtyUnFocused(object sender, EventArgs e)
        {
            //Entry item = (Entry)sender;
            //item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void PackingQtyChanged(object sender, EventArgs e)
        {
            if (FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                if (PackingQty.Text.TryParse(out decimal val) && _CurrentSelectedItem != null)
                {
                    Qty.Text = ItemHelper.GetQuantityFromPacking(val, _CurrentSelectedItem.PackingMeasurementUnitRelationFactor).ToString();
                }
                else
                {
                    Qty.Text = "";
                }
            }
        }

        protected void QtyChanged(object sender, EventArgs e)
        {
            if (FocusedEntry == eFocusedEntry.QTY)
            {
                if (Qty.Text.TryParse(out decimal val) && _CurrentSelectedItem != null)
                {
                    PackingQty.Text = ItemHelper.GetPackingQuantity(val, _CurrentSelectedItem.PackingMeasurementUnitRelationFactor).ToString();
                }
                else
                {
                    PackingQty.Text = "";
                }
            }
        }


        protected void OnDefaultQtyFocused(object sender, EventArgs e)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        protected void OnDefaultQtyUnFocused(object sender, EventArgs e)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        protected void OnNumber1Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(1.ToString());
        }
        protected void OnNumber2Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(2.ToString());
        }
        protected void OnNumber3Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(3.ToString());
        }
        protected void OnNumber4Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(4.ToString());
        }
        protected void OnNumber5Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(5.ToString());
        }
        protected void OnNumber6Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(6.ToString());
        }
        protected void OnNumber7Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(7.ToString());
        }
        protected void OnNumber8Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(8.ToString());
        }
        protected void OnNumber9Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(9.ToString());
        }
        protected void OnNumber0Clicked(object sender, EventArgs e)
        {
            if (DeletePreviousQty && FocusedEntry == eFocusedEntry.QTY)
            {
                Qty.Text = "";
                DeletePreviousQty = false;
            }
            if (DeletePreviousPackingQty && FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                PackingQty.Text = "";
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels(0.ToString());
        }

        protected void OnDelClicked(object sender, EventArgs e)
        {
            if (FocusedEntry == eFocusedEntry.QTY)
            {
                DeletePreviousQty = false;
            }
            if (FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                DeletePreviousPackingQty = false;
            }
            UpdatePopupLabels("DEL");
        }
        protected void OnCommaClicked(object sender, EventArgs e)
        {
            if (FocusedEntry == eFocusedEntry.QTY)
            {
                DeletePreviousQty = false;
            }
            if (FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                DeletePreviousPackingQty = false;
            }

            UpdatePopupLabels(",");
        }

        protected void DefaultQtyToogled(object sender, EventArgs args)
        {
            if (switchDefaultQty.IsToggled)
            {
                txtDefaultQty.IsEnabled = true;
                txtDefaultQty.Text = string.IsNullOrWhiteSpace(txtDefaultQty.Text) ? "1" : txtDefaultQty.Text;
            }
            else
            {
                txtDefaultQty.Text = "";
                txtDefaultQty.IsEnabled = false;
            }
        }

        protected void DefaultQtyChanged(object sender, EventArgs args)
        {
            try
            {
                lock (locker)
                {
                    string res = txtDefaultQty.Text;
                    if (res.Contains("."))
                    {
                        string[] arr = txtDefaultQty.Text.Split(".");
                        for (int i = 0; i < arr.Length; i++)
                        {
                            res = res + arr[i];
                        }
                    }
                    if (res.Contains(","))
                    {
                        string[] arr = res.Split(",");
                        for (int i = 0; i < arr.Length; i++)
                        {
                            res = res + arr[i];
                        }
                    }
                    txtDefaultQty.Text = res;
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }


        private void UpdatePopupLabels(string buttonNumberPressed)
        {
            Entry focusedEntry = new Entry();
            string QtyStr = string.Empty;
            if (FocusedEntry == eFocusedEntry.QTY)
            {
                focusedEntry = Qty;
                QtyStr = Qty.Text;
            }
            else if (FocusedEntry == eFocusedEntry.PACKINGQTY)
            {
                focusedEntry = PackingQty;
                QtyStr = PackingQty.Text;
            }
            if (focusedEntry == null)
            {
                focusedEntry = Qty;
            }

            try
            {
                if (buttonNumberPressed.ToUpper() != "DEL")
                {
                    if (!string.IsNullOrWhiteSpace(QtyStr) && QtyStr.Contains(","))
                    {
                        var array = QtyStr.Split(",");
                        if (array[1].Length > 4)
                            return;
                    }
                }

                if (buttonNumberPressed == ",")
                {
                    if (_CurrentSelectedItem != null)
                        if (!QtyStr.Contains(","))
                        {
                            MeasurementUnit mu = FocusedEntry == eFocusedEntry.QTY ? _CurrentSelectedItem.MeasurementUnit : _CurrentSelectedItem.PackingMeasurementUnit ?? _CurrentSelectedItem.MeasurementUnit;
                            if (mu.SupportDecimal)
                                QtyStr = QtyStr + ",";
                        }
                }
                else if (buttonNumberPressed.ToUpper() == "DEL")
                {
                    if (!string.IsNullOrWhiteSpace(QtyStr) && QtyStr.Length > 0)
                        QtyStr = QtyStr.Substring(0, focusedEntry.Text.Length - 1);
                }
                else
                {
                    QtyStr = QtyStr + buttonNumberPressed;
                }
                decimal totalQty;

                if (QtyStr.TryParse(out totalQty))
                {
                    if (totalQty > MaxQty)
                    {
                        QtyStr = MaxQty.ToString();
                        lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + (MaxQty * (_CurrentSelectedItem?.UnitPriceWithoutVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                        lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + (MaxQty * (_CurrentSelectedItem?.UnitPriceWithVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                    }
                    else
                    {
                        lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + (totalQty * (_CurrentSelectedItem?.UnitPriceWithoutVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                        lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + (totalQty * (_CurrentSelectedItem?.UnitPriceWithVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                    }
                }
                if (string.IsNullOrWhiteSpace(QtyStr))
                {
                    lblTotalPriceWithoutVat.Text = ResourcesRest.TotalPriceWithoutVat + " : " + (0 * (_CurrentSelectedItem?.UnitPriceWithoutVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                    lblTotalPriceWithVat.Text = ResourcesRest.TotalPriceWithVat + " : " + (0 * (_CurrentSelectedItem?.UnitPriceWithVat ?? 0)).ToString("C" + App.OwnerApplicationSettings?.DisplayDigits ?? "2", new CultureInfo("el-GR")) ?? "";
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            focusedEntry.Text = QtyStr;
        }

        #endregion
        ///-----END-------------///



        private void InitiallizeControllers(DocumentHeader header)
        {
            ResourcesRest.Culture = ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
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
            lblQty.Text = ResourcesRest.Qty;
            lblPackingQty.Text = ResourcesRest.PackingQty;
            btnAddBasket.Text = ResourcesRest.Add;
            btnDeleteFromList.Text = ResourcesRest.DeleteFromList;
            btnClose.Text = ResourcesRest.Close;
            lblSearchAll.Text = ResourcesRest.SearchAllCategories;
            lblDefaultQy.Text = ResourcesRest.DefaultQty;
            lblOnlyStockItems.Text = ResourcesRest.OnlyItemsWithStock;
            txtDefaultQty.IsEnabled = switchDefaultQty.IsToggled;
            lblDescription.Text = ResourcesRest.Description;
            lblCode.Text = ResourcesRest.Code;
            lblOrderQty.Text = ResourcesRest.orderQTY;
            lblStock.Text = ResourcesRest.Stock;
            switchActiveItem.TextColor = Color.WhiteSmoke;
            switchSearchAll.TextColor = Color.WhiteSmoke;
            switchOnlyStockItems.TextColor = Color.WhiteSmoke;
            switcDateFromActive.TextColor = Color.WhiteSmoke;
            switchUpdatedActive.TextColor = Color.WhiteSmoke;

            if (header.DocumentType == null)
            {
                header.DocumentType = App.DbLayer.GetDocumentTypeById(header.DocumentTypeOid);
            }
            showStock = header.DocumentType.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS && header.DocumentType.QuantityFactor > 0 && !header.IsSynchronized ? true : false;
            switchOnlyStockItems.Checked = showStock;

        }

    }


    public enum eFocusedEntry
    {
        NONE,
        QTY,
        PACKINGQTY
    }
}
