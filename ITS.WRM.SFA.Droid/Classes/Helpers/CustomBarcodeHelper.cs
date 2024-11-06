using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using ITS.WRM.SFA.Model.Model.NonPersistant;
using ITS.WRM.SFA.Model.NonPersistant;

namespace ITS.WRM.SFA.Droid.Classes.Helpers
{
    public static class CustomBarcodeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>\
        public static BarcodeParseResult ParseCustomBarcode<T>(List<T> barcodeTypes, string scannedBarcode, bool doPadding, int barcodeLength,
            char paddingChar, bool padRight = true) where T : BarcodeType
        {
            BarcodeParseResult barcodeParseResult = new BarcodeParseResult();

            try
            {
                PropertyInfo maskProperty = typeof(T).GetProperty("Mask");
                PropertyInfo prefixProperty = typeof(T).GetProperty("Prefix");


                PropertyInfo prefixIncludedProperty = typeof(T).GetProperty("PrefixIncluded");
                PropertyInfo nonSpecialCharactersIncludedProperty = typeof(T).GetProperty("NonSpecialCharactersIncluded");
                if (maskProperty == null || prefixProperty == null || prefixIncludedProperty == null || nonSpecialCharactersIncludedProperty == null)
                {
                    throw new Exception("Unsupported type");
                }

                int scannedLength = scannedBarcode.Length;
                foreach (T bct in barcodeTypes)
                {
                    string bctMask = maskProperty.GetValue(bct, null) as string;
                    string bctPrefix = prefixProperty.GetValue(bct, null) as string;

                    bool bctNonSpecialCharactersIncluded = (bool)nonSpecialCharactersIncludedProperty.GetValue(bct, null);
                    bool bctPrefixIncludedProperty = (bool)prefixIncludedProperty.GetValue(bct, null);

                    if (!string.IsNullOrWhiteSpace(bctMask) && !string.IsNullOrWhiteSpace(bctPrefix) && scannedBarcode.StartsWith(bctPrefix))
                    {
                        int totalLength = bctMask.Length + bctPrefix.Length;
                        if (totalLength != scannedLength)
                        {
                            continue;
                        }
                        string codeWithoutPrefix = GetMaskCharacterString(scannedBarcode, MaskCustomerCodeChar, MaskIgnoreChar, bctMask, bctPrefix, true);
                        if (codeWithoutPrefix.Length > 0)
                        {
                            ////Customer search
                            barcodeParseResult.PLU = codeWithoutPrefix;
                            string code = bctPrefix + codeWithoutPrefix;
                            if (bctNonSpecialCharactersIncluded && bctPrefixIncludedProperty)
                            {
                                barcodeParseResult.DecodedCode = scannedBarcode;
                            }
                            else if (bctNonSpecialCharactersIncluded && !bctPrefixIncludedProperty)
                            {
                                barcodeParseResult.DecodedCode = scannedBarcode.Substring(bctPrefix.Length);
                            }
                            else if (!bctNonSpecialCharactersIncluded && !bctPrefixIncludedProperty)
                            {
                                barcodeParseResult.DecodedCode = codeWithoutPrefix;
                            }
                            else
                            {
                                barcodeParseResult.DecodedCode = code;
                            }

                            //barcodeParseResult.DecodedCode = code;
                            barcodeParseResult.BarcodeParsingResult = BarcodeParsingResult.CUSTOMER;
                            barcodeParseResult.BarcodeType = GetBarcodeTypeOid(bct);
                        }
                        else
                        {
                            ////Item search
                            codeWithoutPrefix = GetMaskCharacterString(scannedBarcode, MaskItemCodeChar, MaskIgnoreChar, bctMask, bctPrefix, false);
                            if (codeWithoutPrefix.Length > 0)
                            {
                                barcodeParseResult.PLU = codeWithoutPrefix;
                                string code = (bctPrefix + codeWithoutPrefix).PadRight(scannedBarcode.Length, '0');
                                if (doPadding)
                                {
                                    code = code.PadLeft(barcodeLength, paddingChar);
                                }
                                barcodeParseResult.DecodedCode = code;

                                decimal codeValue;
                                if (GetMaskCharacterString(scannedBarcode, MaskQuantityIntegralChar, MaskQuantityDecimalChar, bctMask, bctPrefix, out codeValue))
                                {
                                    barcodeParseResult.BarcodeParsingResult = BarcodeParsingResult.ITEM_CODE_QUANTITY;
                                    barcodeParseResult.Quantity = codeValue;
                                    barcodeParseResult.CodeValue = 0;
                                    barcodeParseResult.BarcodeType = GetBarcodeTypeOid(bct);
                                }
                                else if (GetMaskCharacterString(scannedBarcode, MaskValueIntegralChar, MaskValueDecimalChar, bctMask, bctPrefix, out codeValue))
                                {
                                    barcodeParseResult.BarcodeParsingResult = BarcodeParsingResult.ITEM_CODE_VALUE;
                                    barcodeParseResult.CodeValue = codeValue;
                                    barcodeParseResult.Quantity = 0;
                                    barcodeParseResult.BarcodeType = GetBarcodeTypeOid(bct);
                                }
                                if (barcodeParseResult.BarcodeParsingResult == BarcodeParsingResult.NONE)
                                {
                                    throw new Exception("Incorrect custom barcode: No quantity or value found.");
                                }
                            }
                        }
                    }
                }
                return barcodeParseResult;
            }
            catch
            {
                barcodeParseResult.SetDefaultValues();
                barcodeParseResult.BarcodeParsingResult = BarcodeParsingResult.NONE;
                return barcodeParseResult;
            }
        }

