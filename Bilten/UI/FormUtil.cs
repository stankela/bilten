using Bilten.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bilten.UI
{
    class FormUtil
    {
        public static void initHeaderFooterFromForm(HeaderFooterForm form)
        {
            Opcije o = Opcije.Instance;
            o.Header1Text = form.Header1Text;
            o.Header2Text = form.Header2Text;
            o.Header3Text = form.Header3Text;
            o.Header4Text = form.Header4Text;
            o.FooterText = form.FooterText;

            o.Header1Font = form.Header1Font;
            o.Header2Font = form.Header2Font;
            o.Header3Font = form.Header3Font;
            o.Header4Font = form.Header4Font;
            o.FooterFont = form.FooterFont;

            o.Header1FontSize = form.Header1FontSize;
            o.Header2FontSize = form.Header2FontSize;
            o.Header3FontSize = form.Header3FontSize;
            o.Header4FontSize = form.Header4FontSize;
            o.FooterFontSize = form.FooterFontSize;

            o.Header1FontBold = form.Header1FontBold;
            o.Header2FontBold = form.Header2FontBold;
            o.Header3FontBold = form.Header3FontBold;
            o.Header4FontBold = form.Header4FontBold;
            o.FooterFontBold = form.FooterFontBold;

            o.Header1FontItalic = form.Header1FontItalic;
            o.Header2FontItalic = form.Header2FontItalic;
            o.Header3FontItalic = form.Header3FontItalic;
            o.Header4FontItalic = form.Header4FontItalic;
            o.FooterFontItalic = form.FooterFontItalic;

            o.PrikaziDEOcene = form.PrikaziDEOcene;
            if (form.DeoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                o.StampajSveSpraveTak1 = form.StampajSveSprave;
                if (o.StampajSveSpraveTak1)
                    o.BrojSpravaPoStraniTak1 = form.BrojSpravaPoStrani;
            }
            else
            {
                o.StampajSveSpraveTak3 = form.StampajSveSprave;
                if (o.StampajSveSpraveTak3)
                    o.BrojSpravaPoStraniTak3 = form.BrojSpravaPoStrani;
            }
            o.PrikaziPenalSprave = form.PrikaziPenalSprave;
            o.StampajRedniBrojNaStartListi = form.StampajRedniBrojNaStartListi;
            o.StampajKategoriju = form.StampajKategoriju;
            o.StampajKlub = form.StampajKlub;
            o.BrojEOcenaFormular = form.BrojEOcenaFormular;
        }

        public static void initHeaderFooterFormFromOpcije(HeaderFooterForm form)
        {
            Opcije o = Opcije.Instance;
            form.Header1Text = o.Header1Text;
            form.Header2Text = o.Header2Text;
            form.Header3Text = o.Header3Text;
            form.Header4Text = o.Header4Text;
            form.FooterText = o.FooterText;

            form.Header1Font = o.Header1Font;
            form.Header2Font = o.Header2Font;
            form.Header3Font = o.Header3Font;
            form.Header4Font = o.Header4Font;
            form.FooterFont = o.FooterFont;

            form.Header1FontSize = o.Header1FontSize;
            form.Header2FontSize = o.Header2FontSize;
            form.Header3FontSize = o.Header3FontSize;
            form.Header4FontSize = o.Header4FontSize;
            form.FooterFontSize = o.FooterFontSize;

            form.Header1FontBold = o.Header1FontBold;
            form.Header2FontBold = o.Header2FontBold;
            form.Header3FontBold = o.Header3FontBold;
            form.Header4FontBold = o.Header4FontBold;
            form.FooterFontBold = o.FooterFontBold;

            form.Header1FontItalic = o.Header1FontItalic;
            form.Header2FontItalic = o.Header2FontItalic;
            form.Header3FontItalic = o.Header3FontItalic;
            form.Header4FontItalic = o.Header4FontItalic;
            form.FooterFontItalic = o.FooterFontItalic;

            form.PrikaziDEOcene = o.PrikaziDEOcene;
            if (form.DeoTakKod == DeoTakmicenjaKod.Takmicenje1)
            {
                form.StampajSveSprave = o.StampajSveSpraveTak1;
                if (o.StampajSveSpraveTak1)
                    form.BrojSpravaPoStrani = o.BrojSpravaPoStraniTak1;
                else
                    form.BrojSpravaPoStrani = Opcije.INITIAL_BROJ_SPRAVA_PO_STRANI_TAK1;
            }
            else
            {
                form.StampajSveSprave = o.StampajSveSpraveTak3;
                if (o.StampajSveSpraveTak3)
                    form.BrojSpravaPoStrani = o.BrojSpravaPoStraniTak3;
                else
                    form.BrojSpravaPoStrani = Opcije.INITIAL_BROJ_SPRAVA_PO_STRANI_TAK3;
            }
            form.PrikaziPenalSprave = o.PrikaziPenalSprave;
            form.StampajRedniBrojNaStartListi = o.StampajRedniBrojNaStartListi;
            form.StampajKategoriju = o.StampajKategoriju;
            form.StampajKlub = o.StampajKlub;
            form.BrojEOcenaFormular = o.BrojEOcenaFormular;
        }

    }
}
