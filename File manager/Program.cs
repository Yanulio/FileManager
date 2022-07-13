using System;
using System.IO;
using System.Text;

namespace File_manager
{
    /// <summary>
    /// Основной класс, в котором проводится вся работа.
    /// </summary>
    class Program
    {
        private static string s_fullPath;
        private static string s_drive;

        /// <summary>
        /// Точка входа. Количество строк чуть чуть превышает 40, но это только из-за трай кэтчей. Надеюсь, это не страшно 0-0
        /// </summary>
        static void Main()
        {
            bool endProg = false;
            do
            {
                try
                {
                    endProg = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Добро пожаловать в Файловый Менеджер! Для начала работы необходимо выбрать директорию. Начнём с диска:");
                    s_fullPath = "";
                    GetDrives();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Вы сейчас находитесь в: " + s_fullPath);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    bool check = false, changeDir = false;
                    do
                    {
                        Choice(out bool checkDirectoryEnd, out changeDir);
                        if (checkDirectoryEnd == false)
                            continue;
                        else
                        {
                            while (changeDir != true)
                            {
                                FileOperations(out endProg, out changeDir);
                            }
                            check = true;
                        }
                    } while (check != true);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Нет доступа к выбранному элементу. Попробуйте выбрать директорию заново, нажав на любую клавишу!");
                    Console.ReadKey();
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine("Не хватает памяти. Попробуйте выбрать директорию заново, нажав на любую клавишу!");
                    Console.ReadKey();
                }
                catch (IOException e)
                {
                    Console.WriteLine("Произошла ошибка при работе с файлами: \n" + e.Message + 
                        "\nПопробуйте выбрать директорию заново, нажав на любую клавишу!");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Произошла ошибка: \n" + e.Message + "\nПопробуйте выбрать директорию заново, нажав на любую клавишу!");
                    Console.ReadKey();
                }
            } while (endProg != true);
        }

        /// <summary>
        /// Метод, позволяющий получить список дисков и выбрать один из них. Реализовано с помощью GetLogicalDrives. Выводится список через цикл for.
        /// Далее человек вводит номер, и программа проверяет через if, подходит ли оно. Если нет, то человек вводит заново (из-за do while).
        /// </summary>
        
