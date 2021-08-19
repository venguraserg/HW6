using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HW6
{
    class Program
    {
        static void Main(string[] args)
        {
            string patchRead = "readFile.txt";
            string patchWrite = "writeFile.txt";
            int number;
            if (!(File.Exists(patchRead)))
            {
                Console.WriteLine($"В папке '{Environment.CurrentDirectory}' файл не найден, \nжелаете создать его?(Y/N)");
                if (EnterYesNo(""))
                {
                    //yes
                    Console.Clear();
                    Console.WriteLine("Файл будет создан по следующему пути: " + Environment.CurrentDirectory);
                    Console.WriteLine("Не будем вас утруждать, файл назовем 'readFile.txt'");
                    WriteNumberToFile(patchRead);
                    Console.WriteLine("Для продолжения нажмите любую кнопку...");
                    Console.ReadKey();

                }
                else
                {
                    //no
                    Console.Clear();
                    Console.WriteLine("Работа программы далее не возможна, для выхода нажмите любую клавишу...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            number = ReadNumberFromFile(patchRead);


            Console.WriteLine("Из файла получено число " + number);
            if (EnterYesNo("Желаете изменить/перезаписать (Y/N):"))
            {
                //yes
                WriteNumberToFile(patchRead);
                number = ReadNumberFromFile(patchRead);
            }



            if (EnterYesNo("Желаете ли вы просто посмотреть колличество крупп (Y/N):"))
            {
                int groups = (int)(Math.Log(number, 2) + 1);
                Console.Clear();
                Console.WriteLine($"Количество групп: {groups}");
            }
            else
            {

                Console.Clear();
                Console.WriteLine("Разобьем числа по группам и запишем данные в файл: " + Environment.CurrentDirectory + " " + patchWrite);
                var startWriteTime = DateTime.Now;

                SortAndWriterToFile(number, patchWrite);

                TimeSpan workTime = DateTime.Now.Subtract(startWriteTime);
                Console.WriteLine("Данные успешно записаны...");
                Console.WriteLine($"Время выполнения состравило: {workTime.TotalSeconds:f2} сек.");

                if (EnterYesNo("Хотите заархивировать файл? (Y/N) : "))
                {
                    GZip(patchWrite);
                }



            }


            Console.WriteLine("Конец программы. Для продолжения нажмите любую клавишу . . . ");
            Console.ReadKey();

        }

        public static int ReadNumberFromFile(string patch)
        {
            int number;
            do
            {
                if (int.TryParse(File.ReadAllText(patch), out number) == false && number <= 0) number = 0;
                if (number == 0)
                {
                    Console.WriteLine("Файл содержит не корректные данные.Введите заново");
                    WriteNumberToFile(patch);
                }
            } while (number == 0);


            return number;
        }

        /// <summary>
        /// метод записи числа в файл
        /// </summary>
        /// <param name="patchRead"></param>
        private static void WriteNumberToFile(string patchRead)
        {
            Console.Write("Введите число, с которым в дальнейшем будем работать: ");
            var numb = InputNumber();
            using (StreamWriter sw = new StreamWriter(patchRead))
            {
                sw.Write(numb);
            }
            Console.WriteLine("Файл создан, число записано");
        }

        /// <summary>
        /// Метод ввода Да/Нет
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static bool EnterYesNo(string text)
        {
            if (text != "") Console.WriteLine(text);
            char yn;
            bool correctParse, result = false;
            do
            {
                correctParse = char.TryParse(Console.ReadLine(), out yn);
                if (!(yn == 'n' || yn == 'N' || yn == 'y' || yn == 'Y'))
                {
                    Console.WriteLine("Не корректный ввод, попробуйте еще раз...");
                }
            } while (!(correctParse && (yn == 'n' || yn == 'N' || yn == 'y' || yn == 'Y')));

            if (yn == 'y' || yn == 'Y')
            {
                result = true;
            }
            else if (yn == 'n' || yn == 'N')
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Метод ввода исключительно чисел
        /// </summary>
        /// <param name="n">строка для парсинга</param>
        /// <returns></returns>
        static int InputNumber()
        {
            bool correctParse;
            int outValue;
            do
            {
                correctParse = int.TryParse(Console.ReadLine(), out outValue);
                if (!correctParse) Console.WriteLine("Не корректный ввод, попробуйте еще раз...");
            }
            while (!correctParse);
            return outValue;
        }

        /// <summary>
        /// Сортировка чисел и запись в файл
        /// </summary>
        /// <param name="number">Количество чисел</param>
        public static void SortAndWriterToFile(int number, string patch)
        {

            int groups = (int)(Math.Log(number, 2) + 1);
            Console.WriteLine($"Количество групп: {groups}");

            int count = 0;

            using (StreamWriter sw = new StreamWriter(patch))
            {

                for (int i = 1; i <= number; i++)
                {
                    if (count <= groups)
                    {
                        if (i % Math.Pow(2, count) == 0)
                        {
                            count++;
                            sw.WriteLine();
                            sw.Write($"{i} ");
                        }
                        else
                        {
                            sw.Write($"{i} ");
                        }
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }

        }

        public static void GZip(string patch)
        {
            char[] charSeparator = new char[1] {'.'};
            var newpatch = patch.Split(charSeparator)[0]+".zip";
            using (FileStream ss = new FileStream(patch, FileMode.OpenOrCreate))
            {
                using (FileStream ts = File.Create(newpatch))
                {
                    //поток архивации 
                    using (GZipStream cs = new GZipStream(ts, CompressionMode.Compress))
                    {
                        ss.CopyTo(cs);
                        Console.WriteLine($"Сжатие файла {patch} завершено. Исходный размер - {ss.Length}, размер после сжатия - {ts.Length}");
                    }

                }
            }

        }
    }
}
