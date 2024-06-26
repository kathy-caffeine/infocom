﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using WpfApp.Models;
using WpfApp.Views;
using System.Data.SqlClient;

namespace WpfApp;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<DataRecord> _dataRecords;
    public ObservableCollection<DataRecord> DataRecords
    {
        get => _dataRecords;
        set
        {
            _dataRecords = value;
            OnPropertyChanged(nameof(DataRecords));
        }
    }

    private SeriesCollection _seriesCollection;
    public SeriesCollection SeriesCollection
    {
        get => _seriesCollection;
        set
        {
            _seriesCollection = value;
            OnPropertyChanged(nameof(SeriesCollection));
        }
    }

    private ObservableCollection<string> _carIds;
    public ObservableCollection<string> CarIds
    {
        get => _carIds;
        set
        {
            _carIds = value;
            OnPropertyChanged(nameof(CarIds));
        }
    }

    private readonly SqlConnection _sqlConnection = new SqlConnection(@"Data Source=WIN-2EF5KQG48GQ\SQLEXPRESS;Initial Catalog=DataRecord;Integrated Security=True");

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        DataRecords = new ObservableCollection<DataRecord>();
        SeriesCollection = new SeriesCollection();
        CarIds = new ObservableCollection<string>();

        dataGrid.ItemsSource = DataRecords;
        chart.DataContext = this;
        Task.Run(LoadDataAsync);
    }


    private void AddRandomButton_Click(object sender, RoutedEventArgs e)
    {
        var newRecord = CreateRandomRecord();
        DataRecords.Add(newRecord);
        UpdateSeriesCollection();
        AddRecordToDB(newRecord);
    }

    private void AddCustomButton_Click(object sender, RoutedEventArgs e)
    {
        var inputWindow = new Input();
        if(inputWindow.ShowDialog() is true)
        {
            if(inputWindow.DialogResult is true)
            {
                var newRecord = new DataRecord 
                { 
                    CarId = inputWindow.CarId,
                    GrossWeight = Convert.ToDouble(inputWindow.GrossWeight),
                    TareWeight = Convert.ToDouble(inputWindow.TareWeight),
                    NetWeight = Convert.ToDouble(inputWindow.NetWeight),
                    GrossDate = DateOnly.Parse(inputWindow.GrossDate),
                    TareDate = DateOnly.Parse(inputWindow.TareDate),
                };
                DataRecords.Add(newRecord);
                UpdateSeriesCollection();
                AddRecordToDB(newRecord);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private static string GenerateCarId()
    {
        var random = new Random();
        char firstLetter = (char)random.Next('A', 'Z' + 1);
        int digits = random.Next(100, 1000); // Ensure 3 digits
        string lastTwoLetters = new string(new char[]
        {
                (char)random.Next('A', 'Z' + 1),
                (char)random.Next('A', 'Z' + 1)
        });

        return $"{firstLetter}{digits}{lastTwoLetters}";
    }

    private DataRecord CreateRandomRecord()
    {
        var random = new Random();
        var newRecord = new DataRecord
        {
            CarId = GenerateCarId(),
            GrossWeight = Math.Round(random.NextDouble() * 100, 3),
            TareWeight = Math.Round(random.NextDouble() * 50, 3),
            NetWeight = Math.Round(random.NextDouble() * 75, 3),
            TareDate = DateOnly.FromDateTime(DateTime.Now),
            GrossDate = DateOnly.FromDateTime(DateTime.Now)
        };
        return newRecord;
    }

    private void UpdateSeriesCollection()
    {
        SeriesCollection.Clear();
        CarIds.Clear();

        foreach (var record in DataRecords)
        {
            if (!CarIds.Contains(record.CarId))
            {
                CarIds.Add(record.CarId);
            }
        }

        var grossWeightSeries = new ColumnSeries
        {
            Title = "Gross Weight",
            Values = new ChartValues<double>(DataRecords.Select(record => record.GrossWeight))
        };

        var tareWeightSeries = new ColumnSeries
        {
            Title = "Tare Weight",
            Values = new ChartValues<double>(DataRecords.Select(record => record.TareWeight))
        };

        var netWeightSeries = new ColumnSeries
        {
            Title = "Net Weight",
            Values = new ChartValues<double>(DataRecords.Select(record => record.NetWeight))
        };

        SeriesCollection.Add(grossWeightSeries);
        SeriesCollection.Add(tareWeightSeries);
        SeriesCollection.Add(netWeightSeries);
        chart.Update();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            if (_sqlConnection.State is not System.Data.ConnectionState.Closed)
                _sqlConnection.Close();
            _sqlConnection.Open();
            var command = "select * from data_records;";
            SqlCommand _sqlCommand = new SqlCommand(command, _sqlConnection);
            SqlDataReader reader = _sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var record = new DataRecord
                {
                    CarId = (string)reader["car_id"],
                    GrossWeight = Convert.ToDouble(reader["gross_weight"]),
                    TareWeight = Convert.ToDouble(reader["tare_weight"]),
                    NetWeight = Convert.ToDouble(reader["net_weight"]),
                    GrossDate = DateOnly.FromDateTime((DateTime)reader["gross_date"]),
                    TareDate = DateOnly.FromDateTime((DateTime)reader["tare_date"])
                };
                await Dispatcher.InvokeAsync(() =>
                {
                    DataRecords.Add(record);
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _sqlConnection.Close();
        }
        await Dispatcher.InvokeAsync(() =>
        {
            UpdateSeriesCollection();
            chart.Update();
        });
    }

    private bool AddRecordToDB(DataRecord newRecord)
    {
        bool result;
        if (_sqlConnection.State is not System.Data.ConnectionState.Closed)
            _sqlConnection.Close();
        _sqlConnection.Open();
        var _sqlTransaction = _sqlConnection.BeginTransaction();
        try
        {
            var command = "insert into data_records(car_id, gross_weight, tare_weight, net_weight, gross_date, tare_date) values (@car_id, @gross_weight, @tare_weight, @net_weight, @gross_date, @tare_date)";
            SqlCommand _sqlCommand = new SqlCommand(command, _sqlConnection, _sqlTransaction);
            _sqlCommand.Parameters.AddWithValue("@car_id", newRecord.CarId);
            _sqlCommand.Parameters.AddWithValue("@gross_weight", Convert.ToDecimal(newRecord.GrossWeight));
            _sqlCommand.Parameters.AddWithValue("@tare_weight", Convert.ToDecimal(newRecord.TareWeight));
            _sqlCommand.Parameters.AddWithValue("@net_weight", Convert.ToDecimal(newRecord.NetWeight));
            _sqlCommand.Parameters.AddWithValue("@gross_date", newRecord.GrossDate.ToDateTime(TimeOnly.MinValue));
            _sqlCommand.Parameters.AddWithValue("@tare_date", newRecord.TareDate.ToDateTime(TimeOnly.MinValue));
            _sqlCommand.ExecuteNonQuery();
            _sqlTransaction.Commit();
            result = true;
        }
        catch
        {
            _sqlTransaction.Rollback();
            result = false;
        }
        finally
        {
            _sqlConnection.Close();
        }
        return result;
    }
}
