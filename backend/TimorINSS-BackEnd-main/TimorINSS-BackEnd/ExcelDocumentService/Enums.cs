using System;

namespace TimorINSSBackEnd.ExcelDocumentService
{
    public class Enums
    {
        [Flags]
        public enum ExcelDocumentBorderType
        {
            Top = 1,
            Right = 2,
            Bottom = 4,
            Left = 8
        }
    }
}