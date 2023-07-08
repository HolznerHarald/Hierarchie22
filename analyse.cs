/*test*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Hierarchie22
{
    internal class analyse
    {
        int rekzae =0;
        public string actFilename;
        public Methode actMeth;
        public string actClassname;
        List<string> AllLines = new List<string>();
        public List<string> allLines2 = new List<string>();
        MainWindow WM;
        public List<string> FileKurz = new List<string>();
        public List<string> TestAusgabe = new List<string>();
        public List<Klasse> Klassen = new List<Klasse>();
        public List<Methode> Meths = new List<Methode>();
        List<Tuple<string, string, string>> Filetuple = new List<Tuple<string, string, string>>();

        public analyse()
        {
           WM = Application.Current.MainWindow as MainWindow;  //!!!Highligh
        }
        internal void analyseMain()
        {
            Einlesen();// AllLines mit XXX NeuesFile:
            ausgeben(AllLines, WM.Ausgabe2);
            KommentareEntfernen2();
            ausgeben(AllLines, WM.Ausgabe3);
            analyse1(WM.NurName);// allLines2 mit XXX, FileKurz, Filetuple(3 Strings)
            ausgeben(FileKurz, WM.Ausgabe1);
            ausgeben(allLines2, WM.Ausgabe4);
            ErstelleKlassen();// aus Filetuple Klass (Objektliste)
            TestKlassen();// Aus Klass erstellen TestAusgabe(Klassen mit Methoden)
            //ausgeben(TestAusgabe, WM.Ausgabe3);
            ErstelleMethListe();
            analyse2();
            KlassenTree();            
        }

        private void KlassenTree()
        {
            MenuItem root = new MenuItem() { MTitle =" Methodenaufrufe" };
            root.Klass = "noclass";
            root.IsExpanded = true;
            rekursivMenu(root, Meths);            
            WM.trvMenu.Items.Add(root);   
        }
        
        private void rekursivMenu(MenuItem hroot, List<Methode> methList)
        {
          rekzae++;
          foreach(var hmeth in methList)
            {
                hroot.IsExpanded = true;
                MenuItem hchildItem = new MenuItem() { MTitle = hmeth.name };
                hchildItem.Klass = hmeth.KlassenName;
                hroot.Items.Add(hchildItem);
                if (rekzae<=10)
                    rekursivMenu(hchildItem, hmeth.MethodenAufrufe);
            }
          rekzae--;
        }

        private bool Einlesen()
        {
            string hname;

            for (int ii = 0; ii < WM.Dateiliste.Items.Count; ii++)   // Einlesen der Files in 
            {
                hname = WM.Dateiliste.Items[ii].ToString();
                AllLines.Add("XXX NeuesFile:" + hname);
                AllLines.Add("*****************************************");
                using (StreamReader sr = File.OpenText(hname))
                {
                    while (sr.Peek() >= 0)
                    {
                        AllLines.Add(sr.ReadLine());
                    }
                }
            }
            return true;
        }

        private void KommentareEntfernen2()
        {            
            bool end = false;
            String JAllLines = String.Join("\n", AllLines.ToArray());  //!!!Highlight
            while (!end)
            {
                int gk = -1;
                int zk = -1;

                do
                {
                    zk = JAllLines.IndexOf("//", zk + 1);
                }
                while (zk > 0 && JAllLines[zk - 1] == '"');

                do
                {
                    gk = JAllLines.IndexOf("/*", gk+1);
                }

                while (gk > 0 && JAllLines[gk - 1] == '"');

                if (zk == -1 && gk == -1)
                    break;
                else if ((gk == -1) ||
                    (gk>-1 && zk>-1 && zk < gk ))
                {
                    int nind = JAllLines.IndexOf('\n', zk);
                    if (nind == -1)
                        JAllLines = JAllLines.Remove(zk);
                    else
                        JAllLines = JAllLines.Remove(zk, nind -zk);
                }
                else
                {
                    int zuk = JAllLines.IndexOf("*/", gk);
                    if (zuk == -1)
                        JAllLines = JAllLines.Remove(gk);
                    else
                        JAllLines = JAllLines.Remove(gk, zuk-gk +2);
                }
            }

            String[] SArr = JAllLines.Split('\n');
            AllLines=SArr.ToList<String>();
        }     

        public void analyse1(Dictionary<string, string> NurName)
        {
            Tuple<string, string, string> hT;
            int KlammerZae = 0;
            for (int ii = 0; ii < AllLines.Count(); ii++)
            {
                allLines2.Add(AllLines[ii]);
                String hs = AllLines[ii].TrimStart();

                if (hs.IndexOf("NeuesFile:") == 0)
                {
                    FileKurz.Add(hs);
                    actFilename = NurName[hs.Substring(hs.IndexOf(':') + 1)];
                }
                else if (KlammerZae == 1 && (hs.IndexOf("class ") == 0 || hs.IndexOf(" class ") > -1))
                {
                    FileKurz.Add("   " + hs);
                    ExtractClassname(hs);
                    allLines2.Insert(allLines2.Count - 1,"XXX Klasse:" + hs);
                }
                else if (hs.IndexOf("}") == 0)
                    KlammerZae--;
                else if (hs.IndexOf("{") == 0)
                {
                    KlammerZae++;
                    if (KlammerZae == 3)
                    {
                        //Korrektur Property
                        String hs3 = AllLines[ii - 1].TrimStart();
                        if (hs3.IndexOf("(") < 0 &&
                            (hs3.IndexOf("public") == 0 || hs3.IndexOf("private") == 0))
                            continue;
                        
                        int jj = ii - 1;
                        bool gef = false;
                        int ind = FileKurz.Count;
                        while (jj >= 0 && !gef)
                        {
                            FileKurz.Insert(ind, "      " + AllLines[jj].TrimStart());
                            if (AllLines[jj].IndexOf("(") >= 0)
                                gef = true;
                            jj--;
                        }
                        string MethName = ExtractMethName(AllLines[jj + 1]);
                        allLines2.Insert(allLines2.Count-2,"XXX Methode:" + MethName);
                        hT = new Tuple<string, string, string>(MethName, actClassname, actFilename);
                        Filetuple.Add(hT);
                    }
                }
            }
        }

        public void analyse2()
        {
            int KlammerZae = 0;
            for (int ii = 0; ii < allLines2.Count(); ii++)
            {   
                String hs = allLines2[ii].TrimStart();
                if (hs.IndexOf("XXX Klasse:") == 0)
                    actClassname = hs.Substring(11);
                else if (hs.IndexOf("XXX Methode:") == 0)
                {
                    String actMethName = hs.Substring(12);
                    if (!sucheMethode(actMethName))
                       return;                    
                }
                if (hs.IndexOf("}") == 0)
                    KlammerZae--;
                else if (hs.IndexOf("{") == 0)
                    KlammerZae++;

                if (KlammerZae >= 3)
                {
                    string DavorStr = ".; ({<>=&|!";
                    String DanachStr = "(";

                    foreach (var Meth in Meths)
                    {
                        var ind = hs.IndexOf(Meth.name);
                        var classInd = hs.IndexOf("class ");
                        if (classInd >= 0 && classInd < ind)
                            continue;

                        
                        if ((ind == 0 || (ind >= 0 && DavorStr.IndexOf(hs[ind - 1]) >= 0)) 
                           )
                        {
                            String hs2 = hs;
                            while (hs2[ind + Meth.name.Length] == ' ')
                                hs2 = hs2.Remove(ind + Meth.name.Length, 1);
                            if (DanachStr.IndexOf(hs2[ind + Meth.name.Length]) >= 0)
                            {
                                Meth.VonMethoden.Add(actMeth);
                                actMeth.MethodenAufrufe.Add(Meth);
                            }
                        }                            
                    }
                }
            }
        }

        private bool sucheMethode(string actMethName)
        {
            foreach (var hmeth in Meths)
                if (actMethName == hmeth.name)
                {
                    actMeth = hmeth;
                    return true;
                }

            WM.Ausgabe1.Text = " Error Pos1";  // Methode nicht gefunden
            return false;
        }

        private void ErstelleKlassen()
        {
            foreach (var Tup in Filetuple)
            {
                bool gef = false;
                foreach (var Klass in Klassen)
                    if (Tup.Item2 == Klass.name)
                    {
                        gef = true;
                        Klass.AddMethode(Tup);
                    }
                if (!gef)
                    Klassen.Add(new Klasse(Tup));
            }
        }

        private void TestKlassen()
        {
            foreach (var Klass in Klassen)
            {
                TestAusgabe.Add(Klass.name);
                foreach (var Meth in Klass.Methoden)
                    TestAusgabe.Add("     " + Meth.name);
            }            
        }      

        private void ErstelleMethListe()
        {
            foreach (var Klass in Klassen)
                foreach (var Meth in Klass.Methoden)
                    Meths.Add(Meth);
        }

        //******************* Unterprozeduren************************
        public void ausgeben(List<string> fileKurz, System.Windows.Controls.TextBox Aus)
        {
            String hsausgabe = new String("");
            for (int ii = 0; ii < fileKurz.Count; ii++)
                hsausgabe = hsausgabe + fileKurz[ii] + "\n";
            Aus.Text = hsausgabe;
        }
        private string ExtractMethName(string hs)
        {
            int ii = hs.IndexOf("(") - 1;
            while (ii >= 0)
                if (hs[ii] == ' ')
                {
                    ii--;
                    continue;
                }
                else
                    break;
            int LetzZei = ii;
            while (ii >= 0)
                if (hs[ii] != ' ')
                {
                    ii--;
                    continue;
                }
                else
                    break;
            int erstZei = ii + 1;
            return hs.Substring(erstZei, LetzZei - erstZei + 1);
        }

        private void ExtractClassname(string hs)
        {
            int ii;
            for (ii = (hs.IndexOf(" class ") + 7); ii < hs.Count(); ii++)
                if (hs[ii] == ' ')
                    continue;
                else
                    break;
            int ErstesZei = ii;
            for (ii = ErstesZei; ii < hs.Count(); ii++)
                if (hs[ii] != ' ' && hs[ii] != ':')
                    continue;
                else
                    break;
            int LetztesZei = ii;

            actClassname = hs.Substring(ErstesZei, LetztesZei - ErstesZei);
        }
    }
}