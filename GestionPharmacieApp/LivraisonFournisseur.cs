using System;

namespace GestionPharmacie;

public class LivraisonFournisseur
{
    public int Id { get; set; }
    public Fournisseur Fournisseur { get; set; } = null!;
    public Medicament Medicament { get; set; } = null!;
    public int QuantiteRecue { get; set; }
    public DateTime DateReception { get; set; }

    public LivraisonFournisseur() { }
}