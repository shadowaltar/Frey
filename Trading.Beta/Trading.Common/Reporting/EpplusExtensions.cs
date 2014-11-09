using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using OfficeOpenXml;

namespace Trading.Common.Reporting
{
    public static class ExcelHelper
    {
        public static ExcelPackage New(string fileName)
        {
            return new ExcelPackage(new FileInfo(fileName));
        }

        public static ExcelWorksheet Sheet(this ExcelPackage p, string sheetName)
        {
            var sheet = p.Workbook.Worksheets.FirstOrDefault(s => string.Equals(s.Name, sheetName, StringComparison.InvariantCultureIgnoreCase));
            if (sheet == null)
            {
                return p.Workbook.Worksheets.Add(sheetName);
            }
            return sheet;
        }

        public static ExcelWorksheet SetValue(this ExcelWorksheet sheet, string address, object value)
        {
            sheet.Cells[address].Value = value;
            return sheet;
        }

        public static ExcelWorksheet SetValue(this ExcelWorksheet sheet, int rowIndex, int colIndex, object value)
        {
            sheet.Cells[rowIndex, colIndex].Value = value;
            return sheet;
        }

        public static ExcelWorksheet SetValues<T1, T2>(this ExcelWorksheet sheet, int startRowIndex, int startColIndex,
            Dictionary<T1, T2> values)
        {
            foreach (var pair in values)
            {
                sheet.Cells[startRowIndex, startColIndex].Value = pair.Key;
                sheet.Cells[startRowIndex, startColIndex + 1].Value = pair.Value;
                startRowIndex++;
            }
            return sheet;
        }

        public static ExcelWorksheet SetKeyAndValuesVertical(this ExcelWorksheet sheet, int startRowIndex, int startColIndex,
            params object[] values)
        {
            if (values.Length % 2 != 0)
                throw new InvalidOperationException();

            for (int i = 0; i < values.Length; i++)
            {
                sheet.Cells[startRowIndex, startColIndex].Value = values[i];
                sheet.Cells[startRowIndex, startColIndex + 1].Value = values[i + 1];
                startRowIndex++;
            }
            return sheet;
        }
    }
}