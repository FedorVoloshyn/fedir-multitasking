using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace fedir_multitasking
{
    internal class Program
    {
        static StorageCounter storage = new StorageCounter();
        static object lockCounterObject = new object();
        static int putCarsCount = 100;
        static int popCarsCount = 100;
        static int iteration = 0;

        static void Main()
        {
            List<Task> cars = new List<Task>(putCarsCount + popCarsCount);
            FillCarsTaskListRandomly(cars, putCarsCount, popCarsCount);
            
            Console.WriteLine($"Press any key to start process");
            Console.ReadLine();

            cars.ForEach(c => c.Start());

            Console.ReadLine();
        }

        private static void FillCarsTaskListRandomly(List<Task> cars, int putCarsCount, int popCarsCount)
        {
            Random rand = new Random();

            for (int i = 0; i < putCarsCount; i++)
            {
                cars.Add(new Task(() => putCar()));
            }
            for (int i = 0; i < popCarsCount; i++)
            {
                cars.Add(new Task(() => popCar()));
            }

            Shuffle(cars);

            Console.WriteLine($"Cars count = {cars.Count}");
        }

        static void Shuffle<T>(List<T> list)
        {
            Random random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private static void putCar()
        {
            lock (lockCounterObject)
            {
                iteration++;
                storage.ItemsCount++;
                Console.WriteLine($"Iteration {iteration}, items count: {storage.ItemsCount}");
            }
        }

        private static void popCar()
        {
            lock (lockCounterObject)
            {
                while (storage.ItemsCount < 1)
                {
                    Monitor.Wait(lockCounterObject);
                }
                storage.ItemsCount--;
                Monitor.PulseAll(lockCounterObject);
                iteration++;
                Console.WriteLine($"Iteration {iteration}, items count: {storage.ItemsCount}");
            }
        }
    }
}
