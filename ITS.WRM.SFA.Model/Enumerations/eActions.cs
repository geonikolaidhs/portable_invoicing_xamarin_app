using ITS.WRM.SFA.Resources;
using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Enumerations
{
    //[Editor(typeof(EnumGridComboBox), typeof(UITypeEditor))]
    //[EnumList(typeof(eActions))]
    public enum eActions
    {
        [WrmDisplay(Name = "SHUTDOWN", ResourceType = typeof(ResourcesRest))]
        SHUTDOWN = -10,  //external
        
        [WrmDisplay(Name = "NONEACTION", ResourceType = typeof(ResourcesRest))]
        NONE = 0,      //external
        
        [WrmDisplay(Name = "CHECKPRICE", ResourceType = typeof(ResourcesRest))]
        CHECKPRICE = 1,  //external ελεγχος τιμης
        //KEYMAPPINGS=2, //external obsolete
        //CONFIG=3,  //external  obsolete
        
        [WrmDisplay(Name = "FIND_ITEM", ResourceType = typeof(ResourcesRest))]
        FIND_ITEM = 4, //external ευρεση ειδους
        
        [WrmDisplay(Name = "DELETE_ITEM", ResourceType = typeof(ResourcesRest))]
        DELETE_ITEM = 5, //external διαγραφη γραμμης
        
        [WrmDisplay(Name = "MOVE_UP", ResourceType = typeof(ResourcesRest))]
        MOVE_UP = 6, //external πανω
        
        [WrmDisplay(Name = "MOVE_DOWN", ResourceType = typeof(ResourcesRest))]
        MOVE_DOWN = 7,  //external κατω
        
        [WrmDisplay(Name = "MULTIPLY_QTY", ResourceType = typeof(ResourcesRest))]
        MULTIPLY_QTY = 8,  //external πολλαπλασιαστης ποσοτητας
        
        [WrmDisplay(Name = "DOCUMENT_TOTAL", ResourceType = typeof(ResourcesRest))]
        DOCUMENT_TOTAL = 9, //external συνολο
        //ADD_PAYMENT = 10, //MOVED TO INTERNAL: 100021
        [WrmDisplay(Name = "DELETE_PAYMENT", ResourceType = typeof(ResourcesRest))]
        DELETE_PAYMENT = 11, //external //διαγραφη πληρωμης
        
        [WrmDisplay(Name = "ISSUE_Z", ResourceType = typeof(ResourcesRest))]
        ISSUE_Z = 12, //external εκδοση ζ
        
        [WrmDisplay(Name = "CANCEL_DOCUMENT", ResourceType = typeof(ResourcesRest))]
        CANCEL_DOCUMENT = 13, //external ακυρωση αποδειξης/παραστατικου
        
        [WrmDisplay(Name = "START_FISCAL_DAY", ResourceType = typeof(ResourcesRest))]
        START_FISCAL_DAY = 14,  //external εναρξη ημερας
        
        [WrmDisplay(Name = "START_SHIFT", ResourceType = typeof(ResourcesRest))]
        START_SHIFT = 15, //external εναρξη βαρδειας
        
        [WrmDisplay(Name = "ISSUE_X", ResourceType = typeof(ResourcesRest))]
        ISSUE_X = 16,  //external εκδοση χ
        
        [WrmDisplay(Name = "CANCEL_POS", ResourceType = typeof(ResourcesRest))]
        CANCEL = 17, //external ακυρωση
        
        [WrmDisplay(Name = "BACKSPACE", ResourceType = typeof(ResourcesRest))]
        BACKSPACE = 18, //external 
        
        [WrmDisplay(Name = "ADD_LINE_DISCOUNT", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("DiscountTypeCode", typeof(string))]
        ADD_LINE_DISCOUNT = 19, //external εκπωση γραμμης
        
        [WrmDisplay(Name = "ADD_DOCUMENT_DISCOUNT", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("DiscountTypeCode", typeof(string))]
        ADD_DOCUMENT_DISCOUNT = 20, //external εκπτωση συνολου
        
        [WrmDisplay(Name = "FIND_SCAN_CODE", ResourceType = typeof(ResourcesRest))]
        FIND_SCAN_CODE = 21,  //external 
        //GET_ITEM_PRICE = 22, //MOVED TO INTERNAL : 100022
        
        [WrmDisplay(Name = "USE_PROFORMA", ResourceType = typeof(ResourcesRest))]
        USE_PROFORMA = 30, //external προτιμολογιο
        
        [WrmDisplay(Name = "PREVIEW_X_REPORT", ResourceType = typeof(ResourcesRest))]
        PREVIEW_X_REPORT = 31, //external προεπισκοπηση αναφορας χ
        
        [WrmDisplay(Name = "OPEN_DRAWER", ResourceType = typeof(ResourcesRest))]
        OPEN_DRAWER = 32, //external ανοιγμα συρταριου
        
        [WrmDisplay(Name = "DISPLAY_WITHDRAWAL", ResourceType = typeof(ResourcesRest))]
        DISPLAY_WITHDRAWAL = 33, //external αναληψη
        
        [WrmDisplay(Name = "DISPLAY_DEPOSIT", ResourceType = typeof(ResourcesRest))]
        DISPLAY_DEPOSIT = 34, //external καταθεση
        
        [WrmDisplay(Name = "RETURN_ITEM", ResourceType = typeof(ResourcesRest))]
        RETURN_ITEM = 35, //external επιστροφη
        
        [WrmDisplay(Name = "RESTART", ResourceType = typeof(ResourcesRest))]
        RESTART = 36,  //external επανεκκινηση υπολογιστη
        
        [WrmDisplay(Name = "CHANGE_ITEM_PRICE", ResourceType = typeof(ResourcesRest))]
        CHANGE_ITEM_PRICE = 37, //external αλλαγη τιμης
        
        [WrmDisplay(Name = "APPLICATION_EXIT", ResourceType = typeof(ResourcesRest))]
        APPLICATION_EXIT = 38, //external τερματισμος εφαρμογης
        
        [WrmDisplay(Name = "ADD_PAYMENT", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("PaymentMethodCode", typeof(string))]
        ADD_PAYMENT = 39,  //external πληρωμη
        
        [WrmDisplay(Name = "ADD_ITEM", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("ItemCode", typeof(string))]
        ADD_ITEM = 40,//external ειδος
        
        [WrmDisplay(Name = "ADD_PAYMENT_FROM_FORM", ResourceType = typeof(ResourcesRest))]
        ADD_PAYMENT_FROM_FORM = 41,//external πληρωμη με επιλογη
        
        [WrmDisplay(Name = "ADD_TOTAL_PAYMENT", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("PaymentMethodCode", typeof(string))]
        ADD_TOTAL_PAYMENT = 42,//external συνολικη πληρωμη
        
        [WrmDisplay(Name = "ADD_TOTAL_PAYMENT_FROM_FORM", ResourceType = typeof(ResourcesRest))]
        ADD_TOTAL_PAYMENT_FROM_FORM = 43,//external συνολικη πληρωμη με επιλογη
        
        [WrmDisplay(Name = "DISPLAY_RETURN_ITEM_FORM", ResourceType = typeof(ResourcesRest))]
        DISPLAY_RETURN_ITEM_FORM = 44,//external επιστροφη με επιλογη

        [WrmDisplay(Name = "STRESS_TEST", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("NumberOfReceipts", typeof(int))]
        [ActionKeybindParameter("ItemsPerReceipt", typeof(int))]
        [ActionKeybindParameter("RandomCustomer", typeof(bool))]
        [ActionKeybindParameter("RandomPayment", typeof(bool))]
        [ActionKeybindParameter("RandomCancelLines", typeof(bool))]
        [ActionKeybindParameter("RandomCancelDocument", typeof(bool))]
        [ActionKeybindParameter("RandomProforma", typeof(bool))]
        STRESS_TEST = 45,//external 

        [WrmDisplay(Name = "FISCAL_PRINTER_REPRINT_Z_REPORTS", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("UseDateFilter", typeof(bool))]
        [ActionKeybindParameter("Mode", typeof(eReprintZReportsMode))]
        FISCAL_PRINTER_REPRINT_Z_REPORTS = 46,//external Φορ. Εκτ επανεκτυπωση Ζ
        
        [WrmDisplay(Name = "REPRINT_RECEIPTS", ResourceType = typeof(ResourcesRest))]
        REPRINT_RECEIPTS = 47, //external επανεκτυπωση αποδειξεων
        
        [WrmDisplay(Name = "FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS", ResourceType = typeof(ResourcesRest))]
        FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS = 48, //external 
        
        [WrmDisplay(Name = "SERVICE_FORCED_CANCEL_DOCUMENT", ResourceType = typeof(ResourcesRest))]
        SERVICE_FORCED_CANCEL_DOCUMENT = 49, //external 
        
        [WrmDisplay(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(ResourcesRest))]
        SERVICE_FORCED_START_DAY_FISCAL_PRINTER = 50, //external 
        
        [WrmDisplay(Name = "ADD_ITEM_WEIGHTED", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("ItemCode", typeof(string))]
        ADD_ITEM_WEIGHTED = 51, //external ζυγιζομενο ειδος με ληψη βαρους
        
        [WrmDisplay(Name = "ADD_LINE_DISCOUNT_FROM_FORM", ResourceType = typeof(ResourcesRest))]
        ADD_LINE_DISCOUNT_FROM_FORM = 52, //external προσθηκη εκπτωσης γραμμης με επιλογη
        
        [WrmDisplay(Name = "ADD_DOCUMENT_DISCOUNT_FROM_FORM", ResourceType = typeof(ResourcesRest))]
        ADD_DOCUMENT_DISCOUNT_FROM_FORM = 53, //external προσθηκη εκπτωσης συνολου με επιλογη
        
        [WrmDisplay(Name = "ADD_CUSTOMER", ResourceType = typeof(ResourcesRest))]
        ADD_CUSTOMER = 54, //external πελατης
        
        [WrmDisplay(Name = "CANCEL_DISCOUNT", ResourceType = typeof(ResourcesRest))]
        CANCEL_DISCOUNT = 55, //external ακυρωση εκπτωσης
        
        [WrmDisplay(Name = "SET_FISCAL_ON_ERROR", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("SetFiscalOnError", typeof(bool))]
        SET_FISCAL_ON_ERROR = 56, //θεση φορ μηχ σε βλαβη
        
        [WrmDisplay(Name = "SET_DOCUMENT_ON_HOLD", ResourceType = typeof(ResourcesRest))]
        SET_DOCUMENT_ON_HOLD = 57, // σε αναμονη
        
        [WrmDisplay(Name = "GET_DOCUMENT_FROM_HOLD", ResourceType = typeof(ResourcesRest))]
        GET_DOCUMENT_FROM_HOLD = 58, // απο αναμονη
        
        [WrmDisplay(Name = "SET_OR_GET_DOCUMENT_ON_HOLD", ResourceType = typeof(ResourcesRest))]
        SET_OR_GET_DOCUMENT_ON_HOLD = 59, // σε 'η  απο αναμονη
        
        [WrmDisplay(Name = "GENERIC_CANCEL", ResourceType = typeof(ResourcesRest))]
        GENERIC_CANCEL = 60,
        
        [WrmDisplay(Name = "ACTION_PAUSE", ResourceType = typeof(ResourcesRest))]
        PAUSE = 61,

        [WrmDisplay(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(ResourcesRest))]
        SERVICE_START_LEGAL_RECEIPT_FISCAL_PRINTER = 62, //external 

        [WrmDisplay(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(ResourcesRest))]
        SERVICE_RESTORE_FISCAL_PRINTER = 63,

        [WrmDisplay(Name = "USE_DEFAULT_DOCUMENT_TYPE", ResourceType = typeof(ResourcesRest))]
        USE_DEFAULT_DOCUMENT_TYPE = 64,

        [WrmDisplay(Name = "DISPLAY_VAT_FACTORS", ResourceType = typeof(ResourcesRest))]
        DISPLAY_VAT_FACTORS = 65,

        [WrmDisplay(Name = "SET_STANDALONE_FISCAL_ON_ERROR", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("SetStandaloneFiscalOnError", typeof(bool))]
        SET_STANDALONE_FISCAL_ON_ERROR = 66,

        [WrmDisplay(Name = "FORCE_DELETE_PAYMENT", ResourceType = typeof(ResourcesRest))]
        FORCE_DELETE_PAYMENT = 67,

        [WrmDisplay(Name = "EDPS_CHECK_COMMUNICATION", ResourceType = typeof(ResourcesRest))]
        EDPS_CHECK_COMMUNICATION = 68,

        [WrmDisplay(Name = "ADD_COUPON", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("CouponCode", typeof(string))]
        ADD_COUPON = 69,

        [WrmDisplay(Name = "ISSUE_Z_EAFDSS", ResourceType = typeof(ResourcesRest))]
        ISSUE_Z_EAFDSS = 70,

        [WrmDisplay(Name = "DATABASE_MAINTENANCE", ResourceType = typeof(ResourcesRest))]
        DATABASE_MAINTENANCE = 71,

        [WrmDisplay(Name = "DATABASE_MAINTENANCE_LIGHT", ResourceType = typeof(ResourcesRest))]
        DATABASE_MAINTENANCE_LIGHT = 72,


        [WrmDisplay(Name = "USE_DOCUMENT_TYPE", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("DocumentTypeCode", typeof(string))]
        USE_DOCUMENT_TYPE = 73,

        [WrmDisplay(Name = "USE_SPECIAL_PROFORMA", ResourceType = typeof(ResourcesRest))]
        USE_SPECIAL_PROFORMA = 74, //external ειδικό προτιμολόγιο

        PRINTER_FEED = 75,

        [WrmDisplay(Name = "RESET_EAFDSS_DEVICES_ORDER", ResourceType = typeof(ResourcesRest))]
        RESET_EAFDSS_DEVICES_ORDER = 80,
        [WrmDisplay(Name = "FIND_ITEM_BY_DESCRIPTION", ResourceType = typeof(ResourcesRest))]
        FIND_ITEM_BY_DESCRIPTION = 76,
        [WrmDisplay(Name = "CALL_OTHER_ACTION", ResourceType = typeof(ResourcesRest))]
        CALL_OTHER_ACTION = 1000, // special externals κληση αλλης εντολης
        [WrmDisplay(Name = "USE_OPOS_REPORT", ResourceType = typeof(ResourcesRest))]
        [ActionKeybindParameter("PosReportCode", typeof(string))]
        USE_OPOS_REPORT = 81,
        //Internal Actions > 100000
        KEYBOARD = 100001,  //internal
        //ADD_ITEM = 100002, //internal obsolete
        OPEN_SCANNERS = 100003,  //internal
        START_NEW_DOCUMENT = 100004,  //internal
        CLOSE_DOCUMENT = 100005, //internal
        PUBLISH_DOCUMENT_INFO = 100006,  //internal
        PRINT_RECEIPT = 100007,  //internal
        CUSTOMER_POLE_DISPLAY_MESSAGE = 100008, //internal
        PUBLISH_DOCUMENT_LINE_INFO = 100009,  //internal
        ADD_CUSTOMER_INTERNAL = 100010, //internal
        LOAD_EXISTING_DOCUMENT = 100011,  //internal
        PUBLISH_DOCUMENT_PAYMENT_INFO = 100012,  //internal
        SHOW_ERROR = 100013,  //internal
        PUBLISH_LINE_QUANTITY_INFO = 100014,  //internal
        UPDATE_COMMUNICATION_STATUS = 100015,  //internal
        PUBLISH_KEY_PRESS = 100016,  //internal
        POST_STATUS = 100017,  //internal
        DISPLAY_TOUCH_PAD = 100018,  //internal
        HIDE_TOUCH_PAD = 100019,  //internal
        ADD_ITEM_INTERNAL = 100020,  //internal
        //DEPRECATED_ADD_PAYMENT = 100021,  //MOVED TO EXTERNAL : 39
        //GET_ITEM_PRICE = 100022, //internal
        PUBLISH_MACHINE_STATUS = 100023, //internal
        PUBLISH_DOCUMENT_QUANTITY = 100024, //internal
        FISCAL_PRINTER_PRINT_RECEIPT = 100025,  //internal
        SLIP_PRINT = 100026, // internal
        LOAD_EXISTING_DOCUMENTS_ON_HOLD = 100029,//internal
        CASHIER_POLE_DISPLAY_MESSAGE = 100030, //internal
        EDPS_BATCH_CLOSE = 100130, //internal
        CHECK_STATUS_WITH_FISCAL_PRINTER = 100131,
        CANCEL_NOT_INCLUDED_ITEMS = 100135, //Creates the form where the not included items in new price catalog are listed
        SHOW_BLINKING_ERROR = 100137,//internal
        PRINT_DOCUMENT_TO_WINDOWS_PRINTER = 100181,//internal
        PRINT_REPORT_TO_OPOS_PRINTER = 100182//internal
    }


    /*
    public abstract class GridComboBox : UITypeEditor
    {
        private const string StrAddNew = "<Add New...>";


        private IList _dataList;
        private readonly ListBox _listBox;
        private Boolean _escKeyPressed;
        private ListAttribute _listAttribute;
        private IWindowsFormsEditorService _editorService;


        /// <summary>
        /// Constructor
        /// </summary>
        public GridComboBox()
        {
            _listBox = new ListBox();

            // Properties
            _listBox.BorderStyle = BorderStyle.None;

            // Events
            _listBox.Click += myListBox_Click;
            _listBox.PreviewKeyDown += myListBox_PreviewKeyDown;
        }


        /// <summary>
        /// Get/Set for ListBox
        /// </summary>
        protected ListBox ListBox
        {
            get { return (_listBox); }
        }

        /// <summary>
        /// Get/Set for DataList
        /// </summary>
        protected IList DataList
        {
            get { return (_dataList); }
            set { _dataList = value; }
        }

        /// <summary>
        /// Get/Set for ListAttribute
        /// </summary>
        protected ListAttribute ListAttribute
        {
            get { return (_listAttribute); }
            set { _listAttribute = value; }
        }


        /// <summary>
        /// Close DropDown window to finish editing
        /// </summary>
        public void CloseDropDownWindow()
        {
            if (_editorService != null)
                _editorService.CloseDropDown();
        }


        /// <summary>
        /// Populate the ListBox with data items
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentValue"></param>
        private void PopulateListBox(ITypeDescriptorContext context, Object currentValue)
        {
            // Clear List
            _listBox.Items.Clear();

            // Retrieve the reference to the items to be displayed in the list
            if (_dataList == null)
                RetrieveDataList(context);

            if (_dataList != null)
            {
                if ((_listAttribute is IAddNew) && (((IAddNew)_listAttribute).AddNew))
                    _listBox.Items.Add(StrAddNew);

                // Add Items to the ListBox
                foreach (object obj in _dataList)
                {
                    _listBox.Items.Add(obj);
                }

                // Select current item 
                if (currentValue != null)
                    _listBox.SelectedItem = currentValue;
            }

            // Set the height based on the Items in the ListBox
            _listBox.Height = _listBox.PreferredHeight;
        }


        /// <summary>
        /// Get the object selected in the ComboBox
        /// </summary>
        /// <returns>Selected Object</returns>
        protected abstract object GetDataObjectSelected(ITypeDescriptorContext context);

        /// <summary>
        /// Find the list of data items to populate the ListBox
        /// </summary>
        /// <param name="context"></param>
        protected abstract void RetrieveDataList(ITypeDescriptorContext context);


        /// <summary>
        /// Preview Key Pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myListBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _escKeyPressed = true;
        }

        /// <summary>
        /// ListBox Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myListBox_Click(object sender, EventArgs e)
        {
            //when user clicks on an item, the edit process is done.
            this.CloseDropDownWindow();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                //Uses the IWindowsFormsEditorService to display a 
                // drop-down UI in the Properties window:
                _editorService = provider.GetService(
                                    typeof(IWindowsFormsEditorService))
                                 as IWindowsFormsEditorService;

                if (_editorService != null)
                {
                    // Add Values to the ListBox
                    PopulateListBox(context, value);

                    // Set to false before showing the control
                    _escKeyPressed = false;

                    // Attach the ListBox to the DropDown Control
                    _editorService.DropDownControl(_listBox);

                    // User pressed the ESC key --> Return the Old Value
                    if (!_escKeyPressed)
                    {
                        // Get the Selected Object
                        object obj = GetDataObjectSelected(context);

                        // If an Object is Selected --> Return it
                        if (obj != null)
                            return (obj);
                    }
                }
            }

            return (value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return (UITypeEditorEditStyle.DropDown);
        }

    }


    public class EnumGridComboBox : GridComboBox
    {

        /// <summary>
        /// Get the object selected in the ComboBox
        /// </summary>
        /// <returns>Selected Object</returns>
        protected override object GetDataObjectSelected(ITypeDescriptorContext context)
        {
            return (base.ListBox.SelectedItem);
        }

        /// <summary>
        /// Find the list of data items to populate the ComboBox
        /// </summary>
        /// <param name="context"></param>
        protected override void RetrieveDataList(ITypeDescriptorContext context)
        {
            // Find the Attribute that has the path to the Enumerations list
            foreach (Attribute attribute in context.PropertyDescriptor.Attributes)
            {
                if (attribute is EnumListAttribute)
                {
                    base.ListAttribute = attribute as EnumListAttribute;
                    break;
                }
            }

            // If we found the Attribute, find the Data List
            if (base.ListAttribute != null)
            {
                // Save the DataList
                Type enumType = ((EnumListAttribute)base.ListAttribute).EnumType;
                switch (enumType.Name)
                {

                    case "eActions":
                        ////Get all the actions that are external and do not require keybound parameters.
                        base.DataList = Enum.GetValues(enumType).Cast<eActions>().Where(ac => (int)ac < 10000 && ac.GetActionKeybindParameters().Count == 0).OrderBy(v => v.ToString()).ToList();
                        break;
                    default:
                        base.DataList = Enum.GetValues(enumType);
                        break;
                }



            }
        }
    }

    public class EnumListAttribute : ListAttribute
    {

        private readonly Type _enumType;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">List of items for display in the GridComboBox</param>
        public EnumListAttribute(Type enumType)
        {
            if (enumType.BaseType == typeof(Enum))
                this._enumType = enumType;
            else
                throw new ArgumentException("Argument must be of type Enum");
        }

        /// <summary>
        /// Get/Set for EnumType
        /// </summary>
        public Type EnumType
        {
            get { return (_enumType); }
        }

    }

    public abstract class ListAttribute : Attribute
    {
    }

    public interface IAddNew
    {
        bool AddNew { get; }
    }*/
}
