using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace CubeIntersection {

    /// <summary>
    /// Класс для поиска связанных областей в трехмерном кубе
    /// </summary>
    public class Cube {

        private int[] _mass;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
        private readonly int _xy;
        private readonly int _xyz;
        /// <summary>
        /// Произвольное число, для генерации более рандомных значений
        /// </summary>
        const int RANDOM_NUM = 100000;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="x">величина куба по оси x</param>
        /// <param name="y">величина куба по оси y</param>
        /// <param name="z">величина куба по оси z</param>
        public Cube(int x, int y, int z) {
            _x = x;
            _y = y;
            _z = z;
            _xy = x * y;
            _xyz = x * y * (_z - 1);
            _mass = new int[x * y * z];
        }

        /// <summary>
        /// Заполнение куба рандомными значениями
        /// </summary>
        public void FillCubeRandom(BackgroundWorker worker) {
            var random = new Random();
            for (int i = 0; i < _mass.Length; i++) {
                if (worker.CancellationPending)
                    break;
                var number = random.Next(0, RANDOM_NUM);
                _mass[i] = number < RANDOM_NUM / 2 ? 0 : 1;
            }
        }

        /// <summary>
        /// Заполнение куба заданным массивом значений
        /// </summary>
        /// <param name="mass">массив значений</param>
        public void FillCube(int[] mass) {
            if (mass.Length != _mass.Length)
                throw new Exception("Not correct array size");
            _mass = mass;
        }

        /// <summary>
        /// Получение списка связанных областей в виде словаря [номер области, номера ячеек в области]
        /// </summary>
        public Dictionary<int, HashSet<int>> GetCubeIntersection() {
            var result = new Dictionary<int, HashSet<int>>();
            GetCubeIntersection(null, result);
            return result;
        }

        /// <summary>
        /// Получение количества связанных областей и вывод списка связанных областей в файл
        /// </summary>
        /// <param name="file">Файл для записи</param>
        /// <returns>количество связанных областей</returns>
        public int WriteCubeIntersectionToFile(StreamWriter file) {
            var count = GetCubeIntersection(file, null);
            return count;
        }

        public int GetCubeIntersection(StreamWriter file = null, Dictionary<int, HashSet<int>> collection = null) {
            int groupCount = 0;
            var checkedList = new HashSet<int>();
            for (int i = 0; i < _mass.Length; i++) {
                if (_mass[i] == 1 && !checkedList.Contains(i)) {
                    var result = new HashSet<int>();
                    GetCellIntersercion(i, checkedList, result);
                    if (result.Count > 1) {
                        if (collection != null)
                            collection.Add(groupCount, result);
                        if (file != null)
                            file.Write("Область №" + (groupCount + 1) + ": " + string.Join(", ", result));
                        groupCount++;
                    }
                }
            }
            return groupCount;
        }

        private void GetCellIntersercion(int cellIndex, HashSet<int> checkedList, HashSet<int> result) {
            var notChecked = new Queue<int>();
            notChecked.Enqueue(cellIndex);
            do {
                cellIndex = notChecked.Dequeue();
                if (!checkedList.Contains(cellIndex)) {
                    result.Add(cellIndex);
                    checkedList.Add(cellIndex);
                    AddToQueue(GetPrevCellIndexByX(cellIndex), notChecked);
                    AddToQueue(GetPrevCellIndexByY(cellIndex), notChecked);
                    AddToQueue(GetPrevCellIndexByZ(cellIndex), notChecked);
                    AddToQueue(GetNextCellIndexByX(cellIndex), notChecked);
                    AddToQueue(GetNextCellIndexByY(cellIndex), notChecked);
                    AddToQueue(GetNextCellIndexByZ(cellIndex), notChecked);
                }
            } while (notChecked.Count != 0);
        }

        private void AddToQueue(int? index, Queue<int> queue) {
            if (index.HasValue && _mass[index.Value] == 1)
                queue.Enqueue(index.Value);
        }

        #region GetIndex

        private int? GetPrevCellIndexByX(int cellIndex) {
            if (cellIndex % _x > 0)
                return cellIndex - 1;
            return null;
        }

        private int? GetPrevCellIndexByY(int cellIndex) {
            if (cellIndex % _xy >= _x)
                return cellIndex - _x;
            return null;
        }

        private int? GetPrevCellIndexByZ(int cellIndex) {
            if (cellIndex >= _xy)
                return cellIndex - _xy;
            return null;
        }

        private int? GetNextCellIndexByX(int cellIndex) {
            if ((cellIndex + 1) % _x > 0)
                return cellIndex + 1;
            return null;
        }

        private int? GetNextCellIndexByY(int cellIndex) {
            if ((cellIndex / _x + 1) % _y > 0)
                return cellIndex + _x;
            return null;
        }

        private int? GetNextCellIndexByZ(int cellIndex) {
            if (cellIndex < _xyz)
                return cellIndex + _xy;
            return null;
        }

        #endregion

    }
}
