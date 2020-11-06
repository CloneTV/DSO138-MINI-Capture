using DSO138Device;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Windows;
using System;
using System.Globalization;
using Windows.UI;

namespace DSO138_Capture
{
    public class GuiPlot
    {
        private static readonly string font_ = "Times New Roman";
        public PlotModel plotModel;

        public GuiPlot()
        {
            plotModel = new PlotModel { Title = GuiControl.getStringResource("ChartTitle") };
        }

        public void plotSetSeries(LineSeries series, string date)
        {
            plotModel.Series.Clear();
            plotModel.ResetAllAxes();
            plotModel.Annotations.Clear();
            plotModel.Series.Add(series);
            plotModel.Title = (String.IsNullOrEmpty(date)) ? GuiControl.getStringResource("ChartTitle") : date;
        }

        public void plotFill(Color clr, ref DsoData data)
        {
            try
            {
                LineSeries series;
                String s = null;

                if (data == null)
                {
                    series = new LineSeries
                    {
                        Title = GuiControl.getStringResource("ChartTitle"),
                        MarkerType = MarkerType.Circle,
                        Color = clr.ToOxyColor()
                    };
                    for (int i = 0; i < 1024; i++)
                        series.Points.Add(new DataPoint(0.0, 0.0));
                }
                else
                {
                    data.plot.series.Color = clr.ToOxyColor();
                    series = data.plot.series;
                    s = data.DateCreated;
                }
                plotSetSeries(series, s);
            }
            catch (Exception) { }
        }

        public void plotChangeTheme(bool isChangeTheme)
        {
            try
            {
                OxyColor lcolor = (isChangeTheme) ? OxyColors.DarkGray : OxyColors.Black;
                plotModel.TextColor = (isChangeTheme) ? OxyColors.WhiteSmoke : OxyColors.Black;
                plotModel.TitleColor = lcolor;
                plotModel.PlotAreaBorderColor = lcolor;
                plotModel.LegendTextColor = lcolor;
                for (int i = 0; i < plotModel.Axes.Count; i++)
                {
                    plotModel.Axes[i].MinorTicklineColor = lcolor;
                    plotModel.Axes[i].MinorGridlineColor = lcolor;
                    plotModel.Axes[i].MajorGridlineColor = lcolor;
                    plotModel.Axes[i].ExtraGridlineColor = lcolor;
                }
            }
            catch (Exception) { }
        }

        public void plotSetAnnotations(bool isChangeTheme, ref DsoData data)
        {
            if (data == null)
                return;
            if (String.IsNullOrEmpty(data.info.Vmin))
                return;

            try
            {
                string vmin = data.info.Vmin.Substring(0, data.info.Vmin.Length - 1);
                double h = 0.0, v = 0.0;
                if (!Double.TryParse(vmin, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                    return;

                v -= 0.15;
                for (uint i = 0; i < (uint)DsoDataInfo.Ids.ID_End; i++)
                {
                    h += 700.0;
                    TextAnnotation ta;

                    ta = new TextAnnotation()
                    {
                        Text = DsoDataInfo.Names[(int)DsoDataInfo.dataAnnotationIdx[i]],
                        TextPosition = new DataPoint(h, v),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Font = GuiPlot.font_,
                        TextColor = ((isChangeTheme) ? OxyColors.PapayaWhip : OxyColors.DarkMagenta)
                    };
                    plotModel.Annotations.Add(ta);
                    ta = new TextAnnotation()
                    {
                        Text = data.info.get(DsoDataInfo.dataAnnotationIdx[i]),
                        TextPosition = new DataPoint(h, (v - 0.15)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Font = GuiPlot.font_,
                        TextColor = ((isChangeTheme) ? OxyColors.YellowGreen : OxyColors.DarkRed)
                    };
                    plotModel.Annotations.Add(ta);
                }
            }
            catch (Exception) { }
        }
    }
}
