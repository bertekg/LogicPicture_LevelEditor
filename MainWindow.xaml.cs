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
using System.Windows.Controls;

namespace LogicPictureLE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SetCultureInfo("en-EN");
            InitializeComponent();
            InitialNewSingleLevel();
            OpenFileCore("S_Red Heart_05_05.xml");
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
                "Question before creation new file", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                OpenFileCore(openFileDialog.FileName);
            }
        }

        private void OpenFileCore(string openFilePath)
        {
            try
            {
                //string contenJSON = File.ReadAllText(openFileDialog.FileName);
                //singleLevel = JsonConvert.DeserializeObject<SingleLevel>(contenJSON);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(openFilePath);
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
                MessageBox.Show("Message of error:\n" + exeption.Message, "Error during open file", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void commandBinding_AboutProgram_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void commandBinding_AboutProgram_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AboutProgram apWindow = new AboutProgram();
            apWindow.ShowDialog();
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
        private void CalcHintsData()
        {
            singleLevel.LevelData.HintsDataVertical = new HintData[singleLevel.LevelData.HeightY][];
            for (byte y = 0; y < singleLevel.LevelData.HeightY; y++)
            {
                int previousColorId = -1;
                byte currentIdCombo = 0;
                List<HintData> lVerticalNumberHints = new List<HintData>();
                for (byte x = 0; x < singleLevel.LevelData.WidthX; x++)
                {                  
                    TileData tileDataFound = singleLevel.LevelData.TilesData[x][y];
                    if (tileDataFound.IsSelected)
                    {
                        if (tileDataFound.ColorID  == previousColorId)
                        {
                            currentIdCombo++;
                        }
                        else if (previousColorId >= 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)previousColorId;
                            hdTemp.Value = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        previousColorId = tileDataFound.ColorID;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)previousColorId;
                            hdTemp.Value = currentIdCombo;
                            lVerticalNumberHints.Add(hdTemp);
                            currentIdCombo = 0;
                            previousColorId = -1;
                        }
                    }
                }
                if (currentIdCombo > 0)
                {
                    HintData hdTemp = new HintData();
                    hdTemp.ColorID = (byte)(previousColorId);
                    hdTemp.Value = currentIdCombo;
                    lVerticalNumberHints.Add(hdTemp);
                }
                singleLevel.LevelData.HintsDataVertical[y] = ConvertListToArray(lVerticalNumberHints);
            }

            singleLevel.LevelData.HintsDataHorizontal = new HintData[singleLevel.LevelData.WidthX][];
            for (byte x = 0; x < singleLevel.LevelData.WidthX; x++)
            {
                int prevCellId = -1;
                byte currentIdCombo = 0;
                List<HintData> hints = new List<HintData>();
                for (byte y = singleLevel.LevelData.HeightY; y > 0; y--)
                {
                    TileData tileDataFound = singleLevel.LevelData.TilesData[x][y -1];
                    if (tileDataFound.IsSelected)
                    {
                        if (tileDataFound.ColorID == prevCellId)
                        {
                            currentIdCombo++;
                        }
                        else if (prevCellId >= 0)
                        {
                            HintData hint = new HintData();
                            hint.ColorID = (byte)(prevCellId);
                            hint.Value = currentIdCombo;
                            hints.Add(hint);
                            currentIdCombo = 1;
                        }
                        else
                        {
                            currentIdCombo = 1;
                        }
                        prevCellId = tileDataFound.ColorID;
                    }
                    else
                    {
                        if (currentIdCombo > 0)
                        {
                            HintData hdTemp = new HintData();
                            hdTemp.ColorID = (byte)(prevCellId);
                            hdTemp.Value = currentIdCombo;
                            hints.Add(hdTemp);
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
                    hints.Add(hdTemp);
                    currentIdCombo = 0;
                    prevCellId = 0;
                }
                singleLevel.LevelData.HintsDataHorizontal[x] = ConvertListToArray(hints);
            }
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

        private void commandBinding_Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void commandBinding_PreviewProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_PreviewProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcHintsData();
            PreviewProject previewProject = new PreviewProject(singleLevel);
            previewProject.ShowDialog();
        }

        private void commandBinding_CheckUniqueness_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_CheckUniqueness_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CalcHintsData();
            CheckUniqueness checkUniqueness = new CheckUniqueness(singleLevel);
            checkUniqueness.ShowDialog();
        }
    }
    public static class CustomCommands
    {
        public static readonly RoutedUICommand AboutProgram = new RoutedUICommand
            ("About Program", "About Program", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F1, ModifierKeys.Alt)
                } );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
            ("Exit", "Exit", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Q, ModifierKeys.Alt)
                } );

        public static readonly RoutedUICommand PreviewProject = new RoutedUICommand
            ("Preview Project", "Preview Project", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P, ModifierKeys.Alt)
                } );

        public static readonly RoutedCommand CheckUniqueness = new RoutedUICommand
            ("Check Uniqueness", "Check Uniqueness", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.U, ModifierKeys.Alt)
            } );
    }
}
