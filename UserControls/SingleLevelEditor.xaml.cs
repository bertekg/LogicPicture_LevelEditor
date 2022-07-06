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
            textBox_ProjectTitleEnglish.Text = singleLevel.ProjectStoryEN.Title;
            textBox_ProjectDescriptionEnglish.Text = singleLevel.ProjectStoryEN.Description;
            textBox_ProjectTitlePolish.Text = singleLevel.ProjectStoryPL.Title;
            textBox_ProjectDescriptionPolish.Text = singleLevel.ProjectStoryPL.Description;

            levelEditor = new LevelEditor(singleLevel.LevelData);
            tabItem_LevelData.Content = levelEditor;
        }
        public SingleLevel GetSingleLevelData()
        {
            UpadateProjectStories();
            singleLevel.LevelData = levelEditor.GetLevelData();
            return singleLevel;
        }
        public void UpadateProjectStories()
        {
            singleLevel.ProjectStoryEN.Title = textBox_ProjectTitleEnglish.Text;
            singleLevel.ProjectStoryEN.Description = textBox_ProjectDescriptionEnglish.Text;
            singleLevel.ProjectStoryPL.Title = textBox_ProjectTitlePolish.Text;
            singleLevel.ProjectStoryPL.Description = textBox_ProjectDescriptionPolish.Text;
        }
    }
}
