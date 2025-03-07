using GestionDesRetardsEtAbseneces.Fenetres;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionDesRetardsEtAbseneces
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ouvre la fenetre de gestion des rapports
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Rapports_Click(object sender, RoutedEventArgs e)
        {
            FenetreRapport fenetreRapport = new FenetreRapport();
            fenetreRapport.Show();
        }

        private void Btn_Employes_Click(object sender, RoutedEventArgs e)
        {

        }
      /*  private void OpenFenetreConges()
        {
            FenetreConges fenetreConges = new FenetreConges();
            fenetreConges.Show(); // Affiche FenetreConges comme une nouvelle fenêtre
        }
       
        */
        private void Btn_Notifications_Click(object sender, RoutedEventArgs e)
        {

            //OpenFenetreConges();
        }
    }
}