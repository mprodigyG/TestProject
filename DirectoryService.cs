using System.IO;

namespace TestProject
{
    public class DirectoryService
    {
        public IEnumerable<DirectoryInfo> GetDirectoryInfo(string directoryPath)
        {
            List<DirectoryInfo> directoryInfo = new List<DirectoryInfo>();
            string[] directories = Directory.GetDirectories(directoryPath);
            string[] files = Directory.GetFiles(directoryPath);

            foreach (string directory in directories)
            {
                directoryInfo.Add(new DirectoryInfo { Name = Path.GetFileName(directory), Type = "Folder", Size = 0, Path = Path.GetFullPath(directory) });
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(Path.GetFullPath(file));
                long fileSizeBytes = fileInfo.Length;
                double fileSizeKB = Math.Ceiling((double)fileSizeBytes / 1024);
                directoryInfo.Add(new DirectoryInfo { Name = Path.GetFileName(file), Type = "File", Size = fileSizeKB, Path = Path.GetFullPath(file) });
            }
            return directoryInfo;
        }

        public IEnumerable<DirectoryInfo> GetFilterDirectoryInfo(string directoryPath, string filterValue)
        {
            List<DirectoryInfo> directoryInfo = new List<DirectoryInfo>();
            string[] directories = Directory.GetDirectories(directoryPath, "*" + filterValue.Trim() + "*", SearchOption.AllDirectories);
            string[] files = Directory.GetFiles(directoryPath, "*" + filterValue.Trim() + "*", SearchOption.AllDirectories);

            foreach (string directory in directories)
            {
                directoryInfo.Add(new DirectoryInfo { Name = Path.GetFileName(directory), Type = "Folder", Size = 0, Path = Path.GetFullPath(directory) });
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(Path.GetFullPath(file));
                long fileSizeBytes = fileInfo.Length;
                double fileSizeKB = Math.Ceiling((double)fileSizeBytes / 1024);
                directoryInfo.Add(new DirectoryInfo { Name = Path.GetFileName(file), Type = "File", Size = fileSizeKB, Path = Path.GetFullPath(file) });
            }
            return directoryInfo;
        }

        public bool CheckDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the total files count base on all the files from the top directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public int GetFileCount(string directoryPath, string filterValue = "*.*")
        {
            int fileCount = Directory.GetFiles(directoryPath, filterValue, SearchOption.AllDirectories).Length;
            return fileCount;
        }

        /// <summary>
        /// Get the tolal folders counts base on all the folder from the top directories
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public int GetFolderCount(string directoryPath, string filterValue = "*")
        {
            int folderCount = Directory.GetDirectories(directoryPath, filterValue, SearchOption.AllDirectories).Length;
            return folderCount;
        }

        public List<string> GetDirectoryFolders(string directoryPath)
        {
            List<string> directoryFolders = new List<string>();
            directoryFolders = Directory.GetDirectories(directoryPath, "*", SearchOption.AllDirectories).ToList();
            return directoryFolders;
        }
    }
}
