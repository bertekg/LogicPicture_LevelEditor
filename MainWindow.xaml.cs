using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
            string defaultFileName = string.Concat("S_", singleLevel.ProjectStoryEN.Title, "_", 
                singleLevel.LevelData.WidthX.ToString("D2"), "_", singleLevel.LevelData.HeightY.ToString("D2"));
            saveFileDialog.FileName = defaultFileName;
            bool? saveResult = saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != String.Empty && saveResult.Value == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SingleLevel));
                TextWriter textWriter = new StreamWriter(saveFileDialog.FileName);
                xmlSerializer.Serialize(textWriter, singleLevel);
                textWriter.Close();

                string rootDirectoryPath = Path.GetDirectoryName(saveFileDialog.FileName);
                string directoryName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                string finalDirectoryPath = Path.Combine(rootDirectoryPath, directoryName);
                if (Directory.Exists(finalDirectoryPath))
                {
                    Directory.Delete(finalDirectoryPath, true);
                }
                DirectoryInfo directoryInfo = Directory.CreateDirectory(finalDirectoryPath);

                xmlSerializer = new XmlSerializer(typeof(ProjectCommon));
                ProjectCommon projectCommon = new ProjectCommon(singleLevel.ProjectStoryEN, singleLevel.ProjectStoryPL);
                string projectCommonPath = Path.Combine(directoryInfo.FullName , string.Concat(defaultFileName, "_ProjectCommon.xml"));
                textWriter = new StreamWriter(projectCommonPath);
                xmlSerializer.Serialize(textWriter, projectCommon);
                textWriter.Close();

                xmlSerializer = new XmlSerializer(typeof(Level));
                string levelPath = Path.Combine(directoryInfo.FullName, string.Concat(defaultFileName, "_Level.xml"));
                textWriter = new StreamWriter(levelPath);
                xmlSerializer.Serialize(textWriter, singleLevel.LevelData);
                textWriter.Close();

                string levelPicturePath = Path.Combine(directoryInfo.FullName, string.Concat(defaultFileName, "_Picture.png"));
                PreviewProject previewProject = new PreviewProject(singleLevel);
                WriteableBitmap writeableBitmap = previewProject.GetLevelPicture();
                CreateThumbnail(levelPicturePath, writeableBitmap);
                previewProject.Close();

                string messageBoxText = "Save project finished correct.\nFile/directory name: " + defaultFileName +
                    "\nRoot path:\n" + rootDirectoryPath;
                MessageBox.Show(messageBoxText, "Information after save project", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {
                using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                }
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
