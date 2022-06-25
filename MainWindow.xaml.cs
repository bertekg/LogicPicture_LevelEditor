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
            //SetCultureInfo("en-EN");
            InitializeComponent();
        }
        private static void SetCultureInfo(string cultureInfoToSet)
        {
            CultureInfo ci = new CultureInfo(cultureInfoToSet);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
        private void commandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(textBox_LevelName.Text != string.Empty &
                xctkByteUpDown_LevelWidth.Value.HasValue &
                xctkByteUpDown_LevelHeight.Value.HasValue)
            {
                e.CanExecute = true;
            }
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
