using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Bilten.Exceptions;
using System.Collections;
using System.Diagnostics;

namespace Bilten.Domain
{
    public class Opcije : DomainObject
    {
        protected static Opcije instance = null;

        // TODO4: Ovo bi trebalo da bude singleton.

        // can throw InfrastructureException
        public static Opcije Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public Opcije()
        { 
            _header1Font = "Garamond";
            _header1FontSize = 15;
            _header1FontBold = true;

            _header2Font = "Garamond";
            _header2FontSize = 14;
            _header2FontBold = true;

            _header3Font = "Times New Roman";
            _header3FontSize = 12;
            _header3FontBold = true;
            _header3FontItalic = true;

            _header4Font = "Times New Roman";
            _header4FontSize = 12;
            _header4FontBold = true;
            _header4FontItalic = true;

            _footerFont = "Arial";
            _footerFontSize = 8;

            _tekstFont = "Arial";
            _tekstFontSize = 8;
        }

        private byte brojDecimalaD = 3;
        public virtual byte BrojDecimalaD
        {
            get { return brojDecimalaD; }
            set { brojDecimalaD = value; }
        }

        private byte brojDecimalaE1 = 2;
        public virtual byte BrojDecimalaE1
        {
            get { return brojDecimalaE1; }
            set { brojDecimalaE1 = value; }
        }

        private byte brojDecimalaE = 3;
        public virtual byte BrojDecimalaE
        {
            get { return brojDecimalaE; }
            set { brojDecimalaE = value; }
        }

        private byte brojDecimalaBon = 1;
        public virtual byte BrojDecimalaBon
        {
            get { return brojDecimalaBon; }
            set { brojDecimalaBon = value; }
        }

        private byte brojDecimalaPen = 1;
        public virtual byte BrojDecimalaPen
        {
            get { return brojDecimalaPen; }
            set { brojDecimalaPen = value; }
        }

        private byte brojDecimalaTotal = 3;
        public virtual byte BrojDecimalaTotal
        {
            get { return brojDecimalaTotal; }
            set { brojDecimalaTotal = value; }
        }

        private bool uzimajPrvuSlobodnuRezervu = false;
        public virtual bool UzimajPrvuSlobodnuRezervu
        {
            get { return uzimajPrvuSlobodnuRezervu; }
            set { uzimajPrvuSlobodnuRezervu = value; }
        }

        private string _header1Text;
        public virtual string Header1Text
        {
            get { return _header1Text; }
            set { _header1Text = value; }
        }

        private string _header2Text;
        public virtual string Header2Text
        {
            get { return _header2Text; }
            set { _header2Text = value; }
        }

        private string _header3Text;
        public virtual string Header3Text
        {
            get { return _header3Text; }
            set { _header3Text = value; }
        }

        private string _header4Text;
        public virtual string Header4Text
        {
            get { return _header4Text; }
            set { _header4Text = value; }
        }

        private string _footerText;
        public virtual string FooterText
        {
            get { return _footerText; }
            set { _footerText = value; }
        }

        private string _header1Font;
        public virtual string Header1Font
        {
            get { return _header1Font; }
            set { _header1Font = value; }
        }

        private string _header2Font;
        public virtual string Header2Font
        {
            get { return _header2Font; }
            set { _header2Font = value; }
        }

        private string _header3Font;
        public virtual string Header3Font
        {
            get { return _header3Font; }
            set { _header3Font = value; }
        }

        private string _header4Font;
        public virtual string Header4Font
        {
            get { return _header4Font; }
            set { _header4Font = value; }
        }

        private string _footerFont;
        public virtual string FooterFont
        {
            get { return _footerFont; }
            set { _footerFont = value; }
        }

        private string _tekstFont;
        public virtual string TekstFont
        {
            get { return _tekstFont; }
            set { _tekstFont = value; }
        }

        private int _header1FontSize;
        public virtual int Header1FontSize
        {
            get { return _header1FontSize; }
            set { _header1FontSize = value; }
        }

        private int _header2FontSize;
        public virtual int Header2FontSize
        {
            get { return _header2FontSize; }
            set { _header2FontSize = value; }
        }

        private int _header3FontSize;
        public virtual int Header3FontSize
        {
            get { return _header3FontSize; }
            set { _header3FontSize = value; }
        }

        private int _header4FontSize;
        public virtual int Header4FontSize
        {
            get { return _header4FontSize; }
            set { _header4FontSize = value; }
        }

