﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private int _currentBox;
        private int _currentMargin = 20;
        private int _number;
        private bool _plotted = false;
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
            tb.PreviewMouseDown += TextBoxMouseDown;
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
            tb.PreviewMouseDown += TextBoxMouseDown;
            _pointYBoxes.Add(tb);
            var lb = new Label
                         {
                             Margin = new Thickness(_currentMargin-10, 105, 0, 0),
                             Height = 25,
                             Width = 75,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top,
                             Content="",
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
            Chart.Diagram.Series.Add(new PointSeries2D());
            diagram.Series[2].LabelsVisibility = true;
            _plotted = false;
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
            _plotted = false;
            _currentBox = 0;
            diagram.AxisX.Range.MinValue = -10;
            diagram.AxisX.Range.MaxValue = 10;
            diagram.AxisY.Range.MinValue = -10;
            diagram.AxisY.Range.MaxValue = 10;
            Chart.Diagram.Series[0].Points.Clear();
            Chart.Diagram.Series[1].Points.Clear();
            diagram.Series[2].Points.Clear();
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
            catch (FormatException)
            {
                MessageBox.Show("Введите целое число", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
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
            catch (FormatException)
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
            var isNaN = false;
            foreach(var curc in c)
            {
                if(double.IsNaN(curc) || double.IsInfinity(curc))
                {
                    isNaN = true;
                    break;
                }
            }
            if (!isNaN)
            {
                var s = "";
                for (var i = 0; i < PointsNumber; i++)
                {
                    //c[i] = Math.Round(c[i], 3);
                    s = s + Math.Round(c[i], 3).ToString() + " ";
                }
                for (var i = 0; i < PointsNumber; i++)
                {
                    _cLabels[i].Content = Math.Round(c[i], 3).ToString();
                }
                Plot(c);
            }
            else
                foreach (var label in _cLabels)
                {
                    label.Content = "";
                }
        }

        private void RandomButtonClick(object sender, RoutedEventArgs e)
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
            _plotted = true;
            diagram.Series[2].Points.Clear();
            diagram.AxisX.Range = new AxisRange();
            diagram.AxisY.Range = new AxisRange();
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
            for (var i = 0; i < 31; i++)
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

        private void XyDiagram2DMouseDown1(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(diagram);
            var dc = diagram.PointToDiagram(position);
            var x = Math.Round(dc.NumericalArgument, 2) + 0.7;
            var y = Math.Round(dc.NumericalValue, 2) - 1.5;
            StatusText.Text = "Выбраны координаты: " + x.ToString() + " " + y.ToString();
            _pointXBoxes[_currentBox].Text = x.ToString();
            _pointYBoxes[_currentBox].Text = y.ToString();
            if(!_plotted)
                diagram.Series[2].Points.Add(new SeriesPoint(x, y));
            if(_currentBox != PointsNumber - 1)
                _currentBox++;
        }

        private void TextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            for(var i = 0; i < PointsNumber; i++)
            {
                if ((_pointXBoxes[i].IsKeyboardFocused))
                    _currentBox = i;
                if (_pointYBoxes[i].IsKeyboardFocused)
                    _currentBox = i;
            }
            StatusText.Text = "Выбрана точка " + _currentBox.ToString();
        }
    }
}
