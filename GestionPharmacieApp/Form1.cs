using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using GestionPharmacie;

namespace GestionPharmacieApp;

public partial class Form1 : Form
{
    private record ChoixId(int Id, string Libelle);
    private record ChoixCode(string Code, string Libelle);

    // 1. Clients
    private DataGridView dgvClients = null!;
    private TextBox txtNomClient = null!;
    private TextBox txtPrenomClient = null!;
    private TextBox txtTelClient = null!;
    private TextBox txtAdresseClient = null!;
    private TextBox txtCnamClient = null!;
    private int? _selectedClientId = null;

    // 2. Médicaments
    private DataGridView dgvMedicaments = null!;
    private TextBox txtCodeMed = null!;
    private TextBox txtNomMed = null!;
    private TextBox txtCategorieMed = null!;
    private TextBox txtPrixMed = null!;
    private CheckBox chkOrdonnanceMed = null!;

    // 3. Ventes / Délivrance
    private DataGridView dgvVentes = null!;
    private ComboBox cmbVenteClient = null!;
    private ComboBox cmbVenteMed = null!;
    private TextBox txtVenteQte = null!;
    private ComboBox cmbVentePharmacien = null!;
    private ComboBox cmbVentePaiement = null!;
    private CheckBox chkVenteOrdonnance = null!;
    private ComboBox cmbVenteMedecin = null!;
    private TextBox txtVenteNumOrd = null!;

    // 4. Fournisseurs & Réceptions
    private DataGridView dgvFournisseurs = null!;
    private TextBox txtNomFournisseur = null!;
    private TextBox txtTelFournisseur = null!;
    private TextBox txtAdresseFournisseur = null!;
    private int? _selectedFournisseurId = null;

    private DataGridView dgvLivraisons = null!;
    private ComboBox cmbLivFournisseur = null!;
    private ComboBox cmbLivMedicament = null!;
    private TextBox txtLivQuantite = null!;
    private DateTimePicker dtpLivDate = null!;

    // 5. Pharmaciens
    private DataGridView dgvPharmaciens = null!;
    private TextBox txtNomPharm = null!;
    private TextBox txtPrenomPharm = null!;
    private TextBox txtTelPharm = null!;
    private TextBox txtOrdrePharm = null!;
    private ComboBox cmbRolePharm = null!;
    private TextBox txtHorairesPharm = null!;
    private TextBox txtLoginPharm = null!;
    private TextBox txtMdpPharm = null!;
    private int? _selectedPharmacienId = null;

    // 6. Médecins
    private DataGridView dgvMedecins = null!;
    private TextBox txtNomMedecin = null!;
    private TextBox txtPrenomMedecin = null!;
    private TextBox txtTelMedecin = null!;
    private TextBox txtSpecMedecin = null!;
    private TextBox txtOrdreMedecin = null!;
    private int? _selectedMedecinId = null;

    public Form1()
    {
        ConstruireInterface();
        ToutRecharger();
    }

    private void ToutRecharger()
    {
        ChargerClients();
        ChargerMedicaments();
        ChargerPharmaciens();
        ChargerMedecins();
        ChargerFournisseurs();
        ChargerListesDeroulantes();
        ChargerVentes();
        ChargerLivraisons();
    }

    private void ConstruireInterface()
    {
        Text = "Gestion de Pharmacie - Base de données SQLite";
        Size = new Size(1460, 880);
        MinimumSize = new Size(1280, 760);
        StartPosition = FormStartPosition.CenterScreen;

        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(14, 8)
        };
        tabControl.SelectedIndexChanged += (_, _) => ChargerListesDeroulantes();

        // ==================== ONGLET 1 : CLIENTS ====================
        var tabClients = new TabPage("Clients") { Padding = new Padding(16) };
        var grilleClients = CreerGrilleFormulaire(3);

        txtNomClient = AjouterChampGrille(grilleClients, "Nom :", 0, 0);
        txtPrenomClient = AjouterChampGrille(grilleClients, "Prénom :", 2, 0);
        txtTelClient = AjouterChampGrille(grilleClients, "Téléphone :", 4, 0);
        txtAdresseClient = AjouterChampGrille(grilleClients, "Adresse :", 0, 1);
        txtCnamClient = AjouterChampGrille(grilleClients, "N° CNAM :", 2, 1);

        var barreBoutonsClient = CreerBarreBoutons();
        var btnAjouterClient = CreerBouton("Ajouter", Color.FromArgb(46, 125, 50));
        btnAjouterClient.Click += BtnAjouterClient_Click;
        var btnModifierClient = CreerBouton("Modifier", Color.FromArgb(21, 101, 192));
        btnModifierClient.Click += BtnModifierClient_Click;
        var btnSupprimerClient = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerClient.Click += BtnSupprimerClient_Click;
        var btnViderClient = CreerBouton("Vider", Color.Gray);
        btnViderClient.Click += (_, _) => ViderChampsClient();

        barreBoutonsClient.Controls.AddRange(new Control[] { btnAjouterClient, btnModifierClient, btnSupprimerClient, btnViderClient });
        grilleClients.Controls.Add(barreBoutonsClient, 0, 2);
        grilleClients.SetColumnSpan(barreBoutonsClient, 6);

        dgvClients = CreerTableau();
        dgvClients.CellClick += DgvClients_CellClick;
        tabClients.Controls.Add(dgvClients);
        tabClients.Controls.Add(grilleClients);

        // ==================== ONGLET 2 : MÉDICAMENTS ====================
        var tabMedicaments = new TabPage("Médicaments") { Padding = new Padding(16) };
        var grilleMed = CreerGrilleFormulaire(3);

