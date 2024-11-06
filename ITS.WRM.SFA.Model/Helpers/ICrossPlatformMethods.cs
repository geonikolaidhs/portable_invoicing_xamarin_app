using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Helpers
{
    public interface ICrossPlatformMethods
    {
        void CancelSync();
        bool IsConnected();
        void GenerateDatabase();
        bool SettingsFileExists();
        string SettingsFilePath();
        DatabaseLayer GetDataBaseLayer();
        Task SyncTableAsync(Type type);
        long GetUpdatedOnTicks(Type type);
        List<Type> GetTypesToUpdate();
        List<T> GetValuesList<T>() where T : BasicObj;
        long GetCurrentUpdatingVersion();
        List<ItemCategory> GetAllItemCategories();
        bool ValidateSettings(SFASettings settings, out string error);
        Task<long> GetServerMaxVersionAsync(string typeName, int timeout);
        SFASettings ReadSettingsFromXML();
        void WriteSettingsToXML(SFASettings sfaCommonConstants);
        List<ItemPresent> GetSpecificItem(string FilterName, Guid root);
        Item GetItemDetails(Guid ItemRequest);
        CultureInfo Languageinfo(string LanguageId);
        Item GetSubItem(Guid ItemOid);
        Barcode GetBarcodeObj(string Code);

        Guid GetSendStatus();

        Guid GetCurrentUserId();

        string GetSfaOid();

        void LogError(Exception ex);


        Item GetItem(Guid ItemOid);

        Store GetStore();

        VatLevel GetStoreVatLevel();

        CompanyNew GetOwner();

        List<StorePriceList> GetStorePriceLists();

        List<DocumentType> GetDocumentTypes();

        List<PriceCatalogPolicyDetail> GetAllPriceCatalogPolicyDetails();

        List<MeasurementUnit> GetAllMeasurementUnits();

        List<PriceCatalog> GetAllPriceCatalogs();
        List<PriceCatalogPolicy> GetAllPriceCatalogPolicies();

        List<VatLevel> GetVatLevels();

        List<VatCategory> GetVatCategories();

        List<VatFactor> GetVatFactors();


        OwnerApplicationSettings GetOwnerApplicationSettings();

        void ExportDB();
        void ImportDB();
        void DownloadDB(string URL_Path);
        void UnzipFile();
        void LogInfo(string info);
        void LogWarning(string warning);

    }
}
