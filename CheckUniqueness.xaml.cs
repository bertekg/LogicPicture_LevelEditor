using LogicPictureLE.UserControls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogicPictureLE
{
    public partial class CheckUniqueness : Window
    {
        SingleLevel _singleLevel;
        List<List<Iteration>> _rowIterations, _columnIterations;

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
            _rowIterations = CalcLinesNeedIteration(_singleLevel.LevelData.HintsDataVertical);
            int totalVerticalPossibilities = UpdateIteration(_rowIterations, _singleLevel.LevelData.HintsDataVertical, VerticalHintsIteration, "Row");
            textBox_VerticalHintsIterationTotal.Text = totalVerticalPossibilities.ToString();

            _columnIterations = CalcLinesNeedIteration(_singleLevel.LevelData.HintsDataHorizontal);
            int totalHorizontalPossibilities = UpdateIteration(_columnIterations, _singleLevel.LevelData.HintsDataHorizontal, HorizontalHintsIteration, "Column");
            textBox_HorizontalHintsIterationTotal.Text = totalHorizontalPossibilities.ToString();
        }

        private int UpdateIteration(List<List<Iteration>> allIterations, HintData[][] hintDatas, GroupBox displayGorupBox, string nameType)
        {
            TreeView treeView = new TreeView();
            int totalPossibilities = 1;
            for (int i = 0; i < allIterations.Count; i++)
            {
                TreeViewItem lineLevel = new TreeViewItem();
                string headerLine = $"{nameType} {i + 1}, contains {allIterations[i].Count} iterations, Valuses:";
                totalPossibilities *= allIterations[i].Count;
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
                }
                lineLevel.IsExpanded = true;
                treeView.Items.Add(lineLevel);
            }
            displayGorupBox.Content = treeView;
            return totalPossibilities;
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