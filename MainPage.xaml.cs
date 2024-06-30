using dCalculator.ViewModels;

namespace dCalculator
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            decimalRadio.IsChecked = true;
        }
    }

}
