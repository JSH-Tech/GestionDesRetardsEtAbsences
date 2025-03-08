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
    /// Interaction logic for FenetreDashboard.xaml
    /// </summary>
    public partial class FenetreDashboard : Window
    {
        DbGestgrhContext dbGestgrhContext = new();
        public FenetreDashboard()
        {
            InitializeComponent();
            ChargerDonneesTableauDeBord();

        }
        private void ChargerDonneesTableauDeBord()
        {
            int totalAbsences = ObtenirTotalAbsences();
            int totalRetards = ObtenirTotalRetards();

            // Mise à jour des données affichées
            AbsencesCount.Text = $"Total: {totalAbsences}";
            RetardsCount.Text = $"Total: {totalRetards}";
        }

        private int ObtenirTotalAbsences()
        {
            // Obter le total des absences (à remplacer par l'accès à la base de données).
            return dbGestgrhContext.Absences.Count();
        }

        private int ObtenirTotalRetards()
        {
            // Obter le total des retards (à remplacer par l'accès à la base de données).
            return dbGestgrhContext.Retards.Count();
        }
    }
}