        private int _footerFontSize;
        public virtual int FooterFontSize
        {
            get { return _footerFontSize; }
            set { _footerFontSize = value; }
        }

        private int _tekstFontSize;
        public virtual int TekstFontSize
        {
            get { return _tekstFontSize; }
            set { _tekstFontSize = value; }
        }


        private int _takmicariKategorijeFontSize = 10;
        public virtual int TakmicariKategorijeFontSize
        {
            get { return _takmicariKategorijeFontSize; }
            set { _takmicariKategorijeFontSize = value; }
        }

        private int _sudijskiFormularFontSize = 10;
        public virtual int SudijskiFormularFontSize
        {
            get { return _sudijskiFormularFontSize; }
            set { _sudijskiFormularFontSize = value; }
        }

        private bool _header1FontBold;
        public virtual bool Header1FontBold
        {
            get { return _header1FontBold; }
            set { _header1FontBold = value; }
        }

        private bool _header2FontBold;
        public virtual bool Header2FontBold
        {
            get { return _header2FontBold; }
            set { _header2FontBold = value; }
        }

        private bool _header3FontBold;
        public virtual bool Header3FontBold
        {
            get { return _header3FontBold; }
            set { _header3FontBold = value; }
        }

        private bool _header4FontBold;
        public virtual bool Header4FontBold
        {
            get { return _header4FontBold; }
            set { _header4FontBold = value; }
        }

        private bool _footerFontBold;
        public virtual bool FooterFontBold
        {
            get { return _footerFontBold; }
            set { _footerFontBold = value; }
        }

        private bool _header1FontItalic;
        public virtual bool Header1FontItalic
        {
            get { return _header1FontItalic; }
            set { _header1FontItalic = value; }
        }

        private bool _header2FontItalic;
        public virtual bool Header2FontItalic
        {
            get { return _header2FontItalic; }
            set { _header2FontItalic = value; }
        }

        private bool _header3FontItalic;
        public virtual bool Header3FontItalic
        {
            get { return _header3FontItalic; }
            set { _header3FontItalic = value; }
        }

        private bool _header4FontItalic;
        public virtual bool Header4FontItalic
        {
            get { return _header4FontItalic; }
            set { _header4FontItalic = value; }
        }

        private bool _footerFontItalic;
        public virtual bool FooterFontItalic
        {
            get { return _footerFontItalic; }
            set { _footerFontItalic = value; }
        }

        private bool _headerFooterInitialized;
        public virtual bool HeaderFooterInitialized
        {
            get { return _headerFooterInitialized; }
            set { _headerFooterInitialized = value; }
        }

        private bool _prikaziDEOcene = false;
        public virtual bool PrikaziDEOcene
        {
            get { return _prikaziDEOcene; }
            set { _prikaziDEOcene = value; }
        }

        private bool _prikaziPenalizacijuViseboj = false;
        public virtual bool PrikaziPenalizacijuViseboj
        {
            get { return _prikaziPenalizacijuViseboj; }
            set { _prikaziPenalizacijuViseboj = value; }
        }

        private bool _prikaziClanoveEkipe = true;
        public virtual bool PrikaziClanoveEkipe
        {
            get { return _prikaziClanoveEkipe; }
            set { _prikaziClanoveEkipe = value; }
        }

        private bool _stampajPojedinacneEOcene = false;
        public virtual bool StampajPojedinacneEOcene
        {
            get { return _stampajPojedinacneEOcene; }
            set { _stampajPojedinacneEOcene = value; }
        }

        private bool _stampajSveSpraveTak1 = true;
        public virtual bool StampajSveSpraveTak1
        {
            get { return _stampajSveSpraveTak1; }
            set { _stampajSveSpraveTak1 = value; }
        }

        private bool _stampajSveSpraveTak3 = true;
        public virtual bool StampajSveSpraveTak3
        {
            get { return _stampajSveSpraveTak3; }
            set { _stampajSveSpraveTak3 = value; }
        }

        public const int INITIAL_BROJ_SPRAVA_PO_STRANI_TAK1 = 1;
        public const int INITIAL_BROJ_SPRAVA_PO_STRANI_TAK3 = 3;

        private int _brojSpravaPoStraniTak1 = INITIAL_BROJ_SPRAVA_PO_STRANI_TAK1;
        public virtual int BrojSpravaPoStraniTak1
        {
            get { return _brojSpravaPoStraniTak1; }
            set { _brojSpravaPoStraniTak1 = value; }
        }

