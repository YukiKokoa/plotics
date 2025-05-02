using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plotics
{
    public class ChartRenderer
    {
        // # プロパティ
        public int Width { get { return Image.PixelWidth; } }
        public int Height { get { return Image.PixelHeight; } }
        public int MarginLeft { get { return _renderer.RenderArea.X; } }
        public int MarginTop { get { return _renderer.RenderArea.Y; } }
        public int MarginRight { get { return Width - _renderer.RenderArea.X - _renderer.RenderArea.Width; } }
        public int MarginBottom { get { return Height - _renderer.RenderArea.Y - _renderer.RenderArea.Height; } }
        
        int PlotAreaWidth { get { return _renderer.RenderArea.Width; } }
        int PlotAreaHeight { get { return _renderer.RenderArea.Height; } }


        public WriteableBitmap Image // 出力
        {
            get
            {
                return _renderer.Image;
            }
        }
        
        private Renderer _renderer;

        // # コンストラクタ
        public ChartRenderer(int width, int height)
        {
            _renderer = new Renderer(width, height);
            Margin(100, 100, 100, 100);
        }

        // # メソッド public
        
        // 描画開始
        public void Begin()
        {
            _renderer.Begin();
        }
        
        // 描画終了
        public void End()
        {
            _renderer.End();
        }

        // クリア
        public void Clear()
        {
            _renderer.FillImage();
        }

        // マージンのセット
        public void Margin(int left=100, int top= 100, int right=100, int bottom=100)
        {
            _renderer.RenderArea = new Int32Rect()
            {
                X = left,
                Y = top,
                Width = Image.PixelWidth - left - right,
                Height = Image.PixelHeight - top - bottom
            };
        }

        // Plotのエリアを描画
        public void DrawPlotArea() 
        {
            _renderer.DrawRectangle(MarginLeft, MarginTop, PlotAreaWidth, PlotAreaHeight, Colors.Black);
        }

        // 点を描画
        public void DrawPoint(int x, int y, Color? color = null, int r = 3) 
        {
            color ??= Colors.Black;
            _renderer.FillCircle(x, y, r, (Color)color);
        }

        // 線を描画
        public void DrawLine(int x0, int y0, int x1, int y1, Color? color=null)
        {
            color ??= Colors.Black;
            _renderer.DrawLine(x0, y0, x1, y1, (Color)color);
        }

        // X軸目盛を描画
        public void DrawTickX(int number=10) 
        {
            int width = PlotAreaWidth;

            for (int i = 0; i < number; i++)
            {
                int x0 = MarginLeft + ((width * i) / number);
                int x1 = MarginLeft + ((width * i) / number);
                int y0 = Image.PixelHeight - MarginBottom;
                int y1 = Image.PixelHeight - MarginBottom - 10;

                _renderer.DrawLine(x0, y0, x1, y1, Colors.Black);
            }
        }

        // Y軸目盛を描画
        public void DrawTickY(int number=10) 
        {
            int height = PlotAreaHeight;
   
            for (int i = 0; i < number; i++)
            {
                int x0 = MarginLeft;
                int x1 = MarginLeft + 10;
                int y0 = Height - MarginBottom - ((height * i) / number);
                int y1 = Height - MarginBottom - ((height * i) / number);

                _renderer.DrawLine(x0, y0, x1, y1, Colors.Black);
            }
        }

        // タイトルを描画
        public void DrawTitle(string title) 
        {
            int fontSize = 20;
            int x = MarginLeft + PlotAreaWidth / 2;
            int y = MarginTop - fontSize;

            _renderer.DrawText(x, y, title, new Typeface(""), fontSize, Colors.Black, "center", "bottom");
        }

        // X軸スケールを描画
        public void DrawScaleX(string[] scales) 
        {
            int number = scales.Length;
            int width = PlotAreaWidth;
            int fontSize = 18;

            for (int i = 0; i < number; i++)
            {
                int x = MarginLeft;
                if (number > 1)
                {
                    x = MarginLeft + ((width * i) / (number - 1));
                }
                //int y = Height - (int)(MarginBottom * 0.7);
                int y = MarginTop + PlotAreaHeight + fontSize;

                _renderer.DrawText(x, y, scales[i], new Typeface(""), fontSize, Colors.Black, "center", "top");
            }
        }

        // Y軸スケールを描画
        public void DrawScaleY(string[] scales)
        {
            int number = scales.Length;
            int height = PlotAreaHeight;
            int fontSize = 18;
 
            for (int i = 0; i < number; i++)
            {
                int x = MarginLeft - fontSize;
                int y = Image.PixelHeight - MarginBottom;
                if (number > 1)
                {
                    y = Image.PixelHeight - MarginBottom - (height * i) / (number - 1);
                }
                _renderer.DrawText(x, y, scales[i], new Typeface(""), fontSize, Colors.Black, "right", "center");
            }
        }

        // X軸方向にグリッドを描画
        public void DrawGridX(int number = 10)
        {
            int width = PlotAreaWidth;

            for (int i = 0; i < number; i++)
            {
                int x0 = MarginLeft + ((width * i) / number);
                int x1 = MarginLeft + ((width * i) / number);
                int y0 = Image.PixelHeight - MarginBottom;
                int y1 = MarginTop;
                _renderer.DrawLine(x0, y0, x1, y1, Colors.LightGray);
            }
        }

        // Y軸方向にグリッドを描画
        public void DrawGridY(int number=10)
        {
            int height = PlotAreaHeight;

            for (int i = 0; i < number; i++)
            {
                int x0 = MarginLeft;
                int x1 = Image.PixelWidth - MarginRight;
                int y0 = Image.PixelHeight - MarginBottom - ((height * i) / number);
                int y1 = Image.PixelHeight - MarginBottom - ((height * i) / number);

                _renderer.DrawLine(x0, y0, x1, y1, Colors.LightGray);
            }
        }

        // X軸単位を描画
        public void DrawUnitX(string unit)
        {
            int fontsize = 18;

            int x = MarginLeft + PlotAreaWidth + fontsize;
            int y = MarginTop + PlotAreaHeight + fontsize * 2;
            
            _renderer.DrawText(x, y, unit, new Typeface(""), fontsize, Colors.Black);
        }

        // Y軸単位を描画
        public void DrawUnitY(string unit)
        {
            int fontsize = 18;
            int x = MarginLeft - fontsize * 2;
            int y = MarginTop - fontsize * 2;

            _renderer.DrawText(x, y, unit, new Typeface(""), fontsize, Colors.Black, "right", "center");
        }

        // 縦のラインを描画
        public void DrawVirticalLine(int x, Color? color=null)
        {
            color ??= Colors.Red;
            int x0 = x;
            int y0 = MarginTop;
            int x1 = x;
            int y1 = MarginTop + PlotAreaHeight;

            _renderer.DrawLine(x0, y0, x1, y1, (Color)color);
        }

        // 横のラインを描画
        public void DrawHorizontalLine(int y, Color? color=null)
        {
            color ??= Colors.Red;
            int x0 = MarginLeft;
            int y0 = y;
            int x1 = MarginLeft + PlotAreaWidth;
            int y1 = y;

            _renderer.DrawLine(x0, y0, x1, y1, (Color)color);
        }

        // 任意の位置にテキストを描画
        public void DrawArbitraryText(int x, int y, string text, Color? color=null)
        {
            color ??= Colors.Black;
            int fontSize = 18;
            _renderer.DrawText(x, y, text, new Typeface(""), fontSize, (Color)color);
        }

        // 全体の色を描画
        public void FillBackColor(Color color)
        {
            _renderer.FillImage(color);
        }

        // プロットエリアの色を描画
        public void FillPlotAreaColor(Color color)
        {
            _renderer.FillRenderArea(color);
        }

    }
}
