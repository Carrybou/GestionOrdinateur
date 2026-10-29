namespace GestionOrdinateurs.Shared.Models;

public class Reparation
{
    public int Id { get; set; }

    public int OrdinateurId { get; set; }

    public Ordinateur UnOrdinateur { get; set; } = null!;

    public string TypeReparation { get; set; } = string.Empty;

    public string Technicien { get; set; } = string.Empty;

    public DateTime DateDebut { get; set; }

    public string Statut { get; set; } = string.Empty;
}