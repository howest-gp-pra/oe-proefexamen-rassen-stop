using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using ProefEx.LIB.Entities;
using System.IO;


namespace ProefEx.LIB.Services
{
    public class DataService
    {
        #region PrivateVariabelen
        private List<Ras> rassen;
        private DataSet DS;
        #endregion
        #region Properties
        public List<Ras> Rassen
        {
            get { return rassen; }
        }
        #endregion
        #region Constructor
        public DataService()
        {
            DS = new DataSet();
            // DataSet laten vullen
            ReadData();
            // list laten vullen met records uit dataset
            vulRassen();
        }
        #endregion
        #region PrivateMethoden
        private void ReadData()
        {
            // deze methode probeert het xml bestand te vinden en 
            // uit te lezen.  Indien het XML bestand nog niet bestaat
            // dan wordt de structuur van de dataset aangemaakt
            // en worden voorbeeldgegevens gevuld
            string xmlMap = Directory.GetCurrentDirectory() + "/XMLBestanden";
            string xmlBestand = Directory.GetCurrentDirectory() + "/XMLBestanden/rassen.xml";
            if (!Directory.Exists(xmlMap))
                Directory.CreateDirectory(xmlMap);
            if (!File.Exists(xmlBestand))
            {
                // indien xml bestand nog niet bestaat DataSet aanmaken en vullen
                // met voorbeeldgegevens
                CreateTables();
                CreateSeedings();
            }
            else
            {
                // indien xml bestand wel bestaat DataSet vullen
                // met gegevens uit XML bestand
                DS.ReadXml(xmlBestand, XmlReadMode.ReadSchema);
            }
        }
        private void CreateTables()
        {
            // het XML bestand werd niet gevonden.  We gaan dus de structuur 
            // hier gaan aanmaken.

            // datatable Soorten aanmaken
            DataTable dtRassen = new DataTable();
            dtRassen.TableName = "Rassen";

            // datacolumns aanmaken
            DataColumn dc;
            dc = new DataColumn();
            dc.ColumnName = "ID";
            dc.DataType = typeof(int);
            dc.Unique = true;
            dc.AutoIncrement = true;
            dc.AutoIncrementSeed = 1;
            dc.AutoIncrementStep = 1;
            dtRassen.Columns.Add(dc);
            dtRassen.PrimaryKey = new DataColumn[] { dc };

            dc = new DataColumn();
            dc.ColumnName = "RasNaam";
            dc.DataType = typeof(string);
            dc.Unique = true;
            dc.AllowDBNull = false;
            dtRassen.Columns.Add(dc);

            // de datatable toevoegen aan de dataset
            DS.Tables.Add(dtRassen);
        }
        private void CreateSeedings()
        {
            // voorbeeldgegevens toevoegen aan de dataset
            DataRow dr = DS.Tables["Rassen"].NewRow();
            dr["RasNaam"] = "Cokcer spaniel";
            DS.Tables["Rassen"].Rows.Add(dr);

            dr = DS.Tables["Rassen"].NewRow();
            dr["RasNaam"] = "Duitse herder";
            DS.Tables["Rassen"].Rows.Add(dr);

            dr = DS.Tables["Rassen"].NewRow();
            dr["RasNaam"] = "Canis familiaris";
            DS.Tables["Rassen"].Rows.Add(dr);
        }
        private void vulRassen()
        {
            // de private list variabele rassen vullen
            // met de inhoud van de dataset
            rassen = new List<Ras>();
            foreach (DataRow rw in DS.Tables["rassen"].Rows)
            {
                rassen.Add(new Ras() { ID = int.Parse(rw["id"].ToString()), RasNaam = rw["rasnaam"].ToString() });
            }
            // we sorteren de list op rasnaam
            rassen = rassen.OrderBy(r => r.RasNaam).ToList();
        }
        private bool IsRasNaamUniek(Ras ras)
        {
            // deze methode kijkt na of de rasnaam van een nieuw
            // toe te voegen ras of een te wijzigen rasnaam uniek is
            // zo vermijden we dubbele waarden.
            // deze methode zal uitgevoerd worden telkens 
            // we toevoegen of wijzigen
            DataRow dr = DS.Tables["Rassen"].Select("id<>" + ras.ID.ToString() + " and RasNaam = '" + ras.RasNaam + "'" ).FirstOrDefault();
            if (dr == null)
                return true;
            else
                return false;
        }
        #endregion
        #region PubliekeMethoden
        public bool SaveNewRas(Ras ras)
        {
            // de rasnaam vooraf trimmen om spaties te vermijden
            ras.RasNaam = ras.RasNaam.Trim();
            // de rasnaam dient ingevuld te zijn
            if (ras.RasNaam == "")
                return false;
            // de rasnaam dient uniek te zijn
            if (!IsRasNaamUniek(ras))
                return false;
            // nieuw record aanmaken
            DataRow dr = DS.Tables["Rassen"].NewRow();
            dr["RasNaam"] = ras.RasNaam;
            try
            {
                // nieuw record proberen te bewaren
                DS.Tables["Rassen"].Rows.Add(dr);
                // indien geslaagd, de list Rassen opnieuw vullen
                vulRassen();
                return true;
            }
            catch (Exception fout)
            {
                return false;
            }
        }
        public bool UpdateRas(Ras ras)
        {
            // de rasnaam vooraf trimmen om spaties te vermijden
            ras.RasNaam = ras.RasNaam.Trim();
            // de rasnaam dient ingevuld te zijn
            if (ras.RasNaam == "")
                return false;
            // de rasnaam dient uniek te zijn
            if (!IsRasNaamUniek(ras))
                return false;
            // we proberen het juiste record te vinden
            DataRow dr = DS.Tables["Rassen"].Select("id=" + ras.ID.ToString()).FirstOrDefault();
            if (dr == null)
                return false;
            try
            {
                // we passen het gevonden record aan
                dr["RasNaam"] = ras.RasNaam ;
                // indien geslaagd, de list Rassen opnieuw vullen
                vulRassen();
                return true;
            }
            catch (Exception fout)
            {
                return false;
            }
        }
        public bool DeleteRas(Ras ras)
        {
            // we proberen het juiste record te vinden
            DataRow dr = DS.Tables["Rassen"].Select("id=" + ras.ID.ToString()).FirstOrDefault();
            if (dr == null)
                return false;
            try
            {
                // we verwijderen het gevonden record
                dr.Delete();
                // indien geslaagd, de list Rassen opnieuw vullen
                vulRassen();
                return true;
            }
            catch (Exception fout)
            {
                return false;
            }
        }
        public void SaveData()
        {
            string XMLMap = Directory.GetCurrentDirectory() + "/XMLBestanden";
            string XMLBestand = Directory.GetCurrentDirectory() + "/XMLBestanden/rassen.xml";
            if (!Directory.Exists(XMLMap))
                Directory.CreateDirectory(XMLMap);
            // indien het xml bestand bestaat, wissen we het : we gaan 
            // immers een nieuwe maken.
            if (File.Exists(XMLBestand))
            {
                File.Delete(XMLBestand);
            }
            // dataset wordt weggeschreven naar xml file
            DS.WriteXml(XMLBestand, XmlWriteMode.WriteSchema);
        }
        #endregion

    }
}
