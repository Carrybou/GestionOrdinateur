using GestionOrdinateurs.API.Service;
using GestionOrdinateurs.API.Context;
using GestionOrdinateurs.Shared.Enums;
using GestionOrdinateurs.Shared.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest
{
    private readonly OrdinateurService _service;

    public OrdinateurServiceTest()
    {
        var context = CreerContexte();
        _service = new OrdinateurService(context);
    }

    private static ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    [Fact]
    public async Task Add_OrdinateurValide_EstAjouteALaListe()
    {
        var ordinateur = new Ordinateur("PC-Test", "HP", new Caracteristiques(8, 256, TypeOrdinateur.Portable));

        var resultat = await _service.AddAsync(ordinateur);

        resultat.Should().BeTrue();
        (await _service.GetByIdAsync(ordinateur.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task Add_OrdinateurNull_RetourneFalse()
    {
        var resultat = await _service.AddAsync(null!);

        resultat.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_OrdinateurExistant_RetourneTrueEtLeRetire()
    {
        var ordinateur = new Ordinateur("PC-ASupprimer", "Lenovo", new Caracteristiques(16, 512, TypeOrdinateur.Fixe));
        await _service.AddAsync(ordinateur);

        var resultat = await _service.DeleteAsync(ordinateur.Id);

        resultat.Should().BeTrue();
        (await _service.GetByIdAsync(ordinateur.Id)).Should().BeNull();
    }

    [Fact]
    public async Task Delete_OrdinateurInexistant_RetourneFalse()
    {
        var resultat = await _service.DeleteAsync(-1);

        resultat.Should().BeFalse();
    }

    [Fact]
    public async Task GetByMarque_RetourneUniquementLesOrdinateursCorrespondants()
    {
        await _service.AddAsync(new Ordinateur("PC-HP", "HP", new Caracteristiques(8, 256, TypeOrdinateur.Portable)));
        await _service.AddAsync(new Ordinateur("PC-Lenovo", "Lenovo", new Caracteristiques(16, 512, TypeOrdinateur.Fixe)));

        var resultats = await _service.GetByMarqueAsync(" hp ");

        resultats.Should().ContainSingle();
        resultats[0].Marque.Should().Be("HP");
    }

    [Fact]
    public async Task GetDuPlusCherAuMoinsCher_RetourneLesOrdinateursTriesParPrix()
    {
        await _service.AddAsync(new Ordinateur("PC-Abordable", "HP", new Caracteristiques(8, 256, TypeOrdinateur.Portable), prix: 500));
        await _service.AddAsync(new Ordinateur("PC-Premium", "Lenovo", new Caracteristiques(16, 512, TypeOrdinateur.Fixe), prix: 1500));

        var resultats = await _service.GetDuPlusCherAuMoinsCherAsync();

        resultats.Select(ordinateur => ordinateur.Prix)
            .Should()
            .ContainInOrder(1500, 500);
    }

    [Fact]
    public async Task Update_ModifieLeStockEtLePrix()
    {
        var ordinateur = new Ordinateur("PC-Test", "HP", new Caracteristiques(8, 256, TypeOrdinateur.Portable), stock: 1, prix: 500);
        await _service.AddAsync(ordinateur);

        var ordinateurMisAJour = new Ordinateur("PC-Test", "HP", new Caracteristiques(8, 256, TypeOrdinateur.Portable), stock: 4, prix: 750);
        var resultat = await _service.UpdateAsync(ordinateur.Id, ordinateurMisAJour);

        resultat.Should().NotBeNull();
        resultat!.Stock.Should().Be(4); // Vérifie que le stock a été mis à jour
        resultat.Prix.Should().Be(750); // Vérifie que le prix a été mis à jour
    }


    [Fact]
// ajout de test car l'endpoint etat n'existe pas donc pas modifiable sur le swagger
    public void Diagnostiquer_OrdinateurLentAvecPeuDeRam_RetourneAjouterRam() 
    {
        var ordinateur = new Ordinateur(
            "PC-Lent",
            "HP",
            new Caracteristiques(7, 256, TypeOrdinateur.Portable));
        ordinateur.ChangerEtat(EtatOrdinateur.Lent);

        var resultat = _service.Diagnostiquer(ordinateur);

        resultat.Should().Be("Ajouter RAM");
    }

    [Fact]
    public void Diagnostiquer_OrdinateurHorsService_RetourneRemplacement()
    {
        var ordinateur = new Ordinateur(
            "PC-Hors-Service",
            "Dell",
            new Caracteristiques(16, 512, TypeOrdinateur.Fixe));
        ordinateur.ChangerEtat(EtatOrdinateur.HorsService);

        var resultat = _service.Diagnostiquer(ordinateur);

        resultat.Should().Be("Remplacement");
    }
}