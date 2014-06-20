using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MarkPad.Views
{
    /// <summary>
    /// UI Hint for App bar.
    /// Source: http://code.msdn.microsoft.com/windowsapps/AppBar-Hint-control-17e23dbf/view/Reviews 
    /// </summary>
    public class CollapsedAppBar : Control
    {
        public CollapsedAppBar()
        {
            this.DefaultStyleKey = typeof(CollapsedAppBar);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PointerEntered += CollapsedAppBar_PointerEntered;
            this.PointerExited += CollapsedAppBar_PointerExited;
            this.Tapped += CollapsedAppBar_Tapped;
        }

        private void CollapsedAppBar_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var page = this.GetFirstAncestorOfType<Page>();
            ShowAppBar(page.TopAppBar);
            ShowAppBar(page.BottomAppBar);
        }

        private void ShowAppBar(AppBar appBar)
        {
            if (appBar != null)
            {
                appBar.IsOpen = true;
            }
        }

        private void CollapsedAppBar_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
        }

        private void CollapsedAppBar_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }



    }

    internal static class VisualTreeHelperExtensions
    {
        public static T GetFirstAncestorOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestorsOfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets the the ancestors of a given type.
        /// </summary>
        /// <typeparam name="T">Type of ancestor to look for.</typeparam>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAncestorsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestors().OfType<T>();
        }

        /// <summary>
        /// Gets the ancestors.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject start)
        {
            var parent = Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(start);

            while (parent != null)
            {
                yield return parent;
                parent = Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
            }
        }
    }
}
