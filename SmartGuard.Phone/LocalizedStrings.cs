using SmartGuard.Phone.Resources;

namespace SmartGuard.Phone
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources appResources = new AppResources();

        public AppResources AppResources{ get { return appResources; } }
    }
}