using System.Windows.Controls;

namespace LogicPictureLE.UserControls
{
    public partial class SingleLevelEditor : UserControl
    {
        SingleLevel singleLevel;
        LevelEditor levelEditor;
        bool afterInitial = false;
        public SingleLevelEditor(SingleLevel levelData)
        {
            InitializeComponent();
            singleLevel = levelData;
            LoadLevelData();
            afterInitial = true;
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

        private void textBox_ProjectTitleEnglish_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(afterInitial) 
                singleLevel.ProjectStoryEN.Title = textBox_ProjectTitleEnglish.Text;
        }

        private void textBox_ProjectTitlePolish_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (afterInitial)
                singleLevel.ProjectStoryPL.Title = textBox_ProjectTitlePolish.Text;
        }

        private void textBox_ProjectDescriptionEnglish_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (afterInitial)
                singleLevel.ProjectStoryEN.Description = textBox_ProjectDescriptionEnglish.Text;
        }

        private void textBox_ProjectDescriptionPolish_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (afterInitial)
                singleLevel.ProjectStoryPL.Description = textBox_ProjectDescriptionPolish.Text;
        }
    }
}
