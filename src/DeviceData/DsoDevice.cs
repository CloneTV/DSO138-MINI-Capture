using DSO138_Capture;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace DSO138Device
{
    class DsoDevice
    {
        private SerialDevice dev = null;
        private DataReader reader = null;
        private CancellationTokenSource cancellsrc = null;
        private GuiControl guiCtrl;

        public delegate void ErrorMessage(string s);
        public event ErrorMessage onErrorMessage = null;

        public DsoDevice(ref GuiControl gui) {
            guiCtrl = gui;
        }
        ~DsoDevice() {
            destruct();
        }

        public async Task GetAvailPortsAsync()
        {
            try
            {
                var d = await DeviceInformation.FindAllAsync(
                    SerialDevice.GetDeviceSelector()
                );
                if (d == null)
                    throw new Exception("Port deviceList  not open, permission?");

                for (int i = 0; i < d.Count; i++)
                {
                    if ((d[i] == null) || (String.IsNullOrEmpty(d[i].Name)))
                        continue;
                    if ((d[i].IsEnabled) && (d[i].Name.Contains("(COM")))
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            guiCtrl.OnDeviceList(this, new DsoDeviceList(d[i].Name, d[i].Id));
                        });
                }
            }
            catch (Exception e) {
                throwMessage(e.Message);
            }
        }

        public async Task Begin(string did)
        {
            try
            {
                destruct();

                int id = -1;
                var d = await DeviceInformation.FindAllAsync(
                    SerialDevice.GetDeviceSelector()
                );
                if (d == null)
                    throw new Exception("Port deviceList  not open, permission?");

                for (int i = 0; i < d.Count; i++)
                {
                    if ((d[i] == null) || (String.IsNullOrEmpty(d[i].Name)))
                        continue;
                    if (d[i].Name.Contains(did))
                    {
                        if (String.IsNullOrEmpty(d[i].Id))
                            break;
                        id = i;
                        break;
                    }
                }
                if (id == -1)
                    throw new Exception("Port " + did + " not found");

                dev = await SerialDevice.FromIdAsync(d[id].Id);
                if (dev == null)
                    throw new Exception("Port " + did + " error open from ID");

                dev.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                dev.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                dev.BaudRate = 115200;
                dev.Parity = SerialParity.None;
                dev.StopBits = SerialStopBitCount.One;
                dev.DataBits = 8;

                reader = new DataReader(dev.InputStream);
                if (reader == null)
                    throw new Exception("Port " + did + " not open for read..");

                cancellsrc = new CancellationTokenSource();
                guiCtrl.isSerialActive = true;

                while (true)
                {
                    await readData();
                    if ((cancellsrc == null) || (dev == null) || (cancellsrc.Token.IsCancellationRequested))
                        break;
                }
                destruct();
                guiCtrl.isSerialActive = false;
            }
            catch (Exception e)
            {
                throwMessage(e.Message);
            }
        }

        public void End()
        {
            try
            {
                if ((cancellsrc != null) && (!cancellsrc.IsCancellationRequested))
                    cancellsrc.Cancel();
            }
            catch (Exception e)
            {
                throwMessage(e.Message);
            }
        }

        public async void Loop()
        {
            try
            {
                if (dev != null)
                    throw new Exception("Port device not opened");
                if (reader != null)
                    throw new Exception("Port reader not initialized");

                cancellsrc = new CancellationTokenSource();
                guiCtrl.isSerialActive = true;

                while (true)
                {
                    await readData();
                    if ((cancellsrc.Token.IsCancellationRequested) || (dev == null))
                        break;
                }
                destruct();
                guiCtrl.isSerialActive = false;
            }
            catch (Exception e)
            {
                throwMessage(e.Message);
            }
        }

        private async void throwMessage(string s)
        {
            guiCtrl.isSerialActive = false;
            if (onErrorMessage == null)
                return;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                onErrorMessage(s);
            });
        }

        private void destruct()
        {
            try
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
                if (dev != null)
                {
                    dev.Dispose();
                    dev = null;
                }
                if (cancellsrc != null)
                {
                    cancellsrc.Dispose();
                    cancellsrc = null;
                }
            }
            catch (Exception) { }
        }

        private async Task readData()
        {
            try
            {
                const int RSIZE = 65536;
                StringBuilder sb = new StringBuilder(RSIZE);
                UInt32 b = 0;

                do
                {
                    b = await reader.LoadAsync(RSIZE).AsTask();
                    if (b == 0)
                        return;

                    byte[] bytes = new byte[b];
                    reader.ReadBytes(bytes);

                    if (guiCtrl.isSerialActive)
                        sb.Append(
                            Encoding.ASCII.GetString(bytes, 0, bytes.Length)
                        );

                } while (false);

                if (!guiCtrl.isSerialActive)
                    return;

                DsoData data = new DsoData(guiCtrl.colorLine, ref sb);
                if (data.IsNotEmpty())
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        guiCtrl.OnReadDevice(this, data);
                    });
            }
            catch (Exception e)
            {
                throwMessage(e.Message);
            }
        }
    }
}
