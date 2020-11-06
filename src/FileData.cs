using DSO138Device;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace DSO138_Capture
{
    public class FileData
    {
        public enum SaveType : int
        {
            SAVE_DSO,
            SAVE_SVG,
            SAVE_PDF,
            SAVE_NONE
        };
        private static readonly string[] ext_ = new string[] {
            ".dso138", ".svg", ".pdf"
        };
        private static readonly string[] desc_ = new string[] {
            GuiControl.getStringResource("FileDescDSO"),
            GuiControl.getStringResource("FileDescSVG"),
            GuiControl.getStringResource("FileDescPDF")
        };

        public async void Open(GuiControl guiCtrl)
        {
            try {
                FileOpenPicker fop = new FileOpenPicker();
                fop.ViewMode = PickerViewMode.Thumbnail;
                fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                fop.FileTypeFilter.Add(ext_[0]);
                fop.CommitButtonText = GuiControl.getStringResource("OpenButton");
                var file = await fop.PickSingleFileAsync();
                if (file == null)
                    return;
                
                StringBuilder sb = new StringBuilder(65536);
                sb.Append(await Windows.Storage.FileIO.ReadTextAsync(file));
                if (sb.Length == 0)
                    return;

                DsoData data = new DsoData(guiCtrl.colorLine, ref sb);
                if (data.IsNotEmpty())
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        guiCtrl.dataDso = data;
                    });

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async void Save(PlotModel plotModel, DsoData data)
        {
            if (data == null)
                return;

            try
            {
                var fsp = new FileSavePicker();
                fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                for (int i = 0; i < ext_.Length; i++)
                    fsp.FileTypeChoices.Add(desc_[i], new List<string>() { ext_[i] });

                fsp.SuggestedFileName = data.DateCreated;
                fsp.CommitButtonText = GuiControl.getStringResource("SavePlot");

                var file = await fsp.PickSaveFileAsync();
                if (file == null)
                    throw new Exception(GuiControl.getStringResource("ToFileError1"));

                SaveType type = SaveType.SAVE_NONE;
                for (int i = 0; i < ext_.Length; i++)
                    if (file.Name.EndsWith(ext_[i]))
                    {
                        type = (SaveType)i;
                        break;
                    }
                if (type == SaveType.SAVE_NONE)
                    throw new Exception(GuiControl.getStringResource("ToFileError2"));

                using (var irstream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    switch (type)
                    {
                        case SaveType.SAVE_DSO: {
                                _writeDso(irstream, ref data);
                                break;
                            }
                        case SaveType.SAVE_SVG:
                            {
                                _writeSvg(irstream, ref plotModel);
                                break;
                            }
                        case SaveType.SAVE_PDF:
                            {
                                _writePdf(irstream, ref plotModel);
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void _writeDso(IRandomAccessStream irstream, ref DsoData data)
        {
            int cnt = 0;
            StringBuilder sb = new StringBuilder(65534);
            
            for (int i = 0; i < (int)DsoDataInfo.Ids.ID_End; i++)
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}\r\n", DsoDataInfo.Names[i], data.info.get(i));
            for (int i = 0; i < data.plot.Count(); i++)
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0},{1},{2}\r\n", cnt++, data.plot.series.Points[i].X, data.plot.series.Points[i].Y);

            using (Stream stream = irstream.AsStream())
                stream.Write(Encoding.ASCII.GetBytes(sb.ToString()), 0, sb.Length);
        }
        private void _writeSvg(IRandomAccessStream irstream, ref PlotModel plotModel)
        {
            var exporter = new SvgExporter { Width = 1200, Height = 800 };
            using (Stream stream = irstream.AsStream())
                exporter.Export(plotModel, stream);
        }
        private void _writePdf(IRandomAccessStream irstream, ref PlotModel plotModel)
        {
            var exporter = new PdfExporter { Width = 1200, Height = 800, Background = OxyColors.White };
            using (Stream stream = irstream.AsStream())
                exporter.Export(plotModel, stream);
        }
    }
}
