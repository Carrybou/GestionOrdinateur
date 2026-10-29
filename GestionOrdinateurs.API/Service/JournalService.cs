namespace GestionOrdinateurs.API.Service;

public class JournalService
{
    private readonly object _lock = new();

    public void EcrireReparation(string message)
    {
        Ecrire("Logs/Reparations/reparations.log", message);
    }

    public void EcrireOrdinateur(string message)
    {
        Ecrire("Logs/Ordinateurs/ordinateurs.log", message);
    }

    private void Ecrire(string chemin, string message)
    {
        lock (_lock)
        {
            using var writer = new StreamWriter(chemin, append: true);
            writer.WriteLine($"{DateTime.Now:dd/MM/yyyy} - {message}");
        }
    }
}