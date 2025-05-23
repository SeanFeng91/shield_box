using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCMag30FDevice;

namespace MCMAG30F
{
    /// <summary>
    /// 降采样转换器
    /// </summary>
    public class DownsampleConverter
    {
        private int m_nDecreaseSampleRateFactor = 10;           // 降采样比例因子

        private MultiChannelMagData[] m_dSampleDatas = null;    // 样本数据
        private int m_nSampleCount = 0;                         // 样本数量
        private int m_nNextPos = 0;                             // 下一个样本位置

        /// <summary>
        /// 设置降采样比例因子
        /// </summary>
        /// <param name="nDecreaseSampleRateFactor">降采样比例因子</param>
        /// <returns>设置是否成功</returns>
        public bool SetDecreaseSampleRateFactor(int nDecreaseSampleRateFactor)
        {
            if (nDecreaseSampleRateFactor <= 0)
                return false;

            m_nDecreaseSampleRateFactor = nDecreaseSampleRateFactor;

            // 重置降采样转换器的状态
            Reset();

            return true;
        }

        /// <summary>
        /// 获取降采样后的数据
        /// </summary>
        /// <param name="dOriginData">原始信号</param>
        /// <returns>降采样后的数据</returns>
        public MultiChannelMagData[] GetDownsampledData(MultiChannelMagData[] oOriginData)
        {
            if (oOriginData == null || oOriginData.Length < 1)
            {
                return null;
            }

            MultiChannelMagData[] dDataAfterDownsampled = null;
            int nTotalCount = m_nSampleCount + oOriginData.Length;              // 样本总数
            if (nTotalCount < m_nDecreaseSampleRateFactor)
            {
                // 拷贝数据，然后返回null
                for (int i = 0; i < oOriginData.Length; ++i)
                {
                    m_dSampleDatas[m_nNextPos] = CopyFromMultiChannelMagData(oOriginData[i]);
                    m_nNextPos = (m_nNextPos + 1) % (m_nDecreaseSampleRateFactor);
                }

                m_nSampleCount = nTotalCount;
                return null;
            }
            else if (nTotalCount >= m_nDecreaseSampleRateFactor)
            {
                int nDownsampledCount = nTotalCount / m_nDecreaseSampleRateFactor;  // 降采样后的数据
                dDataAfterDownsampled = new MultiChannelMagData[nDownsampledCount];
                int nIndex = 0;
                if (m_nSampleCount > 0)
                {
                    dDataAfterDownsampled[0] = CopyFromMultiChannelMagData(m_dSampleDatas[0]);
                    nIndex = 1;
                }
                
                for (int i = nIndex; i < nDownsampledCount; ++i)
                {
                    dDataAfterDownsampled[i] = CopyFromMultiChannelMagData(oOriginData[i * m_nDecreaseSampleRateFactor - m_nSampleCount]);
                }

                // 拷贝剩下的数据
                m_nSampleCount = nTotalCount % m_nDecreaseSampleRateFactor;
                m_nNextPos = 0;
                for (int i = oOriginData.Length - m_nSampleCount; i < oOriginData.Length; ++i)
                {
                    m_dSampleDatas[m_nNextPos] = CopyFromMultiChannelMagData(oOriginData[i]);
                    ++m_nNextPos;
                }
                for (int i = m_nNextPos; i < m_nDecreaseSampleRateFactor; ++i)
                {
                    m_dSampleDatas[i] = null;
                }

                return dDataAfterDownsampled;
            }

            return dDataAfterDownsampled;
        }

        /// <summary>
        /// 拷贝原始数据
        /// </summary>
        /// <param name="oOriginData">原始数据</param>
        /// <returns>拷贝出来的数据</returns>
        private MultiChannelMagData CopyFromMultiChannelMagData(MultiChannelMagData oOriginData)
        {
            MultiChannelMagData oTempData = new MultiChannelMagData();
            oTempData.m_nTimeStamp = oOriginData.m_nTimeStamp;
            oTempData.m_dMagData = new double[oOriginData.m_dMagData.Length];
            for (int i = 0; i < oOriginData.m_dMagData.Length; ++i)
            {
                oTempData.m_dMagData[i] = oOriginData.m_dMagData[i];
            }

            return oTempData;
        }

        /// <summary>
        /// 重置降采样转换器的状态
        /// </summary>
        public void Reset()
        {
            m_dSampleDatas = new MultiChannelMagData[m_nDecreaseSampleRateFactor];
            m_nSampleCount = 0;
            m_nNextPos = 0;
        }

    }
}
