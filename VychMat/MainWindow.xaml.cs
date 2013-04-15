using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;

namespace VychMat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Label> _pointNumberLabels;
        private List<TextBox> _pointXBoxes;
        private List<TextBox> _pointYBoxes;
        private List<Label> _cLabels; 
        private double?[] _xValues;
        private double?[] _yValues;
        private int _currentMargin = 20;
        private int _number;
        private const int PlotPointsNumber = 30;
        private const double PointXStep = 20/PlotPointsNumber;
        public static int PointsNumber { get; set; }

        private void PlaceElementGroup()
        {
            _pointNumberLabels.Add(new Label
                                       {
                                           Content = _number.ToString(),
                                           Margin = new Thickness(_currentMargin, 0, 0, 0),
                                           Height = 25,
                                           Width = 25,
                                           HorizontalAlignment = HorizontalAlignment.Left,
                                           VerticalAlignment = VerticalAlignment.Top

                                       });
            var tb = new TextBox
                         {
                             Margin = new Thickness(_currentMargin, 35, 0, 0),
                             Height = 25,
                             Width = 50,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };
            tb.TextChanged += TextBoxTextChanged;
            _pointXBoxes.Add(tb);
            tb = new TextBox
                     {
                         Margin = new Thickness(_currentMargin, 70, 0, 0),
                         Height = 25,
                         Width = 50,
                         HorizontalAlignment = HorizontalAlignment.Left,
                         VerticalAlignment = VerticalAlignment.Top
                     };
            tb.TextChanged += TextBoxTextChanged;
            _pointYBoxes.Add(tb);
            var lb = new Label
                         {
                             Margin = new Thickness(_currentMargin-10, 105, 0, 0),
                             Height = 25,
                             Width = 75,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top,
                             Content="",
                             //FontSize = 14,
                             FontWeight = FontWeights.Bold

                         };
            _cLabels.Add(lb);
            _currentMargin += 60;
            _number++;
        }

        private void InitializeLists()
        {
            _pointNumberLabels = new List<Label>();
            _pointXBoxes = new List<TextBox>();
            _pointYBoxes = new List<TextBox>();
            _cLabels = new List<Label>();
            Chart.Diagram.Series.Add(new LineSeries2D());
            Chart.Diagram.Series[0].DisplayName = "f(x)";
            Chart.Diagram.Series[0].LabelsVisibility = true;
            Chart.Diagram.Series.Add(new LineSeries2D());
            Chart.Diagram.Series[1].DisplayName = "φ(x)";
        }

        private void RemoveAllElements()
        {
            for (var i = 0; i < PointsNumber; i++)
            {
                PointsGrid.Children.Remove(_pointNumberLabels[i]);
                PointsGrid.Children.Remove(_pointXBoxes[i]);
                PointsGrid.Children.Remove(_pointYBoxes[i]);
                PointsGrid.Children.Remove(_cLabels[i]);
            }
        }

        private void PlaceElements()
        {
            _currentMargin = 20;
            _number = 0;
            InitializeLists();
            _xValues = new double?[PointsNumber];
            _yValues = new double?[PointsNumber];
            for (var i = 0; i < PointsNumber; i++)
            {
                PlaceElementGroup();
                _xValues[i] = null;
                _yValues[i] = null;
            }
            for (var i = 0; i < _pointNumberLabels.ToArray().Length; i++)
            {
                PointsGrid.Children.Add(_pointNumberLabels[i]);
                PointsGrid.Children.Add(_pointXBoxes[i]);
                PointsGrid.Children.Add(_pointYBoxes[i]);
                PointsGrid.Children.Add(_cLabels[i]);
            }
        }

        public MainWindow()
        {
            PointsNumber = 2;
            InitializeComponent();
            PointNumberBox.Text = PointsNumber.ToString();
            InitializeLists();
            PlaceElements();
        }

        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            //SolutionBlock.Inlines.Clear();
            Chart.Diagram.Series[0].Points.Clear();
            Chart.Diagram.Series[1].Points.Clear();
            FindCoefButton.IsEnabled = false;
            try
            {
                var pointsNumber = Convert.ToInt32(PointNumberBox.Text);
                if (pointsNumber >= 2 && pointsNumber <= 10)
                {
                    RemoveAllElements();
                    PointsNumber = pointsNumber;
                    InitializeLists();
                    PlaceElements();
                }
                else
                {
                    MessageBox.Show("Введите число узлов от 2 до 10", "Ошибка", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (FormatException exception)
            {
                MessageBox.Show("Введите целое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            //SolutionBlock.Inlines.Clear();
            Chart.Diagram.Series[0].Points.Clear();
            Chart.Diagram.Series[1].Points.Clear();
            try
            {
                double value;
                for (var i = 0; i < PointsNumber; i++)
                {
                    value = Convert.ToDouble(_pointXBoxes[i].Text);
                    _xValues[i] = value;
                }
                for (var i = 0; i < PointsNumber; i++)
                {
                    value = Convert.ToDouble(_pointYBoxes[i].Text);
                    _yValues[i] = value;
                }
                FindCoefButton.IsEnabled = true;
                FindCoefButtonClick(this, e);

            }
            catch (FormatException exception)
            {
                FindCoefButton.IsEnabled = false;
                foreach (var c in _cLabels)
                    c.Content = "";
                Chart.Diagram.Series[0].Points.Clear();
                Chart.Diagram.Series[1].Points.Clear();
            }
        }

        private void FindCoefButtonClick(object sender, RoutedEventArgs e)
        {
            //SolutionBlock.Inlines.Clear();
            var x = new double[PointsNumber, PointsNumber];
            var y = new double[PointsNumber];
            double[] c = null;
            for(var i = 0; i < PointsNumber; i++)
            {
                for(var j = 0; j < PointsNumber; j++)
                    x[i, j] = Math.Pow((double)_xValues[i], j);
                y[i] = (double)_yValues[i];
                c = Gauss.Solve(x, y);
            }
            var s = "";
            for (var i = 0; i < PointsNumber; i++)
            {
                c[i] = Math.Round(c[i], 3);
                s = s + c[i].ToString() + " ";
            }
            //var ff = new FontFamily("Palatino Linotype");
            //var runs = new Run[PointsNumber];
            //var ssruns = new Run[PointsNumber];
            for (var i = 0; i < PointsNumber; i++)
            {
                //var symb = "";
                //if (i != PointsNumber - 1 && c[i] >= 0)
                //    symb = "+";
                //var str = symb + c[i].ToString() + (i != 0 ? "x" : "");
                //runs[i] = new Run(str)
                //                {
                //                    FontFamily = ff,
                //                    FontSize = 24
                //                };
                //ssruns[i] = new Run(i > 1 ? i.ToString() : "")
                //                {
                //                    FontFamily = ff,
                //                    FontSize = 24
                //                };
                //ssruns[i].Typography.Variants = FontVariants.Superscript;
                _cLabels[i].Content = c[i].ToString();
                

            }
            //for(var i = PointsNumber - 1; i >= 0; i--)
            //{
            //    SolutionBlock.Inlines.Add(runs[i]);
            //    SolutionBlock.Inlines.Add(ssruns[i]);
            //}
            //MessageBox.Show(s);
            Plot(c);
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            for(var i = 0; i < PointsNumber; i++)
            {
                _pointXBoxes[i].Text = Math.Round((rnd.NextDouble() * 20 - 10), 2).ToString();
                _pointYBoxes[i].Text = Math.Round((rnd.NextDouble() * 20 - 10), 2).ToString();
            }
            TextBoxTextChanged(this, null);
        }

        private void OptimalButton_Click(object sender, RoutedEventArgs e)
        {
            var a = Convert.ToDouble(SegmentBeginBox.Text);
            var b = Convert.ToDouble(SegmentEndBox.Text);
            for(var i = 0; i < PointsNumber; i++)
            {
                _pointXBoxes[i].Text = (Math.Round((((a + b)/2) + ((b - a)/2)*
                    Math.Cos(((2*i + 1)*Math.PI) / (2*PointsNumber + 2))), 3)).ToString();
            }
            TextBoxTextChanged(this, null);
        }

        private void OptimalSegmentTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDouble(SegmentBeginBox.Text);
                Convert.ToDouble(SegmentEndBox.Text);
                OptimalButton.IsEnabled = true;
            }
            catch (Exception)
            {
                OptimalButton.IsEnabled = false;
            }
        }
        
        private void Plot(double[] c)
        {
            var minX = 1.7e308;
            var maxX = 5.0e-324;
            for (var i = 0; i < PointsNumber; i++)
            {
                Chart.Diagram.Series[0].Points.Add(new SeriesPoint((double) _xValues[i], (double) _yValues[i]));
                minX = (double) _xValues[i] < minX ? (double) _xValues[i] : minX;
                maxX = (double) _xValues[i] > maxX ? (double) _xValues[i] : maxX;
            }
            var step = (maxX - minX)/30;
            var px = minX;
            for (var i = 0; i < 30; i++)
            {
                double py = 0;
                for (var j = 0; j < PointsNumber; j++)
                {
                    py = py + c[j] * Math.Pow(px, j);
                }
                Chart.Diagram.Series[1].Points.Add(new SeriesPoint(px, py));
                px = px + step;
            }
        }
    }
}
