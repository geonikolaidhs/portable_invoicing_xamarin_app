using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.NonPersistant;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public static class ReceiptBuilder
    {
        public static List<string> CreateReceiptHeader(ReceiptSchema receiptSchema, object source, int lineChars)
        {
            return CreateReceiptPart(receiptSchema.ReceiptHeader, source, lineChars);
        }

        public static List<string> CreateReceiptFooter(ReceiptSchema receiptSchema, object source, int lineChars)
        {
            return CreateReceiptPart(receiptSchema.ReceiptFooter, source, lineChars);
        }

        public static List<string> CreateReceiptBody(ReceiptSchema receiptSchema, object source, int lineChars)
        {
            return CreateReceiptPart(receiptSchema.ReceiptBody, source, lineChars);
        }

        public static List<string> CreateDiscountPrintingLines(DocumentHeader header, int lineChars)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart body = new ReceiptPart();
            bool nothingToShow = true;
            List<DocumentDetail> detailsToShow = header.DocumentDetails.Where(x => x.TotalNonDocumentDiscount > 0).ToList();
            if (detailsToShow.Count > 0)
            {
                nothingToShow = false;

                string title = "-" + ResourcesRest.LINE_DISCOUNTS + "-";
                string horizontalLine = BuildHorizontalLine(title.Length);

                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(title) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                foreach (DocumentDetail detail in detailsToShow)
                {
                    bool itemPrinted = false;
                    foreach (DocumentDetailDiscount discount in detail.DocumentDetailDiscounts
                                                                .Where(x =>
                                                                    x.DiscountSource != eDiscountSource.DOCUMENT
                                                                    && x.DiscountSource != eDiscountSource.PRICE_CATALOG
                                                                     && x.DiscountSource != eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT))
                    {

                        switch (discount.DiscountSource)
                        {
                            case eDiscountSource.PROMOTION_LINE_DISCOUNT:
                                {
                                    if (!itemPrinted)
                                    {
                                        DynamicLine dynamicLine1 = new DynamicLine();
                                        dynamicLine1.Cells.Add(new DynamicLineCell()
                                        {
                                            CellAlignment = eAlignment.LEFT,
                                            Content = discount.DocumentDetail.ItemName
                                        });
                                        body.Elements.Add(dynamicLine1);
                                        itemPrinted = true;
                                    }
                                    DynamicLine dynamicLine = new DynamicLine();
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.LEFT,
                                        Content = discount.DisplayName.ToString().ToUpper()
                                    });
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.RIGHT,
                                        Content = discount.Value.ToString("0.00 €")
                                    });
                                    body.Elements.Add(dynamicLine);
                                    break;
                                }
                            case eDiscountSource.CUSTOM:
                                {
                                    if (!itemPrinted)
                                    {
                                        DynamicLine dynamicLine1 = new DynamicLine();
                                        dynamicLine1.Cells.Add(new DynamicLineCell()
                                        {
                                            CellAlignment = eAlignment.LEFT,
                                            Content = discount.DocumentDetail.ItemName
                                        });
                                        body.Elements.Add(dynamicLine1); itemPrinted = true;
                                    }
                                    DynamicLine dynamicLine = new DynamicLine();
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.LEFT,
                                        Content = ResourcesRest.CUSTOM_DISCOUNT.ToString().ToUpper()
                                    });
                                    dynamicLine.Cells.Add(new DynamicLineCell()
                                    {
                                        CellAlignment = eAlignment.RIGHT,
                                        Content = discount.Value.ToString("0.00 €")
                                    });
                                    body.Elements.Add(dynamicLine);
                                    break;
                                }
                        }
                    }
                }

            }
            if (header.DocumentDiscountAmount > 0 || header.PointsDiscountAmount > 0 || header.PromotionsDiscountAmount > 0)
            {
                nothingToShow = false;

                string title = "-" + ResourcesRest.DOCUMENT_DISCOUNTS.ToString().ToUpper() + "-";
                string horizontalLine = BuildHorizontalLine(title.Length);
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(title) { LineAlignment = eAlignment.CENTER });
                body.Elements.Add(new SimpleLine(horizontalLine) { LineAlignment = eAlignment.CENTER });
                if (header.DocumentDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = ResourcesRest.CUSTOM_DISCOUNT.ToString().ToUpper()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.DocumentDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
                if (header.PointsDiscountAmount > 0)
                {
                    DynamicLine dynamicLine = new DynamicLine();
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.LEFT,
                        Content = ResourcesRest.POINTS_DISCOUNT.ToString().ToUpper()
                    });
                    dynamicLine.Cells.Add(new DynamicLineCell()
                    {
                        CellAlignment = eAlignment.RIGHT,
                        Content = header.PointsDiscountAmount.ToString("0.00 €")
                    });
                    body.Elements.Add(dynamicLine);
                }
            }
            schema.ReceiptHeader = body;
            if (nothingToShow)
            {
                return new List<string>();
            }
            else
            {
                List<string> result = CreateReceiptHeader(schema, null, lineChars);
                return result;

            }
        }

        public static List<string> CreatePointsPrintingLines(bool showTotals, int newPoints, int previousTotalPoints,
            int subtractedPoints, int lineChars, params string[] prefixLines)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart header = new ReceiptPart();
            bool nothingToShow = true;
            if (prefixLines != null)
            {
                foreach (string line in prefixLines)
                {
                    SimpleLine prefixSimpleLine = new SimpleLine(line);
                    prefixSimpleLine.LineAlignment = eAlignment.CENTER;
                    header.Elements.Add(prefixSimpleLine);
                }
            }

            int currentTotalPointsBeforeSubstraction = previousTotalPoints + newPoints;
            int currentTotalPoints = currentTotalPointsBeforeSubstraction - subtractedPoints;
            string horizontalLine = BuildHorizontalLine(lineChars);
            header.Elements.Add(new SimpleLine(horizontalLine));

            if (showTotals && previousTotalPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = ResourcesRest.PREVIOUS_TOTAL_POINTS.ToString().ToUpper() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = previousTotalPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (newPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = ResourcesRest.POINTS_OF_TRANSACTION.ToString().ToUpper() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = newPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (subtractedPoints > 0)
            {
                nothingToShow = false;
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = ResourcesRest.CONSUMED_POINTS.ToString().ToUpper() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = "-" + subtractedPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
            }

            if (showTotals && currentTotalPoints > 0)
            {
                nothingToShow = false;
                header.Elements.Add(new SimpleLine(horizontalLine));
                DynamicLine dynamicLine = new DynamicLine();
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.LEFT, Content = ResourcesRest.CURRENT_TOTAL.ToString().ToUpper() });
                dynamicLine.Cells.Add(new DynamicLineCell() { CellAlignment = eAlignment.RIGHT, Content = currentTotalPoints.ToString() });
                dynamicLine.LineAlignment = eAlignment.CENTER;
                header.Elements.Add(dynamicLine);
                header.Elements.Add(new SimpleLine(horizontalLine));
            }
            schema.ReceiptHeader = header;
            if (nothingToShow)
            {
                return new List<string>();
            }
            else
            {
                List<string> result = CreateReceiptHeader(schema, null, lineChars);
                return result;
            }
        }

        private static string BuildHorizontalLine(int lineChars)
        {
            StringBuilder horizontalLineBuilder = new StringBuilder();
            if (lineChars > 0)
            {
                for (int i = 0; i < lineChars; i++)
                {
                    horizontalLineBuilder.Append("-");
                }
            }
            else
            {
                horizontalLineBuilder.Append("----------");
            }
            return horizontalLineBuilder.ToString();
        }

        public static List<string> CreateSimplePrinterLines(eAlignment allignment, int lineChars, bool addCutPaperCommand, params string[] lines)
        {
            ReceiptSchema schema = new ReceiptSchema();
            ReceiptPart header = new ReceiptPart();
            foreach (string line in lines)
            {
                SimpleLine simleLine = new SimpleLine(line);
                simleLine.LineAlignment = allignment;
                header.Elements.Add(simleLine);
            }
            schema.ReceiptHeader = header;
            List<string> result = CreateReceiptHeader(schema, null, lineChars);
            return result;
        }

        private static List<string> CreateReceiptElement(ReceiptElement element, object source, int lineChars)
        {
            List<string> elementStrings = new List<string>();
            string variableIdentifier = @"@";
            OwnerApplicationSettings appSettings = App.OwnerApplicationSettings;
            if (element is SimpleLine)
            {
                elementStrings.Add(CreateReceiptLine(element as ReceiptLine, source, lineChars, variableIdentifier));
            }
            else if (element is DynamicLine)
            {
                if (
                    //((element as DynamicLine).Condition == eCondition.PROFORMA && source is DocumentHeader && (source as DocumentHeader).DocumentType == ConfigurationManager.ProFormaInvoiceDocumentTypeOid)
                    //|| ((element as DynamicLine).Condition == eCondition.RECEIPT && source is DocumentHeader && (source as DocumentHeader).InEmulationMode == false && (source as DocumentHeader).DocumentType != ConfigurationManager.ProFormaInvoiceDocumentTypeOid)
                    //|| ((element as DynamicLine).Condition == eCondition.NONZERODOCUMENTDISCOUNT && source is DocumentHeader && PlatformRoundingHandler.RoundDisplayValue((source as DocumentHeader).GrossTotalBeforeDocumentDiscount - (source as DocumentHeader).GrossTotal) != 0)
                    //|| ((element as DynamicLine).Condition == eCondition.HASCHANGE && source is DocumentHeader && DocumentService.CheckIfShouldGiveChange((source as DocumentHeader)) == true && (source as DocumentHeader).Change > 0)
                    // || ((element as DynamicLine).Condition == eCondition.NONDEFAULTCUSTOMER && source is DocumentHeader && ((source as DocumentHeader).Customer != ConfigurationManager.DefaultCustomerOid || (source as DocumentHeader).CustomerNotFound))
                    ((element as DynamicLine).Condition == eCondition.SINGLEQUANTITY && source is DocumentDetail && (source as DocumentDetail).Qty == 1)
                    || ((element as DynamicLine).Condition == eCondition.HASCHANGE && source is DocumentHeader && (source as DocumentHeader).HasChange && (source as DocumentHeader).Change > 0)
                    || ((element as DynamicLine).Condition == eCondition.NONZERODOCUMENTDISCOUNT && source is DocumentHeader && DocumentHelper.RoundDisplayValue((source as DocumentHeader).GrossTotalBeforeDiscount - (source as DocumentHeader).GrossTotal, App.OwnerApplicationSettings) != 0)
                    || ((element as DynamicLine).Condition == eCondition.MULTIQUANTITY && source is DocumentDetail && (source as DocumentDetail).Qty != 1)
                    || ((element as DynamicLine).Condition == eCondition.NONZEROLINEDISCOUNT && source is DocumentDetail && DocumentHelper.RoundDisplayValue((source as DocumentDetail).TotalNonDocumentDiscount, App.OwnerApplicationSettings) != 0)
                    || ((element as DynamicLine).Condition == eCondition.DOESNOTINCREASEDRAWERAMOUNT && source is ReportPaymentMethod && (source as ReportPaymentMethod).IncreasesDrawerAmount == false)
                    || ((element as DynamicLine).Condition == eCondition.INCREASESDRAWERAMOUNT && source is ReportPaymentMethod && (source as ReportPaymentMethod).IncreasesDrawerAmount == true)
                    || ((element as DynamicLine).Condition == eCondition.NONE))
                {
                    elementStrings.Add(CreateReceiptLine(element as ReceiptLine, source, lineChars, variableIdentifier));
                }
            }
            else if (element is DetailGroup)
            {
                if (element.Source == eSource.DOCUMENTDETAIL)
                {
                    if (!(source is DocumentHeader))
                    {
                        throw new Exception(String.Format(ResourcesRest.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentDetail).ToString(), typeof(DocumentHeader).ToString()));
                    }
                    foreach (DocumentDetail detail in (source as DocumentHeader).DocumentDetails.Where(x => x.IsCanceled == false))
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, detail, lineChars));
                        }
                    }
                }
                else if (element.Source == eSource.DOCUMENTPAYMENT)
                {
                    if (!(source is DocumentHeader))
                    {
                        throw new Exception(String.Format(ResourcesRest.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentPayment).ToString(), typeof(DocumentHeader).ToString()));
                    }
                    foreach (DocumentPayment payment in (source as DocumentHeader).DocumentPayments.Where(x => x.Amount >= 0))
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, payment, lineChars));
                        }
                    }
                }
                else if (element.Source == eSource.DOCUMENTDETAILDISCOUNT)
                {
                    if (!(source is DocumentDetail))
                    {
                        throw new Exception(String.Format(ResourcesRest.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(DocumentDetailDiscount).ToString(), typeof(DocumentDetail).ToString()));
                    }
                    foreach (DocumentDetailDiscount discount in (source as DocumentDetail).NonHeaderDocumentDetailDiscounts)
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, discount, lineChars));
                        }
                    }
                }
                else if (element.Source == eSource.PAYMENTMETHODS)
                {
                    if (!(source is Report))
                    {
                        throw new Exception(String.Format(ResourcesRest.RECEIPT_SOURCE_INVALID, source.GetType().ToString(), typeof(ReportPaymentMethod).ToString(), typeof(Report).ToString()));
                    }
                    foreach (ReportPaymentMethod paymentMethod in (source as Report).PaymentMethods)
                    {
                        foreach (ReceiptElement line in (element as DetailGroup).Lines)
                        {
                            elementStrings.AddRange(CreateReceiptElement(line, paymentMethod, lineChars));
                        }
                    }
                }
            }

            return elementStrings;
        }

        private static List<string> CreateReceiptPart(ReceiptPart receiptPart, object source, int lineChars)
        {
            List<string> lines = new List<string>();
            foreach (ReceiptElement element in receiptPart.Elements)
            {
                lines.AddRange(CreateReceiptElement(element, source, lineChars));
            }
            return lines;
        }

        private static string CreateReceiptLine(ReceiptLine line, object dynamicLineSource, int lineChars, string variableIdentifier)
        {
            if (line is SimpleLine)
            {
                return BuildSimpleLine(line as SimpleLine, lineChars);
            }
            else if (line is DynamicLine)
            {
                return BuildDynamicLine(line as DynamicLine, lineChars, variableIdentifier, dynamicLineSource);
            }
            return "";
        }

        private static object GetPropertyValue(object src, string propName)
        {
            try
            {
                if (src == null) throw new ArgumentException("Value cannot be null.", "src");
                if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

                if (propName.Contains("."))
                {
                    var temp = propName.Split(new char[] { '.' }, 2);
                    return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
                }
                else
                {
                    Guid key = Guid.Empty; ;
                    PropertyInfo prop = src.GetType().GetProperty(propName);
                    if (prop == null)
                    {
                        return null;
                    }
                    if (prop.PropertyType == typeof(Guid) || typeof(BasicObj).IsAssignableFrom(prop.PropertyType))
                    {
                        if (prop.PropertyType == typeof(Guid))
                        {
                            var val = prop.GetValue(src, null);
                            Guid.TryParse(val.ToString(), out key);
                        }
                        else if (typeof(BasicObj).IsAssignableFrom(prop.PropertyType))
                        {
                            var obj = prop.GetValue(src, null);
                            if (obj != null)
                            {
                                return obj;
                            }
                            else
                            {
                                return GetPropertyValue(src, propName + "Oid");
                            }
                        }
                        switch (propName)
                        {
                            case "CustomerOid":
                                return App.DbLayer.GetById<Customer>(key);
                            case "AddressOid":
                                return App.DbLayer.GetById<Address>(key);
                            case "TraderOid":
                                return App.DbLayer.GetById<Trader>(key);
                            case "DefaultAddressOid":
                                return App.DbLayer.GetById<Address>(key);
                            case "DefaultPhoneOid":
                                return App.DbLayer.GetById<Phone>(key);
                            case "StoreOid":
                                return App.Store;
                            case "TaxOfficeLookUpOid":
                                return App.DbLayer.GetById<TaxOffice>(key);
                            case "DocumentTypeOid":
                                return App.DocumentTypes.Where(x => x.Oid == key).FirstOrDefault();
                            case "DocumentSeriesOid":
                                return App.DocumentSeries.Where(x => x.Oid == key).FirstOrDefault();
                            case "ReferenceCompanyOid":
                                return App.Owner;
                            case "MainCompanyOid":
                                return App.Owner;
                            case "UserOid":
                                return App.DbLayer.GetById<User>(key);
                            case "OwnerOid":
                                return App.Owner;
                            case "BillingAddressOid":
                                return App.DbLayer.GetById<Address>(key);

                            case "Customer":
                                return App.DbLayer.GetById<Customer>(key);
                            case "Address":
                                return App.DbLayer.GetById<Address>(key);
                            case "Trader":
                                return App.DbLayer.GetById<Trader>(key);
                            case "DefaultAddress":
                                return App.DbLayer.GetById<Address>(key);
                            case "DefaultPhone":
                                return App.DbLayer.GetById<Phone>(key);
                            case "Store":
                                return App.Store;
                            case "TaxOfficeLookUp":
                                return App.DbLayer.GetById<TaxOffice>(key);
                            case "DocumentType":
                                return App.DocumentTypes.Where(x => x.Oid == key).FirstOrDefault();
                            case "DocumentSeries":
                                return App.DocumentSeries.Where(x => x.Oid == key).FirstOrDefault();
                            case "ReferenceCompany":
                                return App.Owner;
                            case "MainCompany":
                                return App.Owner;
                            case "User":
                                return App.DbLayer.GetById<User>(key);
                            case "Owner":
                                return App.Owner;
                            case "BillingAddress":
                                return App.DbLayer.GetById<Address>(key);
                            case "CreatedBy":
                                return App.DbLayer.GetById<User>(key);
                            case "UpdatedBy":
                                return App.DbLayer.GetById<User>(key);
                        }
                    }
                    return prop != null ? prop.GetValue(src, null) : null;
                }
            }
            catch (Exception ex)
            {
                App.LogError(new Exception(ex.Message + " - " + propName));
                return "";
            }
        }

        private static string ParceDynamicString(string variableIdentifier, string dynamicString, object source)
        {
            Regex regularExpr = new Regex(variableIdentifier + "[^" + variableIdentifier + "]*" + variableIdentifier);
            MatchCollection matches = regularExpr.Matches(dynamicString);
            List<string> properties = new List<string>();
            DocumentHeader header = source as DocumentHeader;
            foreach (Match match in matches)
            {
                properties.Add(match.Value.Trim(variableIdentifier.ToCharArray()));
            }

            foreach (string property in properties)
            {
                object value;
                try
                {
                    if (property.Contains(".") == false)
                    {
                        PropertyInfo prop = source.GetType().GetProperty(property);
                        if (prop == null)
                        {
                            dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, "");
                            throw new Exception("Property Not Found, Propert Name : " + property);
                        }
                        if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                        {
                            value = String.Format("{0:0.00}", prop.GetValue(source, null));
                        }
                        else if (prop.PropertyType == typeof(DateTime))
                        {
                            value = String.Format("{0:dd/MM/yyyy HH:mm}", prop.GetValue(source, null));
                        }
                        else
                        {
                            value = prop.GetValue(source, null) == null ? "" : prop.GetValue(source, null).ToString();
                        }
                        dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, value.ToString());
                    }
                    else
                    {
                        value = GetPropertyValue(source, property).ToString();
                        dynamicString = dynamicString.Replace(variableIdentifier + property + variableIdentifier, value.ToString());
                    }
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    value = "INVALID_PROPERTY" + property;
                }
            }
            return dynamicString;
        }

        private static string BuildSimpleLine(SimpleLine line, int lineChars)
        {
            string finalLine = "";
            if (line.MaxCharacters == 0)
            {
                finalLine = line.Content;
            }
            else
            {
                int contentLength = (line as SimpleLine).Content.Length;
                finalLine = new string(line.Content.Take(line.MaxCharacters > contentLength ? contentLength : (int)line.MaxCharacters).ToArray());
            }
            switch (line.LineAlignment)
            {
                case eAlignment.CENTER:
                    return PadCenter(finalLine, lineChars, ' ');
                case eAlignment.LEFT:
                    return finalLine;
                case eAlignment.RIGHT:
                    return finalLine.PadLeft(lineChars - finalLine.Length, ' ');
            }
            return "";
        }

        private static string BuildDynamicLine(DynamicLine line, int lineChars, string variableIdentifier, object dynamicLineSource)
        {
            if (line.Cells.Count == 0)
            {
                throw new Exception("Invalid file format. No cells found in Dynamic Line.");
            }

            int cellLength = lineChars / line.Cells.Count;
            int remainingCellsCount = line.Cells.Count;
            int currentCellLength = cellLength;

            string finalLine = "";
            foreach (DynamicLineCell cell in line.Cells)
            {
                string cellString = "";

                cellString = ParceDynamicString(variableIdentifier, cell.Content, dynamicLineSource);
                if (cell.MaxCharacters != 0)
                {
                    int contentLength = cellString.Length;
                    cellString = new string(cellString.Take(cell.MaxCharacters > contentLength ? contentLength : (int)cell.MaxCharacters).ToArray());
                    currentCellLength = (int)cell.MaxCharacters;
                }

                switch (cell.CellAlignment)
                {
                    case eAlignment.CENTER:
                        cellString = PadCenter(cellString, currentCellLength, ' ');
                        break;
                    case eAlignment.RIGHT:
                        cellString = cellString.PadLeft(currentCellLength, ' ');
                        break;
                    case eAlignment.LEFT:
                        cellString = cellString.PadRight(currentCellLength);
                        break;
                }
                finalLine += cellString;
                remainingCellsCount--;
                int remainingSpaceFromCellOverflow = (lineChars - finalLine.Length);

                if (cellString.Length > currentCellLength && remainingSpaceFromCellOverflow > 0 && remainingCellsCount > 0)
                {
                    currentCellLength = remainingSpaceFromCellOverflow / remainingCellsCount;
                }
                else
                {
                    currentCellLength = cellLength;
                }
            }
            return finalLine;
        }

        private static string PadCenter(string s, int width, char c)
        {
            if (s == null || width <= s.Length) return s;
            int center = width / 2;
            int posRight = center + (s.Length / 2) + 1;
            string finalStr = s.PadLeft(posRight, c); //first half of centered string
            for (int i = 0; i < width - posRight; i++)
            {
                finalStr += c;
            }
            return finalStr;
        }


        private static byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }

        //private static object ExecuteDynamicFunction(String FunctionName, String assemblyName, String ClassName, DocumentHeader obj)
        //{
        //    object result;
        //    try
        //    {
        //        string assemblyPath = Path.GetDirectoryName(Application.ExecutablePath) + "//Modules//" + assemblyName;
        //        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        //        Type T = assembly.GetType("ITS.POS.Client." + ClassName);
        //        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
        //        object[] constructorParams = new object[] { this.SessionManager, this.ConfigurationManager, obj, Kernel };
        //        object instance = Activator.CreateInstance(T, constructorParams);
        //        MethodInfo callMethod = T.GetMethod(FunctionName, flags);
        //        Type returnType = callMethod.ReturnType;
        //        if (returnType == typeof(string) || returnType.Name == "void" || string.IsNullOrEmpty(returnType.Name))
        //        {
        //            result = "";
        //        }
        //        else
        //        {
        //            result = Activator.CreateInstance(returnType);
        //        }
        //        if (callMethod != null)
        //        {
        //            result = callMethod.Invoke(instance, null).ToString();
        //        }
        //        assembly = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        App.LogError(ex);
        //        return " ";
        //    }
        //    if (result == null)
        //    {
        //        result = "";
        //    }
        //    return result;
        //}

    }
}