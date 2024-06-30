using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace dCalculator.ViewModels
{
    public class OperatorItem : ObservableObject
    {
        private string _display;
        public string Display { get => _display; set => SetProperty(ref _display, value); }

        private string _operator;

        public string Operator { get => _operator; set => SetProperty(ref _operator, value); }

        public OperatorItem(string display, string @operator)
        {
            _operator = @operator;
            _display = display ?? @operator;
        }
    }
}