        txtCodeMed = AjouterChampGrille(grilleMed, "Code :", 0, 0);
        txtNomMed = AjouterChampGrille(grilleMed, "Nom :", 2, 0);
        txtCategorieMed = AjouterChampGrille(grilleMed, "Catégorie :", 4, 0);
        txtPrixMed = AjouterChampGrille(grilleMed, "Prix unitaire :", 0, 1);

        chkOrdonnanceMed = new CheckBox
        {
            Text = "Sur ordonnance",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(8, 12, 12, 12)
        };
        grilleMed.Controls.Add(chkOrdonnanceMed, 2, 1);
        grilleMed.SetColumnSpan(chkOrdonnanceMed, 2);

        var barreBoutonsMed = CreerBarreBoutons();
        var btnAjouterMed = CreerBouton("Ajouter", Color.FromArgb(46, 125, 50));
        btnAjouterMed.Click += BtnAjouterMed_Click;
        var btnModifierMed = CreerBouton("Modifier", Color.FromArgb(21, 101, 192));
        btnModifierMed.Click += BtnModifierMed_Click;
        var btnSupprimerMed = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerMed.Click += BtnSupprimerMed_Click;
        var btnViderMed = CreerBouton("Vider", Color.Gray);
        btnViderMed.Click += (_, _) => ViderChampsMed();

        barreBoutonsMed.Controls.AddRange(new Control[] { btnAjouterMed, btnModifierMed, btnSupprimerMed, btnViderMed });
        grilleMed.Controls.Add(barreBoutonsMed, 0, 2);
        grilleMed.SetColumnSpan(barreBoutonsMed, 6);

        dgvMedicaments = CreerTableau();
        dgvMedicaments.CellClick += DgvMedicaments_CellClick;
        tabMedicaments.Controls.Add(dgvMedicaments);
        tabMedicaments.Controls.Add(grilleMed);

        // ==================== ONGLET 3 : VENTES & ORDONNANCES ====================
        var tabVentes = new TabPage("Ventes / Délivrance") { Padding = new Padding(16) };
        var grilleVentes = CreerGrilleFormulaire(4);

        cmbVenteClient = AjouterComboGrille(grilleVentes, "Client :", 0, 0);
        cmbVenteMed = AjouterComboGrille(grilleVentes, "Médicament :", 2, 0);
        cmbVenteMed.SelectedIndexChanged += CmbVenteMed_SelectedIndexChanged;
        txtVenteQte = AjouterChampGrille(grilleVentes, "Quantité :", 4, 0);
        txtVenteQte.Text = "1";

        cmbVentePharmacien = AjouterComboGrille(grilleVentes, "Pharmacien :", 0, 1);
        cmbVentePaiement = AjouterComboGrille(grilleVentes, "Paiement :", 2, 1);
        cmbVentePaiement.Items.AddRange(new object[] { "Espèces", "Carte bancaire", "Chèque", "CNAM" });
        cmbVentePaiement.SelectedIndex = 0;

        chkVenteOrdonnance = new CheckBox
        {
            Text = "Avec ordonnance",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(8, 12, 12, 12)
        };
        chkVenteOrdonnance.CheckedChanged += (_, _) => BasculerChampsOrdonnance();
        grilleVentes.Controls.Add(chkVenteOrdonnance, 4, 1);
        grilleVentes.SetColumnSpan(chkVenteOrdonnance, 2);

        cmbVenteMedecin = AjouterComboGrille(grilleVentes, "Médecin :", 0, 2);
        txtVenteNumOrd = AjouterChampGrille(grilleVentes, "N° Ordonnance :", 2, 2);
        BasculerChampsOrdonnance();

        var barreBoutonsVente = CreerBarreBoutons();
        var btnAjouterVente = CreerBouton("Enregistrer la vente", Color.FromArgb(46, 125, 50));
        btnAjouterVente.Click += BtnAjouterVente_Click;
        var btnSupprimerVente = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerVente.Click += BtnSupprimerVente_Click;

        barreBoutonsVente.Controls.AddRange(new Control[] { btnAjouterVente, btnSupprimerVente });
        grilleVentes.Controls.Add(barreBoutonsVente, 0, 3);
        grilleVentes.SetColumnSpan(barreBoutonsVente, 6);

        dgvVentes = CreerTableau();
        tabVentes.Controls.Add(dgvVentes);
        tabVentes.Controls.Add(grilleVentes);

        // ==================== ONGLET 4 : FOURNISSEURS & RÉCEPTIONS ====================
        var tabFournisseurs = new TabPage("Fournisseurs & Réceptions") { Padding = new Padding(12) };
        var layoutFournisseurs = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layoutFournisseurs.RowStyles.Add(new RowStyle(SizeType.Percent, 47f));
        layoutFournisseurs.RowStyles.Add(new RowStyle(SizeType.Percent, 53f));

        // Haut : Fournisseurs
        var grpFournisseurs = new GroupBox { Text = "1. Gestion des Fournisseurs", Dock = DockStyle.Fill, Padding = new Padding(10) };
        var grilleFourn = CreerGrilleFormulaire(2);
        txtNomFournisseur = AjouterChampGrille(grilleFourn, "Nom :", 0, 0);
        txtTelFournisseur = AjouterChampGrille(grilleFourn, "Téléphone :", 2, 0);
        txtAdresseFournisseur = AjouterChampGrille(grilleFourn, "Adresse :", 4, 0);

        var barreBoutonsFourn = CreerBarreBoutons();
        var btnAjouterFourn = CreerBouton("Ajouter", Color.FromArgb(46, 125, 50));
        btnAjouterFourn.Click += BtnAjouterFournisseur_Click;
        var btnModifierFourn = CreerBouton("Modifier", Color.FromArgb(21, 101, 192));
        btnModifierFourn.Click += BtnModifierFournisseur_Click;
        var btnSupprimerFourn = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerFourn.Click += BtnSupprimerFournisseur_Click;
        var btnViderFourn = CreerBouton("Vider", Color.Gray);
        btnViderFourn.Click += (_, _) => ViderChampsFournisseur();

