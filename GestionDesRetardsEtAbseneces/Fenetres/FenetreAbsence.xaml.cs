using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Data;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreAbsence.xaml
    /// </summary>
    public partial class FenetreAbsence : Window
    {
        DbGestgrhContext dbGestgrhContext = new();
        CollectionViewSource employeViewSource;
        CollectionViewSource absenceViewSource;
        public FenetreAbsence()
        {
            InitializeComponent();
            employeViewSource = ((CollectionViewSource)(FindResource("employeViewSource")));
            absenceViewSource = ((CollectionViewSource)(FindResource("absenceViewSource")));
            dbGestgrhContext.Database.EnsureCreated();
            dbGestgrhContext.Absences.Include(a => a.IdEmployeNavigation).Load();
            dbGestgrhContext.Employes.Load();
            employeViewSource.Source = dbGestgrhContext.Employes.Local.ToObservableCollection();
            absenceViewSource.Source = dbGestgrhContext.Absences.Local.ToObservableCollection();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        //Vider champs
        public void ViderChamps()
        {
            ComboBox_Employe.SelectedIndex = -1;
            DatePicker_DateAbsence.SelectedDate = null;
            TextBoxJustificatif.Text = "";
            ComboBox_Status.SelectedIndex = -1;
            ComboBox_Type.SelectedIndex = -1;
            Window_Loaded(this, new RoutedEventArgs());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Desactiver les bouttons modifier et supprimer
            Btn_Modifier.IsEnabled = false;
            Btn_Supprimer.IsEnabled = false;

            ComboBox_Type.SelectedItem = null;
            ComboBox_Status.SelectedItem = null;
            ComboBox_Employe.SelectedItem = null;
            DatePicker_DateAbsence.SelectedDate = null;
            TextBoxJustificatif.Text = "";
        }
        public bool ValideBool(string valide)
        {
            if (valide == "Valide")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Fontion pour recuperer les valeurs saisies
        public (bool isValid, int idEmploye, string type, bool status, DateTime dateAbsence, string justificatif)? RecupererInformations()
        {
            int idEmploye = ComboBox_Employe.SelectedIndex;
            if (idEmploye == -1)
            {
                MessageBox.Show("Veuillez selectionner un employé", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            string? type = ComboBox_Type.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Veuillez selectionner un type d'absence", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            string? statusSaisie = ComboBox_Status.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(statusSaisie) || string.IsNullOrEmpty(statusSaisie))
            {
                MessageBox.Show("Veuillez selectionner un status", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            bool status= ValideBool(statusSaisie);
            if (!DatePicker_DateAbsence.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez selectionner une date", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            DateTime dateAbsence = DatePicker_DateAbsence.SelectedDate.Value;

            string justificatif = TextBoxJustificatif.Text;
            if (string.IsNullOrWhiteSpace(justificatif) || string.IsNullOrEmpty(justificatif))
            {
                MessageBox.Show("Veuillez saisir un justificatif", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            return (true, idEmploye, type, status, dateAbsence, justificatif);
        }

        //Boutton ajouter
        private void Btn_Ajouter_Click(object sender, RoutedEventArgs e)
        {
            var informations = RecupererInformations();
            if (informations is null)
            {
                return;
            }

            var (isValid, idEmploye, type, status, dateAbsence, justificatif) = informations.Value;
            if (!isValid)
            {
                return;
            }

            try
            {
                Absence absence = new()
                {
                    IdEmploye = idEmploye,
                    TypeAbsence = type,
                    Valide = status,
                    DateAbsence = dateAbsence,
                    Justification = justificatif
                };
                dbGestgrhContext.Absences.Add(absence);
                dbGestgrhContext.SaveChanges();
                MessageBox.Show("Absence ajoutée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                ViderChamps();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        //Bouton modifier
        private void Btn_Modifier_Click(object sender, RoutedEventArgs e)
        {
            var informations = RecupererInformations();
            if (informations is null)
            {
                return;
            }
            var (isValid, idEmploye, type, status, dateAbsence, justificatif) = informations.Value;
            if (!isValid)
            {
                return;
            }
            if (DataGrid_Absence.SelectedItem is null)
            {
                MessageBox.Show("Veillez selectionné un absence a modifier!", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (DataGrid_Absence.SelectedItem is Absence absenceSelectionnee)
                {
                    if (absenceSelectionnee.IdEmploye==idEmploye && absenceSelectionnee.DateAbsence==dateAbsence && absenceSelectionnee.Valide==status &&
                        string.Equals(absenceSelectionnee.TypeAbsence,type,StringComparison.OrdinalIgnoreCase) && 
                        string.Equals(absenceSelectionnee.Justification,justificatif,StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Aucune modification effectuée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    absenceSelectionnee.IdEmploye = idEmploye;
                    absenceSelectionnee.TypeAbsence = type;
                    absenceSelectionnee.Valide = status;
                    absenceSelectionnee.DateAbsence = dateAbsence;
                    absenceSelectionnee.Justification = justificatif;
                    dbGestgrhContext.SaveChanges();
                    MessageBox.Show("Absence modifiée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    ViderChamps();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
