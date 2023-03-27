using System;
using System.Threading;

namespace pop_lab_2
{
    internal class Program
    {
        private static readonly int dim = 10000000;
        private static readonly int threadNum = 5;

        private readonly Thread[] thread = new Thread[threadNum];

        static void Main(string[] args)
        {
            Program main = new Program();
            main.InitArr();
            //var part_min = main.PartMin(0, dim);
            var parallel_min= main.ParallelMin();

            //Console.WriteLine($"value:{part_min.Elem_value}\tindex:{part_min.Elem_index}");
            Console.WriteLine($"value:{parallel_min.Elem_value}\tindex:{parallel_min.Elem_index}");

            Console.ReadKey();
        }

        private int threadCount = 0;

        private MinElem ParallelMin()
        {
            var partLength = arr.Length / threadNum;
            for (int i = 0; i < threadNum; i++)
            {
                thread[i] = new Thread(StarterThread);
                thread[i].Start(new Bound(partLength * i, partLength*(i+1)));
            }
            lock (lockerForCount)
            {
                while (threadCount < threadNum)
                {
                    Monitor.Wait(lockerForCount);
                }
            }
            return min;
        }

        private readonly int[] arr = new int[dim];

        private void InitArr()
        {   
            Random rand = new Random();
            for (int i = 0; i < dim; i++)
            {
                arr[i] = rand.Next(1,100000);
            }
            arr[rand.Next(0, arr.Length - 1)] = rand.Next(-100000,-1);
        }
        class Bound
        {
            public Bound(int startIndex, int finishIndex)
            {
                StartIndex = startIndex;
                FinishIndex = finishIndex;
            }

            public int StartIndex { get; set; }
            public int FinishIndex { get; set; }
        }

        private readonly object lockerForMin = new object();
        private void StarterThread(object param)
        {
            if (param is Bound)
            {
                MinElem min = PartMin((param as Bound).StartIndex, (param as Bound).FinishIndex);

                lock (lockerForMin)
                {
                    CollectMin(min);
                }
                IncThreadCount();
            }
        }

        private readonly object lockerForCount = new object();
        private void IncThreadCount()
        {
            lock (lockerForCount)
            {
                threadCount++;
                Monitor.Pulse(lockerForCount);
            }
        }
        public class MinElem 
        {
            public MinElem(int elem_value, int elem_index)
            {
                Elem_value = elem_value;
                Elem_index = elem_index;
            }
            public int Elem_value { get; set; }
            public int Elem_index { get; set; }
        }
        private MinElem min= new MinElem(1000, 0);
        public void CollectMin(MinElem min)
        {
            if (min.Elem_value <= this.min.Elem_value) {
                this.min.Elem_value = min.Elem_value;
                this.min.Elem_index = min.Elem_index;
            }
        }

        public MinElem PartMin(int startIndex, int finishIndex)
        {
            MinElem min = new MinElem(arr[0],0);
            
            for (int i = startIndex+1; i < finishIndex; i++)
            {
                if (arr[i] <= min.Elem_value) {
                    min.Elem_value = arr[i];
                    min.Elem_index = i;
                }
            }
            return min;
        }
    }
}
