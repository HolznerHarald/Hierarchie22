using System;
using System.Collections.Generic;
using System.Text;

namespace Hierarchie22
{
    class Methode
    {
        public string name;
        public string KlassenName;
        public List<Methode> MethodenAufrufe = new List<Methode>();
        public List<Methode> VonMethoden = new List<Methode>();

        public Methode(Tuple<string, string, string> KlassenTup)
        {
            name = KlassenTup.Item1;
            KlassenName = KlassenTup.Item2;
        }
    }
}
