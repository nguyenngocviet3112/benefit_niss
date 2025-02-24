using OfficeOpenXml.Style;
using static TimorINSSBackEnd.ExcelDocumentService.Enums;

namespace TimorINSSBackEnd.ExcelDocumentService
{
    public class Models
    {
        public class ExcelDocumentTextStyle
        {
            /// <summary>
            /// Tamanho do texto em pixeis
            /// </summary>
            public int FontSize { get; set; } = 12;
            /// <summary>
            /// Cor de fundo da célula
            /// </summary>
            public string BackgroundColor { get; set; }
            /// <summary>
            /// Cor do texto
            /// </summary>
            public string Color { get; set; } = "#000000";
            /// <summary>
            /// Opção de negrito
            /// </summary>
            public bool Bold { get; set; } = false;
            /// <summary>
            /// Alinhamento horizontal do texto em relação à célula
            /// </summary>
            public ExcelHorizontalAlignment HorizontalAlign { get; set; } = ExcelHorizontalAlignment.Center;
            /// <summary>
            /// Alinhamento vertical do texto em relação à célula
            /// </summary>
            public ExcelVerticalAlignment VerticalAlign { get; set; } = ExcelVerticalAlignment.Center;
            /// <summary>
            /// Estilo da border
            /// </summary>
            public ExcelBorderStyle BorderStyle { get; set; } = ExcelBorderStyle.Thin;
            /// <summary>
            /// Posição das borders
            /// </summary>
            public ExcelDocumentBorderType Border { get; set; }
            /// <summary>
            /// Cor da border
            /// </summary>
            public string BorderColor { get; set; } = "#000000";
            /// <summary>
            /// Máscara para os números
            /// </summary>
            public string NumberFormat { get; set; }
            /// <summary>
            /// Estilo de preenchimento da célula
            /// </summary>
            public ExcelFillStyle BackgroundPatternType { get; set; } = ExcelFillStyle.None;
            /// <summary>
            /// O texto fica todo visível independente do tamanho da célula
            /// </summary>
            public bool WrapText { get; set; } = false;
        }

        public class ExcelDocumentTextPosition
        {
            public ExcelDocumentTextPosition(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        public class ExcelDocumentTextPositionRange
        {
            public ExcelDocumentTextPositionRange(ExcelDocumentTextPosition from, ExcelDocumentTextPosition to)
            {
                From = from;
                To = to;
            }

            public ExcelDocumentTextPosition From { get; set; }
            public ExcelDocumentTextPosition To { get; set; }
        }

        public delegate object ColumnOptionValue<T>(T value);
        public delegate string ColumnOptionString<T>(T value);

        public struct ColumnOption<T>
        {
            /// <summary>
            /// Nome da coluna
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Method que retorna o valor para mostrar nos resultados
            /// </summary>
            public ColumnOptionValue<T> Value { get; set; }
            /// <summary>
            /// Chave do estilo para as colunas
            /// </summary>
            public string ColumnTextStyleKey { get; set; }
            /// <summary>
            /// Chave do estilo para os resultados
            /// </summary>
            public string DataTextStyleKey { get; set; }
            /// <summary>
            /// Method que recebe o item e retorna a chave do estilo para os resultados
            /// </summary>
            public ColumnOptionString<T> DataTextStyleKeyFunc { get; set; }
            /// <summary>
            /// Quantidade de células que a coluna deve ocupar (faz união para a coluna ocupar mais linhas)
            /// </summary>
            public int? RowSpan { get; set; }
            /// <summary>
            /// Largura da coluna
            /// </summary>
            public double? Width { get; set; }
        }

        public class TableOptions
        {
            /// <summary>
            /// Opção de filtrar, se estiver a true, permite ao utilizador filtrar os resultados na coluna
            /// </summary>
            public bool Filter { get; set; } = true;
            /// <summary>
            /// Opção de AutoFit, se estiver a true, a largura das colunas será ajustado automáticamente consoante o conteúdo (só funciona caso não tenha sido atríbuido RowSpan nem Width nas opções das colunas)
            /// </summary>
            public bool AutoFitColumns { get; set; } = true;
        }
    }
}