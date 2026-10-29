using FluentAssertions;
using GestionOrdinateurs.Shared.Models;

namespace GestionOrdinateurs.Tests;

public class ReparationTest
{
    [Fact]
    public void Creation_ReparationAvecTechnicien_EstCorrecte()
    {
        var reparation = new Reparation
        {
            OrdinateurId = 99,
            TypeReparation = "Réparation générale",
            Technicien = "Technicien TP4",
            DateDebut = new DateTime(2026, 10, 13),
            Statut = "En cours"
        };

        reparation.OrdinateurId.Should().Be(99);
        reparation.Technicien.Should().Be("Technicien TP4");
        reparation.TypeReparation.Should().Be("Réparation générale");
        reparation.Statut.Should().Be("En cours");
    }
}