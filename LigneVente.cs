using System;

namespace Pharmacie
{
    /// <summary>
    /// Une ligne d'une vente : un médicament, une quantité et le prix unitaire au moment de la vente.
    /// </summary>
    public class LigneVente
    {
        public int Quantite { get; private set; }
        public decimal PrixUnitaire { get; private set; }
        public Medicament Medicament { get; private set; }

        public LigneVente(Medicament medicament, int quantite)
        {
            if (medicament == null)
                throw new ArgumentNullException(nameof(medicament));
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.", nameof(quantite));

            Medicament = medicament;
            Quantite = quantite;
            // On fige le prix au moment de la vente (le prix du médicament peut changer plus tard)
            PrixUnitaire = medicament.PrixUnitaire;
        }

        /// <summary>Ajoute une quantité à une ligne existante (même médicament).</summary>
        public void AugmenterQuantite(int quantite)
        {
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.", nameof(quantite));
            Quantite += quantite;
        }

        public decimal SousTotal()
        {
            return Quantite * PrixUnitaire;
        }

        public override string ToString()
        {
            return $"{Medicament.Nom} x{Quantite} @ {PrixUnitaire:F2} = {SousTotal():F2}";
        }
    }
}
