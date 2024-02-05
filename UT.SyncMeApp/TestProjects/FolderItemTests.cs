using SyncMeAppLibrary.BL;
using SyncMeAppLibrary.Model;

namespace UT.SyncMeApp.TestProjects
{
    public class MockFileItem(string fullName, string rootDirPath) : IEquatable<MockFileItem?>, IFileItem
    {
        public string FullName { get; set; } = fullName;
        public string RelativePath { get; set; } = fullName.Replace(rootDirPath, string.Empty);
        public string RootDirPath { get; set; } = rootDirPath;

        public override bool Equals(object? obj)
        {
            return Equals(obj as MockFileItem);
        }

        public bool Equals(MockFileItem? file)
        {
            if (file == null)
                return false;
            return RelativePath == file.RelativePath;
        }

        public override int GetHashCode()
        {
            return RelativePath.GetHashCode();
        }
    }

    [TestClass]
    public class FolderItemTests
    {
        private const string sourcePath = "d:\\TestSource\\";
        private const string replicaPath = "d:\\TestSource\\";

        private IFileItem[] emptyArray = [];
        private IFileItem[] array1 = { new MockFileItem(sourcePath + "test.txt", sourcePath), new MockFileItem(sourcePath + "folder1\\" + "test.pdf", sourcePath) };
        private IFileItem[] array2 = { new MockFileItem(sourcePath + "test.txt", sourcePath), new MockFileItem(sourcePath + "folder2\\" + "test.pdf", sourcePath) };

        [TestMethod]
        public void FindDifferentFilesTest_SameArrays()
        {
            var result = SyncLogic.FindDifferentFiles(array1, array1);
            Assert.AreEqual(emptyArray.Length, result.Length);
        }

        [TestMethod]
        public void FindDifferentFilesTest_DifferentArrays()
        {
            var result1 = SyncLogic.FindDifferentFiles(array1, array2);
            var result2 = SyncLogic.FindDifferentFiles(array2, array1);
            Assert.IsTrue(result1.Length == 1);
            Assert.IsTrue(result2.Length == 1);
            Assert.AreEqual(array1[1], result1[0]);
            Assert.AreEqual(array2[1], result2[0]);
        }
    }
}
