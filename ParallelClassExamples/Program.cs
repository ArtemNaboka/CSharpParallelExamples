using System;
using System.Threading.Tasks;

namespace ParallelClassExamples
{
    public class Program
    {
        public static void Main(String[] args)
        {
            ParallelLoopResult loopResult = Parallel.For(2, 100, GetSqrtOfCube);

            // Перевірка того, чи закінчився цикл повністю
            if (!loopResult.IsCompleted)
            {
                Console.WriteLine($"The last iteration was at number {loopResult.LowestBreakIteration}");
            }
            else
            {
                Console.WriteLine("Parallel loop was successfully executed");
            }
        }

        // Метод для підрахунку кореня із кубу числа
        public static void GetSqrtOfCube(Int32 number, ParallelLoopState state)
        {
            Double result = Math.Sqrt(Math.Pow(number, 3));
            Console.WriteLine(result);

            if (result > 800)
            {
                // Зупинення виконання паралельного циклу
                state.Break();
            }
        }
    }
}
