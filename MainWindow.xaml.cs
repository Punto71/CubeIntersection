using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CubeIntersection {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Cube _cube;
        private BackgroundWorker _creationWorker;
        private BackgroundWorker _intersectWorker;

        /// <summary>
        /// Конструктор
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            _creationWorker = new BackgroundWorker();
            _intersectWorker = new BackgroundWorker();
            _intersectWorker.DoWork += RunCheckIntersection;
            _intersectWorker.RunWorkerCompleted += CheckIntersectionComplete;
            _creationWorker.DoWork += RunCubeCreation;
            _creationWorker.RunWorkerCompleted += CubeCreationCompleted;
            RunCubeCreation();
        }

        /// <summary>
        /// Асинхронный запуск поиска связанных областей
        /// </summary>
        void RunCheckIntersection(object sender, DoWorkEventArgs e) {
            var cube = e.Argument as Cube;
            if (cube != null) {
                var timer = DateTime.Now;
                int result = 0;
                using (var writer = new StreamWriter("out.txt")) {
                    result = cube.GetCubeIntersection(writer);
                }
                var time = DateTime.Now - timer;
                e.Result = "Время выполнения: " + time + "\r\nКоличество связанных областей: " + result +
                    "\r\nНомера ячеек находятся в файле out.txt";
            }
        }

        /// <summary>
        /// Завершение поиска связанных областей
        /// </summary>
        void CheckIntersectionComplete(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Result != null) {
                MessageList.Text = e.Result.ToString();
            }
            RunButton.IsEnabled = true;
        }

        /// <summary>
        /// Асинхронная инициализация и заполнение куба
        /// </summary>
        void RunCubeCreation(object sender, DoWorkEventArgs e) {
            var array = e.Argument as int[];
            if (array != null && array.Length == 3) {
                var cube = new Cube(array[0], array[1], array[2]);
                cube.FillCubeRandom();
                e.Result = cube;
            }
        }

        /// <summary>
        /// Завершение инициализации и заполнения куба
        /// </summary>
        void CubeCreationCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var result = e.Result as Cube;
            if (result != null) {
                _cube = result;
            }
            RunButton.IsEnabled = true;
        }

        /// <summary>
        /// Событие нажатия на кнопку запуска поиска связанных областей
        /// </summary>
        private void RunButtonClick(object sender, RoutedEventArgs e) {
            if (_cube != null) {
                MessageList.Text = "Запуск поиска связанных областей...";
                RunButton.IsEnabled = false;
                _intersectWorker.RunWorkerAsync(_cube);
            }
        }

        /// <summary>
        /// Событие изменения настроек куба
        /// </summary>
        private void CubeSettingsChanged(object sender, TextChangedEventArgs e) {
            if (_creationWorker != null) {
                RunCubeCreation();
            }
        }

        /// <summary>
        /// Функция запуска асинхронной инициализации и заполенения куба
        /// </summary>
        private void RunCubeCreation() {
            int x, y, z = 0;
            RunButton.IsEnabled = false;
            if (int.TryParse(NxTextBox.Text, out x) &&
                int.TryParse(NyTextBox.Text, out y) &&
                int.TryParse(NzTextBox.Text, out z)) {
                _creationWorker.RunWorkerAsync(new int[] { x, y, z });
            }
        }
    }
}
