using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Zumbach_DataBackup
{
    public partial class Service1 : ServiceBase
    {   //
        //string source=@"\\10.222.103.216\steelmaster_db$";
        //string target=@"\\10.222.103.226\MSR_Sicherungen$";
        string source = @"C:\Users\tino.schmittat\Desktop\Softwareprojekte\Zumbach\Quelle";
        string target = @"C:\Users\tino.schmittat\Desktop\Softwareprojekte\Zumbach\Ziel";
        //Timer Objekt
        Timer t;
        //Intervall Zähler
        int i = 3000;
        //Aktuelles Datum
        DateTime d;
        //Aktuelles Jahr
        string j;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {   
            
            t = new Timer();
            t.Interval = i;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Tick);
            t.Enabled = true;
            Library.WriteLog("Zumbach Service wurde gestartet!");
        }

        protected override void OnStop()
        {
            Library.WriteLog("Zumbach Service wurde gestoppt!");
        }

        private void t_Tick(object sender, ElapsedEventArgs e) {
            Library.WriteLog("Zumbach Service wurde aktiviert!");

            //Fragt aktuelle Zeit ab
            d = DateTime.Now;
            j = d.Year.ToString();
            //Erstellt Ordner
            MakeDateFolder(source);
            MakeDateFolder(target);
            //Kopiert Daten
            string t1 = CopyDate(source, source);
            string t2 = CopyDate(source, target);

            if ((t1 != String.Empty) && (t2 != String.Empty))
            {
                //Erfolgreich kopiert
                //Lösche Originaldatei
                System.IO.File.Delete(t1);
                Library.WriteLog(" " + t1 + "wurde gelöscht");
            }
            else
            {
                Library.WriteLog("Nichts wurde kopiert!");
            }

        }

        private string CopyDate(string s, string t)
        {
            string c_success = String.Empty;
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(source);

            foreach (System.IO.FileInfo f in directory.GetFiles())
            {
                if (f.Name.Contains("Export"))
                {
                    int p_year = 0;
                    bool d_success = false;

                    string p = f.Name.Substring(7, 2);

                    try
                    {
                       d_success = int.TryParse(p, out p_year);
                    }
                    catch (Exception)
                    {
                        Library.WriteLog("Fehler beim parsen von: " + p );
                    }

                    p_year = p_year + 2000;

                    if (d_success)
                    {
                        string p1 = System.IO.Path.Combine(s, f.Name);
                        string p2 = System.IO.Path.Combine(t, p_year.ToString(), f.Name);
                        try
                        {
                            System.IO.File.Copy(p1,p2, false);
                            c_success = p1;
                        }
                        catch (Exception)
                        {
                            Library.WriteLog("Nicht möglich Datei zu schreiben: " + p1 + "  " + p2);
                        }

                    }


                }

            }
            return c_success;
        }


        private void MakeDateFolder(string o) {

            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(o);
            System.IO.DirectoryInfo[] directories = directory.GetDirectories();

            bool isInThere = false;
            List<string> files = new List<string>();

            foreach (System.IO.DirectoryInfo f in directories)
            {
                files.Add(f.Name);
                //Library.WriteLog(f.Name);
            }
            if (directories.Count() < 1 )
            {
                Library.WriteLog("Achtung keine Daten in Ordner!");
                Library.WriteLog(source);
            }

            foreach (string s in files)
            {
                if (s.Contains(j))
                {
                    isInThere = true;
                }
            }
            if (!isInThere)
            {
                CreateFolder(o + @"\" + j);
            }

        }
        private void CreateFolder(string f)
        {
            System.IO.Directory.CreateDirectory(f);
            Library.WriteLog("Der Ordner " + f + " wurde hinzugefügt!");
        }
    }
}
