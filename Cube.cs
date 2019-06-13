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
        public void FillCubeRandom() {
            var random = new Random();
            for (int i = 0; i < _mass.Length; i++) {
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
        /// Проверка связанности ячейки в трех направлениях
        /// </summary>
        /// <param name="cellIndex">индекс ячейки массива (начинается с 0)</param>
        /// <param name="xIndex">индекс связанной ячейки по оси x (или 0)</param>
        /// <param name="yIndex">индекс связанной ячейки по оси y (или 0)</param>
        /// <param name="zIndex">индекс связанной ячейки по оси z (или 0)</param>
        /// <returns>ячейка связанна с другими ячейками по одному из направлений</returns>
        private bool CellHasIntersect(int cellIndex, ref int xIndex,ref int yIndex,ref int zIndex) {
            if (cellIndex + 1 <= _mass.Length) {
                var result = CheckCellByX(cellIndex, ref xIndex);
                result = CheckCellByY(cellIndex, ref yIndex) || result;
                result = CheckCellByZ(cellIndex, ref zIndex) || result;
                return result;
            }
            return false;
        }

        /// <summary>
        /// Проверка связанности ячейки направлении x
        /// </summary>
        /// <param name="index">индекс ячейки массива (начинается с 0)</param>
        /// <param name="equalsIndex">индекс связанной ячейки по оси x (или 0)</param>
        private bool CheckCellByX(int index, ref int equalsIndex) {
            var nextIndex = index + 1;
            if (nextIndex % _x > 0) {
                var value = _mass[index];
                var result = value == _mass[nextIndex];
                if (result)
                    equalsIndex = nextIndex;
                return result;
            }
            return false;
        }

        /// <summary>
        /// Проверка связанности ячейки направлении y
        /// </summary>
        /// <param name="index">индекс ячейки массива (начинается с 0)</param>
        /// <param name="equalsIndex">индекс связанной ячейки по оси y (или 0)</param>
        private bool CheckCellByY(int index, ref int equalsIndex) {
            var nextIndex = index + 1;
            if (nextIndex % (_xy) > 0 && nextIndex < _mass.Length - _x) {
                var value = _mass[index];
                equalsIndex = index + _x;
                var result = value == _mass[equalsIndex];
                if (!result)
                    equalsIndex = 0;
                return result;
            }
            return false;
        }

        /// <summary>
        /// Проверка связанности ячейки направлении z
        /// </summary>
        /// <param name="index">индекс ячейки массива (начинается с 0)</param>
        /// <param name="equalsIndex">индекс связанной ячейки по оси z (или 0)</param>
        private bool CheckCellByZ(int index, ref int equalsIndex) {
            if (index < _xyz) {
                var value = _mass[index];
                equalsIndex = index + _xy;
                var result = value == _mass[equalsIndex];
                if (!result)
                    equalsIndex = 0;
                return result;
            }
            return false;
        }

        /// <summary>
        /// Получение списка связанных областей в виде словаря [номер области, номера ячеек в области]
        /// </summary>
        public Dictionary<int, HashSet<int>> GetCubeIntersection() {
            var result = new Dictionary<int,HashSet<int>>();
            var groupList = FindCubeIntersection(null, result);
            return result;
        }

        /// <summary>
        /// Получение количества связанных областей и вывод списка связанных областей в файл
        /// </summary>
        /// <param name="file">Файл для записи</param>
        /// <returns>количество связанных областей</returns>
        public int GetCubeIntersection(StreamWriter file) {
            var result = FindCubeIntersection(file);
            return result;
        }

        /// <summary>
        /// Поиск связанных областей в кубе и запись результата в файл или в коллекцию  
        /// </summary>
        /// <param name="file">файл</param>
        /// <param name="collection">коллекция</param>
        /// <returns>количество связанных областей</returns>
        private int FindCubeIntersection(StreamWriter file = null, Dictionary<int, HashSet<int>> collection = null) {
            var checkedList = new HashSet<int>();
            var groupCount = 0;
            for (int i = 0; i < _mass.Length; i++) {
                var res = new HashSet<int>();
                if (CheckCubeIntersection(i, checkedList, res)) {
                    if (collection != null)
                        collection.Add(groupCount, res);
                    if (file != null)
                        file.Write("Область №" + (groupCount + 1) + ": " + string.Join(", ", res));
                    groupCount++;
                }
            }
            return groupCount;
        }

        /// <summary>
        /// Рекурсивная проверка ячеек на связанность по трем направлениям
        /// </summary>
        /// <param name="index">индекс ячейки (начинается с 0)</param>
        /// <param name="checkedList">список уже проверенных ячеек</param>
        /// <param name="result">список связанных ячеек в текущей области</param>
        /// <returns>ячейка связанна с хотя бы одной другой ячейкой</returns>
        private bool CheckCubeIntersection(int index, HashSet<int> checkedList, HashSet<int> result) {
            if (_mass[index] == 1 && !checkedList.Contains(index)) {
                checkedList.Add(index);
                result.Add(index);
                int xIntersect = 0, yIntersect = 0, zIntersect = 0;
                if (CellHasIntersect(index, ref xIntersect, ref yIntersect, ref zIntersect)) {
                    if (xIntersect > 0)
                        CheckCubeIntersection(xIntersect, checkedList, result);
                    if (yIntersect > 0)
                        CheckCubeIntersection(yIntersect, checkedList, result);
                    if (zIntersect > 0)
                        CheckCubeIntersection(zIntersect, checkedList, result);
                    return true;
                }
            }
            return false;
        }
    }
}
