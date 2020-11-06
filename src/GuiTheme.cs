using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace DSO138_Capture
{
    public class GuiTheme
    {
        public bool isMenuOpen = false;
        public bool isColorOpen = false;
        public bool isSerialActive = false;
        public bool isSerialSelected = false;
        public bool isDsoDataEmpty = true;
        public bool isChangeTheme = false;
        public Color colorLine = Color.FromArgb(0xff, 219, 52, 34);

        public GuiTheme()
        {
        }
        
        public void chageMenuTheme(ref ObservableCollection<NavMenu> navigtionList)
        {
            Color fcolor = (isChangeTheme) ? Colors.WhiteSmoke : Colors.Black;
            foreach (var nav in navigtionList)
            {
                switch (nav.Id)
                {
                    case 1:
                        {
                            if ((isSerialSelected) && (!isSerialActive))
                            {
                                nav.ColorText = (isChangeTheme) ? Colors.WhiteSmoke : Colors.Black;
                                nav.ColorIcon = (isChangeTheme) ? Colors.GreenYellow : Colors.DarkGreen;
                                break;
                            }
                            else
                                nav.ColorText = nav.ColorIcon = (isChangeTheme) ? Colors.DarkGray : Colors.Gray;
                            break;
                        }
                    case 2:
                        {
                            if ((isSerialSelected) && (isSerialActive))
                            {
                                nav.ColorText = (isChangeTheme) ? Colors.WhiteSmoke : Colors.Black;
                                nav.ColorIcon = (isChangeTheme) ? Colors.IndianRed : Colors.DarkRed;
                                break;
                            }
                            else
                                nav.ColorText = nav.ColorIcon = (isChangeTheme) ? Colors.DarkGray : Colors.Gray;
                            break;
                        }
                    case 6:
                        {
                            if (isDsoDataEmpty)
                                nav.ColorText = nav.ColorIcon = (isChangeTheme) ? Colors.DarkGray : Colors.Gray;
                            else
                                nav.ColorIcon = nav.ColorText = fcolor;
                            break;
                        }
                    case 7:
                        {
                            if (isSerialActive)
                                nav.ColorText = nav.ColorIcon = (isChangeTheme) ? Colors.DarkGray : Colors.Gray;
                            else
                                nav.ColorIcon = nav.ColorText = fcolor;
                            break;
                        }
                    default:
                        {
                            nav.ColorIcon = nav.ColorText = fcolor;
                            break;
                        }
                }
            }
        }
    }
}
