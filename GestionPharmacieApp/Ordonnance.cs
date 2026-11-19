namespace GestionPharmacie;

// Ordonnance prescrite par un médecin pour un client.
// Composition avec LigneOrdonnance (1..*).
public class Ordonnance
{
    // Durée de validité en mois : à confirmer avec le groupe.
    private const int DureeValiditeMois = 3;

    private readonly List<LigneOrdonnance> _lignes = new();

    public int Id { get; private set; }
    public string NumeroOrdonnance { get; set; } = string.Empty;
    public DateTime DateEmission { get; private set; }
    public Medecin Medecin { get; private set; }
    public Client Client { get; private set; }

    public IReadOnlyList<LigneOrdonnance> Lignes => _lignes;
    public Ordonnance() { }
    public Ordonnance(int id, DateTime dateEmission, Medecin medecin, Client client)
    {
        ArgumentNullException.ThrowIfNull(medecin);
        ArgumentNullException.ThrowIfNull(client);

        Id = id;
        DateEmission = dateEmission;
        Medecin = medecin;
        Client = client;
    }

    public void AjouterLigne(LigneOrdonnance ligne)
    {
        ArgumentNullException.ThrowIfNull(ligne);
        _lignes.Add(ligne);
    }

    // Valide = au moins une ligne, pas datée du futur, pas expirée.
    public bool EstValide()
    {
        if (_lignes.Count == 0) return false;
        if (DateEmission > DateTime.Now) return false;
        return DateEmission.AddMonths(DureeValiditeMois) >= DateTime.Now;
    }

    // Indique si l'ordonnance prescrit ce médicament (comparaison par Code via Equals).
    public bool Contient(Medicament medicament) =>
        medicament is not null && _lignes.Any(l => l.Medicament.Equals(medicament));

    public override string ToString() =>
        $"Ordonnance #{Id} du {DateEmission:dd/MM/yyyy} - {Medecin} pour {Client.NomComplet()}";
}
