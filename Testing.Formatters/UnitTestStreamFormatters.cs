using Forge.Formatters;

namespace Testing.Formatters
{

    [TestClass]
    public class UnitTestStreamFormatters
    {

        [TestMethod]
        public void TestMethodBrotliCompressor()
        {
            BrotliFormatter formatter = new BrotliFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream("Wizard of Wor.d64", FileMode.Open, FileAccess.Read))
                {
                    formatter.Write(ms, fs);
                    fs.Position = 0;
                    ms.Position= 0;
                    byte[] decompressed = formatter.Read(ms);
                    Assert.AreEqual(decompressed.Length, fs.Length);
                    for (long i = 0; i < decompressed.Length; i++)
                    {
                        Assert.AreEqual(fs.ReadByte(), decompressed[i]);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethodGZipCompressor()
        {
            GZipFormatter formatter = new GZipFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream fs = new FileStream("Wizard of Wor.d64", FileMode.Open, FileAccess.Read))
                {
                    formatter.Write(ms, fs);
                    fs.Position = 0;
                    ms.Position = 0;
                    byte[] decompressed = formatter.Read(ms);
                    Assert.AreEqual(decompressed.Length, fs.Length);
                    for (long i = 0; i < decompressed.Length; i++)
                    {
                        Assert.AreEqual(fs.ReadByte(), decompressed[i]);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethodAesByteArrayCompressor()
        {
            AesByteArrayFormatter formatter = new AesByteArrayFormatter();
            using (MemoryStream encryptedStream = new MemoryStream())
            {
                using (FileStream fs = new FileStream("Wizard of Wor.d64", FileMode.Open, FileAccess.Read))
                {
                    formatter.Write(encryptedStream, fs);
                    fs.Position = 0;
                    encryptedStream.Position = 0;
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        formatter.Read(encryptedStream, decryptedStream);
                        decryptedStream.Position = 0;
                        Assert.AreEqual(fs.Length, decryptedStream.Length);
                        for (long i = 0; i < fs.Length; i++)
                        {
                            Assert.AreEqual(fs.ReadByte(), decryptedStream.ReadByte());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethodRijndaelByteArrayCompressor()
        {
            RijndaelByteArrayFormatter formatter = new RijndaelByteArrayFormatter();
            using (MemoryStream encryptedStream = new MemoryStream())
            {
                using (FileStream fs = new FileStream("Wizard of Wor.d64", FileMode.Open, FileAccess.Read))
                {
                    formatter.Write(encryptedStream, fs);
                    fs.Position = 0;
                    encryptedStream.Position = 0;
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        formatter.Read(encryptedStream, decryptedStream);
                        decryptedStream.Position = 0;
                        Assert.AreEqual(fs.Length, decryptedStream.Length);
                        for (long i = 0; i < fs.Length; i++)
                        {
                            Assert.AreEqual(fs.ReadByte(), decryptedStream.ReadByte());
                        }
                    }
                }
            }
        }

    }

}