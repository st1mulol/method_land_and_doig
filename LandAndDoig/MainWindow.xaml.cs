using DLLLandAndDoig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
using System.Windows.Threading;

namespace GraphicLandAndDoig
{
    class SelectBlockObject
    {
        private Equation[] _limits;
        private Equation _limit;
        private Equation _func;
        public SelectBlockObject(Equation[] limits, Equation limit, Equation func)
        {
            _limit = limit;
            _limits = limits;
            _func = func;
        }

        public Equation[] Limits
        {
            get { return _limits; }
        }
        public Equation Limit
        {
            get { return _limit; }
        }
        public Equation Func
        {
            get { return _func; }
        }
    }
    class BindingObject
    {
        public string ProgName
        {
            get;
            set;
        }
        public string BagX2
        {
            get;
            set;
        }
        public string FuncX1
        {
            get;
            set;
        }
        public string FuncX2
        {
            get;
            set;
        }

        public string LimitFirstX1
        {
            get;
            set;
        }

        public string LimitFirstX2
        {
            get;
            set;
        }

        public string LimitFirstC
        {
            get;
            set;
        }

        public string LimitSecondX1
        {
            get;
            set;
        }

        public string LimitSecondX2
        {
            get;
            set;
        }

        public string LimitSecondC
        {
            get;
            set;
        }
    }
    class CoordinatePlane
    {
        private DLLLandAndDoig.Point _start;
        private Canvas _canvas;
        private double _scale = 1;
        private int _step = 30;
        private int _max;
        private int _axesLength;
        private List<Line> _axesLines;
        private List<Label> _axesLabels;
        private List<Line> _grid;
        private List<Line> _divisionLines;
        private List<Label> _divisionLabels;
        private List<Line> _funcLines;
        private List<Label> _funcLabels;
        private List<Equation> _func;
        private Brush _colorGrid = Brushes.AliceBlue;
        private Brush _colorAxes = Brushes.Black;
        private Brush _colorDivision = Brushes.Black;
        private Brush _colorFunc = Brushes.Purple;
        private Brush _colorFuncLabel = Brushes.MediumVioletRed;
        public CoordinatePlane(Canvas canvas, DLLLandAndDoig.Point start, int axesLength)
        {
            _axesLines = new List<Line>();
            _axesLabels = new List<Label>();
            _grid = new List<Line>();
            _divisionLabels = new List<Label>();
            _divisionLines = new List<Line>();
            _func = new List<Equation>();
            _funcLines = new List<Line>();
            _funcLabels = new List<Label>();
            _canvas = canvas;
            _start = start;
            _axesLength = axesLength;
            drowGrid();
            drowAxes();
            reScale(1);
        }
        public void handleSelectEvent(SelectBlockObject obj)
        {
            _func.Clear();
            reScale(1);
            foreach (var t in obj.Limits)
            {
                drowLine(t);
            }
            if (obj.Limit != null) drowLine(obj.Limit);

            DLLLandAndDoig.Point start = new DLLLandAndDoig.Point(0, 0);
            DLLLandAndDoig.Point end = new DLLLandAndDoig.Point(obj.Func.X, obj.Func.Y);
            drowLine(start, end, obj.Func.toString());
        }

        public void drowLine(Equation equation)
        {
            DLLLandAndDoig.Point start = new DLLLandAndDoig.Point(0, 0);
            DLLLandAndDoig.Point end = new DLLLandAndDoig.Point(0, 0);

            Equation axesX = new Equation(1, 0, ">=", 0);
            Equation axesY = new Equation(0, 1, ">=", 0);

            if (Utils.IsLineParallel(equation, axesX))
            {
                start.X = equation.solveY(0);
                start.Y = 0;
                end.X = equation.solveY(0);
                end.Y = _max;
            }
            else if (Utils.IsLineParallel(equation, axesY))
            {
                start.X = 0;
                start.Y = equation.solveX(0);
                end.X = _max;
                end.Y = equation.solveX(0);
            }
            else
            {
                start.Y = equation.solveY(0);
                if (start.Y > _max)
                {
                    reScale(_scale + 1);
                    drowLine(equation);
                    return;
                }
                end.X = equation.solveX(0);
                if (start.Y == end.X)
                {
                    end.X = equation.solveX(_max);
                    end.Y = _max;

                    double temp = equation.solveY(_max);

                    if (temp < end.X)
                    {
                        end.Y = temp;
                        end.X = _max;
                    }
                }
                if (end.X > _max)
                {
                    reScale(_scale + 0.5);
                    drowLine(equation);
                    return;
                }
            }

            drowLine(start, end, equation.toString());
        }

