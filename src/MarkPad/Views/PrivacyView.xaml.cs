using System;
using Windows.System;

namespace MarkPad.Views
{
    public sealed partial class PrivacyView
    {
        public PrivacyView()
        {
            InitializeComponent();
        }

        private void PrivacyClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("http://code52.org/MarkPadRT/privacy.html", UriKind.Absolute));
        }
    }
}
