using Acr.UserDialogs;
using ITS.WRM.SFA.Droid.Classes;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Droid.Pages.DocumentChildViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentDetailPage : ContentPage
    {
        private DocumentDetail documentDetail;
        private DocumentDetail oldDocumentDetail = new DocumentDetail();
        private decimal _LastCustomPrice = 0;
        private bool _HasCustomPrice = false;
        private DocumentDetailDiscount _CustomDisount = null;
        private static List<char> Numeric = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };
        bool _SupportDecimal = false;
        bool _PackingSupportDecimal = false;
        private static object locker = new object();
        private DocumentHeader _CurrentDocument;
        private bool isForWholeSale = false;
        private eFocusedEntry FocusedEntry = eFocusedEntry.NONE;

        public DocumentDetailPage(ref DocumentHeader document, DocumentDetail selectedDocumentDetail)
        {
            InitializeComponent();
            _CurrentDocument = document;
            if (_CurrentDocument.DocumentType == null)
            {
                _CurrentDocument.DocumentType = App.DbLayer.GetDocumentTypeById(documentDetail.DocumentHeader.DocumentTypeOid);
            }
            isForWholeSale = _CurrentDocument.DocumentType.IsForWholesale;
            oldDocumentDetail = selectedDocumentDetail;
            documentDetail = selectedDocumentDetail;
            _HasCustomPrice = selectedDocumentDetail.HasCustomPrice;
            _LastCustomPrice = isForWholeSale ? selectedDocumentDetail.PriceListUnitPriceWithoutVAT : selectedDocumentDetail.PriceListUnitPriceWithVAT;
            this.documentDetail.DocumentDetailDiscounts = App.DbLayer.GetDocumentDetailDiscounts(this.documentDetail.Oid) ?? new List<DocumentDetailDiscount>();
            _CustomDisount = selectedDocumentDetail.DocumentDetailDiscounts.Count > 0 ? selectedDocumentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM).FirstOrDefault()
                                                                                                                                                                : new DocumentDetailDiscount();



            if (documentDetail.MeasurementUnit == null && documentDetail.MeasurementUnitOid != null && documentDetail.MeasurementUnitOid != Guid.Empty)
            {
                documentDetail.MeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == documentDetail.MeasurementUnitOid).FirstOrDefault();
            }
            if (documentDetail.PackingMeasurementUnit == null && documentDetail.PackingMeasurementUnitOid != null && documentDetail.PackingMeasurementUnitOid != Guid.Empty)
            {
                documentDetail.PackingMeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == documentDetail.PackingMeasurementUnitOid).FirstOrDefault();
            }
            if (documentDetail.PackingMeasurementUnit == null && documentDetail.MeasurementUnit != null)
            {
                documentDetail.PackingMeasurementUnit = documentDetail.MeasurementUnit;
            }
            if (!ItemHelper.HasPackingMeasurementunit(_CurrentDocument.DocumentType, documentDetail.PackingMeasurementUnit, documentDetail.MeasurementUnit, documentDetail.PackingMeasurementUnitRelationFactor))
            {
                documentDetail.PackingMeasurementUnit = documentDetail.MeasurementUnit;
                documentDetail.PackingMeasurementUnitOid = documentDetail.MeasurementUnitOid;
                documentDetail.PackingMeasurementUnitRelationFactor = 1;
            }

            _SupportDecimal = documentDetail.MeasurementUnit?.SupportDecimal ?? false;
            _PackingSupportDecimal = documentDetail.PackingMeasurementUnit?.SupportDecimal ?? false;

            if (documentDetail.PackingMeasurementUnitRelationFactor == 0)
            {
                documentDetail.PackingMeasurementUnitRelationFactor = 1;
            }

            InitiallizeControllers();

            txtLineDiscountMoneyValue.Focused += (sender, e) =>
            {
                txtLineDiscountPercentValue.Text = 0.ToString("P2", new CultureInfo("el-GR")); ;
                txtLineDiscountMoneyValue.Text = "";
            };
            txtLineDiscountPercentValue.Focused += (sender, e) =>
            {
                txtLineDiscountMoneyValue.Text = 0.ToString("C2", new CultureInfo("el-GR"));
                txtLineDiscountPercentValue.Text = "";
            };
            txtLineDiscountMoneyValue.Unfocused += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtLineDiscountMoneyValue.Text))
                {
                    txtLineDiscountMoneyValue.Text = _CustomDisount.Value.ToString("C2", new CultureInfo("el-GR"));
                }
            };
            txtLineDiscountPercentValue.Unfocused += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtLineDiscountPercentValue.Text))
                {
                    txtLineDiscountPercentValue.Text = (_CustomDisount.Percentage).ToString("P2", new CultureInfo("el-GR"));
                }
            };

            if (!isForWholeSale)
            {
                txtPrice.Focused += (sender, e) =>
                {
                    txtPrice.Text = "";
                };
            }
            else
            {
                txtNetPrice.Focused += (sender, e) =>
                {
                    txtNetPrice.Text = "";
                };
            }
            if (!isForWholeSale)
            {
                txtPrice.Unfocused += (sender, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtPrice.Text))
                    {
                        txtPrice.Text = _LastCustomPrice.ToString("C2", new CultureInfo("el-GR"));
                    }
                };
            }
            else
            {
                txtNetPrice.Unfocused += (sender, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtNetPrice.Text))
                    {
                        txtNetPrice.Text = _LastCustomPrice.ToString("C2", new CultureInfo("el-GR"));
                    }
                };
            }
        }

        private async void InitiallizeControllers()
        {
            try
            {
                ResourcesRest.Culture = App.Languageinfo(App.SFASettings.Language ?? "en-GB");
                lblCode.Text = ResourcesRest.OrderDocDetailLblCode;
                lblBarccode.Text = ResourcesRest.OrderDocDetailBarcode;
                lblDescription.Text = ResourcesRest.OrdrerDocDetailDescription;
                lblQty.Text = ResourcesRest.OrderDocDetailLblQty + "(" + documentDetail.MeasurementUnit?.Description + ")";
                lblPacingQty.Text = ResourcesRest.PackingQty + "(" + documentDetail.PackingMeasurementUnit?.Description + ")";
                lblMeasurement.Text = ResourcesRest.OrderDocDetailLblMeasurement;
                lblVatCategory.Text = ResourcesRest.OrderDocDetailLblVatCategory;
                lblPrice.Text = ResourcesRest.UnitPrice + " " + ResourcesRest.WithVat;
                lblNetPrice.Text = ResourcesRest.UnitPriceWithoutvat;
                LblNetTotal.Text = ResourcesRest.OrderDocDetailsLblNetTotal;
                LblPriceCatalogDiscountPercent.Text = ResourcesRest.OrderDocDetailsLblPriceDiscount;
                LblPriceCatalogDiscountMoney.Text = ResourcesRest.OrderDocDetailLblPriceDiscountMoney;
                LblLineDiscountPercent.Text = ResourcesRest.OrderDocDetailLblLineDiscount + (isForWholeSale ? " " + ResourcesRest.WithoutVat : "");
                LblLineDiscountMoney.Text = ResourcesRest.OrderDocDetailLblLineDiscountMoney + (isForWholeSale ? " " + ResourcesRest.WithoutVat : "");
                LblTotalDiscountMoney.Text = ResourcesRest.OrderDocDetailLblTotalDiscountMoney + " " + ResourcesRest.WithVat;
                LblTotalAfterDiscount.Text = ResourcesRest.OrderDocDetaulLblTotalAfterDiscount;
                LblTotalVATAmount.Text = ResourcesRest.OrderDocDetailLblTotalVat;
                LblTotal.Text = ResourcesRest.OrderDocDetailLblTotal;
                LblRemarks.Text = ResourcesRest.OrderDocDetailLblRemarks;
                btnCancelLine.Text = ResourcesRest.Cancel;
                btnAddLine.Text = ResourcesRest.Save;
                btnRemovLine.Text = ResourcesRest.RemoveLine;
                btnRecalculate.Text = ResourcesRest.Recaltulate;
                btnSaveAndClose.Text = ResourcesRest.SaveAndClose;

                if (_CurrentDocument.DocumentType == null)
                {
                    _CurrentDocument.DocumentType = App.DbLayer.GetDocumentTypeById(documentDetail.DocumentHeader.DocumentTypeOid);
                }

                if (!_CurrentDocument.DocumentType.AllowItemValueEdit)
                {
                    txtNetPrice.IsEnabled = false;
                    txtPrice.IsEnabled = false;
                    txtNetPrice.BackgroundColor = Color.FromHex("#dde1e2");
                    txtPrice.BackgroundColor = Color.FromHex("#dde1e2");

                }
                else
                {
                    if (isForWholeSale)
                    {
                        txtNetPrice.IsEnabled = true;
                        txtPrice.IsEnabled = false;
                        txtPrice.BackgroundColor = Color.FromHex("#dde1e2");
                    }
                    else
                    {
                        txtPrice.IsEnabled = true;
                        txtNetPrice.IsEnabled = false;
                        txtNetPrice.BackgroundColor = Color.FromHex("#dde1e2");
                    }
                }

                await RecalculateDocumentDetail(true);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        private async void FillData(DocumentDetail documentDetail)
        {
            try
            {

                //OwnerApplicationSettings applicationSettings = App.OwnerApplicationSettings;
                double dblZeroValue = 0;
                if (_CurrentDocument.DocumentType == null)
                {
                    _CurrentDocument.DocumentType = App.DbLayer.GetDocumentTypeById(documentDetail.DocumentHeader.DocumentTypeOid);
                }

                lblCodeValue.Text = documentDetail.Item.Code;
                lblBarcodeValue.Text = documentDetail.BarcodeCode;
                lblDescriptionValue.Text = documentDetail.Item.Name;
                txtQtyValue.Text = documentDetail.Qty.ToString();
                txtPackingQtyValue.Text = ItemHelper.GetPackingQuantity(documentDetail.Qty, documentDetail.PackingMeasurementUnitRelationFactor).ToString();
                if (documentDetail.MeasurementUnit == null)
                {
                    documentDetail.MeasurementUnit = App.MeasurementUnits.Where(x => x.Oid == documentDetail.MeasurementUnitOid).FirstOrDefault();
                }
                lblMeasurementValue.Text = documentDetail.MeasurementUnit.Description;
                lblVatCategoryValue.Text = (documentDetail.VatFactor * 100).ToString() + "%";
                txtNetPrice.Text = documentDetail.PriceListUnitPriceWithoutVAT.ToString("C2", new CultureInfo("el-GR"));
                txtPrice.Text = documentDetail.PriceListUnitPriceWithVAT.ToString("C2", new CultureInfo("el-GR"));
                LblNetTotalValue.Text = isForWholeSale ? documentDetail.NetTotalBeforeDiscount.ToString("C2", new CultureInfo("el-GR"))
                                                       : documentDetail.GrossTotalBeforeDiscount.ToString("C2", new CultureInfo("el-GR"));

                DocumentDetailDiscount PriceCatalogdiscount = documentDetail.DocumentDetailDiscounts.FirstOrDefault(discount => discount.DiscountSource == eDiscountSource.PRICE_CATALOG);
                DocumentDetailDiscount customDiscount = _CustomDisount;
                LblPriceCatalogDiscountPercentValue.Text = PriceCatalogdiscount == null ? dblZeroValue.ToString() : PriceCatalogdiscount.Percentage.ToString("P2", new CultureInfo("el-GR"));
                txtLineDiscountPercentValue.Text = _CustomDisount == null ? dblZeroValue.ToString() : (_CustomDisount.Percentage).ToString("P2", new CultureInfo("el-GR"));
                if (isForWholeSale)
                {

                    LblPriceCatalogDiscountMoneyValue.Text = PriceCatalogdiscount == null ? dblZeroValue.ToString("C2", new CultureInfo("el-GR")) :
                                                                                                                    PriceCatalogdiscount.DiscountWithoutVAT.ToString("C2", new CultureInfo("el-GR"));
                    txtLineDiscountMoneyValue.Text = _CustomDisount == null ? dblZeroValue.ToString("C2", new CultureInfo("el-GR")) : _CustomDisount.DiscountWithoutVAT.ToString("C2", new CultureInfo("el-GR"));
                }
                else
                {

                    LblPriceCatalogDiscountMoneyValue.Text = PriceCatalogdiscount == null ? dblZeroValue.ToString("C2", new CultureInfo("el-GR")) :
                                                                                                                    PriceCatalogdiscount.DiscountWithVAT.ToString("C2", new CultureInfo("el-GR"));


                    txtLineDiscountMoneyValue.Text = _CustomDisount == null ? dblZeroValue.ToString("C2", new CultureInfo("el-GR")) : _CustomDisount.DiscountWithVAT.ToString("C2", new CultureInfo("el-GR"));
                }




                LblTotalDiscountMoneyValue.Text = documentDetail.TotalDiscountIncludingVAT.ToString("C2", new CultureInfo("el-GR"));
                LblTotalAfterDiscountValue.Text = documentDetail.NetTotal.ToString("C2", new CultureInfo("el-GR"));
                LblTotalVATAmountValue.Text = documentDetail.TotalVatAmount.ToString("C2", new CultureInfo("el-GR"));
                LblTotalValue.Text = documentDetail.GrossTotal.ToString("C2", new CultureInfo("el-GR"));
                LblRemarksValue.Text = documentDetail.Remarks;
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }


        async Task RecalculateDocumentDetail(bool showingFirstTime = false)
        {
            try
            {
                if (this.documentDetail.Item == null && documentDetail.ItemOid != Guid.Empty)
                {
                    this.documentDetail.Item = App.DbLayer.GetItem(documentDetail.ItemOid);
                }
                try
                {
                    if (this.documentDetail.Barcode == null && documentDetail.BarcodeOid != Guid.Empty)
                    {
                        this.documentDetail.Barcode = App.DbLayer.GetBarcodeById(documentDetail.BarcodeOid);
                    }
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
                if (string.IsNullOrEmpty(documentDetail.BarcodeCode))
                {
                    documentDetail.BarcodeCode = documentDetail.ItemCode;
                }
                if (this.documentDetail.DocumentHeader == null)
                {
                    this.documentDetail.DocumentHeader = _CurrentDocument;
                }
                this.documentDetail.DocumentDetailDiscounts = App.DbLayer.GetDocumentDetailDiscounts(this.documentDetail.Oid) ?? new List<DocumentDetailDiscount>();
                decimal Qty = documentDetail.Qty;
                if (!showingFirstTime)
                {
                    if (string.IsNullOrEmpty(txtQtyValue.Text))
                    {
                        Qty = 1;
                    }
                    else
                    {
                        if (_SupportDecimal)
                        {
                            txtQtyValue.Text.TryParse(out Qty);
                        }
                        else
                        {
                            string qtyStr = txtQtyValue.Text.Replace(".", "");
                            qtyStr = qtyStr.Replace(",", "");
                            qtyStr.TryParse(out Qty);
                        }
                    }
                    double availiableStock;
                    if (!ItemHelper.CheckItemStockOnUpdateToDocument(documentDetail.Item, documentDetail.DocumentHeader, Qty, out availiableStock))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.OutOfStock + ResourcesRest.AvailiableStock + " " + availiableStock, ResourcesRest.MsgBtnOk);
                        });
                        return;
                    }
                    txtQtyValue.Text = Qty.ToString();
                    decimal DiscountValue = 0;
                    decimal DiscountPercentage = 0;
                    DocumentDetailDiscount customDiscount = documentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM).FirstOrDefault();
                    if (customDiscount != null)
                    {
                        this.documentDetail.DocumentDetailDiscounts.Remove(customDiscount);
                        App.DbLayer.DeleteObj<DocumentDetailDiscount>(customDiscount);
                    }
                    string monedisc = txtLineDiscountMoneyValue.Text;
                    monedisc = monedisc.Trim('€');
                    string percdisc = txtLineDiscountPercentValue.Text;
                    percdisc = percdisc.Trim('%');

                    monedisc.TryParse(out DiscountValue);
                    percdisc.TryParse(out DiscountPercentage);
                    DocumentDetailDiscount newDiscount = null;
                    if (DiscountPercentage != 0)
                    {
                        if (DiscountPercentage > 100)
                            DiscountPercentage = 100;
                        if (DiscountPercentage < 0)
                            DiscountPercentage = 0;
                        if (DiscountPercentage != 0)
                        {
                            newDiscount = new DocumentDetailDiscount();
                            newDiscount.DocumentDetail = documentDetail;
                            newDiscount.DocumentDetailOid = documentDetail.Oid;
                            newDiscount.DiscountSource = eDiscountSource.CUSTOM;
                            newDiscount.DiscountType = eDiscountType.PERCENTAGE;
                            newDiscount.Percentage = DiscountPercentage / 100;
                            this.documentDetail.DocumentDetailDiscounts.Add(newDiscount);
                        }
                    }
                    else if (DiscountValue != 0)
                    {
                        DocumentDetailDiscount cstmDisc = documentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource == eDiscountSource.CUSTOM).FirstOrDefault();
                        decimal customDiscountvalue = 0;
                        if (cstmDisc != null)
                        {
                            customDiscountvalue = _CurrentDocument.DocumentType.IsForWholesale ? cstmDisc.DiscountWithoutVAT : cstmDisc.DiscountWithVAT;
                        }
                        decimal maxDiscount = _CurrentDocument.DocumentType.IsForWholesale ? documentDetail.NetTotal + customDiscountvalue : documentDetail.GrossTotal + customDiscountvalue;
                        if (DiscountValue > maxDiscount)
                            DiscountValue = maxDiscount;
                        newDiscount = new DocumentDetailDiscount();
                        newDiscount.DocumentDetailOid = documentDetail.Oid;
                        newDiscount.DiscountSource = eDiscountSource.CUSTOM;
                        newDiscount.DiscountType = eDiscountType.VALUE;
                        newDiscount.Value = DiscountValue;
                        this.documentDetail.DocumentDetailDiscounts.Add(newDiscount);
                    }
                }
                if (!_HasCustomPrice)
                {
                    _HasCustomPrice = false;
                    _LastCustomPrice = isForWholeSale ? documentDetail.PriceListUnitPriceWithoutVAT : documentDetail.PriceListUnitPriceWithVAT;
                }

                DocumentDetail oldDocumentLine = this.documentDetail;
                documentDetail = DocumentHelper.ComputeDocumentLine(ref _CurrentDocument, App.DbLayer, documentDetail.Item, documentDetail.Barcode, Qty, false, _LastCustomPrice, _HasCustomPrice,
                 documentDetail.Item.Name, documentDetail.DocumentDetailDiscounts ?? new List<DocumentDetailDiscount>(), false, "", oldDocumentLine, null);
                App.DbLayer.InsertOrReplaceObj<DocumentDetail>(documentDetail);
                App.DbLayer.UpdateDocumentHeader(_CurrentDocument);
                documentDetail.DocumentDetailDiscounts = App.DbLayer.GetDocumentDetailDiscounts(this.documentDetail.Oid) ?? new List<DocumentDetailDiscount>();
                _CustomDisount = App.DbLayer.GetDocumentDetailDiscounts(this.documentDetail.Oid)?.Where(x => x.DiscountSource == eDiscountSource.CUSTOM).FirstOrDefault() ?? new DocumentDetailDiscount();
                Device.BeginInvokeOnMainThread(() =>
                {
                    FillData(documentDetail);
                });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        protected async void AddLineItem(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.CalculatePleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    await RecalculateDocumentDetail();
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }
        protected async void RemoveLineItem(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.CalculatePleaseWait, MaskType.Black);
                await Task.Run(() =>
                {
                    DocumentHelper.DeleteItem(ref _CurrentDocument, documentDetail, App.DbLayer);
                    DocumentHelper.RecalculateDocumentCosts(ref _CurrentDocument, App.DbLayer, true, false);
                    App.DbLayer.UpdateDocumentHeader(_CurrentDocument);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopModalAsync(true);
                        UserDialogs.Instance.HideLoading();
                    });
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }
        protected async void RecalculateItem(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.CalculatePleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    await RecalculateDocumentDetail();
                });
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.HideLoading();
                });
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        protected async void SaveAndClose(Object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading(ResourcesRest.CalculatePleaseWait, MaskType.Black);
                await Task.Run(async () =>
                {
                    await RecalculateDocumentDetail();
                });
                UserDialogs.Instance.HideLoading();
                await Navigation.PopModalAsync(true);

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                    UserDialogs.Instance.HideLoading();
                });
            }
        }
        async void CancelLineItem(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopModalAsync(true);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        public void OnPriceChange(object sender, EventArgs args)
        {
            try
            {
                lock (locker)
                {
                    string price = isForWholeSale ? txtNetPrice.Text : txtPrice.Text;
                    price = price.Trim('€');
                    decimal decprice = 0;
                    if (price.TryParse(out decprice))
                    {
                        if (decprice != 0 && decprice != documentDetail.PriceListUnitPriceWithVAT)
                        {
                            _HasCustomPrice = true;
                            _LastCustomPrice = decprice;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        public void OnNetPriceChange(object sender, EventArgs args)
        {
            try
            {
                lock (locker)
                {
                    string price = isForWholeSale ? txtNetPrice.Text : txtPrice.Text;
                    price = price.Trim('€');
                    decimal decprice = 0;
                    if (price.TryParse(out decprice))
                    {
                        if (decprice != 0 && decprice != documentDetail.PriceListUnitPriceWithVAT)
                        {
                            _HasCustomPrice = true;
                            _LastCustomPrice = decprice;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }


        public void OnDiscountMoneyValueChange(object sender, EventArgs args)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLineDiscountMoneyValue.Text))
                {
                    return;
                }
                lock (locker)
                {
                    string monedisc = txtLineDiscountMoneyValue.Text;
                    monedisc = monedisc.Trim('€');
                    decimal value = 0;
                    if (monedisc.TryParse(out value))
                    {
                        _CustomDisount.Value = value;
                        _CustomDisount.Percentage = 0;
                        _CustomDisount.DiscountSource = eDiscountSource.CUSTOM;
                        _CustomDisount.DiscountType = eDiscountType.VALUE;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }
        public void OnDiscounPercentValueChange(object sender, EventArgs args)
        {
            try
            {
                if (string.IsNullOrEmpty(txtLineDiscountPercentValue.Text))
                {
                    return;
                }
                lock (locker)
                {
                    string percdisc = txtLineDiscountPercentValue.Text;
                    percdisc = percdisc.Trim('%');
                    decimal percentage = 0;
                    if (percdisc.TryParse(out percentage))
                    {
                        _CustomDisount.Percentage = percentage / 100;
                        _CustomDisount.Value = 0;
                        _CustomDisount.DiscountSource = eDiscountSource.CUSTOM;
                        _CustomDisount.DiscountType = eDiscountType.PERCENTAGE;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        protected async void AddQty(object sender, EventArgs e)
        {
            try
            {
                FocusedEntry = eFocusedEntry.QTY;
                double AddedQty;
                txtQtyValue.Text.TryParse(out AddedQty);
                txtQtyValue.Text = (AddedQty + 1).ToString();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }
        protected async void RemoveQty(object sender, EventArgs e)
        {
            try
            {
                FocusedEntry = eFocusedEntry.QTY;
                double RemoveQty;
                double.TryParse(txtQtyValue.Text, out RemoveQty);
                if (RemoveQty == 1)
                {
                    return;
                }
                txtQtyValue.Text = (RemoveQty - 1).ToString();
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
            }
        }

        public void OnQtyChanged(object sender, EventArgs args)
        {
            try
            {

                if (FocusedEntry == eFocusedEntry.QTY)
                {
                    if (string.IsNullOrEmpty(txtQtyValue.Text))
                    {
                        return;
                    }
                    lock (locker)
                    {
                        string qtyStr = txtQtyValue.Text;
                        if (qtyStr.Length > 0)
                        {
                            char lastChar = qtyStr.Last();
                            if (!Numeric.Contains(lastChar))
                            {
                                txtQtyValue.Text = qtyStr.Replace(lastChar.ToString(), "");
                            }
                            if (!_SupportDecimal)
                            {
                                if (lastChar == '.')
                                {
                                    txtQtyValue.Text = qtyStr.Replace(".", "");
                                }
                                if (lastChar == ',')
                                {
                                    txtQtyValue.Text = qtyStr.Replace(",", "");
                                }
                            }
                        }

                        if (txtQtyValue.Text.TryParse(out decimal val))
                        {
                            txtPackingQtyValue.Text = ItemHelper.GetPackingQuantity(val, documentDetail.PackingMeasurementUnitRelationFactor).ToString();
                        }
                        else
                        {
                            txtPackingQtyValue.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }


        public async void RemovePackingQty(object sender, EventArgs args)
        {
            try
            {
                try
                {
                    FocusedEntry = eFocusedEntry.PACKINGQTY;
                    double RemoveQty;
                    double.TryParse(txtPackingQtyValue.Text, out RemoveQty);
                    if (RemoveQty == 1)
                    {
                        return;
                    }
                    txtPackingQtyValue.Text = (RemoveQty - 1).ToString();
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }

        public async void AddPackingQty(object sender, EventArgs args)
        {
            try
            {
                try
                {
                    FocusedEntry = eFocusedEntry.PACKINGQTY;
                    double AddedQty;
                    txtPackingQtyValue.Text.TryParse(out AddedQty);
                    txtPackingQtyValue.Text = (AddedQty + 1).ToString();
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    await DisplayAlert(ResourcesRest.MsgTypeAlert, ResourcesRest.sendExceptionMsg + ex.Message, ResourcesRest.MsgBtnOk);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }


        public void OnPackingQtyChanged(object sender, EventArgs args)
        {
            try
            {
                if (FocusedEntry == eFocusedEntry.PACKINGQTY)
                {
                    if (string.IsNullOrEmpty(txtPackingQtyValue.Text))
                    {
                        return;
                    }
                    lock (locker)
                    {
                        string qtyStr = txtPackingQtyValue.Text;
                        if (qtyStr.Length > 0)
                        {
                            char lastChar = qtyStr.Last();
                            if (!Numeric.Contains(lastChar))
                            {
                                txtPackingQtyValue.Text = qtyStr.Replace(lastChar.ToString(), "");
                            }
                            if (!_PackingSupportDecimal)
                            {
                                if (lastChar == '.')
                                {
                                    txtPackingQtyValue.Text = qtyStr.Replace(".", "");
                                }
                                if (lastChar == ',')
                                {
                                    txtPackingQtyValue.Text = qtyStr.Replace(",", "");
                                }
                            }
                        }

                        if (txtPackingQtyValue.Text.TryParse(out decimal val))
                        {
                            txtQtyValue.Text = ItemHelper.GetQuantityFromPacking(val, documentDetail.PackingMeasurementUnitRelationFactor).ToString();
                        }
                        else
                        {
                            txtQtyValue.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
        }



        public void RemarksFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void RemarksUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void DiscountMoneyValueFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void DiscountMoneyValueUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void DiscountPercentValueFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void DiscountPercentValueUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void QtyValueFocused(object sender, EventArgs args)
        {
            FocusedEntry = eFocusedEntry.QTY;
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void QtyValueUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void PackingQtyValueFocused(object sender, EventArgs args)
        {
            FocusedEntry = eFocusedEntry.PACKINGQTY;
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void PackingQtyValueUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnPriceFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnPriceUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_UNFOCUS_COLOR);
        }

        public void OnNetPriceFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }

        public void OnNetPriceUnFocused(object sender, EventArgs args)
        {
            Entry item = (Entry)sender;
            item.BackgroundColor = Color.FromHex(SFAConstants.ON_FOCUS_COLOR);
        }



    }
}
