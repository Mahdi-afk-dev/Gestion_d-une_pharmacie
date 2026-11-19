namespace GestionPharmacie;

public class Client : Personne
{
    private readonly List<Vente> _achats = new();

    public string Adresse { get; set; }
    public string NumeroCnam { get; set; }

    public Client(int id, string nom, string prenom, string telephone,
                  string adresse, string numeroCnam)
        : base(id, nom, prenom, telephone)
    {
        Adresse = adresse;
        NumeroCnam = numeroCnam;
    }

    public void AjouterAchat(Vente vente)
    {
        ArgumentNullException.ThrowIfNull(vente);
        _achats.Add(vente);
    }

    public List<Vente> HistoriqueAchats() => new(_achats);
}
