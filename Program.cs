using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeersOnTheWall
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int x = 99; x >= 1; x--)
            {
                Console.WriteLine(string.Format("{0} bottles of {1} on the wall, take one down pass it around {2} bottles of {3} on the wall.", x, GetBeer(x), x-1, GetBeer(x-1)));
            }

            Console.ReadKey();
        }

        private static string GetBeer(int x)
        {
            if (IsDivisable(x, 7) && IsDivisable(x, 5))
                return "Miller Lite";
            else if (IsDivisable(x, 5))
                return "Lite";
            else if (IsDivisable(x, 7))
                return "Miller";
            else
                return "beer";
        }

        private static bool IsDivisable(int testNumer, int divisor)
        {
            return testNumer % divisor == 0;
        }
    }
}
