namespace GestionPharmacie;

// Inventaire global de la pharmacie : médicament -> ses lots.
public class Stock
{
    private readonly Dictionary<Medicament, List<Lot>> _lots = new();

    // Enregistre une livraison reçue.
    public void AjouterLot(Medicament medicament, Lot lot)
    {
        ArgumentNullException.ThrowIfNull(medicament);
        ArgumentNullException.ThrowIfNull(lot);

        if (!_lots.TryGetValue(medicament, out var lots))
        {
            lots = new List<Lot>();
            _lots[medicament] = lots;
        }

        if (lots.Any(l => l.NumeroLot == lot.NumeroLot))
            throw new InvalidOperationException(
                $"Le lot {lot.NumeroLot} existe déjà pour {medicament.Nom}.");

        lots.Add(lot);
    }

    // Somme des quantités des lots NON périmés.
    public int QuantiteDisponible(Medicament medicament)
    {
        ArgumentNullException.ThrowIfNull(medicament);

        if (!_lots.TryGetValue(medicament, out var lots))
            return 0;

        return lots.Where(l => !l.EstPerime()).Sum(l => l.Quantite);
    }

    // Retire une quantité selon la règle FEFO (First Expired, First Out) :
    // on utilise d'abord le lot qui expire le plus tôt.
    public void Retirer(Medicament medicament, int qte)
    {
        ArgumentNullException.ThrowIfNull(medicament);

        if (qte <= 0)
            throw new ArgumentException("La quantité doit être positive.");

        int disponible = QuantiteDisponible(medicament);
        if (disponible < qte)
            throw new InvalidOperationException(
                $"Stock insuffisant pour {medicament.Nom} : {disponible} disponible(s), {qte} demandé(s).");

        int restant = qte;
        var lotsTries = _lots[medicament]
            .Where(l => !l.EstPerime() && l.Quantite > 0)
            .OrderBy(l => l.DateExpiration);

        foreach (var lot in lotsTries)
        {
            int prelevement = Math.Min(lot.Quantite, restant);
            lot.Quantite -= prelevement;
            restant -= prelevement;

            if (restant == 0) break;
        }
    }

    // Médicaments dont la quantité disponible est au niveau du seuil ou en dessous.
    public List<Medicament> AlertesRupture() =>
        _lots.Keys
            .Where(m => QuantiteDisponible(m) <= m.SeuilAlerte)
            .ToList();

    // Lots non vides qui expirent dans les 'jours' prochains jours (ou déjà périmés).
    public List<Lot> AlertesPeremption(int jours)
    {
        DateTime limite = DateTime.Today.AddDays(jours);

        return _lots.Values
            .SelectMany(lots => lots)
            .Where(l => l.Quantite > 0 && l.DateExpiration.Date <= limite)
            .OrderBy(l => l.DateExpiration)
            .ToList();
    }
}
