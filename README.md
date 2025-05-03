# Plotics

**Plotics** is a charting library for WPF written in C#.  
It supports rendering scatter plots and line graphs.

---

## Features

- Simple plot rendering using `Canvas` (using WriteableBitmap)
- Support for scatter and line graphs
- Designed for integration as a DLL in other WPF apps
- .NET 8.0 compatible

---

## Installation
 
Clone the repository and reference the project directly in your WPF solution.

---

## Getting Started

Here's a basic example of how to use `Plotics`:

```csharp

    int width = 800; int height = 800;
    Plotics.Plotics plotics = new(width, height);

    // Set Area
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

    plotics.DrawArea();

    // Set Series
    plotics.Series.PlotColor = Colors.Blue;
    plotics.Series.Style = SeriesSetting.PlotStyle.SCATTER;
    plotics.Series.Marker = SeriesSetting.MarkerStyle.CIRCLE;

    // Set Points
    plotics.AddPoint(59, 1.4);
    plotics.AddPoint(63, 2.7);
    plotics.AddPoint(68, 3.2);
    plotics.AddPoint(72, 4.1);

    // Another Series and Points
    SeriesSetting setting1 = new SeriesSetting();
    setting1.PlotColor = Colors.Red;
    setting1.Style = SeriesSetting.PlotStyle.LINE;
    List<double> x = new() {0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0, 100.0};
    List<double> y = new() {0.1, 2.3, 4.5, 6.7, 8.9, -0.1, -2.3, -4.5, -6.7, -8.9, 0.0};
    plotics.AddPoints(x, y, setting1);

    // Set Line
    plotics.DrawHorizontalLine(3.8);
    plotics.DrawVirticalLine(54.7);

    plotics.DrawTextOnCodinate(54.7, 3.8, "Target");
    plotics.DrawTextOnPixel(plotics.TransformationXYtoPixcel(54.3, 0)[0], plotics.Image.PixelHeight - 100, "54.7", Colors.Red);

    //plottify.Clear();

    // Draw to Canvas
    System.Windows.Controls.Image img = new();
    img.Source = plotics.Image;
    img.Height = plotics.Image.Height;
    img.Width = plotics.Image.Width;
    canvas.Children.Add(img);

    // Save as File
    /*
    System.Windows.Media.Imaging.BmpBitmapEncoder encoder = new();
    encoder.Frames.Add(BitmapFrame.Create(plottify.Image));
    using (FileStream fs = new FileStream("plot.bmp", FileMode.Create))
    {
        encoder.Save(fs);
    }
    */
```
## License

This project is licensed under the MIT License.

## Author

Created by Yuki K. on 2025-05-03
