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
    public partial class LevelEditor : UserControl
    {
        Level level;
        public LevelEditor(Level levelData)
        {
            InitializeComponent();
            level = levelData;
            UpdateBasicData();
            DrawAllLevelCells();
        }
        public Level GetLevelData()
        {
            return level;
        }
        private void UpdateBasicData()
        {
            xctkByteUpDown_LevelWidth.Value = level.WidthX;
            xctkByteUpDown_LevelHeight.Value = level.HeightY;
            rectangle_Background.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataBackground));
        }
        private void DrawAllLevelCells()
        {
            gMainPlaceGrid.Children.Clear();
            gMainPlaceGrid.RowDefinitions.Clear();
            gMainPlaceGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < level.WidthX; i++)
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(20);
                gMainPlaceGrid.ColumnDefinitions.Add(gridCol);
            }
            for (int i = 0; i < level.HeightY; i++)
            {
                RowDefinition gridRow = new RowDefinition();
                gridRow.Height = new GridLength(20);
                gMainPlaceGrid.RowDefinitions.Add(gridRow);
            }
            for (int i = 0; i < level.WidthX; i++)
            {
                for (int j = 0; j < level.HeightY; j++)
                {
                    Rectangle rectangleTemp = new Rectangle();
                    Point newPoint = new Point(i, j);
                    rectangleTemp.Tag = newPoint;
                    rectangleTemp.MouseLeftButtonDown += RectangleTemp_MouseLeftButtonDown;
                    rectangleTemp.MouseRightButtonDown += RectangleTemp_MouseRightButtonDown;
                    rectangleTemp.MouseEnter += RectangleTemp_MouseEnter;
                    rectangleTemp.HorizontalAlignment = HorizontalAlignment.Stretch;
                    rectangleTemp.VerticalAlignment = VerticalAlignment.Stretch;
                    rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
                    Grid.SetColumn(rectangleTemp, i);
                    Grid.SetRow(rectangleTemp, (level.HeightY - 1) - j);
                    gMainPlaceGrid.Children.Add(rectangleTemp);
                }
            }            
        }
        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;
            //MessageBox.Show("You press left mouse button on cell: " + pTempTag.ToString());
            if (level.TilesData.Any(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y)))
            {
                level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y)).ColorID = 1;
            }
            else
            {
                level.TilesData.Add(new TileData((byte)pTempTag.X, (byte)pTempTag.Y, 1));
            }
            rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorDataTiles(0));
            sender = rectangleTemp;
        }

        private Color GetColorFromColorDataTiles(byte index)
        {
            return Color.FromRgb(level.ColorsDataTiles[index].Red,
                level.ColorsDataTiles[index].Green,
                level.ColorsDataTiles[index].Blue);
        }

        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = GetPointFromTag(sender);
            //MessageBox.Show("You press right mouse button on cell: " + pTempTag.ToString());
            if (level.TilesData.Any(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y)))
            {
                level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y)).ColorID = 0;
            }
            else
            {
                level.TilesData.Add(new TileData((byte)pTempTag.X, (byte)pTempTag.Y, 0));
            }
            rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
            sender = rectangleTemp;
        }
        private Color GetColorFromColorData(ColorData colorData)
        {
            return Color.FromRgb(colorData.Red, colorData.Green, colorData.Blue);
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
            level.WidthX = xctkByteUpDown_LevelWidth.Value.Value;
            level.HeightY = xctkByteUpDown_LevelHeight.Value.Value;
            DrawAllLevelCells();
        }
    }
}
