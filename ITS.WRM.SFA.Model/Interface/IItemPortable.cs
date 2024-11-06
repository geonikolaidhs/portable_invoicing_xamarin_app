using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IItemPortable : IPersistentObjectPortable
    {
        string Description { get; }

        string Name { get; }
        string Code { get; }

        ICompanyPortable Owner { get; }

        string ExtraDescription { get; }

        IBarcodePortable DefaultBarcode { get; }

        IVatCategoryPortable VatCategory { get; }

        Guid? MotherCodeOid { get; }

        IItemPortable MotherCode { get; }

        DateTime InsertedDate { get; }

        ISeasonalityPortable Seasonality { get; }

        IBuyerPortable Buyer { get; }

        double PackingQty { get; }

        double OrderQty { get; }

        double MinOrderQty { get; }

        double MaxOrderQty { get; }

        bool IsCentralStored { get; }

        string Remarks { get; }

        ISupplierPortable DefaultSupplier { get; }



        string ImageInfo { get; }

        string ImageDescription { get; }

        decimal Points { get; }

        bool AreDiscountsAllowed { get; }

        bool AcceptsCustomPrice { get; }

        bool AcceptsCustomDescription { get; }


        // eItemCustomPriceOptions CustomPriceOptions { get; }
    }
}