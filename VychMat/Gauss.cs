using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VychMat
{
    public static class Gauss
    {
        public static void Exchange(double[,] arr, int from, int to)
        {
            for(var i = 0; i < arr.Length; i++)
            {
                arr[from, i] = arr[from, i] + arr[to, i];
                arr[to, i] = arr[from, i] - arr[to, i];
                arr[from, i] = arr[from, i] - arr[to, i];
            }
        }
    }
}
