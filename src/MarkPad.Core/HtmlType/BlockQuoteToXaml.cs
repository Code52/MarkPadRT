using HtmlAgilityPack;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class BlockQuoteToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "blockquote"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var ilContainer = new InlineUIContainer();
            var tbItem = new TextBlock
                {
                    Margin = new Thickness(25, 0, 0, 0), 
                    TextWrapping = TextWrapping.Wrap, 
                    FontStyle = FontStyle.Italic, 
                    Text = htmlNode.InnerText
                };
            ilContainer.Child = tbItem;
            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
