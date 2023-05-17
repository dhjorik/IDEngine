using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WAD;
using WAD.WADFiles;
using WAD.WADFiles.Bitmaps;
using WAD.WADFiles.Colors;
using WAD.WADFiles.Levels;

namespace IDEngine
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// https://doomwiki.org/
    /// https://oldgamesdownload.com/duke-nukem-3d/
    /// https://en.wikipedia.org/wiki/Quake_engine
    /// http://www.wolfensteingoodies.com/archives/olddoom/music.htm
    /// 
    public partial class MainWindow : Window
    {
        uint current_palette = 0;
        uint current_map = 0;

        // readonly string wad_path = @"C:\Developer\ProjectsB\Sharping\IDEngine";
        // readonly string wad_path = @"C:\Pythons\projects\sharps\IDEngine";
        readonly string wad_file = @"WADs\DOOM.WAD";
        readonly WADReader rdr = null;

        public MainWindow()
        {
            InitializeComponent();

            //Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            //Console.WriteLine(System.Environment.CurrentDirectory);
            //Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            //Console.WriteLine(Environment.CurrentDirectory);

            string file_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.wad_file);

            rdr = new WADReader(file_path);

            lbl_screen.Content = rdr.Header.ToString();
            list_screen.ItemsSource = rdr.Entries.ListToStrings();

            list_maps.ItemsSource = rdr.Maps.names;
            list_flats.ItemsSource = rdr.Images.Flats.Flats;
            list_textures.ItemsSource = rdr.Images.Textures.Textures;
            list_patches.ItemsSource = rdr.Images.Patches.Patches;
            list_sprites.ItemsSource = rdr.Images.Sprites.Sprites;
        }

        private void list_maps_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string sel_value = list_maps.SelectedValue as string;
            Console.Write("Maps name: ");
            Console.WriteLine(sel_value);

            Map map = rdr.Maps.MapByName(sel_value);
            this.render(map);
        }

        private void list_flats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MFlat sel_value = list_flats.SelectedValue as MFlat;

            StringBuilder sb = new StringBuilder();
            sb.Append("Flat name: ");
            sb.AppendLine(sel_value.ToString());
            lbl_screen.Content = sb.ToString();

            var pal = rdr.Palettes.GetPalette(0, 0);

            List<MColor> colors = new List<MColor>();
            for (int i = 0; i < 256; i++)
            {
                MColor mcol = pal[i];
                colors.Add(mcol);
            }

            byte[] pixels = new byte[sel_value.Length * 3];
            uint offset = 0;
            for (uint i = 0; i < sel_value.Length; i++)
            {
                byte b = sel_value.Data[i];
                MColor mcol = colors[b];
                pixels[offset] = mcol.R;
                pixels[offset + 1] = mcol.G;
                pixels[offset + 2] = mcol.B;
                offset += 3;
            }

            BitmapSource bitmapSource = BitmapSource.Create(sel_value.Width, sel_value.Height, 96, 96, PixelFormats.Rgb24, null, pixels, sel_value.Width * 3);

            panel_bitmap.Children.Clear();
            draw.image(bitmapSource, 0, 0, panel_bitmap);
        }

        private void list_textures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MPatch sel_value = list_textures.SelectedValue as MPatch;
            MPicture pct = sel_value.Picture;

            StringBuilder sb = new StringBuilder();
            sb.Append("Patch name: ");
            sb.Append(pct.ToString());
            sb.Append(" - ");
            sb.AppendLine(sel_value.ToString());
            lbl_screen.Content = sb.ToString();

            var pal = rdr.Palettes.GetPalette(0, 0);

            List<MColor> colors = new List<MColor>();
            for (int i = 0; i < 256; i++)
            {
                MColor mcol = pal[i];
                colors.Add(mcol);
            }


            byte[] pixels = new byte[pct.Length * 3];
            for (int x = 0; x < pct.Width; x++)
            {
                MColumn col = pct.Columns[x];
                while (col != null)
                {
                    int y = (int)col.TopDelta;
                    int offset = (x + y * pct.Width) * 3;

                    for (int j = 0; j < col.Length; j++)
                    {
                        byte b = col.Data[j];
                        MColor mcol = colors[b];
                        pixels[offset + (j * pct.Width * 3)] = mcol.R;
                        pixels[offset + (j * pct.Width * 3) + 1] = mcol.G;
                        pixels[offset + (j * pct.Width * 3) + 2] = mcol.B;
                    }
                    col = col.Next;
                }
            }

            BitmapSource bitmapSource = BitmapSource.Create(pct.Width, pct.Height, 96, 96, PixelFormats.Rgb24, null, pixels, pct.Width * 3);
            panel_bitmap.Children.Clear();
            draw.image(bitmapSource, 0, 0, panel_bitmap);
        }

        private void list_patches_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void list_sprites_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            MSprite sel_value = list_sprites.SelectedValue as MSprite;
            MPicture pct = sel_value.Picture;

            StringBuilder sb = new StringBuilder();
            sb.Append("Sprite name: ");
            sb.Append(pct.ToString());
            sb.Append(" - ");
            sb.AppendLine(sel_value.ToString());
            lbl_screen.Content = sb.ToString();

            var pal = rdr.Palettes.GetPalette(0, 0);

            List<MColor> colors = new List<MColor>();
            for (int i = 0; i < 256; i++)
            {
                MColor mcol = pal[i];
                colors.Add(mcol);
            }


            byte[] pixels = new byte[pct.Length * 3];
            for (int x = 0; x < pct.Width; x++)
            {
                MColumn col = pct.Columns[x];
                while (col != null)
                {
                    int y = (int)col.TopDelta;
                    int offset = (x + y * pct.Width) * 3;

                    for (int j = 0; j < col.Length; j++)
                    {
                        byte b = col.Data[j];
                        MColor mcol = colors[b];
                        pixels[offset + (j * pct.Width * 3)] = mcol.R;
                        pixels[offset + (j * pct.Width * 3) + 1] = mcol.G;
                        pixels[offset + (j * pct.Width * 3) + 2] = mcol.B;
                    }
                    col = col.Next;
                }
            }

            BitmapSource bitmapSource = BitmapSource.Create(pct.Width, pct.Height, 96, 96, PixelFormats.Rgb24, null, pixels, pct.Width * 3);
            panel_bitmap.Children.Clear();
            draw.image(bitmapSource, 0, 0, panel_bitmap);
        }

        private void render_palette(uint palette, uint colormap)
        {
            panel_palette.Children.Clear();

            double x_size = panel_palette.ActualWidth / 16;
            double y_size = panel_palette.ActualHeight / 16;

            var pal = rdr.Palettes.GetPalette((byte)palette, (byte)colormap);

            for (int i = 0; i < 256; i++)
            {
                MColor mcol = pal[i];
                Brush brush = new SolidColorBrush(Color.FromArgb(255, mcol.R, mcol.G, mcol.B));

                int col = i % 16;
                int row = i / 16;

                double x = x_size * col;
                double y = y_size * row;

                draw.fillrect(x, y, x_size, y_size, panel_palette, brush);
            }
        }

        private void render(Map map)
        {
            panel.Children.Clear();
            MVertexes vtx = map.Vertexes;
            MLines lnx = map.Lines;

            var dest = vtx.Scale_Vertexes(20, 20, panel.ActualWidth - 40, panel.ActualHeight - 40);
            draw.rect(20, 20, panel.ActualWidth - 40, panel.ActualHeight - 40, panel);

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

        private void panel_palette_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lbl_screen.Content = "Palette: " + current_palette.ToString() + " - Color Map: " + current_map.ToString();
            this.render_palette(current_palette, current_map);
            current_palette++;
            if (current_palette >= rdr.Palettes.PlayPal.Palettes.Count)
            {
                current_palette = 0;
            }
        }

        private void panel_colormap_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lbl_screen.Content = "Palette: " + current_palette.ToString() + " - Color Map: " + current_map.ToString();
            this.render_palette(current_palette, current_map);
            current_map++;
            if (current_map >= rdr.Palettes.ColorMaps.ColorMaps.Count)
            {
                current_map = 0;
            }
        }
    }

    internal class draw
    {
        public static void circle(double x, double y, double radius, Canvas cv, Brush color = null)
        {
            double diameter = radius * 2;

            Brush stroke = color;
            if (color == null)
                stroke = Brushes.Red;

            Ellipse circle = new Ellipse()
            {
                Width = diameter,
                Height = diameter,
                StrokeThickness = 1,
                Stroke = stroke
            };

            cv.Children.Add(circle);

            Canvas.SetLeft(circle, x - radius);
            Canvas.SetTop(circle, y - radius);

            //circle.SetValue(Canvas.LeftProperty, x - radius);
            //circle.SetValue(Canvas.TopProperty, y - radius);
        }

        public static void line(double ax, double ay, double bx, double by, Canvas cv, Brush color = null)
        {
            Brush stroke = color;
            if (color == null)
                stroke = Brushes.LightSeaGreen;
            Line line = new Line()
            {
                X1 = ax,
                Y1 = ay,
                X2 = bx,
                Y2 = by,
                StrokeThickness = 1,
                Stroke = stroke
            };
            cv.Children.Add(line);
        }

        public static void fillrect(double ax, double ay, double dx, double dy, Canvas cv, Brush color = null)
        {
            Brush stroke = color;
            if (color == null)
                stroke = Brushes.LightSkyBlue;

            Rectangle rect = new Rectangle()
            {
                Width = dx,
                Height = dy,
                StrokeThickness = 1,
                Fill = stroke,
                Stroke = stroke,
            };
            cv.Children.Add(rect);

            Canvas.SetLeft(rect, ax);
            Canvas.SetTop(rect, ay);
        }
        public static void rect(double ax, double ay, double dx, double dy, Canvas cv, Brush color = null)
        {
            Brush stroke = color;
            if (color == null)
                stroke = Brushes.White;

            Rectangle rect = new Rectangle()
            {
                Width = dx,
                Height = dy,
                StrokeThickness = 1,
                Stroke = stroke
            };
            cv.Children.Add(rect);

            Canvas.SetLeft(rect, ax);
            Canvas.SetTop(rect, ay);
        }

        public static void image(BitmapSource bm, double ax, double ay, Canvas cv)
        {
            Image img = new Image();
            img.Source = bm;

            cv.Children.Add(img);

            Canvas.SetLeft(img, ax);
            Canvas.SetTop(img, ay);
        }
    }
}
