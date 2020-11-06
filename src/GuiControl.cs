using DSO138Device;
using OxyPlot;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;

namespace DSO138_Capture
{
    public class GuiControl : INotify
    {
        private DsoData dataDso_ = null;
        private DsoDeviceList dataDsoDevice_ = null;
        private GuiPlot guiPlot_ = new GuiPlot();
        private GuiTheme guiTheme_ = new GuiTheme();

        public delegate void PlotRefresh(object sender, bool e);
        public event PlotRefresh onPlotRefresh = null;
        public event PlotRefresh onMenuRefresh = null;

        public delegate void DsoDeviceReadData(object sender, DsoData e);
        public delegate void DsoDeviceReadList(object sender, DsoDeviceList e);
        public event DsoDeviceReadData onReadDevice = null;
        public event DsoDeviceReadList onDeviceList = null;

        public ObservableCollection<NavMenu> navigtionList = new ObservableCollection<NavMenu>()
        {
            new NavMenu() { Id = 1, Label = getStringResource("Start"), Icon = Windows.UI.Xaml.Controls.Symbol.Play,         ColorIcon = Colors.DarkGray, ColorText = Colors.DarkGray  },
            new NavMenu() { Id = 2, Label = getStringResource("Stop"),  Icon = Windows.UI.Xaml.Controls.Symbol.Stop,         ColorIcon = Colors.DarkGray, ColorText = Colors.DarkGray  },
            new NavMenu() { Id = 3, Label = getStringResource("Theme"), Icon = Windows.UI.Xaml.Controls.Symbol.UnSyncFolder, ColorIcon = Colors.Black, ColorText = Colors.Black  },
            new NavMenu() { Id = 4, Label = getStringResource("Color"), Icon = Windows.UI.Xaml.Controls.Symbol.Highlight,    ColorIcon = Colors.Black, ColorText = Colors.Black  },
            new NavMenu() { Id = 5, Label = getStringResource("Open"),  Icon = Windows.UI.Xaml.Controls.Symbol.OpenFile,     ColorIcon = Colors.Black, ColorText = Colors.Black  },
            new NavMenu() { Id = 6, Label = getStringResource("Save"),  Icon = Windows.UI.Xaml.Controls.Symbol.SaveLocal,    ColorIcon = Colors.DarkGray, ColorText = Colors.DarkGray  },
            new NavMenu() { Id = 7, Label = getStringResource("Exit"),  Icon = Windows.UI.Xaml.Controls.Symbol.Undo,         ColorIcon = Colors.Black, ColorText = Colors.Black  }
        };
        public static string getStringResource(string ids)
        {
            try
            {
                var r = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                if (r == null)
                    return ids;
                return r.GetString(ids + "/Text");
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// 

        public void OnReadDevice(object sender, DsoData e)
        {
            dataDso = e;
            onReadDevice?.Invoke(sender, e);
        }

        public void OnDeviceList(object sender, DsoDeviceList e)
        {
            onDeviceList?.Invoke(sender, e);
        }

        /// 

        public GuiControl()
        {
            guiPlot_.plotFill(colorLine, ref dataDso_);
        }
        

        public bool isDsoDataEmpty
        {
            get { return guiTheme_.isDsoDataEmpty; }
            set
            {
                if (value != guiTheme_.isDsoDataEmpty)
                {
                    guiTheme_.isDsoDataEmpty = value;
                    OnPropertyChanged("isDsoDataEmpty");
                }
            }
        }

        public bool isMenuOpen
        {
            get { return guiTheme_.isMenuOpen; }
            set
            {
                if (value != guiTheme_.isMenuOpen)
                {
                    guiTheme_.isMenuOpen = value;
                    OnPropertyChanged("isMenuOpen");
                }
            }
        }

        public bool isColorOpen
        {
            get { return guiTheme_.isColorOpen; }
            set
            {
                if (value != guiTheme_.isColorOpen)
                {
                    guiTheme_.isColorOpen = value;
                    OnPropertyChanged("isColorOpen");
                    OnPropertyChanged("visibilityColor");
                    OnPropertyChanged("visibilityGraph");
                }
            }
        }

        public bool isSerialActive
        {
            get { return guiTheme_.isSerialActive; }
            set
            {
                if (value != guiTheme_.isSerialActive)
                {
                    guiTheme_.isSerialActive = value;
                    OnPropertyChanged("isSerialActive");

                    guiTheme_.chageMenuTheme(ref navigtionList);
                    OnPropertyChanged("navigtionList");
                    onMenuRefresh?.Invoke(this, true);
                }
            }
        }

        public bool isSerialSelected
        {
            get { return guiTheme_.isSerialSelected; }
            set
            {
                if (value != guiTheme_.isSerialSelected)
                {
                    guiTheme_.isSerialSelected = value;
                    OnPropertyChanged("isSerialSelected");

                    guiTheme_.chageMenuTheme(ref navigtionList);
                    OnPropertyChanged("navigtionList");
                    onMenuRefresh?.Invoke(this, true);
                }
            }
        }

        public bool isChangeTheme
        {
            get { return guiTheme_.isChangeTheme; }
            set
            {
                if (value != guiTheme_.isChangeTheme)
                {
                    guiTheme_.isChangeTheme = value;
                    OnPropertyChanged("theme");
                    OnPropertyChanged("isChangeTheme");

                    guiPlot_.plotChangeTheme(value);
                    OnPropertyChanged("plotModel");
                    onPlotRefresh?.Invoke(this, true);

                    guiTheme_.chageMenuTheme(ref navigtionList);
                    OnPropertyChanged("navigtionList");
                    onMenuRefresh?.Invoke(this, true);

                    SettingsHelper.Set("themeSelected", guiTheme_.isChangeTheme);
                }
            }
        }

        public ElementTheme theme
        {
            get
            {
                return (guiTheme_.isChangeTheme) ?
                    ElementTheme.Dark : ElementTheme.Light;
            }
        }

        public Visibility visibilityColor
        {
            get { return (guiTheme_.isColorOpen) ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Visibility visibilityGraph
        {
            get { return (!guiTheme_.isColorOpen) ? Visibility.Visible : Visibility.Collapsed; }
            set { }
        }

        public Color colorLine
        {
            get { return guiTheme_.colorLine; }
            set
            {
                if (value != guiTheme_.colorLine)
                {
                    guiTheme_.colorLine = value;
                    guiPlot_.plotFill(guiTheme_.colorLine, ref dataDso_);
                    OnPropertyChanged("colorLine");
                    OnPropertyChanged("plotModel");
                    onPlotRefresh?.Invoke(this, true);

                    SettingsHelper.Set("colorLine", guiTheme_.colorLine.ToString());
                }
            }
        }

        public DsoData dataDso
        {
            get { return dataDso_; }
            set
            {
                if (value != dataDso_)
                {
                    dataDso_ = value;
                    OnPropertyChanged("dataDso");
                    guiPlot_.plotSetSeries(dataDso_.plot.series, dataDso_.DateCreated);
                    guiPlot_.plotSetAnnotations(guiTheme_.isChangeTheme, ref dataDso_);
                    OnPropertyChanged("plotModel");
                    onPlotRefresh?.Invoke(this, true);

                    isDsoDataEmpty = (dataDso_ == null);
                    guiTheme_.chageMenuTheme(ref navigtionList);
                    OnPropertyChanged("navigtionList");
                    onMenuRefresh?.Invoke(this, true);
                }
            }
        }

        public DsoDeviceList dataDsoDevice
        {
            get { return dataDsoDevice_; }
            set
            {
                if (value != dataDsoDevice_)
                {
                    dataDsoDevice_ = value;
                    OnPropertyChanged("dataDsoDevice");
                }
            }
        }

        public PlotModel plotModel
        {
            get { return guiPlot_.plotModel; }
            set { }
        }
    }
}
