using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WAD;
using WAD.WADFiles;

namespace IDEngine
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// https://doomwiki.org/wiki/Linedef
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string wad_path = @"C:\Developer\ProjectsB\Sharping\IDEngine";
        readonly string wad_file = @"WADs\DOOM.WAD";
        readonly WADReader rdr = null;

        public MainWindow()
        {
            InitializeComponent();
            string file_path = System.IO.Path.Combine(this.wad_path, this.wad_file);

            rdr = new WADReader(file_path);

            lbl_screen.Content = rdr.Header.ToString();
            list_screen.ItemsSource = rdr.Entries.ListToStrings();
            list_maps.ItemsSource = rdr.Maps.names;
        }

        private void list_maps_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string sel_value = list_maps.SelectedValue as string;
            Console.Write("Maps name: ");
            Console.WriteLine(sel_value);

            Map map = rdr.Maps.MapByName(sel_value);
            this.render(map);
        }

        private void render(Map map)
        {
            panel.Children.Clear();
            MVertexes vtx = map.Vertexes;
            MLines lnx = map.Lines;

            var dest = vtx.Scale_Vertexes(20, 20, panel.ActualWidth - 40, panel.ActualHeight - 40);
            draw.rect(20, 20, panel.ActualWidth - 40, panel.ActualHeight - 40, panel);


            list_points.ItemsSource = dest; // vtx.Vertexes;
            lbl_screen.Content = vtx.BBoxString();
            foreach (var v in dest)
            {
                draw.circle(v.X, v.Y, 3.0, panel);
            }

            foreach (var ln in lnx.Lines)
            {
                var a = dest[ln.StartVertex];
                var b = dest[ln.EndVertex];

                draw.line(a.X, a.Y, b.X, b.Y, panel);
            }
        }
    }

    internal class draw
    {
        public static void circle(double x, double y, double radius, Canvas cv)
        {
            double diameter = radius * 2;

            Ellipse circle = new Ellipse()
            {
                Width = diameter,
                Height = diameter,
                StrokeThickness = 1,
                Stroke = Brushes.Red
            };

            cv.Children.Add(circle);

            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);

            //circle.SetValue(Canvas.LeftProperty, x - radius);
            //circle.SetValue(Canvas.TopProperty, y - radius);
        }

        public static void line(double ax, double ay, double bx, double by, Canvas cv)
        {
            Line line = new Line()
            {
                X1 = ax,
                Y1 = ay,
                X2 = bx,
                Y2 = by,
                StrokeThickness = 1,
                Stroke = Brushes.Green
            };
            cv.Children.Add(line);
        }

        public static void rect(double ax, double ay, double dx, double dy, Canvas cv)
        {
            Rectangle rect = new Rectangle()
            {
                Width = dx,
                Height = dy,
                StrokeThickness = 1,
                Stroke = Brushes.White
            };
            cv.Children.Add(rect);

            Canvas.SetLeft(rect, ax);
            Canvas.SetTop(rect, ay);
        }
    }
}