        private int _brojSpravaPoStraniTak3 = INITIAL_BROJ_SPRAVA_PO_STRANI_TAK3;
        public virtual int BrojSpravaPoStraniTak3
        {
            get { return _brojSpravaPoStraniTak3; }
            set { _brojSpravaPoStraniTak3 = value; }
        }

        private bool _prikaziPenalSprave = true;
        public virtual bool PrikaziPenalSprave
        {
            get { return _prikaziPenalSprave; }
            set { _prikaziPenalSprave = value; }
        }

        private bool _prikaziBonus = true;
        public virtual bool PrikaziBonus
        {
            get { return _prikaziBonus; }
            set { _prikaziBonus = value; }
        }

        private bool _stampajRedniBrojNaStartListi = false;
        public virtual bool StampajRedniBrojNaStartListi
        {
            get { return _stampajRedniBrojNaStartListi; }
            set { _stampajRedniBrojNaStartListi = value; }
        }

        private bool _stampajKategoriju = true;
        public virtual bool StampajKategoriju
        {
            get { return _stampajKategoriju; }
            set { _stampajKategoriju = value; }
        }

        private bool _stampajKlub = false;
        public virtual bool StampajKlub
        {
            get { return _stampajKlub; }
            set { _stampajKlub = value; }
        }

        private int _brojEOcenaFormular;
        public virtual int BrojEOcenaFormular
        {
            get { return _brojEOcenaFormular; }
            set { _brojEOcenaFormular = value; }
        }

        private string _printerName;
        public virtual string PrinterName
        {
            get { return _printerName; }
            set { _printerName = value; }
        }

        private bool _unosOcenaBezIzrZaCeloTak = false;
        public virtual bool UnosOcenaBezIzrZaCeloTak
        {
            get { return _unosOcenaBezIzrZaCeloTak; }
            set { _unosOcenaBezIzrZaCeloTak = value; }
        }

        private string _jezik = "Srpski";
        public virtual string Jezik
        {
            get { return _jezik; }
            set { _jezik = value; }
        }

        private string redBrojString = "RB";  // "No."
        public virtual string RedBrojString
        {
            get { return redBrojString; }
            set { redBrojString = value; }
        }

        private string brojString = "Broj";  // "Bib"
        public virtual string BrojString
        {
            get { return brojString; }
            set { brojString = value; }
        }

        private string rankString = "Rank";  // "Rank"
        public virtual string RankString
        {
            get { return rankString; }
            set { rankString = value; }
        }

        private string imeString = "Ime";  // "Name"
        public virtual string ImeString
        {
            get { return imeString; }
            set { imeString = value; }
        }

        private string prezimeString = "Prezime";  // "Surname"
        public virtual string PrezimeString
        {
            get { return prezimeString; }
            set { prezimeString = value; }
        }

        private string datumRodjenjaString = "Datum rodjenja";  // "Birth date"
        public virtual string DatumRodjenjaString
        {
            get { return datumRodjenjaString; }
            set { datumRodjenjaString = value; }
        }

        private string klubString = "Klub";  // "Club"
        public virtual string KlubString
        {
            get { return klubString; }
            set { klubString = value; }
        }

        private string drzavaString = "Drzava";  // "National team"
        public virtual string DrzavaString
        {
            get { return drzavaString; }
            set { drzavaString = value; }
        }

        private string ekipaString = "Ekipa";  // "Team"
        public virtual string EkipaString
        {
            get { return ekipaString; }
            set { ekipaString = value; }
        }

        private string klubDrzavaString = "Klub";  // "Club / NT"
        public virtual string KlubDrzavaString
        {
            get { return klubDrzavaString; }
            set { klubDrzavaString = value; }
        }

        private string kategorijaString = "Kategorija";  // "Category"
        public virtual string KategorijaString
        {
            get { return kategorijaString; }
            set { kategorijaString = value; }
        }

        private string totalString = "Ukupno";  // "Total"
        public virtual string TotalString
        {
            get { return totalString; }
            set { totalString = value; }
        }

        // TODO5: D, E, Bonus i Pen. nisu u jeziku i nemoguce ih je menjati. Razmisli da li je potrebno

        private string dString = "D";
        public virtual string DString
        {
            get { return dString; }
            set { dString = value; }
        }

        private string eString = "E";
        public virtual string EString
        {
            get { return eString; }
            set { eString = value; }
        }

