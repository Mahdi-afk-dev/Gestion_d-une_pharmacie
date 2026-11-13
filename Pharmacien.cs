using System;

namespace Pharmacie
{
    /// <summary>
    /// Utilisateur du système, responsable de chaque vente.
    /// Hérite de Personne (Id, Nom, Prenom, Telephone, NomComplet()).
    /// </summary>
    public class Pharmacien : Personne
    {
        public const string RolePharmacien = "pharmacien";
        public const string RoleAdministrateur = "administrateur";

        // Attributs privés (-) dans le diagramme de classes
        private readonly string _login;
        private readonly string _motDePasse;

        public string NumeroOrdre { get; private set; }
        public string Role { get; private set; }

        public Pharmacien(int id, string nom, string prenom, string telephone,
                          string numeroOrdre, string role, string login, string motDePasse)
            : base(id, nom, prenom, telephone)
        {
            if (string.IsNullOrWhiteSpace(numeroOrdre))
                throw new ArgumentException("Le numéro d'ordre est obligatoire.", nameof(numeroOrdre));
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Le login est obligatoire.", nameof(login));
            if (string.IsNullOrEmpty(motDePasse))
                throw new ArgumentException("Le mot de passe est obligatoire.", nameof(motDePasse));

            string roleNormalise = (role ?? string.Empty).Trim().ToLower();
            if (roleNormalise != RolePharmacien && roleNormalise != RoleAdministrateur)
                throw new ArgumentException(
                    $"Rôle invalide : '{role}' (attendu : '{RolePharmacien}' ou '{RoleAdministrateur}').", nameof(role));

            NumeroOrdre = numeroOrdre;
            Role = roleNormalise;
            _login = login;
            _motDePasse = motDePasse;
        }

        /// <summary>
        /// Vérifie les identifiants de connexion.
        /// Le login est insensible à la casse, le mot de passe est sensible à la casse.
        /// </summary>
        public bool Authentifier(string login, string mdp)
        {
            if (login == null || mdp == null)
                return false;

            return string.Equals(_login, login, StringComparison.OrdinalIgnoreCase)
                   && _motDePasse == mdp;
        }

        public bool EstAdministrateur()
        {
            return Role == RoleAdministrateur;
        }

        public override string ToString()
        {
            return $"{NomComplet()} ({Role}) - N° ordre : {NumeroOrdre}";
        }
    }
}
