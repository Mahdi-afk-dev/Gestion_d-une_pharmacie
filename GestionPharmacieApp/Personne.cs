namespace GestionPharmacie;

public abstract class Personne
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    protected Personne() { }
    protected Personne(int id, string nom, string prenom, string telephone)
    {
        Id = id;
        Nom = nom;
        Prenom = prenom;
        Telephone = telephone;
    }

    public string NomComplet() => $"{Prenom} {Nom}";

    public override string ToString() => $"[{Id}] {NomComplet()}";
}
