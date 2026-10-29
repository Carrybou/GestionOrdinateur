using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.Shared.Enums;
using GestionOrdinateurs.Shared.Models;

namespace GestionOrdinateurs.API.Service;

public static class DbInitializer
{
    public static void Initialize(ParcInformatiqueContext context)
    {
        if (context.Ordinateurs.Any())
        {
            return;
        }

        //Ajout des ordinateurs
        context.Ordinateurs.AddRange([
            new Ordinateur("ROGUE", "ASUS", new Caracteristiques(16, 512, TypeOrdinateur.Fixe), prix: 1499),
            new Ordinateur("ROGUE I", "ASUS", new Caracteristiques(16, 512, TypeOrdinateur.Fixe), prix: 1699),
            new Ordinateur("MacBook Pro", "APPLE", new Caracteristiques(32, 1000, TypeOrdinateur.Portable), prix: 2199),
            new Ordinateur("ROGUE II", "ASUS", new Caracteristiques(16, 512, TypeOrdinateur.Fixe), prix: 1299),
            new Ordinateur("Test", "DELL", new Caracteristiques(16, 512, TypeOrdinateur.Server), prix: 799),
        ]);
        context.SaveChanges();
    }
}