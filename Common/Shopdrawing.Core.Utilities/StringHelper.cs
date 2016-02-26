using System;
using System.Data;
using System.Text;
using Shopdrawing.Core.Utilities.Csv;
using System.IO;

namespace Shopdrawing.Core.Utilities
{
    /// <summary>
    /// This class provides a number of methods that help with a string.
    /// </summary>
    public static class TextHelper
    {
        public static string ToDecimalFormat(int numberOfDecimal)
        {
            switch (numberOfDecimal)
            { 
                case 0:
                    return "0";
                case 1:
                    return "0.0";
                case 2:
                    return "0.00";
                case 3:
                    return "0.000";
                case 4:
                    return "0.0000";
                case 5:
                    return "0.00000";
                case 6:
                    return "0.000000";
                case 7:
                    return "0.0000000";
                case 8:
                    return "0.00000000";
                case 9:
                    return "0.000000000";
                case 10:
                    return "0.0000000000";
                default:
                    return "";

            }
        }
    }
}