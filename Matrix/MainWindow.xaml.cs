using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Matrix
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!TryLoadFromFile())
            {
                //  Якщо файлу немає — показуємо порожні матриці 2x2 за замовчуванням
                ApplySizeA(2, 2);
                ApplySizeB(2, 2);
                TxtLoadStatus.Text = "Новий сеанс. Введіть дані у матриці.";
            }
        }

        // ── Автоматичне збереження при закритті вікна ─────────────────
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Бажаєте зберегти матриці перед виходом?",
                "Збереження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                TrySaveToFile();
        }
        // ── Розмір матриці А ──────────────────────────────────────────
        private void BtnApplyA_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseSize(TxtARows.Text, TxtACols.Text,
                    "Матриця A", out int rows, out int cols)) return;
            ApplySizeA(rows, cols);
            HideError();
        }

        private void ApplySizeA(int rows, int cols)
        {
            GridA.ItemsSource = null;
            GridA.Columns.Clear();
            GridA.ItemsSource = MatrixGridHelper
                .CreateEmptyTable(rows, cols).DefaultView;
            TxtARows.Text = rows.ToString();
            TxtACols.Text = cols.ToString();
        }

        // ── Розмір матриці B ──────────────────────────────────────────
        private void BtnApplyB_Click(object sender, RoutedEventArgs e)
        {
            if (!TryParseSize(TxtBRows.Text, TxtBCols.Text,
                    "Матриця B", out int rows, out int cols)) return;
            ApplySizeB(rows, cols);
            HideError();
        }

        private void ApplySizeB(int rows, int cols)
        {
            GridB.ItemsSource = null;
            GridB.Columns.Clear();
            GridB.ItemsSource = MatrixGridHelper
                .CreateEmptyTable(rows, cols).DefaultView;
            TxtBRows.Text = rows.ToString();
            TxtBCols.Text = cols.ToString();
        }

        // ── Операції ──────────────────────────────────────────────────

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
            => PerformOperation("A + B", (a, b) => a + b);

        private void BtnSubtract_Click(object sender, RoutedEventArgs e)
            => PerformOperation("A − B", (a, b) => a - b);

        private void BtnMultiply_Click(object sender, RoutedEventArgs e)
            => PerformOperation("A × B", (a, b) => a * b);

        private void PerformOperation(string name,
            Func<MatrixModel, MatrixModel, MatrixModel> operation)
        {
            HideError();
            try
            {
                MatrixModel a = ReadMatrixFromGrid(GridA, "Матриця A");
                MatrixModel b = ReadMatrixFromGrid(GridB, "Матриця B");
                MatrixModel result = operation(a, b);
                ShowResult(result, name, a, b);
            }
            catch (FormatException ex) { ShowError($"Помилка введення: {ex.Message}"); }
            catch (InvalidOperationException ex) { ShowError($"Помилка: {ex.Message}"); }
            catch (Exception ex) { ShowError($"Несподівана помилка: {ex.Message}"); }
        }

        // ── Збереження / завантаження (кнопки) ───────────────────────

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (TrySaveToFile())
                TxtLoadStatus.Text = "✔ Матриці збережено у файл matrices.txt";
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (TryLoadFromFile())
                TxtLoadStatus.Text = "✔ Матриці завантажено з файлу";
            else
                TxtLoadStatus.Text = "Файл не знайдено або пошкоджений.";
        }

        // ── Збереження / завантаження (логіка) ───────────────────────

        // Зберігає поточні матриці у файл. Повертає true якщо успішно.
        private bool TrySaveToFile()
        {
            try
            {
                MatrixModel a = ReadMatrixFromGrid(GridA, "Матриця A");
                MatrixModel b = ReadMatrixFromGrid(GridB, "Матриця B");
                MatrixFileService.Save(a, b);
                return true;
            }
            catch
            {
                return false; // при автозбереженні не показуємо помилку
            }
        }

        //Завантажує матриці з файлу. Повертає true якщо успішно.
        private bool TryLoadFromFile()
        {
            if (!MatrixFileService.TryLoad(out MatrixModel a, out MatrixModel b))
                return false;

            // Відображаємо матрицю A
            GridA.ItemsSource = null;
            GridA.Columns.Clear();
            GridA.ItemsSource = MatrixGridHelper.MatrixToTable(a).DefaultView;
            TxtARows.Text = a.Rows.ToString();
            TxtACols.Text = a.Cols.ToString();

            // Відображаємо матрицю B
            GridB.ItemsSource = null;
            GridB.Columns.Clear();
            GridB.ItemsSource = MatrixGridHelper.MatrixToTable(b).DefaultView;
            TxtBRows.Text = b.Rows.ToString();
            TxtBCols.Text = b.Cols.ToString();

            TxtLoadStatus.Text = "Дані завантажено з попереднього сеансу";
            return true;
        }

        // ── Допоміжні методи ──────────────────────────────────────────

        private static MatrixModel ReadMatrixFromGrid(DataGrid grid, string name)
        {
            if (grid.ItemsSource is DataView view)
                return MatrixGridHelper.TableToMatrix(view.Table, name);
            throw new InvalidOperationException($"{name}: немає даних.");
        }

        private void ShowResult(MatrixModel result, string opName,
            MatrixModel a, MatrixModel b)
        {
            TxtResultInfo.Text =
                $"Операція: {opName}  |  " +
                $"A: {a.SizeString}  B: {b.SizeString}  →  {result.SizeString}";
            GridResult.ItemsSource = null;
            GridResult.Columns.Clear();
            GridResult.ItemsSource = MatrixGridHelper.MatrixToTable(result).DefaultView;
        }

        private void ShowError(string message)
        {
            TxtError.Text = message;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
        }

        private bool TryParseSize(string rowsText, string colsText,
            string name, out int rows, out int cols)
        {
            rows = 0; cols = 0;
            if (!int.TryParse(rowsText, out rows) || rows < 1 || rows > 10)
            {
                ShowError($"{name}: рядки повинні бути від 1 до 10.");
                return false;
            }
            if (!int.TryParse(colsText, out cols) || cols < 1 || cols > 10)
            {
                ShowError($"{name}: стовпці повинні бути від 1 до 10.");
                return false;
            }
            return true;
        }
    }
}