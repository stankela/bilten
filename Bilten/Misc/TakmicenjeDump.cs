using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Misc
{
    public class TakmicenjeDump
    {
        private int takmicenjeId;

        private string takmicenjeDump;
        private string kluboviDump;
        private string drzaveDump;
        private string gimnasticariDump;
        private string oceneDump;
        private string rasporediNastupaDump;
        private string rezTakmicenjaDump;

        private StringBuilder strBuilder;

        public TakmicenjeDump()
        {
            takmicenjeId = -1;
            strBuilder = new StringBuilder();
        }

        public void dumpTakmicenje(int takmicenjeId)
        {
            this.takmicenjeId = takmicenjeId;

            dumpTakmicenje();
            dumpKlubovi();
            dumpDrzave();
            dumpGimnasticari();
            dumpOcene();
            dumpRasporediNastupa();
            dumpRezTakmicenja();
        }

        private void dumpTakmicenje()
        {
            strBuilder.Clear();
            Takmicenje takmicenje = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO().FindById(takmicenjeId);
            takmicenje.dump(strBuilder);
            takmicenjeDump = strBuilder.ToString();
        }

        private void dumpKlubovi()
        {
            strBuilder.Clear();
            IList<KlubUcesnik> klubovi = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(klubovi.Count.ToString());
            foreach (KlubUcesnik k in klubovi)
                k.dump(strBuilder);
            kluboviDump = strBuilder.ToString();
        }

        private void dumpDrzave()
        {
            strBuilder.Clear();
            IList<DrzavaUcesnik> drzave = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(drzave.Count.ToString());
            foreach (DrzavaUcesnik d in drzave)
                d.dump(strBuilder);
            drzaveDump = strBuilder.ToString();
        }

        private void dumpGimnasticari()
        {
            strBuilder.Clear();
            IList<GimnasticarUcesnik> gimnasticari = DAOFactoryFactory.DAOFactory.GetGimnasticarUcesnikDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(gimnasticari.Count.ToString());
            foreach (GimnasticarUcesnik g in gimnasticari)
                g.dump(strBuilder);
            gimnasticariDump = strBuilder.ToString();
        }

        private void dumpOcene()
        {
            strBuilder.Clear();
            IList<Ocena> ocene = DAOFactoryFactory.DAOFactory.GetOcenaDAO().FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(ocene.Count.ToString());
            foreach (Ocena o in ocene)
                o.dump(strBuilder);
            oceneDump = strBuilder.ToString();
        }

        private void dumpRasporediNastupa()
        {
            strBuilder.Clear();
            IList<RasporedNastupa> rasporediNastupa = DAOFactoryFactory.DAOFactory.GetRasporedNastupaDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rasporediNastupa.Count.ToString());
            foreach (RasporedNastupa r in rasporediNastupa)
                r.dump(strBuilder);
            rasporediNastupaDump = strBuilder.ToString();
        }

        private void dumpRezTakmicenja()
        {
            strBuilder.Clear();
            IList<RezultatskoTakmicenje> rezTakmicenja = DAOFactoryFactory.DAOFactory.GetRezultatskoTakmicenjeDAO()
                .FindByTakmicenje(takmicenjeId);
            strBuilder.AppendLine(rezTakmicenja.Count.ToString());
            foreach (RezultatskoTakmicenje rt in rezTakmicenja)
                rt.dump(strBuilder);
            rezTakmicenjaDump = strBuilder.ToString();
        }

        public string getTakmicenjeDump()
        {
            return takmicenjeId != -1 ? takmicenjeDump : String.Empty;
        }

        public string getKluboviDump()
        {
            return takmicenjeId != -1 ? kluboviDump : String.Empty;
        }

        public string getDrzaveDump()
        {
            return takmicenjeId != -1 ? drzaveDump : String.Empty;
        }

        public string getGimnasticariDump()
        {
            return takmicenjeId != -1 ? gimnasticariDump : String.Empty;
        }

        public string getOceneDump()
        {
            return takmicenjeId != -1 ? oceneDump : String.Empty;
        }

        public string getRasporediNastupaDump()
        {
            return takmicenjeId != -1 ? rasporediNastupaDump : String.Empty;
        }

        public string getRezTakmicenjaDump()
        {
            return takmicenjeId != -1 ? rezTakmicenjaDump : String.Empty;
        }
    }
}