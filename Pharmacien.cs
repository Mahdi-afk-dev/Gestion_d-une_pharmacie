namespace GestionPharmacie;

// Utilisateur du système, responsable de chaque vente. Hérite de Personne.
public class Pharmacien : Personne
{
    public const string RolePharmacien = "pharmacien";
    public const string RoleAdministrateur = "administrateur";

    // Attributs privés (-) dans le diagramme de classes.
    private readonly string _login;
    private readonly string _motDePasse;

    public string NumeroOrdre { get; private set; }
    public string Role { get; private set; }

    public Pharmacien(int id, string nom, string prenom, string telephone,
                      string numeroOrdre, string role, string login, string motDePasse)
        : base(id, nom, prenom, telephone)
    {
        if (string.IsNullOrWhiteSpace(numeroOrdre))
            throw new ArgumentException("Le numéro d'ordre est obligatoire.");
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Le login est obligatoire.");
        if (string.IsNullOrEmpty(motDePasse))
            throw new ArgumentException("Le mot de passe est obligatoire.");

        string roleNormalise = (role ?? string.Empty).Trim().ToLowerInvariant();
        if (roleNormalise != RolePharmacien && roleNormalise != RoleAdministrateur)
            throw new ArgumentException(
                $"Rôle invalide : '{role}' (attendu : '{RolePharmacien}' ou '{RoleAdministrateur}').");

        NumeroOrdre = numeroOrdre;
        Role = roleNormalise;
        _login = login;
        _motDePasse = motDePasse;
    }

    // Login insensible à la casse, mot de passe sensible à la casse.
    public bool Authentifier(string login, string mdp) =>
        login is not null && mdp is not null
        && string.Equals(_login, login, StringComparison.OrdinalIgnoreCase)
        && _motDePasse == mdp;

    public bool EstAdministrateur() => Role == RoleAdministrateur;

    public override string ToString() => $"{NomComplet()} ({Role}) - N° ordre : {NumeroOrdre}";
}