        public void drowLine(DLLLandAndDoig.Point start, DLLLandAndDoig.Point end, string text)
        {
            Line line = new Line();
            line.X1 = _start.X + ((start.X / _scale) * _step);
            line.Y1 = (_axesLength - ((start.Y / _scale) * _step)) - 5;
            line.X2 = _start.X + (end.X / _scale) * _step;
            line.Y2 = (_axesLength - ((end.Y / _scale) * _step)) - 5;
            line.Stroke = _colorFunc;
            _funcLines.Add(line);
            _canvas.Children.Add(line);

            Label labelX = new Label();
            labelX.Content = text;
            Canvas.SetLeft(labelX, (line.X1 + line.X2) / 2);
            Canvas.SetTop(labelX, (line.Y1 + line.Y2) / 2);
            labelX.Foreground = _colorFuncLabel;
            _divisionLabels.Add(labelX);
            _canvas.Children.Add(labelX);
        }

        private void reScale(double scale)
        {
            _scale = scale;
            _max = (int)((_axesLength / _step) * _scale);
            removeAllLabelCanvas(_divisionLabels);
            removeAllLinesCanvas(_divisionLines);
            removeAllLabelCanvas(_funcLabels);
            removeAllLinesCanvas(_funcLines);
            foreach (var t in _func)
            {
                drowLine(t);
            }
            drowDivisions();
        }

        private void removeAllLabelCanvas(List<Label> list)
        {
            foreach (var t in list)
            {
                _canvas.Children.Remove(t);
            }
            list.Clear();
        }

        private void removeAllLinesCanvas(List<Line> list)
        {
            foreach (var t in list)
            {
                _canvas.Children.Remove(t);
            }
            list.Clear();
        }

        private void drowDivisions()
        {
            int sizeLine = 5;
            for (int i = _step; i < _axesLength; i += _step)
            {
                Line lineX = new Line();
                lineX.X1 = _start.X + i;
                lineX.Y1 = _axesLength - 5 - sizeLine;
                lineX.X2 = _start.X + i;
                lineX.Y2 = _axesLength - 5 + sizeLine;
                lineX.Stroke = _colorDivision;
                _divisionLines.Add(lineX);
                _canvas.Children.Add(lineX);

                Label labelX = new Label();
                labelX.Content = "" + ((i / _step) * _scale);
                Canvas.SetLeft(labelX, lineX.X2 - 10);
                Canvas.SetTop(labelX, lineX.Y2 - 5);
                labelX.Foreground = _colorDivision;
                _divisionLabels.Add(labelX);
                _canvas.Children.Add(labelX);

                Line lineY = new Line();
                lineY.X1 = _start.X - sizeLine;
                lineY.Y1 = _axesLength - 5 - i;
                lineY.X2 = _start.X + sizeLine;
                lineY.Y2 = _axesLength - 5 - i;
                lineY.Stroke = _colorDivision;
                _divisionLines.Add(lineY);
                _canvas.Children.Add(lineY);

                Label labelY = new Label();
                labelY.Content = "" + ((i / _step) * _scale);
                Canvas.SetLeft(labelY, lineY.X2 - 30);
                Canvas.SetTop(labelY, lineY.Y2 - 10);
                labelY.Foreground = _colorDivision;
                _divisionLabels.Add(labelY);
                _canvas.Children.Add(labelY);
            }
        }

