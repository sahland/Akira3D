using System.IO;
using System.Reflection;

namespace Akira.Models.Auxiliary
{
    // Вспомогательный класс для загрузки файлов
    public static class ManifestResourceLoader
    {
        // Загружает указанный ресурс по определенному пути
        public static string LoadTextFile(string textFileName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var pathToDots = textFileName.Replace("\\", ".");
            var location = string.Format($"{executingAssembly.GetName().Name}.{pathToDots}");

            using (var stream = executingAssembly.GetManifestResourceStream(location))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
