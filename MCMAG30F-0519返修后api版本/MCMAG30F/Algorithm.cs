using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MCMAG30F
{
    public class Algorithm
    {
        [DllImport("calcFFT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe static extern void CalcFFT(double* dX, int nXLen, ref double* dY, ref double* dFreq, ref int nYLen);

        [DllImport("calcFFT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe static extern void freeDynamicArray(ref double* pArr);

        [DllImport("designButterLPFilter.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private unsafe static extern void DesignButterLPFilter(double Fp, double Fc, double Rp, double Rs, double Fs, ref double* b, ref double* a, ref int order);

        public static void CalcFFT(double[] dX, ref double[] dY, ref double[] dFreq)
        {
            unsafe
            {
                double* pdY = null;
                double* pdFreq = null;
                int nLen = 0;

                fixed (double* pdX = &dX[0])
                {
                    CalcFFT(pdX, dX.Length, ref pdY, ref pdFreq, ref nLen);

                    dY = new double[nLen];
                    dFreq = new double[nLen];
                    for (int i = 0; i < nLen; ++i)
                    {
                        dY[i] = pdY[i];
                        dFreq[i] = pdFreq[i];
                    }

                    freeDynamicArray(ref pdY);
                    freeDynamicArray(ref pdFreq);
                }
            }
        }

        public static void DesignButterLPFilter(double Fp, double Fc, double Rp, double Rs, double Fs, ref double[] b, ref double[] a, ref int order)
        {
            unsafe
            {
                double* pdB = null;
                double* pdA = null;

                DesignButterLPFilter(Fp, Fc, Rp, Rs, Fs, ref pdB, ref pdA, ref order);
                b = new double[order + 1];
                a = new double[order + 1];
                for (int i = 0; i <= order; ++i)
                {
                    b[i] = pdB[i];
                    a[i] = pdA[i];
                }

                freeDynamicArray(ref pdB);
                freeDynamicArray(ref pdA);
            }
        }

    }
}
