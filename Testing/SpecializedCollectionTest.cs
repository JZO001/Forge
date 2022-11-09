using Forge.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Forge.Testing
{

    [TestClass]
    public class SpecializedCollectionTest
    {

        [TestMethod]
        public void ListSpecializedTest()
        {
            ListSpecialized<string> list = new ListSpecialized<string>();
            list.AddRange(new string[] { "A", "B", "C", "D" });

            ListSpecialized<string> targetList = new ListSpecialized<string>();
            foreach (string s in list)
            {
                Assert.IsFalse(targetList.Contains(s));
                targetList.Add(s);
            }

            IEnumeratorSpecialized<string> testEnum = (IEnumeratorSpecialized<string>)targetList.GetEnumerator();
            while (testEnum.MoveNext())
            {
                string s = testEnum.Current;
                Assert.IsTrue(list.Contains(s));
                testEnum.Remove();
            }

            Assert.IsTrue(targetList.Count == 0);
            list.Reverse();
        }

    }

}
