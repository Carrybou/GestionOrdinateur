namespace GestionOrdinateurs.API.Service;

public class StockService
{
    private readonly object _lock = new();

    public int StockSSD { get; private set; } = 10;

    public void ConsommerSSD()
    {
        lock (_lock)
        {
            StockSSD--;
        }
    }
}
