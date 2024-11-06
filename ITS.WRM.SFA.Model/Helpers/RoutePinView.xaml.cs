using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITS.WRM.SFA.Model.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePinView : StackLayout
    {
        private string _display;
        private string _color;
        private string _backColor;
        private string _imageName;

        public RoutePinView(string display, string color, string backColor, string imageName)
        {
            InitializeComponent();
            _display = display;
            _color = color;
            _imageName = imageName;
            _backColor = backColor;
            BindingContext = this;
        }

        public string Display
        {
            get { return _display; }
        }

        public string Color
        {
            get { return _color; }
        }

        public string BackColor
        {
            get { return _backColor; }
        }
        public string ImageName
        {
            get { return _imageName; }
        }
    }
}