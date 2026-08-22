using log4net;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static TimorINSSBackEnd.ExcelDocumentService.Enums;
using static TimorINSSBackEnd.ExcelDocumentService.Models;

namespace TimorINSSBackEnd.ExcelDocumentService
{
    public class ExcelDocument
    {
        /// <summary>
        /// Class base do excel
        /// </summary>
        private ExcelPackage ExcelPackage { get; set; }
        /// <summary>
        /// Folhas do excel
        /// </summary>
        public List<ExcelDocumentPage> Pages { get; private set; }
        private ExcelDocumentOptions Options { get; set; }
        /// <summary>
        /// Estilos de textos e células
        /// </summary>
        public Dictionary<string, ExcelDocumentTextStyle> TextStyles { get; private set; }

        /// <summary>
        /// Cria um novo documento de excel com várias páginas
        /// </summary>
        /// <param name="pageNames">Nomes das páginas</param>
        /// <param name="options">Opções para o documento</param>
        public ExcelDocument(List<string> pageNames, ExcelDocumentOptions options = default)
        {
            ExcelPackage = new ExcelPackage();
            Options = options;
            TextStyles = options.TextStyles ?? new Dictionary<string, ExcelDocumentTextStyle>();
            // Criação das páginas consoante a lista "pageNames"
            Pages = pageNames.Select(e => new ExcelDocumentPage(e, ExcelPackage, Options, TextStyles)).ToList();
        }

        /// <summary>
        /// Cria um novo documento de excel com uma página
        /// </summary>
        /// <param name="pageName">Nome da página</param>
        /// <param name="options">Opções para o documento</param>
        public ExcelDocument(string pageName, ExcelDocumentOptions options = default)
        {
            ExcelPackage = new ExcelPackage();
            Options = options;
            TextStyles = options.TextStyles ?? new Dictionary<string, ExcelDocumentTextStyle>();
            // Criação de uma página consoante a "pageName"
            Pages = new List<ExcelDocumentPage>() { new ExcelDocumentPage(pageName, ExcelPackage, Options, TextStyles) };
        }

        /// <summary>
        /// Adiciona uma folha ao excel com o nome
        /// </summary>
        /// <param name="name">Nome da folha</param>
        public void AddPage(string name)
        {
            Pages.Add(new ExcelDocumentPage(name, ExcelPackage, Options, TextStyles));
        }

        /// <summary>
        /// Devolve o documento excel em byte[]
        /// </summary>
        public byte[] GetFileByteArray() => ExcelPackage.GetAsByteArray();

        /// <summary>
        /// Devolve o documento excel em base64
        /// </summary>
        public string GetFileString() => Convert.ToBase64String(ExcelPackage.GetAsByteArray());
    }

    public class ExcelDocumentOptions
    {
        /// <summary>
        /// Estilos de textos e células
        /// </summary>
        public Dictionary<string, ExcelDocumentTextStyle> TextStyles { get; set; }
        /// <summary>
        /// Protege o documento de excel
        /// </summary>
        public bool IsProtected { get; set; } = false;
        /// <summary>
        /// Permite a seleção de células bloqueadas
        /// </summary>
        public bool AllowSelectLockedCells { get; set; } = false;
    }

