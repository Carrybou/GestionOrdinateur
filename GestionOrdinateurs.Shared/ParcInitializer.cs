using GestionOrdinateurs.Shared.Models;
using GestionOrdinateurs.Shared.Enums;


namespace GestionOrdinateurs.Shared;

public static class ParcInitializer
{
    public static List<Ordinateur> ordinateurs =
    [
        new("PC1", "ASUS", new Caracteristiques(16, 512, TypeOrdinateur.Fixe)),
        new("PC2", "ASUS", new Caracteristiques(64, 1000, TypeOrdinateur.Portable)),
        new("PC3", "APPLE", new Caracteristiques(64, 1000, TypeOrdinateur.Portable)),
        new("PC4", "DELL", new Caracteristiques(64, 1000, TypeOrdinateur.Fixe)),
        new("PC5", "ThinkPad", new Caracteristiques(64, 1000, TypeOrdinateur.Portable))
    ];
}
