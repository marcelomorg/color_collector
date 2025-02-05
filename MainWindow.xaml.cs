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

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public MainWindow()
        {
            InitializeComponent();

            // Criação do temporizador para capturar a cor a cada 100ms
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        // Evento para capturar a cor ao pressionar a tecla Enter
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CaptureColorUnderMouse(); // Captura a cor quando Enter é pressionado
            }
        }

        // Evento chamado a cada 100ms para capturar a cor do mouse
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isCapturingColor)
            {
                UpdateColorUnderMouse(); // Atualiza a cor do ponto sob o mouse em tempo real
            }
        }

        // Função para capturar a cor do ponto onde o mouse está
        private void CaptureColorUnderMouse()
        {
            // Obtém a posição do cursor
            GetCursorPos(out POINT point);

            // Captura o pixel da tela na posição do cursor
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(point.X, point.Y, 0, 0, new System.Drawing.Size(1, 1));
                }

                // Pega a cor do pixel
                System.Drawing.Color color = bitmap.GetPixel(0, 0);

                // Exibe a cor no Retângulo e as informações no TextBox
                ColorDisplay.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
                ColorInfo.Text = $"RGB: {color.R}, {color.G}, {color.B} | HEX: #{color.R:X2}{color.G:X2}{color.B:X2}";
            }
        }

        // Função para exibir a cor do ponto sob o mouse em tempo real
        private void UpdateColorUnderMouse()
        {
            // Obtém a posição do cursor
            GetCursorPos(out POINT point);

            // Captura o pixel da tela na posição do cursor
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(point.X, point.Y, 0, 0, new System.Drawing.Size(1, 1));
                }

                // Pega a cor do pixel
                System.Drawing.Color color = bitmap.GetPixel(0, 0);

                // Atualiza a cor no retângulo de exibição em tempo real
                ColorDisplay.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            }
        }

        // Inicia ou desativa a captura de cor ao pressionar Enter
        private void ToggleColorCapture(object sender, RoutedEventArgs e)
        {
            isCapturingColor = !isCapturingColor;

            if (isCapturingColor)
            {
                // Inicia o temporizador para capturar a cor do mouse
                timer.Start();
            }
            else
            {
                // Para o temporizador
                timer.Stop();
            }

            // Altera o cursor para indicar que a captura de cor está ativada
            Mouse.OverrideCursor = isCapturingColor ? Cursors.Cross : null;
        }

        // Quando a janela for fechada, para o temporizador
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            timer.Stop(); // Para o temporizador quando a janela for fechada
        }
    }
}