        private string bonusString = "Bonus";
        public virtual string BonusString
        {
            get { return bonusString; }
            set { bonusString = value; }
        }

        private string penaltyString = "Pen.";
        public virtual string PenaltyString
        {
            get { return penaltyString; }
            set { penaltyString = value; }
        }

        private string ocenaString = "Ocena";  // "Score"
        public virtual string OcenaString
        {
            get { return ocenaString; }
            set { ocenaString = value; }
        }

        private string rezerveString = "REZERVE";  // "RESERVES"
        public virtual string RezerveString
        {
            get { return rezerveString; }
            set { rezerveString = value; }
        }

        private string stranaString = "Strana";  // "Page"
        public virtual string StranaString
        {
            get { return stranaString; }
            set { stranaString = value; }
        }

        private string vrhovniSudijaMuskiString = "Vrhovni sudija";  // "Superior Jury"
        public virtual string VrhovniSudijaMuskiString
        {
            get { return vrhovniSudijaMuskiString; }
            set { vrhovniSudijaMuskiString = value; }
        }

        private string vrhovniSudijaZenskiString = "Vrhovna sutkinja";  // "Superior Jury"
        public virtual string VrhovniSudijaZenskiString
        {
            get { return vrhovniSudijaZenskiString; }
            set { vrhovniSudijaZenskiString = value; }
        }

        private string viseboj = "Višeboj";
        public virtual string Viseboj
        {
            get { return viseboj; }
            set { viseboj = value; }
        }

        private string kvalSprave = "Kvalifikacije za finale po spravama";
        public virtual string KvalSprave
        {
            get { return kvalSprave; }
            set { kvalSprave = value; }
        }

        private string spraveFinalisti = "Finalisti po spravama";
        public virtual string SpraveFinalisti
        {
            get { return spraveFinalisti; }
            set { spraveFinalisti = value; }
        }

        private string finaleSprave = "Finale po spravama";
        public virtual string FinaleSprave
        {
            get { return finaleSprave; }
            set { finaleSprave = value; }
        }

        private string ekipeRezultati = "Rezultati ekipno";
        public virtual string EkipeRezultati
        {
            get { return ekipeRezultati; }
            set { ekipeRezultati = value; }
        }

        private string kvalStartListe = "Start liste - kvalifikacije";
        public virtual string KvalStartListe
        {
            get { return kvalStartListe; }
            set { kvalStartListe = value; }
        }

        private string finaleStartListe = "Start liste - finale po spravama";
        public virtual string FinaleStartListe
        {
            get { return finaleStartListe; }
            set { finaleStartListe = value; }
        }

        private string rasporedSudija = "Raspored sudija";
        public virtual string RasporedSudija
        {
            get { return rasporedSudija; }
            set { rasporedSudija = value; }
        }

        private string rotacija = "Rotacija";
        public virtual string Rotacija
        {
            get { return rotacija; }
            set { rotacija = value; }
        }
        
        public virtual void UpdateJezik(Jezik jezik)
        {
            Jezik = jezik.Naziv;
            RedBrojString = jezik.RedBroj;
            RankString = jezik.Rank;
            ImeString = jezik.Ime;
            KlubDrzavaString = jezik.KlubDrzava;
            KategorijaString = jezik.Kategorija;
            TotalString = jezik.Ukupno;
            OcenaString = jezik.Ocena;
            RezerveString = jezik.Rezerve;

            BrojString = jezik.Broj;
            PrezimeString = jezik.Prezime;
            DatumRodjenjaString = jezik.DatumRodjenja;
            KlubString = jezik.Klub;
            DrzavaString = jezik.Drzava;
            EkipaString = jezik.Ekipa;
            StranaString = jezik.Strana;
            VrhovniSudijaMuskiString = jezik.VrhovniSudijaMuski;
            VrhovniSudijaZenskiString = jezik.VrhovniSudijaZenski;

            Viseboj = jezik.Viseboj;
            KvalSprave = jezik.KvalSprave;
            SpraveFinalisti = jezik.SpraveFinalisti;
            FinaleSprave = jezik.FinaleSprave;
            EkipeRezultati = jezik.EkipeRezultati;
            KvalStartListe = jezik.KvalStartListe;
            FinaleStartListe = jezik.FinaleStartListe;
            RasporedSudija = jezik.RasporedSudija;
            Rotacija = jezik.Rotacija;
        }
    }
}
