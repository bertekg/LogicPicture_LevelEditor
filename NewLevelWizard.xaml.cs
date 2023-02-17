using System.Windows;

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
            NewSingleLevelMake();
            this.Close();
        }
        public void NewSingleLevelMake()
        {
            ColorData colorDataNeutral = new ColorData(0xC0, 0xC0, 0xC0); //Silver = #FFC0C0C0
            ColorData colorDataBackground = new ColorData(0xFF, 0xFF, 0xFF); //White = #FFFFFFFF
            ColorData colorDataMarker = new ColorData(0xFF, 0xA5, 0x00); //Orange = #FFFFA500
            ColorData[] colorsDataTiles = new ColorData[1]
                {
                    new ColorData(0x00, 0x00, 0x00) //Black = #FF000000
                };
            int width = 5;
            int height = 5;
            TileData[][] tilesData = new TileData[width][];
            for (int i = 0; i < width; i++)
            {
                TileData[] temp = new TileData[height];
                for (int j = 0; j < height; j++)
                {
                    temp[j] = new TileData();
                }
                tilesData[i] = temp;
            }
            HintData[][] hintsDataVertical = new HintData[0][];
            HintData[][] hintsDataHorizontal = new HintData[0][];
            Level level = new Level(5, 5, colorDataNeutral, colorDataBackground, colorDataMarker,
                colorsDataTiles, tilesData, hintsDataHorizontal, hintsDataVertical);
            ProjectStory projectStoryEN = new ProjectStory("Empty Single Project", "Empty description of project.\nSecond line of description.");
            ProjectStory projectStoryPL = new ProjectStory("Pusty pojedyńczy projekt", "Brak opisu projektu.\nDruga linia opisu.");
            singleLevel = new SingleLevel(projectStoryEN, projectStoryPL, level);
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
