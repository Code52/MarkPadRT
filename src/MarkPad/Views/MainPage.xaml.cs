using System;
using MarkPad.ViewModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MarkPad.Views
{
    public sealed partial class MainPage
    {
        private MainViewModel ViewModel { get { return (MainViewModel)DataContext; } }
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private const string Css = @"body { background : #eaeaea; font-family: Segoe UI, sans-serif; }";
        private readonly MarkdownDeep.Markdown _markdown = new MarkdownDeep.Markdown();
        private readonly WebViewBrush _webBrush;

        public MainPage()
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _timer.Tick += TimerTick;
            wv.LoadCompleted += wv_LoadCompleted;
            BottomAppBar.Opened += (s, e) => SwitchWebViewForWebViewBrush();
            BottomAppBar.Closed += (s, e) => SwitchWebViewBrushForWebView();

            _webBrush = new WebViewBrush();
            _webBrush.SetSource(wv);
            webRectangle.Fill = _webBrush;
        }

        private void SwitchWebViewBrushForWebView()
        {
            wv.Visibility = Visibility.Visible;
            webRectangle.Visibility = Visibility.Collapsed;
        }

        private void SwitchWebViewForWebViewBrush()
        {
            if (_webBrush != null)
                _webBrush.Redraw();
            webRectangle.Visibility = Visibility.Visible;
            wv.Visibility = Visibility.Collapsed;
        }

        void wv_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Editor.Focus(FocusState.Keyboard);
           
        }

        void TimerTick(object sender, object e)
        {
            _timer.Stop();

            string text = "";
            if (ViewModel.SelectedDocument != null)
                text = ViewModel.SelectedDocument.Text;
            wv.NavigateToString(string.Format("<html><head><style>{0}</style></head><body>{1}</body></html>", Css, _markdown.Transform(text)));
        }

        private void TextChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedDocument != null)
            {
                string foo;
                Editor.Document.GetText(TextGetOptions.None, out foo);
                ViewModel.SelectedDocument.Text = foo;
            }
            _timer.Stop();
            _timer.Start();
        }

        private void DocumentsSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            Editor.Document.SetText(TextSetOptions.None, ViewModel.SelectedDocument.Text);
        }

        private void BoldClicked(object sender, RoutedEventArgs e)
        {
            TransformText(s => "*" + s + "*");
        }

        private void ItalicClicked(object sender, RoutedEventArgs e)
        {
            TransformText(s => "**" + s + "**");
        }

        private void HeaderClicked(object sender, RoutedEventArgs e)
        {
            TransformText(s => "#" + s);
        }

        private void TransformText(Func<string, string> transform)
        {
            string selection;
            Editor.Document.Selection.GetText(TextGetOptions.None, out selection);
            selection = transform(selection);
            Editor.Document.Selection.SetText(TextSetOptions.None, selection);
        }
    }
}
