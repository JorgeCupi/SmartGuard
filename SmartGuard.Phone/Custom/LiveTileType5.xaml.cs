using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace CustomLiveTiles
{
    public partial class LiveTileType5 : UserControl
    {
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
            get { return background; }
            set { background = value; grdContainer.Background = new SolidColorBrush(Background); }
        }

        public LiveTileType5()
        {
            InitializeComponent();
        }
    }
}
