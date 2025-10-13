# Creater
## Назначение:
Небольшая программа для быстрого перемещения приложения, созданного средствами Native AOT, в папку, которая, например, может содержаться в $PATH (эта папка указывается в коде программы). Это сильно ускоряет релиз небольших приложений и позволяет их моментально использовать после компиляции.

Эта программа обрабатывает не только случаи совместного расположения файлов проекта и решения, но также и случаи хранения проектов в подпапках решения.
## Примеры использования:

Рассмотрим работу программы "Creater" на примере релиза самого "Creater" :)

Расположение проекта:
```CMD
Z:\4_programming\csharp\projects\Creater\ProgramFiles>dir

 Содержимое папки Z:\4_programming\csharp\projects\Creater\ProgramFiles

12.10.2025  18:44    <DIR>          .
12.10.2025  18:44    <DIR>          ..
12.10.2025  18:20    <DIR>          bin
12.10.2025  18:44             6 564 Creater.cs
12.10.2025  18:42               350 Creater.csproj
12.10.2025  18:04             1 118 Creater.sln
12.10.2025  18:41    <DIR>          obj
12.10.2025  18:43               113 Program.cs
               4 файлов          8 145 байт
```

Публикация (используется алиас pub для команды "dotnet publish -r win-x64 -c Release"):
```CMD
Z:\4_programming\csharp\projects\Creater\ProgramFiles>pub

Восстановление завершено (0,8 с)
  Creater успешно выполнено (8,2 с) → bin\Release\net9.0\win-x64\publish\

Сборка успешно выполнено через 9,3 с
```

Использование самой программы (в моём случае называется "crt"):
```CMD
Z:\4_programming\csharp\projects\Creater\ProgramFiles>crt new
Файл new.exe успешно создан!

Z:\4_programming\csharp\projects\Creater\ProgramFiles>

:: Появилось уведомление об успешном создании файла
:: Если файл с таким именем уже будет существовать, программа спросит о необходимости перезаписать этот файл
```

После перемещения программы в $PATH её можно сразу же использовать:
```CMD
Z:\4_programming\csharp\projects\Creater\ProgramFiles>new test
Ошибка: Файл не найден!

Z:\4_programming\csharp\projects\Creater\ProgramFiles>
:: Т.к. файл из bin\Release\net9.0\win-x64\publish\ уже был перемещён в $PATH
```