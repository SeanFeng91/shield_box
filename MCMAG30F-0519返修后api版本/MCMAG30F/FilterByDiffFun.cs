using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCMAG30F
{
    public class FilterByDiffFun
    {
        /// <summary>
        /// a(0)*y(n) = b(0)*x(n) + b(1)*x(n-1) + ... + b(nb)*x(n-nb)
        ///             - a(1)*y(n-1)  - ... - a(na)*y(n-na)
        /// </summary>
        private double[] m_dB = null;
        private double[] m_dA = null;
        private int m_nOrder = 0;

        private double[] m_dX = null;
        private double[] m_dY = null;
        private int m_nNextPos = 0;

        /// <summary>
        /// 设置滤波器参数
        /// </summary>
        /// <param name="dB">B系数</param>
        /// <param name="dA">A系数</param>
        /// <param name="nOrder">阶数</param>
        /// <returns>设置是否成功</returns>
        public bool SetParameter(double[] dB, double[] dA, int nOrder)
        {
            if (dB == null || dB.Length < 1 || dA == null || dA.Length < 1 || nOrder <= 0)
                return false;

            m_dB = new double[dB.Length];
            for (int i = 0; i < dB.Length; ++i)
            {
                m_dB[i] = dB[i] / dA[0];
            }

            m_dA = new double[dA.Length];
            for (int i = 0; i < dA.Length; ++i)
            {
                m_dA[i] = dA[i] / dA[0];
            }

            m_nOrder = nOrder;

            // 重置滤波数据缓存
            ResetFilter();

            return true;
        }

        /// <summary>
        /// 滤波
        /// </summary>
        /// <param name="dOriginData">原始信号</param>
        /// <returns>滤波后的数据</returns>
        public double[] Filter(double[] dOriginData)
        {
            if (dOriginData == null || dOriginData.Length < 1)
            {
                return null;
            }

            double[] dDataAfterFilter = new double[dOriginData.Length];
            for (int i = 0; i < dOriginData.Length; ++i)
            {
                dDataAfterFilter[i] = FilterOneData(dOriginData[i]);
            }

            return dDataAfterFilter;
        }

        /// <summary>
        /// 滤波一个数据
        /// </summary>
        /// <param name="dData">一个原始信号数据</param>
        /// <returns>滤波后的数据</returns>
        public double FilterOneData(double dData)
        {
            double dDataAfterFilter = 0.0;

            m_dX[m_nNextPos] = dData;

            double dBXSum = 0.0;
            int nIndex = 0;
            for (int i = 0; i < m_dB.Length; ++i)
            {
                nIndex = (m_nNextPos - i + m_dB.Length) % m_dB.Length;
                dBXSum += m_dB[i] * m_dX[nIndex];
            }

            double dAYSum = 0.0;
            for (int i = 1; i < m_dA.Length; ++i)
            {
                nIndex = (m_nNextPos - i + m_dA.Length) % m_dA.Length;
                dAYSum += m_dA[i] * m_dY[nIndex];
            }

            dDataAfterFilter = dBXSum - dAYSum;

            m_dY[m_nNextPos] = dDataAfterFilter;

            m_nNextPos = (m_nNextPos + 1) % m_dB.Length;

            return dDataAfterFilter;
        }

        /// <summary>
        /// 重置滤波数据缓存
        /// </summary>
        public void ResetFilter()
        {
            m_dX = new double[m_dB.Length];
            for (int i = 0; i < m_dB.Length; ++i)
            {
                m_dX[i] = 0.0;
            }

            m_dY = new double[m_dA.Length];
            for (int i = 0; i < m_dA.Length; ++i)
            {
                m_dY[i] = 0.0;
            }

            m_nNextPos = 0;
        }

    }
}
