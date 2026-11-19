namespace GestionPharmacie;

// Une ligne d'une vente : un médicament, une quantité et le prix unitaire figé au moment de la vente.
public class LigneVente
{
    public int Id { get; set; }
    public int Quantite { get; private set; }
    public decimal PrixUnitaire { get; private set; }
    public Medicament Medicament { get; private set; }
    public LigneVente() { }
    public LigneVente(Medicament medicament, int quantite)
    {
        ArgumentNullException.ThrowIfNull(medicament);
        if (quantite <= 0)
            throw new ArgumentException("La quantité doit être strictement positive.");

        Medicament = medicament;
        Quantite = quantite;
        // Le prix du médicament peut changer plus tard : on le fige ici.
        PrixUnitaire = medicament.PrixUnitaire;
    }

    // Ajoute une quantité à une ligne existante (même médicament).
    public void AugmenterQuantite(int quantite)
    {
        if (quantite <= 0)
            throw new ArgumentException("La quantité doit être strictement positive.");
        Quantite += quantite;
    }

    public decimal SousTotal() => Quantite * PrixUnitaire;

    public override string ToString() =>
        $"{Medicament.Nom} x{Quantite} @ {PrixUnitaire:0.000} = {SousTotal():0.000} DT";
}
