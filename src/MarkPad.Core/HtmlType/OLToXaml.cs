using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class OLToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "ol"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            InlineUIContainer ilContainer = new InlineUIContainer();
            StackPanel spUL = new StackPanel();
            ilContainer.Child = spUL;
            spUL.Margin = new Thickness(25, 0, 0, 0);
            int i = 0;
            foreach (HtmlNode itm in htmlNode.ChildNodes)
            {
                if (itm.Name.Equals("li"))
                {
                    i++;
                    TextBlock tbItem = new TextBlock();
                    tbItem.Text = i.ToString() + ".  " + itm.InnerText;
                    //tbItem.FontStyle = FontStyles.Italic;
                    spUL.Children.Add(tbItem);
                }
            }
            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