        private void drowGrid()
        {
            for (int i = _step; i < _axesLength; i += _step)
            {
                Line axesX = new Line();
                axesX.X1 = _start.X - 5;
                axesX.Y1 = _axesLength - 5 - i;
                axesX.X2 = _axesLength;
                axesX.Y2 = _axesLength - 5 - i;
                axesX.Stroke = _colorGrid;
                _grid.Add(axesX);
                _canvas.Children.Add(axesX);

                Line axesY = new Line();
                axesY.X1 = _start.X + i;
                axesY.Y1 = _start.Y;
                axesY.X2 = _start.X + i;
                axesY.Y2 = _axesLength;
                axesY.Stroke = _colorGrid;
                _grid.Add(axesY);
                _canvas.Children.Add(axesY);
            }
        }

        private void drowAxes()
        {
            Line axesX = new Line();
            axesX.X1 = _start.X - 5;
            axesX.Y1 = _axesLength - 5;
            axesX.X2 = _axesLength;
            axesX.Y2 = _axesLength - 5;
            axesX.Stroke = _colorAxes;
            _axesLines.Add(axesX);
            _canvas.Children.Add(axesX);

            Label axesXLabel = new Label();
            axesXLabel.Content = "X1";
            Canvas.SetLeft(axesXLabel, axesX.X2 + 3);
            Canvas.SetTop(axesXLabel, axesX.Y2 - 13);
            axesXLabel.Foreground = _colorAxes;
            _axesLabels.Add(axesXLabel);
            _canvas.Children.Add(axesXLabel);

            Line axesXLeft = new Line();
            axesXLeft.X1 = axesX.X2;
            axesXLeft.Y1 = axesX.Y2;
            axesXLeft.X2 = axesX.X2 - 7;
            axesXLeft.Y2 = axesX.Y2 - 5;
            axesXLeft.Stroke = _colorAxes;
            _axesLines.Add(axesXLeft);
            _canvas.Children.Add(axesXLeft);

            Line axesXRight = new Line();
            axesXRight.X1 = axesX.X2;
            axesXRight.Y1 = axesX.Y2;
            axesXRight.X2 = axesX.X2 - 7;
            axesXRight.Y2 = axesX.Y2 + 5;
            axesXRight.Stroke = _colorAxes;
            _axesLines.Add(axesXRight);
            _canvas.Children.Add(axesXRight);

            Line axesY = new Line();
            axesY.X1 = _start.X;
            axesY.Y1 = _start.Y;
            axesY.X2 = _start.X;
            axesY.Y2 = _axesLength;
            axesY.Stroke = _colorAxes;
            _axesLines.Add(axesY);
            _canvas.Children.Add(axesY);

            Label axesYLabel = new Label();
            axesYLabel.Content = "X2";
            Canvas.SetLeft(axesYLabel, axesY.X1 - 10);
            Canvas.SetTop(axesYLabel, axesY.Y1 - 23);
            axesYLabel.Foreground = _colorAxes;
            _axesLabels.Add(axesYLabel);
            _canvas.Children.Add(axesYLabel);

            Line axesYLeft = new Line();
            axesYLeft.X1 = axesY.X1;
            axesYLeft.Y1 = axesY.Y1;
            axesYLeft.X2 = axesY.X1 - 5;
            axesYLeft.Y2 = axesY.Y1 + 7;
            axesYLeft.Stroke = _colorAxes;
            _axesLines.Add(axesYLeft);
            _canvas.Children.Add(axesYLeft);

            Line axesYRight = new Line();
            axesYRight.X1 = axesY.X1;
            axesYRight.Y1 = axesY.Y1;
            axesYRight.X2 = axesY.X1 + 5;
            axesYRight.Y2 = axesY.Y1 + 7;
            axesYRight.Stroke = _colorAxes;
            _axesLines.Add(axesYRight);
            _canvas.Children.Add(axesYRight);
        }
    }
    class Leaf
    {
        public Leaf Left = null;
        public Leaf Right = null;
        public Equation[] Limits = null;
        public Equation Func = null;
        public Equation New = null;
        public PointS Point = null;
        public bool Valid;
        public List<int> Path;
        public Rectangle Rect = null;
    }
    class Three
    {
        private Leaf _root = new Leaf();
        private int _boxWidth = 120;
        private int _boxHeight = 30;
        private Canvas _canvas = null;
        private double _center;
        private List<Rectangle> _rectList;
        private List<Label> _labelList;
        private List<Line> _lineList;
        private int _maxLength = 0;
        private Rectangle _selected = null;
        private Brush _colorGrid = Brushes.AliceBlue;
        private Brush _colorBlock = Brushes.Aqua;
        private Brush _colorBlockSelect = Brushes.Coral;
        private Brush _colorBlockText = Brushes.Black;
        private Brush _colorValid = Brushes.Green;
        private Brush _colotNotValid = Brushes.Red;

