using DSO138Device;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;

namespace DSO138_Capture
{
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<DsoData> deviceData = new ObservableCollection<DsoData>();
        private ObservableCollection<DsoDeviceList> deviceList = new ObservableCollection<DsoDeviceList>();
        public  GuiControl guiCtrl = new GuiControl();
        private FileData file = new FileData();
        private DsoDevice dev;

        private static void setTitle(string s)
        {
            var a = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            if (a == null)
                return;
            a.Title = s;
        }

        public MainPage()
        {
            dev = new DsoDevice(ref guiCtrl);
            ///

            if (!SettingsHelper.Found("firstLaunch"))
            {
                ApplicationView.PreferredLaunchViewSize = new Size(1300, 950);
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                SettingsHelper.Set("firstLaunch", true);
            }
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            ///
            try
            {
                bool b;
                string s = SettingsHelper.Get<string>("colorLine");
                if (!String.IsNullOrEmpty(s))
                    guiCtrl.colorLine = (Color)XamlBindingHelper.ConvertValue(typeof(Color), s);
                else
                    guiCtrl.colorLine = Color.FromArgb(0xff, 219, 52, 34);

                s = SettingsHelper.Get<string>("themeSelected");
                if (!String.IsNullOrEmpty(s))
                {
                    if (Boolean.TryParse(s, out b))
                        guiCtrl.isChangeTheme = b;
                    else
                        guiCtrl.isChangeTheme = (RequestedTheme == ElementTheme.Dark);
                }
            }
            catch (Exception) { }
            ///

            this.InitializeComponent();
            guiCtrl.onReadDevice += _onReadDevice;
            guiCtrl.onDeviceList += _onDeviceList;
            guiCtrl.onPlotRefresh += _onPlotRefresh;
            guiCtrl.onMenuRefresh += _onMenuRefresh;
            guiCtrl.isMenuOpen = true;
            guiCtrl.isColorOpen = false;
            guiCtrl.isChangeTheme = (RequestedTheme == ElementTheme.Dark);
            dev.onErrorMessage += _errorMessage;
            Task.Run(async () => await dev.GetAvailPortsAsync());
        }

        ~MainPage()
        {
            dev.End();
            dev.onErrorMessage -= _errorMessage;
            guiCtrl.onReadDevice -= _onReadDevice;
            guiCtrl.onDeviceList -= _onDeviceList;
            guiCtrl.onPlotRefresh -= _onPlotRefresh;
            guiCtrl.onMenuRefresh -= _onMenuRefresh;
        }

        private void _errorMessage(string s)
        {
            try
            {
                ErrorBoxFlyout.Hide();
            }
            catch (Exception) { }

            ErrorBox.Text = s;
            ErrorBoxFlyout.ShowAt(ErrorBoxContainer);
        }

        private void _onPlotRefresh(object sender, bool e)
        {
            xPlot.InvalidatePlot(e);
        }
        private void _onMenuRefresh(object sender, bool e)
        {
            menuList.InvalidateMeasure();
            menuList.InvalidateArrange();
        }

        private void _onDeviceList(object sender, DsoDeviceList e)
        {
            deviceList.Add(e);
            DeviceSelectList.SelectedItem = e;
        }

        private void _onReadDevice(object sender, DsoData e)
        {
            deviceData.Insert(0, e);
            bool b = (guiCtrl.listBadgeCount == 0);
            guiCtrl.listBadgeCount = deviceData.Count;
            if (b)
            {
                try
                {
                    Style badgeEnable = (Style)Application.Current.Resources["BadgeEnable"];
                    if (badgeEnable != null)
                        timeBadge.Style = badgeEnable;
                }
                catch (Exception ex)
                {
                    _errorMessage(ex.Message);
                }
            }
        }

        private void _deviceList_Selected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox box = (sender as ComboBox);
                if (box == null)
                    return;

