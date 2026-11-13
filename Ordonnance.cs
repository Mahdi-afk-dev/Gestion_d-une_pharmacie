using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacie
{
    /// <summary>
    /// Ordonnance prescrite par un médecin pour un client.
    /// Composition avec LigneOrdonnance (1..*).
    /// </summary>
    public class Ordonnance
    {
        // Durée de validité d'une ordonnance (en mois) - à ajuster selon la règle choisie par le groupe
        private const int DureeValiditeMois = 3;

        private readonly List<LigneOrdonnance> _lignes = new List<LigneOrdonnance>();

        public int Id { get; private set; }
        public DateTime DateEmission { get; private set; }
        public Medecin Medecin { get; private set; }
        public Client Client { get; private set; }

        public IReadOnlyList<LigneOrdonnance> Lignes
        {
            get { return _lignes.AsReadOnly(); }
        }

        public Ordonnance(int id, DateTime dateEmission, Medecin medecin, Client client)
        {
            if (medecin == null) throw new ArgumentNullException(nameof(medecin));
            if (client == null) throw new ArgumentNullException(nameof(client));

            Id = id;
            DateEmission = dateEmission;
            Medecin = medecin;
            Client = client;
        }

        public void AjouterLigne(LigneOrdonnance ligne)
        {
            if (ligne == null)
                throw new ArgumentNullException(nameof(ligne));
            _lignes.Add(ligne);
        }

        /// <summary>
        /// Valide si l'ordonnance contient au moins une ligne, n'est pas datée du futur
        /// et n'a pas dépassé sa durée de validité.
        /// </summary>
        public bool EstValide()
        {
            if (_lignes.Count == 0) return false;
            if (DateEmission > DateTime.Now) return false;
            return DateEmission.AddMonths(DureeValiditeMois) >= DateTime.Now;
        }

        /// <summary>Indique si l'ordonnance prescrit ce médicament.</summary>
        public bool Contient(Medicament medicament)
        {
            if (medicament == null) return false;
            return _lignes.Any(l => l.Medicament.Code == medicament.Code);
        }

        public override string ToString()
        {
            return $"Ordonnance #{Id} du {DateEmission:dd/MM/yyyy} - Dr {Medecin.NomComplet()} pour {Client.NomComplet()}";
        }
    }
}
