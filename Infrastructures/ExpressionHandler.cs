// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Text;

namespace dCalculator.Infrastructures;

public class ExpressionHandler
{
    #region statics
    public const string Left_Parentheses = "(";
    public const string Right_Parentheses = ")";
    public const string Bit_Not = "~";
    public const string Equal = "=";
    public static readonly string[] Numbers = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "A", "B", "C", "D", "E", "F"];
    public static readonly string[] Operators = ["<<", ">>", "~", "&", "|", "^", "%", "+", "-", "*", "/", "(", ")"];
    public static readonly string[] ConnectorOperators = ["%", "+", "-", "*", "/", "<<", ">>", "&", "|"];
    public static bool IsNumber(string? op)
    {
        return Numbers.Contains(op);
    }

    public static bool IsOperator(string? op)
    {
        return Operators.Contains(op);
    }

    public static bool IsConnector(string? op)
    {
        return ConnectorOperators.Contains(op);
    }
    #endregion

    private bool _isEnded;
    private readonly List<string> _array = [];

    public bool IsLastConnector => IsConnector(_array.LastOrDefault());
    public bool IsLastOperator => IsOperator(_array.LastOrDefault());

    public long GetResult()
    {
        return ExpressionExecutor.GetResult(ToExpression());
    }

    public void End(long value)
    {
        if (_isEnded)
            return;
        var lps = GetLeftParenthesesCount();
        if (lps > 0)
        {
            var rps = GetRightParenthesesCount();
            var len = rps - lps;
            if (len > 0)
            {
                for (var i = 0; i < len; i++)
                    AppendOperator(Right_Parentheses);
            }
        }
        if (IsLastConnector)
            AppendValue(value);
        AppendOperator(Equal);
        _isEnded = true;
    }

    public void Append(string op, long value)
    {
        if (op == Right_Parentheses && GetRightParenthesesCount() >= GetLeftParenthesesCount())
            return;

        bool isConnector = IsConnector(op);
        var last = _array.LastOrDefault();
        if (last == null)
        {
            if (isConnector)
            {
                AppendValue(value);
                AppendOperator(op);
                return;
            }
            if (op == Bit_Not)
            {
                AppendOperator(op);
                AppendValue(value);
                return;
            }
            AppendOperator(op);
            return;
        }

        bool isLastOperator = IsConnector(last);
        if (!isLastOperator && !isConnector)
        {
            if (op == Left_Parentheses)
                AppendOperator("*");
            else
                AppendOperator(op);
            return;
        }

        if (op == Left_Parentheses)
        {
            AppendOperator(op);
            return;
        }
        if (op == Right_Parentheses)
        {
            AppendValue(value);
            AppendOperator(op);
            return;
        }
        if (isConnector)
        {
            if (last == Right_Parentheses)
            {
                AppendOperator(op);
                return;
            }
            AppendValue(value);
            AppendOperator(op);
            return;
        }
    }

    private void AppendOperator(string op)
    {
        _array.Add(op);
    }

    private void AppendValue(long value)
    {
        _array.Add(value.ToString());
    }

    private int GetLeftParenthesesCount()
    {
        return _array.Count(x => x == Left_Parentheses);
    }

    private int GetRightParenthesesCount()
    {
        return _array.Count(x => x == Right_Parentheses);
    }

    public void RemoveLast()
    {
        if (_array.Count <= 0)
            return;
        _array.RemoveAt(_array.Count - 1);
    }

    public void Clear()
    {
        _array.Clear();
        _isEnded = false;
    }

    public string ToDisplay(int toBase = 10)
    {
        if (_array.Count == 0)
            return string.Empty;
        StringBuilder sb = new();
        for (int i = 0; i < _array.Count; i++)
        {
            var item = _array[i];
            if (item == Equal)
                _ = sb.Append(item);
            else if (IsOperator(item))
            {
                if (item == "*")
                    _ = sb.Append('×');
                else if (item == "/")
                    _ = sb.Append('÷');
                else
                    _ = sb.Append(item);
            }
            else
                _ = sb.Append(Convert.ToString(Convert.ToInt64(item), toBase).ToUpperInvariant());

            if (i != _array.Count - 1)
                _ = sb.Append(' ');
        }
        return sb.ToString();
    }

    private string ToExpression()
    {
        if (_array.Count == 0)
            return "0";
        StringBuilder sb = new();
        for (int i = 0; i < _array.Count; i++)
        {
            var item = _array[i];
            if (item == Equal)
                break;
            _ = sb.Append(item);
        }
        return sb.ToString();
    }
}
