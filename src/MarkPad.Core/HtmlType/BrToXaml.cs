using HtmlAgilityPack;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class BrToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "br"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            Br(block);
        }
    }
}