        private static string GetMaskPart(string originalstring, string mask, string prefix, char maskIntegralCharacter)
        {
            string fullMask = prefix + mask;

            IEnumerable<int> maskIndices = fullMask.Select((ch, ind) => ch == maskIntegralCharacter && ind >= prefix.Length ? ind : -1).Where(ind => ind >= 0);
            IEnumerable<char> finalString = originalstring.Where((ch, ind) => maskIndices.Contains(ind));
            return new string(finalString.ToArray());
        }

        private static Guid? GetBarcodeTypeOid(BarcodeType barcodeType)
        {
            PropertyInfo property = barcodeType.GetType().GetProperty("Oid");
            if (property != null)
            {
                object val = property.GetValue(barcodeType, null);
                Guid barcodeTypeOid = Guid.Empty;
                if (val != null && Guid.TryParse(val.ToString(), out barcodeTypeOid))
                {
                    return barcodeTypeOid;
                }
            }
            return null;
        }

        private static string GetMaskCharacterString(string originalString, char maskIntegralCharacter, char maskIgnoreCharacter,
            string mask, string prefix, bool removeIgnoredCharacters)
        {
            int maskCharStartIndex = mask.IndexOf(maskIntegralCharacter) >= 0 ? mask.IndexOf(maskIntegralCharacter) + prefix.Length : -1;
            int maskCharEndIndex = mask.LastIndexOf(maskIntegralCharacter) >= 0 ? mask.LastIndexOf(maskIntegralCharacter) + prefix.Length : -1;

            if (maskCharStartIndex > 0 && maskCharEndIndex > 0)
            {
                int strLength = maskCharEndIndex - maskCharStartIndex + 1;
                try
                {
                    string result = originalString.Substring(maskCharStartIndex, strLength);
                    if (removeIgnoredCharacters)
                    {
                        result = GetMaskPart(originalString, mask, prefix, maskIntegralCharacter);
                    }
                    return result;
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        private static bool GetMaskCharacterString(string originalString, char maskIntegralCharacter, char maskDecimalCharacter, string mask, string prefix, out decimal result)
        {
            int maskIntegralCharStartIndex = mask.IndexOf(maskIntegralCharacter) >= 0 ? mask.IndexOf(maskIntegralCharacter) + prefix.Length : -1;
            int maskIntegralCharEndIndex = mask.LastIndexOf(maskIntegralCharacter) >= 0 ? mask.LastIndexOf(maskIntegralCharacter) + prefix.Length : -1;
            int masDecimalCharStartIndex = mask.IndexOf(maskDecimalCharacter) >= 0 ? mask.IndexOf(maskDecimalCharacter) + prefix.Length : -1;
            int masDecimalCharEndIndex = mask.LastIndexOf(maskDecimalCharacter) >= 0 ? mask.LastIndexOf(maskDecimalCharacter) + prefix.Length : -1;

            String integral = "", decim = "";
            if (maskIntegralCharStartIndex > 0 && maskIntegralCharEndIndex > 0)
            {
                int strLength = maskIntegralCharEndIndex - maskIntegralCharStartIndex + 1;
                try
                {
                    integral = originalString.Substring(maskIntegralCharStartIndex, strLength);
                }
                catch
                {
                    integral = "";
                }
            }

            if (masDecimalCharStartIndex > 0 && masDecimalCharEndIndex > 0)
            {
                int strLength = masDecimalCharEndIndex - masDecimalCharStartIndex + 1;
                try
                {
                    decim = originalString.Substring(masDecimalCharStartIndex, strLength);
                }
                catch
                {
                    decim = "";
                }
            }
            Int32 i, d;
            if (Int32.TryParse(integral, out i))
            {
                if (Int32.TryParse(decim, out d) == false)
                {
                    result = i;
                    return true;
                }
                double dbl = d + 0.0;

                dbl /= Math.Pow(10, masDecimalCharEndIndex - masDecimalCharStartIndex + 1);
                result = (decimal)(i + dbl);
                return true;
            }
            else
            {
                result = 0;
                return false;
            }

        }

        public const char MaskQuantityIntegralChar = 'Q';
        public const char MaskQuantityDecimalChar = 'q';
        public const char MaskItemCodeChar = 'I';
        public const char MaskCustomerCodeChar = 'C';
        public const char MaskValueIntegralChar = 'V';
        public const char MaskValueDecimalChar = 'v';
        public const char MaskIgnoreChar = 'X';

        public static List<char> MaskCharacters = new List<char>()
        {
            MaskQuantityIntegralChar,
            MaskItemCodeChar,
            MaskCustomerCodeChar,
            MaskValueIntegralChar,
            MaskQuantityDecimalChar,
            MaskValueDecimalChar,
            MaskIgnoreChar
        };

        /// <summary>
        /// Checks if non special characters (used to determine weight, quantity etc)
        /// exist in the given code and the given mask in the same spots
        /// </summary>
        /// <param name="code">The gieven code</param>
        /// <param name="mask">The given mask</param>
        /// <returns></returns>
        public static bool NonSpecialCharactersMatch(string code, string mask)
        {
            if (code.Length != mask.Length)
            {
                return false;
            }

            for (int index = 0; index < mask.Length; index++)
            {
                if (MaskCharacters.Contains(mask[index]) == false
                    && code[index] != mask[index]
                   )
                {
                    return false;
                }
            }
            return true;
        }


    }
}