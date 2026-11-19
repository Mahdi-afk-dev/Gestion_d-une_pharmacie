using System.ComponentModel.DataAnnotations;
namespace GestionPharmacie;

// Une livraison d'un médicament, avec sa date de péremption.
public class Lot
{
    private int _quantite;
    [Key]
    public string NumeroLot { get; set; }
    public DateTime DateExpiration { get; set; }

    // Encapsulation : la quantité ne peut jamais être négative.
    public int Quantite
    {
        get => _quantite;
        set
        {
            if (value < 0)
                throw new ArgumentException("La quantité d'un lot ne peut pas être négative.");
            _quantite = value;
        }
    }
    public Lot() { }
    public Lot(string numeroLot, DateTime dateExpiration, int quantite)
    {
        if (string.IsNullOrWhiteSpace(numeroLot))
            throw new ArgumentException("Le numéro de lot est obligatoire.");

        NumeroLot = numeroLot;
        DateExpiration = dateExpiration;
        Quantite = quantite;
    }

    public bool EstPerime() => DateExpiration.Date < DateTime.Today;

    public override string ToString() =>
        $"Lot {NumeroLot} - {Quantite} unité(s) - expire le {DateExpiration:dd/MM/yyyy}";
}
