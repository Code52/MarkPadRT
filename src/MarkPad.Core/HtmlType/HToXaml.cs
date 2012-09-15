using System;
using HtmlAgilityPack;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class HToXaml : HtmlTypeBase
    {
        string tag;
        public HToXaml(string tag)
        {
            this.tag = tag;
        }

        public override string TagName
        {
            get { return this.tag; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var s = RichTextboxStyle.GetDefault(htmlNode);

            int baseFontSize = 22;
            int hdrSize = Int32.Parse(htmlNode.Name.Substring(1, 1)) * 2;
            s.FontSize = baseFontSize - hdrSize;

            TextToRun(htmlNode.InnerText, s, block);
            Br(block);
        }
    }
}
