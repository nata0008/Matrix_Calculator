using System;
using System.Data;

namespace Matrix
{
    // Допоміжний клас для перетворення даних між об'єктами MatrixModel та DataTable.
    // Потрібен для того, щоб зручно передавати дані з нашої математичної логіки у візуальний DataGrid.
    public static class MatrixGridHelper
    {
        // Створює порожню таблицю заданого розміру (заповнену нулями)
        // Використовується при ініціалізації матриць або зміні їх розміру користувачем
        public static DataTable CreateEmptyTable(int rows, int cols)
        {
            var table = new DataTable();
            for (int j = 0; j < cols; j++)
                table.Columns.Add((j + 1).ToString(), typeof(string));

            for (int i = 0; i < rows; i++)
            {
                var row = table.NewRow();
                for (int j = 0; j < cols; j++)
                    row[j] = "0";
                table.Rows.Add(row);
            }
            return table;
        }

        // Перетворює дані з візуальної таблиці інтерфейсу (DataTable) у об'єкт MatrixModel для обчислень
        public static MatrixModel TableToMatrix(DataTable table, string matrixName = "Матриця")
        {
            int rows = table.Rows.Count;
            int cols = table.Columns.Count;
            var matrix = new MatrixModel(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string cellValue = table.Rows[i][j]?.ToString() ?? "0";

                    // Замінюємо кому на крапку, щоб уникнути помилок
                    cellValue = cellValue.Replace(',', '.');

                    // Спроба перетворити текст клітинки у число. Якщо там текст (наприклад "abc") - кидаємо помилку
                    if (!double.TryParse(cellValue,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out double val))
                    {
                        throw new FormatException(
                            $"{matrixName}: клітинка [{i + 1},{j + 1}] містить некоректне значення.");
                    }
                    matrix[i, j] = val;
                }
            }
            return matrix;
        }

        // Перетворює обчислену матрицю (MatrixModel) назад у таблицю (DataTable) для виведення на екран
        public static DataTable MatrixToTable(MatrixModel matrix)
        {
            var table = new DataTable();
            for (int j = 0; j < matrix.Cols; j++)
                table.Columns.Add((j + 1).ToString(), typeof(string));

            for (int i = 0; i < matrix.Rows; i++)
            {
                var row = table.NewRow();
                for (int j = 0; j < matrix.Cols; j++)
                {
                    double val = matrix[i, j];

                    // Форматування: якщо число ціле (наприклад 5.0), виводимо "5", інакше залишаємо до 2 знаків після коми
                    row[j] = (val == Math.Floor(val)) ? val.ToString("0") : val.ToString("0.##");
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}