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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicPictureLE.UserControls
{
    public partial class SingleLevelEditor : UserControl
    {
        SingleLevel singleLevel;
        public SingleLevelEditor(SingleLevel levelData)
        {
            InitializeComponent();
            singleLevel = levelData;
            UpdateBasicData();
            DrawAllLevelCells();
        }
        public SingleLevel GetSingleLevel()
        {
            return singleLevel;
        }
        private void UpdateBasicData()
        {
            textBox_LevelName.Text = singleLevel.Name;
            xctkByteUpDown_LevelWidth.Value = singleLevel.Level.WidthX;
            xctkByteUpDown_LevelHeight.Value = singleLevel.Level.HeightY;
        }
        private void DrawAllLevelCells()
        {
            gMainPlaceGrid.Children.Clear();
            gMainPlaceGrid.RowDefinitions.Clear();
            gMainPlaceGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < singleLevel.Level.WidthX; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(20);
                gMainPlaceGrid.ColumnDefinitions.Add(gridCol);
            }
            for (int i = 0; i < singleLevel.Level.HeightY; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(20);
                gMainPlaceGrid.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < singleLevel.Level.WidthX; i++)
            {
                for (int j = 0; j < singleLevel.Level.HeightY; j++)
                {
                    Rectangle rectangleTemp = new Rectangle();
                    Point newPoint = new Point(i, j);
                    rectangleTemp.Tag = newPoint;
                    rectangleTemp.MouseLeftButtonDown += RectangleTemp_MouseLeftButtonDown;
                    rectangleTemp.MouseRightButtonDown += RectangleTemp_MouseRightButtonDown;
                    rectangleTemp.MouseEnter += RectangleTemp_MouseEnter;
                    rectangleTemp.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTemp.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetColumn(rectangleTemp, i);
                    Grid.SetRow(rectangleTemp, (singleLevel.Level.HeightY - 1) - j);
                    rectangleTemp.Fill = new SolidColorBrush(Colors.White);
                    gMainPlaceGrid.Children.Add(rectangleTemp);
                }
            }            
        }
        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pTempTag = GetPointFromTag(sender);
            MessageBox.Show("You press left mouse button on cell: " + pTempTag.ToString());
        }
        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pTempTag = GetPointFromTag(sender);
            MessageBox.Show("You press right mouse button on cell: " + pTempTag.ToString());
        }
        private static Point GetPointFromTag(object sender)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;
            return pTempTag;
        }
        private void RectangleTemp_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        private void commandBinding_Update_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (xctkByteUpDown_LevelWidth.Value.HasValue && xctkByteUpDown_LevelWidth.Value.HasValue)
            {
                e.CanExecute = true;
            }            
        }
        private void commandBinding_Update_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            singleLevel.Level.WidthX = xctkByteUpDown_LevelWidth.Value.Value;
            singleLevel.Level.HeightY = xctkByteUpDown_LevelHeight.Value.Value;
            DrawAllLevelCells();
        }
    }
}
