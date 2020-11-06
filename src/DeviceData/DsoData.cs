using System;
using System.Globalization;
using System.Text;
using Windows.UI;

namespace DSO138Device
{
    public class DsoData : EventArgs
    {
        private readonly char[] SEP1__ = { '\r', '\n' };
        private readonly char[] SEP2__ = { ' ', '\r' };
        private bool isnotempty = false;
        private int cnt = 0, chacheIdx = 0;

        public TimeSpan date;
        public DsoDataInfo info = null;
        public DsoDataPlot plot = null;

        public string DateCreated
        {
            get {
                if (date != null)
                    return date.ToString(@"hh\:mm\:ss");
                return "";
            }
        }

        public DsoData(Color clr)
        {
            info = new DsoDataInfo();
            plot = new DsoDataPlot(clr);
            date = DateTime.Now.TimeOfDay;
        }
        public DsoData(DsoDataInfo i, DsoDataPlot s)
        {
            info = i;
            plot = s;
            date = DateTime.Now.TimeOfDay;
        }
        public DsoData(Color clr, ref StringBuilder sb)
        {
            if (sb.Length == 0)
                return;

            string[] lines = sb.ToString().Split(SEP1__, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                return;

            info = new DsoDataInfo();
            plot = new DsoDataPlot(clr);
            date = DateTime.Now.TimeOfDay;

            foreach (string line in lines)
            {
                string[] tokens = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                switch (tokens.Length)
                {
                    case 2:
                        {
                            parseInfo(ref tokens);
                            break;
                        }
                    case 3:
                        {
                            parseArray(ref tokens);
                            break;
                        }
                    default: continue;
                }
            }

            if (plot.Count() == 0)
                return;
            
            isnotempty = true;
        }

        public bool IsNotEmpty()
        {
            return (isnotempty) ? (cnt == 19) : isnotempty;
        }

        private void parseInfo(ref string[] tokens)
        {
            string valName = tokens[0].Trim(SEP2__[0]);
            string valData = tokens[1].Trim(SEP2__);
            for (int i = chacheIdx; i < (int)DsoDataInfo.Ids.ID_End; i++)
            {
                if (info.update(i, ref valName, ref valData))
                {
                    cnt++;
                    chacheIdx = i;
                    break;
                }
            }
        }
        private void parseArray(ref string[] tokens)
        {
            do
            {
                Int32 an = 0;
                if (!Int32.TryParse(tokens[1], out an))
                    break;
                double ad = 0.0;
                if (!Double.TryParse(tokens[2], NumberStyles.Float, CultureInfo.InvariantCulture, out ad))
                    break;
                
                plot.Add(an, ad);

            } while (false);
        }
    }
}
