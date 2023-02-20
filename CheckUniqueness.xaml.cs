using LogicPictureLE.UserControls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace LogicPictureLE
{
    public partial class CheckUniqueness : Window
    {
        SingleLevel _singleLevel;
        List<List<Iteration>> _rowIterations, _columnIterations;
        IterationCounts _iterationCountsVertical, _iterationCountsHorizontal;
        SelctionMethod _selctionMethod;

        public CheckUniqueness()
        {
            InitializeComponent();
            Title = "Check Uniqueness - Default";
        }

        public CheckUniqueness(SingleLevel level)
        {
            InitializeComponent();
            Title = "Check Uniqueness - Single Level";
            _singleLevel = level;

            createdLevelView.Content = new VisualizationSingleLevel(_singleLevel, true, false);
            //CalcAll();
        }

        private void CalcIterations()
        {
            _rowIterations = CalcLinesNeedIteration(_singleLevel.LevelData.HintsDataVertical);
            _iterationCountsVertical = UpdateIteration(_rowIterations, _singleLevel.LevelData.HintsDataVertical, VerticalHintsIteration, "Row");
            textBox_VerticalHintsIterationCount.Text = _iterationCountsVertical.Count.ToString();
            textBox_VerticalHintsIterationCombination.Text = _iterationCountsVertical.Combination.ToString();

            _columnIterations = CalcLinesNeedIteration(_singleLevel.LevelData.HintsDataHorizontal);
            _iterationCountsHorizontal = UpdateIteration(_columnIterations, _singleLevel.LevelData.HintsDataHorizontal, HorizontalHintsIteration, "Column");
            textBox_HorizontalHintsIterationCount.Text = _iterationCountsHorizontal.Count.ToString();
            textBox_HorizontalHintsIterationCombination.Text = _iterationCountsHorizontal.Combination.ToString();

            if (_iterationCountsVertical.Combination <= _iterationCountsHorizontal.Combination)
            {
                _selctionMethod = SelctionMethod.Vertical;
            }
            else
            {
                _selctionMethod = SelctionMethod.Horizontal;
            }
            textBox_SlecectedMethod.Text = _selctionMethod.ToString();
        }

        private void CalcPossibleSolution()
        {
            if (_rowIterations == null || _columnIterations == null)
            {
                MessageBox.Show("Befor calc possible solution you must generate Iteration.", "Minning iterations infomration", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            AllSolutionsGroupBox.Children.Clear();

            int foundSame = 0;
            if (_selctionMethod == SelctionMethod.Vertical)
            {
                int[][] serchIter = new int[_iterationCountsVertical.Combination][];
                int dimension = _singleLevel.LevelData.HeightY;
                serchIter[0] = new int[dimension];
                int[,] tiles = MakeAllTiles(serchIter[0]);
                HintData[][] hintDatasHorizontal = MakeHorizontalHints(tiles);
                bool same = CheckSameHinstHorizontal(hintDatasHorizontal);
                if (same)
                {
                    foundSame++;
                    TextBlock textBlock = new TextBlock() { Text = $"Option {foundSame}" };
                    AllSolutionsGroupBox.Children.Add(textBlock);
                    Viewbox viewBase = new Viewbox() { Stretch = Stretch.Uniform };
                    Image image = new Image()
                    {
                        Stretch = Stretch.Uniform,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                    image.Source = MakePictureFromTiles(tiles);
                    viewBase.Child = image;
                    AllSolutionsGroupBox.Children.Add(viewBase);
                }
                int[] prev = serchIter[0];
                
                for (int i = 1; i < _iterationCountsVertical.Combination; i++)
                {
                    serchIter[i] = new int[dimension];
                    prev = serchIter[i - 1];
                    bool makeInc = true;
                    int incIndex = 0;
                    while (makeInc)
                    {
                        int newInc = prev[incIndex] + 1;
                        if (newInc < _rowIterations[incIndex].Count)
                        {
                            for (int j = 0; j < prev.Length; j++)
                            {
                                if (j == incIndex)
                                {
                                    serchIter[i][j] = newInc;
                                }
                                else if (j < incIndex)
                                {
                                    serchIter[i][j] = 0;
                                }
                                else
                                {
                                    serchIter[i][j] = prev[j];
                                }
                                makeInc = false;
                            }
                        }
                        else
                        {
                            incIndex++;
                        }
                    }
                    tiles = MakeAllTiles(serchIter[i]);
                    hintDatasHorizontal = MakeHorizontalHints(tiles);
                    same = CheckSameHinstHorizontal(hintDatasHorizontal);
                    if (same)
                    {
                        foundSame++;
                        TextBlock textBlock = new TextBlock() { Text = $"Option {foundSame}" };
                        AllSolutionsGroupBox.Children.Add(textBlock);
                        Viewbox viewBase = new Viewbox() { Stretch = Stretch.Uniform };
                        Image image = new Image() { Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch};
                        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                        image.Source = MakePictureFromTiles(tiles);
                        viewBase.Child = image;
                        AllSolutionsGroupBox.Children.Add(viewBase);
                    }
                }
            }
            textBox_SolutionCount.Text = foundSame.ToString();
            for (int i = 0; i < foundSame; i++)
            {
                TextBlock textBlock = new TextBlock() { Text = $"Option {i+1}" };
            }
        }

        private bool CheckSameHinstHorizontal(HintData[][] hintDatasHorizontal)
        {
            bool same = true;
            for (int i = 0; i < hintDatasHorizontal.Length; i++)
            {
                if (hintDatasHorizontal[i].Length != _singleLevel.LevelData.HintsDataHorizontal[i].Length)
                {
                    same = false;
                }
            }
            if (same)
            {
                for (int i = 0; i < hintDatasHorizontal.Length; i++)
                {
                    for (int j = 0; j < hintDatasHorizontal[i].Length; j++)
                    {
                        if (hintDatasHorizontal[i][j].ColorID != _singleLevel.LevelData.HintsDataHorizontal[i][j].ColorID ||
                            hintDatasHorizontal[i][j].Value != _singleLevel.LevelData.HintsDataHorizontal[i][j].Value)
                        {
                            same = false;
                        }
                    }
                }
            }
            return same;
        }

        private HintData[][] MakeHorizontalHints(int[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            HintData[][] hintDatasVerical = new HintData[width][];
            for (byte x = 0; x < width; x++)
            {
                int prevCellId = -1;
                byte currentIdCombo = 0;
                List<HintData> horizontalHints = new List<HintData>();
                for (int y = height; y > 0; y--)
                {
                    int colorID = tiles[x, y - 1];
                    if (colorID >= 0)
                    {
                        if (colorID == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId >= 0)
                        {
                            HintData hint = new HintData();
                            hint.ColorID = (byte)(prevCellId);
                            hint.Value = currentIdCombo;
                            horizontalHints.Add(hint);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = colorID;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId);
                            hdTemp.Value = currentIdCombo;
                            horizontalHints.Add(hdTemp);
                            currentIdCombo = 0;
                            prevCellId = -1;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.ColorID = (byte)(prevCellId);
                    hdTemp.Value = currentIdCombo;
                    horizontalHints.Add(hdTemp);
                }
                if (horizontalHints.Count == 0)
                {
                    horizontalHints.Add(new HintData(0, 0));
                }
                hintDatasVerical[x] = ConvertListToArray(horizontalHints);
            }
            return hintDatasVerical;
        }

        private HintData[] ConvertListToArray(List<HintData> listHints)
        {
            HintData[] arrayHints = new HintData[listHints.Count];
            for (int i = 0; i < listHints.Count; i++)
            {
                arrayHints[i] = listHints[i];
            }
            return arrayHints;
        }

        private int[,] MakeAllTiles(int[] serchIter)
        {
            int[,] tiles = new int[_singleLevel.LevelData.WidthX, _singleLevel.LevelData.HeightY];
            for (int i = 0; i < _singleLevel.LevelData.WidthX; i++)
            {
                for (int j = 0; j < _singleLevel.LevelData.HeightY; j++)
                {
                    if (_rowIterations[j][serchIter[j]].Cells[i])
                    {
                        tiles[i, j] = 0;
                    }
                    else
                    {
                        tiles[i, j] = -1;
                    }
                }
            }
            return tiles;
        }

        private void CalcAll()
        {
            CalcIterations();
            CalcPossibleSolution();
        }

        private IterationCounts UpdateIteration(List<List<Iteration>> allIterations, HintData[][] hintDatas, GroupBox displayGorupBox, string nameType)
        {
            IterationCounts iterationCounts = new IterationCounts();
            TreeView treeView = new TreeView();
            iterationCounts.Combination = 1;
            for (int i = 0; i < allIterations.Count; i++)
            {
                TreeViewItem lineLevel = new TreeViewItem();
                string headerLine = $"{nameType} {i + 1}, contains {allIterations[i].Count} iterations, Valuses:";
                iterationCounts.Combination *= allIterations[i].Count;
                for (int j = 0; j < hintDatas[i].Length; j++)
                {
                    headerLine += $" {hintDatas[i][j].Value}";
                }
                lineLevel.Header = headerLine;
                for (int j = 0; j < allIterations[i].Count; j++)
                {
                    TreeViewItem iterationsLevel = new TreeViewItem();
                    string headerIteration = $"Iteration {j + 1}: ";
                    for (int k = 0; k < allIterations[i][j].Cells.Length; k++)
                    {
                        headerIteration += DecodeBool(allIterations[i][j].Cells[k]);
                    }
                    iterationsLevel.Header = headerIteration;
                    lineLevel.IsExpanded = true;
                    lineLevel.Items.Add(iterationsLevel);
                    iterationCounts.Count++;
                }
                lineLevel.IsExpanded = true;
                treeView.Items.Add(lineLevel);
            }
            displayGorupBox.Content = treeView;
            return iterationCounts;
        }

        public struct IterationCounts
        {
            public int Count;
            public int Combination;
        }

        private char DecodeBool(bool boolValue)
        {
            if (boolValue) return 'O';
            else return 'X';
        }

        private List<List<Iteration>> CalcLinesNeedIteration(HintData[][] hintDatas)
        {
            List<List<Iteration>> lines = new List<List<Iteration>>();
            int width = hintDatas.Length;
            foreach (HintData[] hints in hintDatas)
            {
                List<Iteration> iterations = new List<Iteration>();

                int sum = 0;
                for (int j = 0; j < hints.Length; j++)
                {
                    sum += hints[j].Value;
                    if (j < hints.Length - 1)
                    {
                        sum++;
                    }
                }

                int[] startsInitialIndex = new int[hints.Length];
                int flyIndex = 0;
                for (int i = 0; i < hints.Length; i++)
                {
                    startsInitialIndex[i] = flyIndex;
                    flyIndex += hints[i].Value + 1;
                }
                int[] indexOffset = new int[hints.Length];
                int maxIndex = startsInitialIndex.Length - 1;
                bool looping = true;
                while (looping)
                {
                    if (startsInitialIndex.Length <= 0 || hints[0].Value == 0) 
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
                lines.Add(iterations);
            }
            return lines;
        }

        private WriteableBitmap MakePictureFromTiles(int[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            byte[] pixels1d = new byte[height * width * 4];
            int index = 0;
            for (int vertical = 0; vertical < height; vertical++)
            {
                for (int horizontal = 0; horizontal < width; horizontal++)
                {
                    //TileData tileDataFound = _singleLevel.LevelData.TilesData[horizontal][height - vertical - 1];
                    int colorId = tiles[horizontal,height - vertical - 1];
                    if (colorId >= 0)
                    {
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[colorId].Blue;
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[colorId].Green;
                        pixels1d[index++] = _singleLevel.LevelData.ColorsDataTiles[colorId].Red;
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
            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;
            writeableBitmap.WritePixels(rect, pixels1d, stride, 0);
            return writeableBitmap;
        }

        private void commandBinding_CalcIteration_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_CalcIteration_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcIterations();
        }

        private void commandBinding_CalcSolutions_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_rowIterations == null || _columnIterations == null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }

        private void commandBinding_CalcSolutions_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcPossibleSolution();
        }

        private void commandBinding_CalcAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_CalcAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcAll();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Escape || e.Key == Key.Enter || e.Key == Key.Back)
            {
                Close();
            }
        }
    }

    public class Iteration
    {
        public bool[] Cells { get; set; }
    }
    public enum SelctionMethod { None, Vertical, Horizontal }

    public static class CustomCommandsUniq
    {
        public static readonly RoutedUICommand CalcIteration = new RoutedUICommand
            ("Calc Iterations", "CalcIterations", typeof(CustomCommandsUniq),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D1, ModifierKeys.Alt)
            } );
        public static readonly RoutedUICommand CalcSolutions = new RoutedUICommand
            ("Calc Solutions", "CalcSolutions", typeof(CustomCommandsUniq),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D2, ModifierKeys.Alt)
            } );
        public static readonly RoutedUICommand CalcAll = new RoutedUICommand
            ("Calc All", "CalcAll", typeof(CustomCommandsUniq),
            new InputGestureCollection()
            {
                new KeyGesture(Key.D3, ModifierKeys.Alt)
            });
    }
}