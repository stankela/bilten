using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja2 : UcesnikFinala
    {
        private GimnasticarUcesnik _gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return _gimnasticar; }
            set { _gimnasticar = value; }
        }

        public virtual Nullable<int> TakmicarskiBroj
        {
            get
            {
                if (Gimnasticar != null && Gimnasticar.TakmicarskiBroj.HasValue)
                    return Gimnasticar.TakmicarskiBroj;
                else
                    return null;
            }
        }

        public virtual string PrezimeIme
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.PrezimeIme;
                else
                    return String.Empty;
            }
        }

        public virtual string KlubDrzava
        {
            get
            {
                if (Gimnasticar != null)
                    return Gimnasticar.KlubDrzava;
                else
                    return String.Empty;
            }
        }

        public UcesnikTakmicenja2()
        { 
        
        }

        public UcesnikTakmicenja2(GimnasticarUcesnik gimnasticar, Nullable<short> qualOrder,
            Nullable<float> qualScore, Nullable<short> qualRank, KvalifikacioniStatus kvalStatus) 
            : base(qualOrder, qualScore, qualRank, kvalStatus)
        {
            _gimnasticar = gimnasticar;
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
        }
    }
}
