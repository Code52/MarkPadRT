using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace MarkPad.Core
{
    public class RichTextboxStyle
    {
        public string FontFamily { get; set; }
        public double? FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }
        public Brush Foreground { get; set; }
        public FontWeight? FontWeight { get; set; }

        public static RichTextboxStyle GetDefault(HtmlNode htmlNode)
        {
            Dictionary<string, string> cssStyle = GetCssStyle(htmlNode);
            RichTextboxStyle rtStyle = new RichTextboxStyle();

            try
            {
                if (cssStyle.ContainsKey("font-family"))
                    rtStyle.FontFamily = cssStyle["font-family"];
                if (cssStyle.ContainsKey("font-size"))
                {
                    string size = cssStyle["font-size"];
                    size = size.Replace("px", "").Replace("pt", "");
                    rtStyle.FontSize = double.Parse(size);
                }
                if (cssStyle.ContainsKey("font-style"))
                {
                    var v = cssStyle["font-style"].ToLower();
                    rtStyle.FontStyle = v == "italic" ? Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal;
                }
                if (cssStyle.ContainsKey("font-weight"))
                {
                    var v = cssStyle["font-weight"].ToLower();
                    if (v == "bold")
                        rtStyle.FontWeight = FontWeights.Bold;
                    else
                        rtStyle.FontWeight = FontWeights.Normal;
                }
                if (cssStyle.ContainsKey("color"))
                {
                    var v = cssStyle["color"].ToLower();
                    rtStyle.Foreground = ConvertStringToColorBrush(v);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("設定style錯誤:", ex.Message));
            }

            return rtStyle;
        }

        private static Dictionary<string, string> GetCssStyle(HtmlNode htmlNode)
        {
            //parent
            Dictionary<string, string> cssStyleDic;
            if (htmlNode.Name.ToLower() == "#document")
                cssStyleDic = new Dictionary<string, string>();
            else
                cssStyleDic = GetCssStyle(htmlNode.ParentNode);

            //parseStyle
            var style = htmlNode.Attributes["style"];
            if (style == null)
                return cssStyleDic;

            var cssSetting = GetCssStyle(style.Value);
            foreach (var item in cssSetting)
            {
                if (cssStyleDic.ContainsKey(item.Key))
                    cssStyleDic[item.Key] = item.Value;
                else
                    cssStyleDic.Add(item.Key, item.Value);
            }

            return cssStyleDic;
        }

        public static Dictionary<string, string> GetCssStyle(string style)
        {
            if (string.IsNullOrEmpty(style))
                return new Dictionary<string, string>();

            try
            {
                return style.Split(';').Where(a => a != "").Select(a => a.Trim())
                        .ToDictionary(a => a.Split(':')[0].Trim(), b => b.Split(':')[1].Trim());
            }
            catch
            {
                throw new Exception("style設定錯誤");
            }
        }

        public static void SetStyle(Inline inline, RichTextboxStyle style)
        {
            if (!string.IsNullOrEmpty(style.FontFamily))
                inline.FontFamily = new FontFamily(style.FontFamily);

            if (style.FontSize.HasValue)
                inline.FontSize = style.FontSize.Value;

            if (style.Foreground != null)
                inline.Foreground = style.Foreground;

            if (style.FontWeight.HasValue)
                inline.FontWeight = style.FontWeight.Value;

            if (style.FontStyle.HasValue)
                inline.FontStyle = style.FontStyle.Value;
        }

        public static SolidColorBrush ConvertStringToColorBrush(string colorStr)
        {
            try
            {
                string s = string.Format(@"
<SolidColorBrush 
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
Color=""{0}"" />"
, colorStr);

                return (SolidColorBrush)XamlReader.Load(s);
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
