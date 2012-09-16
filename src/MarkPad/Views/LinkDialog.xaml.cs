using System;
using System.Text.RegularExpressions;
using MarkPad.ViewModel;
using Windows.UI.Xaml;

namespace MarkPad.Views
{
    public sealed partial class LinkDialog : IDialog
    {
        public LinkDialog(string selectedText)
        {
            InitializeComponent();

            //  Check if the selected text already is a link...
            string text = selectedText, url = string.Empty;
            var match = Regex.Match(selectedText, @"\[(?<text>(?:[^\\]|\\.)+)\]\((?<url>[^)]+)\)");
            if (match.Success)
            {
                text = match.Groups["text"].Value;
                url = match.Groups["url"].Value;
            }

            DataContext = new LinkViewModel {DisplayText = text, LinkAddress = url};
        }

        public Action Added { get; set; }
        public Action Cancelled { get; set; }

        private void BackgroundGridClicked(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (Cancelled != null)
                Cancelled();
        }

        private void ForegroundGridClicked(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (Added != null)
                Added();

            if (Cancelled != null) Cancelled();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            if (Cancelled != null)
                Cancelled();
        }
    }
}
