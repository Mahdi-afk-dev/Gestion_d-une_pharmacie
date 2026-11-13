using System;

namespace Pharmacie
{
    /// <summary>
    /// Une ligne d'ordonnance : un médicament prescrit, sa quantité et sa posologie.
    /// </summary>
    public class LigneOrdonnance
    {
        public int Quantite { get; private set; }
        public string Posologie { get; private set; }
        public Medicament Medicament { get; private set; }

        public LigneOrdonnance(Medicament medicament, int quantite, string posologie)
        {
            if (medicament == null)
                throw new ArgumentNullException(nameof(medicament));
            if (quantite <= 0)
                throw new ArgumentException("La quantité doit être strictement positive.", nameof(quantite));

            Medicament = medicament;
            Quantite = quantite;
            Posologie = posologie ?? string.Empty;
        }

        public override string ToString()
        {
            return $"{Medicament.Nom} x{Quantite} - {Posologie}";
        }
    }
}
