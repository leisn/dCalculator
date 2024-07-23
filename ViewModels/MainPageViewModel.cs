// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.ObjectModel;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using dCalculator.Bases;
using dCalculator.Infrastructures;


namespace dCalculator.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private long _currentValue;
    [ObservableProperty]
    private string _expressionText = "";
    private RadixValue _selectedValue = null!;

    public RadixValue SelectedValue { get => _selectedValue; set { if (SetProperty(ref _selectedValue, value)) UpdateExpression(); } }
    public ObservableCollection<RadixValue> Values { get; }
    public RelayCommand<string> OperatorCommand { get; }

    private bool _numberChanged;

    private readonly ExpressionHandler _expressionHandler = new();
    public MainPageViewModel()
    {
        OperatorCommand = new RelayCommand<string>(OnOperatorClicked);
        Values =
        [
            new RadixValue(16, "HEX",
                           v => RadixValue.InsertString(Convert.ToString(v, 16).ToUpperInvariant(), 4, "  ")),
            new RadixValue(10, "DEC",
                          (v, s) => (v * 10) + int.Parse(s),
                           v => v/10,
                           v => RadixValue.InsertString(v.ToString(), 3, ", ")),
            new RadixValue(8, "OCT",
                           v => RadixValue.InsertString(Convert.ToString(v, 8), 3, "  ")),
            new RadixValue(2, "BIN",
                           v => RadixValue.InsertString(Convert.ToString(v, 2), 8, "  "))
        ];
    }

    private void UpdateExpression()
    {
        if (SelectedValue == null)
            return;
        ExpressionText = _expressionHandler.ToDisplay(SelectedValue.Radix);
    }

    private async void OnOperatorClicked(string? op)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(op, nameof(op));

            if (Equals("+/-", op))
            {
                SwitchSign();
                return;
            }

            if (Equals("Cl", op))
            {
                ClearValues();
                return;
            }

            if (Equals("Del", op))
            {
                DelClicked();
                return;
            }

            if (ExpressionHandler.IsNumber(op))
            {
                NumberClicked(op);
                return;
            }

            if (ExpressionHandler.IsOperator(op))
            {
                HandleOperator(op);
                return;
            }

            if (Equals("=", op))
            {
                EndExpression();
                return;
            }
        }
        catch (Exception ex)
        {
            await Toast.Make($"Error: {ex}").Show();
        }
        finally
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
    }

    private void NumberClicked(string number)
    {
        if (!_numberChanged && _expressionHandler.IsLastOperator)
            UpdateValues(0);
        UpdateValues(SelectedValue.AppendValue(number));
        _numberChanged = true;
    }

    private void DelClicked()
    {
        if (SelectedValue.Value == 0)
        {
            _expressionHandler.RemoveLast();
            UpdateExpression();
            return;
        }
        UpdateValues(SelectedValue.DelValue());
        _numberChanged = true;
    }

    private void SwitchSign()
    {
        foreach (var item in Values)
            item.SwitchSign();
        _numberChanged = true;
    }

    private void ClearValues()
    {
        if (SelectedValue.Value == 0)
        {
            _expressionHandler.Clear();
            UpdateExpression();
            _numberChanged = false;
        }
        else
        {
            UpdateValues(0);
            _numberChanged = true;
        }
    }

    private void UpdateValues(long value)
    {
        foreach (var item in Values)
            item.SetValue(value);
    }

    private void HandleOperator(string op)
    {
        try
        {
            if (!_numberChanged)
            {
                if (_expressionHandler.IsLastOperator)
                {
                    _expressionHandler.Append(op, SelectedValue.Value);
                    return;
                }
            }
            _expressionHandler.Append(op, SelectedValue.Value);
            _numberChanged = false;
        }
        finally
        {
            UpdateExpression();
        }
    }


    private async void EndExpression()
    {
        try
        {
            _expressionHandler.End(SelectedValue.Value);
            UpdateExpression();
            UpdateValues(_expressionHandler.GetResult());
        }
        catch (Exception ex)
        {
            await Toast.Make($"Error: \n{ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long)
                       .Show();
        }
        _numberChanged = true;
    }

}
