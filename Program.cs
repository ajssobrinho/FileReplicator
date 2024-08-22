namespace FileReplicator
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to file synchronizer!");

            if (args.Length < 4)
            {
                Console.WriteLine("Usage: FolderSynchronizer.exe <sourceFolder> <replicaFolder> <intervalInSeconds> <logFilePath>");
                return;
            }

            var sourceFolder = args[0];
            var replicaFolder = args[1];

            if (!int.TryParse(args[2], out int interval))
            {
                Console.WriteLine("Invalid interval. Please provide a valid number for the interval in seconds.");
                return;
            }

            var logFilePath = args[3];

            var synchronizer = new FileSynchronizer(sourceFolder, replicaFolder, interval * 1000, logFilePath);
            synchronizer.StartSynchronization();
        }
    }
}