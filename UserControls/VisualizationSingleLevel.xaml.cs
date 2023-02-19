using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicPictureLE.UserControls
{
    public partial class VisualizationSingleLevel : UserControl
    {
        const int TILE_SIDE = 50;
        const int SPACE_MIN = 5;
        const int SPACE_MAX = 15;
        const int TILES_GROUP = 5;
        const int FONT_SIZE = 35;

        SingleLevel _singleLevel;
        int _offsetY;

        public VisualizationSingleLevel(SingleLevel level, bool isFullFill, bool isMarkerShow)
        {
            InitializeComponent();
            _singleLevel = level;

            _offsetY = level.LevelData.HeightY % TILES_GROUP;

            gridBackgroundGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            UpadteHintsOnSingleTab(gridHintsHorizontal, gridHintsVertical);

            PrepareGridTilesData(gridPicture.ColumnDefinitions, gridPicture.RowDefinitions);
            FillTilesDataInGrid(gridPicture, isFullFill, isMarkerShow);
        }
        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
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
                        FontSize = FONT_SIZE,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
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

        private void FillTilesDataInGrid(Grid tilesDataGrid, bool isEnd, bool isMarker)
        {
            tilesDataGrid.Children.Clear();

            if (isMarker)
            {
                Rectangle rectangleMark = new Rectangle()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Fill = new SolidColorBrush(GetColorFromColorData(_singleLevel.LevelData.ColorDataMarker))
                };
                Grid.SetColumn(rectangleMark, 0);
                Grid.SetRow(rectangleMark, 2 * (_singleLevel.LevelData.HeightY - 1));
                Grid.SetColumnSpan(rectangleMark, 3);
                Grid.SetRowSpan(rectangleMark, 3);
                tilesDataGrid.Children.Add(rectangleMark);
            }

            for (int x = 0; x < _singleLevel.LevelData.WidthX; x++)
            {
                for (int y = 0; y < _singleLevel.LevelData.HeightY; y++)
                {
                    Rectangle rTemp = new Rectangle()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
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
    }
}
