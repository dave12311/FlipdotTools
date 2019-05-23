using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlipdotTools {
    public partial class MainWindow : Window {
        public DataMatrix dataMatrix = new DataMatrix();
        public MainWindow() {
            InitializeComponent();
            for (int s = 0; s < 24; s++) {
                System.Windows.Controls.StackPanel stackPanel = new StackPanel {
                    Height = 210,
                    Width = 30,
                    Orientation = Orientation.Vertical,
                    Tag = s
                };
                MainPanel.Children.Add(stackPanel);
                for (int b = 0; b < 7; b++) {
                    System.Windows.Shapes.Rectangle rectangle = new Rectangle {
                        Height = 30,
                        Width = 30,
                        Tag = b,
                        Fill = Brushes.Black,
                        Stroke = Brushes.Gray
                    };
                    rectangle.MouseDown += Pixel_Click;
                    rectangle.MouseEnter += Pixel_Click;
                    rectangle.MouseLeave += Pixel_Mouse_Leave;
                    dataMatrix.rectangles.Add(rectangle);
                    stackPanel.Children.Add(rectangle);
                }
            }
        }

        private int Decode(char letter) {
            byte num;
            byte[] bytes = Encoding.UTF8.GetBytes(letter.ToString());
            num = bytes[0];
            if (num >= 65 && num <= 90) {
                return num - 65;
            } else if (num >= 48 && num <= 57) {
                return num + 57;
            } else {
                switch (bytes[0]) {
                    case 46:
                        return 115;
                    case 63:
                        return 116;
                    case 33:
                        return 117;
                    case 44:
                        return 118;
                    case 58:
                        return 119;
                    case 40:
                        return 120;
                    case 41:
                        return 121;
                    case 38:
                        return 122;
                    case 47:
                        return 123;
                    case 43:
                        return 124;
                    case 45:
                        return 125;
                    case 61:
                        return 126;
                    case 37:
                        return 127;
                    case 42:
                        return 129;
                    case 64:
                        return 130;
                }
                switch (bytes[1]) {
                    case 129:
                        return 26;
                    case 137:
                        return 27;
                    case 141:
                        return 28;
                    case 147:
                        return 29;
                    case 150:
                        return 30;
                    case 144:
                        return 31;
                    case 154:
                        return 32;
                    case 156:
                        return 33;
                    case 176:
                        return 34;
                }
                return 0;
            }
        }

        public class DataMatrix {
            private bool[,] data = new bool[24, 7];
            private int leftMost;
            private int rightMost;

            public List<Rectangle> rectangles = new List<Rectangle>();

            public int LeftMost { get => leftMost; }
            public int RightMost { get => rightMost; }
            public bool[,] Data { get => data; set => data = value; }

            public void GetEdges() {
                int left = 0;
                int right = 0;
                for (int x = 0; x < 24; x++) {
                    for (int y = 0; y < 7; y++) {
                        if (data[x, y] == true) {
                            left = x;
                            goto Next;
                        }
                    }
                }
            Next:
                for (int x = 23; x >= 0; x--) {
                    for (int y = 0; y < 7; y++) {
                        if (data[x, y] == true) {
                            right = x;
                            goto End;
                        }
                    }
                }
            End:
                leftMost = left;
                rightMost = right;
            }
            public void Clear() {
                for (int x = 0; x < 24; x++) {
                    for (int y = 0; y < 7; y++) {
                        data[x, y] = false;
                    }
                }
            }
        }

        private void Pixel_Click(object sender, RoutedEventArgs e) {
            Rectangle rectangle = (Rectangle)e.Source;
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                int X = (int)(rectangle.Parent as StackPanel).Tag;
                int Y = (int)rectangle.Tag;
                string Binary = "NULL";

                //Change button color, store data
                if (dataMatrix.Data[X, Y] == false) {
                    rectangle.Fill = Brushes.GreenYellow;
                    dataMatrix.Data[X, Y] = true;
                } else {
                    rectangle.Fill = Brushes.Black;
                    dataMatrix.Data[X, Y] = false;
                }

                dataMatrix.GetEdges();

                for (int x = dataMatrix.LeftMost; x <= dataMatrix.RightMost; x++) {
                    if (Binary == "NULL") {
                        Binary = "0b1";
                    } else {
                        Binary += ", 0b0";
                    }

                    for (int y = 0; y < 7; y++) {
                        if (dataMatrix.Data[x, y] == true) {
                            Binary += 1;
                        } else {
                            Binary += 0;
                        }
                    }
                }

                Binary += ",";

                TextOut.Text = Binary;
            } else {
                rectangle.Fill = Brushes.Gray;
            }
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e) {
            dataMatrix.Clear();
            foreach (var rectangle in dataMatrix.rectangles) {
                rectangle.Fill = Brushes.Black;
            }
            TextOut.Text = "Click on a pixel to begin";
        }

        private void Button_Clipboard_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(TextOut.Text);
            string temp = TextOut.Text;
            TextOut.Text = "COPIED TO CLIPBOARD";

        }

        private void Pixel_Mouse_Leave(object sender, RoutedEventArgs e) {
            Rectangle rectangle = (Rectangle)e.Source;
            int X = (int)(rectangle.Parent as StackPanel).Tag;
            int Y = (int)rectangle.Tag;

            if (dataMatrix.Data[X, Y] == true) {
                rectangle.Fill = Brushes.GreenYellow;
            } else {
                rectangle.Fill = Brushes.Black;
            }
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e) {
            string Code;
            if (InputBox.Text.Length > 0) {
                Code = "{";
                foreach (char c in InputBox.Text.ToCharArray()) {
                    Code += (Decode(c)+1).ToString() + ",";
                }
                Code += "0}";
                OutputBox.Text = Code;
            } else {
                OutputBox.Text = "Start typing to begin";
            }

        }
    }
}
