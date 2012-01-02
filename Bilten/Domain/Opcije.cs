using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Bilten.Data;
using Bilten.Exceptions;
using System.Collections;
using Bilten.UI;
using System.Drawing;

namespace Bilten.Domain
{
    public class Opcije : DomainObject
    {
        private bool saved = false;
        public virtual bool Saved
        {
            get { return saved; }
        }

        protected static Opcije instance = null;

        // can throw InfrastructureException
        public static Opcije Instance
        {
            get
            {
                if (instance == null)
                    instance = getInstance();
                return instance;
            }
        }

        private static Opcije getInstance()
        {
            Opcije result = load();
            if (result == null)
            {
                result = new Opcije();
                result.saved = false;
            }
            else
            {
                result.saved = true;
            }
            return result;
        }

        protected Opcije()
        { 
            _header1Font = "Garamond";
            _header1FontSize = 18;
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

        private static Opcije load()
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                // mora ovako zato sto metod dataContext.GetAll<Opcije> trazi da 
                // Opcije imaju public contructor, a to nije moguce jer su Opcije 
                // singleton
                string query = @"from Opcije";
                IList result = dataContext.ExecuteQuery(QueryLanguageType.HQL,
                    query, new string[] { }, new object[] { });
                if (result.Count > 0)
                    return (Opcije)result[0];
                else
                    return null;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
        }

        public virtual void save()
        {
            IDataContext dataContext = null;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                if (saved)
                    dataContext.Save(this);
                else
                    dataContext.Add(this);
                dataContext.Commit();
                saved = true;
            }
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                throw new InfrastructureException(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), ex);
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;
            }
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

        private const int INITIAL_BROJ_SPRAVA_PO_STRANI_TAK1 = 1;
        private const int INITIAL_BROJ_SPRAVA_PO_STRANI_TAK3 = 3;

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

        public virtual void initHeaderFooterFromForm(HeaderFooterForm form)
        {
            Header1Text = form.Header1Text;
            Header2Text = form.Header2Text;
            Header3Text = form.Header3Text;
            Header4Text = form.Header4Text;
            FooterText = form.FooterText;

            Header1Font = form.Header1Font;
            Header2Font = form.Header2Font;
            Header3Font = form.Header3Font;
            Header4Font = form.Header4Font;
            FooterFont = form.FooterFont;

            Header1FontSize = form.Header1FontSize;
            Header2FontSize = form.Header2FontSize;
            Header3FontSize = form.Header3FontSize;
            Header4FontSize = form.Header4FontSize;
            FooterFontSize = form.FooterFontSize;

            Header1FontBold = form.Header1FontBold;
            Header2FontBold = form.Header2FontBold;
            Header3FontBold = form.Header3FontBold;
            Header4FontBold = form.Header4FontBold;
            FooterFontBold = form.FooterFontBold;

            Header1FontItalic = form.Header1FontItalic;
            Header2FontItalic = form.Header2FontItalic;
            Header3FontItalic = form.Header3FontItalic;
            Header4FontItalic = form.Header4FontItalic;
            FooterFontItalic = form.FooterFontItalic;

            PrikaziDEOcene = form.PrikaziDEOcene;
            if (form.DeoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                StampajSveSpraveTak1 = form.StampajSveSprave;
                if (StampajSveSpraveTak1)
                    BrojSpravaPoStraniTak1 = form.BrojSpravaPoStrani;
            }
            else
            {
                StampajSveSpraveTak3 = form.StampajSveSprave;
                if (StampajSveSpraveTak3)
                    BrojSpravaPoStraniTak3 = form.BrojSpravaPoStrani;
            }
        }

        public virtual void initHeaderFooterFormFromOpcije(HeaderFooterForm form)
        {
            form.Header1Text = Header1Text;
            form.Header2Text = Header2Text;
            form.Header3Text = Header3Text;
            form.Header4Text = Header4Text;
            form.FooterText = FooterText;

            form.Header1Font = Header1Font;
            form.Header2Font = Header2Font;
            form.Header3Font = Header3Font;
            form.Header4Font = Header4Font;
            form.FooterFont = FooterFont;

            form.Header1FontSize = Header1FontSize;
            form.Header2FontSize = Header2FontSize;
            form.Header3FontSize = Header3FontSize;
            form.Header4FontSize = Header4FontSize;
            form.FooterFontSize = FooterFontSize;

            form.Header1FontBold = Header1FontBold;
            form.Header2FontBold = Header2FontBold;
            form.Header3FontBold = Header3FontBold;
            form.Header4FontBold = Header4FontBold;
            form.FooterFontBold = FooterFontBold;

            form.Header1FontItalic = Header1FontItalic;
            form.Header2FontItalic = Header2FontItalic;
            form.Header3FontItalic = Header3FontItalic;
            form.Header4FontItalic = Header4FontItalic;
            form.FooterFontItalic = FooterFontItalic;

            form.PrikaziDEOcene = PrikaziDEOcene;
            if (form.DeoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                form.StampajSveSprave = StampajSveSpraveTak1;
                if (StampajSveSpraveTak1)
                    form.BrojSpravaPoStrani = BrojSpravaPoStraniTak1;
                else
                    form.BrojSpravaPoStrani = INITIAL_BROJ_SPRAVA_PO_STRANI_TAK1;
            }
            else
            {
                form.StampajSveSprave = StampajSveSpraveTak3;
                if (StampajSveSpraveTak3)
                    form.BrojSpravaPoStrani = BrojSpravaPoStraniTak3;
                else
                    form.BrojSpravaPoStrani = INITIAL_BROJ_SPRAVA_PO_STRANI_TAK3;
            }
        }

        public virtual FontStyle getFontStyle(bool bold, bool italic)
        {
            if (!bold && !italic)
                return FontStyle.Regular;
            else if (bold && !italic)
                return FontStyle.Bold;
            else if (!bold && italic)
                return FontStyle.Italic;
            else
                return FontStyle.Bold | FontStyle.Italic;
        }

        private string _printerName;
        public virtual string PrinterName
        {
            get { return _printerName; }
            set { _printerName = value; }
        }


    }
}
