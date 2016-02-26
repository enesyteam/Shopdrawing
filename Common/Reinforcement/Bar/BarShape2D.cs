using System.ComponentModel;

namespace Shopdrawing.Reinforcement
{
    /// <summary>
    /// Bar shapes in 2D
    /// </summary>
    public enum BarShape2D
    {
        // Base on BS 8666:2005

        [Description("S00")]
        Shape00 = 0,
        [Description("S01")]
        Shape01 = 1,
        [Description("S11")]
        Shape11 = 2,
        [Description("S12")]
        Shape12 = 3,
        [Description("S13")]
        Shape13 = 4,
        [Description("S14")]
        Shape14 = 5,
        [Description("S15")]
        Shape15 = 6,
        [Description("S21")]
        Shape21 = 7,
        [Description("S22")]
        Shape22 = 8,
        [Description("S23")]
        Shape23 = 9,
        [Description("S24")]
        Shape24 = 10,
        [Description("S25")]
        Shape25 = 11,
        [Description("S26")]
        Shape26 = 12,
        [Description("S27")]
        Shape27 = 13,
        [Description("S28")]
        Shape28 = 14,
        [Description("S29")]
        Shape29 = 15,
        [Description("S31")]
        Shape31 = 16,
        [Description("S32")]
        Shape32 = 17,
        [Description("S33")]
        Shape33 = 18,
        [Description("S34")]
        Shape34 = 19,
        [Description("S35")]
        Shape35 = 20,
        [Description("S36")]
        Shape36 = 21,
        [Description("S41")]
        Shape41 = 22,
        [Description("S44")]
        Shape44 = 23,
        [Description("S46")]
        Shape46 = 24,
        [Description("S47")]
        Shape47 = 25,
        [Description("S51")]
        Shape51 = 26,
        [Description("S56")]
        Shape56 = 27,
        [Description("S63")]
        Shape63 = 28,
        [Description("S64")]
        Shape64 = 29,
        [Description("S67")]
        Shape67 = 30,
        [Description("S75")]
        Shape75 = 31,

        // From Shape77, it's 3D shape
    }
}
