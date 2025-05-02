using System.Windows.Media;

namespace Plotics
{
    public class SeriesSetting
    {
        // 列挙型
        public enum MarkerStyle
        {
            NONE,
            CIRCLE
        }
        public enum PlotStyle
        {
            SCATTER,
            LINE
        }

        // プロパティ
        public MarkerStyle Marker { get; set; } = MarkerStyle.CIRCLE;
        public PlotStyle Style { get; set;} = PlotStyle.SCATTER;
        public Color PlotColor { get; set; } = Colors.Black;

        internal PlotPoint? PrePoint { get; set; } = null;

        // コンストラクタ
        public SeriesSetting() { }
    }
}
