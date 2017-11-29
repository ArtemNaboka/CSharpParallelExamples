using System;
using System.Threading;
using System.Threading.Tasks;

namespace TPLCancellationToken
{
    public class Program
    {
        public static Int32 Factorial(Int32 number, CancellationToken token)
        {
            if (number == 0)
                return 1;

            Int32 result = 1;

            for (Int32 i = 2; i <= number; i++)
            {
                // Затримтка, щоб метод не встиг виконатися
                Thread.Sleep(100);
                // Викидуємо виключення при запиті на відміну
                token.ThrowIfCancellationRequested();
                result *= i;
            }

            return result;
        }


        public static void Main(String[] args)
        {
            // Створення джерела токенів
            CancellationTokenSource source = new CancellationTokenSource();

            // Запуск асинхронної задачі та передача параметрку та токену
            Task<Int32> task = Task.Run(() => Factorial(30, source.Token));

            // Чекаємо 2 секунді
            task.Wait(2000);

            try
            {
                // Відміняємо операцію
                source.Cancel();
                Console.WriteLine($"Factorial of 999 is {task.Result}");
            }
            catch (AggregateException)
            {
                Console.WriteLine("Task has been cancelled!");
            }
        }
    }
}
