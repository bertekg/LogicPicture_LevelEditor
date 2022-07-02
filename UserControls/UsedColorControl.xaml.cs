using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicPictureLE.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy UsedColorControl.xaml
    /// </summary>
    public partial class UsedColorControl : UserControl
    {
        public UsedColorControl()
        {
            InitializeComponent();
        }
        public UsedColorControl(byte index, ColorData colorData)
        {
            InitializeComponent();
            textBlock_ID.Text = index.ToString();
            colorPicker_Color.SelectedColor = Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
            colorPicker_Color.Tag = index;
            button_DeleteColor.Tag = index;
        }
    }
}
