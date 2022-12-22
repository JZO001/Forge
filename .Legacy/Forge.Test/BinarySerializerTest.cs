using Forge.Persistence.Formatters;
using Forge.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.Test
{

    [TestClass]
    public class BinarySerializerTest
    {

        [TestMethod]
        public void TestSerializer()
        {
            ClassificationRootContainer container = new ClassificationRootContainer();
            container.Register(new Classification() { Id = "A", LongName = "long", ShortName = "short" });
            byte[] data = ClassificationRootContainer.Serialize(container);

            ClassificationRootContainer deserializedContainer = ClassificationRootContainer.Deserialize(data);
        }

    }

    [Serializable]
    public class ClassificationRootContainer
    {

        private static readonly BinarySerializerFormatter<ClassificationRootContainer> mFormatter = new BinarySerializerFormatter<ClassificationRootContainer>() { TypeLookupMode = TypeLookupModeEnum.AllowAll };

        private Dictionary<string, Classification> mClassificationDict = new Dictionary<string, Classification>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationRootContainer"/> class.
        /// </summary>
        public ClassificationRootContainer()
        {
            Classifications = new List<Classification>();
        }

        /// <summary>
        /// Serializes the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">container</exception>
        public static byte[] Serialize(ClassificationRootContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            using (MemoryStream ms = new MemoryStream())
            {
                mFormatter.Write(ms, container);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public static ClassificationRootContainer Deserialize(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            ClassificationRootContainer container = null;

            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                container = mFormatter.Read(ms);
                container.BuildIndexes();
            }

            return container;
        }

        /// <summary>
        /// Gets or sets the root items.
        /// </summary>
        /// <value>
        /// The root items.
        /// </value>
        public List<Classification> Classifications { get; set; }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Classification> GetDictionary()
        {
            return mClassificationDict;
        }

        /// <summary>
        /// Registers the specified classification.
        /// </summary>
        /// <param name="classification">The classification.</param>
        public void Register(Classification classification)
        {
            mClassificationDict[classification.Id] = classification;
        }

        /// <summary>
        /// Gets the classification by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Classification GetClassificationById(string id)
        {
            return mClassificationDict[id];
        }

        /// <summary>
        /// Builds the index.
        /// </summary>
        public void BuildIndexes()
        {
        }

    }

    [Serializable]
    public class Classification
    {

        public string Id { get; set; }

        public string ShortName { get; set; }

        public string LongName { get; set; }

    }

}
