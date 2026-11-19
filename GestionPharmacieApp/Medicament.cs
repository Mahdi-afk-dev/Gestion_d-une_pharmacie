using System.ComponentModel.DataAnnotations;
namespace GestionPharmacie;

// Fiche produit du catalogue. Ne contient PAS la quantité (rôle du Stock).
public class Medicament : IEquatable<Medicament>
{
    private decimal _prixUnitaire;
    private int _seuilAlerte;
    [Key]
    public string Code { get; set; }
    public string Nom { get; set; }
    public string Categorie { get; set; }
    public bool SurOrdonnance { get; set; }
    public Fournisseur? Fournisseur { get; set; }

    public decimal PrixUnitaire
    {
        get => _prixUnitaire;
        set
        {
            if (value < 0)
                throw new ArgumentException("Le prix ne peut pas être négatif.");
            _prixUnitaire = value;
        }
    }

    // Quantité minimale avant de déclencher une alerte de rupture.
    public int SeuilAlerte
    {
        get => _seuilAlerte;
        set
        {
            if (value < 0)
                throw new ArgumentException("Le seuil d'alerte ne peut pas être négatif.");
            _seuilAlerte = value;
        }
    }
    public Medicament() { }
    public Medicament(string code, string nom, string categorie, decimal prixUnitaire,
                      bool surOrdonnance, int seuilAlerte, Fournisseur? fournisseur = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Le code est obligatoire.");

        Code = code;
        Nom = nom;
        Categorie = categorie;
        PrixUnitaire = prixUnitaire;
        SurOrdonnance = surOrdonnance;
        SeuilAlerte = seuilAlerte;

        if (fournisseur != null)
            fournisseur.AjouterMedicament(this);
    }

    // Deux médicaments sont identiques s'ils ont le même code
    // (nécessaire pour l'utiliser comme clé dans le Stock).
    public bool Equals(Medicament? autre) => autre is not null && Code == autre.Code;
    public override bool Equals(object? obj) => Equals(obj as Medicament);
    public override int GetHashCode() => Code.GetHashCode();

    public override string ToString() => $"{Code} - {Nom} ({PrixUnitaire:0.000} DT)";
}
