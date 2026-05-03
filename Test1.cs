using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Matrix; // Підключаємо основний простір імен

namespace Matrix.Tests
{
    [TestClass] // Вказує, що це клас із тестами
    public class MatrixModelTests
    {
        // 1. ТЕСТУВАННЯ ДОДАВАННЯ (Правильний сценарій)
        [TestMethod]
        public void Add_ValidMatrices_ReturnsCorrectSum()
        {
            // Arrange (Підготовка даних)
            var m1 = new MatrixModel(new double[,] { { 1, 2 }, { 3, 4 } });
            var m2 = new MatrixModel(new double[,] { { 5, 6 }, { 7, 8 } });

            // Act (Виконання дії)
            var result = m1 + m2; // Використовуємо перевантаження оператора

            // Assert (Перевірка результату)
            Assert.AreEqual(6, result[0, 0]);
            Assert.AreEqual(8, result[0, 1]);
            Assert.AreEqual(10, result[1, 0]);
            Assert.AreEqual(12, result[1, 1]);
        }

        // 2. ТЕСТУВАННЯ ДОДАВАННЯ З ПОМИЛКОЮ (Різні розміри)
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))] // Очікуємо, що метод викине саме цю помилку
        public void Add_DifferentSizes_ThrowsInvalidOperationException()
        {
            var m1 = new MatrixModel(2, 2);
            var m2 = new MatrixModel(3, 3);

            // Ця дія має викликати помилку, і тест буде вважатися пройденим
            var result = m1 + m2;
        }

        // 3. ТЕСТУВАННЯ МНОЖЕННЯ (Правильний сценарій)
        [TestMethod]
        public void Multiply_ValidMatrices_ReturnsCorrectProduct()
        {
            // Arrange
            var m1 = new MatrixModel(new double[,] { { 1, 2 }, { 3, 4 } }); // 2x2
            var m2 = new MatrixModel(new double[,] { { 2, 0 }, { 1, 2 } }); // 2x2

            // Act
            var result = m1 * m2;

            // Assert
            Assert.AreEqual(4, result[0, 0]);  // 1*2 + 2*1
            Assert.AreEqual(4, result[0, 1]);  // 1*0 + 2*2
            Assert.AreEqual(10, result[1, 0]); // 3*2 + 4*1
            Assert.AreEqual(8, result[1, 1]);  // 3*0 + 4*2
        }

        // 4. ТЕСТУВАННЯ МНОЖЕННЯ З ПОМИЛКОЮ (Несумісні розміри)
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Multiply_InvalidSizes_ThrowsInvalidOperationException()
        {
            var m1 = new MatrixModel(2, 3); // Стовпців 3
            var m2 = new MatrixModel(4, 2); // Рядків 4 (3 != 4, множити не можна)

            var result = m1 * m2;
        }

        // 5. ТЕСТУВАННЯ КОНСТРУКТОРА (Некоректний розмір)
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_ZeroSize_ThrowsArgumentException()
        {
            var m = new MatrixModel(0, 2); // Рядків не може бути 0
        }
    }
}