using System;
using System.Threading;

namespace LibrarySync
{
    public class Program
    {
        public static void Main(String[] args)
        {
            for (Int32 i = 0; i < 7; i++)
            {
                // Створюємо читача в окремому потоці
                Thread t = new Thread(Library.Read);
                t.Start(i);
            }

            Console.ReadLine();
        }
    }

    public class Library
    {
        // Створюємо семафор
        private static readonly Semaphore Sem = new Semaphore(3, 3);

        // Метод, з якого почнеться виконання кожного потоку
        public static void Read(Object obj)
        {
            Int32 threadName = (Int32) obj;
            Sem.WaitOne();

            Thread.Sleep(1000);
            Console.WriteLine($"Reader {threadName} entered");

            Thread.Sleep(1000);
            Console.WriteLine($"Reader {threadName} is reading");

            Thread.Sleep(1000);
            Console.WriteLine($"Reader {threadName} leaved");

            Sem.Release();
        }
    }
}
