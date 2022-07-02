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
            DrawColorPickerOptionMode();
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
                    Grid.SetColumn(rectangleTemp, i);
                    Grid.SetRow(rectangleTemp, (level.HeightY - 1) - j);

                    if (level.TilesData.Any(item => (item.PosX == i && item.PosY == j)))
                    {
                        byte colorId = level.TilesData.Find(item => item.PosX == i && item.PosY == j).ColorID;
                        rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorsDataTiles[colorId]));
                    }
                    else
                    {
                        rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
                    }

                    gMainPlaceGrid.Children.Add(rectangleTemp);
                }
            }            
        }        
        private void DrawColorPickerOptionMode()
        {
            stackPanel_UsedColors.Children.Clear();
            for (byte i = 0; i < level.ColorsDataTiles.Count; i++)
            {
                UsedColorControl usedColorControl = new UsedColorControl(i, level.ColorsDataTiles[i]);
                usedColorControl.colorPicker_Color.SelectedColorChanged += colorPicker_Color_SelectedColorChanged;
                usedColorControl.button_DeleteColor.Click += button_DeleteColor_Click;
                stackPanel_UsedColors.Children.Add(usedColorControl);
            }
        }

        private void colorPicker_Color_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Xceed.Wpf.Toolkit.ColorPicker colorPicker = (Xceed.Wpf.Toolkit.ColorPicker)sender;
            byte index = (byte)colorPicker.Tag;
            Color? color = colorPicker.SelectedColor;
            if(color != null)
            {
                level.ColorsDataTiles[index] = GetColorDataFromColor(color.Value);
                if (level.TilesData.Any(item => item.ColorID == index))
                {
                    DrawAllLevelCells();
                }
            }
        }
        private void button_DeleteColor_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            byte index = (byte)button.Tag;
            List<TileData> tiles = level.TilesData.FindAll(item => item.ColorID == index);
            if (tiles.Count > 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("For color to delete some Tiles use it.\n" +
                    "When you delete color all connected tiles will be deleted too.\n" +
                    "Are you sure you want delet this color?", 
                    "Confirmation for delete color", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            level.ColorsDataTiles.RemoveAt(index);
            level.TilesData.RemoveAll(item => item.ColorID == index);
            foreach (TileData tileData in level.TilesData)
            {
                if(tileData.ColorID > index)
                {
                    tileData.ColorID -= 1;
                }
            }
            DrawAllLevelCells();
            DrawColorPickerOptionMode();
        }
        private void button_AddNewColor_Click(object sender, RoutedEventArgs e)
        {
            Byte[] colorByte = new Byte[3];
            Random rnd = new Random();
            rnd.NextBytes(colorByte);
            level.ColorsDataTiles.Add(new ColorData(colorByte[0], colorByte[1], colorByte[2]));
            DrawColorPickerOptionMode();
        }
        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;

            TileData tileData = level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y));
            if (SingleUpDown_LeftMouse.Value.Value == -1)
            {
                if (tileData != null)
                {
                    level.TilesData.Remove(tileData);
                    rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
                }
            }
            else
            {
                if (tileData != null)
                {
                    tileData.ColorID = (byte)SingleUpDown_LeftMouse.Value.Value;
                }
                else
                {
                    level.TilesData.Add(new TileData((byte)pTempTag.X, (byte)pTempTag.Y, 
                        (byte)SingleUpDown_LeftMouse.Value.Value));
                }
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorDataTiles((byte)SingleUpDown_LeftMouse.Value.Value));
            }
            sender = rectangleTemp;
        }

        private Color GetColorFromColorDataTiles(byte index)
        {
            return Color.FromRgb(level.ColorsDataTiles[index].Red,
                level.ColorsDataTiles[index].Green,
                level.ColorsDataTiles[index].Blue);
        }
        private ColorData GetColorDataFromColor(Color color)
        {
            return new ColorData(color.R, color.G, color.B);
        }
        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = GetPointFromTag(sender);
            //MessageBox.Show("You press right mouse button on cell: " + pTempTag.ToString());
            TileData tileData = level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y));
            if (tileData != null)
            {                
                level.TilesData.Remove(tileData);
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
            }           
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
