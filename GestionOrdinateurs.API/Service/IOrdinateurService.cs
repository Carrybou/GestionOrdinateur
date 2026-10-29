using GestionOrdinateurs.API.Event;
using GestionOrdinateurs.Shared.Models;

namespace GestionOrdinateurs.API.Service;

public interface IOrdinateurService
{
    public Task<bool> AddAsync(Ordinateur ordinateur);

    public Task<Ordinateur?> GetByIdAsync(int id);

    public Task<bool> DeleteAsync(int id);

    public Task<Ordinateur?> UpdateAsync(int id, Ordinateur ordinateurUpdate);

    public Task<List<Ordinateur>> GetAllAsync();

    public Task<List<Ordinateur>> GetDisponiblesAsync();

    public Task<List<Ordinateur>> GetByMarqueAsync(string marque);

    public Task<List<Ordinateur>> GetDuPlusCherAuMoinsCherAsync();

    public string Diagnostiquer(Ordinateur ordinateur);

    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques();

    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;

    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

}