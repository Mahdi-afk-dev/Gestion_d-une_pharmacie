namespace GestionPharmacie;

// Une ligne d'ordonnance : un médicament prescrit, sa quantité et sa posologie.
public class LigneOrdonnance
{
    public int Id { get; set; }
    public int Quantite { get; private set; }
    public string Posologie { get; private set; }
    public Medicament Medicament { get; private set; }
    public LigneOrdonnance() { }
    public LigneOrdonnance(Medicament medicament, int quantite, string posologie)
    {
        ArgumentNullException.ThrowIfNull(medicament);
        if (quantite <= 0)
            throw new ArgumentException("La quantité doit être strictement positive.");

        Medicament = medicament;
        Quantite = quantite;
        Posologie = posologie ?? string.Empty;
    }

    public override string ToString() => $"{Medicament.Nom} x{Quantite} - {Posologie}";
}
