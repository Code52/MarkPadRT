using System.Linq;
using HtmlAgilityPack;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace MarkPad.Core.HtmlType
{
    public class TableToXaml : HtmlTypeBase
    {
        public override string TagName
        {
            get { return "table"; }
        }

        public override void ApplyType(HtmlNode htmlNode, Block block)
        {
            var rows = htmlNode.ChildNodes
                .Where(a => a.Name.ToLower() == "tr");
            if (rows.Count() == 0)
                return;

            var cols = rows.First().ChildNodes
                .Where(a => a.Name.ToLower() == "td");
            if (cols.Count() == 0)
                return;

            Grid tableGrid = new Grid();
            //tableGrid.ShowGridLines = true;

            var style = htmlNode.Attributes["style"];
            if (style != null)
            {
                var cssDic = RichTextboxStyle.GetCssStyle(style.Value);
                if (cssDic.ContainsKey("width"))
                {
                    string width = cssDic["width"].Replace("px", "").Replace("pt", "");
                    double ww;
                    double.TryParse(width, out ww);
                    tableGrid.Width = ww;
                }
            }

            for (int i = 0; i < rows.Count(); i++)
                tableGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < cols.Count(); i++)
                tableGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < rows.Count(); i++)
            {
                for (int j = 0; j < cols.Count(); j++)
                {
                    try
                    {
                        TextBlock tb = new TextBlock();
                        var htmlNodeCol = htmlNode
                            .ChildNodes.Where(a => a.Name.ToLower() == "tr").ElementAt(i)
                            .ChildNodes.Where(a1 => a1.Name.ToLower() == "td").ElementAt(j);
                        tb.Text = htmlNodeCol.InnerText;
                        tb.SetValue(Grid.RowProperty, i);
                        tb.SetValue(Grid.ColumnProperty, j);
                        tableGrid.Children.Add(tb);
                    }
                    catch
                    { }
                }
            }

            InlineUIContainer ilContainer = new InlineUIContainer();
            ilContainer.Child = tableGrid;
            (block as Paragraph).Inlines.Add(ilContainer);
        }
    }
}
