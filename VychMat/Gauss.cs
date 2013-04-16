using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VychMat
{
    /// <summary>
    /// Решение СЛАУ методом Гаусса
    /// </summary>
    public static class Gauss
    {
        /// <summary>
        /// Прямой ход
        /// </summary>
        /// <param name="a">Матрица коэффициентов</param>
        /// <param name="b">Матрица свободных членов</param>
        static void ForwardWay(double[,] a, double[]b)
        {
            double v;
            var n = b.Length;
            for (int k = 0, i, j, im; k < n - 1; k++)
            {
                im = k;
                for (i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[im,k]) < Math.Abs(a[i,k]))
                    {
                        im = i;
                    }
                }
                if (im != k)
                {
                    for (j = 0; j < n; j++)
                    {
                        v = a[im,j];
                        a[im,j] = a[k,j];
                        a[k,j] = v;
                    }
                    v = b[im];
                    b[im] = b[k];
                    b[k] = v;
                }
                for (i = k + 1; i < n; i++)
                {
                    v = 1.0 * a[i,k] / a[k,k];
                    a[i,k] = 0;
                    b[i] = b[i] - v * b[k];
                    if (v != 0)
                        for (j = k + 1; j < n; j++)
                        {
                            a[i,j] = a[i,j] - v * a[k,j];
                        }
                }
            }
        }

        /// <summary>
        /// Обратный ход
        /// </summary>
        /// <param name="a">Матрица коэффициентов</param>
        /// <param name="b">Матрица свободных членов</param>
        /// <returns>Матрица неизвестных</returns>
        static double[] BackwardWay(double[,] a, double[] b)
        {
            var n = b.Length;
            var x = new double[n];
            double s = 0;
            x[n - 1] = 1.0 * b[n - 1] / a[n - 1,n - 1];
            for (int i = n - 2, j; 0 <= i; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + a[i,j] * x[j];
                }
                x[i] = 1.0 * (b[i] - s) / a[i,i];
            }
            return x;
        }

        /// <summary>
        /// Решение системы уравлений
        /// </summary>
        /// <param name="a">Матрица коэффициентов</param>
        /// <param name="b">Матрица свободных членов</param>
        /// <returns>Матрица неизвестных</returns>
        public static double[] Solve(double[,] a, double[] b)
        {
            var am = (double[,])a.Clone();
            var bm = (double[])b.Clone();
            ForwardWay(am, bm);
            var x = BackwardWay(am, bm);
            return x;
        }
    }
}
