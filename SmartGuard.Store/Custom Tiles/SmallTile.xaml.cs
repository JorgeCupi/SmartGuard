using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CustomLiveTiles
{
    public partial class SmallTile : UserControl
    {
        private ImageSource backgroundIcon;
        public ImageSource BackgroundIcon
        {
            get
            {
                return backgroundIcon;
            }
            set
            {
                backgroundIcon = value;
                imgBackground.Source = BackgroundIcon;
            }
        }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; grdContainer.Background = new SolidColorBrush(BackgroundColor); }
        }

        public SmallTile()
        {
            InitializeComponent();
        }
    }
}