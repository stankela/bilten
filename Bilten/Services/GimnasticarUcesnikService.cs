using Bilten.Dao;
using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class GimnasticarUcesnikService
    {
        // TODO4: Ovaj metod je prekopiran iz klase TakmicariKategorijeForm. Probaj da oba metoda
        // spojis u jedan.
        public static GimnasticarUcesnik createGimnasticarUcesnik(GimnasticarUcesnik g, TakmicarskaKategorija kategorija)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = g.Ime;
            result.SrednjeIme = g.SrednjeIme;
            result.Prezime = g.Prezime;
            result.DatumRodjenja = g.DatumRodjenja;
            result.TakmicarskaKategorija = kategorija;
            if (g.DrzavaUcesnik == null)
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnik drzavaUcesnik = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO()
                    .FindDrzavaUcesnik(kategorija.Takmicenje.Id, g.DrzavaUcesnik.Naziv);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = g.DrzavaUcesnik.Naziv;
                    drzavaUcesnik.Kod = g.DrzavaUcesnik.Kod;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO().Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (g.KlubUcesnik == null)
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnik klubUcesnik = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO()
                    .FindKlubUcesnik(kategorija.Takmicenje.Id, g.KlubUcesnik.Naziv);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = g.KlubUcesnik.Naziv;
                    klubUcesnik.Kod = g.KlubUcesnik.Kod;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO().Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }
    }
}
