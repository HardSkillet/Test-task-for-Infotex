using Serilog;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using Newtonsoft.Json;

namespace FirstTask
{
    public class Program
    {
        public static void Main()
        {
            var time = DateTime.Now;
            var timeID = time.Day.ToString() + time.Month.ToString() + time.Year.ToString() + 
                time.Hour.ToString() + time.Minute.ToString() + time.Millisecond.ToString();
            var configurationsPath = Path.Combine(Environment.CurrentDirectory, "appsetings.json");
            var json = File.ReadAllText(configurationsPath);
            var configurations = JsonConvert.DeserializeObject<Models.Configurations>(json);
            
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(String.Format("log{0}.txt", timeID),
                    rollingInterval: RollingInterval.Year,
                    rollOnFileSizeLimit: true);
            if (configurations.Log == "Debug") loggerConfiguration.MinimumLevel.Debug();
            if (configurations.Log == "Error") loggerConfiguration.MinimumLevel.Error();
            if (configurations.Log == "Information") loggerConfiguration.MinimumLevel.Information();
            Log.Logger = loggerConfiguration.CreateLogger();

            if (configurations.SourcePaths.Length == 0) {
                Log.Error("Не указано ни исходной папки"); 
            } else {
                var temp = time.Day + "." + time.Month + "." + time.Year;
                configurations.ArchivePath = Path.Combine(configurations.ArchivePath, temp);
                Directory.CreateDirectory(configurations.ArchivePath);
                foreach (var path in configurations.SourcePaths) {
                    Log.Information("Начало копирования файлов из папки: {0}", path);
                    var isCompleted = CopyToArchieve(path, Path.Combine(configurations.ArchivePath, CutDirectory(path)));
                    Log.Information("Резервное копирование файлов из папки завершено " + (isCompleted ? "успешно." : "неуспешно."));
                }
            }

            Log.CloseAndFlush();
            Console.WriteLine();
        }
        private static String CutDirectory(String path) {
            var tempArrayOfString = path.Split(new Char[] {'\\'});
            return tempArrayOfString[tempArrayOfString.Length - 1];
        }
        private static bool CopyToArchieve(String sourcePath, String archivePath) {
            var files = Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly);
            ParallelLoopResult result = Parallel.ForEach(files, (current) => {
                String fileName = current.Substring(sourcePath.Length + 1);
                try
                {
                    Directory.CreateDirectory(archivePath);
                    File.Copy(Path.Combine(sourcePath, fileName), Path.Combine(archivePath, fileName), true);
                }
                catch (SecurityException) {
                    //log
                } catch (Exception e) 
                {
                    //log
                    throw;
                }
            });
            if (result.IsCompleted)
            {
                //log
                return true;
            }
            else {
                //log
                return false;
            }
        }
    }
}
