using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySearch
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] searchItems = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };

            Console.WriteLine(BinarySearch(searchItems, 3));
            Console.WriteLine(BinarySearch(searchItems, 4));
            Console.WriteLine(BinarySearch(searchItems, 5));
            Console.WriteLine(BinarySearch(searchItems, 6));
            Console.WriteLine(BinarySearch(searchItems, 7));
            Console.WriteLine(BinarySearch(searchItems, 8));
            Console.WriteLine(BinarySearch(searchItems, 9));
            Console.WriteLine(BinarySearch(searchItems, 13));
            Console.WriteLine(BinarySearch(searchItems, 14));
            Console.WriteLine(BinarySearch(searchItems, 15));
            Console.WriteLine(BinarySearch(searchItems, 16));
            Console.WriteLine(BinarySearch(searchItems, 17));
            Console.WriteLine(BinarySearch(searchItems, 20));
            Console.WriteLine(BinarySearch(searchItems, 21));
            Console.WriteLine(BinarySearch(searchItems, 22));
            Console.WriteLine(BinarySearch(searchItems, 25));
            Console.ReadKey();
        }

        public static string BinarySearch(int[] searchItems, int itemToLocate)
        {
            SearchRange range = new SearchRange();

            int count = 0;
            range.minimum = 0;
            range.maximum = searchItems.Length - 1;

            do
            {
                range.FindMiddleIndex();
                range.UpdateSearchRange(searchItems, itemToLocate);

                if (searchItems[range.middle] == itemToLocate)
                {
                    return string.Format("Looking for {0} and found {1} in {2} searches.", itemToLocate, searchItems[range.middle], count);
                }

                count++;
            } while (range.minimum <= range.maximum);

            return "Error";
        }

        private class SearchRange
        {
            public int minimum = 0;
            public int middle = 0;
            public int maximum = 0;

            public void FindMiddleIndex()
            {
                this.middle = (this.minimum + this.maximum) / 2;
            }

            public void UpdateSearchRange(int[] searchItems, int itemToLocate)
            {
                Console.WriteLine("Updating Search Range...");

                if (itemToLocate > searchItems[middle])
                {
                    this.minimum = this.middle + 1;
                }
                else
                {
                    this.maximum = this.middle - 1;
                }
            }
        }
    }
}
