using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

internal static class Creater
{
    // Путь куда переместить, путь до приложения NativeAot, стандартное название для конечного файла
    private const string APPS = @"СЮДА НЕОБХОДИМО ВСТАВИТЬ ПУТЬ, ВКЛЮЧЁННЫЙ В $PATH";
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
            MessageBox(IntPtr.Zero, "Ошибка:\nФайл не найден!", "Creater", MS_TYPE_CLASSIC | MS_ICON_ERROR);
            return;
        }

        // Получение имени для приложения
        string name = GetName(args);

        // Получение нового пути приложения
        string newPath = Path.Combine(APPS, name + ".exe");

        // Вопрос о необходимости перезаписи, если файл с новым именем уже существует
        int answer = 0;
        if (Path.Exists(newPath))
        {
            answer = MessageBox(IntPtr.Zero, "Файл уже создан. Перезаписать?", "Creater", MS_TYPE_YES_NO | MS_ICON_QUESTION);
            if (answer == MS_CHOICE_NO)
                return;
        }

        // Пробуем переместить и выводим сообщение об успехе, или сообщение с ошибкой в зависимости от результата
        try
        {
            app.MoveTo(newPath, true);
            MessageBox(IntPtr.Zero, "Файл " + app.Name + " успешно создан!", "Creater", MS_TYPE_CLASSIC);
        }
        catch (Exception ex)
        {
            MessageBox(IntPtr.Zero, "Ошибка:\n" + ex.Message, "Creater", MS_TYPE_CLASSIC | MS_ICON_ERROR);
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

    // Импортируем MessageBox
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    // Типы окон
    private const uint MS_TYPE_CLASSIC= 0x00000000;
    private const uint MS_TYPE_YES_NO = 0x00000004;

    // Иконки
    private const uint MS_ICON_QUESTION = 0x00000020;
    private const uint MS_ICON_ERROR = 0x00000010;

    // Типы ответов при YESNO
    private const int MS_CHOICE_YES = 6;
    private const int MS_CHOICE_NO = 7;
}