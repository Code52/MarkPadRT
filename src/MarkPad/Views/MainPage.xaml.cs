using System;
using System.Collections.Generic;
using MarkPad.ViewModel;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
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

        private void DocumentsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.Document.SetText(TextSetOptions.None, ViewModel.SelectedDocument != null ? ViewModel.SelectedDocument.Text : string.Empty);
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

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {

        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }

        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null)
                viewState = ApplicationView.Value;

            return viewState == ApplicationViewState.FullScreenPortrait || viewState == ApplicationViewState.Snapped;
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            base.GoBack(sender, e);
        }

        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            var logicalPageBack = UsingLogicalPageNavigation(viewState);// && this.itemListView.SelectedItem != null;
            var physicalPageBack = Frame != null && Frame.CanGoBack;
            DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            if (viewState == ApplicationViewState.Filled || viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366)
                    return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            var defaultStateName = base.DetermineVisualState(viewState);
            return defaultStateName;
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        private void BtnPreviewClicked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Snapped_Preview", false);
        }

        private void BtnEditorClicked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Snapped", false);
        }

        private void DistractionToggled(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, ViewModel.Distraction ? "DistractionFree" : "FullScreenLandscapeOrWide", false);
            ViewModel.Distraction = !ViewModel.Distraction;
        }
    }

}
