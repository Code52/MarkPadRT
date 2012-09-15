using System;
using HtmlAgilityPack;
using Windows.UI;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace MarkPad.Core.HtmlType
{
    public class AToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "a"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var url = htmlNode.Attributes["href"];
            if (url == null)
                throw new Exception("請輸入href");
             
            var r = new Run { Text = GetCleanContent(htmlNode.InnerText), Foreground = new SolidColorBrush(Colors.Red)};
            (block as Paragraph).Inlines.Add(r);
        }
    }
}
