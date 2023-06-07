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
using WAD.Doom;
using WAD.Doom.Bitmaps;
using WAD.Doom.Colors;
using WAD.Doom.Levels;
using WAD.Doom.Sounds;

using NAudio.Wave;
using System.Linq;

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

        readonly string wad_file = @"WADs\DOOM.WAD";
        readonly string wad_file_D1 = @"WADs\DOOM.WAD";
        readonly string wad_file_D2 = @"WADs\DOOM2.WAD";
        readonly string wad_file_HT = @"WADs\HERETIC.WAD";
        readonly string wad_file_HX = @"WADs\HEXEN.WAD";

        readonly WADReader rdr = null;

        IWavePlayer waveout = new WaveOut(WaveCallbackInfo.FunctionCallback());

        public MainWindow()
        {
            InitializeComponent();

            string file_path = "";

            file_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.wad_file_D1);
            WADReader rdr1 = new WADReader(file_path);

            file_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.wad_file_D2);
            WADReader rdr2 = new WADReader(file_path);

            file_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.wad_file_HT);
            WADReader rdrht = new WADReader(file_path);

            List<string> commons = new List<string>();
            List<string> rcommons = new List<string>();
            List<string> onlyD1 = new List<string>(rdr1.Entries.ListNames);
            List<string> onlyD2 = new List<string>(rdr2.Entries.ListNames);

            int cnt1 = onlyD1.Count;
            int cnt2 = onlyD2.Count;

            var dub1 = onlyD1.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());
            var dub2 = onlyD2.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());

            foreach (string idx in onlyD1)
            {
                if (onlyD2.Contains(idx))
                {
                    commons.Add(idx);
                }
            }
            foreach (string idx in onlyD2)
            {
                if (onlyD1.Contains(idx))
                {
                    rcommons.Add(idx);
                }
            }

            foreach (string idx in rcommons)
            {
                onlyD1.Remove(idx);
                onlyD2.Remove(idx);
            }

            // Cut execution here to avoid errors
            return;

            //lbl_screen.Content = rdr.Header.ToString();
            //list_screen.ItemsSource = rdr.Entries.ListToStrings();

            //list_maps.ItemsSource = rdr.Maps.names;
            //list_flats.ItemsSource = rdr.Images.Flats.Flats;
            //list_textures.ItemsSource = rdr.Images.Textures.Textures;
            //list_patches.ItemsSource = rdr.Images.Patches.Patches;
            //list_sprites.ItemsSource = rdr.Images.Sprites.Sprites;
            //list_sounds.ItemsSource = rdr.Audio.Samples.Samples;

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            waveout.Dispose();
        }

        private void list_maps_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string sel_value = list_maps.SelectedValue as string;

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
            MTexture sel_value = list_textures.SelectedValue as MTexture;

            StringBuilder sb = new StringBuilder();
            sb.Append("Texture name: ");
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

            foreach (MTexPatch texptc in sel_value.Patches)
            {
                MPatch Patch = texptc.Patch;
                MPicture pct = Patch.Picture;
                for (int x = 0; x < pct.Width; x++)
                {
                    MColumn col = pct.Columns[x];
                    while (col != null)
                    {
                        int y = (int)col.TopDelta;
                        int dx = x + texptc.OriginX;
                        int dy = y + texptc.OriginY;
                        int offset = (dx + dy * sel_value.Width) * 3;

                        for (int j = 0; j < col.Length; j++)
                        {
                            int ry = dy + j;
                            if ((dx >= 0) & (dx < sel_value.Width) & (ry >= 0) & (ry < sel_value.Height))
                            {
                                byte b = col.Data[j];
                                MColor mcol = colors[b];
                                pixels[offset + (j * sel_value.Width * 3)] = mcol.R;
                                pixels[offset + (j * sel_value.Width * 3) + 1] = mcol.G;
                                pixels[offset + (j * sel_value.Width * 3) + 2] = mcol.B;
                            }
                        }
                        col = col.Next;
                    }
                }
            }

            BitmapSource bitmapSource = BitmapSource.Create(sel_value.Width, sel_value.Height, 96, 96, PixelFormats.Rgb24, null, pixels, sel_value.Width * 3);
            panel_bitmap.Children.Clear();
            draw.image(bitmapSource, 0, 0, panel_bitmap);
        }

        private void list_patches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MPatch sel_value = list_patches.SelectedValue as MPatch;
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

        private void list_sprites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void list_screen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void list_sounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MSample sel_value = list_sounds.SelectedValue as MSample;

            StringBuilder sb = new StringBuilder();
            sb.Append("Sample: ");
            sb.AppendLine(sel_value.ToString());
            lbl_sound.Content = sb.ToString();

            WaveFormat waveFormat = new WaveFormat(sel_value.Rate, 8, 1);
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveFormat);
            bufferedWaveProvider.AddSamples(sel_value.Samples, 0, (int)sel_value.Length);
            waveout.Dispose();
            waveout.Init(bufferedWaveProvider);
            waveout.Play();
        }

        private void btn_sound_Click(object sender, RoutedEventArgs e)
        {
            MSample sel_value = list_sounds.SelectedValue as MSample;
            if (sel_value == null) return;

            WaveFormat waveFormat = new WaveFormat(sel_value.Rate, 8, 1);
            BufferedWaveProvider bufferedWaveProvider = new BufferedWaveProvider(waveFormat);
            bufferedWaveProvider.AddSamples(sel_value.Samples, 0, (int)sel_value.Length);
            waveout.Dispose();
            waveout.Init(bufferedWaveProvider);
            waveout.Play();
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
                draw.circle(v.X, v.Y, 2.0, panel);
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
        }

        public static void line(double ax, double ay, double bx, double by, Canvas cv, Brush color = null)
        {
            Brush stroke = color;
            if (color == null)
                stroke = Brushes.White;
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
