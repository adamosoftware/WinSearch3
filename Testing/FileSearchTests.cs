using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WinSearch3.Library;

namespace Testing
{
    [TestClass]
    public class FileSearchTests
    {
        [TestMethod]
        public void FileSearch()
        {
            var fs = new FileSearch()
            {
                Locations = @"C:\Users\Adam\SkyDrive, C:\Users\Adam",
                SearchFilename = "rel"
            };

            var results = fs.ExecuteAsync().Result;
            Assert.IsTrue(results.All(s => s.Contains(fs.SearchFilename)));
        }

        [TestMethod]
        public void FolderSearch()
        {
            var fs = new FileSearch()
            {
                Locations = @"C:\Users\Adam\SkyDrive, C:\Users\Adam",
                SearchFilename = "source/"
            };

            var results = fs.ExecuteAsync().Result;
            Assert.IsTrue(results.All(s => s.Contains("source")));
        }
    }
}
