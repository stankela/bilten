using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bilten.Domain
{
    public class UcesnikTakmicenja3 : UcesnikFinala
    {
        private GimnasticarUcesnik _gimnasticar;
        public virtual GimnasticarUcesnik Gimnasticar
        {
            get { return _gimnasticar; }
            set { _gimnasticar = value; }
        }

        private Sprava _sprava;
        public virtual Sprava Sprava
        {
            get { return _sprava; }
            set { _sprava = value; }
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

        public UcesnikTakmicenja3()
        { 
        
        }
   
        public UcesnikTakmicenja3(GimnasticarUcesnik gimnasticar, Sprava sprava,
            Nullable<short> qualOrder, Nullable<float> qualScore,
            Nullable<short> qualRank, KvalifikacioniStatus kvalStatus) 
            : base(qualOrder, qualScore, qualRank, kvalStatus)
        {
            _gimnasticar = gimnasticar;
            _sprava = sprava;
        }

        public override void dump(StringBuilder strBuilder)
        {
            base.dump(strBuilder);
            strBuilder.AppendLine(Gimnasticar != null ? Gimnasticar.Id.ToString() : NULL);
            strBuilder.AppendLine(Sprava.ToString());
        }

        public virtual void loadFromDump(StringReader reader, IdMap map)
        {
            base.loadFromDump(reader);

            string line = reader.ReadLine();
            Gimnasticar = line != NULL ? map.gimnasticariMap[int.Parse(line)] : null;
            Sprava = (Sprava)Enum.Parse(typeof(Sprava), reader.ReadLine());
        }
    }
}
