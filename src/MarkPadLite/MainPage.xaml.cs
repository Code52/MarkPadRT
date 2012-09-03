using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MarkPadLite
{
    public sealed partial class MainPage
    {
        private DispatcherTimer t = new DispatcherTimer();
        MarkdownDeep.Markdown x = new MarkdownDeep.Markdown();
        public MainPage()
        {
            InitializeComponent();
            t.Interval = new TimeSpan(0,0,0,0,500);
            t.Tick += t_Tick;
            wv.LoadCompleted += wv_LoadCompleted;
        }

        void wv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            tx.Focus(FocusState.Keyboard);
        }

        void t_Tick(object sender, object e)
        {
            t.Stop();
            string foo;
            tx.Document.GetText(TextGetOptions.None, out foo);
            wv.NavigateToString(x.Transform(foo));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void tx_TextChanged_2(object sender, RoutedEventArgs e)
        {
            t.Stop();
            t.Start();
        }
    }
}
