using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Exceptions;

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
    }
}
