using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class CalculatorEngine
    {
        private List<string> tokens = new List<string> { "0" };
        private bool justCalculated = false;

        public string Expression => string.Join(" ", tokens);

        public void Clear()
        {
            tokens.Clear();
            tokens.Add("0");
            justCalculated = false;
        }

        public void AddNumber(string number)
        {
            if (justCalculated) Clear();

            if (tokens.Count == 0)
            {
                tokens.Add(number == "." ? "0." : number);
                return;
            }

            if (tokens[^1] == "0" && number != ".")
                tokens[^1] = number;
            else
                tokens[^1] += number;
        }

        public void AddOperator(string op)
        {
            if (tokens.Count == 0 && op == "-")
            {
                tokens.Add(op);
                return;
            }

            if (IsOperator(tokens[^1]))
                tokens[^1] = op;
            else
            {
                tokens.Add(op);
                tokens.Add("");
            }

            justCalculated = false;
        }

        public void ToggleSign()
        {
            if (double.TryParse(tokens[^1], out double value))
                tokens[^1] = (-value).ToString(CultureInfo.InvariantCulture);
        }

        public void Percent()
        {
            if (double.TryParse(tokens[^1], out double value))
                tokens[^1] = (value / 100).ToString(CultureInfo.InvariantCulture);
        }

        public string Calculate()
        {
            try
            {
                string expr = Expression.Replace("×", "*").Replace("÷", "/");

                if (HasDivisionByZero(expr))
                {
                    Clear();
                    return "Division by zero";
                }

                double result = Evaluate(expr);

                Clear();
                tokens[^1] = result.ToString(CultureInfo.InvariantCulture);
                justCalculated = true;
                return result.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                Clear();
                return "Invalid expression";
            }
        }

        private static bool IsOperator(string token) => token == "+" || token == "-" || token == "×" || token == "÷";

        private static bool HasDivisionByZero(string expression)
        {
            var matches = Regex.Matches(expression, @"/\s*0+(\.0+)?");
            return matches.Count > 0;
        }

        private static double Evaluate(string expression)
        {
            var table = new System.Data.DataTable();
            return Convert.ToDouble(table.Compute(expression, string.Empty));
        }
    }
}
