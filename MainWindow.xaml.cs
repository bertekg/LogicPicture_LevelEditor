﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;
using LogicPictureLE.UserControls;
using System.Xml.Serialization;
using System.Xml;

namespace LogicPictureLE
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SetCultureInfo("en-EN");
            InitializeComponent();
            InitialNewSingleLevel();
        }
        private void InitialNewSingleLevel()
        {
            NewLevelWizard newLevelWizard = new NewLevelWizard();
            newLevelWizard.NewSingleLevelMake();
            singleLevel = newLevelWizard.singleLevel;
            newLevelWizard.Close();
            grid_MainContent.Children.Clear();
            singleLevelEditor = new SingleLevelEditor(singleLevel);
            singleLevelEditor.tabItem_LevelData.IsSelected = true;
            grid_MainContent.Children.Add(singleLevelEditor);
        }
        SingleLevel singleLevel;
        SingleLevelEditor singleLevelEditor;
        private static void SetCultureInfo(string cultureInfoToSet)
        {
            CultureInfo ci = new CultureInfo(cultureInfoToSet);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
        private void commandBinding_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void commandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (singleLevel != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want create new project and clear all current project data?",
                "Question befor creation new file", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CreateNewLevel();
                }
            }
            else
            {                
                CreateNewLevel();
            }
        }
        private void CreateNewLevel()
        {
            NewLevelWizard newLevelWizard = new NewLevelWizard();
            newLevelWizard.ShowDialog();
            if (newLevelWizard.singleLevel != null)
            {
                singleLevel = newLevelWizard.singleLevel;
                grid_MainContent.Children.Clear();
                singleLevelEditor = new SingleLevelEditor(singleLevel);
                grid_MainContent.Children.Add(singleLevelEditor);
            }
        }
        private void commandBinding_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void commandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML|*.xml";
            openFileDialog.Title = "Open Level File";
            bool? openResult = openFileDialog.ShowDialog();
            if (openFileDialog.FileName != String.Empty && openResult.Value == true)
            {
                try
                {
                    //string contenJSON = File.ReadAllText(openFileDialog.FileName);
                    //singleLevel = JsonConvert.DeserializeObject<SingleLevel>(contenJSON);
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(openFileDialog.FileName);
                    string xmlString = xmlDocument.OuterXml;
                    using (StringReader read = new StringReader(xmlString))
                    {
                        Type outType = typeof(SingleLevel);

                        XmlSerializer serializer = new XmlSerializer(outType);
                        using (XmlReader reader = new XmlTextReader(read))
                        {
                            singleLevel = (SingleLevel)serializer.Deserialize(reader);
                            reader.Close();
                        }
                        read.Close();
                    }
                    grid_MainContent.Children.Clear();
                    singleLevelEditor = new SingleLevelEditor(singleLevel);
                    grid_MainContent.Children.Add(singleLevelEditor);
                }
                catch (ArgumentException exeption)
                {
                    MessageBox.Show("Messege of error:\n" + exeption.Message, "Error during open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void commandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void commandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcHintsData();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML|*.xml";
            saveFileDialog.Title = "Save Level File";
            SingleLevel singleLevel = singleLevelEditor.GetSingleLevelData();
            string defaultFileName = singleLevel.ProjectStoryEN.Title + "_" +
                singleLevel.LevelData.WidthX.ToString("D2") + "_" +
                singleLevel.LevelData.HeightY.ToString("D2");
            saveFileDialog.FileName = defaultFileName;
            bool? saveResult = saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != String.Empty && saveResult.Value == true)
            {
                XmlSerializer ser = new XmlSerializer(typeof(SingleLevel));
                TextWriter writer = new StreamWriter(saveFileDialog.FileName);
                ser.Serialize(writer, singleLevel);
                writer.Close();
                //string jsonContent = JsonConvert.SerializeObject(singleLevel);

                //File.WriteAllText(saveFileDialog.FileName, jsonContent);
                MessageBox.Show("Save file finished correct. File path:\n" + saveFileDialog.FileName,
                    "Information after save project", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void button_PreviewProject_Click(object sender, RoutedEventArgs e)
        {
            CalcHintsData();
            PreviewProject previewProject = new PreviewProject(singleLevel);
            previewProject.ShowDialog();
        }
        private void CalcHintsData()
        {
            singleLevel.LevelData.HintsDataVertical.Clear();
            for (byte y = 0; y < singleLevel.LevelData.HeightY; y++)
            {
                byte prevCellId = 0;
                byte currCellId = 0;
                byte currentIdCombo = 0;
                List<HintData> lVerticalNumberHints = new List<HintData>();
                for (byte x = 0; x < singleLevel.LevelData.WidthX; x++)
                {
                    Point currentPoint = new Point(x, y);                    
                    TileData tileDataFound = singleLevel.LevelData.TilesData.Find(item => (item.PosX == currentPoint.X && item.PosY == currentPoint.Y));
                    if (tileDataFound != null)
                    {
                        currCellId = (byte)(tileDataFound.ColorID + 1);
                        if (currCellId == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId - 1);
                            hdTemp.Value = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = currCellId;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId - 1);
                            hdTemp.Value = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 0;
                            prevCellId = 0;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.ColorID = (byte)(prevCellId - 1);
                    hdTemp.Value = currentIdCombo;
                    lVerticalNumberHints.Add(hdTemp);
                    currentIdCombo = 0;
                    prevCellId = 0;
                }
                singleLevel.LevelData.HintsDataVertical.Add(lVerticalNumberHints);
            }

            singleLevel.LevelData.HintsDataHorizontal.Clear();
            for (byte x = 0; x < singleLevel.LevelData.WidthX; x++)
            {
                byte prevCellId = 0;
                byte currentIdCombo = 0;
                List<HintData> lHorizontalNumberHints = new List<HintData>();
                for (byte y = singleLevel.LevelData.HeightY; y > 0; y--)
                {
                    Point currentPoint = new Point(x, y - 1);
                    byte currCellId = 0;
                    TileData tileDataFound = singleLevel.LevelData.TilesData.Find(item => (item.PosX == currentPoint.X && item.PosY == currentPoint.Y));
                    if (tileDataFound != null)
                    {
                        currCellId = (byte)(tileDataFound.ColorID + 1);
                        if (currCellId == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId - 1);
                            hdTemp.Value = currentIdCombo;
                            lHorizontalNumberHints.Add(hdTemp);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = currCellId;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId - 1);
                            hdTemp.Value = currentIdCombo;
                            lHorizontalNumberHints.Add(hdTemp);
                            currentIdCombo = 0;
                            prevCellId = 0;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.ColorID = (byte)(prevCellId - 1);
                    hdTemp.Value = currentIdCombo;
                    lHorizontalNumberHints.Add(hdTemp);
                    currentIdCombo = 0;
                    prevCellId = 0;
                }
                singleLevel.LevelData.HintsDataHorizontal.Add(lHorizontalNumberHints);
            }
        }
    }
}
