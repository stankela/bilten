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

        private Nullable<float> e1;
        public virtual Nullable<float> E1
        {
            get { return e1; }
            protected set { e1 = value; }
        }

        private Nullable<float> e2;
        public virtual Nullable<float> E2
        {
            get { return e2; }
            protected set { e2 = value; }
        }

        private Nullable<float> e3;
        public virtual Nullable<float> E3
        {
            get { return e3; }
            protected set { e3 = value; }
        }

        private Nullable<float> e4;
        public virtual Nullable<float> E4
        {
            get { return e4; }
            protected set { e4 = value; }
        }

        private Nullable<float> e5;
        public virtual Nullable<float> E5
        {
            get { return e5; }
            protected set { e5 = value; }
        }

        private Nullable<float> e6;
        public virtual Nullable<float> E6
        {
            get { return e6; }
            protected set { e6 = value; }
        }

        private Nullable<float> bonus = 0.1f;
        public virtual Nullable<float> Bonus
        {
            get { return bonus; }
            protected set { bonus = value; }
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
                E1 = o.E1;
                E2 = o.E2;
                E3 = o.E3;
                E4 = o.E4;
                E5 = o.E5;
                E6 = o.E6;
                Penalty = o.Penalty;
                Total = o.Total;
            }
            else
            {
                D = null;
                E = null;
                E1 = null;
                E2 = null;
                E3 = null;
                E4 = null;
                E5 = null;
                E6 = null;
                Penalty = null;
                Total = null;
            }
        }

        public virtual void clearOcena()
        {
            D = null;
            E = null;
            E1 = null;
            E2 = null;
            E3 = null;
            E4 = null;
            E5 = null;
            E6 = null;
            Penalty = null;
            Total = null;
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

        // TODO5: Da li ovde (i u loadFromDump) treba da se dodaju E1, E2, ...
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
