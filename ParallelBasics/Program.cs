using System;
using System.Linq;
using System.Threading;

namespace ParallelBasics
{
    public class FuncInfo
    {
        public FuncInfo(Int32 startNumber, Int32 endNumber)
        {
            StartNumber = startNumber;
            EndNumber = endNumber;
            Result = 0;
        }

        // Число, з якого буде починатися пошук
        // простих чисел для даного потоку
        public Int32 StartNumber { get; set; }

        // Число (включно), на якому закінчиться пошук
        // простих чисел для даного потоку
        public Int32 EndNumber { get; set; }

        // Сюди будемо записувати кількість простих чисел
        // на даному відрізку
        public Int32 Result { get; set; }
    }

    public class Program
    {
        public static void Main(String[] args)
        {
            const Int32 maxBound = 10000000;

            // Кількість ядер на даному ПК
            Int32 coresNumber = Environment.ProcessorCount;

            Thread[] threads = new Thread[coresNumber - 1];
            FuncInfo[] funcInfos = new FuncInfo[coresNumber];

            Int32 iterationsPerThread = maxBound / coresNumber;
            Int32 i, currentStartNumber;
            for (i = 0, currentStartNumber = 0; i < coresNumber - 1; i++, currentStartNumber += iterationsPerThread)
            {
                funcInfos[i] = new FuncInfo(currentStartNumber + 1, currentStartNumber + iterationsPerThread);

                threads[i] = new Thread(PrintPrimeCount);
                threads[i].Start(funcInfos[i]);
            }

            funcInfos[i] = new FuncInfo(currentStartNumber + 1, currentStartNumber + iterationsPerThread);

            PrintPrimeCount(funcInfos[i]);

            for (Int32 j = 0; j < threads.Length; j++)
            {
                threads[j].Join();
            }

            Int32 result = funcInfos.Select(fi => fi.Result).Sum();

            Console.WriteLine($"There are {result} prime numbers between 0..10 000 000");
            Console.WriteLine("Program has finished!!!");
        }

        public static void PrintPrimeCount(Object obj)
        {
            // Кількість простих чисел на даному відрізку
            Int32 count = 0;
            FuncInfo funcInfo = (FuncInfo)obj;

            for (Int32 currentNumber = funcInfo.StartNumber; currentNumber <= funcInfo.EndNumber; currentNumber++)
            {
                if (IsPrime(currentNumber))
                    count++;
            }

            Console.WriteLine(
                $"There are {count} prime numbers between {funcInfo.StartNumber}..{ funcInfo.EndNumber}");

            // Записуємо результат роботи потоку
            funcInfo.Result = count;
        }

        public static Boolean IsPrime(Int32 number)
        {
            if (number < 2)
                return false;

            for (Int32 i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }
    }
}
