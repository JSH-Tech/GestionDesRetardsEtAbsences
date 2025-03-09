using GestionDesRetardsEtAbseneces.Model;
using System.Windows;
using System.Windows.Threading;

namespace GestionDesRetardsEtAbseneces.Controllers
{
    
    class Utilitaires
    {
        //public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        DbGestgrhContext dbgestgrhContext = new DbGestgrhContext();
        public void AfficherNotifications(int idEmploye)
        {
            var notifications = dbgestgrhContext.Notifications.Where(n => n.IdEmploye == idEmploye && !n.Statut).ToList();

            //Si le nombre de notification est different de 0 ou il y a des notifications avec status false
            if (notifications.Count != 0)
            {
                string message = "Vous avez " + notifications.Count + " nouvelle(s) notification(s):\n\n";
                foreach (var notification in notifications)
                {
                    message +=$"{notification.TypeNotification}: {notification.MessageNotification}\n";
                    notification.Statut = true;
                }
                dbgestgrhContext.SaveChanges();
                MessageBox.Show(message, "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Vous n'avez pas de nouvelle notification", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void Deconnexion(Window fenetreActuelle)
        {
            // Arrêter le timer pour éviter qu'il tourne en arrière-plan
            if (timerInactivite.IsEnabled)
            {
                timerInactivite.Stop();
            }

            // Réinitialiser les données utilisateur
            SessionUtilisateur.IdEmploye = 0;
            SessionUtilisateur.Nom = null;
            SessionUtilisateur.Prenom = null;
            SessionUtilisateur.Email = null;
            SessionUtilisateur.Role = null;
            SessionUtilisateur.HeureConnexion = DateTime.MinValue;

            // Ouvrir la fenêtre de connexion

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            fenetreActuelle.Close();

        }


        public static DispatcherTimer timerInactivite = new();
        public static void InitialiserTimer(Window fenetreAtuelle)
        {
            timerInactivite.Stop();
            timerInactivite = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.5)
            };
            timerInactivite.Tick += (s, e) => Deconnexion(fenetreAtuelle);
            timerInactivite.Start();
        }

    }
}
