using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogicPictureLE
{
    public partial class PreviewProject : Window
    {
        const int TILE_SIDE = 50;
        const int SPACE_MIN = 5;
        const int SPACE_MAX = 15;
        const int TILES_GROUP = 5;
        const int FONT_SIZE = 35;

        enum Lang { EN, PL, Other }

        SingleLevel _singleLevel;
        int _offsetY;
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

            gStartBackgroundGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            gEndBackgroundGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));

            _offsetY = level.LevelData.HeightY % TILES_GROUP;

            UpdateAllHints();
            UpadteTilesDataGrids();
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
                double percentOfColor = PERCENT * tilesInColor.Count / selectedTilesCount;
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

        private void UpdateAllHints()
        {
            UpadteHintsOnSingleTab(gStartHintsHorizontal, gStartHintsVertical);
            UpadteHintsOnSingleTab(gEndHintsHorizontal, gEndHintsVertical);
        }

        private void UpadteHintsOnSingleTab(Grid hintsHorizontal, Grid hintsVertical)
        {
            PrepareHintsHorizontal(hintsHorizontal.ColumnDefinitions, hintsHorizontal.RowDefinitions);
            FillHintsHorizontal(hintsHorizontal);

            PrepareHintsVerical(hintsVertical.ColumnDefinitions, hintsVertical.RowDefinitions);
            FillHintsVertical(hintsVertical);
        }

        private int MaxSizeHints(HintData[][] hintDatas)
        {
            return hintDatas.OrderByDescending(list => list.Count()).First().Length;
        }

        private void PrepareHintsHorizontal(ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            columnDefinitions.Clear();

            double spacegrid;
            for (int i = 0; i < _singleLevel.LevelData.WidthX; i++)
            {
                if ((i % TILES_GROUP) == 0 && i != 0) { spacegrid = SPACE_MAX; }
                else { spacegrid = SPACE_MIN; }

                ColumnDefinition cdTemp = new ColumnDefinition() { Width = new GridLength(spacegrid) };
                columnDefinitions.Add(cdTemp);
                cdTemp = new ColumnDefinition() { Width = new GridLength(TILE_SIDE) };
                columnDefinitions.Add(cdTemp);
                if (i == _singleLevel.LevelData.WidthX - 1)
                {
                    cdTemp = new ColumnDefinition() { Width = new GridLength(SPACE_MIN) };
                    columnDefinitions.Add(cdTemp);
                }
            }

            rowDefinitions.Clear();
            int maxSizeHints = MaxSizeHints(_singleLevel.LevelData.HintsDataHorizontal);
            for (int i = 0; i < maxSizeHints; i++)
            {
                RowDefinition rdTemp = new RowDefinition() { Height = new GridLength(TILE_SIDE) };
                rowDefinitions.Add(rdTemp);
            }
        }

        private void FillHintsHorizontal(Grid hintsHorizontal)
        {
            hintsHorizontal.Children.Clear();

            int maxSizeHints = MaxSizeHints(_singleLevel.LevelData.HintsDataHorizontal);
            for (int i = 0; i < _singleLevel.LevelData.WidthX; i++)
            {
                for (int j = 0; j < _singleLevel.LevelData.HintsDataHorizontal[i].Length; j++)
                {
                    TextBlock textBlockHint = new TextBlock()
                    {
                        Text = _singleLevel.LevelData.HintsDataHorizontal[i][j].Value.ToString(),
                        FontSize = FONT_SIZE,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHints - _singleLevel.LevelData.HintsDataHorizontal[i].Length + j);
                    if (_singleLevel.LevelData.HintsDataHorizontal[i][j].Value != 0)
                    {
                        byte colorID = _singleLevel.LevelData.HintsDataHorizontal[i][j].ColorID;
                        textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorsDataTiles[colorID]));
                    }
                    else
                    {
                        textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorDataNeutral));
                    }
                    hintsHorizontal.Children.Add(textBlockHint);
                }
            }
        }

        private void PrepareHintsVerical(ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            columnDefinitions.Clear();
            rowDefinitions.Clear();

            int maxSizeHints = MaxSizeHints(_singleLevel.LevelData.HintsDataVertical);
            double spacegrid;
            for (int i = 0; i < maxSizeHints; i++)
            {
                ColumnDefinition cdTemp = new ColumnDefinition() { Width = new GridLength(TILE_SIDE) };
                columnDefinitions.Add(cdTemp);
            }

            for (int i = 0; i < _singleLevel.LevelData.HeightY; i++)
            {
                if ((i - _offsetY) % TILES_GROUP == 0 && i != 0) spacegrid = SPACE_MAX;
                else spacegrid = SPACE_MIN;

                RowDefinition rdTemp = new RowDefinition() { Height = new GridLength(spacegrid) };
                rowDefinitions.Add(rdTemp);
                rdTemp = new RowDefinition() { Height = new GridLength(TILE_SIDE) };
                rowDefinitions.Add(rdTemp);
                if (i == _singleLevel.LevelData.HeightY - 1)
                {
                    rdTemp = new RowDefinition() { Height = new GridLength(SPACE_MIN) };
                    rowDefinitions.Add(rdTemp);
                }
            }
        }
       
        private void FillHintsVertical(Grid hintsVertical)
        {
            hintsVertical.Children.Clear();

            int maxSizeHints = MaxSizeHints(_singleLevel.LevelData.HintsDataVertical);
            for (int j = 0; j < _singleLevel.LevelData.HeightY; j++)
            {
                for (int i = 0; i < _singleLevel.LevelData.HintsDataVertical[j].Length; i++)
                {
                    TextBlock textBlockHint = new TextBlock()
                    {
                        Text = _singleLevel.LevelData.HintsDataVertical[j][i].Value.ToString(),
                        FontSize = FONT_SIZE, FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center
                };
                    Grid.SetColumn(textBlockHint, maxSizeHints - _singleLevel.LevelData.HintsDataVertical[j].Length + i);
                    Grid.SetRow(textBlockHint, 2 * ((int)_singleLevel.LevelData.HeightY - j - 1) + 1);
                    if (_singleLevel.LevelData.HintsDataVertical[j][i].Value != 0)
                    {
                        byte colorId = _singleLevel.LevelData.HintsDataVertical[j][i].ColorID;
                        textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorsDataTiles[colorId]));
                    }
                    else
                    {
                        textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorDataNeutral));
                    }
                    hintsVertical.Children.Add(textBlockHint);
                }
            }
        }

        private void UpadteTilesDataGrids()
        {
            PrepareGridTilesData(gStartPicture.ColumnDefinitions, gStartPicture.RowDefinitions);
            FillTilesDataInGrid(gStartPicture, false);

            PrepareGridTilesData(gEndPicture.ColumnDefinitions, gEndPicture.RowDefinitions);
            FillTilesDataInGrid(gEndPicture, true);
        }

        private void PrepareGridTilesData(ColumnDefinitionCollection columnDefinitions, RowDefinitionCollection rowDefinitions)
        {
            double spacegrid;

            columnDefinitions.Clear();
            for (int i = 0; i < _singleLevel.LevelData.WidthX; i++)
            {
                if (i % TILES_GROUP == 0 && i != 0) spacegrid = SPACE_MAX;
                else spacegrid = SPACE_MIN;

                ColumnDefinition cdTemp = new ColumnDefinition() { Width = new GridLength(spacegrid) };
                columnDefinitions.Add(cdTemp);
                cdTemp = new ColumnDefinition() { Width = new GridLength(TILE_SIDE) };
                columnDefinitions.Add(cdTemp);
                if (i == _singleLevel.LevelData.WidthX - 1)
                {
                    cdTemp = new ColumnDefinition() { Width = new GridLength(SPACE_MIN) };
                    columnDefinitions.Add(cdTemp);
                }
            }

            rowDefinitions.Clear();
            for (int i = 0; i < _singleLevel.LevelData.HeightY; i++)
            {
                if ((i - _offsetY) % TILES_GROUP == 0 && i != 0) spacegrid = SPACE_MAX;
                else spacegrid = SPACE_MIN;

                RowDefinition rdTemp = new RowDefinition() { Height = new GridLength(spacegrid) };
                rowDefinitions.Add(rdTemp);
                rdTemp = new RowDefinition() { Height = new GridLength(TILE_SIDE) };
                rowDefinitions.Add(rdTemp);
                if (i == _singleLevel.LevelData.HeightY - 1)
                {
                    rdTemp = new RowDefinition() { Height = new GridLength(SPACE_MIN) };
                    rowDefinitions.Add(rdTemp);
                }
            }
        }

        private void FillTilesDataInGrid(Grid tilesDataGrid, bool isEnd)
        {
            tilesDataGrid.Children.Clear();

            Rectangle rectangleMark = new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
                Fill = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorDataMarker))
            };
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (_singleLevel.LevelData.HeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            tilesDataGrid.Children.Add(rectangleMark);

            for (int x = 0; x < _singleLevel.LevelData.WidthX; x++)
            {
                for (int y = 0; y < _singleLevel.LevelData.HeightY; y++)
                {
                    Rectangle rTemp = new Rectangle()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch
                    };
                    Grid.SetColumn(rTemp, 2 * x + 1);
                    Grid.SetRow(rTemp, 2 * ((_singleLevel.LevelData.HeightY - 1) - y) + 1);
                    TileData tileData = _singleLevel.LevelData.TilesData[x][y];
                    if (tileData.IsSelected && isEnd)
                    {
                        rTemp.Fill = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorsDataTiles[tileData.ColorID]));
                    }
                    else
                    {
                        rTemp.Fill = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorDataNeutral));
                    }
                    tilesDataGrid.Children.Add(rTemp);
                }
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
    }
}
