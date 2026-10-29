using GestionOrdinateurs.Shared.Enums;

namespace GestionOrdinateurs.Shared.Models;

public class Caracteristiques
{
    public int RamGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; set; }
    public int StockageGo { get; set; }

    private Caracteristiques()
    {
    }

    public Caracteristiques(int ramGo, int stockageGo, TypeOrdinateur typeOrdi)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
    }
}