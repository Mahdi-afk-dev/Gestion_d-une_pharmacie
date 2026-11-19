namespace GestionPharmacie;

// Une vente réalisée par un pharmacien, pour un client (optionnel),
// éventuellement basée sur une ordonnance (optionnelle).
// Composition avec LigneVente (1..*).
public class Vente
{
    private readonly List<LigneVente> _lignes = new();

    public int Id { get; private set; }
    public DateTime DateVente { get; private set; }
    public string ModePaiement { get; private set; }
    public bool Validee { get; private set; }

    public Pharmacien Pharmacien { get; private set; }   // "réalisée par" (1)
    public Client? Client { get; private set; }          // "pour" (0..1)
    public Ordonnance? Ordonnance { get; private set; }  // "basée sur" (0..1)

    public IReadOnlyList<LigneVente> Lignes => _lignes;
    public Vente() { }
    public Vente(int id, Pharmacien pharmacien, string modePaiement,
                 Client? client = null, Ordonnance? ordonnance = null)
    {
        ArgumentNullException.ThrowIfNull(pharmacien);

        if (ordonnance is not null && client is not null && ordonnance.Client != client)
            throw new ArgumentException("L'ordonnance n'appartient pas à ce client.");

        Id = id;
        DateVente = DateTime.Now;
        Pharmacien = pharmacien;
        ModePaiement = modePaiement;
        Client = client ?? ordonnance?.Client;
        Ordonnance = ordonnance;
        Validee = false;
    }

    // Ajoute un médicament ; s'il est déjà dans la vente, on augmente la quantité.
    public void AjouterLigne(Medicament medicament, int quantite)
    {
        if (Validee)
            throw new InvalidOperationException("Impossible de modifier une vente déjà validée.");
        ArgumentNullException.ThrowIfNull(medicament);
        if (quantite <= 0)
            throw new ArgumentException("La quantité doit être strictement positive.");

        var existante = _lignes.FirstOrDefault(l => l.Medicament.Equals(medicament));
        if (existante is not null)
            existante.AugmenterQuantite(quantite);
        else
            _lignes.Add(new LigneVente(medicament, quantite));
    }

    public decimal Total() => _lignes.Sum(l => l.SousTotal());

    // Valide la vente : contrôle l'ordonnance et le stock, retire du stock,
    // puis enregistre l'achat dans l'historique du client.
    public void Valider(Stock stock)
    {
        ArgumentNullException.ThrowIfNull(stock);
        if (Validee)
            throw new InvalidOperationException("Cette vente est déjà validée.");
        if (_lignes.Count == 0)
            throw new InvalidOperationException("La vente ne contient aucune ligne.");

        // 1) Médicaments sur ordonnance : une ordonnance valide qui les contient est obligatoire.
        foreach (var ligne in _lignes.Where(l => l.Medicament.SurOrdonnance))
        {
            if (Ordonnance is null || !Ordonnance.EstValide() || !Ordonnance.Contient(ligne.Medicament))
                throw new InvalidOperationException(
                    $"Le médicament '{ligne.Medicament.Nom}' nécessite une ordonnance valide qui le contient.");
        }

        // 2) Vérifier tout le stock AVANT de retirer quoi que ce soit.
        foreach (var ligne in _lignes)
        {
            int dispo = stock.QuantiteDisponible(ligne.Medicament);
            if (dispo < ligne.Quantite)
                throw new InvalidOperationException(
                    $"Stock insuffisant pour '{ligne.Medicament.Nom}' (demandé : {ligne.Quantite}, disponible : {dispo}).");
        }

        // 3) Retirer du stock (règle FEFO gérée par Stock).
        foreach (var ligne in _lignes)
            stock.Retirer(ligne.Medicament, ligne.Quantite);

        Validee = true;

        // 4) Historique du client, s'il y en a un.
        Client?.AjouterAchat(this);
    }

    public override string ToString() =>
        $"Vente #{Id} du {DateVente:dd/MM/yyyy HH:mm} - {_lignes.Count} ligne(s) - Total : {Total():0.000} DT - {(Validee ? "Validée" : "En cours")}";
}
