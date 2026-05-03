using System;

namespace Matrix
{
    // Клас, що описує математичну матрицю та операції над нею.
    // Інкапсулює дані, щоб зовнішній код (інтерфейс) не міг їх випадково пошкодити.
    public class MatrixModel
    {
        // Внутрішнє сховище даних матриці. 
        // Зроблено readonly для захисту від перезапису самого масиву в пам'яті.
        private readonly double[,] _data;

        // Властивості для отримання розмірності матриці (доступні лише для читання ззовні).
        public int Rows { get; }
        public int Cols { get; }

        // Конструктор для створення порожньої матриці заданого розміру.
        public MatrixModel(int rows, int cols)
        {
            // Валідація: розміри не можуть бути від'ємними або нульовими.
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Розміри матриці повинні бути більше нуля.");

            Rows = rows;
            Cols = cols;
            _data = new double[rows, cols];
        }

        // Конструктор для створення матриці на основі вже існуючого двовимірного масиву.
        public MatrixModel(double[,] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Rows = data.GetLength(0);
            Cols = data.GetLength(1);

            // Робимо копію масиву (Clone), щоб зміни у вихідному масиві не впливали на нашу матрицю.
            _data = (double[,])data.Clone();
        }

        // Індексатор для зручного доступу до елементів (дозволяє писати matrix[0, 1] замість методів Get/Set).
        public double this[int row, int col]
        {
            get
            {
                ValidateIndex(row, col); // Перевіряємо, чи не виходимо за межі масиву
                return _data[row, col];
            }
            set
            {
                ValidateIndex(row, col);
                _data[row, col] = value;
            }
        }

        // Статичний метод для додавання двох матриць.
        public static MatrixModel Add(MatrixModel a, MatrixModel b)
        {
            CheckSameSize(a, b, "додавання"); // Матриці обов'язково повинні бути однакового розміру
            var result = new MatrixModel(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    result[i, j] = a[i, j] + b[i, j];
            return result;
        }

        // Статичний метод для віднімання двох матриць.
        public static MatrixModel Subtract(MatrixModel a, MatrixModel b)
        {
            CheckSameSize(a, b, "віднімання");
            var result = new MatrixModel(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    result[i, j] = a[i, j] - b[i, j];
            return result;
        }

        // Статичний метод для множення двох матриць (за правилом: рядок на стовпець).
        public static MatrixModel Multiply(MatrixModel a, MatrixModel b)
        {
            // Головне правило множення: кількість стовпців першої має дорівнювати кількості рядків другої.
            if (a.Cols != b.Rows)
                throw new InvalidOperationException(
                    $"Неможливо помножити: кількість стовпців A ({a.Cols}) " +
                    $"не дорівнює кількості рядків B ({b.Rows}).");

            // Результуюча матриця має кількість рядків від першої і кількість стовпців від другої.
            var result = new MatrixModel(a.Rows, b.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < b.Cols; j++)
                    for (int k = 0; k < a.Cols; k++)
                        result[i, j] += a[i, k] * b[k, j];
            return result;
        }

        // Перевантаження операторів, щоб у коді інтерфейсу можна було зручно писати: a + b, a - b, a * b.
        public static MatrixModel operator +(MatrixModel a, MatrixModel b) => Add(a, b);
        public static MatrixModel operator -(MatrixModel a, MatrixModel b) => Subtract(a, b);
        public static MatrixModel operator *(MatrixModel a, MatrixModel b) => Multiply(a, b);

        // Допоміжна властивість для швидкого виводу розміру (наприклад, "2x3") у повідомлення та UI.
        public string SizeString => $"{Rows}×{Cols}";

        // Приватний метод для перевірки, чи існують вказані індекси в масиві (захист від виходу за межі).
        private void ValidateIndex(int row, int col)
        {
            if (row < 0 || row >= Rows)
                throw new IndexOutOfRangeException($"Рядок {row} виходить за межі.");
            if (col < 0 || col >= Cols)
                throw new IndexOutOfRangeException($"Стовпець {col} виходить за межі.");
        }

        // Приватний метод перевірки сумісності матриць для операцій додавання та віднімання.
        private static void CheckSameSize(MatrixModel a, MatrixModel b, string operation)
        {
            if (a.Rows != b.Rows || a.Cols != b.Cols)
                throw new InvalidOperationException(
                    $"Неможливо виконати {operation}: матриці мають різні розміри " +
                    $"({a.SizeString} та {b.SizeString}).");
        }
    }
}