        public delegate void SelectBlockHandler(SelectBlockObject point);
        public event SelectBlockHandler SelectBlock;
        public Three(Canvas canvas)
        {
            _canvas = canvas;
            _center = canvas.Width / 2;
            _rectList = new List<Rectangle>();
            _labelList = new List<Label>();
            _lineList = new List<Line>();
        }
        public void Add(Equation[] limits, Equation limit, PointS point, bool valid, List<int> path, Equation func)
        {
            Leaf temp = _root;
            foreach (var t in path)
            {
                if (t == 1)
                {
                    if (temp.Left == null) temp.Left = new Leaf();
                    temp = temp.Left;
                }
                else
                {
                    if (temp.Right == null) temp.Right = new Leaf();
                    temp = temp.Right;
                }
            }
            temp.Limits = limits;
            temp.New = limit;
            temp.Point = point;
            temp.Valid = valid;
            temp.Path = path;
            temp.Func = func;
        }

        public void Clear()
        {
            _root = new Leaf();
            foreach (var t in _rectList)
            {
                _canvas.Children.Remove(t);
            }
            foreach (var t in _lineList)
            {
                _canvas.Children.Remove(t);
            }
            foreach (var t in _labelList)
            {
                _canvas.Children.Remove(t);
            }
            _rectList.Clear();
            _lineList.Clear();
            _labelList.Clear();
        }

        public void Drow()
        {
            DrowGrid();
            DrowThree(_root, 0);
        }

        public void DrowGrid()
        {
            int step = 30;

            for (int i = step; i < _canvas.Width; i += step)
            {
                Line axesX = new Line();
                axesX.X1 = 0;
                axesX.Y1 = i;
                axesX.X2 = _canvas.Width;
                axesX.Y2 = i;
                axesX.Stroke = _colorGrid;
                _lineList.Add(axesX);
                _canvas.Children.Add(axesX);

                Line axesY = new Line();
                axesY.X1 = i;
                axesY.Y1 = 0;
                axesY.X2 = i;
                axesY.Y2 = _canvas.Width;
                axesY.Stroke = _colorGrid;
                _lineList.Add(axesY);
                _canvas.Children.Add(axesY);
            }
        }