    public class ExcelDocumentPage
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExcelDocumentPage));

        private ExcelWorksheet ExcelWorkSheet { get; set; }

        /// <summary>
        /// Estilos de textos e células
        /// </summary>
        private Dictionary<string, ExcelDocumentTextStyle> TextStyles { get; set; }

        public ExcelDocumentPage(string page, ExcelPackage excelPackage, ExcelDocumentOptions options, Dictionary<string, ExcelDocumentTextStyle> textStyles)
        {
            //ExcelPackage = excelPackage;
            TextStyles = textStyles;
            ExcelWorkSheet = excelPackage.Workbook.Worksheets.Add(page);
            ExcelWorkSheet.Protection.IsProtected = options.IsProtected;
            ExcelWorkSheet.Protection.AllowSelectLockedCells = options.AllowSelectLockedCells;
        }

        /// <summary>
        /// Adiciona uma fórmula de soma numa célula ---> SUBTOTAL(9, ...)
        /// </summary>
        /// <param name="position">Posição da célula na folha</param>
        /// <param name="range">Range das células para somar</param>
        /// <param name="textStyleKey">Chave do estilo</param>
        /// <param name="mergeToPosition">Posição da célula para unir</param>
        public void AddSum(ExcelDocumentTextPosition position, ExcelDocumentTextPositionRange range, string textStyleKey = null, ExcelDocumentTextPosition mergeToPosition = null)
        {
            AddText("", position, textStyleKey, mergeToPosition);
            var cell = ExcelWorkSheet.Cells[position.Y, position.X];

            var fromCell = ExcelWorkSheet.Cells[range.From.Y, range.From.X];
            var toCell = ExcelWorkSheet.Cells[range.To.Y, range.To.X];
            cell.Formula = $"SUBTOTAL(9, {fromCell.Address}:{toCell.Address})";
        }

        /// <summary>
        /// Adiciona texto numa célula
        /// </summary>
        /// <param name="text">O texto para aparecer</param>
        /// <param name="position">Posição da célula na folha</param>
        /// <param name="textStyleKey">Chave do estilo</param>
        /// <param name="mergeToPosition">Posição da célula para unir</param>
        public void AddText(object text, ExcelDocumentTextPosition position, string textStyleKey = null, ExcelDocumentTextPosition mergeToPosition = null)
        {
            var cell = ExcelWorkSheet.Cells[position.Y, position.X];
            var textStyle = (textStyleKey != null && TextStyles.ContainsKey(textStyleKey) ? TextStyles[textStyleKey] : TextStyles.FirstOrDefault().Value) ?? default;

            cell.Value = text;
            cell.Style.Font.Size = textStyle.FontSize;
            cell.Style.HorizontalAlignment = textStyle.HorizontalAlign;
            cell.Style.VerticalAlignment = textStyle.VerticalAlign;
            if (textStyle.WrapText) cell.Style.WrapText = true;

            if (textStyle.BackgroundPatternType == ExcelFillStyle.None && textStyle.BackgroundColor != null) textStyle.BackgroundPatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.PatternType = textStyle.BackgroundPatternType;
            if (textStyle.BackgroundPatternType != ExcelFillStyle.None && textStyle.BackgroundColor != null) cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(textStyle.BackgroundColor));
            cell.Style.Font.Color.SetColor(ColorTranslator.FromHtml(textStyle.Color));
            cell.Style.Font.Bold = textStyle.Bold;
            if (textStyle.NumberFormat != null) cell.Style.Numberformat.Format = textStyle.NumberFormat;
            //if (textStyle.AutoFit) cell.AutoFitColumns();

            if (textStyle.Border.HasFlag(ExcelDocumentBorderType.Top))
            {
                cell.Style.Border.Top.Style = textStyle.BorderStyle;
                cell.Style.Border.Top.Color.SetColor(ColorTranslator.FromHtml(textStyle.BorderColor));
            }
            if (textStyle.Border.HasFlag(ExcelDocumentBorderType.Right))
            {
                cell.Style.Border.Right.Style = textStyle.BorderStyle;
                cell.Style.Border.Right.Color.SetColor(ColorTranslator.FromHtml(textStyle.BorderColor));
            }
            if (textStyle.Border.HasFlag(ExcelDocumentBorderType.Bottom))
            {
                cell.Style.Border.Bottom.Style = textStyle.BorderStyle;
                cell.Style.Border.Bottom.Color.SetColor(ColorTranslator.FromHtml(textStyle.BorderColor));
            }
            if (textStyle.Border.HasFlag(ExcelDocumentBorderType.Left))
            {
                cell.Style.Border.Left.Style = textStyle.BorderStyle;
                cell.Style.Border.Left.Color.SetColor(ColorTranslator.FromHtml(textStyle.BorderColor));
            }

            if (mergeToPosition != null) ExcelWorkSheet.Cells[position.Y, position.X, mergeToPosition.Y, mergeToPosition.X].Merge = true;
        }

        /// <summary>
        /// Adiciona uma tabela com cabeçalho e linhas de resultado
        /// </summary>
        /// <typeparam name="T">Modelo dos items da listagem</typeparam>
        /// <param name="startPosition">Posição da célula na folha (corresponde ao canto superior esquerdo da tabela que vai ser gerada)</param>
        /// <param name="data">Resultados para apresentar na listagem</param>
        /// <param name="columnOptions">Opções das colunas</param>
        /// <param name="options">Opções extra da tabela</param>
        public void AddTable<T>(ExcelDocumentTextPosition startPosition, List<T> data, List<ColumnOption<T>> columnOptions, TableOptions options = null)
        {
            var maxRowSpan = columnOptions.Max(e => e.RowSpan ?? 0);
            if (maxRowSpan == 1 || maxRowSpan < 0) maxRowSpan = 0;

            int col = startPosition.X;
            int row = startPosition.Y + maxRowSpan;

            foreach (var column in columnOptions)
            {
                var rowSpan = column.RowSpan ?? 0;
                if (rowSpan == 1 || rowSpan < 0) rowSpan = 0;

                if (column.Width != null)
                {
                    ExcelWorkSheet.Column(col).Width = column.Width.Value;
                }
                AddText(column.Name, new ExcelDocumentTextPosition(col, row - rowSpan), column.ColumnTextStyleKey, rowSpan > 1 ? new ExcelDocumentTextPosition(col, row) : null);
                col++;
            }

            col = startPosition.X;
            row++;

            foreach (var item in data)
            {
                foreach (var column in columnOptions)
                {
                    AddText(column.Value(item), new ExcelDocumentTextPosition(col, row), column.DataTextStyleKeyFunc?.Invoke(item) ?? column.DataTextStyleKey);
                    col++;
                }
                col = startPosition.X;
                row++;
            }

            options ??= new TableOptions();

            if (options.Filter) ExcelWorkSheet.Cells[startPosition.Y + maxRowSpan, startPosition.X, startPosition.Y + maxRowSpan, startPosition.X + columnOptions.Count - 1].AutoFilter = true;
            // [PT] O auto-fit do EPPlus 4 mede o texto atraves do System.Drawing (GDI+). Em Linux
            // isso exige a libgdiplus instalada; sem ela o .NET lanca TypeInitializationException
            // em 'Gdip' e o pedido inteiro rebentava com HTTP 400 -- por causa de largura de
            // colunas, que e so estetica. Aqui o ficheiro sai na mesma, apenas sem auto-fit.
            // [VI] Auto-fit cua EPPlus 4 do be rong chu bang System.Drawing (GDI+). Tren Linux
            // can co libgdiplus; thieu no thi .NET nem TypeInitializationException o 'Gdip' va
            // ca request chet voi HTTP 400 -- chi vi do rong cot, thuan tuy tham my. O day file
            // Excel van xuat binh thuong, chi khong duoc auto-fit.
            if (options.AutoFitColumns)
            {
                try
                {
                    ExcelWorkSheet.Cells[ExcelWorkSheet.Dimension.Address].AutoFitColumns();
                }
                catch (Exception ex)
                {
                    Log.Warn("AutoFitColumns indisponivel (GDI+/libgdiplus em falta); a gerar o Excel sem auto-fit de colunas.", ex);
                }
            }
        }
    }
}