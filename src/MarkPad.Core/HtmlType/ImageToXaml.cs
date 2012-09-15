using System;
using HtmlAgilityPack;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MarkPad.Core.HtmlType
{
    public class ImageToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "img"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var src = htmlNode.Attributes["src"];
            if (src == null)
                throw new Exception("請輸入src");

            InlineUIContainer ilContainer = new InlineUIContainer();
            var alt = htmlNode.Attributes["alt"];

            Image img = new Image();
            ImageSource imgSource = new BitmapImage(new Uri(src.Value, UriKind.RelativeOrAbsolute));
            img.Source = imgSource;
            img.Stretch = Stretch.None;
            if (alt != null && !string.IsNullOrEmpty(alt.Value))
                ToolTipService.SetToolTip(img, alt);
            ilContainer.Child = img;
            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
