using HtmlAgilityPack;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public interface IHtmlType
    {
        string TagName { get; }
        void ApplyType(HtmlNode htmlNode, Block block);
    }
}
