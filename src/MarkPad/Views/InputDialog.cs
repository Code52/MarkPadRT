using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MarkPad.Views
{
    public static class InputDialog
    {
        public static async Task ShowAsync(this IDialog view)
        {
            var tcs = new TaskCompletionSource<int>();

            var p = new Popup
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = Window.Current.Bounds.Width,
                    Height = Window.Current.Bounds.Height
                };

            ((UserControl)view).Width = Window.Current.Bounds.Width;
            ((UserControl)view).Height = Window.Current.Bounds.Height;
            view.Cancelled += () =>
                {
                    tcs.TrySetResult(1);
                    p.IsOpen = false;
                };

            p.Child = (UserControl)view;
            p.IsOpen = true;
            await tcs.Task;
            return;
        }
    }
}