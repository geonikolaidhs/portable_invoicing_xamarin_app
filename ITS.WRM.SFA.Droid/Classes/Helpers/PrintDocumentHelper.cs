using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Droid.Classes.DocumentFormat;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public class PrintDocumentHelper
    {
        public static void PrintDocument(List<byte> bw, DocumentHeader docHeader)
        {
            Encoding EncodingTo = App.SFASettings.EncodingTo ?? Encoding.GetEncoding(1253);
            Encoding EncodingFrom = App.SFASettings.EncodingFrom ?? Encoding.Unicode;
            Receipt receipt = new Receipt();
            List<string> allTheLines = CreateReceiptLines(receipt, docHeader);
            bw.AddRange(AsciiControlChars.Newline);
            if (App.SFASettings.Zpl)
            {
                bw.GreekEncoding2();
            }
            foreach (string line in allTheLines)
            {
                bw.AddRange(new byte[] { 0x1B, 0x40 });
                if (App.SFASettings.PrinterConvertEncoding)
                {
                    bw.AddRange(Encoding.Convert(EncodingFrom, EncodingTo, EncodingFrom.GetBytes(line)));
                }
                else
                {
                    bw.AddRange(Encoding.UTF8.GetBytes(line));
                }
                bw.AddRange(AsciiControlChars.Newline);
            }
            bw.AddRange(AsciiControlChars.Newline);
            bw.Add(AsciiControlChars.EndOfTransmission);
        }



        public static List<string> CreateReceiptLines(Receipt receipt, DocumentHeader header)
        {
            ReceiptSchema format = GetDocumentPrintFormat(header.DocumentTypeOid);
            if (format == null)
            {
                throw new Exception(ResourcesRest.InvalidSchema);
            }
            List<string> allTheLines = new List<string>();
            receipt.Header = ReceiptBuilder.CreateReceiptHeader(format, header, App.SFASettings.PrinterLineChars);
            receipt.Body = ReceiptBuilder.CreateReceiptBody(format, header, App.SFASettings.PrinterLineChars);
            receipt.Footer = ReceiptBuilder.CreateReceiptFooter(format, header, App.SFASettings.PrinterLineChars);
            allTheLines = receipt.GetReceiptLines();
            return allTheLines;
        }

        private static ReceiptSchema GetDocumentPrintFormat(Guid docTypeOid)
        {
            ReceiptSchema schema = null;
            try
            {
                POSPrintFormat format = App.DbLayer.GetPrintFormat(docTypeOid);
                if (format != null && !string.IsNullOrEmpty(format.Format))
                {
                    schema = new ReceiptSchema();
                    schema.LoadFromXmlString(format.Format);
                }
            }
            catch (Exception ex)
            {
                App.LogError(ex);
            }
            return schema;
        }


        //const int FIRST_COL_PAD = 15;
        //const int SECOND_COL_PAD = 7;
        //const int THIRD_COL_PAD = 15;
        //const int FOURTH_COL_PAD = 7;
        //public static int length = 0;
        //public static int width = 560;

        /// <summary>
        /// This is the method we print the receipt the way we want.Note the spaces. 
        /// </summary>
        /// <param name = "bw" ></ param >
        //public static void PrintReceipt(List<byte> bw, DocumentHeader docHeader)
        //{
        //    bool zpl = App.SFASettings.Zpl;
        //    try
        //    {
        //        if (!zpl)
        //        {
        //            bw.AddRange(new byte[] { 0x1B, 0x40 });
        //            bw.AddRange(AsciiControlChars.CenterJustification);
        //            bw.GreekEncoding();

        //            bw.NormalFont("-----------------------------", zpl);
        //            bw.LargeText(App.Owner.CompanyName, zpl);
        //            bw.LargeText(App.Owner.Profession, zpl);
        //            bw.LargeText(App.Store.Name, zpl);
        //            Address address = App.DbLayer.GetAddressById(App.Store.AddressOid);
        //            Phone phone = App.DbLayer.GetPhoneById(address.DefaultPhoneOid);
        //            if ((address != null && address.Street != null && phone != null))
        //            {
        //                bw.LargeText(address.Street, zpl);
        //                bw.LargeText("ΤΗΛ-ΦΑΞ: " + phone.Number + " - " + phone.Number, zpl);
        //            }
        //            Trader trader = App.DbLayer.GetTraderById(App.Owner.TraderOid);
        //            if (trader != null && trader.TaxCode != null && trader.TaxOffice != null)
        //            {
        //                bw.LargeText("ΑΦΜ: " + trader.TaxCode + ", ΔΟΥ " + trader.TaxOffice, zpl);
        //                //bw.LargeText("ΕΠΑΓΓ: " + App.Owner.Profession, zpl);
        //            }

        //            bw.FeedLines(1, zpl);

        //            DocumentType doctype = App.DbLayer.GetDocumentTypeById(docHeader.DocumentTypeOid);
        //            bw.LargeText(doctype.Description, zpl);

        //            bw.FeedLines(1, zpl);
        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            bw.NormalFont("ΗΜ/ΝΙΑ: " + docHeader.CreatedOn.ToShortDateString() + " " + docHeader.CreatedOn.ToShortTimeString(), zpl);

        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            bw.NormalFont("ΑΡ. ΤΙΜΟΛΟΓΙΟΥ: " + docHeader.DocumentNumber, zpl, false);
        //            //bw.AddRange(AsciiControlChars.RightJustification);
        //            bw.NormalFont(String.Format("ΧΡΗΣΤΗΣ: " + App.UserName).PadLeft(45), zpl);

        //            bw.FeedLines(1, zpl);

        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            bw.NormalFont("ΠΕΛΑΤΗΣ: " + docHeader.CustomerName, zpl);

        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            Customer cust = App.DbLayer.GetById<Customer>(docHeader.CustomerOid);
        //            trader = App.DbLayer.GetTraderById(cust.TraderOid);
        //            address = App.DbLayer.GetAddressById(docHeader.BillingAddressOid);
        //            docHeader.BillingAddress = address;
        //            docHeader.Customer = cust;
        //            if (cust != null && cust.Profession != null)
        //            {
        //                bw.NormalFont("ΕΠΑΓΓ: " + cust.Profession, zpl);
        //            }
        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            if (address != null && address.Street != null)
        //            {
        //                bw.NormalFont("ΔΙΕΥΘΥΝΣΗ: " + address.Street, zpl);
        //            }

        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            phone = App.DbLayer.GetPhoneById(address.DefaultPhoneOid);
        //            if (phone != null && phone.Number != null)
        //            {
        //                bw.NormalFont("ΤΗΛΕΦΩΝΟ: " + phone.Number, zpl);
        //            }
        //            if (trader != null && trader.TaxCode != null)
        //            {
        //                bw.NormalFont("ΑΦΜ: " + trader.TaxCode + ", ΔΟΥ " + trader.TaxOffice, zpl);
        //            }
        //            if (docHeader.BillingAddress != null && docHeader.BillingAddress.Street != null)
        //            {
        //                bw.NormalFont("ΠΑΡΑΔΟΣΗ: " + docHeader.BillingAddress.Street, zpl);
        //            }

        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΕΙΔΗ:", zpl);

        //            foreach (var docDetail in docHeader.DocumentDetails)
        //            {
        //                //bw.GreekEncoding();
        //                bw.AddRange(AsciiControlChars.LeftJustification);
        //                string ItemDescription = docDetail.CustomDescription.Length > 50 ? docDetail.CustomDescription.Substring(0, 50) : docDetail.CustomDescription;
        //                bw.NormalFont(docDetail.ItemCode + "  " + ItemDescription, zpl);

        //                bw.AddRange(AsciiControlChars.RightJustification);
        //                string breakDown = docDetail.Qty > 0 ? docDetail.Qty.ToString() : string.Empty;
        //                bw.NormalFont(breakDown.PadRight(5), false, zpl);
        //                bw.NormalFont(" x " + string.Format("{0:0.00}", docDetail.FinalUnitPrice.ToString().PadRight(8)), zpl, false);

        //                bw.AddRange(AsciiControlChars.RightJustification);
        //                bw.NormalFont(string.Format("{0:0.00}", docDetail.GrossTotal), zpl, false);
        //                bw.NormalFont((docDetail.VatFactor * 100).ToString() + " %", zpl);

        //                if (docDetail.GrossTotalBeforeDiscount != docDetail.GrossTotal)
        //                {
        //                    bw.NormalFont("ΕΚΠΤΩΣΗ ", zpl, false);
        //                    bw.NormalFont(string.Format("{0:0.00} ΕΥΡΩ", -(docDetail.GrossTotalBeforeDiscount - docDetail.GrossTotal)), zpl);
        //                    bw.FeedLines(1, zpl);
        //                }
        //            }

        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΣΥΝΟΛΟ:  " + docHeader.GrossTotalBeforeDocumentDiscount + " ΕΥΡΩ", zpl);
        //            bw.NormalFont("ΣΥΝΟΛΙΚΗ ΕΚΠΤΩΣΗ:  " + docHeader.TotalDiscountAmount + " ΕΥΡΩ", zpl);
        //            bw.NormalFont("ΣΥΝΟΛΟ ΧΩΡΙΣ ΦΠΑ:  " + docHeader.NetTotal + " ΕΥΡΩ", zpl);
        //            bw.NormalFont("ΠΟΣΟ ΦΠΑ:  " + docHeader.TotalVatAmount + " ΕΥΡΩ", zpl);
        //            bw.Enlarged("ΤΕΛΙΚΟ ΣΥΝΟΛΟ:  " + docHeader.GrossTotal + " ΕΥΡΩ", zpl);

        //            bw.AddRange(AsciiControlChars.LeftJustification);

        //            bw.FeedLines(1, zpl);


        //            bw.AddRange(AsciiControlChars.RightJustification);
        //            bw.NormalFont("ΤΡΟΠΟΙ ΠΛΗΡΩΜΗΣ", zpl);
        //            foreach (DocumentPayment payment in docHeader.DocumentPayments)
        //            {
        //                if (payment.Amount > 0)
        //                {
        //                    bw.NormalFont(payment.PaymentMethod.Description.ToUpper() + ": " + payment.Amount + " ΕΥΡΩ", zpl);
        //                }
        //                else
        //                {
        //                    bw.NormalFont("ΡΕΣΤΑ: " + payment.Amount + " ΕΥΡΩ", zpl);
        //                }
        //            }


        //            bw.FeedLines(1, zpl);

        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            bw.NormalFont(String.Format("ΤΕΡΜΑΤΙΚΟ: " + App.SFASettings.SfaId), zpl);
        //            bw.AddRange(AsciiControlChars.LeftJustification);
        //            bw.NormalFont(String.Format("ΑΡ. ΜΕΣΟΥ: " + App.SFASettings.VehicleNumber), zpl);
        //            bw.FeedLines(1, zpl);

        //            bw.AddRange(AsciiControlChars.CenterJustification);
        //            bw.NormalFont("ΠΑΡΑΔΟΣΗ            ΠΑΡΑΛΑΒΗ", zpl);
        //            bw.FeedLines(5, zpl);
        //            bw.NormalFont("ΥΠΟΓΡΑΦΗ          ΥΠΟΓΡΑΦΗ", zpl); ;

        //            bw.Finish(zpl);
        //        }
        //        else
        //        {
        //            bw.AddRange(Encoding.UTF8.GetBytes("^XA"));

        //            bw.NormalFont("-----------------------------", zpl, true, "C");
        //            bw.LargeText(App.Owner.CompanyName, zpl, "C");
        //            bw.LargeText(App.Owner.Profession, zpl, "C");
        //            bw.LargeText(App.Store.Name, zpl, "C");
        //            Address address = App.DbLayer.GetAddressById(App.Store.AddressOid);
        //            Phone phone = App.DbLayer.GetPhoneById(address.DefaultPhoneOid);
        //            if ((address != null && address.Street != null && phone != null))
        //            {
        //                bw.LargeText(address.Street, zpl, "C");
        //                bw.LargeText("ΤΗΛ-ΦΑΞ: " + phone.Number + " - " + phone.Number, zpl, "C");
        //            }
        //            Trader trader = App.DbLayer.GetTraderById(App.Owner.TraderOid);
        //            if (trader != null && trader.TaxCode != null && trader.TaxOffice != null)
        //            {
        //                bw.LargeText("ΑΦΜ: " + trader.TaxCode + ", ΔΟΥ " + trader.TaxOffice, zpl, "C");
        //                //bw.LargeText("ΕΠΑΓΓ: " + App.Owner.Profession, zpl, "C");
        //            }

        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΑΡ. ΤΙΜΟΛΟΓΙΟΥ: " + docHeader.DocumentNumber, zpl, false, "L");
        //            bw.NormalFont("ΗΜ/ΝΙΑ: " + docHeader.CreatedOn.ToShortDateString(), zpl, false, "L");
        //            bw.NormalFont("ΠΕΛΑΤΗΣ: " + docHeader.CustomerName, zpl, false, "L");
        //            trader = App.DbLayer.GetTraderById(docHeader.Customer.TraderOid);
        //            address = App.DbLayer.GetAddressById(docHeader.BillingAddressOid);
        //            if (docHeader.Customer != null && docHeader.Customer.Profession != null)
        //            {
        //                bw.NormalFont("ΕΠΑΓΓ: " + docHeader.Customer.Profession, zpl, false, "L");
        //            }
        //            if (address != null && address.Street != null)
        //            {
        //                bw.NormalFont("ΔΙΕΥΘΥΝΣΗ: " + address.Street, zpl, false, "L");
        //            }
        //            phone = App.DbLayer.GetPhoneById(address.DefaultPhoneOid);
        //            if (phone != null && phone.Number != null)
        //            {
        //                bw.NormalFont("ΤΗΛΕΦΩΝΟ: " + phone.Number, zpl, false, "L");
        //            }
        //            trader = App.DbLayer.GetTraderById(docHeader.Customer.TraderOid);
        //            if (docHeader.Customer != null && docHeader.Customer.Trader != null && docHeader.Customer.Trader.TaxCode != null)
        //            {
        //                bw.NormalFont("ΑΦΜ: " + docHeader.Customer.Trader.TaxCode + ", ΔΟΥ " + docHeader.Customer.Trader.TaxOffice, zpl, false, "L");
        //            }

        //            if (docHeader.BillingAddress != null && docHeader.BillingAddress.Street != null)
        //            {
        //                bw.NormalFont("ΠΑΡΑΔΟΣΗ: " + docHeader.BillingAddress.Street, zpl, false, "L");
        //            }

        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΕΙΔΗ:", zpl, false, "L");

        //            foreach (var docDetail in docHeader.DocumentDetails)
        //            {
        //                string ItemDescription = docDetail.CustomDescription.Length > 50 ? docDetail.CustomDescription.Substring(0, 50) : docDetail.CustomDescription;
        //                bw.NormalFont(ItemDescription, zpl, false, "L");

        //                string breakDown = docDetail.Qty > 0 ? docDetail.Qty.ToString() : string.Empty;
        //                bw.NormalFont(breakDown.PadLeft(4) + " x " + string.Format("{0:0.00}", docDetail.FinalUnitPrice.ToString().PadLeft(4)) + string.Format("{0:0.00}", docDetail.GrossTotal.ToString().PadLeft(4)) + (docDetail.VatFactor * 100).ToString().PadLeft(4) + " %", zpl, false, "R");

        //                if (docDetail.GrossTotalBeforeDiscount != docDetail.GrossTotal)
        //                {
        //                    bw.NormalFont("ΕΚΠΤΩΣΗ " + string.Format("{0:0.00} ΕΥΡΩ", -(docDetail.GrossTotalBeforeDiscount - docDetail.GrossTotal)), zpl, false, "R");
        //                }
        //            }
        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΣΥΝΟΛΟ:  " + docHeader.GrossTotalBeforeDocumentDiscount + " ΕΥΡΩ", zpl, false, "R");
        //            bw.NormalFont("ΣΥΝΟΛΙΚΗ ΕΚΠΤΩΣΗ:  " + docHeader.TotalDiscountAmount + " ΕΥΡΩ", zpl, false, "R");
        //            bw.NormalFont("ΣΥΝΟΛΟ ΧΩΡΙΣ ΦΠΑ:  " + docHeader.NetTotal + " ΕΥΡΩ", zpl, false, "R");
        //            bw.NormalFont("ΠΟΣΟ ΦΠΑ:  " + docHeader.TotalVatAmount + " ΕΥΡΩ", zpl, false, "R");
        //            bw.Enlarged("ΤΕΛΙΚΟ ΣΥΝΟΛΟ:  " + docHeader.GrossTotal + " ΕΥΡΩ", zpl, "R");
        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΤΡΟΠΟΙ ΠΛΗΡΩΜΗΣ", zpl, true, "R");

        //            foreach (DocumentPayment payment in docHeader.DocumentPayments)
        //            {
        //                bw.NormalFont(payment.PaymentMethod.Description.ToUpper() + ": " + payment.Amount + " ΕΥΡΩ", zpl, false, "R");
        //            }

        //            bw.FeedLines(1, zpl);

        //            bw.NormalFont("ΠΑΡΑΔΟΣΗ            ΠΑΡΑΛΑΒΗ", zpl, true, "C");
        //            bw.FeedLines(5, zpl);
        //            bw.NormalFont("ΥΠΟΓΡΑΦΗ          ΥΠΟΓΡΑΦΗ", zpl, true, "C");

        //            bw.Finish(zpl, "C");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Fail to create commands for printer: " + ex.Message);
        //    }
        //}

    }
}