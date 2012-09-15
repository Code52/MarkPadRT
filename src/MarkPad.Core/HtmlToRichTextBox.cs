using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MarkPad.Core.HtmlType;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core
{
    public class HtmlToRichTextBox
    {
        HtmlDocument htmlDoc;
        Dictionary<string, IHtmlType> ruleDic = new Dictionary<string, IHtmlType>();

        public HtmlToRichTextBox(string html)
        {
            Init(html);
        }

        private void Init(string html)
        {
            //Initialize HtmlAgilityPack
            htmlDoc = new HtmlDocument
                {
                    OptionCheckSyntax = true,
                    OptionFixNestedTags = false, 
                    OptionAutoCloseOnEnd = true
                };
            htmlDoc.LoadHtml(html);

            //add tag type
            AppendHtmlType(new HToXaml("h1"));
            AppendHtmlType(new HToXaml("h2"));
            AppendHtmlType(new HToXaml("h3"));
            AppendHtmlType(new HToXaml("h4"));
            AppendHtmlType(new HToXaml("h5"));
            AppendHtmlType(new HToXaml("h6"));
            AppendHtmlType(new HToXaml("h7"));
            AppendHtmlType(new BrToXaml());
            AppendHtmlType(new IToXaml());
            AppendHtmlType(new StrongToXaml("b"));
            AppendHtmlType(new StrongToXaml("strong"));
            AppendHtmlType(new AToXaml());

            //add special type
            AppendHtmlType(new ImageToXaml());
            AppendHtmlType(new ULToXaml());
            AppendHtmlType(new OLToXaml());
            AppendHtmlType(new DLToXaml());
            AppendHtmlType(new BlockQuoteToXaml());
            AppendHtmlType(new TableToXaml());

            //add default type
            AppendHtmlType(new DefaultToXaml());
        }

        /// <summary>
        /// Apply RichTextBox
        /// </summary>
        /// <param name="rtb"></param>
        public void ApplyHtmlToRichTextBox(RichTextBlock rtb)
        {
            rtb.Blocks.Clear();
            Paragraph block = new Paragraph();
            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
                ConvertHtmlNode(node, block);
            rtb.Blocks.Add(block);
        }

        /// <summary>
        /// Append Html Type
        /// </summary>
        /// <param name="hType"></param>
        public void AppendHtmlType(IHtmlType hType)
        {

            if (ruleDic.ContainsKey(hType.TagName))
                ruleDic[hType.TagName] = hType;
            else
                ruleDic.Add(hType.TagName, hType);
        }

        private void ConvertHtmlNode(HtmlNode htmlNode, Block block)
        {
            string htmlNodeName = htmlNode.Name.ToLower();

            if (new[] { "p", "div" }.Contains(htmlNodeName))
            {
                foreach (var childHtmlNode in htmlNode.ChildNodes)
                {
                    if (string.IsNullOrEmpty(htmlNode.InnerHtml))
                        continue;
                    ConvertHtmlNode(childHtmlNode, block);
                }
                var br = new LineBreak();
                (block as Paragraph).Inlines.Add(br);
            }

            if (new[] { "span" }.Contains(htmlNodeName))
            {
                foreach (var childHtmlNode in htmlNode.ChildNodes)
                {
                    if (string.IsNullOrEmpty(htmlNode.InnerHtml))
                        continue;
                    ConvertHtmlNode(childHtmlNode, block);
                }
            }

            if (ruleDic.ContainsKey(htmlNodeName))
                ruleDic[htmlNodeName].ApplyType(htmlNode, block);
        }
    }
}
