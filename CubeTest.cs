using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeIntersection {
    [TestClass]
    public class CubeTest {

        /// <summary>
        /// Проверка правильности поиска связанных областей
        /// </summary>
        [TestMethod]
        public void TestIntersecion() {
            var testArray = new int[] { 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 0, 0 };
            var cube = new Cube(3, 3, 3);
            cube.FillCube(testArray);
            var except = new Dictionary<int, HashSet<int>>();
            except.Add(0, new HashSet<int>() { 0, 1, 4, 10, 11, 20 });
            except.Add(1, new HashSet<int>() { 16, 17 });
            except.Add(2, new HashSet<int>() { 21, 22, 24 });
            var result = cube.GetCubeIntersection();
            Assert.AreEqual(except.Count, result.Count);
            for (int i = 0; i < except.Count; i++) {
                CollectionAssert.AreEqual(except[i].ToList(), result[i].OrderBy(t => t).ToList());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestIntersecion2() {
            var testArray = new int[] { 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var cube = new Cube(4, 4, 2);
            cube.FillCube(testArray);
            var except = new Dictionary<int, HashSet<int>>();
            except.Add(0, new HashSet<int>() { 0, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 23, 24, 25, 26, 27, 28, 29, 30, 31 });
            var result = cube.GetCubeIntersection();
            Assert.AreEqual(except.Count, result.Count);
            for (int i = 0; i < except.Count; i++) {
                CollectionAssert.AreEqual(except[i].ToList(), result[i].OrderBy(t => t).ToList());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestIntersecion3() {
            var testArray = new int[] { 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 1 };
            var cube = new Cube(5, 6, 1);
            cube.FillCube(testArray);
            var except = new Dictionary<int, HashSet<int>>();
            except.Add(0, new HashSet<int>() { 0, 2, 4, 5, 7, 8, 9, 10, 13, 15, 17, 18, 19, 20, 21, 22, 25, 27, 28, 29 });
            var result = cube.GetCubeIntersection();
            Assert.AreEqual(except.Count, result.Count);
            for (int i = 0; i < except.Count; i++) {
                CollectionAssert.AreEqual(except[i].ToList(), result[i].OrderBy(t => t).ToList());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestIntersecion4() {
            var testArray = new int[] { 1, 1, 1, 1, 1,1,1,1,1 };
            var cube = new Cube(3, 3, 1);
            cube.FillCube(testArray);
            var except = new Dictionary<int, HashSet<int>>();
            except.Add(0, new HashSet<int>() { 0, 1,2,3,4,5,6,7,8 });
            var result = cube.GetCubeIntersection();
            Assert.AreEqual(except.Count, result.Count);
            for (int i = 0; i < except.Count; i++) {
                CollectionAssert.AreEqual(except[i].ToList(), result[i].OrderBy(t => t).ToList());
            }
        }
    }
}
