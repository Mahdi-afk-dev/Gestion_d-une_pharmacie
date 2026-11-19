namespace GestionPharmacie;

public class Fournisseur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;

    // Un fournisseur fournit plusieurs médicaments (1 -> *).
    private readonly List<Medicament> _medicaments = new();
    public IReadOnlyList<Medicament> Medicaments => _medicaments;

    public Fournisseur(int id, string nom, string telephone, string adresse)
    {
        Id = id;
        Nom = nom;
        Telephone = telephone;
        Adresse = adresse;
    }

    public void AjouterMedicament(Medicament medicament)
    {
        ArgumentNullException.ThrowIfNull(medicament);
        if (_medicaments.Contains(medicament)) return;

        _medicaments.Add(medicament);
        medicament.Fournisseur = this;
    }

    public override string ToString() => $"[{Id}] {Nom}";
}
