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
using Newtonsoft.Json;
using System.IO;
using LogicPictureLE.UserControls;

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
        SingleLevel singleLevel;
        SingleLevelEditor editor;
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
                editor = new SingleLevelEditor(singleLevel);
                grid_MainContent.Children.Add(editor);
            }
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
            string defaultFileName = editor.textBox_LevelName.Text + "_" +
                editor.xctkByteUpDown_LevelWidth.Value.Value.ToString("D2") + "_" +
                editor.xctkByteUpDown_LevelHeight.Value.Value.ToString("D2");
            saveFileDialog.FileName = defaultFileName;
            bool? saveResult = saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != String.Empty && saveResult.Value == true)
            {
                string jsonContent = JsonConvert.SerializeObject(editor.GetSingleLevel());

                File.WriteAllText(saveFileDialog.FileName, jsonContent);
                MessageBox.Show("Save file finished correct. File path:\n" + saveFileDialog.FileName,
                    "Information after save project", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
