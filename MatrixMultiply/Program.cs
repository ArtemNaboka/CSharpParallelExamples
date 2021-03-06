﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiply
{
    public class Program
    {
        // Кількість ядер в процесорі
        public static Int32 ProcessorCount = Environment.ProcessorCount;

        public static void Main(String[] args)
        {
            // Розмір матриць 2048*2048
            const Int32 n = 2048;
            // Кількість повторів заміру часу
            const Int32 tryCount = 3;

            // Ініціалізуємо першу матрицю
            Double[,] m1 = InitializeMatrix(n, n);
            // Ініціалізуємо другу матрицю
            Double[,] m2 = InitializeMatrix(n, n);
            // Результат множення при послідовному алгоритмі
            Double[,] res = new Double[n, n];
            // Результат паралельного множення
            // при використанні класу Parallel
            Double[,] resParallel = new Double[n, n];
            // Результат паралельного множення
            // при використанні класу Tasks
            Double[,] resTasks = new Double[n, n];
            // Результат паралельного множення
            // при використанні класу Thread
            Double[,] resThreads = new Double[n, n];

            // Таймер для виміру часу
            Stopwatch timer = new Stopwatch();
            Int64 minTimeSeq, minTimeParallel, minTimeTasks, minTimeThreads;
            minTimeSeq = minTimeParallel = minTimeTasks = minTimeThreads = Int64.MaxValue;

            for (Int32 i = 0; i < tryCount; i++)
            {

                // Вимір часу для послідовного алгоритму
                timer.Start();
                MultiplyMatricesSequential(m1, m2, res, n);
                timer.Stop();
                minTimeSeq = minTimeSeq > timer.ElapsedMilliseconds
                    ? timer.ElapsedMilliseconds
                    : minTimeSeq;

                timer.Reset();

                // Вимір часу для класу Parallel
                timer.Start();
                MultiplyMatricesParallel(m1, m2, resParallel, n);
                timer.Stop();
                minTimeParallel = minTimeParallel > timer.ElapsedMilliseconds
                    ? timer.ElapsedMilliseconds
                    : minTimeParallel;

                // Перевірка правильності результату
                if (!AreMatricesEqual(res, resParallel))
                {
                    Console.WriteLine("ALARM!! Different result in Sequential and Parallel.For() multiply");
                }

                timer.Reset();

                // Вимір часу для класу Task
                timer.Start();
                MultiplyMatricesTasks(m1, m2, resTasks, n);
                timer.Stop();
                minTimeTasks = minTimeTasks > timer.ElapsedMilliseconds
                   ? timer.ElapsedMilliseconds
                   : minTimeTasks;

                // Перевірка правильності результату
                if (!AreMatricesEqual(res, resTasks))
                {
                    Console.WriteLine("ALARM!! Different result in Sequential and Tasks multiply");
                }

                timer.Reset();

                // Вимір часу для класу Thread
                timer.Start();
                MultiplyMatricesThreads(m1, m2, resThreads, n);
                timer.Stop();
                minTimeThreads = minTimeThreads > timer.ElapsedMilliseconds
                   ? timer.ElapsedMilliseconds
                   : minTimeThreads;

                // Перевірка правильності результату
                if (!AreMatricesEqual(res, resThreads))
                {
                    Console.WriteLine("ALARM!! Different result in Sequential and Threads multiply");
                }

                timer.Reset();

                // Обнуляємо матриці результатів
                res = InitializeMatrix(n, n, true, 0);
                resParallel = InitializeMatrix(n, n, true, 0);
                resTasks = InitializeMatrix(n, n, true, 0);
                resThreads = InitializeMatrix(n, n, true, 0);
            }

            Console.WriteLine($"Suquential: {minTimeSeq} milisecs");
            Console.WriteLine($"Parallel: {minTimeParallel} milisecs");
            Console.WriteLine($"Tasks: {minTimeTasks} milisecs");
            Console.WriteLine($"Threads: {minTimeThreads} milisecs");

            //Double[,] resCPP = new Double[n, n];
            //MultiplyMatricesCPP(m1, m2, resCPP, n);
        }

        #region Helpers

        // Початкова ініціалізація матриці
        public static Double[,] InitializeMatrix(Int32 rows, Int32 cols,
            Boolean useDefaultValue = false, Double defaultValue = 0)
        {
            Double[,] matrix = new Double[rows, cols];

            Random r = new Random();
            for (Int32 i = 0; i < rows; i++)
            {
                for (Int32 j = 0; j < cols; j++)
                {
                    matrix[i, j] = useDefaultValue ? defaultValue : r.Next(100);
                }
            }
            return matrix;
        }

        public static Double[][] InitMatrix(Int32 rows, Int32 cols)
        {
            Double[][] matrix = new Double[rows][];

            Random r = new Random();
            for (Int32 i = 0; i < rows; i++)
            {
                matrix[i] = new Double[cols];
                for (Int32 j = 0; j < cols; j++)
                {
                    matrix[i][j] = r.Next(100);
                }
            }
            return matrix;
        }

        // Перевірка еквівалентності матриць
        public static Boolean AreMatricesEqual(Double[,] matrA, Double[,] matrB)
        {
            for (Int32 i = 0; i < matrA.GetLength(0); i++)
            {
                for (Int32 j = 0; j < matrB.GetLength(1); j++)
                {
                    if (Math.Abs(matrA[i, j] - matrB[i, j]) > 10e-10)
                        return false;
                }
            }

            return true;
        }

        #endregion

        public static void MultiplyMatricesSequential(Double[,] matrA, Double[,] matrB,
                                            Double[,] result, Int32 n)
        {
            for (Int32 i = 0; i < n; i++)
            {
                for (Int32 j = 0; j < n; j++)
                {
                    Double temp = matrA[i, j];
                    for (Int32 k = 0; k < n; k++)
                    {
                        result[i, k] += temp * matrB[j, k];
                    }
                }
            }
        }

        public static void MultiplyMatricesParallel(Double[,] matrA, Double[,] matrB,
            Double[,] result, Int32 n)
        {
            Parallel.For(0, n, i =>
            {
                for (Int32 j = 0; j < n; j++)
                {
                    Double temp = matrA[i, j];
                    for (Int32 k = 0; k < n; k++)
                    {
                        result[i, k] += temp * matrB[j, k];
                    }
                }
            });
        }

        public static void MultiplyMatricesTasks(Double[,] matrA, Double[,] matrB,
            Double[,] result, Int32 n)
        {
            // Масив задач
            Task[] tasks = new Task[ProcessorCount - 1];
            // Кількість ітерацій на потік
            Int32 iterationsPerTask = n / ProcessorCount;
            Int32 i, currentStartNumber;
            for (i = 0, currentStartNumber = 0; i < ProcessorCount - 1; i++, currentStartNumber += iterationsPerTask)
            {
                Int32 number = currentStartNumber;
                // Створюємо та одразу запускаємо нову задачу
                tasks[i] = Task.Run(() => 
                    MultiplyWithBounds(matrA, matrB, result, n, number, number + iterationsPerTask - 1));
            }

            // Для головного потоку
            MultiplyWithBounds(matrA, matrB, result, n, currentStartNumber, currentStartNumber + iterationsPerTask - 1);

            // Чекаємо, коли усі задачі завершаться
            Task.WhenAll(tasks).Wait();
        }

        public static void MultiplyMatricesThreads(Double[,] matrA, Double[,] matrB,
            Double[,] result, Int32 n)
        {
            Thread[] threads = new Thread[ProcessorCount - 1];
            // Кількість ітерацій на потік
            Int32 iterationsPerTask = n / ProcessorCount;
            Int32 i, currentStartNumber;
            for (i = 0, currentStartNumber = 0; i < ProcessorCount - 1; i++, currentStartNumber += iterationsPerTask)
            {
                Int32 number = currentStartNumber;
                // Створюємо новий потік
                threads[i] = new Thread(() 
                    => MultiplyWithBounds(matrA, matrB, result, n, number, number + iterationsPerTask - 1));
                threads[i].Start();
            }

            // Для головного потоку
            MultiplyWithBounds(matrA, matrB, result, n, currentStartNumber, currentStartNumber + iterationsPerTask - 1);

            // Чекаємо завершення усіх потоків
            for (Int32 j = 0; j < ProcessorCount - 1; j++)
            {
                threads[j].Join();
            }
        }

        // Множення певних рядів матриць
        public static void MultiplyWithBounds(Double[,] matrA, Double[,] matrB,
            Double[,] result, Int32 n, Int32 startRow, Int32 finishRow)
        {
            for (Int32 i = startRow; i <= finishRow; i++)
            {
                for (Int32 j = 0; j < n; j++)
                {
                    Double temp = matrA[i, j];
                    for (Int32 k = 0; k < n; k++)
                    {
                        result[i, k] += temp * matrB[j, k];
                    }
                }
            }
        }

        [DllImport(@"C:\Users\User\Documents\Visual Studio 2015\Projects\CSharpParallelExamples\Release\MatrixMultiplyCPP.dll")]
        public static extern void MultiplyMatricesCPP(Double[,] matrA, Double[,] matrB,
            Double[,] result, Int32 n);
    }
}
