namespace TestProject
{
    /// <summary>
    /// The properties of the directory
    /// </summary>
    public class DirectoryInfo
    {
        // The name of file
        public string? Name { get; set; }

        // Full path of the folder of file within the directory
        public string? Path { get; set; }

        // Folder or File
        public string? Type { get; set; }

        // Size in KB of the file
        public int? Size { get; set; } = 0;
    }
}
