using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace WpfApp.Views;

/// <summary>
/// Логика взаимодействия для Input.xaml
/// </summary>
public partial class Input : Window
{
    public Input()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var crunch = true;
        string reg;
        if (carId != null)
        {
            reg = @"^[A-Z]{1}\d{3}[A-Z]{2}$";
            if (string.IsNullOrEmpty(carId.Text) || !Regex.IsMatch(carId.Text, reg))
            {
                crunch = false;
                carId.BorderBrush = Brushes.Red;
            }
        }
        reg = @"^\d+(?:[\.,]\d{1,3}|)$";
        if (!Regex.IsMatch(grossWeight.Text, reg))
        {
            crunch = false;
            grossWeight.BorderBrush = Brushes.Red;
        }
        if(!Regex.IsMatch(tareWeight.Text, reg))
        {
            crunch = false;
            tareWeight.BorderBrush = Brushes.Red;
        }
        if(!Regex.IsMatch(netWeight.Text, reg))
        {
            crunch = false;
            netWeight.BorderBrush = Brushes.Red;
        }
        if(string.IsNullOrEmpty(tareDate.Text))
        {
            crunch = false;
            tareDate.BorderBrush = Brushes.Red;
        }
        if(string.IsNullOrEmpty(grossDate.Text))
        {
            crunch = false;
            grossDate.BorderBrush = Brushes.Red;
        }

        if(crunch) this.DialogResult = true;
    }

    #region
    public string CarId
    {
        get { return carId.Text; }
    }
    public string GrossWeight
    {
        get { return grossWeight.Text; }
    }
    public string TareWeight
    {
        get { return tareWeight.Text; }
    }
    public string NetWeight
    {
        get { return netWeight.Text; }
    }
    public string GrossDate
    {
        get { return grossDate.Text; }
    }
    public string TareDate
    {
        get { return tareDate.Text; }
    }
    #endregion
}
