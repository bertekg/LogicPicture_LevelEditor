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
using System.Windows.Shapes;

namespace LogicPictureLE
{
    /// <summary>
    /// Logika interakcji dla klasy NewLevelWizard.xaml
    /// </summary>
    public partial class NewLevelWizard : Window
    {
        public NewLevelWizard()
        {
            InitializeComponent();
        }
        public SingleLevel singleLevel;
        private void button_EmptySingle_Click(object sender, RoutedEventArgs e)
        {
            NewSingleLevelMake();
            this.Close();
        }
        public void NewSingleLevelMake()
        {
            ColorData colorDataNeutral = new ColorData(0xC0, 0xC0, 0xC0); //Silver = #FFC0C0C0
            ColorData colorDataBackground = new ColorData(0xFF, 0xFF, 0xFF); //White = #FFFFFFFF
            ColorData colorDataMarker = new ColorData(0xFF, 0xA5, 0x00); //Orange = #FFFFA500
            List<ColorData> colorsDataTiles = new List<ColorData>();
            colorsDataTiles.Add(new ColorData(0x00, 0x00, 0x00)); //Black = #FF000000
            //colorsDataTiles.Add(new ColorData(0x00, 0x00, 0xFF)); //Blue = #FF0000FF
            List<TileData> tilesData = new List<TileData>();
            List<List<HintData>> hintsDataVertical = new List<List<HintData>>();
            List<List<HintData>> hintsDataHorizontal = new List<List<HintData>>();
            Level level = new Level(5, 5, colorDataNeutral, colorDataBackground, colorDataMarker,
                colorsDataTiles, tilesData, hintsDataHorizontal, hintsDataVertical);
            singleLevel = new SingleLevel("Empty Single Level", "Pusty pojedyńczy poziom", level);
        }

        private void button_EmptyBig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Empty Big TODO");
        }
        private void button_EmptyGif_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Empty Gif TODO");
        }
        private void button_ImportSingle_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Import Single TODO");
        }
        private void button_ImportBig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Import Big TODO");
        }
    }
}
