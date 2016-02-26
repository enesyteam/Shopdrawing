using System;

namespace Shopdrawing.BridgeFS
{
    public enum Unit
    {
        /// <summary>
        /// Chiều dài
        /// </summary>
        Length = 0,
        /// <summary>
        /// Khối lượng
        /// </summary>
        Mass = 1,
        /// <summary>
        /// Diện tích
        /// </summary>
        Area = 2,
        /// <summary>
        /// Thể tích
        /// </summary>
        Volume = 3,
        /// <summary>
        /// Mật độ (khối lượng riêng)
        /// </summary>
        Density = 4,

        Custom = 9
    }
}
