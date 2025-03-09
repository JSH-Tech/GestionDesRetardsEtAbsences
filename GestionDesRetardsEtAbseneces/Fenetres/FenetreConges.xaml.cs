using GestionDesRetardsEtAbseneces.Controllers;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Data;
namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Logique d'interaction pour FenetreConges.xaml
    /// </summary>
    public partial class FenetreConges : Window
    {

        public class DemandeConge
        {

            public int IdDemande { get; set; }
            public string Nom { get; set; }
            public string Prenom { get; set; }
            public string TypeConge { get; set; }
            public DateTime DateDebut { get; set; }
            public DateTime DateFin { get; set; }
            public string Justification { get; set; }
            public string Statut { get; set; }
            public string NomComplet
            {
                get
                {
                    return $"{Nom ?? string.Empty} {Prenom ?? string.Empty}".Trim();
                }
                set
                {
                    // Par exemple, découper la valeur du nom complet en nom et prénom.
                    var parts = value.Split(' ');
                    Nom = parts.Length > 0 ? parts[0] : string.Empty;
                    Prenom = parts.Length > 1 ? parts[1] : string.Empty;
                }
            }

        }


        public FenetreConges()
        {

            InitializeComponent();
            Demandes = new ObservableCollection<DemandeConge>();
            this.DataContext = this;
            ChargerDemandes();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        // La chaîne de connexion à ta base de données MySQL
        private string connectionString = "Server=localhost;Database=db_gestgrh;Uid=root";

        // Méthode pour exécuter une requête et retourner les résultats sous forme de DataTable
        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l'exécution de la requête : " + ex.Message);
                }
            }

            return dataTable;
        }

        // Méthode pour exécuter une commande INSERT, UPDATE, DELETE
        public void ExecuteNonQuery(string query)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de l'exécution de la commande : " + ex.Message);
                }
            }



        }

        public ObservableCollection<DemandeConge> Demandes { get; set; }

        private int GetIdEmployeByNomPrenom(string nomComplet)
        {
            string[] noms = nomComplet.Split(' '); // Séparer le nom et prénom
            string nom = noms[0];
            string prenom = noms[1];

            // Requête SQL pour récupérer l'ID de l'employé
            string query = "SELECT idEmploye FROM employe WHERE nom = @nom AND prenom = @prenom";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@prenom", prenom);

                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0; // Si l'employé existe, retourner son ID
            }
        }

        // Fonction pour insérer une demande de congé dans la base de données

        private void Btn_Soumettre_Click(object sender, RoutedEventArgs e)
        {
            // Vérification que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(NomCompletTextBox.Text) ||
                myComboBox.SelectedIndex == -1 ||
                DateDebutDatePicker.SelectedDate == null ||
                DateFinDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(JustificatifTextBox.Text))
            {
                MessageBox.Show("Veuillez renseigner tous les champs.");
                return;  // Ne pas soumettre si les champs ne sont pas remplis
            }

            // Vérification que la date de fin n'est pas inférieure à la date de début
            if (DateDebutDatePicker.SelectedDate > DateFinDatePicker.SelectedDate)
            {
                MessageBox.Show("La date de fin ne peut pas être inférieure à la date de début.");
                return;  // Ne pas soumettre si la date de fin est avant la date de début
            }

            // Récupérer les informations du formulaire
            string nomComplet = NomCompletTextBox.Text;
            string typeConge = (myComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime dateDebut = DateDebutDatePicker.SelectedDate.Value;
            DateTime dateFin = DateFinDatePicker.SelectedDate.Value;
            string justificatif = JustificatifTextBox.Text;

            // Récupérer l'ID de l'employé
            int idEmploye = GetIdEmployeByNomPrenom(nomComplet);

            if (idEmploye == 0)
            {
                MessageBox.Show("Employé non trouvé.");
                return;
            }
            if (idDemandeEnModification == -1)
            {
                // Requête SQL pour insérer la demande de congé
                string query = "INSERT INTO demandeconge (idEmploye, typeConge, dateDebut, dateFin, justification, statut) " +
                           "VALUES (@idEmploye, @typeConge, @dateDebut, @dateFin, @justification, 'En attente')";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("@idEmploye", idEmploye);
                        command.Parameters.AddWithValue("@typeConge", typeConge);
                        command.Parameters.AddWithValue("@dateDebut", dateDebut);
                        command.Parameters.AddWithValue("@dateFin", dateFin);
                        command.Parameters.AddWithValue("@justification", justificatif);

                        command.ExecuteNonQuery(); // Exécuter la requête
                        MessageBox.Show("Demande de congé soumise avec succès.");
                        NettoyerChamps();
                        // Rafraîchir la liste des demandes de congé après l'ajout
                        ChargerDemandes();
                        NomCompletTextBox.Clear();
                        JustificatifTextBox.Clear();
                        DateDebutDatePicker.SelectedDate = null;
                        DateFinDatePicker.SelectedDate = null;
                        myComboBox.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de l'insertion de la demande : " + ex.Message);
                    }
                }
            }
            else
            {
                //ceci est un commentaire
                // Si une demande est en modification, on met à jour la demande existante
                string query = "UPDATE demandeconge SET idEmploye = @idEmploye, typeConge = @typeConge, " +
                               "dateDebut = @dateDebut, dateFin = @dateFin, justification = @justification WHERE idDemande = @idDemande";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("@idEmploye", idEmploye);
                        command.Parameters.AddWithValue("@typeConge", typeConge);
                        command.Parameters.AddWithValue("@dateDebut", dateDebut);
                        command.Parameters.AddWithValue("@dateFin", dateFin);
                        command.Parameters.AddWithValue("@justification", justificatif);
                        command.Parameters.AddWithValue("@idDemande", idDemandeEnModification);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Demande de congé modifiée avec succès.");

                        // Réinitialiser l'ID de la demande modifiée
                        idDemandeEnModification = -1;

                        // Changer le texte du bouton "Enregistrer" de retour à "Soumettre"
                        Btn_Soumettre.Content = "Soumettre";
                        NettoyerChamps();
                        // Rafraîchir la liste des demandes
                        ChargerDemandes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors de la mise à jour de la demande : " + ex.Message);
                    }
                }
            }
        }

        // Fonction pour charger la liste des demandes de congé
        private void ChargerDemandes()
        {
            string query = "SELECT d.idDemande, e.nom, e.prenom, d.typeConge, d.dateDebut, d.dateFin, d.justification " +
                           "FROM demandeconge d JOIN employe e ON d.idEmploye = e.idEmploye";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable demandeCongeData = new DataTable();
                    adapter.Fill(demandeCongeData); // Remplir le DataTable avec les résultats

                    // Effacer les anciennes données
                    Demandes.Clear();

                    // Ajouter les nouvelles demandes à la collection
                    foreach (DataRow row in demandeCongeData.Rows)
                    {
                        Demandes.Add(new DemandeConge
                        {
                            IdDemande = Convert.ToInt32(row["idDemande"]),
                            Nom = row["nom"].ToString(),
                            Prenom = row["prenom"].ToString(),
                            TypeConge = row["typeConge"].ToString(),
                            DateDebut = Convert.ToDateTime(row["dateDebut"]),
                            DateFin = Convert.ToDateTime(row["dateFin"]),
                            Justification = row["justification"].ToString()

                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors du chargement des demandes : " + ex.Message);
                }
            }
        }
        private int idDemandeEnModification = -1;  // Variable pour stocker l'ID de la demande à modifier

        private void Btn_Modifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Vérifie que le sender est un bouton
                if (sender is Button button && button.Tag != null)
                {
                    // Récupérer l'ID de la demande à modifier
                    int idDemande = (int)button.Tag;

                    if (idDemande == 0)
                    {
                        MessageBox.Show("Aucune demande sélectionnée.");
                        return;
                    }

                    // Requête pour récupérer les détails de la demande à modifier
                    string query = "SELECT d.idDemande, e.nom, e.prenom, d.typeConge, d.dateDebut, d.dateFin, d.justification " +
                                   "FROM demandeconge d JOIN employe e ON d.idEmploye = e.idEmploye WHERE d.idDemande = @idDemande";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("@idDemande", idDemande);

                        MySqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // Remplir les champs avec les données de la demande
                            NomCompletTextBox.Text = reader["nom"].ToString() + " " + reader["prenom"].ToString();
                            myComboBox.SelectedItem = myComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == reader["typeConge"].ToString());
                            DateDebutDatePicker.SelectedDate = Convert.ToDateTime(reader["dateDebut"]);
                            DateFinDatePicker.SelectedDate = Convert.ToDateTime(reader["dateFin"]);
                            JustificatifTextBox.Text = reader["justification"].ToString();


                            // Désactive le bouton Soumettre et change son texte en Enregistrer
                            Btn_Soumettre.Content = "Enregistrer";
                            idDemandeEnModification = idDemande;  // Met à jour l'ID de la demande en modification

                        }
                        else
                        {
                            MessageBox.Show("Demande non trouvée !");
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Tag de bouton non valide.");
                }
            }

            catch (Exception ex)
            {
                // Afficher l'exception dans un message box
                MessageBox.Show("Erreur lors de la récupération des données : " + ex.Message);
            }

        }



        // Fonction pour gérer la suppression d'une demande

        private void Btn_Supprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Vérifier que le sender est un bouton et que le Tag est un int
                if (sender is Button button && button.Tag is int idDemande)
                {
                    if (idDemande == 0)
                    {
                        MessageBox.Show("ID de demande invalide.");
                        return;
                    }

                    // Demander confirmation à l'utilisateur
                    MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cette demande de congé ?",
                                                              "Confirmation de suppression",
                                                              MessageBoxButton.YesNo,
                                                              MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Connexion à la base de données pour supprimer la demande de congé
                        string query = "DELETE FROM demandeconge WHERE idDemande = @idDemande";

                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            MySqlCommand command = new MySqlCommand(query, conn);
                            command.Parameters.AddWithValue("@idDemande", idDemande);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Demande de congé supprimée avec succès.");
                                // Recharger les données dans le DataGrid après la suppression
                                ChargerDemandesConges();  // Assure-toi que cette méthode recharge correctement les données dans le DataGrid
                            }
                            else
                            {
                                MessageBox.Show("Erreur lors de la suppression de la demande.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Tag de bouton non valide.");
                }
            }
            catch (Exception ex)
            {
                // Afficher l'exception dans un message box
                MessageBox.Show("Erreur lors de la suppression des données : " + ex.Message);
            }
        }

        private void ChargerDemandesConges()
        {
            try
            {
                string query = "SELECT d.idDemande, e.nom, e.prenom, d.typeConge, d.dateDebut, d.dateFin, d.justification " +
                               "FROM demandeconge d JOIN employe e ON d.idEmploye = e.idEmploye";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand(query, conn);
                    MySqlDataReader reader = command.ExecuteReader();

                    Demandes.Clear(); // Vide la collection avant de la recharger

                    while (reader.Read())
                    {
                        DemandeConge demande = new DemandeConge
                        {
                            IdDemande = reader.GetInt32("idDemande"),
                            NomComplet = reader.GetString("nom") + " " + reader.GetString("prenom"),
                            TypeConge = reader.GetString("typeConge"),
                            DateDebut = reader.GetDateTime("dateDebut"),
                            DateFin = reader.GetDateTime("dateFin"),
                            Justification = reader.GetString("justification"),
                            Statut = reader.GetString("statut")
                        };
                        Demandes.Add(demande); // Ajoute la demande dans la collection
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des demandes de congé : " + ex.Message);
            }
        }


        private void NettoyerChamps()
        {
            // Réinitialiser les champs TextBox
            NomCompletTextBox.Clear();
            JustificatifTextBox.Clear();

            myComboBox.SelectedIndex = -1;
            DateDebutDatePicker.SelectedDate = null;
            DateFinDatePicker.SelectedDate = null;
        }



        private void Btn_Annuler_Click(object sender, RoutedEventArgs e)
        {
            NettoyerChamps();

        }




    }

}
