using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace ITS.WRM.SFA.Model
{
    public class DatabaseLayer : IDisposable
    {
        private SQLiteConnection _SQLconnection;
        protected SQLiteConnection SQLconnection
        {
            get
            {
                return _SQLconnection == null ? _SQLconnection = new SQLiteConnection(this.Connection) : _SQLconnection;
            }
        }
        private string Connection;

        public DatabaseLayer(string connection)
        {
            this.Connection = connection;
            _SQLconnection = null;
            _SQLconnection = new SQLiteConnection(this.Connection);
        }

        private void OpenConnection()
        {
            try
            {
                this.CloseConnection();
                _SQLconnection = new SQLiteConnection(this.Connection);
            }
            catch (Exception ex)
            {

            }
        }

        private void CloseConnection()
        {
            try
            {
                if (_SQLconnection != null)
                {
                    _SQLconnection.Dispose();
                    _SQLconnection = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Dispose()
        {
            if (_SQLconnection != null)
            {
                _SQLconnection.Dispose();
                _SQLconnection = null;
            }
        }

        public T GetById<T>(Guid Oid) where T : BasicObj
        {

            try
            {
                Type t = typeof(T);
                string name = typeof(T).Name;
                string sql = "SELECT * FROM " + name + " WHERE Oid = " + Oid;
                OpenConnection();
                var mapping = SQLconnection.GetMapping(t, CreateFlags.None);
                var result = SQLconnection.Find(Oid, mapping);
                if (result == null)
                {
                    return null;
                }
                return result as T;
            }
            catch (Exception ex)
            {
                DependencyService.Get<ICrossPlatformMethods>().LogError(ex);
                return null;
            }
            finally
            {
                this.CloseConnection();
            }
        }


        public List<T> GetAll<T>() where T : BasicObj
        {
            try
            {
                Type t = typeof(T);
                string name = typeof(T).Name;
                string sql = "SELECT * FROM " + name;
                OpenConnection();
                var mapping = SQLconnection.GetMapping(t, CreateFlags.None);
                List<T> result = SQLconnection.FindWithQuery(mapping, sql) as List<T>;
                if (result == null)
                {
                    result = new List<T>();
                }
                return result as List<T>;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }


        public void Update(object obj, Type type)
        {
            try
            {
                OpenConnection();
                SQLconnection.Update(obj, type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public List<TableVersion> GetTableVersions()
        {
            this.OpenConnection();
            List<TableVersion> results = SQLconnection.Table<TableVersion>()?.ToList() ?? new List<TableVersion>();
            this.CloseConnection();
            return results;
        }

        public TableVersion GetTableVersionByName(string entityName)
        {
            TableVersion obj = null;
            try
            {
                OpenConnection();
                obj = SQLconnection.Table<TableVersion>().Where(x => x.TableName == entityName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
            return obj;
        }

        public TableVersion GetTableVersionById(Guid id)
        {
            TableVersion obj = null;
            try
            {
                OpenConnection();
                obj = SQLconnection.Find<TableVersion>(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
            return obj;
        }

        public TableVersion CreateTableVersionRow(Type type, long ver)
        {
            this.OpenConnection();
            TableVersion tblVersionRow = new TableVersion();
            try
            {
                tblVersionRow = new TableVersion { Oid = Guid.NewGuid(), TableName = type.Name, Version = ver, UpdatedOnticks = DateTime.Now.Ticks };
                SQLconnection.InsertOrReplace(tblVersionRow);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
            return tblVersionRow;

        }


        public Barcode GetTaxCodeBarcode(string barcodeCode)
        {
            this.OpenConnection();
            Barcode taxCodeBarcode = SQLconnection.Table<Barcode>().Where(x => x.Code == barcodeCode).FirstOrDefault();
            this.CloseConnection();
            return taxCodeBarcode;
        }

        public List<TaxOffice> GetTaxOffices()
        {
            this.OpenConnection();
            List<TaxOffice> result = SQLconnection.Table<TaxOffice>().ToList();
            this.CloseConnection();
            return result;
        }

        public ItemBarcode GetItemBarcode(Item item, Barcode taxCodeBarcode, CompanyNew owner)
        {
            this.OpenConnection();
            ItemBarcode obj = SQLconnection.Table<ItemBarcode>().Where(x => x.ItemOid == item.Oid
                                                                                && x.BarcodeOid == taxCodeBarcode.Oid
                                                                                && x.OwnerOid == owner.Oid).FirstOrDefault();
            this.CloseConnection();
            return obj;

        }

        public VatCategory GetDefaultVatCategoty()
        {
            this.OpenConnection();
            VatCategory obj = SQLconnection.Table<VatCategory>().Where(x => x.IsDefault == true).FirstOrDefault();
            this.CloseConnection();
            return obj;
        }


        public VatLevel GetDefaultVatLevel()
        {
            this.OpenConnection();
            VatLevel obj = SQLconnection.Table<VatLevel>().Where(x => x.IsDefault == true).FirstOrDefault();
            this.CloseConnection();
            return obj;
        }

        public List<VatLevel> GetVatLevels()
        {
            this.OpenConnection();
            List<VatLevel> result = SQLconnection.Table<VatLevel>().Where(x => x.IsActive).ToList();
            this.CloseConnection();
            return result;
        }

        public List<VatFactor> GetVatFactors()
        {
            this.OpenConnection();
            List<VatFactor> result = SQLconnection.Table<VatFactor>().Where(x => x.IsActive).ToList();
            this.CloseConnection();
            return result;
        }

        public List<VatCategory> GetVatCategories()
        {
            this.OpenConnection();
            List<VatCategory> result = SQLconnection.Table<VatCategory>().Where(x => x.IsActive).ToList();
            this.CloseConnection();
            return result;
        }


        public Phone GetPhoneById(Guid? oid)
        {
            try
            {
                if (oid == null || oid == Guid.Empty)
                {
                    return null;
                }
                this.OpenConnection();
                Phone obj = SQLconnection.Find<Phone>(oid);
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public POSPrintFormat GetPrintFormat(Guid docTypeOid)
        {
            try
            {
                return SQLconnection.Table<POSPrintFormat>().Where(x => x.FormatType == eFormatType.DocumentHeader && x.DocumentTypeOid == docTypeOid).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public VatFactor GeVatFactor(Guid VatCategoryOid, Guid VatLevelOid)
        {
            this.OpenConnection();
            VatFactor obj = SQLconnection.Table<VatFactor>().Where(x => x.VatCategoryOid == VatCategoryOid && x.VatLevelOid == VatLevelOid).FirstOrDefault();
            this.CloseConnection();
            return obj;
        }

        public VatFactor GeItemVatFactor(Guid documentVatLevelOid, Guid ItemVatCategoryOid)
        {
            this.OpenConnection();
            VatFactor obj = SQLconnection.Table<VatFactor>().Where(x => x.VatCategoryOid == ItemVatCategoryOid && x.VatLevelOid == documentVatLevelOid).FirstOrDefault();
            this.CloseConnection();
            return obj;
        }

        public List<DocumentStatus> GetAllDocumentStatus()
        {
            try
            {
                this.OpenConnection();
                List<DocumentStatus> results = results = SQLconnection.Table<DocumentStatus>().ToList();
                return results != null && results.Count() > 0 ? results : new List<DocumentStatus>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentType> GetAllDocumentTypes()
        {
            try
            {
                this.OpenConnection();
                List<DocumentType> results = SQLconnection.Table<DocumentType>().ToList();
                return results != null && results.Count() > 0 ? results : new List<DocumentType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentType> GetAllValidDocumentTypes()
        {
            try
            {
                this.OpenConnection();
                List<Guid> series = SQLconnection.Table<DocumentSeries>().Where(x => x.eModule == eModule.SFA).ToList().Select(x => x.Oid).ToList();
                List<Guid> sdst = SQLconnection.Table<StoreDocumentSeriesType>().Where(x => series.Contains(x.DocumentSeriesOid)).ToList().Select(x => x.DocumentTypeOid).ToList();
                List<DocumentType> results = SQLconnection.Table<DocumentType>().ToList();
                return results != null && results.Count() > 0 ? results : new List<DocumentType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentSeries> GetAllDocumentSeries()
        {
            try
            {
                this.OpenConnection();
                List<DocumentSeries> results = SQLconnection.Table<DocumentSeries>().ToList();
                return results != null && results.Count() > 0 ? results : new List<DocumentSeries>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<StockDocumentHeader> GetStockDocuments(DateTime From, DateTime To)
        {
            try
            {
                this.OpenConnection();
                long fromDateTicks = From.Date.Ticks;
                long toDateTicks = To.Date.AddHours(23).AddMinutes(59).Ticks;
                List<StockDocumentHeader> results = SQLconnection.Table<StockDocumentHeader>().Where(x => x.CreatedOnTicks >= fromDateTicks && x.CreatedOnTicks <= toDateTicks).ToList();
                return results != null && results.Count() > 0 ? results : new List<StockDocumentHeader>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public StockDocumentHeader GetStockDocumentById(Guid oid)
        {
            StockDocumentHeader obj = null;
            try
            {
                this.OpenConnection();
                List<StockDetail> details = new List<StockDetail>();
                obj = SQLconnection.Find<StockDocumentHeader>(oid);
                if (obj != null)
                {
                    details = SQLconnection.Table<StockDetail>().Where(x => x.StockHeaderOid == obj.Oid)?.ToList() ?? new List<StockDetail>();
                    obj.Details = details;
                }
                return obj;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {

                this.CloseConnection();

            }
            return obj;
        }

        public List<ITS.WRM.SFA.Model.Model.SFA> GetAllSfaDevices()
        {
            try
            {
                this.OpenConnection();
                List<ITS.WRM.SFA.Model.Model.SFA> results = results = SQLconnection.Table<ITS.WRM.SFA.Model.Model.SFA>().ToList();
                return results != null && results.Count() > 0 ? results : new List<ITS.WRM.SFA.Model.Model.SFA>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<BarcodeType> GetAllBarcodeTypes()
        {
            try
            {
                this.OpenConnection();
                List<BarcodeType> results = results = SQLconnection.Table<BarcodeType>().ToList();
                return results != null && results.Count() > 0 ? results : new List<BarcodeType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<Store> GetAllStores()
        {
            try
            {
                this.OpenConnection();
                List<Store> results = results = SQLconnection.Table<Store>().ToList();
                return results != null && results.Count() > 0 ? results : new List<Store>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<Address> GetAddressByTrader(Guid oid)
        {
            try
            {
                this.OpenConnection();
                return SQLconnection.Table<Address>().Where(x => x.TraderOid == oid)?.ToList() ?? new List<Address>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Customer GetCustomerByTrader(Guid TraderOid)
        {
            try
            {
                this.OpenConnection();
                return SQLconnection.Table<Customer>().Where(x => x.TraderOid == TraderOid)?.FirstOrDefault() ?? null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }


        public List<StoreDocumentSeriesType> GetAllStoreDocumentSeriesTypes(Guid storeOid)
        {
            try
            {
                this.OpenConnection();
                List<StoreDocumentSeriesType> results = new List<StoreDocumentSeriesType>();
                if (storeOid != null && storeOid != Guid.Empty)
                {
                    List<Guid> series = SQLconnection.Table<DocumentSeries>().Where(x => x.StoreOid == storeOid).Select(x => x.Oid).ToList();
                    results = results = SQLconnection.Table<StoreDocumentSeriesType>().Where(x => series.Contains(x.DocumentSeriesOid)).ToList() ?? new List<StoreDocumentSeriesType>();
                }
                else
                {
                    results = results = SQLconnection.Table<StoreDocumentSeriesType>().ToList() ?? new List<StoreDocumentSeriesType>();
                }
                return results != null && results.Count() > 0 ? results : new List<StoreDocumentSeriesType>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public ItemCategory GetItemCategoryById(object CurrentOid)
        {
            try
            {
                this.OpenConnection();
                ItemCategory obj = SQLconnection.Find<ItemCategory>(CurrentOid);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public CategoryNode GetCategoryNodeById(object CurrentOid)
        {
            try
            {
                this.OpenConnection();
                CategoryNode obj = SQLconnection.Find<CategoryNode>(CurrentOid);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<PriceCatalogPolicy> GetAllPriceCatalogPolicies()
        {
            try
            {
                this.OpenConnection();
                List<PriceCatalogPolicy> result = SQLconnection.Table<PriceCatalogPolicy>().Where(x => x.IsActive == true)?.ToList() ?? new List<PriceCatalogPolicy>();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<PriceCatalogPolicyDetail> GetAllPriceCatalogPolicyDetails()
        {
            try
            {
                this.OpenConnection();
                List<PriceCatalogPolicyDetail> result = SQLconnection.Table<PriceCatalogPolicyDetail>()?.ToList() ?? new List<PriceCatalogPolicyDetail>();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<StorePriceCatalogPolicy> GetStorePriceCatalogPolicies(Guid storeOid)
        {
            try
            {
                this.OpenConnection();
                List<StorePriceCatalogPolicy> result = SQLconnection.Table<StorePriceCatalogPolicy>().Where(x => x.StoreOid == storeOid)?.ToList() ?? new List<StorePriceCatalogPolicy>();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Trader GetTraderById(Guid CurrentOid)
        {
            try
            {
                this.OpenConnection();
                Trader result = SQLconnection.Find<Trader>(CurrentOid);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Address GetAddressById(Guid CurrentOid)
        {
            try
            {
                this.OpenConnection();
                Address result = SQLconnection.Find<Address>(CurrentOid);
                this.CloseConnection();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<CustomPin> GetAllAddressWithLocation()
        {
            try
            {
                int index = 0;
                this.OpenConnection();
                List<CustomPin> Pins = new List<CustomPin>();
                var addresses = SQLconnection.Table<Address>().Where(x => x.Latitude != 0 && x.Longitude != 0 && x.TraderOid != Guid.Empty && x.TraderOid != null)?.ToList() ?? new List<Address>();
                foreach (var addr in addresses)
                {
                    index++;
                    if (index > 150)
                    {
                        continue;
                    }
                    Trader trader = SQLconnection.Find<Trader>(addr.TraderOid);
                    if (trader != null)
                    {
                        Customer cust = SQLconnection.Table<Customer>().Where(x => x.TraderOid == trader.Oid).FirstOrDefault();
                        if (cust != null)
                        {
                            Pin mapPin = new Pin()
                            {
                                //Icon = BitmapDescriptorFactory.FromStream
                                Type = PinType.Place,
                                Position = new Position(addr.Latitude, addr.Longitude),
                                Label = cust.CompanyName,
                                Address = addr.FullDescription
                            };
                            CustomPin pin = new CustomPin(addr.Oid, mapPin, addr, cust, trader);
                            Pins.Add(pin);
                        }
                    }
                }
                this.CloseConnection();
                return Pins;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentHeader> GetUnSyncedDocumentHeaders(Guid OidStatus, Guid UserOid)
        {
            try
            {
                this.OpenConnection();
                List<DocumentHeader> results = SQLconnection.Table<DocumentHeader>().Where(x => x.IsSynchronized == false)?.ToList() ?? new List<DocumentHeader>();
                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<DocumentHeader>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentHeader> GetDocumentHeaders(Guid OidStatus)
        {
            try
            {
                this.OpenConnection();
                List<DocumentHeader> results = SQLconnection.Table<DocumentHeader>().Where(x => x.IsSynchronized == false && x.StatusOid == OidStatus)?.ToList() ?? new List<DocumentHeader>();
                //foreach (var header in results)
                //{
                //    SQLconnection.GetChildren(header.Customer);
                //    foreach (DocumentDetail dtl in header.DocumentDetails)
                //    {
                //        dtl.DocumentDetailDiscounts = SQLconnection.Table<DocumentDetailDiscount>().Where(x => x.DocumentDetailOid == dtl.Oid)?.ToList() ?? new List<DocumentDetailDiscount>();
                //    }
                //}
                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<DocumentHeader>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<AddressType> GetAddressTypes()
        {
            this.OpenConnection();
            List<AddressType> result = SQLconnection.Table<AddressType>().ToList();
            this.CloseConnection();
            return result;
        }

        public PriceCatalog GetStorePriceCatalogById(Guid CurrentOid)
        {
            this.OpenConnection();
            PriceCatalog result = SQLconnection.Find<PriceCatalog>(CurrentOid);
            this.CloseConnection();
            return result;
        }

        public List<PriceCatalog> GetAllPriceCatalogs()
        {
            this.OpenConnection();
            List<PriceCatalog> result = SQLconnection.Table<PriceCatalog>().Where(x => x.IsActive == true).ToList();
            this.CloseConnection();
            return result;
        }

        public List<ItemPresent> GetSpecificItem(string FilterName, Guid RootOid)
        {


            try
            {
                this.OpenConnection();
                string filter = FilterName.ToUpper();
                List<ItemPresent> ItemList = new List<ItemPresent>();
                List<ItemPresent> results = new List<ItemPresent>();

                List<Item> items = SQLconnection.Table<Item>().Where(x => x.IsActive == true &&

                                                                    (x.Name.Contains(filter.ToUpper())
                                                                     || x.Name.Contains(filter.ToLower())
                                                                     || x.Code.Contains(filter.ToUpper())
                                                                     || x.Code.Contains(filter.ToLower())
                                                                     )).ToList();

                if (items.Count > 0)
                {
                    List<Guid> itemOids = items.Select(it => it.Oid).ToList();
                    string itemOidsQuery = "'" + String.Join("','", itemOids);
                    itemOidsQuery = itemOidsQuery + "'";

                    ItemList = SQLconnection.Query<ItemPresent>("select Item.Oid,Item.Name,VatCategory.Description as VatDescription,Barcode.Code as BarcodeCode, MeasurementUnit.Description as MeasurementDescription,Item.IsActive as IsActive,ItemAnalyticTree.Root,ItemAnalyticTree.Node "
                                  + "from Item "
                                  + "join Barcode on Item.DefaultBarcode = Barcode.Oid "
                                  + "join VatCategory on Item.VatCategory = VatCategory.Oid "
                                  + "join ItemBarcode on Barcode.Oid = ItemBarcode.Barcode "
                                  + "Join MeasurementUnit on ItemBarcode.MeasurementUnit = MeasurementUnit.Oid "
                                  + "join ItemAnalyticTree  on Item.Oid = ItemAnalyticTree.Object "
                                  + "where Item.Oid in (" + itemOidsQuery + ") "
                                  );
                }
                else
                {
                    return new List<ItemPresent>();
                }
                if (ItemList.Any())
                {
                    results = ItemList.Where(x => x.Root == RootOid).ToList();
                    return results;
                }
                else
                {
                    return new List<ItemPresent>();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<PaymentMethod> GetPaymentMethods()
        {
            try
            {
                this.OpenConnection();
                var results = SQLconnection.GetAllWithChildren<PaymentMethod>();
                if (results.Count > 0)
                {
                    return results;
                }
                else
                {
                    return new List<PaymentMethod>();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentPayment> GetDocumentPayments(Guid DocumentHeaderOid)
        {
            try
            {
                this.OpenConnection();
                var results = SQLconnection.GetAllWithChildren<DocumentPayment>(headerOid => headerOid.DocumentHeaderOid.Equals(DocumentHeaderOid));
                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<DocumentPayment>();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void DeleteEmptyDocument(Guid HeaderOid)
        {
            try
            {

                this.OpenConnection();
                DocumentHeader header = SQLconnection.Find<DocumentHeader>(HeaderOid);
                if (header != null)
                {
                    List<DocumentDetail> details = SQLconnection.Table<DocumentDetail>().Where(x => x.DocumentHeaderOid == HeaderOid)?.ToList();
                    if (details == null || details.Count < 1)
                    {
                        SQLconnection.Delete(header);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentDetailDiscount> GetDocumentDetailDiscounts(Guid DetailOid)
        {
            try
            {
                this.OpenConnection();
                List<DocumentDetailDiscount> results = SQLconnection.Table<DocumentDetailDiscount>().Where(x => x.DocumentDetailOid == DetailOid)?.ToList() ?? new List<DocumentDetailDiscount>();
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public DocumentDetailDiscount GetDocumentDetailPriceCatalogDiscountBy(Guid DetailOid)
        {
            try
            {
                DocumentDetailDiscount obj = null;
                this.OpenConnection();
                obj = SQLconnection.Table<DocumentDetailDiscount>().Where(x => x.DocumentDetailOid == DetailOid && x.DiscountSource == eDiscountSource.PRICE_CATALOG).FirstOrDefault();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public DocumentHeader LoadDocumentFromDatabase(Guid HeaderOid)
        {
            this.OpenConnection();
            DocumentHeader document = SQLconnection.Find<DocumentHeader>(HeaderOid);
            if (document != null)
            {
                try
                {
                    if (document.Store == null)
                    {
                        document.Store = DependencyService.Get<ICrossPlatformMethods>().GetStore();
                    }
                    if (document.Store.Owner == null)
                    {
                        document.Store.Owner = DependencyService.Get<ICrossPlatformMethods>().GetOwner();
                    }
                    if (document.DocumentType == null)
                    {
                        document.DocumentType = DependencyService.Get<ICrossPlatformMethods>().GetDocumentTypes().Where(x => x.Oid == document.DocumentTypeOid).FirstOrDefault();
                    }
                    if (document.PriceCatalogPolicy == null && document.PriceCatalogPolicyOid != null && document.PriceCatalogPolicyOid != Guid.Empty)
                    {
                        document.PriceCatalogPolicy = DependencyService.Get<ICrossPlatformMethods>().GetAllPriceCatalogPolicies().Where(x => x.Oid == document.PriceCatalogPolicyOid).FirstOrDefault();
                    }
                    if (document.PriceCatalog == null && document.PriceCatalogOid != null && document.PriceCatalogOid != Guid.Empty)
                    {
                        document.PriceCatalog = DependencyService.Get<ICrossPlatformMethods>().GetAllPriceCatalogs().Where(x => x.Oid == document.PriceCatalogOid).FirstOrDefault();
                    }
                    if (document.DocumentDetails == null || document.DocumentDetails.Count == 0)
                    {
                        document.DocumentDetails = SQLconnection.Table<DocumentDetail>().Where(x => x.DocumentHeaderOid == document.Oid)?.ToList() ?? new List<DocumentDetail>();
                    }
                    if (document.DocumentPayments == null || document.DocumentPayments.Count == 0)
                    {
                        document.DocumentPayments = SQLconnection.Table<DocumentPayment>().Where(x => x.DocumentHeaderOid == document.Oid)?.ToList() ?? new List<DocumentPayment>();
                    }
                    foreach (DocumentDetail detail in document.DocumentDetails)
                    {
                        if (document.Division != eDivision.Financial)
                        {
                            if (detail.Item == null && detail.ItemOid != Guid.Empty)
                            {
                                detail.Item = SQLconnection.Find<Item>(detail.ItemOid);
                            }
                            if (detail.Item.LinkedItems == null || detail.Item.LinkedItems.Count == 0 && detail.ItemOid != Guid.Empty)
                            {
                                detail.Item.LinkedItems = SQLconnection.GetAllWithChildren<LinkedItem>(x => x.ItemOid == detail.ItemOid);
                            }
                            if (detail.Item.DefaultBarcode == null && detail.Item.DefaultBarcodeOid != null && detail.Item.DefaultBarcodeOid != Guid.Empty)
                            {
                                detail.Item.DefaultBarcode = SQLconnection.Find<Barcode>(detail.Item.DefaultBarcodeOid);
                            }
                            if (detail.Barcode == null && detail.BarcodeOid != Guid.Empty)
                            {
                                detail.Barcode = SQLconnection.Find<Barcode>(detail.BarcodeOid);
                            }
                            if (detail.MeasurementUnit == null)
                            {
                                detail.MeasurementUnit = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == detail.MeasurementUnitOid).FirstOrDefault();
                            }
                            if (detail.DocumentDetailDiscounts == null || detail.DocumentDetailDiscounts.Count == 0)
                            {
                                detail.DocumentDetailDiscounts = SQLconnection.Table<DocumentDetailDiscount>().Where(x => x.DocumentDetailOid == detail.Oid)?.ToList() ?? new List<DocumentDetailDiscount>();
                            }
                        }
                        else
                        {
                            if (detail.SpecialItemOid != null && detail.SpecialItemOid != Guid.Empty)
                            {
                                detail.SpecialItem = SQLconnection.Find<SpecialItem>(detail.SpecialItemOid);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    //throw ex;
                }
                finally
                {
                    this.CloseConnection();
                }
            }
            return document;
        }

        public DocumentType GetDocumentTypeById(Guid CurrentOid)
        {
            try
            {
                this.OpenConnection();
                DocumentType result = SQLconnection.Find<DocumentType>(CurrentOid);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }


        }

        public Barcode GetBarcodeByCode(string Code)
        {
            try
            {
                this.OpenConnection();
                Barcode obj = SQLconnection.Table<Barcode>().Where(x => x.Code == Code).FirstOrDefault();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Barcode GetBarcodeById(Guid Oid)
        {
            Barcode obj = null;
            try
            {
                this.OpenConnection();
                obj = SQLconnection.Find<Barcode>(Oid);

            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
            return obj;
        }

        public MeasurementUnit GetMeasurementUnitByItemBarcode(Guid ItemBarcodeOid)
        {
            try
            {
                this.OpenConnection();
                MeasurementUnit obj = SQLconnection.Find<MeasurementUnit>(ItemBarcodeOid);
                if (obj == null)
                {
                    obj = new MeasurementUnit();
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public DocumentSeries GetSerieByDocTypeAndStore(Guid StoreOid, Guid DocTypeOid)
        {
            try
            {
                this.OpenConnection();
                DocumentSeries serie = null;
                List<StoreDocumentSeriesType> sdst = SQLconnection.Table<StoreDocumentSeriesType>().Where(x => x.DocumentTypeOid == DocTypeOid)?.ToList();
                if (sdst != null && sdst.Count > 0)
                {
                    List<DocumentSeries> series = SQLconnection.Table<DocumentSeries>().Where(x => x.StoreOid == StoreOid && !x.IsCancelingSeries)?.ToList();
                    if (series != null && series.Count > 0)
                    {
                        serie = series.Where(x => sdst.Select(z => z.DocumentSeriesOid).ToList().Contains(x.Oid)).FirstOrDefault();
                        return serie;
                    }
                }
                return serie;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public MeasurementUnit GetMeasurementUnitByBarcode(Guid BarcodeOid)
        {
            try
            {
                this.OpenConnection();
                MeasurementUnit ms = null;
                ItemBarcode obj = SQLconnection.Table<ItemBarcode>().Where(x => x.BarcodeOid == BarcodeOid).FirstOrDefault();
                if (obj != null)
                {
                    ms = SQLconnection.Find<MeasurementUnit>(obj.MeasurementUnitOid);
                }

                if (ms == null)
                {
                    ms = new MeasurementUnit();
                }
                return ms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<LinkedItemsDetails> GetLinkedItemDetails(Guid itemOid)
        {
            try
            {
                this.OpenConnection();
                List<LinkedItemsDetails> resultList = SQLconnection.Query<LinkedItemsDetails>("select i.Name as Name,i.code as Code, LinkedItem.QtyFactor as QtyFactor from Item " +
                                                                " join LinkedItem on Item.Oid = LinkedItem.Item" +
                                                                " join Item i on i.Oid = LinkedItem.SubItem where Item.Oid='" + itemOid.ToString() + "'");
                if (resultList.Any())
                {
                    return resultList;
                }
                else
                {
                    return new List<LinkedItemsDetails>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public List<LinkedItem> GetLinkedItemsCustom(Guid itemOid)
        {
            try
            {
                this.OpenConnection();

                List<LinkedItem> LinkedItems = SQLconnection.GetAllWithChildren<LinkedItem>(x => x.ItemOid == itemOid);
                foreach (LinkedItem linked in LinkedItems)
                {
                    if (linked.Item == null)
                    {
                        linked.Item = SQLconnection.Find<Item>(itemOid);
                    }
                    if (linked.SubItem == null)
                    {
                        linked.SubItem = SQLconnection.Find<Item>(linked.SubItemOid);
                    }
                    if (linked.SubItem.DefaultBarcode == null)
                    {
                        linked.SubItem.DefaultBarcode = SQLconnection.Find<Barcode>(linked.SubItem.DefaultBarcodeOid);
                    }
                    if (linked.SubItem.ItemBarcodes == null)
                    {
                        linked.SubItem.ItemBarcodes = SQLconnection.Table<ItemBarcode>().Where(x => x.ItemOid == linked.SubItemOid).ToList();
                    }
                }

                if (LinkedItems.Any())
                {
                    return LinkedItems;
                }
                else
                {
                    return new List<LinkedItem>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public List<LinkedItem> GetLinkedItems(Guid itemOid)
        {
            try
            {
                this.OpenConnection();

                var results = SQLconnection.GetAllWithChildren<LinkedItem>(x => x.ItemOid == itemOid);
                foreach (var item in results)
                {
                    SQLconnection.GetChildren(item.SubItem);
                }

                if (results.Any())
                {
                    return results;
                }
                else
                {
                    return new List<LinkedItem>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public List<BarcodeDetails> GetBarcodeDetails(Guid itemOid)
        {
            try
            {
                this.OpenConnection();
                List<BarcodeDetails> resultList = SQLconnection.Query<BarcodeDetails>("select Barcode.Oid as BarcodeOid,  Barcode.Code as Barcode, MeasurementUnit.Description as MeasurementUnit, ItemBarcode.CreatedOnTicks as CreatedOnTicks, ItemBarcode.UpdatedOnTicks as  UpdatedOnTicks  " +
                                                                                      " from ItemBarcode " +
                                                                                      " join Barcode on Barcode.Oid = ItemBarcode.Barcode  and ItemBarcode.Item ='" + itemOid.ToString() + "'" +
                                                                                      " join MeasurementUnit on MeasurementUnit.Oid = ItemBarcode.MeasurementUnit ");
                if (resultList.Any())
                {
                    return resultList;
                }
                else
                {
                    return new List<BarcodeDetails>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Item GetItemDetails(Guid ItemOid)
        {
            try
            {
                Item item = SQLconnection.Find<Item>(ItemOid);
                if (item != null)
                {
                    item.BarcodeDetails = GetBarcodeDetails(item.Oid);

                    this.OpenConnection();
                    if (item.DefaultBarcode == null)
                    {
                        item.DefaultBarcode = SQLconnection.Find<Barcode>(item.DefaultBarcodeOid);
                    }
                    if (item.VatCategory == null)
                    {
                        item.VatCategory = SQLconnection.Find<VatCategory>(item.VatCategoryOid);
                    }
                    if (item.MotherCode != null && item.MotherCode != Guid.Empty)
                    {
                        item.MotherItem = SQLconnection.Find<Item>(item.MotherCode);
                    }
                    item.PriceCatalogDetails = SQLconnection.Table<PriceCatalogDetail>().Where(x => x.ItemOid == ItemOid)?.ToList() ?? new List<PriceCatalogDetail>();
                    foreach (PriceCatalogDetail pcd in item.PriceCatalogDetails)
                    {
                        pcd.PriceCatalog = SQLconnection.Find<PriceCatalog>(pcd.PriceCatalogOid) ?? new PriceCatalog();
                        pcd.TimeValues = SQLconnection.Table<PriceCatalogDetailTimeValue>().Where(x => x.PriceCatalogDetailOid == pcd.Oid)?.ToList() ?? new List<PriceCatalogDetailTimeValue>();
                    }
                    item.ItemAnalyticTrees = SQLconnection.Table<ItemAnalyticTree>().Where(x => x.ObjectOid == ItemOid)?.ToList() ?? new List<ItemAnalyticTree>();
                    item.LinkedItemsDetails = GetLinkedItemDetails(item.Oid);
                }
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<ItemCategory> GetAllItemCategories()
        {
            try
            {
                this.OpenConnection();
                List<ItemCategory> result = SQLconnection.Table<ItemCategory>().ToList();
                if (result.Any())
                {
                    return result;
                }
                else
                {
                    return new List<ItemCategory>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }


        }

        public List<ItemAnalyticTree> GetItemAnalyticTrees(Guid itemOid)
        {
            try
            {
                this.OpenConnection();
                List<ItemAnalyticTree> result = SQLconnection.Table<ItemAnalyticTree>().Where(x => x.NodeOid == itemOid).ToList();
                if (result.Any())
                {
                    return result;
                }
                else
                {
                    return new List<ItemAnalyticTree>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }


        }

        public void DeleteList<T>(List<T> DeleteList) where T : BasicObj
        {
            try
            {
                this.OpenConnection();
                foreach (var item in DeleteList)
                {
                    SQLconnection.Delete(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        ////Update specific item of table
        public void Update<T>(T obj) where T : BasicObj
        {
            this.OpenConnection();
            SQLconnection.Update(obj);
            this.CloseConnection();
        }

        public void InsertOrReplaceObj<T>(T obj) where T : BasicObj
        {
            this.OpenConnection();
            try
            {
                long now = DateTime.Now.Ticks;
                //User user = DependencyService.Get<ICrossPlatformMethods>().GetCurrentUser();
                obj.UpdatedBy = DependencyService.Get<ICrossPlatformMethods>().GetCurrentUserId();
                obj.UpdatedOnTicks = now;
                SQLconnection.InsertOrReplace(obj);
            }


            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public void InsertOrReplaceObjWitoutStamp<T>(T obj) where T : BasicObj
        {
            this.OpenConnection();
            try
            {
                SQLconnection.InsertOrReplace(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }
        public void CreateTable<T>() where T : BasicObj
        {
            try
            {
                this.OpenConnection();
                var type = typeof(T);
                List<SQLiteConnection.ColumnInfo> tableInfo = SQLconnection.GetTableInfo(type.Name);
                SQLconnection.CreateTable<T>();

                this.CloseConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public long GetVersion<T>() where T : BasicObj
        {
            try
            {
                this.OpenConnection();
                string tableName = typeof(T).Name;
                TableVersion tableVersion = (from currentTableVersionColumns in SQLconnection.Table<TableVersion>().Where(tableColumn => tableColumn.TableName.Equals(tableName)) select currentTableVersionColumns).FirstOrDefault();

                if (tableVersion == null)
                {
                    return 0;
                }
                else
                {
                    return tableVersion.Version;
                }
            }
            catch (Exception exception)
            {
                //TODO
                return 0;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public static IList DeserializeList(string value, Type type)
        {
            try
            {
                var listType = typeof(List<>).MakeGenericType(type);
                IList list = JsonConvert.DeserializeObject(value.ToString(), listType, JSON_SERIALIZER_SETTINGS) as IList;
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            Culture = DefaultCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");



        public long GetMaxUpdatedOnTicksFromTable(Type type)
        {
            this.OpenConnection();
            long UpdatedOnTicks = 0;
            try
            {
                TableMapping map = SQLconnection.GetMapping(type, CreateFlags.None);
                var result = SQLconnection.Query(map, "select UpdatedOnTicks from " + type.Name + " order by UpdatedOnTicks Desc limit 1", new object[] { }).FirstOrDefault() as BasicObj;
                long.TryParse(result?.UpdatedOnTicks.ToString() ?? 0.ToString(), out UpdatedOnTicks);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
            return UpdatedOnTicks;
        }

        public List<Route> GetAllRoutes()
        {
            try
            {
                this.OpenConnection();
                List<Route> results = SQLconnection.Table<Route>().Where(x => x.RowDeleted == false).ToList();
                if (results != null && results.Count > 0)
                {
                    return results;
                }
                else
                {
                    return new List<Route>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<CustomerPresent> GetCustomerSearch(string Filter, out int count)
        {
            try
            {

                this.OpenConnection();
                string strFilter = Filter.ToUpper();
                count = 0;

                List<CustomerPresent> result = new List<CustomerPresent>();
                string sql = "select Customer.Oid as Oid, Customer.CompanyName as CompanyName,Customer.Code as Code,Customer.Profession as Profession,Trader.TaxCode as TaxCode, IFNULL(Number, '' ) as Number, Address.City || ' ' || Address.Street || ' '  || Address.PostCode || ' ' || Address.POBox as Address "
                                                                        + "from Customer "
                                                                        + "join Trader on Customer.Trader = Trader.Oid "
                                                                        + "left join Address on Customer.DefaultAddress = Address.Oid "
                                                                        + "left join Phone on Address.DefaultPhoneOid = Phone.Oid "
                                                                        + "where Customer.CompanyName like '%" + Filter.ToUpper() + "%' OR "
                                                                        + "Customer.CompanyName like '%" + Filter.ToLower() + "%' OR "
                                                                        + "Customer.Code like '%" + Filter.ToLower() + "%' OR "
                                                                        + "Customer.Code like '%" + Filter.ToUpper() + "%' OR "
                                                                        + "Trader.TaxCode like '%" + Filter.ToUpper() + "%' OR "
                                                                        + "Trader.TaxCode like '%" + Filter.ToLower() + "%' ";

                result = SQLconnection.Query<CustomerPresent>(sql) ?? new List<CustomerPresent>();
                count = result.Count;
                if (count < 1)
                {


                    List<Customer> customerByName = SQLconnection.Table<Customer>().Where(x => x.CompanyName.Contains(strFilter.ToUpper())
                                                                                 || x.CompanyName.Contains(strFilter.ToLower())
                                                                                 || x.Code.Contains(strFilter.ToUpper())
                                                                                 || x.Code.Contains(strFilter.ToLower())
                                                                                 ).ToList();
                    if (customerByName.Count == 0)
                    {
                        List<Trader> customerByTaxCode = SQLconnection.Table<Trader>().Where(x => x.TaxCode.Contains(strFilter)).ToList();
                        if (customerByTaxCode.Count == 0)
                        {
                            List<Phone> customerByPhone = SQLconnection.Table<Phone>().Where(x => x.Number.Equals(strFilter)).ToList();
                            if (customerByPhone.Count == 0)
                            {
                                return new List<CustomerPresent>();
                            }
                            else
                            {
                                List<Guid> PhoneOids = customerByPhone.Select(x => x.Oid).ToList();
                                string strPhobeOids = "'" + String.Join("','", PhoneOids);
                                strPhobeOids = strPhobeOids + "'";
                                List<CustomerPresent> Results = SQLconnection.Query<CustomerPresent>("select Customer.Oid as Oid, Customer.CompanyName as CompanyName,Customer.Code as Code,Customer.Profession as Profession,Trader.TaxCode as TaxCode,Phone.Number as Number, Address.City || ' ' || Address.Street || ' '  || Address.PostCode || ' ' || Address.POBox as Address "
                                                                                        + "from Customer "
                                                                                        + "join Trader on Customer.Trader = Trader.Oid "
                                                                                        + "left join Address on Customer.DefaultAddress = Address.Oid "
                                                                                        + "left join Phone on Address.DefaultPhoneOid = Phone.Oid "
                                                                                        + "where Phone.Oid in (" + strPhobeOids + ")");
                                count = Results.Count;
                                return count > 0 ? Results : new List<CustomerPresent>();
                            }
                        }
                        else
                        {
                            List<Guid> TaxCodeOids = customerByTaxCode.Select(x => x.Oid).ToList();
                            string strTaxCodeOids = "'" + String.Join("','", TaxCodeOids);
                            strTaxCodeOids = strTaxCodeOids + "'";
                            List<CustomerPresent> Results = SQLconnection.Query<CustomerPresent>("select Customer.Oid as Oid, Customer.CompanyName as CompanyName,Customer.Code as Code,Customer.Profession as Profession,Trader.TaxCode as TaxCode,Phone.Number as Number, Address.City || ' ' || Address.Street || ' '  || Address.PostCode || ' ' || Address.POBox as Address "
                                                                                    + "from Customer "
                                                                                    + "join Trader on Customer.Trader = Trader.Oid "
                                                                                    + "left join Address on Customer.DefaultAddress = Address.Oid "
                                                                                    + "left join Phone on Address.DefaultPhoneOid = Phone.Oid "
                                                                                    + "where Trader.Oid in (" + strTaxCodeOids + ")");
                            count = Results.Count;
                            return count > 0 ? Results : new List<CustomerPresent>();
                        }
                    }
                    else
                    {
                        List<Guid> CustomerCodeOids = customerByName.Select(x => x.Oid).ToList();
                        string strCustomerOids = "'" + String.Join("','", CustomerCodeOids);
                        strCustomerOids = strCustomerOids + "'";
                        List<CustomerPresent> Results = SQLconnection.Query<CustomerPresent>("select Customer.Oid as Oid, Customer.CompanyName as CompanyName,Customer.Code as Code,Customer.Profession as Profession,Trader.TaxCode as TaxCode,Phone.Number as Number, Address.City || ' ' || Address.Street || ' '  || Address.PostCode || ' ' || Address.POBox as Address "
                                                                                + "from Customer "
                                                                                + "join Trader on Customer.Trader = Trader.Oid "
                                                                                + "left join Address on Customer.DefaultAddress = Address.Oid "
                                                                                + "left join Phone on Address.DefaultPhoneOid = Phone.Oid "
                                                                                + "where Customer.Oid in (" + strCustomerOids + ")");
                        count = Results.Count;
                        return count > 0 ? Results : new List<CustomerPresent>();
                    }
                }
                else
                {
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<ItemCategory> GetAllRootCategoryNodes()
        {
            try
            {
                this.OpenConnection();
                List<ItemCategory> results = SQLconnection.Table<ItemCategory>().Where(x => x.ParentOid == null || x.ParentOid == Guid.Empty).ToList();
                if (results != null && results.Count > 0)
                {
                    return results;
                }
                else
                {
                    return new List<ItemCategory>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<Guid> ChildrenCategories(Guid CurrentParentOid)
        {
            try
            {
                this.OpenConnection();
                List<ItemCategory> CategorieItems = SQLconnection.Table<ItemCategory>().ToList().Where(x => x.ParentOid != null && x.ParentOid.Equals(CurrentParentOid)).ToList();
                List<Guid> Categories = CategorieItems.Select(x => x.Oid).ToList();

                foreach (Guid CategoryOid in CategorieItems.Select(x => x.Oid))
                {
                    List<Guid> SubCategories = ChildrenCategories(CategoryOid);
                    if (SubCategories.Count > 0)
                    {
                        Categories.AddRange(SubCategories);
                    }
                }
                return Categories;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public ItemDetail GetItemByScanner(Guid? CategoryNode, string FilterName, bool IsActiveItem, bool SearchAllcategories,
                                                        DateTime DateFrom, DateTime UpdatedDate, enumModel.SearchCriteria QueryType, bool onlyItemsWithStock, bool showStock)
        {
            try
            {
                this.OpenConnection();
                ItemDetail ScannedItem = null;
                string filter = FilterName.ToString().TrimEnd('\r');
                Barcode barcode = SQLconnection.Table<Barcode>().Where(x => x.Code == filter && x.IsActive).FirstOrDefault();
                if (barcode != null)
                {
                    ItemBarcode itemBarcode = SQLconnection.Table<ItemBarcode>().Where(x => x.BarcodeOid == barcode.Oid).FirstOrDefault();
                    if (itemBarcode != null)
                    {
                        Item item = SQLconnection.Find<Item>(itemBarcode.ItemOid);
                        if (item != null)
                        {
                            if (onlyItemsWithStock && item.Stock <= 0)
                            {
                                return null;
                            }
                            bool itemIsValid = true;
                            switch (QueryType)
                            {
                                case enumModel.SearchCriteria.IsActiveDateFromAndIsActiveUpdatedDate:
                                    itemIsValid = (item.CreatedOnTicks >= DateFrom.Date.Ticks && item.UpdatedOnTicks >= UpdatedDate.Date.Ticks) ? true : false;
                                    break;
                                case enumModel.SearchCriteria.IsActiveDateFrom:
                                    itemIsValid = item.CreatedOnTicks >= DateFrom.Date.Ticks ? true : false;
                                    break;
                                case enumModel.SearchCriteria.IsActiveUpdatedFrom:
                                    itemIsValid = item.UpdatedOnTicks >= UpdatedDate.Date.Ticks ? true : false;
                                    break;
                                default: break;
                            }
                            itemIsValid = IsActiveItem && !item.IsActive ? false : true;
                            if (itemIsValid)
                            {
                                ScannedItem = new ItemDetail();
                                ScannedItem.ItemBarcode = itemBarcode;
                                ScannedItem.Item = item;
                                ScannedItem.Barcode = barcode;
                                ScannedItem.ItemOid = item.Oid;
                                ScannedItem.BarcodeOid = barcode.Oid;
                                ScannedItem.Barcodecode = barcode.Code;
                                ScannedItem.PackingMeasurementUnitOid = item.PackingMeasurementUnitOid;
                                ScannedItem.PackingMeasurementUnit = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == item.PackingMeasurementUnitOid).FirstOrDefault();
                                ScannedItem.PackingMeasurementUnitRelationFactor = item.PackingQty;
                                ScannedItem.Itemcode = item.Code;
                                ScannedItem.Stock = item.Stock;
                                ScannedItem.ShowStock = showStock;
                                ScannedItem.MeasurementUnit = DependencyService.Get<ICrossPlatformMethods>().GetAllMeasurementUnits().Where(x => x.Oid == itemBarcode.MeasurementUnitOid).FirstOrDefault();
                                ScannedItem.MeasurementUnitOid = ScannedItem.MeasurementUnit?.Oid ?? Guid.Empty;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                if (ScannedItem == null)
                {
                    return null;
                }
                if (!SearchAllcategories)
                {
                    List<Guid> validCategories = null;
                    if (CategoryNode != null && CategoryNode != Guid.Empty)
                    {
                        validCategories = DependencyService.Get<ICrossPlatformMethods>().GetAllItemCategories()?.Where(x => x.Oid == CategoryNode.Value).FirstOrDefault()?.CategoryOids ?? new List<Guid>();
                        bool found = false;
                        List<Guid> itemCats = SQLconnection.Table<ItemAnalyticTree>().Where(x => x.ObjectOid == ScannedItem.ItemOid && x.IsActive).ToList().Select(z => z.NodeOid).ToList();
                        foreach (Guid itemCatOid in itemCats)
                        {
                            if (validCategories.Contains(itemCatOid))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found == false)
                        {
                            return null;
                        }
                    }
                }
                this.CloseConnection();
                return ScannedItem;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Sequence"))
                {
                    return null;
                }
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<ItemPresent> SearchItems(Guid? CategoryNode, string FilterName, bool barcodesearch, bool IsActiveItem, bool SearchAllcategories,
                                                    DateTime DateFrom, DateTime UpdatedDate, enumModel.SearchCriteria QueryType, bool onlyItemsWithStock, bool showStock, out int countResults)
        {
            try
            {
                countResults = 0;
                IList<Item> items = new List<Item>();
                List<ItemPresent> ItemList = new List<ItemPresent>();
                FilterName = FilterName.ToUpper();
                int Active = IsActiveItem == true ? 1 : 0;
                string WhereFilterQueryType = string.Empty;
                string WhereFilterName = !string.IsNullOrEmpty(FilterName) ? " (Item.Name like '%" + FilterName.ToUpper() + "%'  or Item.Name like '%" + FilterName.ToLower() + "%' )" : string.Empty;
                string WhereFilteBarcode = !string.IsNullOrEmpty(FilterName) && barcodesearch == true ? " Barcode.Code  =" + "'" + FilterName + "'" : string.Empty;
                string WhereActive = Active == 1 ? " Item.IsActive = " + 1 : string.Empty;
                string WhereStock = onlyItemsWithStock ? " Item.Stock > 0 " : string.Empty;
                string WhereCategoryFilter = string.Empty;
                int countWhereClause = 0;

                switch (QueryType)
                {
                    case enumModel.SearchCriteria.IsActiveDateFromAndIsActiveUpdatedDate:
                        WhereFilterQueryType = " Item.CreatedOnTicks > " + DateFrom.Ticks + " and Item.UpdatedOnTicks > " + UpdatedDate.Ticks;
                        break;
                    case enumModel.SearchCriteria.IsActiveDateFrom:
                        WhereFilterQueryType = "  Item.CreatedOnTicks > " + DateFrom.Ticks;
                        break;
                    case enumModel.SearchCriteria.IsActiveUpdatedFrom:
                        WhereFilterQueryType = "  Item.UpdatedOnTicks > " + UpdatedDate.Ticks;
                        break;
                    default: break;
                }

                if (SearchAllcategories)
                {
                    CategoryNode = null;
                }
                if (!SearchAllcategories)
                {
                    string CategoriesOids = string.Empty;
                    if (CategoryNode != null && CategoryNode != Guid.Empty)
                    {
                        List<Guid> subcategories = ChildrenCategories(CategoryNode.Value) ?? new List<Guid>();
                        subcategories.Add(CategoryNode.Value);
                        CategoriesOids = "'" + String.Join("','", subcategories);
                        CategoriesOids = CategoriesOids + "'";
                        WhereCategoryFilter = " ItemCategory.Oid in (" + CategoriesOids + ")";
                    }
                }

                string WhereClause = string.IsNullOrEmpty(WhereFilterQueryType) && string.IsNullOrEmpty(WhereFilterName) && string.IsNullOrEmpty(WhereActive)
                     && string.IsNullOrEmpty(WhereFilteBarcode) ? string.Empty : " where ";
                if (!string.IsNullOrEmpty(WhereClause))
                {
                    if (!string.IsNullOrEmpty(WhereFilterQueryType))
                    {
                        WhereClause = WhereClause + WhereFilterQueryType;
                        countWhereClause++;
                    }
                    if (barcodesearch)
                    {
                        if (!string.IsNullOrEmpty(WhereFilteBarcode))
                        {
                            WhereClause = countWhereClause > 0 ? WhereClause + " and " + WhereFilteBarcode : WhereClause + WhereFilteBarcode;
                            countWhereClause++;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(WhereFilterName))
                        {
                            WhereClause = countWhereClause > 0 ? WhereClause + " and " + WhereFilterName : WhereClause + WhereFilterName;
                            countWhereClause++;
                        }
                    }
                    if (!string.IsNullOrEmpty(WhereActive))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + WhereActive : WhereClause + WhereActive;
                        countWhereClause++;
                    }
                    if (!string.IsNullOrEmpty(WhereCategoryFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + WhereCategoryFilter : WhereClause + WhereCategoryFilter;
                        countWhereClause++;
                    }
                    if (!string.IsNullOrEmpty(WhereStock))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + WhereStock : WhereClause + WhereStock;
                        countWhereClause++;
                    }
                }

                string sql = GetQueryString(CategoryNode, barcodesearch, showStock) + WhereClause;
                this.OpenConnection();
                ItemList = SQLconnection.Query<ItemPresent>(sql);
                countResults = ItemList.Count;
                return countResults > 0 ? ItemList : new List<ItemPresent>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        private static string GetQueryString(Guid? CategoryNode, bool barcodeSearch, bool showStock)
        {
            string sql = string.Empty;
            string stockStatement = (showStock ? 1 : 0) + " as ShowStock,";
            if (barcodeSearch)
            {
                if (CategoryNode == null || CategoryNode == Guid.Empty)
                {
                    sql = "select Item.Oid ,Barcode.Oid as BarcodeOid, Item.PackingMeasurementUnit as PackingMeasurementUnitOid, Item.PackingQty as PackingMeasurementUnitRelationFactor,  Item.Stock as Stock, Item.Code as Itemcode, Item.Name,VatCategory.Description as VatDescription,Barcode.Code as BarcodeCode," + stockStatement
                                                                      + " MeasurementUnit.Description as MeasurementDescription,Item.IsActive,Item.CreatedOnTicks,Item.UpdatedOnTicks ,MeasurementUnit.Oid as MeasurementUnitOid,"
                                                                      + " MeasurementUnit.SupportDecimal as SupportDecimal"
                                                                      + " from Item "
                                                                      + " join VatCategory on Item.VatCategory = VatCategory.Oid "
                                                                      + " join ItemBarcode on Item.Oid = ItemBarcode.Item "
                                                                      + " join MeasurementUnit on ItemBarcode.MeasurementUnit = MeasurementUnit.Oid "
                                                                      + " join Barcode on ItemBarcode.Barcode = Barcode.Oid ";

                }
                else
                {
                    sql = "select Item.Oid,Barcode.Oid as BarcodeOid, Item.PackingMeasurementUnit as PackingMeasurementUnitOid, Item.PackingQty as PackingMeasurementUnitRelationFactor,  Item.Code as Itemcode, Item.Name,VatCategory.Description as VatDescription,Barcode.Code as BarcodeCode," + stockStatement
                                                                       + " MeasurementUnit.Description as MeasurementDescription,Item.IsActive,Item.CreatedOnTicks,Item.UpdatedOnTicks ,MeasurementUnit.Oid as MeasurementUnitOid ,MeasurementUnit.SupportDecimal as SupportDecimal"
                                                                       + " from ItemAnalyticTree "
                                                                       + " join Item on ItemAnalyticTree.Object = Item.Oid"
                                                                       + " join ItemCategory on ItemCategory.Oid=ItemAnalyticTree.Node"
                                                                       + " join VatCategory on Item.VatCategory = VatCategory.Oid "
                                                                       + " join ItemBarcode on Item.Oid = ItemBarcode.Item "
                                                                       + " join MeasurementUnit on ItemBarcode.MeasurementUnit = MeasurementUnit.Oid "
                                                                       + " join Barcode on ItemBarcode.Barcode = Barcode.Oid ";
                }
            }
            else
            {
                if (CategoryNode == null || CategoryNode == Guid.Empty)
                {
                    sql = "select Item.Oid,Barcode.Oid as BarcodeOid, Item.PackingMeasurementUnit as PackingMeasurementUnitOid, Item.PackingQty as PackingMeasurementUnitRelationFactor,  Item.Stock as Stock, Item.Code as Itemcode, Item.Name,VatCategory.Description as VatDescription," + stockStatement
                                    + " Barcode.Code as BarcodeCode, MeasurementUnit.Description as MeasurementDescription,Item.IsActive as IsActive  ,MeasurementUnit.SupportDecimal as SupportDecimal, MeasurementUnit.Oid as MeasurementUnitOid"
                                    + " from Item "
                                    + " join VatCategory on Item.VatCategory = VatCategory.Oid "
                                    + " join Barcode on Item.DefaultBarcode = Barcode.Oid "
                                    + " join ItemBarcode on Barcode.Oid = ItemBarcode.Barcode "
                                    + " Join MeasurementUnit on ItemBarcode.MeasurementUnit = MeasurementUnit.Oid ";
                }
                else
                {
                    sql = "select Item.Oid,Barcode.Oid as BarcodeOid, Item.PackingMeasurementUnit as PackingMeasurementUnitOid, Item.PackingQty as PackingMeasurementUnitRelationFactor,  Item.Stock as Stock, Item.Code as Itemcode, Item.Name,VatCategory.Description as VatDescription," + stockStatement
                                   + " Barcode.Code as BarcodeCode, MeasurementUnit.Description as MeasurementDescription,Item.IsActive as IsActive  ,MeasurementUnit.SupportDecimal as SupportDecimal,MeasurementUnit.Oid as MeasurementUnitOid"
                                   + " from Item "
                                   + " join ItemAnalyticTree on ItemAnalyticTree.Object = Item.Oid"
                                   + " join ItemCategory on ItemCategory.Oid=ItemAnalyticTree.Node"
                                   + " join Barcode on Item.DefaultBarcode = Barcode.Oid "
                                   + " join VatCategory on Item.VatCategory = VatCategory.Oid "
                                   + " join ItemBarcode on Barcode.Oid = ItemBarcode.Barcode "
                                   + " Join MeasurementUnit on ItemBarcode.MeasurementUnit = MeasurementUnit.Oid ";
                }
            }
            return sql;
        }

        public List<ItemStockPresent> SearchStockItems(string filter, bool fromScanner)
        {
            List<ItemStockPresent> ItemList = new List<ItemStockPresent>();
            try
            {
                string whereClause = string.Empty;

                if (!string.IsNullOrEmpty(filter))
                {
                    whereClause = fromScanner ? "and Barcode.Code = " + "'" + filter + "'" : " and (Item.Name like '%" + filter.ToUpper() + "%'  or Item.Name like '%" + filter.ToLower() + "%' )";
                }
                string sql = "select DISTINCT  Item.Oid as Oid, Item.Code as Code, Item.Name as Description, Item.Stock as Stock"
                                                                  + " from Item "
                                                                  + " join ItemBarcode on Item.Oid = ItemBarcode.Item "
                                                                  + " join Barcode on ItemBarcode.Barcode = Barcode.Oid "
                                                                  + " Where Item.Stock > 0 " + whereClause;
                this.OpenConnection();
                ItemList = SQLconnection.Query<ItemStockPresent>(sql);
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<ItemStockPresent> GetAllStockItems()
        {
            List<ItemStockPresent> ItemList = new List<ItemStockPresent>();
            try
            {
                string sql = "select Item.Oid as Oid, Item.Code as Code, Item.Name as Description, Item.Stock as Stock"
                                                                  + " from Item "
                                                                  + " Where Item.Stock > 0 ";
                this.OpenConnection();
                ItemList = SQLconnection.Query<ItemStockPresent>(sql);
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<Item> GetStockItems()
        {
            List<Item> ItemList = new List<Item>();
            try
            {
                this.OpenConnection();
                ItemList = SQLconnection.Table<Item>().Where(x => x.Stock > 0).ToList();
                return ItemList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Item GetSubItem(Guid SubItemOid)
        {
            try
            {
                this.OpenConnection();
                Item results = SQLconnection.Find<Item>(x => x.Oid == SubItemOid);
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Barcode GetBarcodeObj(string Code)
        {
            try
            {
                this.OpenConnection();
                string filter = Code.ToUpper();
                Barcode obj = SQLconnection.Table<Barcode>().Where(x => x.Code.Equals(filter)).FirstOrDefault();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public PriceCatalogDetail GetPriceCatalogDetail(PriceCatalog ParamCatalog, Guid ParamBarcodeOid, Guid ItemOid)
        {
            try
            {
                this.OpenConnection();
                DateTime now = DateTime.Now;
                if (ParamCatalog == null || (!ParamCatalog.IsActive || !(ParamCatalog.StartDate <= now) || !(ParamCatalog.EndDate >= now)))
                {
                    return null;
                }

                PriceCatalogDetail obj = SQLconnection.Table<PriceCatalogDetail>().Where(x => x.ItemOid == ItemOid && x.BarcodegOid == ParamBarcodeOid && x.PriceCatalogOid == ParamCatalog.Oid).FirstOrDefault();
                if (obj != null)
                {
                    List<PriceCatalogDetailTimeValue> effectiveTimevalues = new List<PriceCatalogDetailTimeValue>();
                    List<PriceCatalogDetailTimeValue> result = SQLconnection.Table<PriceCatalogDetailTimeValue>().Where(x => x.PriceCatalogDetailOid == obj.Oid)?.ToList() ?? new List<PriceCatalogDetailTimeValue>();
                    foreach (var timeVal in result)
                    {
                        if (timeVal.TimeValueValidFrom <= now.Ticks && timeVal.TimeValueValidUntil >= now.Ticks)
                        {
                            effectiveTimevalues.Add(timeVal);
                        }
                    }
                    obj.TimeValues = effectiveTimevalues;
                }

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public OwnerApplicationSettings GetOwnerApplicationSettings(Guid id)
        {
            try
            {
                this.OpenConnection();
                return SQLconnection.Table<OwnerApplicationSettings>().Where(x => x.OwnerOid == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Item GetItem(Guid ItemOid)
        {
            try
            {
                this.OpenConnection();
                Item obj = SQLconnection.Find<Item>(ItemOid);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<ItemBarcode> GetItemBarcode(Guid BarcodeOid, Guid companyOid)
        {
            try
            {
                this.OpenConnection();

                List<ItemBarcode> results = SQLconnection.Table<ItemBarcode>().Where(x => x.BarcodeOid == BarcodeOid && x.OwnerOid == companyOid)?.ToList();
                if (results.Any())
                {
                    return results;
                }
                else
                {
                    return results = new List<ItemBarcode>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void InsertNewRecord<T>(T obj) where T : BasicObj
        {
            try
            {
                obj.CreatedByDevice = obj.UpdateByDevice = DependencyService.Get<ICrossPlatformMethods>().GetSfaOid();
                List<T> ListObj = new List<T>();
                ListObj.Add(obj);
                this.OpenConnection();
                SQLconnection.InsertOrReplace(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public Customer GetCustomer(Guid CustomerOid)
        {
            try
            {
                this.OpenConnection();

                Customer customer = SQLconnection.Find<Customer>(CustomerOid);
                if (customer != null)
                {
                    customer.Trader = SQLconnection.Find<Trader>(customer.TraderOid);
                }
                else
                {
                    customer = new Customer();
                }
                return customer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void InsertNewDocumentDetail(DocumentDetail objDocDetail)
        {
            try
            {
                this.OpenConnection();
                SQLconnection.InsertOrReplace(objDocDetail);
                if (objDocDetail.DocumentDetailDiscounts != null)
                {
                    foreach (var currentDiscount in objDocDetail.DocumentDetailDiscounts)
                    {
                        currentDiscount.DocumentDetailOid = objDocDetail.Oid;
                        SQLconnection.InsertOrReplace(currentDiscount);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void DeleteDocumentDetail(DocumentDetail objDocDetail)
        {
            try
            {
                this.OpenConnection();
                foreach (var itemDiscount in objDocDetail.DocumentDetailDiscounts)
                {
                    SQLconnection.Delete(itemDiscount);
                }
                SQLconnection.Delete(objDocDetail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<MeasurementUnit> GetMeasurementUnits()
        {
            try
            {
                this.OpenConnection();
                var results = SQLconnection.Table<MeasurementUnit>().Where(x => x.IsActive).ToList();

                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<MeasurementUnit>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<DocumentHeader> GetAffectStockOpenDocuments()
        {

            {
                try
                {
                    this.OpenConnection();
                    List<DocumentHeader> results = SQLconnection.Table<DocumentHeader>().Where(x => x.ItemStockAffectionOptions == ItemStockAffectionOptions.AFFECTS && x.IsSynchronized == false).ToList();
                    results.ForEach(x => x.DocumentDetails = SQLconnection.Table<DocumentDetail>().Where(z => z.DocumentHeaderOid == x.Oid).ToList() ?? new List<DocumentDetail>());
                    if (results.Count() > 0)
                    {
                        return results;
                    }
                    else
                    {
                        return new List<DocumentHeader>();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    this.CloseConnection();
                }
            }
        }

        public List<StorePriceList> GetStorepriceList(Guid storeOid)
        {
            try
            {
                this.OpenConnection();
                var results = SQLconnection.GetAllWithChildren<StorePriceList>(x => x.StoreOid == storeOid);

                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<StorePriceList>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<User> GetUsers()
        {
            try
            {
                this.OpenConnection();
                var results = SQLconnection.Table<User>().ToList();
                if (results.Count() > 0)
                {
                    return results;
                }
                else
                {
                    return new List<User>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public User GetUserByUsername(string strUserName)
        {
            try
            {
                this.OpenConnection();
                User user = null;
                user = SQLconnection.Table<User>().Where(x => x.UserName == strUserName).FirstOrDefault();
                return user;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public User CheckUser(string strUserName, string strPassword)
        {
            try
            {
                this.OpenConnection();
                User user = null;
                user = SQLconnection.Table<User>().Where(x => x.UserName == strUserName).FirstOrDefault();
                if (user != null && user.Password == strPassword)
                {
                    return user;
                }
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<string> GetDocumentList(Guid storeOid, out Dictionary<DocumentType, string> DocumentList)
        {
            try
            {
                this.OpenConnection();
                List<DocumentType> documents = SQLconnection.Table<DocumentType>().ToList();
                List<Guid> series = SQLconnection.Table<DocumentSeries>().Where(x => x.eModule == eModule.SFA && x.StoreOid == storeOid && !x.IsCancelingSeries).ToList().Select(x => x.Oid).ToList();
                List<Guid> sdst = SQLconnection.Table<StoreDocumentSeriesType>().Where(x => series.Contains(x.DocumentSeriesOid)).ToList().Select(x => x.DocumentTypeOid).ToList();
                List<DocumentType> sfaDocuments = new List<DocumentType>();
                foreach (Guid sd in sdst)
                {
                    if (sd != Guid.Empty)
                        if (!sfaDocuments.Select(x => x.Oid).ToList().Contains(sd))
                        {
                            DocumentType doctype = documents.Where(x => x.Oid == sd).FirstOrDefault();
                            if (doctype != null)
                            {
                                sfaDocuments.Add(documents.Where(x => x.Oid == sd).FirstOrDefault());
                            }
                        }
                }
                List<string> result = new List<string>();
                DocumentList = new Dictionary<DocumentType, string>();
                foreach (DocumentType doc in sfaDocuments)
                {
                    DocumentList.Add(doc, doc.Description);
                    result.Add(doc.Description);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

        }

        public List<string> GetCustomerListAdresses(Guid DefaultCustomerAddresOid, Guid traderOid, out Dictionary<Address, string> TraderAddresses)
        {
            List<string> result = new List<string>();
            TraderAddresses = new Dictionary<Address, string>();
            try
            {
                List<Address> Adresses = GetAddressByTrader(traderOid);
                if (Adresses.Count > 0)
                {
                    foreach (Address address in Adresses)
                    {
                        string addr = address.City + " " + address.Street + " " + address.PostCode + " " + address.PostCode;
                        TraderAddresses.Add(address, addr);
                        result.Add(addr);
                    }
                }
                else
                {
                    Address DefaultAddress = GetAddressById(DefaultCustomerAddresOid);
                    if (DefaultAddress != null)
                    {
                        string addr = DefaultAddress.City + " " + DefaultAddress.Street + " " + DefaultAddress.PostCode + " " + DefaultAddress.PostCode;
                        TraderAddresses.Add(DefaultAddress, addr);
                        result.Add(addr);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public int UpdateDocumentSequence()
        {
            try
            {
                this.OpenConnection();
                var RemoteDevice = SQLconnection.Table<RemoteDeviceSequence>().FirstOrDefault();
                if (RemoteDevice == null)
                {
                    RemoteDeviceSequence remoteDeviceObj = new RemoteDeviceSequence();
                    remoteDeviceObj.RemoteDeviceSequenceNumber = 1;
                    SQLconnection.Insert(remoteDeviceObj);
                    return remoteDeviceObj.RemoteDeviceSequenceNumber;
                }
                else
                {
                    RemoteDevice.RemoteDeviceSequenceNumber += 1;
                    SQLconnection.Update(RemoteDevice);
                    return RemoteDevice.RemoteDeviceSequenceNumber;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void InsertNewDocumentHeader(DocumentHeader obj)
        {
            try
            {
                List<DocumentHeader> ListObj = new List<DocumentHeader>();
                ListObj.Add(obj);
                this.OpenConnection();
                SQLconnection.InsertOrReplace(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public void UpdateDocumentHeader(DocumentHeader obj)
        {
            try
            {
                this.OpenConnection();
                if (obj.CreatedBy == null || obj.CreatedBy == Guid.Empty)
                {
                    obj.CreatedBy = DependencyService.Get<ICrossPlatformMethods>().GetCurrentUserId();
                }
                obj.UpdatedBy = DependencyService.Get<ICrossPlatformMethods>().GetCurrentUserId();
                obj.UpdatedOnTicks = DateTime.Now.Ticks;
                SQLconnection.InsertOrReplace(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public PriceCatalogPolicy GetPriceCatalogPolicy(Guid oid)
        {
            try
            {
                this.OpenConnection();
                var ListobjResults = SQLconnection.GetAllWithChildren<PriceCatalogPolicy>(CatalogOid => CatalogOid.Oid == oid);
                foreach (var item in ListobjResults)
                {
                    foreach (var currentCatalog in item.PriceCatalogPolicyDetails)
                    {
                        SQLconnection.GetChildren(currentCatalog);
                        SQLconnection.GetChildren(currentCatalog.PriceCatalogPolicy);
                    }
                }

                return ListobjResults.FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public long UpdateTableFromJson(List<BasicObj> DataList, Type type)
        {
            long maxVersion = 0;
            try
            {
                var dontUpdatePropNames = type.GetRuntimeProperties().Where(p => p.CanRead
                                                          && p.CanWrite
                                                          && p.GetCustomAttributes<IgnoreUpdateProperty>().Count() > 0)
                                                          ?.Select(p => p.Name)?.ToList() ?? new List<string>();

                this.OpenConnection();
                int objectUpdated = 0;
                foreach (var obj in DataList.OrderBy(x => x.UpdatedOnTicks))
                {
                    if (dontUpdatePropNames != null && dontUpdatePropNames.Count() > 0)
                    {
                        var mapping = SQLconnection.GetMapping(type, CreateFlags.None);
                        var databaseObject = SQLconnection.Find(obj.Oid, mapping);
                        if (databaseObject != null)
                        {
                            foreach (string propName in dontUpdatePropNames)
                            {
                                try
                                {
                                    var oldvalue = type.GetProperty(propName)?.GetValue(databaseObject);
                                    type.GetProperty(propName)?.SetValue(obj, oldvalue);
                                }
                                catch (Exception ex)
                                {
                                    DependencyService.Get<ICrossPlatformMethods>().LogError(ex);
                                }
                            }
                        }
                    }
                    SQLconnection.InsertOrReplace(obj);
                    maxVersion = obj.UpdatedOnTicks > maxVersion ? obj.UpdatedOnTicks : maxVersion;
                    objectUpdated++;
                }
                if (objectUpdated > 0)
                {
                    TableVersion tblVersionRow = SQLconnection.Table<TableVersion>().Where(x => x.TableName == type.Name).FirstOrDefault();
                    if (tblVersionRow != null)
                    {
                        tblVersionRow.Version = maxVersion;
                        tblVersionRow.UpdatedOnticks = DateTime.Now.Ticks;
                        SQLconnection.InsertOrReplace(tblVersionRow);
                    }
                    else
                    {
                        tblVersionRow = new TableVersion { Oid = Guid.NewGuid(), TableName = type.Name, Version = maxVersion, UpdatedOnticks = DateTime.Now.Ticks };
                        SQLconnection.InsertOrReplace(tblVersionRow);
                    }
                }
                this.CloseConnection();
            }
            catch (Exception ex)
            {
                DependencyService.Get<ICrossPlatformMethods>().LogError(ex);
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }

            return maxVersion;
        }

        public List<OrderPresent> GetAllOrders(Guid StatusOid, Guid TypeOid, string filter, DateTime From, DateTime To, enumSearchOrder.Criteria searchType)
        {
            try
            {
                int countWhereClause = 0;
                if (filter == null)
                {
                    filter = "";
                }
                string statusFilter = string.Empty;
                string typeFilter = string.Empty;
                string customerFilter = string.Empty;
                string fromDateFilter = string.Empty;
                string toDateFilter = string.Empty;

                if (searchType != enumSearchOrder.Criteria.All)
                {
                    if (StatusOid != null && StatusOid != Guid.Empty)
                    {
                        statusFilter = "DocumentStatus.Oid = '" + StatusOid + "'";
                    }
                    if (TypeOid != null && TypeOid != Guid.Empty)
                    {
                        typeFilter = "DocumentType.Oid = '" + TypeOid + "'";
                    }
                    if (!string.IsNullOrEmpty(filter))
                    {
                        customerFilter = " (DocumentHeader.CustomerName like '%" + filter.ToUpper() + "%'  or DocumentHeader.CustomerName like '%" + filter.ToLower() + "%'  or DocumentHeader.CustomerLookUpTaxCode like '%" + filter.ToUpper() + "%') ";
                    }
                }

                if (searchType != enumSearchOrder.Criteria.All)
                {
                    long fromTicks = new DateTime(From.Ticks).Date.Ticks;
                    long toTicks = new DateTime(To.Ticks).Date.AddHours(23).Ticks;
                    fromDateFilter = "DocumentHeader.CreatedOnTicks >=" + fromTicks;
                    toDateFilter = "DocumentHeader.CreatedOnTicks <=" + toTicks;
                }

                string WhereClause = string.IsNullOrEmpty(statusFilter) && string.IsNullOrEmpty(typeFilter) && string.IsNullOrEmpty(customerFilter) ? string.Empty : " where ";

                if (!string.IsNullOrEmpty(WhereClause))
                {
                    if (!string.IsNullOrEmpty(statusFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + statusFilter : WhereClause + statusFilter;
                        countWhereClause++;
                    }

                    if (!string.IsNullOrEmpty(typeFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + typeFilter : WhereClause + typeFilter;
                        countWhereClause++;
                    }

                    if (!string.IsNullOrEmpty(customerFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + customerFilter : WhereClause + customerFilter;
                        countWhereClause++;
                    }

                    if (!string.IsNullOrEmpty(fromDateFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + fromDateFilter : WhereClause + fromDateFilter;
                        countWhereClause++;
                    }

                    if (!string.IsNullOrEmpty(toDateFilter))
                    {
                        WhereClause = countWhereClause > 0 ? WhereClause + " and " + toDateFilter : WhereClause + toDateFilter;
                        countWhereClause++;
                    }
                }

                string sql = "select DocumentHeader.Oid as Oid, DocumentHeader.IsSynchronized as IsSynchronized ,DocumentStatus.Description as Status,DocumentStatus.Oid as StatusOid, DocumentHeader.FinalizedDate as FinalizedDate, "
                                                                + " DocumentHeader.NetTotal as NetTotal ,DocumentHeader.GrossTotal as GrossTotal, DocumentType.Description as Type, DocumentHeader.CreatedonTicks as CreatedonTicks,"
                                                                + " DocumentHeader.CustomerCode as CustomerCode, DocumentHeader.CustomerName as CompanyName, DocumentHeader.TotalDiscountAmount as Discount,"
                                                                + " DocumentHeader.RemoteDeviceSequence,DocumentType.Oid as TypeOid from DocumentHeader "
                                                                + " join DocumentStatus on DocumentHeader.Status = DocumentStatus.Oid "
                                                                + " join DocumentType on DocumentHeader.DocumentType = DocumentType.Oid " + WhereClause;
                this.OpenConnection();

                List<OrderPresent> results = SQLconnection.Query<OrderPresent>(sql)?.ToList() ?? new List<OrderPresent>();
                this.CloseConnection();
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public List<OrderPresent> GetCustomerDocuments(Guid customerOid)
        {
            try
            {

                string sql = "select DocumentHeader.Oid as Oid, DocumentHeader.IsSynchronized as IsSynchronized ,DocumentStatus.Description as Status,DocumentStatus.Oid as StatusOid, DocumentHeader.FinalizedDate as FinalizedDate, "
                                                                + " DocumentHeader.NetTotal as NetTotal ,DocumentHeader.GrossTotal as GrossTotal, DocumentType.Description as Type, DocumentHeader.CreatedonTicks as CreatedonTicks,"
                                                                + " DocumentHeader.CustomerCode as CustomerCode, DocumentHeader.CustomerName as CompanyName, DocumentHeader.TotalDiscountAmount as Discount,"
                                                                + " DocumentHeader.RemoteDeviceSequence,DocumentType.Oid as TypeOid from DocumentHeader "
                                                                + " join DocumentStatus on DocumentHeader.Status = DocumentStatus.Oid "
                                                                + " join DocumentType on DocumentHeader.DocumentType = DocumentType.Oid where DocumentHeader.Customer = '" + customerOid + "'";

                this.OpenConnection();

                List<OrderPresent> results = SQLconnection.Query<OrderPresent>(sql)?.ToList() ?? new List<OrderPresent>();
                this.CloseConnection();
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void DeleteObj<T>(T obj)
        {
            try
            {
                this.OpenConnection();
                SQLconnection.Delete(obj);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.CloseConnection();
            }
        }

        public void RemoveDocumentHeaders(DocumentHeader DocumentHeaderObj)
        {
            try
            {
                this.OpenConnection();
                if (DocumentHeaderObj.DocumentPayments != null && DocumentHeaderObj.DocumentPayments.Count > 0)
                {
                    foreach (var pay in DocumentHeaderObj.DocumentPayments)
                    {
                        SQLconnection.Delete(pay);
                    }
                }
                if (DocumentHeaderObj.DocumentDetails != null && DocumentHeaderObj.DocumentDetails.Count > 0)
                {
                    foreach (var detail in DocumentHeaderObj.DocumentDetails)
                    {
                        if (detail.DocumentDetailDiscounts != null && detail.DocumentDetailDiscounts.Count > 0)
                        {
                            foreach (DocumentDetailDiscount disc in detail.DocumentDetailDiscounts)
                            {
                                SQLconnection.Delete(disc);
                            }
                        }
                        SQLconnection.Delete(detail);
                    }
                }
                SQLconnection.Delete(DocumentHeaderObj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }


        public void AddIndex()
        {
            try
            {

                this.OpenConnection();

                List<string> indexList = new List<string>();
                indexList.Add("CREATE INDEX IF NOT EXISTS BarcodeItemCodePer on ItemBarcode(Oid,Barcode,Item,MeasurementUnit);");
                indexList.Add("CREATE INDEX IF NOT EXISTS BarcodeItemPer ON ItemBarcode(Oid, Barcode, MeasurementUnit);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemBarcode_Barcode on ItemBarcode(Barcode);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemBarcode_Item on ItemBarcode(Item);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemBarcodePer ON ItemBarcode(Barcode,MeasurementUnit);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemBarcode_Item on ItemBarcode(Oid,Item);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemBarcode_MeasurementUnit on ItemBarcode(Oid,MeasurementUnit);");

                indexList.Add("CREATE INDEX IF NOT EXISTS BarcodePer ON Barcode(Oid, Code);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Barcode_Code_per on Barcode(Code);");

                indexList.Add("CREATE INDEX IF NOT EXISTS ItemAnalyticTreeItemPer on ItemAnalyticTree(Oid, Object, Node);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemAnalyticTree_ItemCategory on ItemAnalyticTree(Oid,Node);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemAnalyticTree_Object on ItemAnalyticTree(Object);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemToAnalyticPer on ItemAnalyticTree(Oid, Object);");

                indexList.Add("CREATE INDEX IF NOT EXISTS ItemPer ON Item(IsActive, Oid, Name, VatCategory, DefaultBarcode);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Item_DefaultBarcode on Item(DefaultBarcode);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Item_IsActive on Item(IsActive);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Item_Name on Item(Name);");

                indexList.Add("CREATE INDEX IF NOT EXISTS Customer_Code on Customer(Code);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Customer_Address on Customer(DefaultAddress);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Customer_Trader on Customer(Oid,Trader);");

                indexList.Add("CREATE INDEX IF NOT EXISTS Trader_Taxcode on Trader(Taxcode);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Address_Trader on Trader(Oid,Trader);");

                indexList.Add("CREATE INDEX IF NOT EXISTS Address_Phone on Address(DefaultPhoneOid);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Phone_Address on Phone(Oid,Address);");
                indexList.Add("CREATE INDEX IF NOT EXISTS Phone_Number on Phone(Number);");

                indexList.Add("CREATE INDEX IF NOT EXISTS MeasurementUnit_ItemBarcode on MeasurementUnit(MeasurementUnit,Barcode);");

                indexList.Add("CREATE INDEX IF NOT EXISTS CategoryNode_Parent on CategoryNode(Oid,ParentOid);");
                indexList.Add("CREATE INDEX IF NOT EXISTS ItemCategory_Parent on ItemCategory(Oid,ParentOid);");


                indexList.Add("CREATE INDEX IF NOT EXISTS DocumentDetail_DocumentHeader on DocumentDetail(DocumentHeader);");
                indexList.Add("CREATE INDEX IF NOT EXISTS DocumentDiscount_DocumentDetail on DocumentDiscount(DocumentDetail);");
                indexList.Add("CREATE INDEX IF NOT EXISTS DocumentPayment_DocumentDetail on DocumentPayment(DocumentDetail);");
                indexList.Add("CREATE INDEX IF NOT EXISTS DocumentDetail_Item on DocumentDetail(Item);");
                indexList.Add("CREATE INDEX IF NOT EXISTS DocumentDetail_Barcode on DocumentDetail(Barcode);");


                foreach (var currentIndex in indexList)
                {
                    try
                    {
                        SQLconnection.Execute(currentIndex);
                        SQLconnection.Commit();
                    }
                    catch (Exception ex)
                    {
                        string error = currentIndex.ToString();
                        continue;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
    }
}
