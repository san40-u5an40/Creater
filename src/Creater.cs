using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class Creater
{
    // Путь куда переместить, путь до приложения NativeAot, стандартное название для конечного файла
    private const string APPS = @"*ВСТАВИТЬ СВОЙ ПУТЬ, ВКЛЮЧЁННЫЙ В $PATH*";
    private const string EXE = @".\bin\Release\net9.0\win-x64\publish\";
    private const string DEFAULT_NAME_EXE = "app";

    // Программа работает по следующему алгоритму:
    // Ищет расположение в текущей папке *.exe с программой, скомпилированной средствами Native AOT
    // Сохраняет переданный аргумент в переменную названия программы, которое ей будет присвоено на выходе (выход в папке apps, которая может быть включена в $PATH, например)
    // На основе этого имени получает новый конечный путь программы
    // Если такой путь уже существует то спрашивает о перезаписи
    // Ну и при необходимости перемещает найденную программу в получившийся новый путь: $"{$PATH}\{name_from_argument}.exe"
    internal static void Create(string[] args)
    {
        // Получение *.exe со скомпилированной программой, средствами NativeAot
        FileInfo? app = GetExeFile();
        if(app == null)
        {
            Console.WriteLine("Ошибка: Файл не найден!");
            return;
        }

        // Получение имени для приложения
        string name = GetName(args);

        // Получение нового пути приложения
        string newPath = Path.Combine(APPS, name + ".exe");

        // Вопрос о необходимости перезаписи, если файл с новым именем уже существует
        if (Path.Exists(newPath))
        {
            Console.Write("Файл уже создан. Перезаписать? (Y/N): _\b");
            var answer = Console.ReadKey();

            Console.Write('\r' + new string(' ', Console.WindowWidth) + '\r'); // Очистка строки

            if (answer.Key != ConsoleKey.Y)
                return;
        }

        // Пробуем переместить и выводим сообщение об успехе, или сообщение с ошибкой в зависимости от результата
        try
        {
            app.MoveTo(newPath, true);
            Console.WriteLine("Файл " + app.Name + " успешно создан!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка:\n" + ex.Message);
        }
    }

    // Блок поиска расположения программы, скомпилированной средствами Native AOT:
    //
    // Основная проблема заключается в том, что относительной путь в зависимости от того, содержатся ли в папке решения файлы проектов, будет отличаться
    // Для поиска пути сначала рассматривается случай, когда программа находится в папке, которая содержит "bin" с искомым *.exe
    // Если в текущей директории отсутствует "bin" с искомым *.exe, она ищет его в подпапках текущей директории
    // Во всех подпапках должен содержаться единственный "bin" с *.exe, иные случаи эта программа не обрабатывает
    private static FileInfo? GetExeFile()
    {
        DirectoryInfo currDir = new DirectoryInfo(Environment.CurrentDirectory);
        DirectoryInfo dirExe = new DirectoryInfo(Path.Combine(currDir.FullName, EXE));
        bool isContains = false;

        if (dirExe.Exists && dirExe.GetFiles("*.exe").Count() == 1)
            isContains = true;

        if (!isContains)
        {
            var dirsExeList = new List<DirectoryInfo>();

            foreach (var dir in currDir.GetDirectories())
            {
                string tempPath = Path.Combine(dir.FullName, EXE);
                DirectoryInfo tempDir = new DirectoryInfo(tempPath);

                if (tempDir.Exists && tempDir.GetFiles("*.exe").Count() == 1)
                    dirsExeList.Add(tempDir);
            }

            if (dirsExeList.Count() != 1)
                return null;

            dirExe = dirsExeList[0];
        }

        return dirExe.GetFiles("*.exe")[0];
    }

    // Блок получения имени из аргументов
    //
    // При некорректных случаях возвращает константу DEFAULT_NAME_EXE
    // При наличии информации в аргументах объединяет её через пробел и возвращает
    // Это в том числе позволяет вводить название без обособления его кавычками
    private static string GetName(string[] args)
    {
        if (args.Length == 0 || args.Length > 0 && args.Any(string.IsNullOrEmpty))
            return DEFAULT_NAME_EXE;
        else
            return string.Join(' ', args);
    }
}