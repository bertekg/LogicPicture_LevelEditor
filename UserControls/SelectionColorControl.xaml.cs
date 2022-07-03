using System.Windows.Controls;

namespace LogicPictureLE.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy SelectionColorControl.xaml
    /// </summary>
    public partial class SelectionColorControl : UserControl
    {
        byte colorID;
        public SelectionColorControl()
        {
            InitializeComponent();
            colorID = 0;
            UpdateID();
        }
        private void UpdateID()
        {
            textBlock_ID.Text = colorID.ToString();
        }
        public SelectionColorControl(byte id)
        {
            InitializeComponent();
            colorID = id;
        }
        public byte GetID()
        {
            return colorID;
        }
    }
}
