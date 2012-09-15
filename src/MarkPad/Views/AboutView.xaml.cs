using System;
using Windows.ApplicationModel;

namespace MarkPad.Views
{
    public sealed partial class AboutView
    {
        public string Version { get; set; }
        public AboutView()
        {
            InitializeComponent();
            
            PackageVersion version = Package.Current.Id.Version;
            Version = string.Format("{0}.{1}", version.Major, version.Minor);

            DataContext = this;
        }
    }
}