                DsoDeviceList dso = (box.SelectedItem as DsoDeviceList);
                if ((dso == null) || (String.IsNullOrEmpty(dso.Name)))
                {
                    box.Text = "";
                    guiCtrl.isSerialSelected = false;
                    return;
                }
                guiCtrl.dataDsoDevice = dso;
                guiCtrl.isSerialSelected = true;
                setTitle(dso.Name);
            }
            catch (Exception ex)
            {
                DeviceSelectList.Text = "";
                guiCtrl.isSerialSelected = false;
                _errorMessage(ex.Message);
            }
        }

        private void _theme_ToggleClick(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = (sender as ToggleSwitch);
            if (ts == null)
                return;
            guiCtrl.isChangeTheme = ts.IsOn;
            RequestedTheme = guiCtrl.theme;
        }
        private void _menuList_ToggleClick(object sender, TappedRoutedEventArgs e)
        {
            guiCtrl.isMenuOpen = !guiCtrl.isMenuOpen;
            menuSplit.DisplayMode = (guiCtrl.isMenuOpen) ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.CompactInline;
        }
        private void _menuList_OpenClick(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                if (!guiCtrl.isSerialActive)
                {
                    deviceList.Clear();
                    Task.Run(async () => await dev.GetAvailPortsAsync());
                }
                if (guiCtrl.isMenuOpen)
                    return;
                _menuList_ToggleClick(sender, e);
            }
            catch (Exception ex)
            {
                _errorMessage(ex.Message);
            }
        }

        private void _menuList_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavMenu nvl = (e.ClickedItem as NavMenu);
            if (nvl == null)
                return;

            switch (nvl.Id)
            {
                case 1:
                    {
                        try
                        {
                            if (guiCtrl.isSerialActive)
                                break;
                            if (guiCtrl.dataDsoDevice == null)
                                break;
                            if (String.IsNullOrEmpty(guiCtrl.dataDsoDevice.Name))
                                break;
                            
                            setTitle(guiCtrl.dataDsoDevice.Name);
                            Task.Run(async () => await dev.Begin(guiCtrl.dataDsoDevice.Name));
                            guiCtrl.isSerialActive = true;
                        }
                        catch (Exception ex) {
                            guiCtrl.isSerialActive = false;
                            _errorMessage(ex.Message);
                        }
                        break;
                    }
                case 2:
                    {
                        try
                        {
                            if (!guiCtrl.isSerialActive)
                                break;

                            dev.End();
                            setTitle("");
                        }
                        catch (Exception ex)
                        {
                            _errorMessage(ex.Message);
                        }
                        guiCtrl.isSerialActive = false;
                        break;
                    }
                case 3:
                    {
                        guiCtrl.isChangeTheme = !guiCtrl.isChangeTheme;
                        RequestedTheme = guiCtrl.theme;
                        break;
                    }
                case 4:
                    {
                        guiCtrl.isColorOpen = !guiCtrl.isColorOpen;
                        break;
                    }
                case 5:
                    {
                        file.Open(guiCtrl);
                        break;
                    }
                case 6:
                    {
                        if (guiCtrl.isDsoDataEmpty)
                            return;
                        file.Save(guiCtrl.plotModel, guiCtrl.dataDso);
                        break;
                    }
                case 7:
                    {
                        if (guiCtrl.isSerialActive)
                            break;
                        CoreApplication.Exit();
                        break;
                    }
            }
        }

        private void _timeList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView lv = (sender as ListView);
            if ((lv == null) || (lv.SelectedItem == null))
                return;
            DsoData data = (lv.SelectedItem as DsoData);
            if (data == null)
                return;
            
            guiCtrl.dataDso = data;
        }

        private void _colorLine_ButtonClick(object sender, RoutedEventArgs e)
        {
            guiCtrl.colorLine = ColorLine.Color;
            guiCtrl.isColorOpen = false;
        }

        private void timeSelect_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            TimePicker picker = (sender as TimePicker);
            if (picker == null)
                return;
            TimeSpan time = picker.Time;
            DsoData low = null, hi = null, eq = null, dso = null;

            foreach (var dd in deviceData)
            {
                switch (time.CompareTo(dd.date))
                {
                    case -1:
                        {
                            hi = dd;
                            break;
                        }
                    case 0:
                        {
                            eq = dd;
                            break;
                        }
                    case 1:
                        {
                            low = dd;
                            break;
                        }
                }
                if ((eq != null) || (hi != null))
                    break;
            }
            if (eq != null)
                dso = eq;
            else if (low != null)
                dso = low;
            else if (hi != null)
                dso = hi;
            else
                return;

            timeList.SelectedItem = dso;
            timeList.InvalidateArrange();
            guiCtrl.OnReadDevice(timeList, dso);
        }
    }
}