        private void DrowThree(Leaf leaf, int length)
        {
            if (leaf.Left != null) DrowThree(leaf.Left, length + 1);
            if (leaf.Right != null) DrowThree(leaf.Right, length + 1);

            if (_maxLength < length) _maxLength = length;

            Rectangle rectangle = new Rectangle();
            rectangle.Fill = _colorBlock;
            rectangle.Width = _boxWidth;
            rectangle.Height = _boxHeight;
            leaf.Rect = rectangle;

            int pos = 0;
            string name = "Rect";

            int count = 0;
            foreach (var t in leaf.Path)
            {
                if (name != "Rect") name += "_";
                name += t;
                if (t == 1) pos -= _boxWidth * (_maxLength - count);
                else pos += _boxWidth * (_maxLength - count);
                count++;
            }

            rectangle.Name = name;
            rectangle.MouseDown += HandlerClick;
            Canvas.SetLeft(rectangle, _center + pos - (rectangle.Width / 2));
            Canvas.SetTop(rectangle, ((_boxHeight * 2) * length));
            _rectList.Add(rectangle);
            _canvas.Children.Add(rectangle);

            Label pointLabel = new Label();
            pointLabel.Content = $"X({Math.Round(leaf.Point.X, 2)} , {Math.Round(leaf.Point.Y, 2)})";
            Canvas.SetLeft(pointLabel, Canvas.GetLeft(rectangle));
            Canvas.SetTop(pointLabel, Canvas.GetTop(rectangle) - 3);
            pointLabel.Foreground = _colorBlockText;
            pointLabel.FontSize = 10;
            _labelList.Add(pointLabel);
            _canvas.Children.Add(pointLabel);

            if (length != 0)
            {
                if (leaf.Valid)
                {
                    Label pointD = new Label();
                    pointD.Content = $"F(x)={Math.Round(leaf.Point.D, 2)}";
                    Canvas.SetLeft(pointD, Canvas.GetLeft(rectangle) + _boxWidth / 2);
                    Canvas.SetTop(pointD, Canvas.GetTop(rectangle) - 3);
                    pointD.Foreground = _colorBlockText;
                    pointD.FontSize = 10;
                    _labelList.Add(pointD);
                    _canvas.Children.Add(pointD);
                }
                Label newLimit = new Label();
                newLimit.Content = leaf.New.toString();
                Canvas.SetLeft(newLimit, Canvas.GetLeft(rectangle));
                Canvas.SetTop(newLimit, Canvas.GetTop(rectangle) + 8);
                if (leaf.Valid)
                {
                    newLimit.Foreground = _colorValid;
                }
                else
                {
                    newLimit.Foreground = _colotNotValid;
                }
                newLimit.FontSize = 10;
                _labelList.Add(newLimit);
                _canvas.Children.Add(newLimit);
            }

            string toolTip = $"X({leaf.Point.X} , {leaf.Point.Y})\n";
            toolTip += $"F(x)={leaf.Point.D}\n";
            toolTip += "Ограничения{\n";
            foreach (var t in leaf.Limits)
            {
                toolTip += "\t" + t.toString() + "\n";
            }
            if (leaf.New != null) toolTip += "\t" + leaf.New.toString() + "\n";
            toolTip += "}";
            rectangle.ToolTip = toolTip;

            if (leaf.Left != null || leaf.Right != null)
            {
                Line centerLine = new Line();
                centerLine.X1 = Canvas.GetLeft(rectangle) + (rectangle.Width / 2);
                centerLine.Y1 = Canvas.GetTop(rectangle) + rectangle.Height;
                centerLine.X2 = Canvas.GetLeft(rectangle) + (rectangle.Width / 2);
                centerLine.Y2 = Canvas.GetTop(rectangle) + rectangle.Height + _boxHeight / 2;
                centerLine.Stroke = _colorBlock;
                _lineList.Add(centerLine);
                _canvas.Children.Add(centerLine);


                if (leaf.Left != null)
                {
                    Line centerLeft = new Line();
                    centerLeft.X1 = centerLine.X2;
                    centerLeft.Y1 = centerLine.Y2;
                    centerLeft.X2 = centerLine.X2 - (_boxWidth * (((_maxLength) - length)));
                    centerLeft.Y2 = centerLine.Y2;
                    centerLeft.Stroke = _colorBlock;
                    _lineList.Add(centerLeft);
                    _canvas.Children.Add(centerLeft);

                    Line centerLeftDown = new Line();
                    centerLeftDown.X1 = centerLeft.X2;
                    centerLeftDown.Y1 = centerLeft.Y2;
                    centerLeftDown.X2 = centerLeft.X2;
                    centerLeftDown.Y2 = centerLine.Y2 + _boxHeight / 2;
                    centerLeftDown.Stroke = _colorBlock;
                    _lineList.Add(centerLeftDown);
                    _canvas.Children.Add(centerLeftDown);
                }
                if (leaf.Right != null)
                {
                    Line centerRight = new Line();
                    centerRight.X1 = centerLine.X2;
                    centerRight.Y1 = centerLine.Y2;
                    centerRight.X2 = centerLine.X2 + (_boxWidth * (((_maxLength) - length)));
                    centerRight.Y2 = centerLine.Y2;
                    centerRight.Stroke = _colorBlock;
                    _lineList.Add(centerRight);
                    _canvas.Children.Add(centerRight);

                    Line centerRightDown = new Line();
                    centerRightDown.X1 = centerRight.X2;
                    centerRightDown.Y1 = centerRight.Y2;
                    centerRightDown.X2 = centerRight.X2;
                    centerRightDown.Y2 = centerLine.Y2 + _boxHeight / 2;
                    centerRightDown.Stroke = _colorBlock;
                    _lineList.Add(centerRightDown);
                    _canvas.Children.Add(centerRightDown);
                }
            }
        }

