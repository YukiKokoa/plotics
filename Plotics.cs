using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Plotics
{
    public class Plotics
    {
        public const string VERSION = "1.0.0";

        // # プロパティ
        public WriteableBitmap Image
        {
            get
            {
                return  Renderer.Image;
            }
        }
        public AreaSetting Area { get; set; } = new();
        public SeriesSetting Series { get; set; } = new();

        ChartRenderer Renderer { get; set; }

        // # コンストラクタ
        public Plotics(int width, int height) { Renderer = new(width, height); }
        public Plotics(ChartRenderer renderer) { Renderer = renderer; }

        // # メソッド private
        // X,YをPLOT座標へ変換
        private PlotPoint Transformation(double x, double y, AreaSetting? are=null)
        {
            are ??= Area;
            double normalizedX = (x - Area.Minimum_X) / (Area.Maximum_X - Area.Minimum_X);
            double normalizedY = (y - Area.Minimum_Y) / (Area.Maximum_Y - Area.Minimum_Y);

            int pixelX = Renderer.MarginLeft + (int)(normalizedX * (Renderer.Width - Renderer.MarginLeft - Renderer.MarginRight));
            int pixelY = Renderer.Height - Renderer.MarginTop - (int)(normalizedY * (Renderer.Height - Renderer.MarginTop - Renderer.MarginBottom)); // 上下反転

            return new PlotPoint(pixelX, pixelY);
        }

        // 軸スケールラベルを生成
        private string[] GenerateScaleLabels(double min, double max, int divisions)
        {
            if (divisions < 1)
            {
                return new string[0];
            }

            string[] labels = new string[divisions + 1];
            double step = (max - min) / divisions;

            for (int i = 0; i <= divisions; i++)
            {
                double value = min + (step * i);
                labels[i] = $"{value:0.##}";
            }

            return labels;
        }

        // X軸のスケールを描画
        private void RenderScaleX(string[]? labels = null, AreaSetting? area = null)
        {
            area ??= Area;
            labels ??= GenerateScaleLabels(area.Minimum_X, area.Maximum_X, area.TickCountX);
            
            Renderer.DrawScaleX(labels);
        }

        // Y軸のラベルを描画
        private void RenderScaleY(string[]? labels = null, AreaSetting? area = null)
        {
            area ??= Area;
            labels ??= GenerateScaleLabels(area.Minimum_Y, area.Maximum_Y, area.TickCountY);
            
            Renderer.DrawScaleY(labels);
        }

        // X=0の線を描画
        private void RenderZeroX(AreaSetting area)
        {
            PlotPoint p = Transformation(0, 0, area);
            Renderer.DrawVirticalLine(p.X, Colors.Black);
        }
        // Y=0の線を描画
        private void RenderZeroY(AreaSetting area) 
        {
            PlotPoint p = Transformation(0, 0, area);
            Renderer.DrawHorizontalLine(p.Y, Colors.Black);
        }

        // # メソッド public
        // エリアの描画
        public void DrawArea(AreaSetting? area=null)
        {
            area ??= Area;

            Renderer.Begin();

            if (area.BackColor != null) Renderer.FillBackColor((Color)area.BackColor);
            if (area.PlotAreaColor != null) Renderer.FillPlotAreaColor((Color)area.PlotAreaColor);

            if (area.OnGridX) Renderer.DrawGridX(area.TickCountX);
            if (area.OnGridY) Renderer.DrawGridY(area.TickCountY);

            if (area.OnUnitX && area.UnitX != "") Renderer.DrawUnitX(area.UnitX);
            if (area.OnUnitY && area.UnitY != "") Renderer.DrawUnitY(area.UnitY);

            if (area.OnScaleX) RenderScaleX(area.ScaleX, area);
            if (area.OnScaleY) RenderScaleY(area.ScaleY, area);

            if (area.OnZeroX && area.Minimum_X * area.Maximum_X < 0) RenderZeroX(area);
            if (area.OnZeroY && area.Minimum_Y * area.Maximum_Y < 0) RenderZeroY(area);

            if (area.OnTitle && area.Title != string.Empty) Renderer.DrawTitle(area.Title);
   
            Renderer.DrawTickX(area.TickCountX);
            Renderer.DrawTickY(area.TickCountY);
            Renderer.DrawPlotArea();

            Renderer.End();  
        }

        // ポイントをリストで追加
        public void AddPoints(List<double> xs, List<double> ys, SeriesSetting? setting=null)
        {
            setting ??= Series;
            if (xs.Count != ys.Count)
            {
                throw new ArgumentException("Difference count between X and Y");
            }
            
            Renderer.Begin();
            for(int i = 0; i < xs.Count; i++)
            {
                PlotPoint point = Transformation(xs[i], ys[i]);
                if (setting.Marker == SeriesSetting.MarkerStyle.CIRCLE) Renderer.DrawPoint(point.X, point.Y, setting.PlotColor);
                if (setting.PrePoint is PlotPoint pp && setting.Style == SeriesSetting.PlotStyle.LINE)
                {
                    Renderer.DrawLine(pp.X, pp.Y, point.X, point.Y, setting.PlotColor);
                }
                setting.PrePoint = point;
            }
            Renderer.End();
        }

        // ポイントを追加
        public void AddPoint(double x, double y, SeriesSetting? setting=null)
        {
            setting ??= Series;

            Renderer.Begin();

            PlotPoint point = Transformation(x, y);

            if (setting.Marker == SeriesSetting.MarkerStyle.CIRCLE) Renderer.DrawPoint(point.X, point.Y, setting.PlotColor);
            if (setting.PrePoint is PlotPoint pp && setting.Style == SeriesSetting.PlotStyle.LINE)
            {
                Renderer.DrawLine(pp.X, pp.Y, point.X, point.Y, setting.PlotColor);
            }
            setting.PrePoint = point;

            Renderer.End();
        }

        // X軸ラベル描画
        public void DrawScaleLabelX(string[]? labels = null, AreaSetting? area = null)
        {
            Renderer.Begin();
            RenderScaleX(labels, area);
            Renderer.End();
        }

        // Y軸ラベル描画
        public void DrawScaleLabelY(string[]? labels = null, AreaSetting? area = null)
        {
            Renderer.Begin();
            RenderScaleY(labels, area);
            Renderer.End();
        }

        // 縦のラインを描画
        public void DrawVirticalLine(double x, Color? color=null)
        {
            Renderer.Begin();
            PlotPoint p = Transformation(x, 0);
            Renderer.DrawVirticalLine(p.X, color);
            Renderer.End();
        }

        // 横のラインを描画
        public void DrawHorizontalLine(double y, Color? color=null)
        {
            Renderer.Begin();
            PlotPoint p = Transformation(0, y);
            Renderer.DrawHorizontalLine(p.Y, color);
            Renderer.End();
        }

        // 座標指定でテキストを描画
        public void DrawTextOnCodinate(double x, double y, string text, Color? color=null)
        {
            PlotPoint p = Transformation(x, y);
            DrawTextOnPixel(p.X, p.Y, text, color);
        }

        // ピクセル指定でテキストを描画
        public void DrawTextOnPixel(int x, int y, string text, Color? color = null)
        {
            Renderer.Begin();
            Renderer.DrawArbitraryText(x, y, text, color);
            Renderer.End();
        }

        // 上下左右のマージンをセット
        public void SetMargin(int left, int top, int right, int bottom)
        {
            Renderer.Margin(left, top, right, bottom);
        }

        // マージンのセット
        public void SetMargin(int margin)
        {
            Renderer.Margin(margin, margin, margin, margin);
        }

        // 外部へ座標変換結果を出力
        public int[] TransformationXYtoPixcel(double x, double y)
        {
            PlotPoint point = Transformation(x, y);

            return [ point.X, point.Y ];
        }

        // Imageをクリア
        public void Clear()
        {
            Renderer.Begin();
            Renderer.Clear();
            Renderer.End();
        }
    }
}
