using Microsoft.Win32;
using System;
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
            SingleLevel singleLevel = singleLevelEditor.GetSingleLevelData();
            if (singleLevel == null) return;
            singleLevel.CalcHintsData();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML|*.xml";
            saveFileDialog.Title = "Save Level File";
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
            singleLevel.CalcHintsData();
            PreviewProject previewProject = new PreviewProject(singleLevel);
            previewProject.ShowDialog();
        }

        private void commandBinding_CheckUniqueness_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void commandBinding_CheckUniqueness_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            singleLevel.CalcHintsData();
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
                    new KeyGesture(Key.F1)
                } );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
            ("Exit", "Exit", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Q, ModifierKeys.Control)
                } );

        public static readonly RoutedUICommand PreviewProject = new RoutedUICommand
            ("Preview Project", "Preview Project", typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P, ModifierKeys.Control)
                } );

        public static readonly RoutedCommand CheckUniqueness = new RoutedUICommand
            ("Check Uniqueness", "Check Uniqueness", typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.U, ModifierKeys.Control)
            } );
    }
}
