using GestionDesRetardsEtAbseneces.Controllers;
using GestionDesRetardsEtAbseneces.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreNotification.xaml
    /// </summary>
    public partial class FenetreNotification : Window
    {
        DbGestgrhContext gestgrhContext = new();
        CollectionViewSource notificationViewSource;
        CollectionViewSource employeViewSource;
        public FenetreNotification()
        {
            InitializeComponent();
            notificationViewSource = (CollectionViewSource)FindResource(nameof(notificationViewSource));
            employeViewSource = (CollectionViewSource)FindResource(nameof(employeViewSource));
            gestgrhContext.Database.EnsureCreated();

            gestgrhContext.Notifications.Include(e=> e.IdEmployeNavigation).Load();
            gestgrhContext.Employes.Load();

            notificationViewSource.Source = gestgrhContext.Notifications.Local.ToObservableCollection();
            employeViewSource.Source = gestgrhContext.Employes.Local.ToObservableCollection();
            if (Utilitaires.timerInactivite.IsEnabled)
            {
                Utilitaires.timerInactivite.Stop();
            }
            Utilitaires.InitialiserTimer(this);
        }

        /// <summary>
        /// Recuperer les informations saisies par l'utilisateur
        /// </summary>
        /// <returns></returns>
        public (bool isValid, string typeNotification, DateOnly dateEnvoie, string contenuNotification, int idEmploye)? RecupererInformation()
        {
            string typeNotification = ComboBox_TypeNotification.Text;
            if (string.IsNullOrEmpty(typeNotification))
            {
                MessageBox.Show("Veuillez choisir le type de notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            if (!DatePicker_DateEnvoie.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez choisir la date d'envoie de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            DateOnly dateEnvoie = DateOnly.FromDateTime(DatePicker_DateEnvoie.SelectedDate.Value);

            TextRange contenuNotification = new TextRange(RichtxtBox_Message.Document.ContentStart, RichtxtBox_Message.Document.ContentEnd);
            if (string.IsNullOrEmpty(contenuNotification.Text))
            {
                MessageBox.Show("Veuillez saisir le contenu de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            string? idEmployeSelectionne = ComboBox_Employe.SelectedValue.ToString();
            if (string.IsNullOrEmpty(idEmployeSelectionne))
            {
                MessageBox.Show("Veuillez choisir l'employé", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            int idEmploye = int.Parse(idEmployeSelectionne);

            return (true, typeNotification, dateEnvoie, contenuNotification.Text, idEmploye);
        }
        /// <summary>
        /// Vider les champs de saisie
        /// </summary>
        public void ViderChamps()
        {
            ComboBox_TypeNotification.Text = "";
            ComboBox_Employe.SelectedIndex = -1;
            DatePicker_DateEnvoie.SelectedDate = null;
            RichtxtBox_Message.Document.Blocks.Clear();
            Window_Loaded(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Au chargement de la fenetre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Btn_Modifier.IsEnabled = false;
            Btn_Supprimer.IsEnabled = false;
            DataGrid_Notification.SelectedItem = null;
        }

        private void DataGrid_Notification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid_Notification.SelectedItem is null)
            {
                ViderChamps();
                return;
            }
            Btn_Modifier.IsEnabled = DataGrid_Notification.SelectedItem != null;
            Btn_Supprimer.IsEnabled = DataGrid_Notification.SelectedItem != null;

            if (DataGrid_Notification.SelectedItem is Notification notificationSelectionnee)
            {
                ComboBox_TypeNotification.Text = notificationSelectionnee.TypeNotification;
                DatePicker_DateEnvoie.SelectedDate = notificationSelectionnee.DateEnvoi;
                RichtxtBox_Message.Document.Blocks.Add(new Paragraph(new Run(notificationSelectionnee.MessageNotification)));
                ComboBox_Employe.SelectedValue = notificationSelectionnee.IdEmploye;
            }
        }
        /// <summary>
        /// Fontionnalité du boutton envoyer notifiation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Envoyer_Click(object sender, RoutedEventArgs e)
        {
            var information = RecupererInformation();
            if (information is null)
            {
                return;
            }

            var (isValid, typeNotification, dateEnvoie, contenuNotification, idEmploye) = information.Value;

            if (!isValid)
            {
                return;
            }

            Notification notification = new()
            {
                TypeNotification = typeNotification,
                DateEnvoi = dateEnvoie.ToDateTime(TimeOnly.MinValue),
                MessageNotification = contenuNotification,
                Statut = false,
                IdEmploye = idEmploye
            };

            try
            {
                gestgrhContext.Notifications.Add(notification);
                gestgrhContext.SaveChanges();
                MessageBox.Show("Notification envoyée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                ViderChamps();

            }
            catch (Exception)
            {
                MessageBox.Show("Erreur lors de l'envoie de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Fonctionnalité du boutton modifier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Modifier_Click(object sender, RoutedEventArgs e)
        {


            var information = RecupererInformation();
            if (information is null)
            {
                return;
            }

            var (isValid, typeNotification, dateEnvoie, contenuNotification, idEmploye) = information.Value;
            if (!isValid) { return; }

            if (DataGrid_Notification.SelectedItem is Notification notificationSelectionnee)
            {
                //Verifier si une modification a ete effectuée

                if (notificationSelectionnee.TypeNotification == typeNotification && notificationSelectionnee.DateEnvoi == dateEnvoie.ToDateTime(TimeOnly.MinValue) && notificationSelectionnee.MessageNotification == contenuNotification && notificationSelectionnee.IdEmploye == idEmploye)
                {
                    MessageBox.Show("Aucune modification n'a été effectuée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    Notification? notification = gestgrhContext.Notifications.Find(notificationSelectionnee.IdNotification);
                    if (notification is null)
                    {
                        MessageBox.Show("Notification introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    notificationSelectionnee.TypeNotification = typeNotification;
                    notificationSelectionnee.DateEnvoi = dateEnvoie.ToDateTime(TimeOnly.MinValue);
                    notificationSelectionnee.MessageNotification = contenuNotification;
                    notificationSelectionnee.IdEmploye = idEmploye;

                    try
                    {
                        gestgrhContext.SaveChanges();
                        MessageBox.Show("Notification modifiée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        ViderChamps();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Erreur lors de la modification de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        ViderChamps();
                        return;

                    }
                }

            }
        }

        /// <summary>
        /// Fonctionnalité du boutton supprimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid_Notification.SelectedItem is Notification notificationSelectionnee)
            {
                MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimer cette notification ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Notification? notification = gestgrhContext.Notifications.Find(notificationSelectionnee.IdNotification);
                    if (notification is null)
                    {
                        MessageBox.Show("Notification introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    gestgrhContext.Notifications.Remove(notification);
                    try
                    {
                        gestgrhContext.SaveChanges();
                        MessageBox.Show("Notification supprimée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        ViderChamps();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Erreur lors de la suppression de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        ViderChamps();
                        return;
                    }
                }

            }
        }
        /// <summary>
        /// Fonctionnalité du boutton actualiser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Actualiser_Click(object sender, RoutedEventArgs e)
        {
            ViderChamps();
        }
    }
}