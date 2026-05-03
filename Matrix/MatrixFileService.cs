using System;
using System.IO;
using System.Text;

namespace Matrix
{
    // Відповідає за збереження та завантаження матриць у текстовий файл.
    // Формат файлу: спочатку розмір, потім рядки з числами через пробіл.
   
    public static class MatrixFileService
    {
        // Ім'я файлу поруч із програмою
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "matrices.txt");

        
        // Зберігає дві матриці у файл.
        public static void Save(MatrixModel matrixA, MatrixModel matrixB)
        {
            var sb = new StringBuilder();

            sb.AppendLine("# Матриця A");
            WriteMatrix(sb, matrixA);

            sb.AppendLine("# Матриця B");
            WriteMatrix(sb, matrixB);

            File.WriteAllText(FilePath, sb.ToString(), Encoding.UTF8);
        }

        // Завантажує дві матриці з файлу.
        // Повертає false якщо файл не існує або пошкоджений.
        public static bool TryLoad(out MatrixModel matrixA, out MatrixModel matrixB)
        {
            matrixA = null;
            matrixB = null;

            if (!File.Exists(FilePath))
                return false;

            try
            {
                string[] lines = File.ReadAllLines(FilePath, Encoding.UTF8);
                int index = 0;

                matrixA = ReadMatrix(lines, ref index);
                matrixB = ReadMatrix(lines, ref index);

                return matrixA != null && matrixB != null;
            }
            catch
            {
                // Якщо файл пошкоджений — просто починаємо з чистого аркуша
                return false;
            }
        }

        // ── Приватні методи ───────────────────────────────────────────

        private static void WriteMatrix(StringBuilder sb, MatrixModel m)
        {
            sb.AppendLine($"{m.Rows} {m.Cols}");
            for (int i = 0; i < m.Rows; i++)
            {
                var parts = new string[m.Cols];
                for (int j = 0; j < m.Cols; j++)
                    parts[j] = m[i, j].ToString(
                        System.Globalization.CultureInfo.InvariantCulture);
                sb.AppendLine(string.Join(" ", parts));
            }
        }

        private static MatrixModel ReadMatrix(string[] lines, ref int index)
        {
            // Пропускаємо коментарі та порожні рядки
            while (index < lines.Length &&
                   (lines[index].StartsWith("#") || string.IsNullOrWhiteSpace(lines[index])))
                index++;

            if (index >= lines.Length)
                return null;

            // Читаємо розмір: "2 3"
            var sizeParts = lines[index++].Trim().Split(' ');
            int rows = int.Parse(sizeParts[0]);
            int cols = int.Parse(sizeParts[1]);

            var matrix = new MatrixModel(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                var nums = lines[index++].Trim().Split(' ');
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = double.Parse(nums[j],
                        System.Globalization.CultureInfo.InvariantCulture);
            }

            return matrix;
        }
    }
}