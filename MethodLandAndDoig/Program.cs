using DLLLandAndDoig;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MethodLandAndDoigNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Параметров не передано!");
            }
            else
            {
                string str = Utils.Base64Decode(args[0]);
                var utf8Reader = new Utf8JsonReader(Encoding.ASCII.GetBytes(str));
                JSONEquations json = JsonSerializer.Deserialize<JSONEquations>(ref utf8Reader);
                Equation func = new Equation(json.FuncX1, json.FuncX2, null, 0);
                Equation[] limits = new Equation[2];
                limits[0] = new Equation(json.Limit1X1, json.Limit1X2, "<=", json.Limit1C);
                limits[1] = new Equation(json.Limit2X1, json.Limit2X2, "<=", json.Limit2C);
                MethodLandAndDoig method = new MethodLandAndDoig(func, limits);
                method.BranchMethod += HandlerBranch;
                method.EndMethod += endHandler;
                method.StartMethod += startHandler;
                method.solve();
                Console.ReadKey();
            }
        }

        private static void startHandler(StartMethodObject obj)
        {
            Console.WriteLine("========================\nНачало");
            Console.WriteLine($"X({obj.X} , {obj.Y}) F(x)={obj.D}");
            Console.WriteLine("========================");
        }

        private static void endHandler(object sender, EndMethodObject obj)
        {
            Console.WriteLine($"Результат: X*({obj.X} , {obj.Y}) f* = {obj.D}");
        }

        private static void HandlerBranch(BranchMethodObject obj) // А это сам обработчик события их может быть сколько угодно
        {
            Console.WriteLine("========================");
            Console.Write("Задача: ");
            foreach (var t in obj.Branch)
            {
                Console.Write(t + "-");
            }
            Console.WriteLine();
            string res = "имеет решение";
            if (!obj.Valid) res = "НЕ имеет решение";
            Console.WriteLine("Ветка " + res);
            Console.WriteLine($"X*({obj.X} , {obj.Y}) f*={obj.D}");
            Console.WriteLine("Ограничения ветки: {");
            foreach (var t in obj.Limits)
            {
                string sign = "=";
                if (t.Sign == 1) sign = ">=";
                if (t.Sign == -1) sign = "<=";
                Console.WriteLine($"\t{t.X}X1 + {t.Y}X2 {sign} {t.Result}");
            }
            Console.WriteLine("}");
            Console.WriteLine("========================");
        }
    }
}
