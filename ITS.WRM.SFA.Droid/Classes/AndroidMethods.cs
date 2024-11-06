using Acr.UserDialogs;
using Android.Content;
using Android.Locations;
using Android.Net;
using ITS.WRM.SFA.Model;
using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Helpers;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Resources;
using Java.Util.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Forms;


[assembly: Xamarin.Forms.Dependency(typeof(ITS.WRM.SFA.Droid.Classes.AndroidMethods))]
namespace ITS.WRM.SFA.Droid.Classes
{
    public class AndroidMethods : ICrossPlatformMethods
    {
        private static bool Cancel = false;

        private static long CurrentUpdatingVersion = -1;

        private static string DBPath = null;

        public static string GetDBPath()
        {
            if (DBPath == null)
            {
                DBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SFADroidConstants.CONNECTION_STRING);
            }
            return DBPath;
        }

        public void GenerateDatabase()
        {
            string databaseFilePath = GetDBPath();
            Java.IO.File file = new Java.IO.File(databaseFilePath);
            if (Directory.Exists(file.Parent) == false)
            {
                try
                {
                    Directory.CreateDirectory(file.Parent);
                }
                catch (Exception exception)
                {
                    App.LogError(exception);
                    return;
                }
            }

            DatabaseLayer databaseLayer = new DatabaseLayer(databaseFilePath);
            CreateTableVersionTable(databaseLayer);
            List<Type> extraTypes = new List<Type>() { typeof(StockDocumentHeader), typeof(StockDetail), typeof(DocumentHeader),
                typeof(DocumentDetail), typeof(DocumentDetailDiscount), typeof(DocumentPayment),typeof(Route) };
            List<Type> ListTypes = typeof(CompanyNew).Assembly.GetTypes().Where(x => x.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().Count() == 1)
                                                                         .OrderBy(x => x.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().First().Order).ToList();
            ListTypes.AddRange(extraTypes);
            List<TableVersion> TableVrsions = App.DbLayer.GetTableVersions();
            foreach (Type type in ListTypes)
            {
                try
                {
                    MethodInfo createTableMethodInfo = App.DbLayer.GetType().GetMethod("CreateTable");
                    MethodInfo genericMethodCreateTable = createTableMethodInfo.MakeGenericMethod(new Type[] { type });
                    genericMethodCreateTable.Invoke(App.DbLayer, new object[] { });

                    if (type.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().ToList().Where(x => x.Permissions == eUpdateDirection.MASTER_TO_SFA).FirstOrDefault() != null)
                    {
                        long version = App.DbLayer.GetMaxUpdatedOnTicksFromTable(type);
                        TableVersion tableVersion = TableVrsions.Where(x => x.TableName == type.Name).FirstOrDefault();
                        if (tableVersion != null)
                        {
                            tableVersion.Version = version;
                            App.DbLayer.Update(tableVersion, typeof(TableVersion));
                        }
                        else
                        {
                            App.DbLayer.CreateTableVersionRow(type, version);
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
            }
            if (file.Exists())
            {
                try
                {
                    App.DbLayer.AddIndex();
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                }
            }
        }

        public void CancelSync()
        {
            Cancel = true;
        }

        public long GetCurrentUpdatingVersion()
        {
            return CurrentUpdatingVersion;
        }

        public Guid GetSendStatus()
        {
            return App.SFASettings.DocumentStatusToSendOid;
        }

        public Guid GetCurrentUserId()
        {
            return App.UserId;
        }

        public string GetSfaOid()
        {
            return App.DbLayer.GetAllSfaDevices().Where(x => x.ID == App.SFASettings.SfaId).FirstOrDefault()?.Oid.ToString() ?? "";
        }

        public async Task SyncTableAsync(Type type)
        {
            string expandedTables = (Activator.CreateInstance(type) as BasicObj).GetExpandUrl();
            long resultPageSize = 0;
            HttpRequests client = new HttpRequests(App.SFASettings.ApiTimeout);
            int maxTries = 3;
            CurrentUpdatingVersion = -1;
            do
            {
                try
                {
                    long version = App.DbLayer.GetTableVersionByName(type.Name)?.Version ?? 0;
                    string url = string.Format("/{0}?{1}&$filter=UpdatedOnTicks ge {2} &$top={3}&$orderby=UpdatedOnTicks asc", type.Name, expandedTables, version, SFAConstants.DOWNLOAD_PACKAGE_SIZE);
                    string content = string.Empty;
                    string parseJson = string.Empty;

                    content = await client.Get(url);
                    if (string.IsNullOrEmpty(content) || content.Contains("error"))
                    {
                        maxTries--;
                        continue;
                    }
                    try
                    {
                        JObject objJson = JObject.Parse(content);
                        parseJson = objJson["value"].ToString();
                    }
                    catch (Exception ex)
                    {
                        maxTries--;
                        continue;
                    }
                    if (parseJson == "[]")
                    {
                        break;
                    }
                    List<BasicObj> DataList = DatabaseLayer.DeserializeList(parseJson, type).Cast<BasicObj>().ToList();
                    if (DataList == null)
                    {
                        break;
                    }
                    resultPageSize = DataList.Count();
                    if (resultPageSize < 1)
                    {
                        break;
                    }
                    CurrentUpdatingVersion = App.DbLayer.UpdateTableFromJson(DataList, type);
                    if (Cancel == true)
                    {
                        break;
                    }
                    if (maxTries < 1)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    maxTries--;
                    App.LogError(ex);
                    if (maxTries < 1)
                    {
                        throw ex;
                    }
                }
            } while (resultPageSize >= SFAConstants.DOWNLOAD_PACKAGE_SIZE);
            Cancel = false;
        }

        public Store GetStore()
        {
            return App.Store;
        }

        public CompanyNew GetOwner()
        {
            return App.Owner;
        }

        public List<StorePriceList> GetStorePriceLists()
        {
            return App.StorePriceList; ;
        }

        public List<PriceCatalogPolicyDetail> GetAllPriceCatalogPolicyDetails()
        {
            return App.PriceCatalogPolicyDetails; ;
        }

        public List<PriceCatalog> GetAllPriceCatalogs()
        {
            return App.PriceCatalogs;
        }

        public List<PriceCatalogPolicy> GetAllPriceCatalogPolicies()
        {
            return App.PriceCatalogPolicies;
        }

        public List<VatLevel> GetVatLevels()
        {
            return App.VatLevels;
        }

        public VatLevel GetStoreVatLevel()
        {
            return App.StoreVatLevel;
        }

        public List<VatCategory> GetVatCategories()
        {
            return App.VatCategories;
        }

        public List<DocumentType> GetDocumentTypes()
        {
            return App.DocumentTypes;
        }

        public List<VatFactor> GetVatFactors()
        {
            return App.VatFactors;
        }

        public List<MeasurementUnit> GetAllMeasurementUnits()
        {
            return App.MeasurementUnits;
        }



        private void CreateTableVersionTable(DatabaseLayer databaseLayer)
        {
            MethodInfo createTableMethodInfo = databaseLayer.GetType().GetMethod("CreateTable");
            MethodInfo genericMethodCreateTable = createTableMethodInfo.MakeGenericMethod(new Type[] { typeof(TableVersion) });
            genericMethodCreateTable.Invoke(databaseLayer, new object[] { });
        }

        public bool SettingsFileExists()
        {
            string settingsFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SFADroidConstants.SETTINGS_FILE);
            LogInfo("SettingsFileExists() Will Finish");
            return File.Exists(settingsFile);
        }

        public string SettingsFilePath()
        {
            string settingsFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SFADroidConstants.SETTINGS_FILE);
            return settingsFile;
        }

        public async Task<long> GetServerMaxVersionAsync(string typeName, int timeout)
        {
            using (HttpRequests client = new HttpRequests(timeout))
            {
                string content = await client.Get(@"/Updater/GetMaxVersion/" + typeName);
                long version;
                long.TryParse(content, out version);
                return version;
            }
        }

        public DatabaseLayer GetDataBaseLayer()
        {
            return GetDataLayer();
        }

        private static DatabaseLayer GetDataLayer()
        {
            DatabaseLayer databaseLayer = new DatabaseLayer(GetDBPath());
            return databaseLayer;
        }

        public List<Type> GetTypesToUpdate()
        {
            return typeof(CompanyNew).Assembly.GetTypes()
                                       .Where(x => x.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().Count() == 1 && x.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().First().Permissions == eUpdateDirection.MASTER_TO_SFA)
                                       .OrderBy(x => x.GetCustomAttributes<CreateOrUpdaterOrderAttribute>().First().Order).ToList();
        }

        public long GetUpdatedOnTicks(Type type)
        {
            try
            {
                MethodInfo UpdateTableMethodInfo = App.DbLayer.GetType().GetMethod("GetVersion");
                MethodInfo genericMethodUpdateTable = UpdateTableMethodInfo.MakeGenericMethod(new Type[] { type });
                return (long)genericMethodUpdateTable.Invoke(App.DbLayer, new object[] { });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                return 0;
            }
        }

        public List<T> GetValuesList<T>() where T : BasicObj
        {
            try
            {
                MethodInfo LoadValuesMethodInfo = App.DbLayer.GetType().GetMethod("Get");
                MethodInfo genericMethodLoadValues = LoadValuesMethodInfo.MakeGenericMethod(new Type[] { typeof(T) });
                return (List<T>)genericMethodLoadValues.Invoke(App.DbLayer, new object[] { });
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            return new List<T>();
        }

        public bool IsConnected()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            var isOnline = networkInfo == null ? false : networkInfo.IsConnected;
            return isOnline;
        }

        private static List<Encoding> Encodings = new List<Encoding>() { Encoding.ASCII, Encoding.BigEndianUnicode, Encoding.Unicode, Encoding.UTF7, Encoding.UTF8, Encoding.GetEncoding(1253) };
        public SFASettings ReadSettingsFromXML()
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SFASettings));
                using (FileStream fileStream = new FileStream(SettingsFilePath(), FileMode.Open, FileAccess.Read))
                {
                    using (TextReader textReader = new StreamReader(fileStream))
                    {
                        SFASettings settings = xmlSerializer.Deserialize(textReader) as SFASettings;
                        if (settings != null)
                        {
                            settings.EncodingFrom = Encodings.Where(x => x.EncodingName == settings.EncodingFromStr).FirstOrDefault();
                            settings.EncodingTo = Encodings.Where(x => x.EncodingName == settings.EncodingToStr).FirstOrDefault();
                        }
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                File.Delete(SettingsFilePath());
                throw;
            }
        }

        public void WriteSettingsToXML(SFASettings sfaCommonConstants)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SFASettings));
            App.SFASettings.EncodingFromStr = App.SFASettings.EncodingFrom?.EncodingName ?? "";
            App.SFASettings.EncodingToStr = App.SFASettings.EncodingTo?.EncodingName ?? "";
            using (FileStream fs = new FileStream(SettingsFilePath(), FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fs))
                {
                    xmlSerializer.Serialize(textWriter, sfaCommonConstants);
                }
            }
        }

        public List<ItemPresent> GetSpecificItem(string FilterName, Guid root)
        {
            return App.DbLayer.GetSpecificItem(FilterName, root);
        }

        public Item GetItemDetails(Guid ItemRequest)
        {
            try
            {
                return App.DbLayer.GetItemDetails(ItemRequest);
            }
            catch
            {
                throw;
            }
        }

        public Item GetItem(Guid ItemOid)
        {
            return App.DbLayer.GetItem(ItemOid);
        }

        public List<ItemCategory> GetAllItemCategories()
        {
            return App.DbLayer.GetAllItemCategories();
        }

        public CultureInfo Languageinfo(string LanguageId)
        {
            return System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(LanguageId);
        }

        public List<LinkedItem> GetLinkedItem(Guid ItemOid)
        {
            return App.DbLayer.GetLinkedItems(ItemOid);
        }

        public Item GetSubItem(Guid ItemOid)
        {
            return App.DbLayer.GetSubItem(ItemOid);
        }

        public Barcode GetBarcodeObj(string Code)
        {
            return App.DbLayer.GetBarcodeObj(Code);
        }

        //public OwnerApplicationSettings GetOwnerAppSettings()
        //{
        //    return App.DbLayer.GetOwnerApp();
        //}

        public bool ValidateSettings(SFASettings settings, out string error)
        {
            error = string.Empty;
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(settings.ServerURL))
            {
                sb.AppendLine("Server Url Required");
            }
            if (string.IsNullOrEmpty(settings.AuthenticationURL))
            {
                sb.AppendLine("Authentication Url Required");
            }
            if (string.IsNullOrEmpty(settings.DatabaseDownloadURL))
            {
                sb.AppendLine("DatabaseDown Url Required");
            }
            if (settings.DefaultDocumentStatusOid == null || settings.DefaultDocumentStatusOid == Guid.Empty)
            {
                sb.AppendLine("Default Document Status Required");
            }
            if (settings.DocumentStatusToSendOid == null || settings.DocumentStatusToSendOid == Guid.Empty)
            {
                sb.AppendLine("Send Document Status Required");
            }
            if (settings.DefaultDocumentTypeOid == null || settings.DefaultDocumentTypeOid == Guid.Empty)
            {
                sb.AppendLine("Default DocumentType Required");
            }
            if (settings.DocumentSeries == null || settings.DocumentSeries == Guid.Empty)
            {
                sb.AppendLine("DocumentSeries For DocumentType Required");
            }
            if (settings.DefaultStore == null || settings.DefaultStore == Guid.Empty)
            {
                sb.AppendLine("DefaultStore Required");
            }
            if (settings.SfaId == null || settings.SfaId < 0)
            {
                sb.AppendLine("Device Id Required");
            }
            error = sb.ToString();
            return error == string.Empty ? true : false;
        }

        public void ExportDB()
        {
            try
            {

                var dir1 = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/exportdb/");
                var dir2 = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/db/");
                Export(dir1);
                Export(dir2);

            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        private void Export(Java.IO.File exportDir)
        {
            try
            {
                if (!exportDir.Exists())
                {
                    exportDir.Mkdirs();
                }

                string FromFilePath = GetDBPath();
                string ToFilePath = exportDir + "/" + SFADroidConstants.CONNECTION_STRING;
                if (File.Exists(ToFilePath))
                {
                    File.Delete(ToFilePath);
                }
                System.IO.File.Copy(FromFilePath, ToFilePath);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public void ImportDB()
        {
            try
            {
                var dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/db/");
                if (!dir.Exists())
                {
                    dir.Mkdirs();
                }
                string FromFilePath = dir + "/" + SFADroidConstants.CONNECTION_STRING;
                string ToFilePath = GetDBPath();
                if (File.Exists(ToFilePath))
                {
                    File.Delete(ToFilePath);
                }
                System.IO.File.Copy(FromFilePath, ToFilePath);
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public void DownloadDB(string URL_Path)
        {
            try
            {

                var dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/db/");
                if (!dir.Exists())
                {
                    dir.Mkdirs();
                }
                string destinationPath = dir + "/" + SFADroidConstants.DESTINATION_FILE_NAME;
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }
                if (IsConnected())
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(URL_Path + SFADroidConstants.SFA_ZIP_FILE, destinationPath);
                    webClient.Dispose();
                }
                else
                {
                    throw new Exception(ResourcesRest.NoConnectionFoundTurnOnYourWiFi);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public OwnerApplicationSettings GetOwnerApplicationSettings()
        {
            return App.OwnerApplicationSettings;
        }

        public void UnzipFile()
        {
            var dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/db/");
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }
            string strSourcePath = dir + "/" + SFADroidConstants.DESTINATION_FILE_NAME;

            string strDestFolderPath = dir.ToString();
            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(strSourcePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.NextEntry) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        directoryName = Path.Combine(strDestFolderPath, directoryName);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(Path.Combine(strDestFolderPath, theEntry.Name)))
                            {
                                int size = 2048;
                                byte[] data = new byte[size];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (File.Exists(strSourcePath))
                {
                    File.Delete(strSourcePath);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
                throw;
            }
        }

        public void LogInfo(string info)
        {
            Android.Util.Log.Info("ITS WRM SFA", info);
        }

        public void LogWarning(string warning)
        {
            Android.Util.Log.Warn("ITS WRM SFA", warning);
        }

        public void LogError(Exception ex)
        {
            App.LogError(ex);
        }

        public void LogError(string error)
        {
            Android.Util.Log.Error("ITS WRM SFA", error);
        }
    }
}