        barreBoutonsFourn.Controls.AddRange(new Control[] { btnAjouterFourn, btnModifierFourn, btnSupprimerFourn, btnViderFourn });
        grilleFourn.Controls.Add(barreBoutonsFourn, 0, 1);
        grilleFourn.SetColumnSpan(barreBoutonsFourn, 6);

        dgvFournisseurs = CreerTableau();
        dgvFournisseurs.CellClick += DgvFournisseurs_CellClick;
        grpFournisseurs.Controls.Add(dgvFournisseurs);
        grpFournisseurs.Controls.Add(grilleFourn);
        layoutFournisseurs.Controls.Add(grpFournisseurs, 0, 0);

        // Bas : Réceptions de médicaments
        var grpLivraisons = new GroupBox { Text = "2. Enregistrer une réception (Médicament, Quantité reçue, Date)", Dock = DockStyle.Fill, Padding = new Padding(10) };
        var grilleLiv = CreerGrilleFormulaire(2);
        cmbLivFournisseur = AjouterComboGrille(grilleLiv, "Fournisseur :", 0, 0);
        cmbLivMedicament = AjouterComboGrille(grilleLiv, "Médicament :", 2, 0);
        txtLivQuantite = AjouterChampGrille(grilleLiv, "Quantité reçue :", 4, 0);

