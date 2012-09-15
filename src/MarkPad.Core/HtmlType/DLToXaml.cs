using HtmlAgilityPack;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class DLToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "dl"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            InlineUIContainer ilContainer = new InlineUIContainer();
            StackPanel spUL = new StackPanel();
            ilContainer.Child = spUL;

            foreach (HtmlNode itm in htmlNode.ChildNodes)
            {
                TextBlock tbItem = new TextBlock { Text = itm.InnerText, FontStyle = FontStyle.Italic };
                switch (itm.Name.ToLower())
                {
                    case "dt":
                        tbItem.Margin = new Thickness(15, 0, 0, 0);
                        break;

                    case "dd":
                        tbItem.Margin = new Thickness(25, 0, 0, 0);
                        break;
                }
                spUL.Children.Add(tbItem);
            }
            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
