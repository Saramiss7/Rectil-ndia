namespace Rectilàndia;

public class RutaData
{
    public List<long> Pendents { get; set; } = new();
    public List<long> Comprovats { get; set; } = new();
    
    //Cada cop que arribem a una ciutat guardem de quina provenim B: A
    public Dictionary<long, long> Anterior { get; set; } = new(); // Cada cop que es trobi una ruta més curta es guardarà la ciutat anterior des de la qual hem arribat
    public Dictionary<long, int> Distancies { get; set; } = new(); // Els kilometres que porta per arribar a cada ciutat
}