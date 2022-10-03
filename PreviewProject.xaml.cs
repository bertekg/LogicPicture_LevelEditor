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
    /// Logika interakcji dla klasy PreviewProject.xaml
    /// </summary>
    public partial class PreviewProject : Window
    {
        SingleLevel singleLevel;
        int offsetY;
        enum Lang { EN, PL, Other}
        Lang lang;
        public PreviewProject()
        {
            InitializeComponent();
            singleLevel = new SingleLevel();
            Window_PrevieProject.Title = "Preview Project - Default";
        }
        public PreviewProject(SingleLevel level)
        {
            InitializeComponent();
            singleLevel = level;
            Window_PrevieProject.Title = "Preview Project - Single Level";
            int widthX = level.LevelData.WidthX;
            tbLevelWidth.Text = widthX.ToString();
            int heightY = level.LevelData.HeightY;
            tbLevelHeight.Text = heightY.ToString();
            tbLevelTotalCells.Text = (widthX * heightY).ToString();
            tbLevelFilledAllCellsCount.Text = level.LevelData.TilesData.Count.ToString();
            List<TileData> insideTiles = level.LevelData.TilesData.FindAll(item => item.PosX < widthX && item.PosY < heightY);
            List<TileData> outsideTiles = level.LevelData.TilesData.FindAll(item => item.PosX >= widthX || item.PosY >= heightY);
            tbLevelFilledViewCellsCount.Text = (insideTiles.Count).ToString();
            tbLevelFilledOutsideViewCellsCount.Text = outsideTiles.Count.ToString();
            tbLevelFilledViewInPrecent.Text = (((double)(insideTiles.Count) / (double)(widthX * heightY)) * 100.0).ToString() + "%";
            lvAllCells.ItemsSource = level.LevelData.TilesData;
            lvInsideCells.ItemsSource = insideTiles;
            lvOutsideCells.ItemsSource = outsideTiles;
            List<ColorDetail> colors = new List<ColorDetail>();
            for (byte i = 0; i < level.LevelData.ColorsDataTiles.Count; i++)
            {
                Color color = GetColorFromColorData(level.LevelData.ColorsDataTiles[i]);
                List<TileData> tilesInColor = level.LevelData.TilesData.FindAll(item => item.ColorID == i);
                double dProcentOfColor = (((double)tilesInColor.Count) / (level.LevelData.TilesData.Count)) * 100.0;
                ColorDetail cdTemp = new ColorDetail(i, color, tilesInColor.Count, dProcentOfColor);
                colors.Add(cdTemp);
            }
            lvCellsWithColors.ItemsSource = colors;

            gStartBackgroundGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            gEndBackgroundGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            
            offsetY = level.LevelData.HeightY % 5;
            AddHintsToGrid();
            UpdatesGridsLayoutOfPicture();
            AddCellsToGrids();
            UpadteFinalPicture();
            UpdateLanguage(Lang.EN);            
        }
        private void UpdateLanguage(Lang language)
        {
            lang = language;
            switch (lang)
            {
                case Lang.EN:
                    textBlock_FinalCongratulation.Text = "Level Finished!!!";
                    tbFinish_NameLevel.Text = singleLevel.ProjectStoryEN.Title + " [" + singleLevel.LevelData.WidthX.ToString() +
                        "," + singleLevel.LevelData.HeightY.ToString() + "]";
                    button_FianlBacToMenu.Content = "Back To Menu";
                    break;
                case Lang.PL:
                    textBlock_FinalCongratulation.Text = "Poziom ukończony!!!";
                    tbFinish_NameLevel.Text = singleLevel.ProjectStoryPL.Title + " [" + singleLevel.LevelData.WidthX.ToString() +
                        "," + singleLevel.LevelData.HeightY.ToString() + "]";
                    button_FianlBacToMenu.Content = "Powrót do Menu";
                    break;
                case Lang.Other:
                    MessageBox.Show("Place for next future language.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show("Missing language option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        private void AddHintsToGrid()
        {
            gStartHintsHorizontal.ColumnDefinitions.Clear();
            gEndHintsHorizontal.ColumnDefinitions.Clear();
            gStartHintsHorizontal.RowDefinitions.Clear();
            gEndHintsHorizontal.RowDefinitions.Clear();
            gStartHintsVertical.ColumnDefinitions.Clear();
            gEndHintsVertical.ColumnDefinitions.Clear();
            gStartHintsVertical.RowDefinitions.Clear();
            gEndHintsVertical.RowDefinitions.Clear();
            ColumnDefinition gridColDefTemp;
            double spacegrid;

            for (int i = 0; i < singleLevel.LevelData.WidthX; i++)
            {
                if (i % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(spacegrid);
                gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(spacegrid);
                gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                if (i == singleLevel.LevelData.WidthX - 1)
                {
                    gridColDefTemp = new ColumnDefinition();
                    gridColDefTemp.Width = new GridLength(5);
                    gStartHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                    gridColDefTemp = new ColumnDefinition();
                    gridColDefTemp.Width = new GridLength(5);
                    gEndHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                }
            }

            int maxSizeHintsHorizontalCounts = singleLevel.LevelData.HintsDataHorizontal.OrderByDescending(list => list.Count()).First().Count;
            RowDefinition gridRowDefTemp;
            for (int i = 0; i < maxSizeHintsHorizontalCounts; i++)
            {
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gStartHintsHorizontal.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gEndHintsHorizontal.RowDefinitions.Add(gridRowDefTemp);
            }
            gStartHintsHorizontal.Children.Clear();
            gEndHintsHorizontal.Children.Clear();
            for (int i = 0; i < singleLevel.LevelData.WidthX; i++)
            {
                for (int j = 0; j < singleLevel.LevelData.HintsDataHorizontal[i].Count; j++)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = singleLevel.LevelData.HintsDataHorizontal[i][j].Value.ToString();
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts - singleLevel.LevelData.HintsDataHorizontal[i].Count + j);
                    byte byteResult = singleLevel.LevelData.HintsDataHorizontal[i][j].ColorID;
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[byteResult]));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsHorizontal.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = singleLevel.LevelData.HintsDataHorizontal[i][j].Value.ToString();
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts - singleLevel.LevelData.HintsDataHorizontal[i].Count + j);
                    byteResult = singleLevel.LevelData.HintsDataHorizontal[i][j].ColorID;
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[byteResult]));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsHorizontal.Children.Add(textBlockHint);
                }
                if(singleLevel.LevelData.HintsDataHorizontal[i].Count == 0)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = "0";
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts);
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsHorizontal.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = "0";
                    Grid.SetColumn(textBlockHint, 2 * i + 1);
                    Grid.SetRow(textBlockHint, maxSizeHintsHorizontalCounts);
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsHorizontal.Children.Add(textBlockHint);
                }
            }

            int maxSizeHintsVerticalCounts = singleLevel.LevelData.HintsDataVertical.OrderByDescending(list => list.Count()).First().Count;
            for (int i = 0; i < maxSizeHintsVerticalCounts; i++)
            {
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gStartHintsVertical.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                gEndHintsVertical.ColumnDefinitions.Add(gridColDefTemp);
            }
            for (int i = 0; i < singleLevel.LevelData.HeightY; i++)
            {
                if ((i - offsetY) % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(spacegrid);
                gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(spacegrid);
                gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                if (i == singleLevel.LevelData.HeightY - 1)
                {
                    gridRowDefTemp = new RowDefinition();
                    gridRowDefTemp.Height = new GridLength(5);
                    gStartHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                    gridRowDefTemp = new RowDefinition();
                    gridRowDefTemp.Height = new GridLength(5);
                    gEndHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                }
            }

            gStartHintsVertical.Children.Clear();
            gEndHintsVertical.Children.Clear();
            for (int j = 0; j < singleLevel.LevelData.HeightY; j++)
            {
                for (int i = 0; i < singleLevel.LevelData.HintsDataVertical[j].Count; i++)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = singleLevel.LevelData.HintsDataVertical[j][i].Value.ToString();
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts - singleLevel.LevelData.HintsDataVertical[j].Count + i);
                    Grid.SetRow(textBlockHint, 2 * ((int)singleLevel.LevelData.HeightY - j - 1) + 1);
                    byte byteResult = singleLevel.LevelData.HintsDataVertical[j][i].ColorID;
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[byteResult]));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsVertical.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = singleLevel.LevelData.HintsDataVertical[j][i].Value.ToString();
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts - singleLevel.LevelData.HintsDataVertical[j].Count + i);
                    Grid.SetRow(textBlockHint, 2 * (singleLevel.LevelData.HeightY - j - 1) + 1);
                    byteResult = singleLevel.LevelData.HintsDataVertical[j][i].ColorID;
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[byteResult]));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsVertical.Children.Add(textBlockHint);
                }
                if (singleLevel.LevelData.HintsDataVertical[j].Count == 0)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = "0";
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts);
                    Grid.SetRow(textBlockHint, 2 * ((int)singleLevel.LevelData.HeightY - j - 1) + 1);
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gStartHintsVertical.Children.Add(textBlockHint);
                    textBlockHint = new TextBlock();
                    textBlockHint.Text = "0";
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts);
                    Grid.SetRow(textBlockHint, 2 * (singleLevel.LevelData.HeightY - j - 1) + 1);
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    gEndHintsVertical.Children.Add(textBlockHint);
                }
            }
        }
        private void UpdatesGridsLayoutOfPicture()
        {
            gStartPicture.RowDefinitions.Clear();
            gStartPicture.ColumnDefinitions.Clear();
            gEndPic.RowDefinitions.Clear();
            gEndPic.ColumnDefinitions.Clear();
            ColumnDefinition gridColTemp;
            double spacegrid;
            for (int i = 0; i < singleLevel.LevelData.WidthX; i++)
            {
                if (i % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(spacegrid);
                gStartPicture.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(spacegrid);
                gEndPic.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                gStartPicture.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                gEndPic.ColumnDefinitions.Add(gridColTemp);
                if (i == singleLevel.LevelData.WidthX - 1)
                {
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    gStartPicture.ColumnDefinitions.Add(gridColTemp);
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    gEndPic.ColumnDefinitions.Add(gridColTemp);
                }
            }
            RowDefinition gridRowTemp;
            for (int i = 0; i < singleLevel.LevelData.HeightY; i++)
            {
                if ((i - offsetY) % 5 == 0 && i != 0)
                {
                    spacegrid = 15;
                }
                else
                {
                    spacegrid = 5;
                }
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(spacegrid);
                gStartPicture.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(spacegrid);
                gEndPic.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                gStartPicture.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                gEndPic.RowDefinitions.Add(gridRowTemp);
                if (i == singleLevel.LevelData.HeightY - 1)
                {
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    gStartPicture.RowDefinitions.Add(gridRowTemp);
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    gEndPic.RowDefinitions.Add(gridRowTemp);
                }
            }
        }
        private void AddCellsToGrids()
        {
            gStartPicture.Children.Clear();
            gEndPic.Children.Clear();
            Rectangle rectangleMark = new Rectangle();
            rectangleMark.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangleMark.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (singleLevel.LevelData.HeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            rectangleMark.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataMarker));
            gStartPicture.Children.Add(rectangleMark);
            rectangleMark = new Rectangle();
            rectangleMark.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangleMark.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (singleLevel.LevelData.HeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            rectangleMark.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataMarker));
            gEndPic.Children.Add(rectangleMark);
            Rectangle rectangleTempStart;
            Rectangle rectangleTempEnd;
            for (int x = 0; x < singleLevel.LevelData.WidthX; x++)
            {
                for (int y = 0; y < singleLevel.LevelData.HeightY; y++)
                {
                    rectangleTempStart = new Rectangle();
                    rectangleTempEnd = new Rectangle();
                    rectangleTempStart.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTempEnd.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTempStart.VerticalAlignment = VerticalAlignment.Stretch;
                    rectangleTempEnd.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTempStart, 2 * x + 1);
                    Grid.SetColumn(rectangleTempEnd, 2 * x + 1);
                    Grid.SetRow(rectangleTempStart, 2 * ((singleLevel.LevelData.HeightY - 1) - y) + 1);
                    Grid.SetRow(rectangleTempEnd, 2 * ((singleLevel.LevelData.HeightY - 1) - y) + 1);
                    rectangleTempStart.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    TileData tileData = singleLevel.LevelData.TilesData.Find(item => (item.PosX == x && item.PosY == y));
                    if (tileData != null)
                    {
                        rectangleTempEnd.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[tileData.ColorID]));
                    }
                    else
                    {
                        rectangleTempEnd.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    }
                    gStartPicture.Children.Add(rectangleTempStart);
                    gEndPic.Children.Add(rectangleTempEnd);
                }
            }
        }
        private void UpadteFinalPicture()
        {
            iFinal.Source = GetLevelPicture();
        }
        public WriteableBitmap GetLevelPicture()
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(singleLevel.LevelData.WidthX, singleLevel.LevelData.HeightY, 96, 96, PixelFormats.Bgra32, null);
            ///Place where this functionality is described: http://csharphelper.com/blog/2015/07/set-the-pixels-in-a-wpf-bitmap-in-c/
            byte[] pixels1d = new byte[singleLevel.LevelData.HeightY * singleLevel.LevelData.WidthX * 4];
            int index = 0;
            for (int x = 0; x < singleLevel.LevelData.HeightY; x++)
            {
                for (int y = 0; y < singleLevel.LevelData.WidthX; y++)
                {
                    TileData tileDataFound = singleLevel.LevelData.TilesData.Find(item => (item.PosX == y && item.PosY == singleLevel.LevelData.HeightY - x - 1));
                    if (tileDataFound != null)
                    {
                        byte byteResult = tileDataFound.ColorID;
                        pixels1d[index++] = singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Blue;
                        pixels1d[index++] = singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Green;
                        pixels1d[index++] = singleLevel.LevelData.ColorsDataTiles[tileDataFound.ColorID].Red;
                    }
                    else
                    {
                        pixels1d[index++] = singleLevel.LevelData.ColorDataNeutral.Blue;
                        pixels1d[index++] = singleLevel.LevelData.ColorDataNeutral.Green;
                        pixels1d[index++] = singleLevel.LevelData.ColorDataNeutral.Red;
                    }
                    pixels1d[index++] = 255;
                }
            }
            Int32Rect rect = new Int32Rect(0, 0, singleLevel.LevelData.WidthX, singleLevel.LevelData.HeightY);
            int stride = 4 * singleLevel.LevelData.WidthX;
            writeableBitmap.WritePixels(rect, pixels1d, stride, 0);
            return writeableBitmap;
        }

        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
        }
        public class ColorDetail
        {
            public byte colId { get; set; }
            public Color colName { get; set; }
            public int countOfCol { get; set; }
            public double procToAll { get; set; }
            public ColorDetail()
            {
                colId = 0;
                colName = new Color();
                countOfCol = 0;
                procToAll = 0;
            }
            public ColorDetail(byte ColorId, Color ColorName, int CountOfColor, double ProcentToAll)
            {
                colId = ColorId;
                colName = ColorName;
                countOfCol = CountOfColor;
                procToAll = ProcentToAll;
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
