using System;
using System.Collections.Generic;
using System.Text;

namespace Hierarchie22
{
    class Klasse
    {
        public List<Methode> Methoden = new List<Methode>();
        public string name;
        public Klasse(Tuple<string, string, string> KlassenTup)
        {            
            name = KlassenTup.Item2;
            Methoden.Add(new Methode(KlassenTup));
            
        }

        internal void AddMethode(Tuple<string, string, string> KlassenTup)
        {
            Methoden.Add(new Methode(KlassenTup));
        }
    }
}
