using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shopdrawing.Structures.Culvert
{
    public interface IBoxCulvertSection
    {
        //CulvertSectionType SectionType { get; set; }
        /// <summary>
        /// Số cell trên mặt cắt ngang (Với cống hộp)
        /// </summary>
        NumberOfCells NumberOfCells { get; set; }
        /// <summary>
        /// Bề rộng mỗi cell
        /// </summary>
        double B { get; set; }
        /// <summary>
        /// Chiều cao mỗi cell
        /// </summary>
        double H { get; set; }
        /// <summary>
        /// Chiều dày bản trên
        /// </summary>
        double TopThickness { get; set; }
        /// <summary>
        /// Chiều dày bản dưới
        /// </summary>
        double BottomThickness { get; set; }
        /// <summary>
        /// Bề dày thành ngoài
        /// </summary>
        double ExternalWallThickness { get; set; }
        /// <summary>
        /// Bề dày thành trong
        /// </summary>
        double InternalWallThickness { get; set; }
        /// <summary>
        /// Có bản quá độ phía trái
        /// </summary>
        bool HasApproachSlabOnTheLeft { get; set; }
        /// <summary>
        /// Có bản quá độ phía phải
        /// </summary>
        bool HasApproachSlabOnTheRight { get; set; }
    }
}
