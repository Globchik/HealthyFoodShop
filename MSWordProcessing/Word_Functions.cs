using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace HealthyFood_Shop
{
    static class Word_Functions
    {
        public static string? GetFilepathFromSaveFileWindow(in string file_type, in string description, in string? def_name = null)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog svf = new();
                svf.DefaultExt = file_type;
                if(def_name is not null)
                    svf.FileName = def_name;
                svf.Filter = description + "|*" + file_type;
                bool? res = svf.ShowDialog();
                if (res == true)
                    return svf.FileName;
                else
                    return null;
            }
            catch (Exception)
            {
                throw new Exception("Немає доступу до файлу!");
            }
        }

        public static void MakeTableRowBold(in Table table, int row_index)
        {
            TableRow row = table.Elements<TableRow>().ElementAt(row_index);
            foreach (TableCell cell in row.Elements<TableCell>())
            {
                foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                {
                    foreach (Run run in paragraph.Elements<Run>())
                    {
                        RunProperties? runProperties = run.GetFirstChild<RunProperties>();
                        if (runProperties is null)
                        {
                            runProperties = new RunProperties();
                            run.PrependChild(runProperties);
                        }
                        runProperties.Bold = new Bold();
                    }
                }
            }
        }

        public static void InsertParagraphToDoc(WordprocessingDocument doc, in string text,
            in JustificationValues? just = null, in int half_font_size = 28,
            bool is_bold = false, bool is_italic = false)
        {
            if (doc is null || doc.MainDocumentPart is null ||
                doc.MainDocumentPart.Document is null
                || doc.MainDocumentPart.Document.Body is null)
                throw new Exception("Немає доступу до документу");

            Paragraph paragraph = doc.MainDocumentPart.Document.
               Body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new();
            paragraph.ParagraphProperties.AddChild(new Justification()
            {
                Val = just is null? JustificationValues.Left : just
            });
            Run run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
            run.RunProperties = new();
            if(is_bold)
                run.RunProperties.AddChild(new Bold());
            if (is_italic)
                run.RunProperties.AddChild(new Italic());
            run.RunProperties.AddChild(new FontSize() { Val = half_font_size.ToString() });
        }

        public static void AddCellToRow(TableRow my_row, in string text,
            JustificationValues? h_align = null, TableVerticalAlignmentValues? v_align = null )
        {
            var tcell = new TableCell();
            Paragraph c_par = new Paragraph(
                    new Run(new Text(text)));
            c_par.ParagraphProperties = new();
            c_par.ParagraphProperties.Justification = 
                new Justification() { Val = h_align.GetValueOrDefault(JustificationValues.Left) };
            tcell.TableCellProperties = new TableCellProperties();
            tcell.TableCellProperties.TableCellVerticalAlignment =
                new TableCellVerticalAlignment
                { Val = v_align.GetValueOrDefault(TableVerticalAlignmentValues.Center) };
            tcell.Append(c_par);
            my_row.Append(tcell);
        }



        public static void InsertLeftoversTable(WordprocessingDocument doc,
            in List<string[]> table)
        {
            if (doc is null || doc.MainDocumentPart is null ||
                doc.MainDocumentPart.Document is null
                || doc.MainDocumentPart.Document.Body is null)
                throw new Exception("Немає доступу до документу");

            //Таблиця
            Table dTable = new();
            TableProperties props = new();
            var borderValues = new EnumValue<BorderValues>(BorderValues.Single);
            var tableBorders = new TableBorders(
                                 new TopBorder { Val = borderValues, Size = 4 },
                                 new BottomBorder { Val = borderValues, Size = 4 },
                                 new LeftBorder { Val = borderValues, Size = 4 },
                                 new RightBorder { Val = borderValues, Size = 4 },
                                 new InsideHorizontalBorder { Val = borderValues, Size = 4 },
                                 new InsideVerticalBorder { Val = borderValues, Size = 4 },
                                 new TableCellMarginDefault(
                                     new TopMargin() { Width = "20", Type = TableWidthUnitValues.Dxa },
                                     new StartMargin() { Width = "114", Type = TableWidthUnitValues.Dxa },
                                     new BottomMargin() { Width = "20", Type = TableWidthUnitValues.Dxa },
                                     new EndMargin() { Width = "114", Type = TableWidthUnitValues.Dxa })
                                 );
            props.Append(tableBorders);
            var tableWidth = new TableWidth()
            {
                Width = "5000",
                Type = TableWidthUnitValues.Pct
            };
            props.Append(tableWidth);
            dTable.PrependChild<TableProperties>(props);



            var trow = new TableRow();
            AddCellToRow(trow, "№", JustificationValues.Center);
            AddCellToRow(trow, "Товар", JustificationValues.Center);
            AddCellToRow(trow, "Термін придатності партії", JustificationValues.Center);
            AddCellToRow(trow, "Кількість товару в партії", JustificationValues.Center);
            dTable.Append(trow);

            int sum_items = 0;
            TableCell prev_sum_cell = new();
            string prev_name = "";
            int table_row_count = 0;
            foreach (string[] row in table)
            {
                var tr = new TableRow();
                bool same_name = prev_name == row[0];
                if (same_name)
                    --table_row_count;
                else 
                    sum_items = 0;


                var count_cell = new TableCell();
                count_cell.Append(
                        new Paragraph(
                            new Run(
                                new Text((table_row_count+1).ToString())
                                )));
                count_cell.TableCellProperties = new TableCellProperties();
                count_cell.TableCellProperties.TableCellVerticalAlignment =
                    new TableCellVerticalAlignment
                    { Val = TableVerticalAlignmentValues.Center };
                count_cell.TableCellProperties.VerticalMerge = new VerticalMerge
                {
                    Val = same_name ?
                            MergedCellValues.Continue : MergedCellValues.Restart
                };
                if (table_row_count % 2 == 0)
                {
                    var shading = new Shading()
                    {
                        Color = "auto",
                        Fill = "F2F2F2",
                        Val = ShadingPatternValues.Clear
                    };
                    count_cell.TableCellProperties.Append(shading);
                }
                tr.Append(count_cell);

                for (int i = 0; i < row.Length; ++i)
                {
                    var tc = new TableCell();
                    tc.Append(
                        new Paragraph(
                            new Run(
                                new Text(row[i])
                                )));
                    tc.TableCellProperties = new TableCellProperties();
                    tc.TableCellProperties.TableCellVerticalAlignment =
                        new TableCellVerticalAlignment
                        { Val = TableVerticalAlignmentValues.Center };

                    if (i == 0)
                    {
                        tc.TableCellProperties.VerticalMerge = new VerticalMerge
                        {
                            Val = same_name ?
                            MergedCellValues.Continue : MergedCellValues.Restart
                        };
                    }
                    if (i == 2)
                        sum_items += Convert.ToInt32(row[i]);


                    if (table_row_count % 2 == 0)
                    {
                        var shading = new Shading()
                        {
                            Color = "auto",
                            Fill = "F2F2F2",
                            Val = ShadingPatternValues.Clear
                        };
                        tc.TableCellProperties.Append(shading);
                    }
                    tr.Append(tc);
                }

                var total_cell = new TableCell();
                if (!same_name)
                    prev_sum_cell = total_cell;
                total_cell.Append(
                    new Paragraph(
                        new Run(
                            new Text(same_name? "" : sum_items.ToString())
                            )));
                prev_sum_cell.RemoveAllChildren<Paragraph>();
                prev_sum_cell.Append(
                    new Paragraph(
                        new Run(
                            new Text(sum_items.ToString())
                            )));

                total_cell.TableCellProperties = new TableCellProperties();
                total_cell.TableCellProperties.TableCellVerticalAlignment =
                    new TableCellVerticalAlignment
                    { Val = TableVerticalAlignmentValues.Center };
                total_cell.TableCellProperties.VerticalMerge = new VerticalMerge
                {
                    Val = same_name ?
                            MergedCellValues.Continue : MergedCellValues.Restart
                };
                if (table_row_count % 2 == 0)
                {
                    var shading = new Shading()
                    {
                        Color = "auto",
                        Fill = "F2F2F2",
                        Val = ShadingPatternValues.Clear
                    };
                    total_cell.TableCellProperties.Append(shading);
                }
                tr.Append(total_cell);

                prev_name = row[0];
                dTable.Append(tr);
                ++table_row_count;
            }


            if (doc is null || doc.MainDocumentPart is null ||
                doc.MainDocumentPart.Document is null
                || doc.MainDocumentPart.Document.Body is null)
                throw new Exception("Немає доступу до документу");
            doc.MainDocumentPart.Document.Body.Append(dTable);
        }



        public static void CreateStorageLeftoversWordReport(in string full_filepath)
        {
            using WordprocessingDocument doc =
                WordprocessingDocument.Create(full_filepath,
                WordprocessingDocumentType.Document, true);
            MainDocumentPart mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            mainPart.Document.Body = new Body();
            SectionProperties props = new SectionProperties();
            mainPart.Document.Body.AppendChild(props);

            string para_text = "Інвентаризаційний звіт про залишки на складі";
            //Заголовок
            InsertParagraphToDoc(doc, para_text,
                JustificationValues.Center, 32, true);

            if (doc is null || doc.MainDocumentPart is null ||
                doc.MainDocumentPart.Document is null
                || doc.MainDocumentPart.Document.Body is null)
                throw new Exception("Немає доступу до документу");


            Paragraph paragraph = doc.MainDocumentPart.Document.
               Body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new();
            paragraph.ParagraphProperties.AddChild(
                new Justification() { Val = JustificationValues.Left });
            Run run = paragraph.AppendChild(new Run());
            Text txt = run.AppendChild(new Text("Звіт для магазину: "));
            txt.Space = SpaceProcessingModeValues.Preserve;
            run.RunProperties = new();
            run.RunProperties.AddChild(new FontSize() { Val = "28" });
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text("\"" + CurrentSettings.ShortShopName + "\""));
            run.RunProperties = new();
            run.RunProperties.AddChild(new Underline() { Val = UnderlineValues.Single });
            run.RunProperties.AddChild(new FontSize() { Val = "28" });


            paragraph = doc.MainDocumentPart.Document.
               Body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new();
            paragraph.ParagraphProperties.AddChild(
                new Justification() {Val = JustificationValues.Left});
            run = paragraph.AppendChild(new Run());
            txt = run.AppendChild(new Text("Дата створення звіту: "));
            txt.Space = SpaceProcessingModeValues.Preserve;
            run.RunProperties = new();
            run.RunProperties.AddChild(new FontSize() { Val = "28"});
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(DateTime.Now.ToString("yyyy-MM-dd")));
            run.RunProperties = new();
            run.RunProperties.AddChild(new Underline() { Val = UnderlineValues.Single });
            run.RunProperties.AddChild(new FontSize() { Val = "28" });

            paragraph = doc.MainDocumentPart.Document.
         Body.AppendChild(new Paragraph());
            paragraph.ParagraphProperties = new();
            paragraph.ParagraphProperties.AddChild(
                new Justification() { Val = JustificationValues.Left });
            run = paragraph.AppendChild(new Run());
            txt = run.AppendChild(new Text("ПІБ працівника, який сформував звіт: "));
            txt.Space = SpaceProcessingModeValues.Preserve;
            run.RunProperties = new();
            run.RunProperties.AddChild(new FontSize() { Val = "28" });
            run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(CurrentUser.current_user.PIB));
            run.RunProperties = new();
            run.RunProperties.AddChild(new Underline() { Val = UnderlineValues.Single });
            run.RunProperties.AddChild(new FontSize() { Val = "28" });

            InsertParagraphToDoc(doc,"");
            para_text = "Залишки на складі";
            InsertParagraphToDoc(doc, para_text,
                JustificationValues.Center, 28);

            List<string[]> tb;
            SQLite_Statistics.GetStockAvailableTable(out tb);
            InsertLeftoversTable(doc, tb);

            InsertParagraphToDoc(doc, "");
            para_text = "Товари, у яких вийшов строк придатності";
            InsertParagraphToDoc(doc, para_text,
                JustificationValues.Center, 28);

            SQLite_Statistics.GetStockExpiredTable(out tb);
            InsertLeftoversTable(doc, tb);
        }



    }

}
