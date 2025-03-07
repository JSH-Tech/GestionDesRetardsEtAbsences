using GestionDesRetardsEtAbseneces.Model;
using System.Windows;

namespace GestionDesRetardsEtAbseneces.Controllers
{
    
    class Utilitaires
    {
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
    }
}
