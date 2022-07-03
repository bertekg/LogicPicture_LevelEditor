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
        int leftButtonEditMode, rightButtonEditMode;
        public LevelEditor(Level levelData)
        {
            InitialEditMode();
            InitializeComponent();
            level = levelData;
            UpdateBasicData();
            DrawAllLevelCells();
            DrawColorPickerOptionMode();
            UpdateEditMode();
        }
        private void InitialEditMode()
        {
            leftButtonEditMode = 0;
            rightButtonEditMode = -1;
        }

        private void UpdateEditMode()
        {
            SolidColorBrush scbWhite = new SolidColorBrush(Colors.White);
            LinearGradientBrush lgbGreenYelowToWhite90 = new LinearGradientBrush(Colors.GreenYellow, Colors.Cyan, 45.0);
            LinearGradientBrush lgbPurpleToRed90 = new LinearGradientBrush(Colors.Purple, Colors.Red, 45.0);
            for (int i = 0; i < level.ColorsDataTiles.Count; i++)
            {
                SelectionColorControl borderTemp = (SelectionColorControl)wrapPanel_ColorsForSelection.Children[i];
                if (i == leftButtonEditMode)
                {
                    borderTemp.bSampleSelectMode.BorderBrush = lgbGreenYelowToWhite90;
                }
                else if (i == rightButtonEditMode)
                {
                    borderTemp.bSampleSelectMode.BorderBrush = lgbPurpleToRed90;
                }
                else
                {
                    borderTemp.bSampleSelectMode.BorderBrush = scbWhite;
                }
                wrapPanel_ColorsForSelection.Children[i] = borderTemp;
            }
            if (leftButtonEditMode == -1)
            {
                border_ClearTileDataMode.BorderBrush = lgbGreenYelowToWhite90;
            }
            else if (rightButtonEditMode == -1)
            {
                border_ClearTileDataMode.BorderBrush = lgbPurpleToRed90;
            }
            else
            {
                border_ClearTileDataMode.BorderBrush = scbWhite;
            }
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
            wrapPanel_ColorsForSelection.Children.Clear();
            for (byte i = 0; i < level.ColorsDataTiles.Count; i++)
            {
                UsedColorControl usedColorControl = new UsedColorControl(i, level.ColorsDataTiles[i]);
                usedColorControl.colorPicker_Color.SelectedColorChanged += colorPicker_Color_SelectedColorChanged;
                usedColorControl.button_DeleteColor.Click += button_DeleteColor_Click;
                stackPanel_UsedColors.Children.Add(usedColorControl);

                SelectionColorControl selectionColorControl = new SelectionColorControl(i);
                selectionColorControl.MouseLeftButtonDown += SelectionColorControl_MouseLeftButtonDown;
                selectionColorControl.MouseRightButtonDown += SelectionColorControl_MouseRightButtonDown;
                selectionColorControl.rectangle_Color.Fill = new SolidColorBrush(Color.FromRgb(level.ColorsDataTiles[i].Red, level.ColorsDataTiles[i].Green, level.ColorsDataTiles[i].Blue));
                
                wrapPanel_ColorsForSelection.Children.Add(selectionColorControl);
            }
        }
        private void SelectionColorControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectionColorControl selectionColorControl = (SelectionColorControl)sender;
            byte currentRightModeSelected = selectionColorControl.GetID();
            if (currentRightModeSelected != leftButtonEditMode)
            {
                if (currentRightModeSelected != rightButtonEditMode)
                {
                    rightButtonEditMode = currentRightModeSelected;
                    UpdateEditMode();
                }
            }
            else
            {
                MessageBox.Show("You cannot change to same mode like left mous");
            }            
        }
        private void SelectionColorControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectionColorControl selectionColorControl = (SelectionColorControl)sender;
            byte currentLeftModeSelected = selectionColorControl.GetID();
            if (currentLeftModeSelected != rightButtonEditMode)
            {
                if (currentLeftModeSelected != leftButtonEditMode)
                {
                    leftButtonEditMode = currentLeftModeSelected;
                    UpdateEditMode();
                }
            }
            else
            {
                MessageBox.Show("You cannot change to same mode like left mous");
            }
        }
        private void border_ClearTileDataMode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int clearTileModeConst = -1;
            if (clearTileModeConst != rightButtonEditMode)
            {
                if (clearTileModeConst != leftButtonEditMode)
                {
                    leftButtonEditMode = clearTileModeConst;
                    UpdateEditMode();
                }
            }
            else
            {
                MessageBox.Show("You cannot change to same mode like left mous");
            }
        }
        private void border_ClearTileDataMode_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            int clearTileModeConst = -1;
            if (clearTileModeConst != leftButtonEditMode)
            {
                if (clearTileModeConst != rightButtonEditMode)
                {
                    rightButtonEditMode = clearTileModeConst;
                    UpdateEditMode();
                }
            }
            else
            {
                MessageBox.Show("You cannot change to same mode like left mous");
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
                DrawColorPickerOptionMode();
                UpdateEditMode();
            }
        }
        private void button_DeleteColor_Click(object sender, RoutedEventArgs e)
        {
            if (level.ColorsDataTiles.Count > 1)
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
                    if (tileData.ColorID > index)
                    {
                        tileData.ColorID -= 1;
                    }
                }
                InitialEditMode();
                DrawAllLevelCells();
                DrawColorPickerOptionMode();
                UpdateEditMode();
                MessageBox.Show("Edit Mode was rest after delete color.", "Notification about edit mode", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("You cannot delete all possible colors. Minimum on color for selection must exist.", "Limitiation for deleteing color",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void button_AddNewColor_Click(object sender, RoutedEventArgs e)
        {
            Byte[] colorByte = new Byte[3];
            Random rnd = new Random();
            rnd.NextBytes(colorByte);
            level.ColorsDataTiles.Add(new ColorData(colorByte[0], colorByte[1], colorByte[2]));
            DrawColorPickerOptionMode();
            UpdateEditMode();
        }
        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;

            TileData tileData = level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y));
            if (leftButtonEditMode == -1)
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
                    tileData.ColorID = (byte)leftButtonEditMode;
                }
                else
                {
                    level.TilesData.Add(new TileData((byte)pTempTag.X, (byte)pTempTag.Y, 
                        (byte)leftButtonEditMode));
                }
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorDataTiles((byte)leftButtonEditMode));
            }
            sender = rectangleTemp;
        }
        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = GetPointFromTag(sender);
            TileData tileData = level.TilesData.Find(item => (item.PosX == pTempTag.X && item.PosY == pTempTag.Y));
            if (rightButtonEditMode == -1)
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
                    tileData.ColorID = (byte)rightButtonEditMode;
                }
                else
                {
                    level.TilesData.Add(new TileData((byte)pTempTag.X, (byte)pTempTag.Y,
                        (byte)rightButtonEditMode));
                }
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorDataTiles((byte)rightButtonEditMode));
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

        private void ClearAllTilesData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (level != null)
            {
                if (level.TilesData.Count > 0)
                {
                    e.CanExecute = true;
                }
            }                    
        }
        private void ClearAllTilesData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want delete all Tiles Data?",
                "Confirmation befere delete all tiles data", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                level.TilesData.Clear();
                DrawAllLevelCells();
            }            
        }
    }
    public static class CustomCommands
    {
        public static readonly RoutedUICommand ClearAllTilesData = new RoutedUICommand
            ("Clear All Tiles Data", "ClearAllTilesData", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.C, ModifierKeys.Alt)
                }
            );
    }
}
