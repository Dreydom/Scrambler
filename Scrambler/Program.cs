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
        static string TextToBinary(string input)
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
        static int And(string input1, string input2)
        {
            int count = 0;
            for (int i = 0; i < input1.Length; i++)
            {
                if (input1[i] == '1' && input1[i] == input2[i])
                    count++;
            }
            return count;
        }
        static string PAKF(string input)
        {
            string bytes = input;
            int len = bytes.Length;
            List<int> C = new List<int>();
            for (int i = 0; i < input.Length; i++)
            {
                int w = And(input, bytes);
                C.Add(w);
                string last = bytes[len - 1].ToString();
                bytes = bytes.Insert(0, last);
                bytes = bytes.Remove(len);
            }
            return String.Format("PAKF: {0}", String.Join(",", C));
        }
        static char Xor(char c1, char c2)
        {
            return c1 == c2 ? '0' : '1';
        }
        static string ScrSync(string input, string RG, int m, int n)
        {
            string output = "";
            foreach (char c in input)
            {
                char ch = Xor(c, Xor(RG[m - 1], RG[n - 1]));
                output += ch;
                RG = RG.Insert(0, ch.ToString()).Remove(m);
            }
            return output;
        }
        static string DescrSync(string input, string RG, int m, int n)
        {
            string output = "";
            foreach (char c in input)
            {
                char ch = Xor(c, Xor(RG[m - 1], RG[n - 1]));
                output += ch;
                RG = RG.Insert(0, c.ToString()).Remove(m);
            }
            return output;
        }
        static string ScrAdd(string input, string RG, int m, int n) //Скремблер == дескремблер
        {
            string output = "";
            foreach (char c in input)
            {
                char ch = Xor(RG[m - 1], RG[n - 1]);
                output += Xor(c, ch);
                RG = RG.Insert(0, ch.ToString()).Remove(m);
            }
            return output;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                
                Console.Write("М: ");
                int index = M.IndexOf(Int32.Parse(Console.ReadLine()));
                int m = M[index];
                int n = N[index];
                Console.WriteLine("N: {0}", n);
                string RG1 = new string('0', m - 1);
                RG1 += ('1');
                Console.Write("Введите строку: ");
                string input = TextToBinary(Console.ReadLine());
                Console.WriteLine("Сообщение:     {0}", input);
                string scrsync = ScrSync(input, RG1, m, n);
                Console.WriteLine("Скр. синхр.:   {0}", scrsync);
                //Console.WriteLine(PAKF(scrsync.Substring(0, (int)Math.Pow(2,m)-1)));
                string descrsync = DescrSync(scrsync, RG1, m, n);
                Console.WriteLine("Дескр. синхр.: {0}", descrsync);
                string scradd = ScrAdd(input, RG1, m, n);
                Console.WriteLine("Скр. адд.:     {0}", scradd);
                //Console.WriteLine(PAKF(scradd.Substring(0, (int)Math.Pow(2, m) - 1)));
                string descradd = ScrAdd(scradd, RG1, m, n);
                Console.WriteLine("Дескр. адд.:   {0}", descradd);
                Console.WriteLine(); 
            }
            
        }
    }
}