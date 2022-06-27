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
using System.Windows.Shapes;

namespace LogicPictureLE
{
    /// <summary>
    /// Logika interakcji dla klasy NewLevelWizard.xaml
    /// </summary>
    public partial class NewLevelWizard : Window
    {
        public NewLevelWizard()
        {
            InitializeComponent();
        }
        public SingleLevel singleLevel;
        private void button_EmptySingle_Click(object sender, RoutedEventArgs e)
        {
            singleLevel = new SingleLevel();
            singleLevel.Name = "Empty Single Level";
            singleLevel.Width = 5;
            singleLevel.Height = 5;
            this.Close();
        }
        private void button_EmptyBig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Empty Big TODO");
        }
        private void button_EmptyGif_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Empty Gif TODO");
        }
        private void button_ImportSingle_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Import Single TODO");
        }
        private void button_ImportBig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Import Big TODO");
        }
    }
}
