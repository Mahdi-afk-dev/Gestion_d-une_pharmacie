using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacie
{
    /// <summary>
    /// Une vente réalisée par un pharmacien, pour un client (optionnel),
    /// éventuellement basée sur une ordonnance (optionnelle).
    /// Composition avec LigneVente (1..*).
    /// </summary>
    public class Vente
    {
        private readonly List<LigneVente> _lignes = new List<LigneVente>();

        public int Id { get; private set; }
        public DateTime DateVente { get; private set; }
        public string ModePaiement { get; private set; }
        public bool Validee { get; private set; }

        public Pharmacien Pharmacien { get; private set; }  // "réalisée par" (1)
        public Client Client { get; private set; }          // "pour" (0..1)
        public Ordonnance Ordonnance { get; private set; }  // "basée sur" (0..1)

        public IReadOnlyList<LigneVente> Lignes
        {
            get { return _lignes.AsReadOnly(); }
        }

        public Vente(int id, Pharmacien pharmacien, string modePaiement,
                     Client client = null, Ordonnance ordonnance = null)
        {
            if (pharmacien == null)
                throw new ArgumentNullException(nameof(pharmacien));

            Id = id;
            DateVente = DateTime.Now;
            Pharmacien = pharmacien;
            ModePaiement = modePaiement;
            Client = client;
            Ordonnance = ordonnance;
            Validee = false;
        }

        /// <summary>
        /// Ajoute un médicament à la vente. Si le médicament est déjà présent, on augmente la quantité.
        /// </summary>
        public void AjouterLigne(Medicament medicament, int quantite)
        {
            if (Validee)
                throw new InvalidOperationException("Impossible de modifier une vente déjà validée.");
            if (medicament == null)
                throw new ArgumentNullException(nameof(medicament));
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.", nameof(quantite));

            LigneVente existante = _lignes.FirstOrDefault(l => l.Medicament.Code == medicament.Code);
            if (existante != null)
                existante.AugmenterQuantite(quantite);
            else
                _lignes.Add(new LigneVente(medicament, quantite));
        }

        public decimal Total()
        {
            return _lignes.Sum(l => l.SousTotal());
        }

        /// <summary>
        /// Valide la vente : vérifie les ordonnances et le stock, retire les quantités du stock,
        /// puis enregistre l'achat dans l'historique du client.
        /// </summary>
        public void Valider(Stock stock)
        {
            if (stock == null)
                throw new ArgumentNullException(nameof(stock));
            if (Validee)
                throw new InvalidOperationException("Cette vente est déjà validée.");
            if (_lignes.Count == 0)
                throw new InvalidOperationException("La vente ne contient aucune ligne.");

            // 1) Médicaments sur ordonnance : une ordonnance valide qui les contient est obligatoire
            foreach (LigneVente ligne in _lignes)
            {
                if (ligne.Medicament.SurOrdonnance)
                {
                    if (Ordonnance == null || !Ordonnance.EstValide() || !Ordonnance.Contient(ligne.Medicament))
                        throw new InvalidOperationException(
                            $"Le médicament '{ligne.Medicament.Nom}' nécessite une ordonnance valide qui le contient.");
                }
            }

            // 2) Vérifier tout le stock AVANT de retirer quoi que ce soit
            foreach (LigneVente ligne in _lignes)
            {
                int dispo = stock.QuantiteDisponible(ligne.Medicament);
                if (dispo < ligne.Quantite)
                    throw new InvalidOperationException(
                        $"Stock insuffisant pour '{ligne.Medicament.Nom}' (demandé : {ligne.Quantite}, disponible : {dispo}).");
            }

            // 3) Retirer du stock
            foreach (LigneVente ligne in _lignes)
                stock.Retirer(ligne.Medicament, ligne.Quantite);

            Validee = true;

            // 4) Historique du client (si la vente est liée à un client)
            if (Client != null)
                Client.AjouterAchat(this);
        }

        public override string ToString()
        {
            return $"Vente #{Id} du {DateVente:dd/MM/yyyy HH:mm} - {_lignes.Count} ligne(s) - Total : {Total():F2} - {(Validee ? "Validée" : "En cours")}";
        }
    }
}