        private void HandlerClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectBlockMethod(((Rectangle)sender).Name);
            }
        }
        public void SelectBlockMethod(string str)
        {
            var temp = _root;
            foreach (var t in str.Substring(4).Split('_'))
            {
                if (t == "1")
                {
                    temp = temp.Left;
                }
                else if (t == "0")
                {
                    temp = temp.Right;
                }
            }

            if (_selected != null)
            {
                _selected.Fill = _colorBlock;
            }
            _selected = temp.Rect;
            _selected.Fill = _colorBlockSelect;

            SelectBlock?.Invoke(new SelectBlockObject(temp.Limits, temp.New, temp.Func));
        }
    }

    class HandlerInterface
    {
        private BindingObject _binding;
        private Equation[] _limits;
        private Equation _func;
        private Three _three;
        private CoordinatePlane _plane;
        private MainWindow _window;
        private string _pathToEXE = "C:/Users/st1mu/Desktop/Соня/LandAndDoig/MethodLandAndDoig/bin/Debug/netcoreapp3.1/MethodLandAndDoig.exe";

        public HandlerInterface(BindingObject binding, CoordinatePlane plane, Three three, MainWindow window)
        {
            _binding = binding;
            _limits = new Equation[2];
            _plane = plane;
            _three = three;
            _window = window;
        }

        public void readFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json array (.json)|*.json";

            if ((bool)dlg.ShowDialog())
            {
                string str = File.ReadAllText(dlg.FileName);
                var utf8Reader = new Utf8JsonReader(Encoding.ASCII.GetBytes(str));
                JSONEquations json = JsonSerializer.Deserialize<JSONEquations>(ref utf8Reader);
                _binding.FuncX1 = json.FuncX1 + "";
                _window.FuncX1.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.FuncX2 = json.FuncX2 + "";
                _window.FuncX2.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitFirstX1 = json.Limit1X1 + "";
                _window.Limit1X1.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitFirstX2 = json.Limit1X2 + "";
                _window.Limit1X2.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitFirstC = json.Limit1C + "";
                _window.Limit1C.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitSecondX1 = json.Limit2X1 + "";
                _window.Limit2X1.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitSecondX2 = json.Limit2X2 + "";
                _window.Limit2X2.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                _binding.LimitSecondC = json.Limit2C + "";
                _window.Limit2C.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

        public void startDLL(object sender, RoutedEventArgs e)
        {
            if (!readInteface()) return;
            _three.Clear();
            MethodLandAndDoig method = new MethodLandAndDoig(_func, _limits);
            method.BranchMethod += branchHandler;
            method.StartMethod += startHandler;
            method.EndMethod += endHandler;
            method.solve();
        }

        public void startThreed(object sender, RoutedEventArgs e)
        {
            if (!readInteface()) return;
            _three.Clear();
            MethodLandAndDoig method = new MethodLandAndDoig(_func, _limits);
            method.BranchMethod += branchHandler;
            method.StartMethod += startHandler;
            method.EndMethod += endHandler;
            Thread thread = new Thread(
            new ThreadStart(method.solve));
            thread.Start();
        }

        public void startApp(object sender, RoutedEventArgs e)
        {
            if (!readInteface()) return;
            JSONEquations json = new JSONEquations();
            json.FuncX1 = _func.X;
            json.FuncX2 = _func.Y;
            json.Limit1X1 = _limits[0].X;
            json.Limit1X2 = _limits[0].Y;
            json.Limit1C = _limits[0].Result;
            json.Limit2X1 = _limits[1].X;
            json.Limit2X2 = _limits[1].Y;
            json.Limit2C = _limits[1].Result;
            string str = Utils.Base64Encode(JsonSerializer.Serialize(json));
            Process.Start(_pathToEXE, str);
        }

        private bool readInteface()
        {
            double x, y, c;
            try
            {
                x = Double.Parse(_binding.FuncX1);
                y = Double.Parse(_binding.FuncX2);
                _func = new Equation(x, y, null, 0);
                x = Double.Parse(_binding.LimitFirstX1);
                y = Double.Parse(_binding.LimitFirstX2);
                c = Double.Parse(_binding.LimitFirstC);
                _limits[0] = new Equation(x, y, "<=", c);
                x = Double.Parse(_binding.LimitSecondX1);
                y = Double.Parse(_binding.LimitSecondX2);
                c = Double.Parse(_binding.LimitSecondC);
                _limits[1] = new Equation(x, y, "<=", c);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Не возможно прочитать некоторые поля\nПожалуйста проверьте правильность введённых данных!", "Уведомление");
            }
            return false;
        }

        public void branchHandler(BranchMethodObject obj)
        {
            _three.Add(obj.Limits, obj.Limit, new PointS(new DLLLandAndDoig.Point(obj.X, obj.Y), obj.D), obj.Valid, obj.Branch, obj.Func);
        }
        public void startHandler(StartMethodObject obj)
        {
            _three.Add(obj.Limits, null, new PointS(new DLLLandAndDoig.Point(obj.X, obj.Y), obj.D), true, new List<int>(), obj.Func);
        }
        public void endHandler(object sender, EndMethodObject obj)
        {
            _window.Dispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(delegate ()
            {
                _three.Drow();
                _window.Scroll.ScrollToHorizontalOffset(_window.ScrollCanvas.Width / 2);
                _three.SelectBlockMethod("Rect");
                _window.ResultMecthod.Content = $"X*({obj.X} , {obj.Y}) F(x)={obj.D}";
            }));
        }
    }

    public partial class MainWindow : Window
    {
        private BindingObject _binding = null;
        private bool _scrollCanvas = false;
        private System.Windows.Point _lastPostion;
        private Three _canvasThree;
        public MainWindow()
        {
            InitializeComponent();
            _binding = new BindingObject();
            this.DataContext = _binding;

            _binding.ProgName = "GraphicLandAndDoig";
            _binding.BagX2 = "X2 <=";

            CoordinatePlane plane = new CoordinatePlane(RootGrid, new DLLLandAndDoig.Point(30, 20), 330);
            _canvasThree = new Three(ScrollCanvas);
            _canvasThree.DrowGrid();
            _canvasThree.SelectBlock += plane.handleSelectEvent;

            Scroll.MouseDown += MouseDownScroll;
            Scroll.MouseUp += MouseUpScroll;
            Scroll.MouseMove += MouseMoveScroll;
            Scroll.MouseLeave += MouseUpScroll;

            HandlerInterface inter = new HandlerInterface(_binding, plane, _canvasThree, this);
            ButtonDLL.Click += inter.startDLL;
            ButtonExe.Click += inter.startApp;
            ButtonThreed.Click += inter.startThreed;
            ReadFile.Click += inter.readFile;
        }

        private void MouseMoveScroll(object sender, MouseEventArgs e)
        {
            if (!_scrollCanvas) return;
            System.Windows.Point point = e.GetPosition(ScrollCanvas);
            double x = _lastPostion.X - point.X;
            double y = _lastPostion.Y - point.Y;
            Scroll.ScrollToHorizontalOffset(Scroll.ContentHorizontalOffset + x);
            Scroll.ScrollToVerticalOffset(Scroll.ContentVerticalOffset + y);
        }

        private void MouseDownScroll(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                _scrollCanvas = true;
                _lastPostion = e.GetPosition(ScrollCanvas);
            }
        }

        private void MouseUpScroll(object sender, MouseEventArgs e)
        {
            _scrollCanvas = false;
        }
    }
}
