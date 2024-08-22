namespace FileReplicator
{
    using System;

    public class Logger
    {
        private readonly string _logFilePath;

        public Logger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }
        public void Log(string message)
        {
            var logMessage = $"{DateTime.Now}: {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}
