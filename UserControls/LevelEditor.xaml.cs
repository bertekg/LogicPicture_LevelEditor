using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            cpBackgroundColor.SelectedColor = GetColorFromColorData(level.ColorDataBackground);
            cpTilesNaturalColor.SelectedColor = GetColorFromColorData(level.ColorDataNeutral);
            cpMarkerColor.SelectedColor = GetColorFromColorData(level.ColorDataMarker);
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
            for (int i = 0; i < level.ColorsDataTiles.Length; i++)
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

                    if (level.TilesData[i][j].IsSelected)
                    {
                        int colorId = level.TilesData[i][j].ColorID;
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
            for (byte i = 0; i < level.ColorsDataTiles.Length; i++)
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
                MessageBox.Show("You cannot change to same mode like left mouse");
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
                MessageBox.Show("You cannot change to same mode like right mouse");
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
                MessageBox.Show("You cannot change to same mode like left mouse");
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
                MessageBox.Show("You cannot change to same mode like right mouse");
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
                bool isContaining = LevelContainColor(index);
                if (isContaining)
                {
                    DrawAllLevelCells();                    
                }
                DrawColorPickerOptionMode();
                UpdateEditMode();
            }
        }

        private bool LevelContainColor(byte index)
        {
            bool isContaining = false;
            for (int i = 0; i < level.WidthX; i++)
            {
                for (int j = 0; j < level.HeightY; j++)
                {
                    if (level.TilesData[i][j].IsSelected && level.TilesData[i][j].ColorID == index)
                    {
                        isContaining = true;
                        break;
                    }
                }
                if (isContaining)
                {
                    break;
                }
            }
            return isContaining;
        }

        private void button_DeleteColor_Click(object sender, RoutedEventArgs e)
        {
            if (level.ColorsDataTiles.Length > 1)
            {
                Button button = sender as Button;
                byte index = (byte)button.Tag;
                if (LevelContainColor(index))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("For color to delete some Tiles use it.\n" +
                        "When you delete color all connected tiles will be deleted too.\n" +
                        "Are you sure you want delete this color?",
                        "Confirmation for delete color", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (messageBoxResult != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }
                level.ColorsDataTiles = RemoveColor(index);
                DeselectTilesWithColor(index);
                InitialEditMode();
                DrawAllLevelCells();
                DrawColorPickerOptionMode();
                UpdateEditMode();
                MessageBox.Show("Edit Mode was rest after delete color.", "Notification about edit mode", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("You cannot delete all possible colors. Minimum on color for selection must exist.", "Limitation for deleting color",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeselectTilesWithColor(byte index)
        {
            foreach (TileData[] tiles in level.TilesData)
            {
                foreach (var tile in tiles)
                {
                    if (tile.IsSelected && tile.ColorID == index)
                    {
                        tile.IsSelected = false;
                    }
                    if (tile.IsSelected && tile.ColorID > index)
                    {
                        tile.ColorID -= 1;
                    }
                }
            }
        }

        private ColorData[] RemoveColor(byte index)
        {
            ColorData[] colors = new ColorData[level.ColorsDataTiles.Length - 1];
            for (int i = 0; i < colors.Length; i++)
            {
                if (i < index)
                {
                    colors[i] = level.ColorsDataTiles[i];
                }
                else
                {
                    colors[i] = level.ColorsDataTiles[i + 1];
                }
            }
            return colors;
        }

        private void button_AddNewColor_Click(object sender, RoutedEventArgs e)
        {
            Byte[] colorByte = new Byte[3];
            Random rnd = new Random();
            rnd.NextBytes(colorByte);
            level.ColorsDataTiles = AddNewColor(new ColorData(colorByte[0], colorByte[1], colorByte[2]));
            DrawColorPickerOptionMode();
            UpdateEditMode();
        }

        private ColorData[] AddNewColor(ColorData colorData)
        {
            ColorData[] colors = new ColorData[level.ColorsDataTiles.Length + 1];
            for (int i = 0; i < level.ColorsDataTiles.Length; i++)
            {
                ColorData color = level.ColorsDataTiles[i];
                colors[i] = color;
            }
            colors[colors.Length - 1] = colorData;
            return colors;
        }

        private void RectangleTemp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = (Point)rectangleTemp.Tag;

            TileData tileData = level.TilesData[(int)pTempTag.X][(int)pTempTag.Y];
            if (leftButtonEditMode == -1)
            {
                level.TilesData[(int)pTempTag.X][(int)pTempTag.Y].IsSelected = false;
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
            }
            else
            {
                level.TilesData[(int)pTempTag.X][(int)pTempTag.Y].IsSelected = true;
                level.TilesData[(int)pTempTag.X][(int)pTempTag.Y].ColorID = (byte)leftButtonEditMode;
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorDataTiles((byte)leftButtonEditMode));
            }
            sender = rectangleTemp;
        }
        private void RectangleTemp_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangleTemp = (Rectangle)sender;
            Point pTempTag = GetPointFromTag(sender);
            if (rightButtonEditMode == -1)
            {
                level.TilesData[(int)pTempTag.X][(int)pTempTag.Y].IsSelected = false;
                rectangleTemp.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataNeutral));
            }
            else
            {
                level.TilesData[(int)pTempTag.X][(int)pTempTag.Y].IsSelected = true;
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
                if (level.TilesData.Length > 0)
                {
                    e.CanExecute = true;
                }
            }                    
        }
        private void CpTilesNaturalColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (level == null) return;
            if (cpTilesNaturalColor.SelectedColor.HasValue)
            {
                level.ColorDataNeutral = GetColorDataFromColor(cpTilesNaturalColor.SelectedColor.Value);
                DrawAllLevelCells();
            }
        }
        private void CpBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (level == null) return;
            if (cpBackgroundColor.SelectedColor.HasValue)
            {
                level.ColorDataBackground = GetColorDataFromColor(cpBackgroundColor.SelectedColor.Value);
                rectangle_Background.Fill = new SolidColorBrush(GetColorFromColorData(level.ColorDataBackground));
            }
        }
        private void CpMarkerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (level == null) return;
            if (cpMarkerColor.SelectedColor.HasValue)
            {
                level.ColorDataMarker = GetColorDataFromColor(cpMarkerColor.SelectedColor.Value);
            }
        }
        private void ClearAllTilesData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want delete all Tiles Data?",
                "Confirmation before delete all tiles data", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (TileData[] tiles in level.TilesData)
                {
                    foreach(TileData tile in tiles)
                    {
                        tile.IsSelected = false;
                    }
                }
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
        public static readonly RoutedUICommand UpdateLevelDimension = new RoutedUICommand
            ("Update Level Dimension", "UpdateLevelDimension", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.U, ModifierKeys.Alt)
            }
            );
    }
}
