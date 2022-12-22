using Forge.Security;
using System.Security.Cryptography.X509Certificates;

namespace Testing.Security
{

    [TestClass]
    public class CertificateCreationTest
    {

        [TestMethod]
        public void TestCertificationCreation()
        {
            string subjectName = "CN=subjectName";
            string password = "Passw0rd";

            byte[] data = CertificateFactory.CreateSelfSignCertificatePfx(subjectName, DateTime.UtcNow.AddDays(-1), DateTime.MaxValue, password);
            X509Certificate2 cert = new X509Certificate2(data, password);
            Assert.IsTrue(subjectName.Equals(cert.SubjectName.Name));
        }

    }

}