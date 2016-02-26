using System;

namespace Shopdrawing.Reinforcement
{
    public class ValidationDimensions
    {
        /// <summary>
        /// Phương thức kiểm tra sự hợp lệ của kích thước thanh thép
        /// </summary>
        /// <param name="bar">Thanh cần kiểm tra</param>
        /// <returns>true nếu các kích thước thoả mãn các điều kiện kiểm tra</returns>
        public static bool Check(Bar bar)
        {
            switch (bar.ShapeCode)
            {
                case BarShape2D.Shape11:
                    {
                        return (bar.Dimensions.A > 0 && bar.Dimensions.B > 0);
                    }
                case BarShape2D.Shape12:
                    {
                        return (bar.Dimensions.A > 0 && bar.Dimensions.B > 0);
                    }
                case BarShape2D.Shape13:
                    {
                        if(bar.Dimensions.A > 0 
                            && bar.Dimensions.B > 0 
                            && bar.Dimensions.C > 0
                            && bar.Dimensions.A >= 0.5*bar.Dimensions.B
                            && bar.Dimensions.C >= 0.5 * bar.Dimensions.B) return true;
                            return false;
                    }
                default: return false;
            }
        
        }

        public static string GetNotifycation(Bar bar)
        {
            switch (bar.ShapeCode)
            {
                case BarShape2D.Shape11:
                    {
                        return "Neither A nor B shall be less than P in table 2";
                    }
                case BarShape2D.Shape12:
                    {
                        return "Neither A nor B shall be less than P in table 2 no less than R + 6d";
                    }
                case BarShape2D.Shape13:
                    {
                        return "B shall not be less than 2(r + d). Neither A nor C shall be less than P in Table 2 nor less than (B/2 + 5d). See Note 3";
                    }
                default: return string.Empty;
            }
        
        }
    }
}
