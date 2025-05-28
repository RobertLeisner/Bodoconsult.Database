// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Test.Utilities.Helpers
{
    /// <summary>
    /// Helper class for file handling
    /// </summary>
    public static class FileHelper
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        static FileHelper()
        {
            BasePath =  Path.Combine(Path.GetTempPath(), "BodoconsultDatabase");

            CreateFolder(BasePath);

            DataPath = Path.Combine(BasePath, "SqlData");

            CreateFolder(DataPath);

            // Remove old backups
            BackupPath = Path.Combine(BasePath, "Backups");
            CreateFolder(BackupPath, "*.bak");

            // Remove old entity backups
            EntityBackupPath = Path.Combine(BasePath, "EntityBackups");
            CreateFolder(BackupPath, "*.bak");
        }

        /// <summary>
        /// Create a folder
        /// </summary>
        /// <param name="path">Full folder path</param>
        /// <param name="pattern">Clean the folder from existing files met by the pattern</param>
        private static DirectoryInfo CreateFolder(string path, string pattern)
        {
            var dir = CreateFolder(path);

            foreach (var file in dir.GetFiles("*.bak"))
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    // Do nothing
                    throw;
                }
            }

            return dir;
        }

        /// <summary>
        /// Create a folder
        /// </summary>
        /// <param name="path">Full folder path</param>
        public static DirectoryInfo CreateFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                var dir = !Directory.Exists(path) ? Directory.CreateDirectory(path) : new DirectoryInfo(path);

                return dir;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Base data path for test environment
        /// </summary>
        public static string BasePath { get; set; }


        private static string _appPath;

        private static string _tempPath;

        private static string _outputPath;


        /// <summary>
        /// Get path of current application
        /// </summary>
        /// <returns>current application path</returns>
        public static string GetAppPath()
        {
            if (!string.IsNullOrEmpty(_appPath))
            {
                return _appPath;
            }

            var s = typeof(FileHelper).Assembly.Location;

            s = new FileInfo(s).DirectoryName;

            _appPath = s;
            return _appPath;
        }
        
        /// <summary>
        /// Get the current temporary path for test data
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            if (!string.IsNullOrEmpty(_tempPath))
            {
                return _tempPath;
            }

#pragma warning disable CA1031
            try
            {

                _tempPath = Path.Combine(BasePath, "Temp");

                CreateFolder(_tempPath);

                return _tempPath;
            }
            catch
            {
                _tempPath = null;
                return _tempPath;
            }
#pragma warning restore CA1031
        }



        /// <summary>
        /// Get the current temporary path for test data
        /// </summary>
        /// <returns></returns>
        public static string GetOutputPath()
        {
            if (!string.IsNullOrEmpty(_outputPath))
            {
                return _outputPath;
            }

#pragma warning disable CA1031
            try
            {

                _outputPath = Path.Combine(BasePath, "Output");

                if (!Directory.Exists(_outputPath))
                {
                    Directory.CreateDirectory(_outputPath);
                }

                return _outputPath;
            }
            catch
            {
                _outputPath = null;
                return _outputPath;
            }
#pragma warning restore CA1031
        }



        /// <summary>
        /// Get the current backup path for test databases
        /// </summary>
        /// <returns></returns>
        public static string BackupPath { get; }

        /// <summary>
        /// Get the current entitybackup path for test databases
        /// </summary>
        /// <returns></returns>
        public static string EntityBackupPath { get; }


        /// <summary>
        /// Get the current path for saving database files in
        /// </summary>
        /// <returns></returns>
        public static string DataPath { get; }
    }


}

