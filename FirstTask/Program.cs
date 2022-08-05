using Serilog;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using Newtonsoft.Json;

namespace FirstTask
{
    public static class Program
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
                .WriteTo.File("log" + timeID + ".txt",
                    rollingInterval: RollingInterval.Infinite,
                    rollOnFileSizeLimit: true);
            if (configurations.Log == "Debug") loggerConfiguration.MinimumLevel.Debug();
            if (configurations.Log == "Error") loggerConfiguration.MinimumLevel.Error();
            if (configurations.Log == "Information") loggerConfiguration.MinimumLevel.Information();
            Log.Logger = loggerConfiguration.CreateLogger();
            
            Log.Debug("Логгирование запущенно");
            Log.Information("Программа запущенна");

            if (configurations.SourcePaths.Length == 0) {
                Log.Error("Не указано ни одной исходной папки"); 
            } else {
                configurations.ArchivePath = Path.Combine(configurations.ArchivePath, timeID);
                Directory.CreateDirectory(configurations.ArchivePath);
                foreach (var path in configurations.SourcePaths) {
                    var flag = true;
                    
                    Log.Information("Начало копирования файлов из папки {0}", path);
                    
                    try
                    {
                        CopyToArchieve(path, Path.Combine(configurations.ArchivePath, CutDirectory(path)));
                    }
                    catch (Exception) {
                        flag = false;    
                        throw; 
                    }
                    finally
                    {
                        Log.Information("Резервное копирование файлов из папки {0} завершено " + (flag ? "успешно" : "неуспешно"), path);
                    }
                }
            }

            Log.Information("Программа завершила выполнение");
            Log.CloseAndFlush();
            Console.WriteLine();
        }
        private static String CutDirectory(String path) {
            var tempArrayOfString = path.Split(new Char[] {'\\'});
            return tempArrayOfString[tempArrayOfString.Length - 1];
        }
        private static void CopyToArchieve(String sourcePath, String archivePath) {
            var files = Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly);
            ParallelLoopResult result = Parallel.ForEach(files, (current) => {
                String fileName = current.Substring(sourcePath.Length + 1);
                try
                {
                    Directory.CreateDirectory(archivePath);
                    File.Copy(Path.Combine(sourcePath, fileName), Path.Combine(archivePath, fileName), true);
                    Log.Debug("Файл {0} успешно скопирован", current);
                }
                catch (SecurityException) {
                    Log.Warning(String.Format("Не удалось получить доступ к файлу {0}. Программа продолжит свое выполнение, пропустив его", current));
                } catch (Exception e) 
                {
                    Log.Error(e.Message);
                    throw;
                }
            });
        }
    }
}
