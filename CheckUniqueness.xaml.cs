using LogicPictureLE.UserControls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace LogicPictureLE
{
    public partial class CheckUniqueness : Window
    {
        SingleLevel _singleLevel;

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
            ButtonFunction();
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
                for (int j = 0; j < _singleLevel.LevelData.HintsDataVertical[i].Length; j++)
                {
                    Debug.Write($" {_singleLevel.LevelData.HintsDataVertical[i][j].Value}");
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
            int width = _singleLevel.LevelData.WidthX;
            foreach (HintData[] hints in _singleLevel.LevelData.HintsDataVertical)
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
                rows.Add(iterations);
            }
            return rows;
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
}