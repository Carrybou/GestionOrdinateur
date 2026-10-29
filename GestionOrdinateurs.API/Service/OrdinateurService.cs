using GestionOrdinateurs.Shared.Models;
using GestionOrdinateurs.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.API.Event;

namespace GestionOrdinateurs.API.Service;

public class OrdinateurService(ParcInformatiqueContext context): IOrdinateurService
{

    private readonly DbSet<Ordinateur> _ordinateurs = context.Ordinateurs;
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;


    public async Task<bool> AddAsync(Ordinateur ordinateur) //Ajouter un ordinateur
    {
        if (ordinateur is null) return false;

        _ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();
        
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        return true;
    }

    public Task<Ordinateur?> GetByIdAsync(int id)
    {
        return _ordinateurs.FirstOrDefaultAsync(ordinateur => ordinateur.Id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ordinateur = await GetByIdAsync(id);
        if (ordinateur is null) return false;

        _ordinateurs.Remove(ordinateur);
        await context.SaveChangesAsync();
        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
        return true;
    }

    public async Task<Ordinateur?> UpdateAsync(int id, Ordinateur ordinateurUpdate)
    {
        var ordinateur = await GetByIdAsync(id);
        if (ordinateur is null) return null;

        ordinateur.Renommer(ordinateurUpdate.Nom);
        ordinateur.Caracteristiques.RamGo = ordinateurUpdate.Caracteristiques.RamGo;
        ordinateur.Caracteristiques.StockageGo = ordinateurUpdate.Caracteristiques.StockageGo;
        ordinateur.Caracteristiques.TypeOrdi = ordinateurUpdate.Caracteristiques.TypeOrdi;
        ordinateur.ModifierStock(ordinateurUpdate.Stock);
        ordinateur.ModifierPrix(ordinateurUpdate.Prix);
        ordinateur.ChangerEtat(ordinateurUpdate.Etat);

        await context.SaveChangesAsync();
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(ordinateur));
        return ordinateur;
    }

    public Task<List<Ordinateur>> GetAllAsync()
    {
        return _ordinateurs.ToListAsync();
    }

    public Task<List<Ordinateur>> GetDisponiblesAsync()
    {
        return _ordinateurs
            .Where(ordinateur => ordinateur.Stock > 0)
            .ToListAsync();
    }

    public Task<List<Ordinateur>> GetByMarqueAsync(string marque)
    {
        if (string.IsNullOrWhiteSpace(marque))
        {
            return Task.FromResult(new List<Ordinateur>());
        }

        var marqueRecherchee = marque.Trim().ToUpper();

        return _ordinateurs
            .Where(ordinateur => ordinateur.Marque.ToUpper() == marqueRecherchee)
            .ToListAsync();
    }

    public Task<List<Ordinateur>> GetDuPlusCherAuMoinsCherAsync()
    {
        return _ordinateurs
            .OrderByDescending(ordinateur => ordinateur.Prix)
            .ToListAsync();
    }

    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",
            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques: { RamGo: < 8 }
            } => "Ajouter RAM",
            _ => "Aucune action"
        };
    }

    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques()
    {
        return (
            _ordinateurs.Count(),
            _ordinateurs.Count(ordinateur => ordinateur.Stock > 0),
            _ordinateurs.Sum(ordinateur => ordinateur.Stock * ordinateur.Prix),
            context.Reparations.Count());
    }
    
}