using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace ITS.WRM.SFA.Droid.Classes.DocumentFormat
{
    public class ReceiptSchema
    {
        public ReceiptPart ReceiptHeader { get; set; }
        public ReceiptPart ReceiptBody { get; set; }
        public ReceiptPart ReceiptFooter { get; set; }

        public void LoadFromXml(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            LoadXmlDocument(doc);
        }
        public void LoadFromXmlString(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            LoadXmlDocument(doc);
        }

        private void LoadXmlDocument(XmlDocument doc)
        {
            XmlNodeList headerList = doc.GetElementsByTagName("Header");
            XmlNodeList bodyList = doc.GetElementsByTagName("Body");
            XmlNodeList footerList = doc.GetElementsByTagName("Footer");
            if (headerList.Count != 1 || headerList.Count != 1 || footerList.Count != 1)
            {
                throw new Exception(ResourcesRest.InvalidSchema);
            }
            XmlNode xmlHeader = headerList[0];
            XmlNode xmlBody = bodyList[0];
            XmlNode xmlFooter = footerList[0];
            ReceiptHeader = ParseReceiptPart(xmlHeader);
            ReceiptBody = ParseReceiptPart(xmlBody);
            ReceiptFooter = ParseReceiptPart(xmlFooter);
        }

        protected ReceiptElement ParseReceiptElement(XmlNode xmlNode)
        {
            ReceiptElement element = null;
            switch (xmlNode.Name.ToUpper())
            {

                case "SIMPLELINE":
                    element = new SimpleLine(xmlNode.InnerText);
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        switch (attr.Name.ToUpper())
                        {
                            case "ALIGN":
                                (element as SimpleLine).LineAlignment = (eAlignment)Enum.Parse(typeof(eAlignment), attr.Value.ToUpper());
                                break;
                            case "BOLD":
                                (element as SimpleLine).IsBold = bool.Parse(attr.Value);
                                break;
                            case "MAXCHARS":
                                (element as SimpleLine).MaxCharacters = uint.Parse(attr.Value);
                                break;
                            case "CONDITION":
                                (element as SimpleLine).Condition = (eCondition)Enum.Parse(typeof(eCondition), attr.Value.ToUpper());
                                break;
                            case "SOURCE":
                                (element as SimpleLine).Source = (eSource)Enum.Parse(typeof(eSource), attr.Value.ToUpper());
                                break;
                        }
                    }
                    break;
                case "DYNAMICLINE":
                    element = new DynamicLine();
                    foreach (XmlNode xmlCell in xmlNode.ChildNodes)
                    {
                        if (xmlCell.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (xmlCell.Name.ToUpper() == "CELL")
                        {
                            DynamicLineCell cell = new DynamicLineCell();
                            cell.Content = xmlCell.InnerText;
                            foreach (XmlAttribute attr in xmlCell.Attributes)
                            {
                                switch (attr.Name.ToUpper())
                                {
                                    case "ALIGN":
                                        cell.CellAlignment = (eAlignment)Enum.Parse(typeof(eAlignment), attr.Value.ToUpper());
                                        break;
                                    case "BOLD":
                                        cell.IsBold = bool.Parse(attr.Value);
                                        break;
                                    case "MAXCHARS":
                                        cell.MaxCharacters = uint.Parse(attr.Value);
                                        break;
                                }
                            }
                            (element as DynamicLine).Cells.Add(cell);
                        }
                    }
                    foreach (XmlAttribute attr in xmlNode.Attributes)
                    {
                        switch (attr.Name.ToUpper())
                        {
                            case "ALIGN":
                                (element as DynamicLine).LineAlignment = (eAlignment)Enum.Parse(typeof(eAlignment), attr.Value.ToUpper());
                                break;
                            case "BOLD":
                                (element as DynamicLine).IsBold = bool.Parse(attr.Value);
                                break;
                            case "MAXCHARS":
                                (element as DynamicLine).MaxCharacters = uint.Parse(attr.Value);
                                break;
                            case "CONDITION":
                                (element as DynamicLine).Condition = (eCondition)Enum.Parse(typeof(eCondition), attr.Value.ToUpper());
                                break;
                            case "SOURCE":
                                (element as DynamicLine).Source = (eSource)Enum.Parse(typeof(eSource), attr.Value.ToUpper());
                                break;
                        }
                    }
                    break;
                case "DETAIL":
                    element = new DetailGroup();
                    foreach (XmlNode detailNode in xmlNode.ChildNodes)
                    {
                        if (detailNode.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        ReceiptElement childElement = ParseReceiptElement(detailNode);
                        (element as DetailGroup).Lines.Add(childElement);

                    }
                    break;
            }

            foreach (XmlAttribute attr in xmlNode.Attributes)
            {
                switch (attr.Name.ToUpper())
                {
                    case "SOURCE":
                        element.Source = (eSource)Enum.Parse(typeof(eSource), attr.Value.ToUpper());
                        break;
                }
            }

            return element;
        }

        protected ReceiptPart ParseReceiptPart(XmlNode xmlPart)
        {
            ReceiptPart receiptPart = new ReceiptPart();
            foreach (XmlNode xmlNode in xmlPart.ChildNodes)
            {
                if (xmlNode.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                ReceiptElement element = ParseReceiptElement(xmlNode);
                receiptPart.Elements.Add(element);
            }
            return receiptPart;
        }
    }
}
