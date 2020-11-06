using DSO138Device;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DSO138_Capture
{
    public class NavMenu : INotify
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public Symbol Icon { get; set; }
        public Color ColorIcon { 
            get { return colorIcon_;  }
            set {
                if (value != colorIcon_)
                {
                    colorIcon_ = value;
                    OnPropertyChanged("ColorIcon");
                    OnPropertyChanged("BrushIcon");
                }
            }
        }
        public Color ColorText
        {
            get { return colorText_; }
            set
            {
                if (value != colorText_)
                {
                    colorText_ = value;
                    OnPropertyChanged("ColorText");
                    OnPropertyChanged("BrushText");
                }
            }
        }
        public SolidColorBrush BrushIcon
        {
            get { return new SolidColorBrush(ColorIcon);  }
        }
        public SolidColorBrush BrushText
        {
            get { return new SolidColorBrush(ColorText); }
        }

        private Color colorIcon_, colorText_;
    }
}
