using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Domain
{
    public class IdMap
    {
        public IDictionary<int, Takmicenje> takmicenjeMap = new Dictionary<int, Takmicenje>();
        public IDictionary<int, TakmicarskaKategorija> kategorijeMap = new Dictionary<int, TakmicarskaKategorija>();
        public IDictionary<int, RezultatskoTakmicenjeDescription> descriptionsMap
            = new Dictionary<int, RezultatskoTakmicenjeDescription>();
        public IDictionary<int, KlubUcesnik> kluboviMap = new Dictionary<int, KlubUcesnik>();
        public IDictionary<int, DrzavaUcesnik> drzaveMap = new Dictionary<int, DrzavaUcesnik>();
        public IDictionary<int, GimnasticarUcesnik> gimnasticariMap = new Dictionary<int, GimnasticarUcesnik>();
        public IDictionary<int, Ekipa> ekipeMap = new Dictionary<int, Ekipa>();
    }
}
