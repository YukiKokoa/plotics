using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Plotics
{
    internal class Renderer
    {
        // # プロパティ
        public WriteableBitmap Image { get; set; } // 全体画像
        public Int32Rect RenderArea { get; set; } // 描画エリア

        // # コンストラクタ
        // Bitmapを作成
        public Renderer(int width, int height)
        {
            Image = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            RenderArea = new Int32Rect(0, 0, width, height);
        }

        // # メソッド private
        // ピクセルを描画
        private unsafe void SetPixel(int x, int y, Color color, int thickness = 1)
        {
            // 描画内かをチェック
            if (x < 0 || y < 0 || x >= Image.PixelWidth || y >= Image.PixelHeight) return;
            if (x < RenderArea.X || y < RenderArea.Y || x > RenderArea.X + RenderArea.Width || y > RenderArea.Y + RenderArea.Height) return;

            IntPtr pBackBuffer = Image.BackBuffer;
            int stride = Image.BackBufferStride;

            int half = thickness / 2;
            for (int dy = -half; dy <= half; dy++)
            {
                for (int dx = -half; dx <= half; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    if (px >= 0 && py >= 0 && px < Image.PixelWidth && py < Image.PixelHeight)
                    {
                        int pixelOffset = py * stride + px * 4;
                        byte* pPixel = (byte*)pBackBuffer + pixelOffset;
                        pPixel[0] = color.B;
                        pPixel[1] = color.G;
                        pPixel[2] = color.R;
                        pPixel[3] = color.A;
                    }
                }
            }
        }

        // テキストbitmap
        private RenderTargetBitmap TextBitmap(string text, Typeface fontFace, double fontSize, Brush color)
        {
            text = string.IsNullOrEmpty(text) ? " " : text;

            // 描画の入れ物
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // フォントの形式
            FormattedText formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, fontFace, fontSize, Brushes.Black, 96);

            // 正しい色をセット
            formattedText.SetForegroundBrush(color);

            // 描画
            drawingContext.DrawText(formattedText, new Point(0, 0));
            drawingContext.Close();  // ← Closeは先に！

            // bitmapを作成
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(formattedText.Width), (int)Math.Ceiling(formattedText.Height), 96, 96, PixelFormats.Pbgra32);

            bitmap.Render(drawingVisual);
            return bitmap;
        }

        // # メソッド public
        // 描画を開始
        public void Begin()
        {
            Image.Lock();
        }

        // 描画を終了
        public void End()
        {
            Image.AddDirtyRect(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight));
            Image.Unlock();
        }

        // クリア
        // Image全体を塗りつぶし
        public void FillImage(Color? color=null)
        {
            color ??= new Color() { A=0, R=0, G=0, B=0}; // 透明な黒
            
            int width = Image.PixelWidth;
            int height = Image.PixelHeight;
            int[] pixels = new int[width * height];
            int rgb = (color.Value.A << 24) | (color.Value.R << 16) | (color.Value.G << 8) | color.Value.B; // ARGB

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = rgb;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            Image.WritePixels(rect, pixels, width * 4, 0);
        }

        // 描画エリアを塗りつぶし
        public void FillRenderArea(Color? color = null)
        {
            color ??= Colors.White;

            int width = Image.PixelWidth;
            int height = Image.PixelHeight;
            int[] pixels = new int[width * height];
            int rgb = (color.Value.A << 24) | (color.Value.R << 16) | (color.Value.G << 8) | color.Value.B; // ARGB

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = rgb;
            }

            Int32Rect rect = new Int32Rect(RenderArea.X, RenderArea.Y, RenderArea.Width, RenderArea.Height);
            Image.WritePixels(rect, pixels, width * 4, 0);
        }

        // リセット
        public void ResetImage()
        {
            int width = Image.PixelWidth;
            int height = Image.PixelHeight;
            double dpiX = Image.DpiX;
            double dpiY = Image.DpiY;
            PixelFormat pixelFormat = Image.Format;
            Image = new WriteableBitmap(width, height, dpiX, dpiY, pixelFormat, null);
        }

        // 直線を描画
        public void DrawLine(int x0, int y0, int x1, int y1, Color color, int thickness=1)
        {
            int dx = Math.Abs(x0 - x1);
            int dy = Math.Abs(y0 - y1);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;


            while(true)
            {
                SetPixel(x0, y0, color, thickness);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        // 円を描画
        public void DrawCircle(int x0, int y0, int r, Color color, int thickness=1)
        {
            int x = r;
            int y = 0;
            int err = 0;

            while (x >= y)
            {
                SetPixel(x0 + x, y0 + y, color, thickness);
                SetPixel(x0 + y, y0 + x, color, thickness);
                SetPixel(x0 - y, y0 + x, color, thickness);
                SetPixel(x0 - x, y0 + y, color, thickness);
                SetPixel(x0 - x, y0 - y, color, thickness);
                SetPixel(x0 - y, y0 - x, color, thickness);
                SetPixel(x0 + y, y0 - x, color, thickness);
                SetPixel(x0 + x, y0 - y, color, thickness);

                y++;

                if (err <= 0)
                {
                    err += 2 * y + 1;
                }
                else
                {
                    x--;
                    err += 2 * (y - x) + 1;
                }
            }

        }

        // 円を塗りつぶし描画
        public void FillCircle(int x0, int y0, int r, Color color)
        {
            // 最初に外円を描く
            DrawCircle(x0, y0, r, color);

            // 内部を塗りつぶす（円の内側の各スキャンライン）
            for (int y = -r; y <= r; y++)
            {
                int dx = (int)Math.Sqrt(r * r - y * y);  // そのy位置でのxの最大値
                for (int x = -dx; x <= dx; x++)
                {
                    SetPixel(x0 + x, y0 + y, color);  // 塗りつぶし
                }
            }
        }

        // 四角を描画
        public void DrawRectangle(int x0, int y0, int width, int height, Color color, int thickness=1)
        {
            DrawLine(x0, y0, x0 + width, y0, color, thickness);         // 上辺
            DrawLine(x0 + width, y0, x0 + width, y0 + height, color, thickness); // 右辺
            DrawLine(x0 + width, y0 + height, x0, y0 + height, color, thickness); // 下辺
            DrawLine(x0, y0 + height, x0, y0, color, thickness);        // 左辺
        }
        
        // 四角の塗りつぶし描画
        public void FillRectangle(int x0, int y0, int width, int height, Color color)
        {
            // 矩形の内部を塗りつぶす
            for (int y = y0; y < y0 + height; y++)
            {
                for (int x = x0; x < x0 + width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }

        // テキストを描画
        public void DrawText(int x, int y, string text, Typeface fontFace, double fontSize, Color? color, string hAlign="left", string vAlign="top")
        {
            color ??= Colors.Black;
            // テキストbitmapを作成
            RenderTargetBitmap textBitmap = TextBitmap(text, fontFace, fontSize, new SolidColorBrush((Color)color));
            int textWidth = textBitmap.PixelWidth;
            int textHeight = textBitmap.PixelHeight;
            int textStride = textWidth * (textBitmap.Format.BitsPerPixel / 8);
            byte[] textPixels = new byte[textStride * textHeight];
            textBitmap.CopyPixels(textPixels, textStride, 0);
           
            switch (hAlign.ToLower())
            {
                case "left":
                    break;

                case "right":
                    x = x - textWidth;
                    break;
                case "center":
                    x = x - textWidth / 2;
                    break;
            }

            switch (vAlign.ToLower())
            {
                case "top":
                    break;
                case "bottom":
                    y = y - textHeight;
                    break;
                case "center":
                    y = y - textHeight / 2;
                    break;
            }

            IntPtr backBuffer = Image.BackBuffer;
            int imageStride = Image.BackBufferStride;

            unsafe
            {
                byte* pBackBuffer = (byte*)backBuffer.ToPointer();

                for (int h = 0; h < textHeight; h++)
                {
                    for (int w = 0; w < textWidth; w++)
                    {
                        int textPixcelIndex = (h * textStride) + (w * 4);
                        byte a = textPixels[textPixcelIndex + 3]; // アルファ値

                        if (a > 0)
                        {
                            int imagePixelIndex = (h + y)* imageStride + (w + x)* 4;
                            pBackBuffer[imagePixelIndex + 0] = textPixels[textPixcelIndex + 0]; // B
                            pBackBuffer[imagePixelIndex + 1] = textPixels[textPixcelIndex + 1]; // G
                            pBackBuffer[imagePixelIndex + 2] = textPixels[textPixcelIndex + 2]; // R
                            pBackBuffer[imagePixelIndex + 3] = textPixels[textPixcelIndex + 3]; // A
                        }
                    }
                }
            }
            Image.AddDirtyRect(new Int32Rect(x, y, textWidth, textHeight));
          }

    }
}
