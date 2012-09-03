using System;
using MarkPad.ViewModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace MarkPad.Views
{
    public sealed partial class MainPage
    {
        private MainViewModel ViewModel { get { return (MainViewModel) DataContext; } }
        private DispatcherTimer t = new DispatcherTimer();
        private const string Css = @"body { background : #eaeaea; font-family: Segoe UI, sans-serif; }";
        private readonly MarkdownDeep.Markdown _markdown = new MarkdownDeep.Markdown();

        public MainPage()
        {
            InitializeComponent();
            t.Interval = new TimeSpan(0, 0, 0, 0, 500);
            t.Tick += t_Tick;
            wv.LoadCompleted += wv_LoadCompleted;
        }

        void wv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Editor.Focus(FocusState.Keyboard);
        }

        void t_Tick(object sender, object e)
        {
            t.Stop();

            string text = "";
            if (ViewModel.SelectedDocument != null)
                text = ViewModel.SelectedDocument.Text;
            wv.NavigateToString(string.Format("<html><head><style>{0}</style></head><body>{1}</body></html>", Css, _markdown.Transform(text)));
        }

        private void tx_TextChanged_2(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedDocument != null)
            {
                string foo;
                Editor.Document.GetText(TextGetOptions.None, out foo);
                ViewModel.SelectedDocument.Text = foo;
            }
            t.Stop();
            t.Start();
        }

        private void DocumentsSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            Editor.Document.SetText(TextSetOptions.None, ViewModel.SelectedDocument.Text);
        }
    }
}
