using HtmlAgilityPack;
using MarkPad.Core.HtmlType;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core
{
    public abstract class HtmlTypeBase : IHtmlType
    {
        internal string GetCleanContent(string content)
        {
            return content.Replace("&", "&amp;")
                            .Replace("\"", "&quot;")
                            .Replace("'", "`")
                            .Replace("\n", "")
                            .Replace("\r", "");
        }

        internal void TextToRun(string content, RichTextboxStyle style, Block block)
        {
            string cContent = GetCleanContent(content);
            if (string.IsNullOrEmpty(cContent))
                return;

            Run run = new Run();
            RichTextboxStyle.SetStyle(run, style);
            run.Text = cContent;

            (block as Paragraph).Inlines.Add(run);
        }

        internal void Br(Block block)
        {
            var br = new LineBreak();
            (block as Paragraph).Inlines.Add(br);
        }

        #region IHtmlRule Members

        public abstract string TagName { get; }

        public abstract void ApplyType(HtmlNode htmlNode, Block block);

        #endregion
    }

}
