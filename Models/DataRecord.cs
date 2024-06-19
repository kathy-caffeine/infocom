namespace WpfApp.Models;

public class DataRecord
{
    public required string CarId { get; set; }
    public required double GrossWeight { get; set; }
    public required double TareWeight { get; set; }
    public required double NetWeight { get; set; }
    public required DateOnly TareDate { get; set; }
    public required DateOnly GrossDate { get; set; }
}
