using System;
using System.Drawing; // Necessário para manipulação de imagem
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Color_Collector
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private bool isCapturingColor = false;
        private ObservableCollection<SavedColor> savedColors;

        public MainWindow()
        {
            InitializeComponent();

            savedColors = new ObservableCollection<SavedColor>();
            ColorPalette.ItemsSource = savedColors;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void CaptureColorUnderMouse()
        {
            GetCursorPos(out POINT point);

            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(point.X, point.Y, 0, 0, new System.Drawing.Size(1, 1));
                }

                System.Drawing.Color color = bitmap.GetPixel(0, 0);
                var mediaColor = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                ColorDisplay.Fill = new SolidColorBrush(mediaColor);
                ColorInfo.Text = $"RGB: {color.R}, {color.G}, {color.B} | HEX: #{color.R:X2}{color.G:X2}{color.B:X2}";

                // Adiciona a cor capturada à lista
                savedColors.Add(new SavedColor
                {
                    Description = $"Cor {savedColors.Count + 1}",
                    Color = mediaColor,
                    HexCode = $"#{color.R:X2}{color.G:X2}{color.B:X2}"
                });
            }
        }

        private void UpdateColorUnderMouse()
        {
            GetCursorPos(out POINT point);

            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(point.X, point.Y, 0, 0, new System.Drawing.Size(1, 1));
                }

                System.Drawing.Color color = bitmap.GetPixel(0, 0);
                ColorDisplay.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            }
        }

        private void ToggleColorCapture(object sender, RoutedEventArgs e)
        {
            isCapturingColor = !isCapturingColor;

            if (isCapturingColor)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }

            Mouse.OverrideCursor = isCapturingColor ? Cursors.Cross : null;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer.Stop();
}
