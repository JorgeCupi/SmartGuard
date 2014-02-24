using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CustomLiveTiles
{
    public partial class MediumTileImage : UserControl
    {
        private Color backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                SolidColorBrush brush = new SolidColorBrush(BackgroundColor);
                grdContainer.Background = brush;
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                txtTitle.Text = Title;
            }
        }

        private ImageSource backgroundImage;
        public ImageSource BackgroundImage
        {
            get
            {
                return backgroundImage;
            }
            set
            {
                backgroundImage = value;
                imgBackground.Source = BackgroundImage;
            }
        }

        public MediumTileImage()
        {
            InitializeComponent();
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part1");
            liveTileAnim1_Part1.Completed += liveTileAnim1_Part1_Completed;
            liveTileAnim1_Part2.Completed+=liveTileAnim1_Part2_Completed;
            liveTileAnim2_Part1.Completed+=liveTileAnim2_Part1_Completed;
            liveTileAnim2_Part2.Completed+=liveTileAnim2_Part2_Completed;
            anim.Begin();
        }

        private void liveTileAnim1_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part2");
            anim.Begin();
        }

        private void liveTileAnim1_Part2_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim2_Part1");
            anim.Begin();
        }

        private void liveTileAnim2_Part1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim2_Part2");
            anim.Begin();
        }

        private void liveTileAnim2_Part2_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Part1");
            anim.Begin();
        }
    }
}