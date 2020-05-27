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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProefEx.LIB.Entities;
using ProefEx.LIB.Services;

namespace ProefEx.WPF
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

        // globale var om bij bewaren te weten of het over een nieuw record of een bestaand record gaat
        bool isNieuw;
        // globale var dataService : alle functionaliteiten van deze klasse zijn hierdoor
        // overal in onze WPF code behind beschikbaar
        DataService dataService;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // instantie maken van onze globale var
            // de klasse voert nu zijn constructor uit en gaat ofwel een dataset 
            // aanmaken, ofwel de XML file uitlezen
            dataService = new DataService();
            // listbox vullen met de inhoud van de list die aangeboden wordt
            // via de property van de klasse DataService (na het uitvoeren van
            // constructor is deze nu immers gevuld
            lstRassen.ItemsSource = dataService.Rassen;

            // screen visueel organiseren
            grpRassen.IsEnabled = true;
            grpBewerken.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // wanneer het programma stopt dient deze methode uitgevoerd te worden
            // zodat de dataset in een XML file bewaard wordt.
            dataService.SaveData();
        }
        private void lstRassen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtRas.Text = "";
            if (lstRassen.SelectedItem == null) return;

            Ras ras =(Ras)lstRassen.SelectedItem;
            txtRas.Text = ras.RasNaam;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            isNieuw = true;
            // screen visueel organiseren
            grpRassen.IsEnabled = false;
            grpBewerken.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            txtRas.Text = "";
            txtRas.Focus();

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // indien niets geselecteerd in listbox, geen actie ondernemen
            if (lstRassen.SelectedItem == null) return;
            
            isNieuw = false;
            // screen visueel organiseren
            grpRassen.IsEnabled = false;
            grpBewerken.IsEnabled = true;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            txtRas.Focus();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // indien niets geselecteerd in listbox, geen actie ondernemen
            if (lstRassen.SelectedItem == null) return;

            if (dataService.DeleteRas((Ras)lstRassen.SelectedItem))
            {
                // verwijderen is geslaagd.  Listbox opnieuw vullen
                lstRassen.ItemsSource = null;
                lstRassen.Items.Clear();
                lstRassen.ItemsSource = dataService.Rassen;
            }
            else
            {
                // verwijderen mislukt, algemene foutmelding
                MessageBox.Show("Het verwijderen is niet geslaagd", "DB ERROR");
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Ras ras;
            if(isNieuw)
            {
                // nieuw object Ras aanmaken
                ras = new Ras();
                ras.RasNaam = txtRas.Text;
                if(dataService.SaveNewRas(ras))
                {
                    // toevoegen is geslaagd, listbox opnieuw vullen
                    lstRassen.ItemsSource = null;
                    lstRassen.Items.Clear();
                    lstRassen.ItemsSource = dataService.Rassen;
                }
                else
                {
                    // toevoegen is mislukt, algemene foutmelding
                    MessageBox.Show("Het toevoegen is niet geslaagd : je dient een unieke rasnaam op te geven", "DB ERROR");
                    return;
                }
            }
            else
            {
                // het geselecteerde Ras object uit de listbox halen
                ras = (Ras)lstRassen.SelectedItem;
                // de originele rasnaam bijhouden : mocht het wijzigen mislukken
                // dan moeten we terugvallen op de oude rasnaam.
                string oudeRasNaam = ras.RasNaam;

                // de eigenschap rasnaam nu aanpassen               
                ras.RasNaam = txtRas.Text;
                if(dataService.UpdateRas(ras))
                {
                    // wijziging is geslaagd, listbox opnieuw vullen
                    lstRassen.ItemsSource = null;
                    lstRassen.Items.Clear();
                    lstRassen.ItemsSource = dataService.Rassen;
                }
                else
                {
                    // wijziging is mislukt, algemene foutmelding
                    MessageBox.Show("Het wijzigen is niet geslaagd : je dient een unieke rasnaam op te geven", "DB ERROR");
                    // niet vergeten, het onderliggen record is dus NIET
                    // aangepast, dus de property rasnaam terugzetten
                    ras.RasNaam = oudeRasNaam;

                    return;
                }
            }
            // toevoeging of wijziging is geslaagd
            // we proberen in onze listbox nog het ras te selecteren
            // dat we zopas hebben toegevoegd/gewijzigd
            int indeks = 0;
            foreach(Ras zoekRas in lstRassen.Items)
            {
                if(zoekRas.RasNaam == ras.RasNaam)
                {
                    lstRassen.SelectedIndex = indeks;
                    lstRassen_SelectionChanged(null, null);
                    break;
                }
                indeks++;
            }

            // screen visueel organiseren
            grpRassen.IsEnabled = true;
            grpBewerken.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;


        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // screen visueel organiseren
            grpRassen.IsEnabled = true;
            grpBewerken.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            // via code behind het klikken op de listbox simuleren
            lstRassen_SelectionChanged(null, null);
            lstRassen.Focus();
        }


    }
}
