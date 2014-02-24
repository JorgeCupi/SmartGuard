using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace CustomLiveTiles
{
    public partial class LiveTileType2 : UserControl
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
                txtTitleBack.Text = Title;
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

        private ImageSource source;
        public ImageSource Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                imgBackground.Source = Source;
            }
        }

        private Color background;
        new public Color Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;
                SolidColorBrush brush = new SolidColorBrush(Background);
                gridFont.Background = brush;
                gridBack.Background = brush;
            }
        }

        public LiveTileType2()
        {
            InitializeComponent();
            Storyboard anim = (Storyboard)FindName("liveTileAnim1");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4, 15)));
            anim.Begin();
        }

        private void liveTileAnim1_Completed_1(object sender, EventArgs e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1_Inverse");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4,15)));
            anim.Begin();
        }

        private void liveTileAnim1_Inverse_Completed_1(object sender, EventArgs e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnim1");
            anim.Duration = new Duration(new TimeSpan(0, 0, 0, new Random().Next(4,15)));
            anim.Begin();
        }
    }
}
