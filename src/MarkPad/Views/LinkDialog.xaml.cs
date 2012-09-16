using System;

namespace MarkPad.Views
{
    public sealed partial class LinkDialog : IDialog
    {
        public LinkDialog()
        {
            InitializeComponent();
        }

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
    }
}
