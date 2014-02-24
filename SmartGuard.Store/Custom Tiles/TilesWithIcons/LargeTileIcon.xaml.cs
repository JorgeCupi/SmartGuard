using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace CustomLiveTiles
{
    public partial class LargeTileIcon : UserControl
    {
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
        private string backTitle;
        public string BackTitle
        {
            get
            {
                return backTitle;
            }
            set
            {
                backTitle = value;
                txtBackTitle.Text = BackTitle;
            }
        }
        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                txtMessage.Text = Message;
            }
        }
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
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                SolidColorBrush brush = new SolidColorBrush(BackgroundColor);
                gridFont.Background = brush;
                gridBack.Background = brush;
            }
        }

        public LargeTileIcon()
        {
            InitializeComponent();
            Storyboard anim = (Storyboard)FindName("liveTileAnim1");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4, 15)));
            anim.Begin();

            liveTileAnim1_Inverse.Completed+=liveTileAnim1_Inverse_Completed;
            liveTileAnim1.Completed += liveTileAnim1_Completed;
        }

        private void liveTileAnim1_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Inverse");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4, 15)));
            anim.Begin();
        }

        void liveTileAnim1_Inverse_Completed(object sender, object e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4, 15)));
            anim.Begin();
        }
    }
}