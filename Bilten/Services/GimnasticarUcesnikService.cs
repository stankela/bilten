using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Exceptions;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bilten.Services
{
    public class GimnasticarUcesnikService
    {
        public static GimnasticarUcesnik createGimnasticarUcesnik(GimnasticarUcesnik g, TakmicarskaKategorija kategorija)
        {
            string nazivDrzave = null;
            string kodDrzave = null;
            if (g.DrzavaUcesnik != null)
            {
                nazivDrzave = g.DrzavaUcesnik.Naziv;
                kodDrzave = g.DrzavaUcesnik.Kod;
            }
            string nazivKluba = null;
            string kodKluba = null;
            if (g.KlubUcesnik != null)
            {
                nazivKluba = g.KlubUcesnik.Naziv;
                kodKluba = g.KlubUcesnik.Kod;
            }
            return createGimnasticarUcesnik(g.Ime, g.SrednjeIme, g.Prezime, g.DatumRodjenja, kategorija, nazivDrzave,
                kodDrzave, nazivKluba, kodKluba);
        }

        public static GimnasticarUcesnik createGimnasticarUcesnik(Gimnasticar g, TakmicarskaKategorija kategorija)
        {
            string nazivDrzave = null;
            string kodDrzave = null;
            if (g.Drzava != null)
            {
                nazivDrzave = g.Drzava.Naziv;
                kodDrzave = g.Drzava.Kod;
            }
            string nazivKluba = null;
            string kodKluba = null;
            if (g.Klub != null)
            {
                nazivKluba = g.Klub.Naziv;
                kodKluba = g.Klub.Kod;
            }
            return createGimnasticarUcesnik(g.Ime, g.SrednjeIme, g.Prezime, g.DatumRodjenja, kategorija, nazivDrzave,
                kodDrzave, nazivKluba, kodKluba);
        }

        private static GimnasticarUcesnik createGimnasticarUcesnik(string ime, string srednjeIme, string prezime,
            Datum datumRodjenja, TakmicarskaKategorija kategorija, string nazivDrzave, string kodDrzave, string nazivKluba,
            string kodKluba)
        {
            GimnasticarUcesnik result = new GimnasticarUcesnik();
            result.Ime = ime;
            result.SrednjeIme = srednjeIme;
            result.Prezime = prezime;
            result.DatumRodjenja = datumRodjenja;
            result.TakmicarskaKategorija = kategorija;
            if (String.IsNullOrEmpty(nazivDrzave))
                result.DrzavaUcesnik = null;
            else
            {
                DrzavaUcesnikDAO drzavaUcesnikDAO = DAOFactoryFactory.DAOFactory.GetDrzavaUcesnikDAO();
                DrzavaUcesnik drzavaUcesnik = drzavaUcesnikDAO.FindDrzavaUcesnik(kategorija.Takmicenje.Id, nazivDrzave);
                if (drzavaUcesnik == null)
                {
                    drzavaUcesnik = new DrzavaUcesnik();
                    drzavaUcesnik.Naziv = nazivDrzave;
                    drzavaUcesnik.Kod = kodDrzave;
                    drzavaUcesnik.Takmicenje = kategorija.Takmicenje;
                    drzavaUcesnikDAO.Add(drzavaUcesnik);
                }
                result.DrzavaUcesnik = drzavaUcesnik;
            }
            if (String.IsNullOrEmpty(nazivKluba))
                result.KlubUcesnik = null;
            else
            {
                KlubUcesnikDAO klubUcesnikDAO = DAOFactoryFactory.DAOFactory.GetKlubUcesnikDAO();
                KlubUcesnik klubUcesnik = klubUcesnikDAO.FindKlubUcesnik(kategorija.Takmicenje.Id, nazivKluba);
                if (klubUcesnik == null)
                {
                    klubUcesnik = new KlubUcesnik();
                    klubUcesnik.Naziv = nazivKluba;
                    klubUcesnik.Kod = kodKluba;
                    klubUcesnik.Takmicenje = kategorija.Takmicenje;
                    klubUcesnikDAO.Add(klubUcesnik);
                }
                result.KlubUcesnik = klubUcesnik;
            }
            return result;
        }
    }
}
