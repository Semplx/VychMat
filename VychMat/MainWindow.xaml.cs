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
        private double?[] _xValues;
        private double?[] _yValues;
        private int _currentMargin = 20;
        private int _number;
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
                             Width = 25,
                             HorizontalAlignment = HorizontalAlignment.Left,
                             VerticalAlignment = VerticalAlignment.Top
                         };
            tb.TextChanged += TextBoxTextChanged;
            _pointXBoxes.Add(tb);
            tb = new TextBox
                     {
                         Margin = new Thickness(_currentMargin, 70, 0, 0),
                         Height = 25,
                         Width = 25,
                         HorizontalAlignment = HorizontalAlignment.Left,
                         VerticalAlignment = VerticalAlignment.Top
                     };
            tb.TextChanged += TextBoxTextChanged;
            _pointYBoxes.Add(tb);
            _currentMargin += 50;
            _number++;
        }

        private void InitializeLists()
        {
            _pointNumberLabels = new List<Label>();
            _pointXBoxes = new List<TextBox>();
            _pointYBoxes = new List<TextBox>();
        }

        private void RemoveAllElements()
        {
            for (var i = 0; i < PointsNumber; i++)
            {
                PointsGrid.Children.Remove(_pointNumberLabels[i]);
                PointsGrid.Children.Remove(_pointXBoxes[i]);
                PointsGrid.Children.Remove(_pointYBoxes[i]);
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
            }
            catch (FormatException exception)
            {
                FindCoefButton.IsEnabled = false;
            }
        }
    }
}
