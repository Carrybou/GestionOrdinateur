using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Service;

public record HistoriqueReparationRecord(
    int OrdinateurId,
    string NomOrdinateur,
    string Technicien,
    string TypeReparation,
    DateTime DateIntervention
);

public class ReparationService(ParcInformatiqueContext context, JournalService journalService)
{
    private readonly object _lock = new();

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Réparation générale",
            Technicien = "Technicien TP4",
            DateDebut = DateTime.UtcNow,
            Statut = "En cours"
        };

        lock (_lock)
        {
            context.Reparations.Add(reparation);
        }

        await Task.Delay(TimeSpan.FromSeconds(2));

        reparation.Statut = "Terminée";
        lock (_lock)
        {
            context.SaveChanges();
        }
        journalService.EcrireReparation($"{ordinateur.Nom} réparé");
        Console.WriteLine($"{ordinateur.Nom} réparé");
    }

    public async Task<List<Ordinateur>> GetOrdinateursAsync(int nombre)
    {
        return await context.Ordinateurs
            .OrderBy(ordinateur => ordinateur.Id)
            .Take(nombre)
            .ToListAsync();
    }

    public async Task<List<Reparation>> GetReparationsAsync()
    {
        return await context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .OrderByDescending(reparation => reparation.DateDebut)
            .ToListAsync();
    }

    public async Task<List<HistoriqueReparationRecord>> GetHistoriqueAsync()
    {
        return await context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .OrderByDescending(reparation => reparation.DateDebut)
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.OrdinateurId,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut))
            .ToListAsync();
    }
}
