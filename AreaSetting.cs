using System.Windows.Media;

namespace Plotics
{
    public class AreaSetting
    {
        // プロパティ
        public double Maximum_X { get; set; } = 100.0;
        public double Minimum_X { get; set; } = 0.0;
        public double Maximum_Y { get; set; } = 1.0;
        public double Minimum_Y { get; set; } = -1.0;

        public string Title { get; set; } = "";
        public bool OnTitle { get; set; } = true;

        public int TickCountX { get; set; } = 10;
        public int TickCountY { get; set; } = 10;

        public string UnitX { get; set; } = "Hz";
        public bool OnUnitX { get; set; } = true;

        public string UnitY { get; set; } = "V";
        public bool OnUnitY { get; set; } = true;

        public bool OnGridX { get; set; } = true;
        public bool OnGridY { get; set; } = true;

        public string[]? ScaleX { get; set; } = null;
        public bool OnScaleX { get; set; } = true;

        public string[]? ScaleY { get; set; } = null;
        public bool OnScaleY { get; set; } = true;

        public bool OnZeroX { get; set; } = true;
        public bool OnZeroY { get; set; } = true;

        public Color? BackColor { get; set; } = null;
        public Color? PlotAreaColor { get; set; } = null;

        // コンストラクタ
        public AreaSetting() { }
    }
}
