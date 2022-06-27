using Microsoft.Win32;
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
using Newtonsoft.Json;
using System.IO;

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
        }
        SingleLevel level;
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
            if (level != null)
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
                level = newLevelWizard.singleLevel;
                UpdateNewLevelData();
            }
        }
        private void UpdateNewLevelData()
        {
            textBox_LevelName.Text = level.Name;
            xctkByteUpDown_LevelWidth.Value = level.Width;
            xctkByteUpDown_LevelHeight.Value = level.Height;
        }

        private void commandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void commandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON|*.json";
            saveFileDialog.Title = "Save Level File";
            string defaultFileName = textBox_LevelName.Text + "_" +
                xctkByteUpDown_LevelWidth.Value.Value.ToString("D2") + "_" +
                xctkByteUpDown_LevelHeight.Value.Value.ToString("D2");
            saveFileDialog.FileName = defaultFileName;
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != String.Empty)
            {
                SingleLevel singleLevel = new SingleLevel
                {
                    Name = textBox_LevelName.Text,
                    Width = xctkByteUpDown_LevelWidth.Value.Value,
                    Height = xctkByteUpDown_LevelHeight.Value.Value
                };

                string jsonContent = JsonConvert.SerializeObject(singleLevel);

                File.WriteAllText(saveFileDialog.FileName, jsonContent);
            }
        }       
    }
}
