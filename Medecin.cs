namespace GestionPharmacie;

public class Medecin : Personne
{
    public string Specialite { get; set; }
    public string NumeroOrdre { get; set; }

    public Medecin(int id, string nom, string prenom, string telephone,
                   string specialite, string numeroOrdre)
        : base(id, nom, prenom, telephone)
    {
        Specialite = specialite;
        NumeroOrdre = numeroOrdre;
    }

    public override string ToString() => $"Dr {NomComplet()} ({Specialite})";
}
