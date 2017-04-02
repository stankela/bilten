using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;
using System.IO;

namespace Bilten.Domain
{
    public class StartListaNaSpravi : DomainObject
    {
        private Sprava sprava;
        public virtual Sprava Sprava
        {
            get { return sprava; }
            set { sprava = value; }
        }

        // TODO: Preimenuj grupu u turnus
        private byte grupa;
        public virtual byte Grupa
        {
            get { return grupa; }
            set { grupa = value; }
        }

        private byte rotacija;
        public virtual byte Rotacija
        {
            get { return rotacija; }
            set { rotacija = value; }
        }

        private NacinRotacije nacinRotacije;
        public virtual NacinRotacije NacinRotacije
        {
            get { return nacinRotacije; }
            set { nacinRotacije = value; }
        }

        private IList<NastupNaSpravi> nastupi = new List<NastupNaSpravi>();
        public virtual IList<NastupNaSpravi> Nastupi
        {
            get { return nastupi; }
            set { nastupi = value; }
        }

        public virtual IList<GimnasticarUcesnik> Gimnasticari
        {
            get
            {
                IList<GimnasticarUcesnik> result = new List<GimnasticarUcesnik>();
                foreach (NastupNaSpravi n in Nastupi)
                    result.Add(n.Gimnasticar);
                return result;
            }
        }

        public StartListaNaSpravi()
        { 
        
        }

        public StartListaNaSpravi(Sprava sprava,
            byte grupa, byte rotacija, NacinRotacije nacinRotacije)
        {
            this.sprava = sprava;
            this.grupa = grupa;
            this.rotacija = rotacija;
            this.nacinRotacije = nacinRotacije;
        }

        public virtual void addNastup(NastupNaSpravi nastup)
        {
            // TODO4: Dodaj ovakve provere u svim kolekcijama. Ako se null objekat doda u kolekciju, dump i loadFromDump
            // metodi nece raditi.
            if (nastup != null)
                Nastupi.Add(nastup);
        }

        public virtual void removeNastup(NastupNaSpravi nastup)
        {
            Nastupi.Remove(nastup);
        }

        public virtual bool canAddGimnasticar(GimnasticarUcesnik g)
        {
            return !gimnasticarExists(g);
        }

        public virtual void addGimnasticar(GimnasticarUcesnik g)
        {
            if (gimnasticarExists(g))
                throw new BusinessException(
                    String.Format("Gimnasticar {0} je vec na start listi.", g));

            NastupNaSpravi nastup = new NastupNaSpravi(g, 0);
            addNastup(nastup);
        }

        private bool gimnasticarExists(GimnasticarUcesnik g)
        {
            foreach (GimnasticarUcesnik g2 in Gimnasticari)
            {
                if (g2.Id == g.Id)
                    return true;
            }
            return false;
        }

        public virtual bool empty()
        {
            return Nastupi.Count == 0;
        }


        public virtual void clear()
        {
            while (!empty())
                removeNastup(Nastupi[0]);
        }

        public virtual bool moveNastupUp(NastupNaSpravi nastup)
        {
            int index = Nastupi.IndexOf(nastup);
            if (index < 1)
                return false;

            Nastupi.Remove(nastup);
            Nastupi.Insert(index - 1, nastup);
            return true;
        }

        public virtual bool moveNastupDown(NastupNaSpravi nastup)
        {
            int index = Nastupi.IndexOf(nastup);
            if (index < 0 || index == Nastupi.Count - 1)
                return false;

            Nastupi.Remove(nastup);
            Nastupi.Insert(index + 1, nastup);
            return true;
        }

        public virtual void dump(StringBuilder strBuilder)
        {
            // TODO4: Prebaci sve ovakve pozive u DomainObject, a ovde pozivaj base.dump(). Takodje, sve loadFromDump
            // metode pocinji sa base.loadFromDump.
            strBuilder.AppendLine(Id.ToString());

            strBuilder.AppendLine(Sprava.ToString());
            strBuilder.AppendLine(Grupa.ToString());
            strBuilder.AppendLine(Rotacija.ToString());
            strBuilder.AppendLine(NacinRotacije.ToString());

            strBuilder.AppendLine(Nastupi.Count.ToString());
            foreach (NastupNaSpravi n in Nastupi)
                n.dump(strBuilder);
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
            Grupa = byte.Parse(reader.ReadLine());
            Rotacija = byte.Parse(reader.ReadLine());
            NacinRotacije = (NacinRotacije)Enum.Parse(typeof(NacinRotacije), reader.ReadLine());

            int count = int.Parse(reader.ReadLine());
            for (int i = 0; i < count; ++i)
            {
                reader.ReadLine();  // id
                NastupNaSpravi n = new NastupNaSpravi();
                n.loadFromDump(reader, map);
                Nastupi.Add(n);
            }
        }
    }
}
