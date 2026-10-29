using GestionOrdinateurs.Shared.Models;

namespace GestionOrdinateurs.Shared;

public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    public Ordinateur this[int index] => Ordinateurs[index];

    public Ordinateur? this[string reference] =>
        Ordinateurs.FirstOrDefault(ordinateur =>
            ordinateur.Nom.Equals(reference, StringComparison.OrdinalIgnoreCase));// Recherche par référence (nom)
}