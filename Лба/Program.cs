using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Homework_Strings_Collections
{
    class Program
    {
        // Вынес словарь в статическое поле для чистоты кода
        private static readonly Dictionary<string, string> CorrectionsMap = new Dictionary<string, string>
        {
            ["првиет"] = "привет",
            ["пирвет"] = "привет",
            ["здраствуйте"] = "здравствуйте",
            ["програмирование"] = "программирование",
            ["ошибко"] = "ошибка",
            ["колекция"] = "коллекция" // Добавил еще одно слово для отличия
        };

        static void Main(string[] args)
        {
            Console.Title = "Редактор текстовых файлов v1.0";
            Console.Write("Укажите путь к папке с документами: ");
            string path = Console.ReadLine();

            // Проверяем, существует ли указанный путь на диске
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                Console.WriteLine("Критическая ошибка: путь не найден.");
                return;
            }

            ProcessFiles(path);

            Console.WriteLine("\nВсе операции успешно завершены.");
            Console.WriteLine("Нажмите любую клавишу для закрытия программы...");
            Console.ReadKey();
        }

        private static void ProcessFiles(string folderPath)
        {
            // Получаем только .txt файлы через поиск
            var files = Directory.GetFiles(folderPath, "*.txt");

            if (files.Length == 0)
            {
                Console.WriteLine("В данной директории отсутствуют текстовые файлы.");
                return;
            }

            foreach (var filePath in files)
            {
                Console.WriteLine($"-> Обработка объекта: {Path.GetFileName(filePath)}");

                string text = File.ReadAllText(filePath);

                // 1. Исправляем ошибки, используя словарь
                text = FixSpellingErrors(text);

                // 2. Форматируем номера телефонов
                text = ReformatPhoneNumbers(text);

                // Перезаписываем файл обновленным контентом
                File.WriteAllText(filePath, text);
            }
        }

        private static string FixSpellingErrors(string source)
        {
            string result = source;
            foreach (var entry in CorrectionsMap)
            {
                // Используем Regex.Replace с учетом границ слова (\b) и игнорированием регистра
                string pattern = @"\b" + Regex.Escape(entry.Key) + @"\b";
                result = Regex.Replace(result, pattern, entry.Value, RegexOptions.IgnoreCase);
            }
            return result;
        }

        private static string ReformatPhoneNumbers(string input)
        {
            /* 
               Поиск шаблона: (0XX) XXX-XX-XX
               Группировка:
               1 - код города (без нуля)
               2 - первая часть номера
               3 - вторая часть
               4 - третья часть
            */
            const string phonePattern = @"\(0(\d{2})\)\s?(\d{3})-(\d{2})-(\d{2})";
            const string targetFormat = "+380 $1 $2 $3 $4";

            return Regex.Replace(input, phonePattern, targetFormat);
        }
    }
}