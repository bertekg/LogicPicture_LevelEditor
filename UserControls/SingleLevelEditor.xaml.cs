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
    /// <summary>
    /// Logika interakcji dla klasy SingleLevelEditor.xaml
    /// </summary>
    public partial class SingleLevelEditor : UserControl
    {
        SingleLevel singleLevel;
        LevelEditor levelEditor;
        public SingleLevelEditor(SingleLevel levelData)
        {
            InitializeComponent();
            singleLevel = levelData;
            LoadLevelData();
        }
        private void LoadLevelData()
        {
            textBox_LevelNameEnglish.Text = singleLevel.NameEnglish;
            textBox_LevelNamePolish.Text = singleLevel.NamePolish;

            levelEditor = new LevelEditor(singleLevel.LevelData);
            tabItem_LevelData.Content = levelEditor;
        }
        public SingleLevel GetSingleLevelData()
        {
            UpadateLevelNames();
            singleLevel.LevelData = levelEditor.GetLevelData();
            return singleLevel;
        }
        public void UpadateLevelNames()
        {
            singleLevel.NameEnglish = textBox_LevelNameEnglish.Text;
            singleLevel.NamePolish = textBox_LevelNamePolish.Text;
        }
    }
}
