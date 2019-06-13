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
            foreach (var values in result) {
                CollectionAssert.AreEqual(except[values.Key].ToList(), values.Value.ToList());
            }
        }
    }
}