        var lblDateLiv = new Label { Text = "Date réception :", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(8, 14, 12, 14) };
        dtpLivDate = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Dock = DockStyle.Fill,
            MinimumSize = new Size(170, 0),
            Margin = new Padding(4, 10, 24, 10)
        };
        grilleLiv.Controls.Add(lblDateLiv, 0, 1);
        grilleLiv.Controls.Add(dtpLivDate, 1, 1);

        var barreBoutonsLiv = CreerBarreBoutons();
        barreBoutonsLiv.Margin = new Padding(8, 4, 8, 4);
        var btnAjouterLiv = CreerBouton("Enregistrer réception", Color.FromArgb(46, 125, 50));
        btnAjouterLiv.Click += BtnAjouterLivraison_Click;
        var btnSupprimerLiv = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerLiv.Click += BtnSupprimerLivraison_Click;

        barreBoutonsLiv.Controls.AddRange(new Control[] { btnAjouterLiv, btnSupprimerLiv });
        grilleLiv.Controls.Add(barreBoutonsLiv, 2, 1);
        grilleLiv.SetColumnSpan(barreBoutonsLiv, 4);

        dgvLivraisons = CreerTableau();
        grpLivraisons.Controls.Add(dgvLivraisons);
        grpLivraisons.Controls.Add(grilleLiv);
        layoutFournisseurs.Controls.Add(grpLivraisons, 0, 1);

        tabFournisseurs.Controls.Add(layoutFournisseurs);
        // ==================== ONGLET 5 : PHARMACIENS ====================
        var tabPharmaciens = new TabPage("Pharmaciens") { Padding = new Padding(16) };
        var grillePharm = CreerGrilleFormulaire(4);

        txtNomPharm = AjouterChampGrille(grillePharm, "Nom :", 0, 0);
        txtPrenomPharm = AjouterChampGrille(grillePharm, "Prénom :", 2, 0);
        txtTelPharm = AjouterChampGrille(grillePharm, "Téléphone :", 4, 0);

        txtOrdrePharm = AjouterChampGrille(grillePharm, "N° Ordre :", 0, 1);
        cmbRolePharm = AjouterComboGrille(grillePharm, "Rôle :", 2, 1);
        cmbRolePharm.Items.AddRange(new object[] { Pharmacien.RolePharmacien, Pharmacien.RoleAdministrateur });
        cmbRolePharm.SelectedIndex = 0;
        txtHorairesPharm = AjouterChampGrille(grillePharm, "Horaires :", 4, 1);

        txtLoginPharm = AjouterChampGrille(grillePharm, "Login :", 0, 2);
        txtMdpPharm = AjouterChampGrille(grillePharm, "Mot de passe :", 2, 2);
        txtMdpPharm.UseSystemPasswordChar = true;

        var barreBoutonsPharm = CreerBarreBoutons();
        var btnAjouterPharm = CreerBouton("Ajouter", Color.FromArgb(46, 125, 50));
        btnAjouterPharm.Click += BtnAjouterPharmacien_Click;
        var btnModifierPharm = CreerBouton("Modifier", Color.FromArgb(21, 101, 192));
        btnModifierPharm.Click += BtnModifierPharmacien_Click;
        var btnSupprimerPharm = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerPharm.Click += BtnSupprimerPharmacien_Click;
        var btnViderPharm = CreerBouton("Vider", Color.Gray);
        btnViderPharm.Click += (_, _) => ViderChampsPharmacien();

        barreBoutonsPharm.Controls.AddRange(new Control[] { btnAjouterPharm, btnModifierPharm, btnSupprimerPharm, btnViderPharm });
        grillePharm.Controls.Add(barreBoutonsPharm, 0, 3);
        grillePharm.SetColumnSpan(barreBoutonsPharm, 6);

        dgvPharmaciens = CreerTableau();
        dgvPharmaciens.CellClick += DgvPharmaciens_CellClick;
        tabPharmaciens.Controls.Add(dgvPharmaciens);
        tabPharmaciens.Controls.Add(grillePharm);

        // ==================== ONGLET 6 : MÉDECINS ====================
        var tabMedecins = new TabPage("Médecins") { Padding = new Padding(16) };
        var grilleMedecins = CreerGrilleFormulaire(3);

        txtNomMedecin = AjouterChampGrille(grilleMedecins, "Nom :", 0, 0);
        txtPrenomMedecin = AjouterChampGrille(grilleMedecins, "Prénom :", 2, 0);
        txtTelMedecin = AjouterChampGrille(grilleMedecins, "Téléphone :", 4, 0);
        txtSpecMedecin = AjouterChampGrille(grilleMedecins, "Spécialité :", 0, 1);
        txtOrdreMedecin = AjouterChampGrille(grilleMedecins, "N° Ordre :", 2, 1);

        var barreBoutonsMedecin = CreerBarreBoutons();
        var btnAjouterMedecin = CreerBouton("Ajouter", Color.FromArgb(46, 125, 50));
        btnAjouterMedecin.Click += BtnAjouterMedecin_Click;
        var btnModifierMedecin = CreerBouton("Modifier", Color.FromArgb(21, 101, 192));
        btnModifierMedecin.Click += BtnModifierMedecin_Click;
        var btnSupprimerMedecin = CreerBouton("Supprimer", Color.FromArgb(198, 40, 40));
        btnSupprimerMedecin.Click += BtnSupprimerMedecin_Click;
        var btnViderMedecin = CreerBouton("Vider", Color.Gray);
        btnViderMedecin.Click += (_, _) => ViderChampsMedecin();

        barreBoutonsMedecin.Controls.AddRange(new Control[] { btnAjouterMedecin, btnModifierMedecin, btnSupprimerMedecin, btnViderMedecin });
        grilleMedecins.Controls.Add(barreBoutonsMedecin, 0, 2);
        grilleMedecins.SetColumnSpan(barreBoutonsMedecin, 6);

        dgvMedecins = CreerTableau();
        dgvMedecins.CellClick += DgvMedecins_CellClick;
        tabMedecins.Controls.Add(dgvMedecins);
        tabMedecins.Controls.Add(grilleMedecins);

        tabControl.TabPages.AddRange(new[]
        {
            tabClients, tabMedicaments, tabVentes, tabFournisseurs, tabPharmaciens, tabMedecins
        });
        Controls.Add(tabControl);
    }

    // ==================== CHARGEMENT DES LISTES DÉROULANTES ====================

    private void ChargerListesDeroulantes()
    {
        using var db = new PharmacieContext();

        var clients = db.Clients.Select(c => new ChoixId(c.Id, $"{c.Prenom} {c.Nom}")).ToList();
        RemplirCombo(cmbVenteClient, clients);

        var meds = db.Medicaments.Select(m => new ChoixCode(m.Code, $"{m.Code} - {m.Nom} ({m.PrixUnitaire:0.000} DT)")).ToList();
        RemplirCombo(cmbVenteMed, meds);
        RemplirCombo(cmbLivMedicament, meds);

        var pharmaciens = db.Pharmaciens.Select(p => new ChoixId(p.Id, $"{p.Prenom} {p.Nom} ({p.Role})")).ToList();
        RemplirCombo(cmbVentePharmacien, pharmaciens);

        var medecins = db.Medecins.Select(m => new ChoixId(m.Id, $"Dr {m.Prenom} {m.Nom} ({m.Specialite})")).ToList();
        RemplirCombo(cmbVenteMedecin, medecins);

        var fournisseurs = db.Fournisseurs.Select(f => new ChoixId(f.Id, f.Nom)).ToList();
        RemplirCombo(cmbLivFournisseur, fournisseurs);
    }

    private static void RemplirCombo<T>(ComboBox cmb, System.Collections.Generic.List<T> items)
    {
        if (cmb == null) return;
        cmb.DataSource = null;
        cmb.DataSource = items;
        cmb.DisplayMember = "Libelle";
        cmb.ValueMember = typeof(T) == typeof(ChoixCode) ? "Code" : "Id";
    }

    // ==================== 1. CRUD CLIENTS ====================

    private void ChargerClients()
    {
        using var db = new PharmacieContext();
        dgvClients.DataSource = db.Clients
            .Select(c => new { c.Id, c.Nom, c.Prenom, c.Telephone, c.Adresse, c.NumeroCnam })
            .ToList();
    }

    private void BtnAjouterClient_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomClient.Text) || string.IsNullOrWhiteSpace(txtPrenomClient.Text))
        {
            MessageBox.Show("Veuillez saisir au moins le nom et le prénom du client.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        db.Clients.Add(new Client(0, txtNomClient.Text.Trim(), txtPrenomClient.Text.Trim(), txtTelClient.Text.Trim(), txtAdresseClient.Text.Trim(), txtCnamClient.Text.Trim()));
        db.SaveChanges();
        ViderChampsClient();
        ChargerClients();
    }

    private void BtnModifierClient_Click(object? sender, EventArgs e)
    {
        if (_selectedClientId == null) return;
        using var db = new PharmacieContext();
        var client = db.Clients.Find(_selectedClientId.Value);
        if (client == null) return;

        client.Nom = txtNomClient.Text.Trim();
        client.Prenom = txtPrenomClient.Text.Trim();
        client.Telephone = txtTelClient.Text.Trim();
        client.Adresse = txtAdresseClient.Text.Trim();
        client.NumeroCnam = txtCnamClient.Text.Trim();
        db.SaveChanges();

        ViderChampsClient();
        ChargerClients();
    }

    private void BtnSupprimerClient_Click(object? sender, EventArgs e)
    {
        if (_selectedClientId == null) return;
        if (MessageBox.Show("Supprimer ce client ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var client = db.Clients.Find(_selectedClientId.Value);
        if (client != null) { db.Clients.Remove(client); db.SaveChanges(); }
        ViderChampsClient();
        ChargerClients();
    }

    private void DgvClients_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvClients.Rows[e.RowIndex];
        _selectedClientId = Convert.ToInt32(row.Cells["Id"].Value);
        txtNomClient.Text = row.Cells["Nom"].Value?.ToString() ?? "";
        txtPrenomClient.Text = row.Cells["Prenom"].Value?.ToString() ?? "";
        txtTelClient.Text = row.Cells["Telephone"].Value?.ToString() ?? "";
        txtAdresseClient.Text = row.Cells["Adresse"].Value?.ToString() ?? "";
        txtCnamClient.Text = row.Cells["NumeroCnam"].Value?.ToString() ?? "";
    }

    private void ViderChampsClient()
    {
        _selectedClientId = null;
        txtNomClient.Clear(); txtPrenomClient.Clear(); txtTelClient.Clear(); txtAdresseClient.Clear(); txtCnamClient.Clear();
    }

    // ==================== 2. CRUD MÉDICAMENTS ====================

    private void ChargerMedicaments()
    {
        using var db = new PharmacieContext();
        dgvMedicaments.DataSource = db.Medicaments
            .Select(m => new { m.Code, m.Nom, m.Categorie, m.PrixUnitaire, m.SurOrdonnance })
            .ToList();
    }

    private void BtnAjouterMed_Click(object? sender, EventArgs e)
    {
        string code = txtCodeMed.Text.Trim();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(txtNomMed.Text))
        {
            MessageBox.Show("Veuillez saisir le code et le nom du médicament.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!decimal.TryParse(txtPrixMed.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prix) || prix < 0)
        {
            MessageBox.Show("Veuillez saisir un prix unitaire valide.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        if (db.Medicaments.Find(code) != null)
        {
            MessageBox.Show("Un médicament avec ce code existe déjà.", "Doublon", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        db.Medicaments.Add(new Medicament
        {
            Code = code,
            Nom = txtNomMed.Text.Trim(),
            Categorie = txtCategorieMed.Text.Trim(),
            PrixUnitaire = prix,
            SurOrdonnance = chkOrdonnanceMed.Checked
        });
        db.SaveChanges();
        ViderChampsMed();
        ChargerMedicaments();
    }

    private void BtnModifierMed_Click(object? sender, EventArgs e)
    {
        string code = txtCodeMed.Text.Trim();
        if (string.IsNullOrWhiteSpace(code)) return;
        if (!decimal.TryParse(txtPrixMed.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal prix) || prix < 0) return;

        using var db = new PharmacieContext();
        var med = db.Medicaments.Find(code);
        if (med == null) return;

        med.Nom = txtNomMed.Text.Trim();
        med.Categorie = txtCategorieMed.Text.Trim();
        med.PrixUnitaire = prix;
        med.SurOrdonnance = chkOrdonnanceMed.Checked;
        db.SaveChanges();

        ViderChampsMed();
        ChargerMedicaments();
    }

    private void BtnSupprimerMed_Click(object? sender, EventArgs e)
    {
        string code = txtCodeMed.Text.Trim();
        if (string.IsNullOrWhiteSpace(code)) return;
        if (MessageBox.Show("Supprimer ce médicament ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var med = db.Medicaments.Find(code);
        if (med != null) { db.Medicaments.Remove(med); db.SaveChanges(); }
        ViderChampsMed();
        ChargerMedicaments();
    }

    private void DgvMedicaments_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvMedicaments.Rows[e.RowIndex];
        txtCodeMed.Text = row.Cells["Code"].Value?.ToString() ?? "";
        txtCodeMed.ReadOnly = true;
        txtNomMed.Text = row.Cells["Nom"].Value?.ToString() ?? "";
        txtCategorieMed.Text = row.Cells["Categorie"].Value?.ToString() ?? "";
        txtPrixMed.Text = row.Cells["PrixUnitaire"].Value?.ToString() ?? "";
        chkOrdonnanceMed.Checked = Convert.ToBoolean(row.Cells["SurOrdonnance"].Value);
    }

    private void ViderChampsMed()
    {
        txtCodeMed.ReadOnly = false;
        txtCodeMed.Clear(); txtNomMed.Clear(); txtCategorieMed.Clear(); txtPrixMed.Clear();
        chkOrdonnanceMed.Checked = false;
    }

    // ==================== 3. VENTES & ORDONNANCES ====================

    private void BasculerChampsOrdonnance()
    {
        bool actif = chkVenteOrdonnance.Checked;
        cmbVenteMedecin.Enabled = actif;
        txtVenteNumOrd.Enabled = actif;
        if (!actif) txtVenteNumOrd.Clear();
    }

    private void CmbVenteMed_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbVenteMed.SelectedValue is not string code) return;
        using var db = new PharmacieContext();
        var med = db.Medicaments.Find(code);
        if (med != null)
            chkVenteOrdonnance.Checked = med.SurOrdonnance;
    }

    private void ChargerVentes()
    {
        using var db = new PharmacieContext();
        var ventes = db.Ventes
            .Include(v => v.Client)
            .Include(v => v.Pharmacien)
            .Include(v => v.Ordonnance).ThenInclude(o => o!.Medecin)
            .Include(v => v.Lignes).ThenInclude(l => l.Medicament)
            .OrderByDescending(v => v.Id)
            .ToList();

        dgvVentes.DataSource = ventes.Select(v => new
        {
            v.Id,
            Date = v.DateVente.ToString("dd/MM/yyyy HH:mm"),
            Client = v.Client != null ? v.Client.NomComplet() : "Anonyme",
            Medicaments = string.Join(", ", v.Lignes.Select(l => $"{l.Medicament.Nom} x{l.Quantite}")),
            Total = v.Total().ToString("0.000") + " DT",
            AvecOrdonnance = v.Ordonnance != null ? "Oui" : "Non",
            NumeroOrdonnance = v.Ordonnance?.NumeroOrdonnance ?? "-",
            Medecin = v.Ordonnance?.Medecin != null ? $"Dr {v.Ordonnance.Medecin.NomComplet()}" : "-",
            Pharmacien = v.Pharmacien != null ? v.Pharmacien.NomComplet() : "-",
            v.ModePaiement
        }).ToList();
    }

    private void BtnAjouterVente_Click(object? sender, EventArgs e)
    {
        if (cmbVenteClient.SelectedValue is not int clientId ||
            cmbVenteMed.SelectedValue is not string medCode ||
            cmbVentePharmacien.SelectedValue is not int pharmacienId)
        {
            MessageBox.Show("Assurez-vous d'avoir créé au moins un client, un médicament et un pharmacien.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtVenteQte.Text.Trim(), out int qte) || qte <= 0)
        {
            MessageBox.Show("Veuillez saisir une quantité valide (> 0).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        var client = db.Clients.Find(clientId)!;
        var med = db.Medicaments.Find(medCode)!;
        var pharmacien = db.Pharmaciens.Find(pharmacienId)!;

        if (med.SurOrdonnance && !chkVenteOrdonnance.Checked)
        {
            MessageBox.Show($"Le médicament '{med.Nom}' nécessite obligatoirement une ordonnance !", "Ordonnance requise", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Ordonnance? ordonnance = null;
        if (chkVenteOrdonnance.Checked)
        {
            if (cmbVenteMedecin.SelectedValue is not int medecinId || string.IsNullOrWhiteSpace(txtVenteNumOrd.Text))
            {
                MessageBox.Show("Veuillez sélectionner le médecin et saisir le numéro d'ordonnance.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var medecin = db.Medecins.Find(medecinId)!;
            ordonnance = new Ordonnance(0, DateTime.Now, medecin, client)
            {
                NumeroOrdonnance = txtVenteNumOrd.Text.Trim()
            };
            ordonnance.AjouterLigne(new LigneOrdonnance(med, qte, "Selon prescription"));
            db.Ordonnances.Add(ordonnance);
        }

        string paiement = cmbVentePaiement.SelectedItem?.ToString() ?? "Espèces";
        var vente = new Vente(0, pharmacien, paiement, client, ordonnance);
        vente.AjouterLigne(med, qte);

        db.Ventes.Add(vente);
        db.SaveChanges();

        txtVenteQte.Text = "1";
        txtVenteNumOrd.Clear();
        ChargerVentes();
    }

    private void BtnSupprimerVente_Click(object? sender, EventArgs e)
    {
        if (dgvVentes.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvVentes.CurrentRow.Cells["Id"].Value);
        if (MessageBox.Show("Supprimer cette vente ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var vente = db.Ventes.Include(v => v.Lignes).FirstOrDefault(v => v.Id == id);
        if (vente != null)
        {
            db.Ventes.Remove(vente);
            db.SaveChanges();
        }
        ChargerVentes();
    }

    // ==================== 4. FOURNISSEURS & RÉCEPTIONS ====================

    private void ChargerFournisseurs()
    {
        using var db = new PharmacieContext();
        dgvFournisseurs.DataSource = db.Fournisseurs
            .Select(f => new { f.Id, f.Nom, f.Telephone, f.Adresse })
            .ToList();
    }

    private void BtnAjouterFournisseur_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomFournisseur.Text))
        {
            MessageBox.Show("Veuillez saisir le nom du fournisseur.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        db.Fournisseurs.Add(new Fournisseur(0, txtNomFournisseur.Text.Trim(), txtTelFournisseur.Text.Trim(), txtAdresseFournisseur.Text.Trim()));
        db.SaveChanges();
        ViderChampsFournisseur();
        ChargerFournisseurs();
        ChargerListesDeroulantes();
    }

    private void BtnModifierFournisseur_Click(object? sender, EventArgs e)
    {
        if (_selectedFournisseurId == null) return;
        using var db = new PharmacieContext();
        var f = db.Fournisseurs.Find(_selectedFournisseurId.Value);
        if (f == null) return;

        f.Nom = txtNomFournisseur.Text.Trim();
        f.Telephone = txtTelFournisseur.Text.Trim();
        f.Adresse = txtAdresseFournisseur.Text.Trim();
        db.SaveChanges();
        ViderChampsFournisseur();
        ChargerFournisseurs();
        ChargerListesDeroulantes();
    }

    private void BtnSupprimerFournisseur_Click(object? sender, EventArgs e)
    {
        if (_selectedFournisseurId == null) return;
        if (MessageBox.Show("Supprimer ce fournisseur ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var f = db.Fournisseurs.Find(_selectedFournisseurId.Value);
        if (f != null) { db.Fournisseurs.Remove(f); db.SaveChanges(); }
        ViderChampsFournisseur();
        ChargerFournisseurs();
        ChargerListesDeroulantes();
    }

    private void DgvFournisseurs_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvFournisseurs.Rows[e.RowIndex];
        _selectedFournisseurId = Convert.ToInt32(row.Cells["Id"].Value);
        txtNomFournisseur.Text = row.Cells["Nom"].Value?.ToString() ?? "";
        txtTelFournisseur.Text = row.Cells["Telephone"].Value?.ToString() ?? "";
        txtAdresseFournisseur.Text = row.Cells["Adresse"].Value?.ToString() ?? "";
    }

    private void ViderChampsFournisseur()
    {
        _selectedFournisseurId = null;
        txtNomFournisseur.Clear(); txtTelFournisseur.Clear(); txtAdresseFournisseur.Clear();
    }

    private void ChargerLivraisons()
    {
        using var db = new PharmacieContext();
        var liste = db.LivraisonsFournisseurs
            .Include(l => l.Fournisseur)
            .Include(l => l.Medicament)
            .OrderByDescending(l => l.Id)
            .ToList();

        dgvLivraisons.DataSource = liste.Select(l => new
        {
            l.Id,
            Fournisseur = l.Fournisseur.Nom,
            Medicament = $"{l.Medicament.Code} - {l.Medicament.Nom}",
            l.QuantiteRecue,
            DateReception = l.DateReception.ToString("dd/MM/yyyy")
        }).ToList();
    }

    private void BtnAjouterLivraison_Click(object? sender, EventArgs e)
    {
        if (cmbLivFournisseur.SelectedValue is not int fournisseurId ||
            cmbLivMedicament.SelectedValue is not string medCode)
        {
            MessageBox.Show("Veuillez sélectionner un fournisseur et un médicament.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtLivQuantite.Text.Trim(), out int qte) || qte <= 0)
        {
            MessageBox.Show("Veuillez saisir une quantité reçue valide (> 0).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        var fournisseur = db.Fournisseurs.Find(fournisseurId)!;
        var med = db.Medicaments.Find(medCode)!;

        fournisseur.AjouterMedicament(med);
        db.LivraisonsFournisseurs.Add(new LivraisonFournisseur
        {
            Fournisseur = fournisseur,
            Medicament = med,
            QuantiteRecue = qte,
            DateReception = dtpLivDate.Value.Date
        });
        db.SaveChanges();

        txtLivQuantite.Clear();
        ChargerLivraisons();
    }

    private void BtnSupprimerLivraison_Click(object? sender, EventArgs e)
    {
        if (dgvLivraisons.CurrentRow == null) return;
        int id = Convert.ToInt32(dgvLivraisons.CurrentRow.Cells["Id"].Value);
        if (MessageBox.Show("Supprimer cette réception ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var liv = db.LivraisonsFournisseurs.Find(id);
        if (liv != null) { db.LivraisonsFournisseurs.Remove(liv); db.SaveChanges(); }
        ChargerLivraisons();
    }

    // ==================== 5. CRUD PHARMACIENS ====================

    private void ChargerPharmaciens()
    {
        using var db = new PharmacieContext();
        dgvPharmaciens.DataSource = db.Pharmaciens
            .Select(p => new { p.Id, p.Nom, p.Prenom, p.Telephone, p.NumeroOrdre, p.Role, p.HorairesTravail })
            .ToList();
    }

    private void BtnAjouterPharmacien_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomPharm.Text) || string.IsNullOrWhiteSpace(txtPrenomPharm.Text) ||
            string.IsNullOrWhiteSpace(txtOrdrePharm.Text) || string.IsNullOrWhiteSpace(txtLoginPharm.Text) ||
            string.IsNullOrWhiteSpace(txtMdpPharm.Text))
        {
            MessageBox.Show("Veuillez remplir Nom, Prénom, N° Ordre, Login et Mot de passe.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        string role = cmbRolePharm.SelectedItem?.ToString() ?? Pharmacien.RolePharmacien;
        var pharmacien = new Pharmacien(
            0,
            txtNomPharm.Text.Trim(),
            txtPrenomPharm.Text.Trim(),
            txtTelPharm.Text.Trim(),
            txtOrdrePharm.Text.Trim(),
            role,
            txtLoginPharm.Text.Trim(),
            txtMdpPharm.Text
        )
        {
            HorairesTravail = txtHorairesPharm.Text.Trim()
        };

        db.Pharmaciens.Add(pharmacien);
        db.SaveChanges();
        ViderChampsPharmacien();
        ChargerPharmaciens();
        ChargerListesDeroulantes();
    }

    private void BtnModifierPharmacien_Click(object? sender, EventArgs e)
    {
        if (_selectedPharmacienId == null) return;
        using var db = new PharmacieContext();
        var p = db.Pharmaciens.Find(_selectedPharmacienId.Value);
        if (p == null) return;

        p.Nom = txtNomPharm.Text.Trim();
        p.Prenom = txtPrenomPharm.Text.Trim();
        p.Telephone = txtTelPharm.Text.Trim();
        p.NumeroOrdre = txtOrdrePharm.Text.Trim();
        p.Role = cmbRolePharm.SelectedItem?.ToString() ?? Pharmacien.RolePharmacien;
        p.HorairesTravail = txtHorairesPharm.Text.Trim();
        db.SaveChanges();

        ViderChampsPharmacien();
        ChargerPharmaciens();
        ChargerListesDeroulantes();
    }

    private void BtnSupprimerPharmacien_Click(object? sender, EventArgs e)
    {
        if (_selectedPharmacienId == null) return;
        if (MessageBox.Show("Supprimer ce pharmacien ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var p = db.Pharmaciens.Find(_selectedPharmacienId.Value);
        if (p != null) { db.Pharmaciens.Remove(p); db.SaveChanges(); }
        ViderChampsPharmacien();
        ChargerPharmaciens();
        ChargerListesDeroulantes();
    }

    private void DgvPharmaciens_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvPharmaciens.Rows[e.RowIndex];
        _selectedPharmacienId = Convert.ToInt32(row.Cells["Id"].Value);
        txtNomPharm.Text = row.Cells["Nom"].Value?.ToString() ?? "";
        txtPrenomPharm.Text = row.Cells["Prenom"].Value?.ToString() ?? "";
        txtTelPharm.Text = row.Cells["Telephone"].Value?.ToString() ?? "";
        txtOrdrePharm.Text = row.Cells["NumeroOrdre"].Value?.ToString() ?? "";
        cmbRolePharm.SelectedItem = row.Cells["Role"].Value?.ToString() ?? Pharmacien.RolePharmacien;
        txtHorairesPharm.Text = row.Cells["HorairesTravail"].Value?.ToString() ?? "";
    }

    private void ViderChampsPharmacien()
    {
        _selectedPharmacienId = null;
        txtNomPharm.Clear(); txtPrenomPharm.Clear(); txtTelPharm.Clear();
        txtOrdrePharm.Clear(); txtHorairesPharm.Clear(); txtLoginPharm.Clear(); txtMdpPharm.Clear();
    }

    // ==================== 6. CRUD MÉDECINS ====================

    private void ChargerMedecins()
    {
        using var db = new PharmacieContext();
        dgvMedecins.DataSource = db.Medecins
            .Select(m => new { m.Id, m.Nom, m.Prenom, m.Telephone, m.Specialite, m.NumeroOrdre })
            .ToList();
    }

    private void BtnAjouterMedecin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNomMedecin.Text) || string.IsNullOrWhiteSpace(txtPrenomMedecin.Text))
        {
            MessageBox.Show("Veuillez saisir au moins le nom et le prénom du médecin.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var db = new PharmacieContext();
        db.Medecins.Add(new Medecin(0, txtNomMedecin.Text.Trim(), txtPrenomMedecin.Text.Trim(), txtTelMedecin.Text.Trim(), txtSpecMedecin.Text.Trim(), txtOrdreMedecin.Text.Trim()));
        db.SaveChanges();
        ViderChampsMedecin();
        ChargerMedecins();
        ChargerListesDeroulantes();
    }

    private void BtnModifierMedecin_Click(object? sender, EventArgs e)
    {
        if (_selectedMedecinId == null) return;
        using var db = new PharmacieContext();
        var m = db.Medecins.Find(_selectedMedecinId.Value);
        if (m == null) return;

        m.Nom = txtNomMedecin.Text.Trim();
        m.Prenom = txtPrenomMedecin.Text.Trim();
        m.Telephone = txtTelMedecin.Text.Trim();
        m.Specialite = txtSpecMedecin.Text.Trim();
        m.NumeroOrdre = txtOrdreMedecin.Text.Trim();
        db.SaveChanges();

        ViderChampsMedecin();
        ChargerMedecins();
        ChargerListesDeroulantes();
    }

    private void BtnSupprimerMedecin_Click(object? sender, EventArgs e)
    {
        if (_selectedMedecinId == null) return;
        if (MessageBox.Show("Supprimer ce médecin ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        using var db = new PharmacieContext();
        var m = db.Medecins.Find(_selectedMedecinId.Value);
        if (m != null) { db.Medecins.Remove(m); db.SaveChanges(); }
        ViderChampsMedecin();
        ChargerMedecins();
        ChargerListesDeroulantes();
    }

    private void DgvMedecins_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var row = dgvMedecins.Rows[e.RowIndex];
        _selectedMedecinId = Convert.ToInt32(row.Cells["Id"].Value);
        txtNomMedecin.Text = row.Cells["Nom"].Value?.ToString() ?? "";
        txtPrenomMedecin.Text = row.Cells["Prenom"].Value?.ToString() ?? "";
        txtTelMedecin.Text = row.Cells["Telephone"].Value?.ToString() ?? "";
        txtSpecMedecin.Text = row.Cells["Specialite"].Value?.ToString() ?? "";
        txtOrdreMedecin.Text = row.Cells["NumeroOrdre"].Value?.ToString() ?? "";
    }

    private void ViderChampsMedecin()
    {
        _selectedMedecinId = null;
        txtNomMedecin.Clear(); txtPrenomMedecin.Clear(); txtTelMedecin.Clear(); txtSpecMedecin.Clear(); txtOrdreMedecin.Clear();
    }

    // ==================== UTILITAIRES INTERFACE ====================

    private static TableLayoutPanel CreerGrilleFormulaire(int nbLignes)
    {
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 6,
            RowCount = nbLignes,
            Padding = new Padding(6, 8, 6, 14)
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

        for (int i = 0; i < nbLignes; i++)
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        return table;
    }

    private static TextBox AjouterChampGrille(TableLayoutPanel grille, string labelText, int colLabel, int row)
    {
        var lbl = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(8, 12, 10, 12) };
        var txt = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(4, 8, 24, 8) };
        grille.Controls.Add(lbl, colLabel, row);
        grille.Controls.Add(txt, colLabel + 1, row);
        return txt;
    }

    private static ComboBox AjouterComboGrille(TableLayoutPanel grille, string labelText, int colLabel, int row)
    {
        var lbl = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(8, 12, 10, 12) };
        var cmb = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(4, 8, 24, 8) };
        grille.Controls.Add(lbl, colLabel, row);
        grille.Controls.Add(cmb, colLabel + 1, row);
        return cmb;
    }

    private static FlowLayoutPanel CreerBarreBoutons()
    {
        return new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(8, 14, 8, 6)
        };
    }

    private static Button CreerBouton(string texte, Color couleur)
    {
        return new Button
        {
            Text = texte,
            BackColor = couleur,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            AutoSize = true,
            MinimumSize = new Size(155, 44),
            Padding = new Padding(16, 6, 16, 6),
            Margin = new Padding(0, 0, 20, 0),
            Cursor = Cursors.Hand
        };
    }

    private static DataGridView CreerTableau()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            BackgroundColor = Color.White
        };
        dgv.RowTemplate.Height = 34;
        return dgv;
    }
}