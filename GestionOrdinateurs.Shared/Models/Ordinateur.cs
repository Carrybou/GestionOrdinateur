using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using GestionOrdinateurs.Shared.Enums;

namespace GestionOrdinateurs.Shared.Models;

[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; private set; }
    public string Nom { get; private set; } = null!;
    public string Marque { get; init; } = null!;

    [DefaultValue(0)]
    public int Stock { get; private set; }

    [DefaultValue(0)]
    public decimal Prix { get; private set; }

    public bool Vendu { get; private set; } = false;

    public EtatOrdinateur Etat { get; private set; }
    public Caracteristiques Caracteristiques { get; private set; } = null!; // Association avec la classe Caracteristiques
    public List<Reparation> Reparations { get; set; } = new();

    private Ordinateur()
    {
    }

    public Ordinateur(string nom, string marque, Caracteristiques caracteristiques, int stock = 1, decimal prix = 0)
    {
        Nom = nom;
        Marque = marque;
        Caracteristiques = caracteristiques;
        Stock = stock;
        Prix = prix;
        Etat = EtatOrdinateur.EnService;

    }

    public void ChangerEtat(EtatOrdinateur nouvelEtat)
    {
        Etat = nouvelEtat;
    }

    public void Renommer(string nom)
    {
        Nom = nom;
    }

    public void ModifierStock(int stock)
    {
        Stock = stock;
    }

    public void ModifierPrix(decimal prix)
    {
        Prix = prix;
    }
    

    public override string ToString()
    {
        return $"[{Id}] {Nom} - {Marque} | {Caracteristiques.TypeOrdi} | {Caracteristiques.RamGo} Go RAM | Stock: {Stock} | Prix: {Prix} | {Etat}";
    }
    
}