using System.Threading.Tasks;
using MarkPad.GitHub;
using Windows.ApplicationModel.Activation;
using Windows.Storage.Pickers.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace MarkPad
{

    public sealed partial class GitHubFileOpenPickerPage
    {
        internal FileOpenPickerUI FileOpenPickerUi = null;
        private GitHubClient client;
        public GitHubFileOpenPickerPage()
        {
            InitializeComponent();
            client = new GitHubClient();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Login();
        }

        private async Task Login()
        {
            await client.Login();
            var results = await client.GetRepos();
            foreach (var r in results)
            items.Items.Add(r);
        }


        public void Activate(FileOpenPickerActivatedEventArgs args)
        {
            FileOpenPickerUi = args.FileOpenPickerUI;
            FileOpenPickerUi.Title = "github";
            Window.Current.Content = this;
            OnNavigatedTo(null);
            Window.Current.Activate();
        }
    }
}
