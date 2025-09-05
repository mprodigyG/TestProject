using System.IO;

namespace TestProject
{
    public class DirectoryService
    {
        public IEnumerable<DirectoryInfo> GetDirectoryInfo(string directoryPath)
        {
            List<DirectoryInfo> directoryInfo = new List<DirectoryInfo>();
            string[] files = Directory.GetFiles(directoryPath);
            string[] directories = Directory.GetDirectories(directoryPath);

            return directoryInfo;
        }

        public bool CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            return true;
        }

        public int GetFileCount(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);
            return files.Length;
        }

        public int GetFolderCount(string directoryPath)
        {
            string[] directories = Directory.GetDirectories(directoryPath);
            return directories.Length;
        }
    }
}
