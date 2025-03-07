using GestionDesRetardsEtAbseneces.Model;
using System.Security.Cryptography;

namespace GestionDesRetardsEtAbseneces.Controllers
{
    class SessionEtAuthentification
    {
        DbGestgrhContext dbGestgrhContext=new DbGestgrhContext();
        public static string GenererTokenSession()
        {
            byte[] tokenData = new byte[32];
            RandomNumberGenerator.Fill(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public void MettreAJourAuthentification(int idEmploye)
        {
            string tokenSession = GenererTokenSession();
            DateTime dateConnexion = DateTime.Now;
            DateTime dateExpiration = dateConnexion.AddMinutes(5);

            //Enregistrement de la session
            Authentification nouvelAuthentification = new();
            nouvelAuthentification.IdEmploye = idEmploye;
            nouvelAuthentification.DateDerniereConnexion = dateConnexion;
            nouvelAuthentification.TokenSession = tokenSession;
            nouvelAuthentification.DateExpiration = dateExpiration;

            dbGestgrhContext.Authentifications.Add(nouvelAuthentification);
            dbGestgrhContext.SaveChanges();

            SessionUtilisateur.IdEmploye = idEmploye;
            SessionUtilisateur.HeureConnexion = dateConnexion;
            SessionUtilisateur.Email = dbGestgrhContext.Employes.Find(idEmploye)?.Email;
            SessionUtilisateur.Role = dbGestgrhContext.Employes.Find(idEmploye)?.RoleEmploye;
            SessionUtilisateur.Nom = dbGestgrhContext.Employes.Find(idEmploye)?.Nom;
            SessionUtilisateur.Prenom = dbGestgrhContext.Employes.Find(idEmploye)?.Prenom;

        }

        
    }
}
