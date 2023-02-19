using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LogicPictureLE
{
    public partial class PreviewProject : Window
    {
        enum Lang { EN, PL, Other }

        SingleLevel _singleLevel;
        Lang _lang;

        public PreviewProject()
        {
            InitializeComponent();
            _singleLevel = new SingleLevel();
            Title = "Preview Project - Default";
        }

        public PreviewProject(SingleLevel level)
        {
            InitializeComponent();
            _singleLevel = level;
            Title = "Preview Project - Single Level";
            FillBasicInfoTabItem();
            tabItemStart.Content = new UserControls.VisualizationSingleLevel(level, false, true);
            tabItemEnd.Content = new UserControls.VisualizationSingleLevel(level, true, true);
            UpadteFinalPicture();
            UpdateLanguage(Lang.EN);
        }

        private void FillBasicInfoTabItem()
        {
            const double PERCENT = 100.0d;
            double widthX = _singleLevel.LevelData.WidthX;
            tbLevelWidth.Text = widthX.ToString();
            double heightY = _singleLevel.LevelData.HeightY;
            tbLevelHeight.Text = heightY.ToString();
            double total = widthX * heightY;
            tbLevelTotalCells.Text = total.ToString();
            double selectedTilesCount = CountTilesIsSelected();
            tbLevelFilledAllCellsCount.Text = selectedTilesCount.ToString();
            tbLevelFilledViewInPrecent.Text = (PERCENT * selectedTilesCount / total).ToString();
            List<ColorDetail> colors = new List<ColorDetail>();
            for (byte i = 0; i < _singleLevel.LevelData.ColorsDataTiles.Length; i++)
            {
                Color color = GetColorFromColorData(_singleLevel.LevelData.ColorsDataTiles[i]);
                List<TileData> tilesInColor = GetListOfSpecificColor(i);
                double percentOfColor;
                if (selectedTilesCount != 0)
                {
                    percentOfColor = PERCENT * tilesInColor.Count / selectedTilesCount;
                }
                else
                {
                    percentOfColor = 0;
                }
                double percentOfAll = PERCENT * tilesInColor.Count / total;
                ColorDetail cdTemp = new ColorDetail(i, color, tilesInColor.Count, percentOfColor, percentOfAll);
                colors.Add(cdTemp);
            }
            lvCellsWithColors.ItemsSource = colors;
        }

        private double CountTilesIsSelected()
        {
            double count = 0;
            foreach (TileData[] tiles in _singleLevel.LevelData.TilesData)
            {
                foreach (TileData tile in tiles)
                {
                    if (tile.IsSelected) { count++; }
                }
            }
            return count;
        }

        private List<TileData> GetListOfSpecificColor(byte i)
        {
            List <TileData> foundTiles = new List <TileData>();
            foreach (TileData[] tiles in _singleLevel.LevelData.TilesData)
            {
                foreach (TileData tile in tiles)
                {
                    if (tile.IsSelected && tile.ColorID == i) { foundTiles.Add(tile); }
                }
            }
            return foundTiles;
        }

        private void UpdateLanguage(Lang language)
        {
            _lang = language;
            switch (_lang)
            {
                case Lang.EN:
                    textBlock_LevelTitle.Text = _singleLevel.ProjectStoryEN.Title;
                    tbFinish_LevelDescription.Text = _singleLevel.ProjectStoryEN.Description;
                    button_FianlBackToMenu.Content = "Back to menu";
                    break;
                case Lang.PL:
                    textBlock_LevelTitle.Text = _singleLevel.ProjectStoryPL.Title;
                    tbFinish_LevelDescription.Text = _singleLevel.ProjectStoryPL.Description;
                    button_FianlBackToMenu.Content = "Powrót do menu";
                    break;
                case Lang.Other:
                    MessageBox.Show("Place for next future language.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show("Missing language option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void UpadteFinalPicture()
        {
            iFinal.Source = GetLevelPicture();
        }
        public WriteableBitmap GetLevelPicture()
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(_singleLevel.LevelData.WidthX, _singleLevel.LevelData.HeightY, 96, 96, PixelFormats.Bgra32, null);
            byte[] pixels1d = new byte[_singleLevel.LevelData.HeightY * _singleLevel.LevelData.WidthX * 4];
            int index = 0;
            for (int vertical = 0; vertical < _singleLevel.LevelData.HeightY; vertical++)
            {
                for (int horizontal = 0; horizontal < _singleLevel.LevelData.WidthX; horizontal++)
                {
                    TileData tileDataFound = _singleLevel.LevelData.TilesData[horizontal][_singleLevel.LevelData.HeightY - vertical - 1];
                    if (tileDataFound.IsSelected)
                    {
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Blue;
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Green;
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Red;
                    }
                    else
                    {
                        pixels1d[index++] = _singleLevel.LevelData.ColorDataNeutral.Blue;
                        pixels1d[index++] = _singleLevel.LevelData.ColorDataNeutral.Green;
                        pixels1d[index++] = _singleLevel.LevelData.ColorDataNeutral.Red;
                    }
                    pixels1d[index++] = 255;
                }
            }
            Int32Rect rect = new Int32Rect(0, 0, _singleLevel.LevelData.WidthX, _singleLevel.LevelData.HeightY);
            int stride = 4 * _singleLevel.LevelData.WidthX;
            writeableBitmap.WritePixels(rect, pixels1d, stride, 0);
            return writeableBitmap;
        }

        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
        }
        public class ColorDetail
        {
            public byte colorId { get; set; }
            public Color colorName { get; set; }
            public int countOfColor { get; set; }
            public double percentToAllColors { get; set; }
            public double percentToAllTiles { get; set; }
            public ColorDetail()
            {
                colorId = 0;
                colorName = new Color();
                countOfColor = 0;
                percentToAllColors = 0;
                percentToAllTiles = 0;
            }
            public ColorDetail(byte ColorId, Color ColorName, int CountOfColor, 
                double PercentToAllColors, double PercentToAllTiles)
            {
                colorId = ColorId;
                colorName = ColorName;
                countOfColor = CountOfColor;
                percentToAllColors = PercentToAllColors;
                percentToAllTiles = PercentToAllTiles;
            }
        }
        private void button_FinalLangEnglish_Click(object sender, RoutedEventArgs e)
        {
            UpdateLanguage(Lang.EN);
        }
        private void button_FinalLangPolish_Click(object sender, RoutedEventArgs e)
        {
            UpdateLanguage(Lang.PL);
        }
        private void button_FinalLangOther_Click(object sender, RoutedEventArgs e)
        {
            UpdateLanguage(Lang.Other);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Back)
            {
                Close();
            }
        }
    }
}
