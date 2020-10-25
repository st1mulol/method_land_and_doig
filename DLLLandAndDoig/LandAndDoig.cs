using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLLandAndDoig
{
    public class JSONEquations
    {
        public double FuncX1 { get; set; }
        public double FuncX2 { get; set; }
        public double Limit1X1 { get; set; }
        public double Limit1X2 { get; set; }
        public double Limit1C { get; set; }
        public double Limit2X1 { get; set; }
        public double Limit2X2 { get; set; }
        public double Limit2C { get; set; }
    }
    public class Equation
    {
        private double _x;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private int _sign;
        public int Sign
        {
            get { return _sign; }
            set { _sign = value; }
        }

        private double _result;
        public double Result
        {
            get { return _result; }
            set { _result = value; }
        }
        public bool isX()
        {
            return _x != 0;
        }
        public bool isY()
        {
            return _y != 0;
        }
        public Equation(double x, double y, string sign, double result)
        {
            X = x;
            Y = y;
            Result = result;
            if (sign == null) Sign = 0;
            else if (sign == "<=") Sign = -1;
            else Sign = 1;
        }

        public bool check(double x, double y)
        {
            if (x < 0 || y < 0) return false;
            double temp = (X * x + Y * y);
            if (Sign == 1)
            {
                return temp >= Result;
            }
            else if (Sign == -1)
            {
                return temp <= Result;
            }
            else
            {
                return temp == Result;
            }
        }

        public double solveY(double x)
        {
            if (!isX()) return Result / Y;
            if (!isY()) return Result;
            return ((-X * x) + Result) / Y;
        }

        public double solveX(double y)
        {
            if (!isY()) return Result / X;
            if (!isX()) return Result;
            return ((-Y * y) + Result) / X;
        }

        public string toString()
        {
            string result = "";
            string sign = "-> max";
            if (Sign == 1)
            {
                sign = $">= {Result}";
            }
            else if (Sign == -1)
            {
                sign = $"<= {Result}";
            }
            if (X != 0) result += $"{X}X1 ";
            if (Y != 0)
            {
                if (result != "") result += "+ ";
                result += $"{Y}X2 ";
            }
            return result + sign;
        }
    }

    public class Point
    {
        private double _x;
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        public Point(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }

    public class EquationSystem
    {
        private Equation[] _arr;
        public EquationSystem(Equation[] arr)
        {
            _arr = arr;
        }
        public Point solve()
        {
            double y = 0;
            double x = 0;
            if (_arr.Length != 2) throw new Exception("I don't know");
            if (!_arr[0].isX() || !_arr[0].isY())
            {
                if (!_arr[0].isX() && _arr[0].isY())
                {
                    if (!_arr[1].isX() || !_arr[1].isY())
                    {
                        if (!_arr[1].isX() && _arr[1].isY())
                        {
                            throw new Exception("I don't know");
                        }
                        else if (_arr[1].isX() && !_arr[1].isY())
                        {
                            x = _arr[1].Result / _arr[1].X;
                            y = _arr[0].Result / _arr[0].Y;
                            return new Point(x, y);
                        }
                        else
                        {
                            throw new Exception("I don't know");
                        }
                    }
                    else
                    {
                        y = _arr[0].Result / _arr[0].Y;
                        x = ((-_arr[1].Y * y) + _arr[1].Result) / _arr[1].X;
                        return new Point(x, y);
                    }
                }
                else if (_arr[0].isX() && !_arr[0].isY())
                {
                    if (!_arr[1].isX() || !_arr[1].isY())
                    {
                        if (!_arr[1].isX() && _arr[1].isY())
                        {
                            x = _arr[0].Result / _arr[0].X;
                            y = _arr[1].Result / _arr[1].Y;
                            return new Point(x, y);
                        }
                        else if (_arr[1].isX() && !_arr[1].isY())
                        {
                            throw new Exception("I don't know");
                        }
                        else
                        {
                            throw new Exception("I don't know");
                        }
                    }
                    else
                    {
                        x = _arr[0].Result / _arr[0].X;
                        y = ((-_arr[1].X * x) + _arr[1].Result) / _arr[1].Y;
                        return new Point(x, y);
                    }
                }
                else
                {
                    throw new Exception("I don't know");
                }
            }
            if (!_arr[1].isX() || !_arr[1].isY())
            {
                if (!_arr[1].isX() && _arr[1].isY())
                {
                    y = _arr[1].Result / _arr[1].Y;
                    x = ((-_arr[0].Y * y) + _arr[0].Result) / _arr[0].X;
                    return new Point(x, y);
                }
                else if (_arr[1].isX() && !_arr[1].isY())
                {
                    x = _arr[1].Result / _arr[1].X;
                    y = ((-_arr[0].X * x) + _arr[0].Result) / _arr[0].Y;
                    return new Point(x, y);
                }
                else
                {
                    throw new Exception("I don't know");
                }
            }
            y = (((-_arr[1].Result / _arr[1].X) * _arr[0].X) + _arr[0].Result) / (((-_arr[1].Y / _arr[1].X) * _arr[0].X) + _arr[0].Y);
            x = ((-_arr[1].Y * y) + _arr[1].Result) / _arr[1].X;
            return new Point(x, y);
        }
    }

    public class PointS
    {
        private Point _point;
        private double _D;
        public PointS(Point point, double D)
        {
            _point = point;
            _D = D;
        }
        public double X
        {
            get { return _point.X; }
        }
        public double Y
        {
            get { return _point.Y; }
        }
        public double D
        {
            get { return _D; }
        }
    }

    public class Utils
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static bool IsInteger(double num)
        {
            return ((int)num) == num;
        }
        public static bool IsInteger(Point point)
        {
            return IsInteger(point.X) && IsInteger(point.Y);
        }

        public static bool IsLineParallel(Equation one, Equation two)
        {
            double[] coef = new double[3];

            if (one.X == 0 && two.X == 0) return true;
            if (one.Y == 0 && two.Y == 0) return true;

            if (two.X == 0) coef[0] = Double.MaxValue;
            else coef[0] = one.X / two.X;

            if (two.Y == 0) coef[1] = Double.MaxValue;
            else coef[1] = one.Y / two.Y;

            if (two.Result == 0) coef[2] = Double.MaxValue;
            else coef[2] = one.Result / two.Result;

            return coef[0] == coef[1] && coef[0] != coef[2];
        }
    }

    public class StartMethodObject
    {
        private PointS _point;
        private Equation[] _limits;
        private Equation _func;
        public StartMethodObject(PointS point, Equation[] limits, Equation func)
        {
            _point = point;
            _limits = limits;
            _func = func;
        }

        public Equation Func
        {
            get { return _func; }
        }

        public Equation[] Limits
        {
            get { return _limits; }
        }

        public double X
        {
            get { return _point.X; }
        }
        public double Y
        {
            get { return _point.Y; }
        }
        public double D
        {
            get { return _point.D; }
        }
    }

    public class BranchMethodObject
    {
        private bool _valid;
        private PointS _point;
        private List<int> _list;
        private Equation[] _limits;
        private Equation _limit;
        private Equation _func;
        public BranchMethodObject(PointS point, List<int> list /* Содержит путь ветки, 1 лево 0 право */, bool valid /* решается ли ветка */, Equation[] limits, Equation limit, Equation func)
        {
            _list = list;
            _point = point;
            _valid = valid;
            _limits = limits;
            _limit = limit;
            _func = func;
        }
        public Equation Func
        {
            get { return _func; }
        }
        public Equation Limit
        {
            get { return _limit; }
        }
        public Equation[] Limits
        {
            get { return _limits; }
        }
        public bool Valid
        {
            get { return _valid; }
        }
        public double X
        {
            get { return _point.X; }
        }
        public double Y
        {
            get { return _point.Y; }
        }
        public double D
        {
            get { return _point.D; }
        }
        public List<int> Branch
        {
            get { return _list; }
        }
    }

    public class EndMethodObject : EventArgs
    {
        private PointS _point;
        private bool _valid;
        public EndMethodObject(PointS point, bool valid)
        {
            _point = point;
            _valid = valid;
        }
        public double X
        {
            get { return _point.X; }
        }
        public double Y
        {
            get { return _point.Y; }
        }
        public double D
        {
            get { return _point.D; }
        }
        public bool Valid
        {
            get { return _valid; }
        }
    }

    public class MethodLandAndDoig
    {
        private PointS _maxS;
        private Equation[] _limits;
        private Equation _func;

        public delegate void StartMethodHandler(StartMethodObject point);
        public event StartMethodHandler StartMethod;

        public delegate void BranchMethodHandler(BranchMethodObject message);
        public event BranchMethodHandler BranchMethod;

        public event EventHandler<EndMethodObject> EndMethod;

        public MethodLandAndDoig(Equation func, Equation[] limit)
        {
            _limits = limit;
            _func = func;
            _maxS = null;
        }


        private double solveFunction(double x, double y)
        {
            return _func.X * x + _func.Y * y;
        }


        private void solve(Equation[] limits, Equation limit, List<int> list)
        {
            bool isBranchX = true;

            if (limit.Sign == 1)
            {
                list.Add(1);
            }
            else
            {
                list.Add(0);
            }

            if (limit.X == 0)
            {
                isBranchX = false;
            }

            double maxValue = Double.MaxValue;

            foreach (var equation in limits)
            {

                if (!Utils.IsLineParallel(limit, equation))
                {
                    Equation[] equations = new Equation[2];
                    equations[0] = limit;
                    equations[1] = equation;
                    EquationSystem system = new EquationSystem(equations);
                    var temp = system.solve();
                    if (isBranchX && maxValue > temp.Y) maxValue = temp.Y;
                    if (!isBranchX && maxValue > temp.X) maxValue = temp.X;
                }
            }

            bool valid = true;

            foreach (var equation in limits)
            {
                if (isBranchX)
                {
                    if (!equation.check(limit.X, maxValue)) valid = false;
                }
                else
                {
                    if (!equation.check(maxValue, limit.Y)) valid = false;
                }
            }

            Point result = null;
            if (isBranchX)
            {
                result = new Point(limit.Result, maxValue);
            }
            else
            {
                result = new Point(maxValue, limit.Result);
            }



            Equation[] newLimits = new Equation[limits.Length + 1];
            for (int i = 0; i < limits.Length; i++)
            {
                newLimits[i] = limits[i];
            }
            newLimits[limits.Length] = limit;

            /* Метод инициирует событие */
            BranchMethod?.Invoke(new BranchMethodObject(new PointS(result, solveFunction(result.X, result.Y)), new List<int>(list), valid, limits, limit, _func)); // И передаёт данные по текущей ветке

            if (!valid) return;

            if (Utils.IsInteger(result))
            {
                var temp = solveFunction(result.X, result.Y);
                if (_maxS == null || temp > _maxS.D) _maxS = new PointS(result, temp);
            }
            else
            {
                if (!Utils.IsInteger(result.X))
                {
                    solve(newLimits, new Equation(1, 0, ">=", Math.Ceiling(result.X)), copyList(list));
                    solve(newLimits, new Equation(1, 0, "<=", Math.Floor(result.X)), copyList(list));
                }
                if (!Utils.IsInteger(result.Y))
                {
                    solve(newLimits, new Equation(0, 1, ">=", Math.Ceiling(result.Y)), copyList(list));
                    solve(newLimits, new Equation(0, 1, "<=", Math.Floor(result.Y)), copyList(list));
                }
            }
        }

        private List<int> copyList(List<int> list)
        {
            List<int> result = new List<int>();
            foreach (var t in list)
            {
                result.Add(t);
            }
            return result;
        }

        public void solve()
        {
            EquationSystem system = new EquationSystem(_limits);
            var result = system.solve();

            StartMethod?.Invoke(new StartMethodObject(new PointS(result, solveFunction(result.X, result.Y)), _limits, _func));

            if (Utils.IsInteger(result))
            {
                _maxS = new PointS(result, solveFunction(result.X, result.Y));
            }
            else
            {
                List<int> list = new List<int>();
                if (!Utils.IsInteger(result.X))
                {
                    solve(_limits, new Equation(1, 0, ">=", Math.Ceiling(result.X)), new List<int>(list));
                    solve(_limits, new Equation(1, 0, "<=", Math.Floor(result.X)), new List<int>(list));
                }
                else
                {
                    solve(_limits, new Equation(0, 1, ">=", Math.Ceiling(result.Y)), new List<int>(list));
                    solve(_limits, new Equation(0, 1, "<=", Math.Floor(result.Y)), new List<int>(list));
                }
            }
            EndMethod.Invoke(this, new EndMethodObject(_maxS, true));
        }
    }
}
