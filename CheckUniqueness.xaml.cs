using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Interaction logic for CheckUniqueness.xaml
    /// </summary>
    public partial class CheckUniqueness : Window
    {
        SingleLevel singleLevel;
        int offsetY;
        public CheckUniqueness()
        {
            InitializeComponent();
            Window_CheckUniqueness.Title = "Check Uniqueness - Default";
            
        }
        public CheckUniqueness(SingleLevel level)
        {
            InitializeComponent();
            Window_CheckUniqueness.Title = "Check Uniqueness - Single Level";
            singleLevel = level;
            offsetY = singleLevel.LevelData.HeightY % 5;
            inputResultGrid.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            AddHintsToGrid();
            UpdatesGridLayoutOfPicture();
            AddCellsToGrids();
        }
        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Size: {singleLevel.LevelData.WidthX},{singleLevel.LevelData.HeightY}");
        }
        private void AddHintsToGrid()
        {
            inputResultHintsHorizontal.ColumnDefinitions.Clear();
            inputResultHintsHorizontal.RowDefinitions.Clear();
            inputResultHintsVertical.ColumnDefinitions.Clear();
            inputResultHintsVertical.RowDefinitions.Clear();
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
                inputResultHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                inputResultHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                if (i == singleLevel.LevelData.WidthX - 1)
                {
                    gridColDefTemp = new ColumnDefinition();
                    gridColDefTemp.Width = new GridLength(5);
                    inputResultHintsHorizontal.ColumnDefinitions.Add(gridColDefTemp);
                }
            }

            int maxSizeHintsHorizontalCounts = singleLevel.LevelData.HintsDataHorizontal.OrderByDescending(list => list.Count()).First().Count;
            RowDefinition gridRowDefTemp;
            for (int i = 0; i < maxSizeHintsHorizontalCounts; i++)
            {
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                inputResultHintsVertical.RowDefinitions.Add(gridRowDefTemp);
            }
            inputResultHintsVertical.Children.Clear();
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
                    inputResultHintsHorizontal.Children.Add(textBlockHint);
                }
                if (singleLevel.LevelData.HintsDataHorizontal[i].Count == 0)
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
                    inputResultHintsHorizontal.Children.Add(textBlockHint);
                }
            }

            int maxSizeHintsVerticalCounts = singleLevel.LevelData.HintsDataVertical.OrderByDescending(list => list.Count()).First().Count;
            for (int i = 0; i < maxSizeHintsVerticalCounts; i++)
            {
                gridColDefTemp = new ColumnDefinition();
                gridColDefTemp.Width = new GridLength(50);
                inputResultHintsVertical.ColumnDefinitions.Add(gridColDefTemp);
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
                inputResultHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                gridRowDefTemp = new RowDefinition();
                gridRowDefTemp.Height = new GridLength(50);
                inputResultHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                if (i == singleLevel.LevelData.HeightY - 1)
                {
                    gridRowDefTemp = new RowDefinition();
                    gridRowDefTemp.Height = new GridLength(5);
                    inputResultHintsVertical.RowDefinitions.Add(gridRowDefTemp);
                }
            }

            inputResultHintsVertical.Children.Clear();
            for (int j = 0; j < singleLevel.LevelData.HeightY; j++)
            {
                for (int i = 0; i < singleLevel.LevelData.HintsDataVertical[j].Count; i++)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = singleLevel.LevelData.HintsDataVertical[j][i].Value.ToString();
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts - singleLevel.LevelData.HintsDataVertical[j].Count + i);
                    Grid.SetRow(textBlockHint, 2 * (singleLevel.LevelData.HeightY - j - 1) + 1);
                    byte byteResult = singleLevel.LevelData.HintsDataVertical[j][i].ColorID;
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[byteResult]));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    inputResultHintsVertical.Children.Add(textBlockHint);
                }
                if (singleLevel.LevelData.HintsDataVertical[j].Count == 0)
                {
                    TextBlock textBlockHint = new TextBlock();
                    textBlockHint.Text = "0";
                    Grid.SetColumn(textBlockHint, maxSizeHintsVerticalCounts);
                    Grid.SetRow(textBlockHint, 2 * (singleLevel.LevelData.HeightY - j - 1) + 1);
                    textBlockHint.Foreground = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    textBlockHint.FontSize = 35;
                    textBlockHint.FontWeight = FontWeights.Bold;
                    textBlockHint.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockHint.VerticalAlignment = VerticalAlignment.Center;
                    inputResultHintsVertical.Children.Add(textBlockHint);
                }
            }
        }
        private void UpdatesGridLayoutOfPicture()
        {
            inputResultGrid.RowDefinitions.Clear();
            inputResultGrid.ColumnDefinitions.Clear();
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
                inputResultGrid.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                inputResultGrid.ColumnDefinitions.Add(gridColTemp);
                if (i == singleLevel.LevelData.WidthX - 1)
                {
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    inputResultGrid.ColumnDefinitions.Add(gridColTemp);
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
                inputResultGrid.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                inputResultGrid.RowDefinitions.Add(gridRowTemp);
                if (i == singleLevel.LevelData.HeightY - 1)
                {
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    inputResultGrid.RowDefinitions.Add(gridRowTemp);
                }
            }
        }
        private void AddCellsToGrids()
        {
            inputResultGrid.Children.Clear();
            Rectangle rectangleMark = new Rectangle();
            rectangleMark.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangleMark.VerticalAlignment = VerticalAlignment.Stretch;
            Grid.SetColumn(rectangleMark, 0);
            Grid.SetRow(rectangleMark, 2 * (singleLevel.LevelData.HeightY - 1));
            Grid.SetColumnSpan(rectangleMark, 3);
            Grid.SetRowSpan(rectangleMark, 3);
            rectangleMark.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataMarker));
            inputResultGrid.Children.Add(rectangleMark);
            Rectangle rectangleTemp;
            for (int x = 0; x < singleLevel.LevelData.WidthX; x++)
            {
                for (int y = 0; y < singleLevel.LevelData.HeightY; y++)
                {
                    rectangleTemp = new Rectangle();
                    rectangleTemp.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTemp.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTemp, 2 * x + 1);
                    Grid.SetRow(rectangleTemp, 2 * ((singleLevel.LevelData.HeightY - 1) - y) + 1);
                    TileData tileData = singleLevel.LevelData.TilesData.Find(item => (item.PosX == x && item.PosY == y));
                    if (tileData != null)
                    {
                        rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[tileData.ColorID]));
                    }
                    else
                    {
                        rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    }
                    inputResultGrid.Children.Add(rectangleTemp);
                }
            }
        }
    }
}
