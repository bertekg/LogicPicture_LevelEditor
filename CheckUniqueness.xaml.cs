using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
           
            inputResultBackground.Background = new SolidColorBrush(GetColorFromColorData(level.LevelData.ColorDataBackground));
            offsetY = singleLevel.LevelData.HeightY % 5;
            AddHintsToGrid();
            UpdatesGridsLayoutOfPicture();
            AddCellsToGrids();
            ButtonFunction();
        }
        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonFunction();
        }

        private void ButtonFunction()
        {
            List<List<Iteration>> rowIterations = CalcRowsNeedIteration();
            int totalPossibilities = 1;
            for (int i = 0; i < rowIterations.Count; i++)
            {
                Debug.Write($"Row {i}, contains {rowIterations[i].Count} iterations, Valuse:");
                totalPossibilities *= rowIterations[i].Count;
                for (int j = 0; j < singleLevel.LevelData.HintsDataVertical[i].Count; j++)
                {
                    Debug.Write($" {singleLevel.LevelData.HintsDataVertical[i][j].Value}");
                }
                Debug.WriteLine(".");
                for (int j = 0; j < rowIterations[i].Count; j++)
                {
                    Debug.Write($"Row {i}, Iteration {j}:");
                    for (int k = 0; k < rowIterations[i][j].Cells.Length; k++)
                    {
                        Debug.Write(DecodeBool(rowIterations[i][j].Cells[k]));
                    }
                    Debug.WriteLine(".");
                }
            }
            Debug.WriteLine($"Total possibilites: {totalPossibilities}");
        }

        private char DecodeBool(bool boolValue)
        {
            if (boolValue) return 'O';
            else return 'X';
        }

        private List<List<Iteration>> CalcRowsNeedIteration()
        {
            List<List<Iteration>> rows = new List<List<Iteration>>();
            int width = singleLevel.LevelData.WidthX;
            foreach (List<HintData> hints in singleLevel.LevelData.HintsDataVertical)
            {
                List<Iteration> iterations = new List<Iteration>();

                int sum = 0;
                for (int j = 0; j < hints.Count; j++)
                {
                    sum += hints[j].Value;
                    if (j < hints.Count - 1)
                    {
                        sum++;
                    }
                }
                int left = width - sum;

                int[] startsInitialIndex = new int[hints.Count];
                int flyIndex = 0;
                for (int i = 0; i < hints.Count; i++)
                {
                    startsInitialIndex[i] = flyIndex;
                    flyIndex += hints[i].Value + 1;
                }
                int[] indexOffset = new int[hints.Count];
                int maxIndex = startsInitialIndex.Length - 1;
                bool looping = true;
                while (looping)
                {
                    if (startsInitialIndex.Length <= 0) 
                    {
                        Iteration iterationZero = new Iteration();
                        bool[] cellsZero = new bool[width];
                        iterationZero.Cells = cellsZero;
                        iterations.Add(iterationZero);
                        break; }

                    if ((startsInitialIndex[0] + indexOffset[0] + sum) >= width)
                        looping = false;

                    Iteration iteration = new Iteration();
                    bool[] cells = new bool[width];
                    for (int i = 0; i < startsInitialIndex.Length; i++)
                    {
                        for (int j = 0; j < hints[i].Value; j++)
                        {
                            cells[startsInitialIndex[i] + indexOffset[i] + j] = true;
                        }
                    }
                    iteration.Cells = cells;
                    iterations.Add(iteration);

                    indexOffset[maxIndex]++;
                    if (startsInitialIndex[maxIndex] + indexOffset[maxIndex] + hints[maxIndex].Value > width)
                    {
                        bool shift = true;
                        int indexShift = maxIndex;
                        while (shift)
                        {
                            if (indexShift > 0)
                            {
                                int temp = startsInitialIndex[indexShift] + 1;
                                for (int k = indexShift; k < maxIndex; k++)
                                {
                                    temp += 1 + startsInitialIndex[k + 1];
                                }
                                if (temp < width)
                                {
                                    indexShift--;
                                }
                                else 
                                {
                                    shift = false;
                                }
                            }
                            else 
                            {
                                shift = false;
                            }
                        }
                        startsInitialIndex[indexShift]++;
                        for (int i = indexShift + 1; i <= maxIndex; i++)
                        {
                            startsInitialIndex[i] = startsInitialIndex[i - 1] + 1 + hints[i - 1].Value;
                        }
                        for (int i = 0; i <= maxIndex; i++)
                        {
                            indexOffset[i] = 0;
                        }
                    }
                }
                rows.Add(iterations);
            }
            return rows;
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
                inputResultHintsHorizontal.RowDefinitions.Add(gridRowDefTemp);
            }
            inputResultHintsHorizontal.Children.Clear();
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
        private void UpdatesGridsLayoutOfPicture()
        {
            inputResultPicture.RowDefinitions.Clear();
            inputResultPicture.ColumnDefinitions.Clear();
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
                inputResultPicture.ColumnDefinitions.Add(gridColTemp);
                gridColTemp = new ColumnDefinition();
                gridColTemp.Width = new GridLength(50);
                inputResultPicture.ColumnDefinitions.Add(gridColTemp);
                if (i == singleLevel.LevelData.WidthX - 1)
                {
                    gridColTemp = new ColumnDefinition();
                    gridColTemp.Width = new GridLength(5);
                    inputResultPicture.ColumnDefinitions.Add(gridColTemp);
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
                inputResultPicture.RowDefinitions.Add(gridRowTemp);
                gridRowTemp = new RowDefinition();
                gridRowTemp.Height = new GridLength(50);
                inputResultPicture.RowDefinitions.Add(gridRowTemp);
                if (i == singleLevel.LevelData.HeightY - 1)
                {
                    gridRowTemp = new RowDefinition();
                    gridRowTemp.Height = new GridLength(5);
                    inputResultPicture.RowDefinitions.Add(gridRowTemp);
                }
            }
        }
        private void AddCellsToGrids()
        {
            inputResultPicture.Children.Clear();
            Rectangle rectangleTempEnd;
            for (int x = 0; x < singleLevel.LevelData.WidthX; x++)
            {
                for (int y = 0; y < singleLevel.LevelData.HeightY; y++)
                {
                    rectangleTempEnd = new Rectangle();
                    rectangleTempEnd.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTempEnd.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTempEnd, 2 * x + 1);
                    Grid.SetRow(rectangleTempEnd, 2 * ((singleLevel.LevelData.HeightY - 1) - y) + 1);
                    TileData tileData = singleLevel.LevelData.TilesData.Find(item => (item.PosX == x && item.PosY == y));
                    if (tileData != null)
                    {
                        rectangleTempEnd.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorsDataTiles[tileData.ColorID]));
                    }
                    else
                    {
                        rectangleTempEnd.Fill = new SolidColorBrush(GetColorFromColorData(singleLevel.LevelData.ColorDataNeutral));
                    }
                    inputResultPicture.Children.Add(rectangleTempEnd);
                }
            }
        }
    }
    public class Iteration
    {
        public bool[] Cells { get; set; }
    }
}