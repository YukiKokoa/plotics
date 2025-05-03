# Plotics

**Plotics** はWPFでグラフを表示するためのライブラリです。
散布図と折れ線図を作成できます。

---

## 特徴

- WPFの `Canvas` にグラフを表示します。 (イメージの形式は WriteableBitmapです。)
- 散布図と折れ線図を描きます。
- 対象は, NET 8.0 です 

---

## インストール
 
コードを直接プロジェクトにインポートしてください。
- Plotics.cs : 使用先で呼び出すクラス
- Renderer.cs : WriteableBitmapに簡単な図を描きます
- ChartRenderer.cs : Renderer を使ってグラフの要素を描きます
- AreaSetting.cs : 最大値や最小値などの軸設定など、グラフエリアの設定です。Ploticsクラスで使用します
- SeriesSetting.cs : 散布図または折れ線、マーカーなどのデータ系列描画の設定です。Ploticsクラスで使用します。
- PlotPoint.cs : 整数型のX,Yを与えるための構造体です。

---

## 使い方

 `Plotics`の使い方は下のようなです。:

```csharp

    int width = 800; int height = 800;
    Plotics.Plotics plotics = new(width, height);

    // グラフエリアの設定
    plotics.Area.Title = "Plotics Plot";
    plotics.Area.Maximum_X = 100.0;
    plotics.Area.Minimum_X = 0.0;
    plotics.Area.Maximum_Y = 10.0;
    plotics.Area.Minimum_Y = -10.0;
    plotics.Area.UnitX = "Sec";
    plotics.Area.UnitY = "V";
    plotics.Area.TickCountX = 5;
    plotics.Area.TickCountY = 4;

    plotics.Area.BackColor = Colors.LightGray;
    plotics.Area.PlotAreaColor = Colors.White;

    plotics.DrawArea(); // グラフエリアを描画

    // 点を描くための設定
    plotics.Series.PlotColor = Colors.Blue;
    plotics.Series.Style = SeriesSetting.PlotStyle.SCATTER;
    plotics.Series.Marker = SeriesSetting.MarkerStyle.CIRCLE;

    // 点を追加
    plotics.AddPoint(59, 1.4);
    plotics.AddPoint(63, 2.7);
    plotics.AddPoint(68, 3.2);
    plotics.AddPoint(72, 4.1);

    // 別の系列で点を描画したい場合
    SeriesSetting setting1 = new SeriesSetting();
    setting1.PlotColor = Colors.Red;
    setting1.Style = SeriesSetting.PlotStyle.LINE;
    List<double> x = new() {0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0, 100.0};
    List<double> y = new() {0.1, 2.3, 4.5, 6.7, 8.9, -0.1, -2.3, -4.5, -6.7, -8.9, 0.0};
    plotics.AddPoints(x, y, setting1);

    // 任意のラインとテキストを追加
    plotics.DrawHorizontalLine(3.8);
    plotics.DrawVirticalLine(54.7);

    plotics.DrawTextOnCodinate(54.7, 3.8, "Target");
    plotics.DrawTextOnPixel(plotics.TransformationXYtoPixcel(54.3, 0)[0], plotics.Image.PixelHeight - 100, "54.7", Colors.Red);

    // 画像をクリア
    //plottify.Clear();

    // WPFのCanvasで表示
    System.Windows.Controls.Image img = new();
    img.Source = plotics.Image;
    img.Height = plotics.Image.Height;
    img.Width = plotics.Image.Width;
    canvas.Children.Add(img);

    // ファイルに保存する例
    /*
    System.Windows.Media.Imaging.BmpBitmapEncoder encoder = new();
    encoder.Frames.Add(BitmapFrame.Create(plottify.Image));
    using (FileStream fs = new FileStream("plot.bmp", FileMode.Create))
    {
        encoder.Save(fs);
    }
    */
```

## ライセンス

MIT License です.

## 作成者

Yuki K. on 2025-05-03
