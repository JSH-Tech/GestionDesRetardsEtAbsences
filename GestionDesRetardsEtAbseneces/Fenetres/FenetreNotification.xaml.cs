using GestionDesRetardsEtAbseneces.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GestionDesRetardsEtAbseneces.Fenetres
{
    /// <summary>
    /// Interaction logic for FenetreNotification.xaml
    /// </summary>
    public partial class FenetreNotification : Window
    {
        public FenetreNotification()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Recuperer les informations saisies par l'utilisateur
        /// </summary>
        /// <returns></returns>
        public (bool isValid, string typeNotification, DateOnly dateEnvoie, string contenuNotification)? RecupererInformation()
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
            DateOnly dateEnvoie = DateOnly.FromDateTime(DatePicker_DateEnvoie.SelectedDate.Value) ;
            
            TextRange contenuNotification = new TextRange(RichtxtBox_Message.Document.ContentStart, RichtxtBox_Message.Document.ContentEnd);
            if (string.IsNullOrEmpty(contenuNotification.Text))
            {
                MessageBox.Show("Veuillez saisir le contenu de la notification", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return (true, typeNotification, dateEnvoie, contenuNotification.Text);
        }
        /// <summary>
        /// Vider les champs de saisie
        /// </summary>
        public void ViderChamps()
        {
            ComboBox_TypeNotification.Text = "";
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
            Btn_Modifier.IsEnabled=DataGrid_Notification.SelectedItem != null;
            Btn_Supprimer.IsEnabled = DataGrid_Notification.SelectedItem != null;

            if (DataGrid_Notification.SelectedItem is Notification notificationSelectionnee )
            {
                ComboBox_TypeNotification.SelectedItem = notificationSelectionnee.TypeNotification;
                DatePicker_DateEnvoie.SelectedDate = notificationSelectionnee.DateEnvoi;
                RichtxtBox_Message.Document.Blocks.Add(new Paragraph(new Run(notificationSelectionnee.MessageNotification)));

            }
        }
        private void Btn_Envoyer_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
