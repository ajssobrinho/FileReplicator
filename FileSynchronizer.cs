namespace FileReplicator
{
    using System;
    using System.Linq;

    public class FileSynchronizer
    {
        private readonly string _sourceFolder;
        private readonly string _replicaFolder;
        private readonly int _syncIntervalInSeconds;
        private readonly Logger _logger;

        public FileSynchronizer(string sourceFolder, string replicaFolder, int syncIntervalInSeconds, string logFilePath)
        {
            _sourceFolder = sourceFolder;
            _replicaFolder = replicaFolder;
            _syncIntervalInSeconds = syncIntervalInSeconds;
            _logger = new Logger(logFilePath);
        }

        public void StartSynchronization()
        {
            while (true)
            {
                try
                {
                    SynchronizeFolders();
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error during synchronization: {ex.Message}");
                }

                Thread.Sleep(_syncIntervalInSeconds);
            }
        }

        private void SynchronizeFolders()
        {
            _logger.Log("Synchronization Started");

            if (!Directory.Exists(_sourceFolder))
            {
                _logger.Log($"Source folder '{_sourceFolder}' does not exist.");
                return;
            }

            if (!Directory.Exists(_replicaFolder))
            {
                Directory.CreateDirectory(_replicaFolder);
                _logger.Log($"Created replica folder '{_replicaFolder}'.");
            }

            CopyFiles();

            RemoveNonExistingFiles();

            RemoveEmptyDirectories();

            _logger.Log("Synchronization complete.");
        }

        private void RemoveEmptyDirectories()
        {
            foreach (var replicaDir in Directory.GetDirectories(_replicaFolder, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
            {
                if (!Directory.EnumerateFileSystemEntries(replicaDir).Any())
                {
                    Directory.Delete(replicaDir);
                    _logger.Log($"Deleted empty directory: {replicaDir}");
                }
            }
        }

        private void RemoveNonExistingFiles()
        {
            foreach (var replicaFilePath in Directory.GetFiles(_replicaFolder, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(_replicaFolder, replicaFilePath);
                var sourceFilePath = Path.Combine(_sourceFolder, relativePath);

                if (!File.Exists(sourceFilePath))
                {
                    File.Delete(replicaFilePath);
                    _logger.Log($"Deleted file: {replicaFilePath}");
                }
            }
        }

        private void CopyFiles()
        {
            foreach (var sourceFilePath in Directory.GetFiles(_sourceFolder, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(_sourceFolder, sourceFilePath);
                var replicaFilePath = Path.Combine(_replicaFolder, relativePath);

                if (!File.Exists(replicaFilePath) || File.GetLastWriteTimeUtc(sourceFilePath) > File.GetLastWriteTimeUtc(replicaFilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(replicaFilePath) ?? string.Empty);
                    File.Copy(sourceFilePath, replicaFilePath, true);
                    _logger.Log($"Copied file: {sourceFilePath} to {replicaFilePath}");
                }
            }
        }
    }
}
