using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace MarkPad
{
    public sealed partial class MainPage
    {
        private DispatcherTimer t = new DispatcherTimer();
        MarkdownDeep.Markdown x = new MarkdownDeep.Markdown();
        public MainPage()
        {
            InitializeComponent();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
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
            wv.NavigateToString(string.Format("<html><head><style>{0}</style></head><body>{1}</body></html>", css, x.Transform(foo)));
        }

        private void tx_TextChanged_2(object sender, RoutedEventArgs e)
        {
            t.Stop();
            t.Start();
        }

        private string css = @"body { background : #eaeaea; font-family: Segoe UI, sans-serif; }";
    }
}
