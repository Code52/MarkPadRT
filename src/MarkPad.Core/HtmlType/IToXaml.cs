using HtmlAgilityPack;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class IToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "i"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var s = RichTextboxStyle.GetDefault(htmlNode);
            s.FontStyle = FontStyle.Italic;

            TextToRun(htmlNode.InnerText, s, block);
        }
    }
}
