using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrambler
{
    class Program
    {
        static Encoding cp866 = Encoding.GetEncoding("cp866");
        public static List<int> M = new List<int>()
        { 3,4,5,6,7,9,10,11,15,17,18,20,21,22,23,25,28,29,31 };
        public static List<int> N = new List<int>()
        { 2,3,3,5,6,5, 7, 9,14,14,11,17,19,21,18,22,25,27,28 };
        /// <summary>
        /// Переводит текст в бинарную строку.
        /// </summary>
        /// <param name="input">исходный текст</param>
        /// <returns>бинарная строка</returns>
        static string TextToBinary(string input) //описывать не буду, взял с инета ;D
        {
            byte[] bytes = cp866.GetBytes(input);
            string str = "";
            List<int> binary = new List<int>();
            foreach (byte b in bytes)
            {
                str += Convert.ToString(b, 2).PadLeft(8, '0');
            }
            return str;
        }
        /// <summary>
        /// Считает количество совпадающих единиц в двух бинарных строках. Нужно для функции PAKF.
        /// </summary>
        /// <param name="input1">исходная строка</param>
        /// <param name="input2">строка, которую сравнивают с исходной</param>
        /// <returns>количество парных единиц</returns>
        static int And(string input1, string input2)
        {
            int count = 0;
            for (int i = 0; i < input1.Length; i++)
                if (input1[i] == '1' && input1[i] == input2[i]) count++; //если обе единички, увеличиваем значение
            return count;
        }
        /// <summary>
        /// Высчитывает значение периодической автокорреляционной функции (Леухину не показывай только).
        /// </summary>
        /// <param name="input">Бинарная строка которую нужно проверить</param>
        /// <returns>значения And для всех смещенных строк</returns>
        static string PAKF(string input)
        {
            //будем смещать bytes, не трогая input
            string bytes = input;

            //длина строк
            int len = bytes.Length;

            //значения функции сохраняются в этот список
            List<int> C = new List<int>();

            //заполнение списка значениями для всех смещений
            for (int i = 0; i < input.Length; i++)
            {
                //добавление значения And в список
                int w = And(input, bytes);
                C.Add(w);

                //смещение на 1 бит вправо
                string last = bytes[len - 1].ToString();
                bytes = bytes.Insert(0, last);
                bytes = bytes.Remove(len);
            }
            //выписываем все значения в строку
            return String.Format("PAKF: {0}", String.Join(",", C));
        }
        /// <summary>
        /// Ксорит два бита между собой. Нужно для скремблера.
        /// </summary>
        /// <param name="c1">Один бит</param>
        /// <param name="c2">Второй бит</param>
        /// <returns>Третий бит</returns>
        static char Xor(char c1, char c2)
        {
            return c1 == c2 ? '0' : '1';
        }
        /// <summary>
        /// Самосинхронизирующийся скремблер
        /// </summary>
        /// <param name="input">Строка, которая подается на вход</param>
        /// <param name="RG">Регистр сдвига</param>
        /// <param name="m">индекс M</param>
        /// <param name="n">индекс N</param>
        /// <returns>скремблированная строка</returns>
        static string ScrSync(string input, string RG, int m, int n)
        {
            string output = "";
            foreach (char c in input) //для каждого бита входной строки
            {
                char ch = Xor(c, Xor(RG[m - 1], RG[n - 1])); //ксорим бит входной строки с M и N
                RG = RG.Insert(0, ch.ToString()).Remove(m); //результат идет в начало регистра
                output += ch; //результат также идет на выходную строку
            }
            return output;
        }
        /// <summary>
        /// Самосинхронизирующийся дескремблер
        /// </summary>
        /// <param name="input">Строка, которая подается на вход</param>
        /// <param name="RG">Регистр сдвига</param>
        /// <param name="m">индекс M</param>
        /// <param name="n">индекс N</param>
        /// <returns>дескремблированная строка</returns>
        static string DescrSync(string input, string RG, int m, int n)
        {
            string output = "";
            foreach (char c in input) //для каждого бита входной строки
            {
                char ch = Xor(c, Xor(RG[m - 1], RG[n - 1])); //ксорим бит входной строки с M и N
                RG = RG.Insert(0, c.ToString()).Remove(m); //в отличие от скремблера, в начало регистра передается бит из ВХОДНОЙ СТРОКИ
                output += ch; //а результат ксора идет на выходную строку
            }
            return output;
        }
        /// <summary>
        /// Аддитивный скремблер/дескремблер
        /// </summary>
        /// <param name="input">Строка, которая подается на вход</param>
        /// <param name="RG">Регистр сдвига</param>
        /// <param name="m">индекс M</param>
        /// <param name="n">индекс N</param>
        /// <returns>дескремблированная строка</returns>
        static string ScrAdd(string input, string RG, int m, int n) //Скремблер == дескремблер
        {
            string output = "";
            foreach (char c in input) //для каждого бита входной строки
            {
                char ch = Xor(RG[m - 1], RG[n - 1]); //ксорим ТОЛЬКО M и N
                RG = RG.Insert(0, ch.ToString()).Remove(m); //результат идет в начало регистра
                output += Xor(c, ch); //ксорим результат еще и с битом входной строки, затем на выходную строку
            }
            return output;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("М: ");

                //определение столбца в таблице M и N
                int index = M.IndexOf(Int32.Parse(Console.ReadLine()));

                // Нахождение значений по столбцу
                int m = M[index]; 
                int n = N[index];
                Console.WriteLine("N: {0}", n);

                // Заполнение регистра длиной M нулями и одной единицей
                string RG1 = new string('0', m - 1);
                RG1 += ('1');

                // Перевод текста в строку нулей и единиц (бинарную строку)
                Console.Write("Введите строку: ");
                string input = TextToBinary(Console.ReadLine());
                Console.WriteLine("Сообщение:     {0}", input);

                // Самосинхронизирующийся скремблер
                string scrsync = ScrSync(input, RG1, m, n);
                Console.WriteLine("Скр. синхр.:   {0}", scrsync);
                
                // Самосинхронизирующийся дескремблер
                string descrsync = DescrSync(scrsync, RG1, m, n);
                Console.WriteLine("Дескр. синхр.: {0}", descrsync);

                // Аддитивный скремблер
                string scradd = ScrAdd(input, RG1, m, n);
                Console.WriteLine("Скр. адд.:     {0}", scradd);
                
                // Аддитивный дескремблер (используется одна и та же функция)
                string descradd = ScrAdd(scradd, RG1, m, n);
                Console.WriteLine("Дескр. адд.:   {0}", descradd);

                Console.WriteLine();
            }
        }
    }
}
/*
 * Чтобы проверить ПАКФ какой-либо строки:
 * Console.WriteLine(PAKF(scrsync));
 * 
 * Тюкаев зачем-то просил прогонять по ПАКФ только часть строки длиной 2^M-1. Если и тебя попросит, как-то так:
 * Console.WriteLine(PAKF(scrsync.Substring(0, (int)Math.Pow(2,m)-1)));
 */
