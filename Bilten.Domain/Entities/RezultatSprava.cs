using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class RezultatSprava : Rezultat
    {
        private GimnasticarUcesnik gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return gimnasticar; }
            set { gimnasticar = value; }
        }

        private Nullable<float> d;
        public virtual Nullable<float> D
        {
            get { return d; }
            protected set { d = value; }
        }

        private Nullable<float> e;
        public virtual Nullable<float> E
        {
            get { return e; }
            protected set { e = value; }
        }

        private Nullable<float> penalty;
        public virtual Nullable<float> Penalty
        {
            get { return penalty; }
            protected set { penalty = value; }
        }

        public virtual void setOcena(Ocena o)
        {
            if (o != null)
            {
                D = o.D;
                E = o.E;
                Penalty = o.Penalty;
                Total = o.Total;
            }
            else
            {
                D = null;
                E = null;
                Penalty = null;
                Total = null;
            }
        }

        public virtual void clearOcena(Ocena o)
        {
            D = null;
            E = null;
            Penalty = null;
            Total = null;
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

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(D != null ? D.Value.ToString() : NULL);
            strBuilder.AppendLine(E != null ? E.Value.ToString() : NULL);
            strBuilder.AppendLine(Penalty != null ? Penalty.Value.ToString() : NULL);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;

            line = reader.ReadLine();
            D = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            E = line != NULL ? float.Parse(line) : (float?)null;

            line = reader.ReadLine();
            Penalty = line != NULL ? float.Parse(line) : (float?)null;
        }
    }
}
