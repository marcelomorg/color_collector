using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Gma.System.MouseKeyHook;

namespace Color_Collector
{
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents globalHook; // Hook global do mouse
        private List<ColorDescription> colorPalette = new List<ColorDescription>(); // Paleta de cores

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public MainWindow()
        {
            InitializeComponent();
            StartGlobalMouseHook(); // Inicia o hook global
        }

        private void StartGlobalMouseHook()
        {
            globalHook = Hook.GlobalEvents(); // Cria o hook global

            // Registra o evento para clique do botão esquerdo do mouse
            globalHook.MouseDownExt += GlobalMouseDown;
        }

        private void GlobalMouseDown(object sender, MouseEventExtArgs e)
        {
            // Verifica se é o botão esquerdo do mouse
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Captura a posição atual do cursor
                GetCursorPos(out POINT point);

                // Captura a cor do pixel na posição do cursor
                using (var bitmap = new Bitmap(1, 1))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(point.X, point.Y, 0, 0, new System.Drawing.Size(1, 1));
                    }

                    System.Drawing.Color color = bitmap.GetPixel(0, 0); // Cor capturada
                    string colorInfo = $"RGB: {color.R}, {color.G}, {color.B} | HEX: #{color.R:X2}{color.G:X2}{color.B:X2}";

                    // Cria uma descrição da cor
                    var colorDescription = new ColorDescription
                    {
                        Description = colorInfo,
                        Color = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B)
                    };

                    // Adiciona a cor à paleta
                    colorPalette.Add(colorDescription);
                    ColorPalette.Items.Add(colorDescription);

                    // Atualiza o UI com a cor atual
                    ColorDisplay.Fill = new SolidColorBrush(colorDescription.Color);
                    ColorInfo.Text = colorInfo;
                }

                // Evita processar o clique várias vezes
                e.Handled = true;
            }
        }

        private void SavePalette_Click(object sender, RoutedEventArgs e)
        {
            string paletteName = PaletteName.Text;
            if (string.IsNullOrWhiteSpace(paletteName))
            {
                MessageBox.Show("Insira um nome para a paleta.");
                return;
            }

            // Atualiza o nome da paleta na Label
            PaletteNameLabel.Content = $"Paleta: {paletteName}";

            // Salva a paleta de cores em um arquivo
            List<string> colorStrings = new List<string>();
            foreach (var color in colorPalette)
            {
                colorStrings.Add(color.Description);
            }
            System.IO.File.WriteAllLines($"{paletteName}.txt", colorStrings);
            MessageBox.Show($"Paleta '{paletteName}' salva com sucesso!");
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            // Libera o hook global ao fechar a janela
            if (globalHook != null)
            {
                globalHook.Dispose();
            }
        }
    }

    // Classe para armazenar a descrição da cor e a cor propriamente dita
    public class ColorDescription
    {
        public string Description { get; set; }
        public System.Windows.Media.Color Color { get; set; }
    }
}
