using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class ULToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "ul"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var ilContainer = new InlineUIContainer();
            var spUL = new StackPanel();
            ilContainer.Child = spUL;
            spUL.Margin = new Thickness(25, 0, 0, 0);
            foreach (HtmlNode itm in htmlNode.ChildNodes)
            {
                if (!itm.Name.Equals("li"))
                    continue;

                var tbItem = new TextBlock { Text = "« " + itm.InnerText };
                spUL.Children.Add(tbItem);
            }

            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
