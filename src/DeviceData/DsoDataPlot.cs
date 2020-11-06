using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Windows;
using Windows.UI;

namespace DSO138Device
{
    public class DsoDataPlot
    {
        private int count = 0;
        public LineSeries series;

        public DsoDataPlot(Color clr)
        {
            series = new LineSeries {
                Title = DateTime.Now.ToShortTimeString(),
                MarkerType = MarkerType.Circle,
                Color = clr.ToOxyColor()
            };
        }

        public void Add(Int16 i, Int32 n, double d)
        {
            count++;
            series.Points.Add(new DataPoint(n, d));
        }
        public void Add(double n, double d)
        {
            count++;
            series.Points.Add(new DataPoint(n, d));
        }

        public int Count()
        {
            return count;
        }
    };
}