        public static void GetDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            //Вывод списка дисков в консоль.
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < drives.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + drives[i]);
            }
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            int number = 0;
            Console.WriteLine("Введите номер нужного вам диска.");
            //Получение номера из списка до тех пор, пока он не подойдёт под условия.
            do
            {
                Console.Write("Мой выбор: ");
                string numberStr = Console.ReadLine();
                if (!int.TryParse(numberStr, out number) || (number < 1) || (number > drives.Length))
                {
                    Console.WriteLine("Неверно введено значение, попробуйте еще раз.");
                    number = 0;
                }
            } while (number == 0);
            //Меняем значение пути.
            s_fullPath = drives[number - 1].ToString();
            s_drive = drives[number - 1].ToString();
        }

        /// <summary>
        /// Метод, дающий либо закончить выбор директории, либо остаться в нынешней. Если в директории нет поддиректорий, то сразу
        /// начинается работа с файлами. Реализовано с помощью switch - кейса.
        /// </summary>

        public static void Choice(out bool checkDirectoryEnd, out bool changeDir)
        {
            checkDirectoryEnd = changeDir = false;
            if (Directory.GetDirectories(s_fullPath).Length != 0)
            {
                Console.WriteLine("1. Я хочу работать в этой директории.");
                Console.WriteLine("2. Я хочу продолжить выбор директории или вернуться назад.");
                bool check = true;
                do
                {
                    Console.Write("Мой выбор: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "2":
                            GetDirectories(out checkDirectoryEnd, out changeDir);
                            check = true;
                            break;
                        case "1":
                            checkDirectoryEnd = true;
                            check = true;
                            break;
                        default:
                            Console.WriteLine("Неправильный ввод, попробуйте снова.");
                            check = false;
                            break;
                    }
                } while (check != true);
            }
            else
            {
                Console.WriteLine("В выбранной вами директории больше нет других папок. Переходим к работе с файлами.");
                checkDirectoryEnd = true;
            }
        }

        /// <summary>
        /// Метод, получающий все папки в выбранной директории и дающий выбрать одну из них или откатить на директорию назад. 
        /// Использован Directory.GetDirectories.
        /// </summary>

        public static void GetDirectories(out bool checkDirectoryEnd, out bool changeDir)
        {
            checkDirectoryEnd = changeDir = false;
            string[] allDirectories = Directory.GetDirectories(s_fullPath);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < allDirectories.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + allDirectories[i]);
            }
            int number = 0;
            Console.WriteLine((allDirectories.Length + 1) + ". Вернуться на директорию назад.");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Введите номер нужной вам директории или операции.");
            //Получение номера из списка до тех пор, пока он не подойдёт под условия.
            do
            {
                Console.Write("Мой выбор: ");
                string numberStr = Console.ReadLine();
                if (!int.TryParse(numberStr, out number) || (number < 1) || (number > allDirectories.Length + 1))
                {
                    Console.WriteLine("Неверно введено значение, попробуйте еще раз.");
                    number = 0;
                }
            } while (number == 0);
            if ((number == allDirectories.Length + 1) && (s_fullPath != s_drive))
                DirectoryBack();
            //Если находимя в диске, то откатываем программу назад и даем заново выбрать диск.
            else if ((number == allDirectories.Length + 1) && (s_fullPath == s_drive))
            {
                checkDirectoryEnd = changeDir = true;
            }
            else s_fullPath = allDirectories[number - 1];
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Вы сейчас находитесь в: " + s_fullPath);
            Console.ForegroundColor = ConsoleColor.Cyan;
        }
        
        /// <summary>
        /// Метод, выводящий список возможных операций с файлами и позволяющий выбрать одну из них.
        /// </summary>
        /// <param name="number">Номер выбранной операции.</param>

        public static void AllFileOperations(out int number)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("1. Просмотреть список файлов.");
            Console.WriteLine("2. Вывести содержимое файла в текстовом виде из этой директории и выбрать кодировку.");
            Console.WriteLine("3. Скопировать файл в эту директорию.");
            Console.WriteLine("4. Перенести файл в эту директорию.");
            Console.WriteLine("5. Удалить файл из этой директории.");
            Console.WriteLine("6. Создать пустой текстовый файл в выбранной кодировке.");
            Console.WriteLine("7. Создать текстовый файл в выбранной кодировке и записать в него строку текста.");
            Console.WriteLine("8. Сложить содержимое нескольких файлов и вывести в консоль.");
            Console.WriteLine("9. Заново выбрать директорию.");
            Console.WriteLine("10. Завершить работу программы.");
            Console.WriteLine("11. Вывести сценарий Шрека.");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            number = 0;
            Console.WriteLine("Введите номер нужной вам операции.");
            do
            {
                Console.Write("Мой выбор: ");
                string numberStr = Console.ReadLine();
                if (!int.TryParse(numberStr, out number) || (number < 1) || (number > 11))
                {
                    Console.WriteLine("Неверно введено значение, попробуйте еще раз.");
                    number = 0;
                }
            } while (number == 0);
        }

        /// <summary>
        /// Метод, реализующий файловые операции, принимая во внимание номер оперции, который указал пользователь.
        /// Реализовано с помощью switch кейсов.
        /// </summary>
        /// <param name="endProg">Переменная, дающая понять, надо ли завершать работу программы.</param>
        /// <param name="changeDir">Переменная, дающая понять, надо ли начать выбор директории заново.</param>

        public static void FileOperations(out bool endProg, out bool changeDir)
        {
            endProg = changeDir = false;
            AllFileOperations(out int number);
            switch (number)
            {
                case 1:
                    GetAllFiles(out string[] files);
                    break;
                case 2:
                    ReadWithEnc();
                    break;
                case 3:
                    CopyFile();
                    break;
                case 4:
                    FileMove();
                    break;
                case 5:
                    DeleteFile();
                    break;
                case 6:
                    CreateWithEnc();
                    break;
                case 7:
                    WriteWithEnc();
                    break;
                case 8:
                    FilesSum();
                    break;
                case 9:
                    changeDir = true;
                    break;
                case 10:
                    endProg = changeDir = true;
                    break;
                case 11:
                    ShrekScript();
                    break;
            }
        }

        /// <summary>
        /// Метод, получающий список всех файлов в директории. Если файлов нет, то выводим об этом подсказку.
        /// Использовано Directory.GetFiles.
        /// </summary>
        /// <param name="files">Массив всех путей к файлам в директории.</param>

        public static void GetAllFiles(out string[] files)
        {
            files = Directory.GetFiles(s_fullPath);
            if (files.Length != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("=====================================================================================");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine((i + 1) + ". " + files[i]);
                }
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("=====================================================================================");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
                NoFiles();
        }

        /// <summary>
        /// Метод, подсказывающий, что в директории нет файлов для работы.
        /// </summary>

        public static void NoFiles()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("К сожалению, в выбранной вами директории нет файлов, с которыми можно работать.");
            Console.WriteLine("Вы можете воспользоваться функциями, для которых не нужно иметь файл в директории ");
            Console.WriteLine("или выбрать директорию заново.");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=====================================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        /// <summary>
        /// Метод, выводящий полный сценарий Шрека в консоль ^_^
        /// </summary>

        public static void ShrekScript()
        {
            if (File.Exists("shrekScript.txt"))
            {
                using (StreamReader sr = new StreamReader("shrekScript.txt"))
                {
                    String line = sr.ReadToEnd();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(line);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    sr.Close();
                }
            }
            else Console.WriteLine("А вот зачем было убирать из нужного места файл? 0_0 Теперь никакого Шрека не будет!");
        }

        /// <summary>
        /// Метод, получающий предыдущую директорию.
        /// </summary>

        public static void DirectoryBack()
        {
            s_fullPath = Directory.GetParent(s_fullPath).ToString();
        }

        /// <summary>
        /// Метод, позволяющий удалить необходимый файл из директории.
        /// </summary>

        public static void DeleteFile()
        {
            GetAllFiles(out string[] files);
            if (files.Length != 0)
            {
                Console.WriteLine("Введите номер файла для удаления.");
                Console.Write("Мой выбор: ");
                string numberStr = Console.ReadLine();
                if (!int.TryParse(numberStr, out int number) || (number < 1) || (number > files.Length))
                    Console.WriteLine("Неверно введено значение, попробуйте снова.");
                else
                {
                    File.Delete(files[number - 1]);
                    Console.WriteLine("Файл был удален!");
                }
            }
        }

        /// <summary>
        /// Метод, позволяющий скопировать файл в выбранную директорию и выбрать для него имя.
        /// </summary>

        public static void CopyFile()
        {
            Console.WriteLine("Введите абсолютный путь файла, который хотите скопировать.");
            bool check = false;
            do 
            { 
            Console.Write("Путь файла: ");
            string filePath = Console.ReadLine();
                if (!File.Exists(filePath))
                    Console.WriteLine("Такого файла не существует, введите путь еще раз.");
                else
                {
                    check = true;
                    bool checkSecond = false;
                    Console.WriteLine("Введите название нового файла, который будет являться копией предыдущего.");
                    do
                    {
                        NewName(out bool invCheck, out string newFileName);
                        if (invCheck == true)
                            Console.WriteLine("В имени содержатся недопустимые символы, попробуйте еще раз.");
                        else if (File.Exists(Path.Combine(s_fullPath, newFileName)))
                            Console.WriteLine("Такой файл уже существует в директории, попробуйте еще раз.");
                        else
                        {
                            File.Copy(filePath, Path.Combine(s_fullPath, newFileName + Path.GetExtension(filePath)));
                            Console.WriteLine("Файл был скопирован!");
                            checkSecond = true;
                        }
                    } while (checkSecond != true);
                }
            } while (check != true);
        }

        /// <summary>
        /// Метод, перемещающий файл в выбранную директорию.
        /// </summary>

        public static void FileMove()
        {
            Console.WriteLine("Введите абсолютный путь файла, который хотите перенести.");
            bool check = false;
            do
            {
                Console.Write("Путь файла: ");
                string file = Console.ReadLine();
                if (!File.Exists(file))
                    Console.WriteLine("Такого файла не существует, введите путь еще раз.");
                else
                {
                    check = true;
                    File.Move(file, Path.Combine(s_fullPath, Path.GetFileName(file)));
                    Console.WriteLine("Файл был перемещен!");
                }
            } while (check != true);
        }

        /// <summary>
        /// Метод, позволяющий пользователю выбрать одну из четырех предоставленных кодировок.
        /// </summary>
        /// <param name="encoding">Выбранная кодировка.</param>

        public static void Encoding(out Encoding encoding)
        {
            Console.WriteLine("Выберите нужную вам кодировку:");
            Console.WriteLine("1. UTF-8");
            Console.WriteLine("2. UTF-16");
            Console.WriteLine("3. UTF-32");
            Console.WriteLine("4. ASCII");
            bool check = true;
            do
            {
                Console.Write("Мой выбор: ");
                string number = Console.ReadLine();
                encoding = System.Text.Encoding.Default;
                switch (number)
                {
                    case "1":
                        encoding = System.Text.Encoding.UTF8;
                        check = true;
                        break;
                    case "2":
                        encoding = System.Text.Encoding.Unicode;
                        check = true;
                        break;
                    case "3":
                        encoding = System.Text.Encoding.UTF32;
                        check = true;
                        break;
                    case "4":
                        encoding = System.Text.Encoding.ASCII;
                        check = true;
                        break;
                    default:
                        Console.WriteLine("Неверно введено значение, попробуйте снова.");
                        check = false;
                        break;
                }
            } while (check != true);
        }

        /// <summary>
        /// Метод, позволяющий вывести содержимое файла в консоль в выбранной кодировке.
        /// </summary>

        public static void ReadWithEnc()
        {
            Encoding(out Encoding encoding);
            GetAllFiles(out string[] files);
            if (files.Length != 0)
            {
                Console.WriteLine("Введите номер файла для вывода в консоль.");
                Console.Write("Мой выбор: ");
                string numberStr = Console.ReadLine();
                if (!int.TryParse(numberStr, out int number) || (number < 1) || (number > files.Length))
                    Console.WriteLine("Неверно введено значение, попробуйте снова.");
                else
                {
                    StreamReader strReader = new StreamReader(files[number - 1], encoding, false);
                    string text = strReader.ReadToEnd();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(text);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    strReader.Close();
                }
            }
        }

        /// <summary>
        /// Метод, дающий пользователю выбрать имя файла.
        /// </summary>
        /// <param name="invCheck">Переменная, дающая знать, вляется ли имя корректным</param>
        /// <param name="newFileName">Новое имя файла.</param>

        public static void NewName(out bool invCheck, out string newFileName)
        {
            invCheck = false;
            Console.Write("Название файла: ");
            newFileName = Console.ReadLine();
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                if (newFileName.Contains(invalid[i]))
                {
                    invCheck = true;
                    break;
                }
                invCheck = false;
            }
        }

        /// <summary>
        /// Метод, позволяющий записать текст в файл в выбранной кодировке.
        /// </summary>

        public static void WriteWithEnc()
        {
            bool check = false;
            Console.WriteLine("Введите название нового файла, в который будет записан текст.");
            do
            {
                NewName(out bool invCheck, out string newFileName);
                if (invCheck == true)
                    Console.WriteLine("В имени содержатся недопустимые символы, попробуйте еще раз.");
                else if (File.Exists(Path.Combine(s_fullPath, newFileName)))
                    Console.WriteLine("Такой файл уже существует в директории, попробуйте еще раз.");
                else
                {
                    check = true;
                    Encoding(out Encoding encoding);
                    string newPath = Path.Combine(s_fullPath, newFileName) + ".txt";
                    Console.WriteLine("Введите строку текста, которую хотите записать.");
                    string text = Console.ReadLine();
                    File.WriteAllText(newPath, text, encoding);
                    Console.WriteLine("Текст был записан!");
                }
            } while (check != true);
        }

        /// <summary>
        /// Метод, создающий пустой текстовый файл в выбранной кодировке.
        /// </summary>
        
        public static void CreateWithEnc()
        {
            bool check = false;
            Console.WriteLine("Введите название нового файла.");
            do
            {
                NewName(out bool invCheck, out string newFileName);
                if (invCheck == true)
                    Console.WriteLine("В имени содержатся недопустимые символы, попробуйте еще раз.");
                else if (File.Exists(Path.Combine(s_fullPath, newFileName)))
                    Console.WriteLine("Такой файл уже существует в директории, попробуйте еще раз.");
                else
                {
                    check = true;
                    Encoding(out Encoding encoding);
                    string newPath = Path.Combine(s_fullPath, newFileName) + ".txt";
                    File.WriteAllText(newPath, "", encoding);
                    Console.WriteLine("Файл был создан!");
                }
            } while (check != true);
        }

        /// <summary>
        /// Метод, позволяющий ввести номера файлов для сложения. Пришлось пожертвовать длиной метода, так как используется
        /// большое количество циклов и подсказок для пользователя и смысла делить это 
        /// на другие методы нет. В строке не должно быть null, символов кроме цифр и пробелов,
        /// а также числа должны соответствовать номерам файлов.
        /// </summary>
        public static void ChoosingFiles(out int[] choice, out string[] filesCopy)
        {
            GetAllFiles(out string[] files);
            filesCopy = files;
            choice = new int[files.Length];
            if (files.Length != 0)
            {
                Console.WriteLine("Введите в строчку через пробел номера двух или более файлов, которые хотите объединить.");
                Console.WriteLine("Пример: выбираются файлы номер 1, 2 и 3. Правильная запись: 1 2 3");
                bool check = false;
                do
                {
                    bool secondCheck = true;
                    string numbers = "";
                    do
                    {
                        numbers = Console.ReadLine();
                        if (numbers == null)
                            Console.WriteLine("Пустая строка не подходит! Попробуйте ещё раз.");
                    } while (numbers == null);
                    string[] choiceStr = numbers.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    choice = new int[choiceStr.Length];
                    if (choice.Length < 2)
                        Console.WriteLine("Нужно выбрать два или более файла. Попробуйте снова.");
                    else
                    {
                        for (int i = 0; i < choiceStr.Length; i++)
                        {
                            if (!int.TryParse(choiceStr[i], out choice[i]) || choice[i] < 1 || choice[i] > files.Length)
                            {
                                Console.WriteLine("Неправильный ввод, попробуйте снова.");
                                secondCheck = false;
                                break;
                            }
                        }
                        if (secondCheck != false)
                        {
                            Array.Sort(choice);
                            for (int i = 0; i < choiceStr.Length - 1; i++)
                            {
                                if (choice[i] == choice[i + 1])
                                {
                                    Console.WriteLine("Номера не должны повторяться... Попробуйте снова.");
                                    secondCheck = false;
                                    break;
                                }
                            }
                        }
                        if (secondCheck == true)
                            check = true;
                    }
                } while (check != true);
            }
        }

        /// <summary>
        /// Метод, суммирующий содержимое выбранных файлов и выводящий результат в консоль.
        /// </summary>

        public static void FilesSum()
        {
            ChoosingFiles(out int[] choice, out string[] files);
            bool check = false;
            Console.WriteLine("Введите название нового файла.");
            do
            {

                NewName(out bool invCheck, out string newFileName);
                if (invCheck == true)
                    Console.WriteLine("В имени содержатся недопустимые символы, попробуйте еще раз.");
                else if (File.Exists(Path.Combine(s_fullPath, newFileName)))
                    Console.WriteLine("Такой файл уже существует в директории, попробуйте еще раз.");
                else
                {
                    check = true;
                    for (int i = 0; i < choice.Length; i++)
                    {
                        string newText = File.ReadAllText(files[choice[i] - 1]) + "\n";
                        File.AppendAllText(Path.Combine(s_fullPath, newFileName) + ".txt", newText);
                    }
                    StreamReader strReader = new StreamReader(Path.Combine(s_fullPath, newFileName) + ".txt",
                        System.Text.Encoding.UTF8, false);
                    string text = strReader.ReadToEnd();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(text);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    strReader.Close();
                }
            } while (check != true);
        }
    }
}
