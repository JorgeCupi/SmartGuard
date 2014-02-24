using Bing.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SmartGuard.Store.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GetDirectionsView : SmartGuard.Store.Common.LayoutAwarePage
    {
        public GetDirectionsView()
        {
            this.InitializeComponent();
            myMap.ZoomLevel = 14;
            myMap.Center = new Location(staticObjects.CurrentLatitude, staticObjects.CurrentLongitude);
            GPS.GetDirections(myMap, RouteResults);
        }


        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
