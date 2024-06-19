using System.Windows;

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
        this.DialogResult = true;
        // валидацию где?
    }

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
}
