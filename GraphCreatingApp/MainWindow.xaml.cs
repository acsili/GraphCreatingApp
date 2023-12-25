using GraphCreatingApp.Graph;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphCreatingApp
{
    
    public partial class MainWindow : Window
    {
        List<(Ellipse Vertex, Point Point)> Vertices;
        List<(Line Edge, int V1, int V2)> Edges;


        int selected1 = -1;
        int selected2 = -1;

        const int R = 15;

        public MainWindow()
        {
            InitializeComponent();
            Vertices = new List<(Ellipse, Point)>();
            Edges = new List<(Line, int, int)>();
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (!CreateVertexButton.IsEnabled)
            {
                Vertices.Add((
                    new Ellipse
                    {
                        Height = 2 * R,
                        Width = 2 * R,
                        StrokeThickness = 3,
                        Stroke = Brushes.Black,
                        StrokeDashCap = PenLineCap.Round,

                        Fill = Brushes.DarkGray,
                    },
                    e.GetPosition(this)
                ));

                MainCanvas.Children.Add(Vertices[^1].Vertex);

                Canvas.SetLeft(Vertices[^1].Vertex, Vertices[^1].Point.X - R);
                Canvas.SetTop(Vertices[^1].Vertex, Vertices[^1].Point.Y - R);
            }

            if (!DeleteVertexButton.IsEnabled)
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    if (Math.Pow(Vertices[i].Point.X - e.GetPosition(this).X, 2) + Math.Pow(Vertices[i].Point.Y - e.GetPosition(this).Y, 2) <= R * R)
                    {
                        for (int j = 0; j < Edges.Count; j++)
                        {
                            if ((Edges[j].V1 == i) || (Edges[j].V2 == i))
                            {
                                MainCanvas.Children.Remove(Edges[j].Edge);
                                Edges.RemoveAt(j);
                                j--;
                            }
                            else
                            {
                                if (Edges[j].V1 > i) Edges[j] = (Edges[j].Edge, Edges[j].V1 - 1, Edges[j].V2);
                                if (Edges[j].V2 > i) Edges[j] = (Edges[j].Edge, Edges[j].V1, Edges[j].V2 - 1);
                            }
                        }
                        MainCanvas.Children.Remove(Vertices[i].Vertex);
                        Vertices.RemoveAt(i);
                        break;
                    }
                }
            }

            if (!CreateEdgeButton.IsEnabled)
            {
                for (int i = 0; i < Vertices.Count; i++)
                {
                    if (Math.Pow(Vertices[i].Point.X - e.GetPosition(this).X, 2) + Math.Pow(Vertices[i].Point.Y - e.GetPosition(this).Y, 2) <= R * R)
                    {
                        if (selected1 == -1)
                        {
                            selected1 = i;
                            break;
                        }
                        if (selected2 == -1)
                        {

                            selected2 = i;
                            Edges.Add((
                                new Line
                                {
                                    Stroke = Brushes.Black,
                                    StrokeThickness = 5,
                                    X1 = Vertices[selected1].Point.X,
                                    Y1 = Vertices[selected1].Point.Y,
                                    X2 = Vertices[selected2].Point.X,
                                    Y2 = Vertices[selected2].Point.Y,
                                },

                                selected1,
                                selected2
                            ));


                            MainCanvas.Children.Add(Edges[^1].Edge);

                            selected1 = -1;
                            selected2 = -1;

                            break;
                        }
                    }
                }
            }


            if (!DeleteEdgeButton.IsEnabled)
            {
                for (int i = 0; i < Edges.Count; i++)
                {
                    if (((e.GetPosition(this).X - Vertices[Edges[i].V1].Point.X) * (Vertices[Edges[i].V2].Point.Y - Vertices[Edges[i].V1].Point.Y) / (Vertices[Edges[i].V2].Point.X - Vertices[Edges[i].V1].Point.X) + Vertices[Edges[i].V1].Point.Y) <= (e.GetPosition(this).Y + 4) &&
                                ((e.GetPosition(this).X - Vertices[Edges[i].V1].Point.X) * (Vertices[Edges[i].V2].Point.Y - Vertices[Edges[i].V1].Point.Y) / (Vertices[Edges[i].V2].Point.X - Vertices[Edges[i].V1].Point.X) + Vertices[Edges[i].V1].Point.Y) >= (e.GetPosition(this).Y - 4))
                    {
                        if ((Vertices[Edges[i].V1].Point.X <= Vertices[Edges[i].V2].Point.X && Vertices[Edges[i].V1].Point.X <= e.GetPosition(this).X && e.GetPosition(this).X <= Vertices[Edges[i].V2].Point.X) ||
                            (Vertices[Edges[i].V1].Point.X >= Vertices[Edges[i].V2].Point.X && Vertices[Edges[i].V1].Point.X >= e.GetPosition(this).X && e.GetPosition(this).X >= Vertices[Edges[i].V2].Point.X))
                        {
                            MainCanvas.Children.Remove(Edges[i].Edge);
                            Edges.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVertexButton.IsEnabled = false;
            CreateEdgeButton.IsEnabled = true;
            DeleteVertexButton.IsEnabled = true;
            DeleteEdgeButton.IsEnabled = true;
            ChromaticIndexButton.IsEnabled = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVertexButton.IsEnabled = true;
            CreateEdgeButton.IsEnabled = true;
            DeleteVertexButton.IsEnabled = false;
            DeleteEdgeButton.IsEnabled = true;
            ChromaticIndexButton.IsEnabled = true;
        }

        private void CreateEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVertexButton.IsEnabled = true;
            CreateEdgeButton.IsEnabled = false;
            DeleteVertexButton.IsEnabled = true;
            DeleteEdgeButton.IsEnabled = true;
            ChromaticIndexButton.IsEnabled = true;
        }

        private void DeleteEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            CreateVertexButton.IsEnabled = true;
            CreateEdgeButton.IsEnabled = true;
            DeleteVertexButton.IsEnabled = true;
            DeleteEdgeButton.IsEnabled = false;
            ChromaticIndexButton.IsEnabled = true;

        }


        List<SolidColorBrush> brushes= new List<SolidColorBrush>()
        {
            Brushes.LightPink,
            Brushes.DarkKhaki,
            Brushes.DarkGreen,
            Brushes.Fuchsia,
            Brushes.DimGray,
            Brushes.Firebrick,
            Brushes.Magenta,
            Brushes.White,
            Brushes.LightGreen,
            Brushes.LightBlue,
            Brushes.AliceBlue,
            Brushes.Aqua,
            Brushes.Aquamarine,
            Brushes.Blue,
            Brushes.Orchid,
            Brushes.Azure,
            Brushes.Brown,
            Brushes.BurlyWood,
            Brushes.Cornsilk,
            Brushes.Cyan,
            Brushes.DarkBlue,
            Brushes.DarkCyan,
            Brushes.DarkGoldenrod
        };

        private void ChromaticIndexButton_Click(object sender, RoutedEventArgs e)
        {

            List<Edge> edges = new List<Edge>();
            edges = Edges.Select(e =>
                new Edge
                {
                    FirstVertex = ++e.V1,
                    SecondVertex = ++e.V2
                }
            ).ToList();

            var chromaticIndex = new ChromaticIndex();

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"\nХроматический индекс данного графа равен: {chromaticIndex.FindChromaticIndex(edges, edges.Count)} ");

            for (int i = 0; i < edges.Count; i++)
                stringBuilder.Append($"\nЦвет ребра между вершинами {edges[i].FirstVertex} и {edges[i].SecondVertex} это: цвет C{edges[i].Color}.");

            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < brushes.Count; j++)
                {
                    if (edges[i].Color == j)
                    {
                        Edges[i].Edge.Stroke = brushes[j];
                        break;
                    }

                }
            }


            MessageBox.Show(stringBuilder.ToString());


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveGraph saveGraphs = new SaveGraph();
            saveGraphs.Vertex = Vertices.Select(e => 
            new VertexInfo
            {
                VertexX = e.Point.X,
                VertexY = e.Point.Y,

            }).ToList();
            saveGraphs.Edge = Edges.Select(e =>
            new EdgeInfo
            {
                V1 = e.V1,
                V2 = e.V2,

            }).ToList();

            var fileName = string.Empty;
            var sd = new SaveFileDialog
            {
                DefaultExt = "json"
            };
            if (sd.ShowDialog() == true)
            {
                fileName = sd.FileName;
            }

            if (fileName == string.Empty)
                return;
            var fs = File.Open(fileName, FileMode.Create);

            var json = JsonSerializer.Serialize(saveGraphs);
            var info = new UTF8Encoding(true).GetBytes(json);
            fs.Write(info, 0, info.Length);
            fs.Close();

        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog
            {
                Filter = "*.json|*.json"
            };
            if (fd.ShowDialog() == false)
            {
                MessageBox.Show("Файл не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var file = fd.FileName;
            FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
            var list = JsonSerializer.Deserialize<SaveGraph>(fs);
            if (list == null)
                return;
            

            Vertices = new List<(Ellipse, Point)>();
            Edges = new List<(Line, int, int)>();

            Vertices = list.Vertex.Select(e =>
            (
                new Ellipse
                {
                    Height = 2 * R,
                    Width = 2 * R,
                    StrokeThickness = 3,
                    Stroke = Brushes.Black,
                    StrokeDashCap = PenLineCap.Round,

                    Fill = Brushes.DarkGray,
                },
                new Point(e.VertexX, e.VertexY)
            )).ToList();

            Edges = list.Edge.Select(e =>
            (
                new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 5,
                    X1 = Vertices[e.V1].Point.X,
                    Y1 = Vertices[e.V1].Point.Y,
                    X2 = Vertices[e.V2].Point.X,
                    Y2 = Vertices[e.V2].Point.Y,
                },

                e.V1,
                e.V2
            )).ToList();

            MainCanvas.Children.Clear();

            foreach (var vertex in Vertices)
            {
                MainCanvas.Children.Add(vertex.Vertex);

                Canvas.SetLeft(vertex.Vertex, vertex.Point.X - R);
                Canvas.SetTop(vertex.Vertex, vertex.Point.Y - R);
            }

            foreach (var edge in Edges)
                MainCanvas.Children.Add(edge.Edge);

            fs.Close();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программное обеспечение для визуализуального представления графов\n"
                + "Автор: Овчинников И. С.\n"
                + "Возможности:\n"
                + "1. Добавление вершин\n"
                + "2. Удаление вершин\n"
                + "3. Добавление ребер\n"
                + "4. Удаление ребер\n"
                + "5. Вычисление хроматического индекса графа\n"
                + "6. Сохранение графа в формате json\n"
                + "7. Открытие сохраненного графа\n",
                "Справочная информация");
        }
    }
}
