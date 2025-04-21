using Bilten.Dao;
using Bilten.Data;
using Bilten.Domain;
using Bilten.Services;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bilten.UI
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        private void btnUpdateFinalaKupa_Click(object sender, EventArgs e)
        {
            updateFinalaKupa(TipTakmicenja.FinaleKupa);
        }

        private void btnUpdateZbirViseKola_Click(object sender, EventArgs e)
        {
            updateFinalaKupa(TipTakmicenja.ZbirViseKola);
        }

        private void updateFinalaKupa(TipTakmicenja tipTakmicenja)
        {
            IList<Takmicenje> takmicenja = findTakmicenja(tipTakmicenja);
            for (int i = 0; i < takmicenja.Count; ++i)
            {
                // TODO5: U finalu kupa "ZSG - UKUPNO I i II KOLO PGL SRBIJE ŽSG - REGION I SPRAVE, SUBOTICA, 6.11.2021"
                // postoji ozbiljna greska u bazi podataka. Drugo kolo nije ZSG nego je neko MSG takmicenje. Ovo je
                // nemoguce da se desi kada se kreira finale kupa, zato sto se u dijalogu u kome se biraju prethodna kola
                // prikazuju samo odgovarajuca takmicenja (MSG ili ZSG). Kada se otvori navedeno finale kupa, rezultati su
                // u redu, tj kada je finale kupa kreirano prethodna kola su bila odgovarajuca. Gresku sam otkrio kada
                // sam promenio LastModified za finale kupa - kada je program pokusao da ponovo kreira rezultate iz
                // prethodnih kola, jedno od kola je bilo MSG umesto ZSG.
                // Isto i za sledeca zbir vise kola takmicenja:
                // "{ZSG - UKUPNO I i II KOLO PGL SRBIJE ŽSG - REGION I, Subotica, 6.11.2021}"
                // "{MSG - UKUPNO I i II KOLO PGL VOJVODINE MSG 2021., Novi Sad, 30.10.2021}"
                // "{ZSG - UKUPNO I i II kolo KUP-a Vojvodine "C" program ŽSG Senta 2020, Senta, 24.10.2020}"
                // "{ZSG - UKUPNO I i II kolo PGL Vojvodine ŽSG 2020, Subotica, 31.10.2020}"
                // "{MSG - Ukupno I i II kolo PGL Vojvodine MSG 2022, Novi Sad, 29.10.2022}"
                // "{ZSG - UKUPNO KUP VOJVODINE "C program" ŽSG 2022., Novi Sad, 22.10.2022}"
                // "{ZSG - Ukupno I i II kolo ŽSG "C program" GSS - Region I, Novi Sad, 22.10.2022}"

                if (tipTakmicenja == TipTakmicenja.FinaleKupa)
                {
                    //if (/*i+1 != 6 &&*/ i + 1 != 38)
                    //continue;
                }
                if (tipTakmicenja == TipTakmicenja.ZbirViseKola)
                {
                    //if (/*i + 1 != 12 && i+1 != 13 && i+1 != 21 && i+1 != 22 && i+1 != 28 && i+1 != 29 &&*/ i+1 != 30)
                        //continue;
                }
                Takmicenje t = takmicenja[i];
                if (t.TipTakmicenja != tipTakmicenja)
                    throw new Exception("Pogresan tip takmicenja");

                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                ISession session = null;
                try
                {
                    using (session = NHibernateHelper.Instance.OpenSession())
                    using (session.BeginTransaction())
                    {
                        CurrentSessionContext.Bind(session);

                        string takmicenjeCaption = (i + 1).ToString() + "/" + takmicenja.Count;
                        takmicenjeCaption += " (" + t.Naziv + " " + t.Datum.ToShortDateString() + ")";

                        TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                        takmicenjeDAO.Attach(t, false);
                        bool shouldUpdate = t.PrvoKolo != null && t.PrvoKolo.LastModified > t.LastModified
                            || t.DrugoKolo != null && t.DrugoKolo.LastModified > t.LastModified
                            || t.TreceKolo != null && t.TreceKolo.LastModified > t.LastModified
                            || t.CetvrtoKolo != null && t.CetvrtoKolo.LastModified > t.LastModified;

                        shouldUpdate = true;
                        if (shouldUpdate)
                        {
                            MessageDialogs.showMessage("Updating takmicenje " + takmicenjeCaption, "");
                            TakmicenjeService.updateViseKola(t);
                            t.LastModified = DateTime.Now;
                            session.Transaction.Commit();
                        }
                        else
                        {
                            MessageDialogs.showMessage("Takmicenje " + takmicenjeCaption + " ne treba update", "");
                        }
                        //if (!MessageDialogs.queryConfirmation("Da li zelite da nastavite?", ""))
                        //break;
                    }
                }
                catch (Exception)
                {
                    if (session != null && session.Transaction != null && session.Transaction.IsActive)
                        session.Transaction.Rollback();
                    throw;
                }
                finally
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                    CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                }
            }
        }

        private static IList<Takmicenje> findTakmicenja(TipTakmicenja tipTakmicenja)
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    return takmicenjeDAO.FindByTipTakmicenja(tipTakmicenja);
                }
            }
            catch (Exception)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void dumpTakmicenja(TipTakmicenja tipTakmicenja, string fileName)
        {
            StreamWriter logStreamWriter = File.CreateText(fileName);
            IList<Takmicenje> takmicenja = findTakmicenja(tipTakmicenja);
            for (int i = 0; i < takmicenja.Count; ++i)
            {
                if (tipTakmicenja == TipTakmicenja.FinaleKupa)
                {
                    //if (/*i+1 != 6 &&*/ i + 1 != 38)
                    //continue;
                }
                if (tipTakmicenja == TipTakmicenja.ZbirViseKola)
                {
                    //if (/*i + 1 != 11 && i + 1 != 12 && i+1 != 13 && i+1 != 21 && i+1 != 22 && i+1 != 28
                    //     && i+1 != 29 &&*/ i+1 != 30)
                    //continue;
                }
                Takmicenje t = takmicenja[i];
                if (t.TipTakmicenja != tipTakmicenja)
                    throw new Exception("Pogresan tip takmicenja");

                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
                ISession session = null;
                try
                {
                    using (session = NHibernateHelper.Instance.OpenSession())
                    using (session.BeginTransaction())
                    {
                        CurrentSessionContext.Bind(session);

                        //string takmicenjeCaption = (i + 1).ToString() + "/" + takmicenja.Count;
                        //takmicenjeCaption += " (" + t.Naziv + " " + t.Datum.ToShortDateString() + ")";

                        TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                        takmicenjeDAO.Attach(t, false);

                        logStreamWriter.WriteLine((i + 1).ToString() + ". " + t.ToString());
                        string tab = (i + 1 < 10) ? "   " : "    ";
                        if (t.PrvoKolo != null)
                            logStreamWriter.WriteLine(tab + t.PrvoKolo.ToString());
                        if (t.DrugoKolo != null)
                            logStreamWriter.WriteLine(tab + t.DrugoKolo.ToString());
                        if (t.TreceKolo != null)
                            logStreamWriter.WriteLine(tab + t.TreceKolo.ToString());
                        if (t.CetvrtoKolo != null)
                            logStreamWriter.WriteLine(tab + t.CetvrtoKolo.ToString());
                        logStreamWriter.WriteLine("");
                    }
                }
                catch (Exception)
                {
                    if (session != null && session.Transaction != null && session.Transaction.IsActive)
                        session.Transaction.Rollback();
                    logStreamWriter.Close();
                    throw;
                }
                finally
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                    CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
                }
            }
            logStreamWriter.Close();
        }

        private void btnDumpFinaleKupa_Click(object sender, EventArgs e)
        {
            dumpTakmicenja(TipTakmicenja.FinaleKupa, "dump_finale_kupa.txt");
        }

        private void btnDumpZbirViseKola_Click(object sender, EventArgs e)
        {
            dumpTakmicenja(TipTakmicenja.ZbirViseKola, "dump_zbir_vise_kola.txt");
        }

        private void btnIspraviFinalaKupa_Click(object sender, EventArgs e)
        {
            ispraviFinalaKupa();
        }

        public static void ispraviFinalaKupa()
        {
            ispraviTakmicenje(TipTakmicenja.FinaleKupa,
                "ZSG - UKUPNO I i II KOLO PGL SRBIJE ŽSG - REGION I SPRAVE, SUBOTICA, 6.11.2021",
                "ZSG - II kolo PGL Srbije ŽSG - Region I, Subotica, 6.11.2021", false);
        }

        private static void ispraviTakmicenje(TipTakmicenja tipTakmicenja, string takmicenjeStr, string prethodnoKoloStr,
            bool prvoKolo)
        {
            IList<Takmicenje> takmicenja = findTakmicenja(tipTakmicenja);
            Takmicenje t = null;
            for (int i = 0; i < takmicenja.Count; ++i)
            {
                if (takmicenja[i].ToString() == takmicenjeStr)
                {
                    if (t != null)
                        throw new Exception("Greska u programu1");
                    t = takmicenja[i];
                }
            }
            IList<Takmicenje> takmicenja2 = findTakmicenja(TipTakmicenja.StandardnoTakmicenje);
            Takmicenje prethodnoKolo = null;
            for (int i = 0; i < takmicenja2.Count; ++i)
            {
                if (takmicenja2[i].ToString() == prethodnoKoloStr)
                {
                    if (prethodnoKolo != null)
                        throw new Exception("Greska u programu2");
                    prethodnoKolo = takmicenja2[i];
                }
            }
            if (t == null || prethodnoKolo == null)
                throw new Exception("Greska u programu3");

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    takmicenjeDAO.Attach(t, false);
                    if (prvoKolo)
                        t.PrvoKolo = prethodnoKolo;
                    else
                        t.DrugoKolo = prethodnoKolo;
                    t.LastModified = DateTime.Now;
                    takmicenjeDAO.Update(t);
                    session.Transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private static void ispraviTakmicenje(TipTakmicenja tipTakmicenja, string takmicenjeStr, string prvoKoloStr,
            string drugoKoloStr)
        {
            IList<Takmicenje> takmicenja = findTakmicenja(tipTakmicenja);
            Takmicenje t = null;
            for (int i = 0; i < takmicenja.Count; ++i)
            {
                if (takmicenja[i].ToString() == takmicenjeStr)
                {
                    if (t != null)
                        throw new Exception("Greska u programu1");
                    t = takmicenja[i];
                }
            }
            IList<Takmicenje> takmicenja2 = findTakmicenja(TipTakmicenja.StandardnoTakmicenje);
            Takmicenje prvoKolo = null;
            Takmicenje drugoKolo = null;
            for (int i = 0; i < takmicenja2.Count; ++i)
            {
                if (takmicenja2[i].ToString() == prvoKoloStr)
                {
                    if (prvoKolo != null)
                        throw new Exception("Greska u programu2");
                    prvoKolo = takmicenja2[i];
                }
                if (takmicenja2[i].ToString() == drugoKoloStr)
                {
                    if (drugoKolo != null)
                        throw new Exception("Greska u programu3");
                    drugoKolo = takmicenja2[i];
                }
            }
            if (t == null || prvoKolo == null || drugoKolo == null)
                throw new Exception("Greska u programu4");

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    takmicenjeDAO.Attach(t, false);
                    t.PrvoKolo = prvoKolo;
                    t.DrugoKolo = drugoKolo;
                    t.LastModified = DateTime.Now;
                    takmicenjeDAO.Update(t);
                    session.Transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private static void ispraviTakmicenje(TipTakmicenja tipTakmicenja, string takmicenjeStr, string prvoKoloStr,
           string drugoKoloStr, string treceKoloStr, string cetvrtoKoloStr)
        {
            IList<Takmicenje> takmicenja = findTakmicenja(tipTakmicenja);
            Takmicenje t = null;
            for (int i = 0; i < takmicenja.Count; ++i)
            {
                if (takmicenja[i].ToString() == takmicenjeStr)
                {
                    if (t != null)
                        throw new Exception("Greska u programu1");
                    t = takmicenja[i];
                }
            }
            IList<Takmicenje> takmicenja2 = findTakmicenja(TipTakmicenja.StandardnoTakmicenje);
            Takmicenje prvoKolo = null;
            Takmicenje drugoKolo = null;
            Takmicenje treceKolo = null;
            Takmicenje cetvrtoKolo = null;
            for (int i = 0; i < takmicenja2.Count; ++i)
            {
                if (takmicenja2[i].ToString() == prvoKoloStr)
                {
                    if (prvoKolo != null)
                        throw new Exception("Greska u programu2");
                    prvoKolo = takmicenja2[i];
                }
                if (takmicenja2[i].ToString() == drugoKoloStr)
                {
                    if (drugoKolo != null)
                        throw new Exception("Greska u programu3");
                    drugoKolo = takmicenja2[i];
                }
                if (takmicenja2[i].ToString() == treceKoloStr)
                {
                    if (treceKolo != null)
                        throw new Exception("Greska u programu4");
                    treceKolo = takmicenja2[i];
                }
                if (takmicenja2[i].ToString() == cetvrtoKoloStr)
                {
                    if (cetvrtoKolo != null)
                        throw new Exception("Greska u programu5");
                    cetvrtoKolo = takmicenja2[i];
                }
            }
            if (t == null || prvoKolo == null || drugoKolo == null)
                throw new Exception("Greska u programu6");

            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);

                    TakmicenjeDAO takmicenjeDAO = DAOFactoryFactory.DAOFactory.GetTakmicenjeDAO();
                    takmicenjeDAO.Attach(t, false);
                    t.PrvoKolo = prvoKolo;
                    t.DrugoKolo = drugoKolo;
                    t.TreceKolo = treceKolo;
                    t.CetvrtoKolo = cetvrtoKolo;
                    t.LastModified = DateTime.Now;
                    takmicenjeDAO.Update(t);
                    session.Transaction.Commit();
                }
            }
            catch (Exception)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw;
            }
            finally
            {
                Cursor.Hide();
                Cursor.Current = Cursors.Arrow;
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void btnIspraviZbirViseKola_Click(object sender, EventArgs e)
        {
            ispraviZbirViseKola();
        }

        public static void ispraviZbirViseKola()
        {
            // 11
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - Ukupno I i II kolo PGL Vojvodine ŽSG 2021, Novi Sad, 2.10.2021",
                "ZSG - II kolo PGL Vojvodine ŽSG 2021, Novi Sad, 2.10.2021", false);
            // 12
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - UKUPNO I i II KOLO PGL SRBIJE ŽSG - REGION I, Subotica, 6.11.2021",
                "ZSG - II kolo PGL Srbije ŽSG - Region I, Subotica, 6.11.2021", false);
            // 13
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "MSG - UKUPNO I i II KOLO PGL VOJVODINE MSG 2021., Novi Sad, 30.10.2021",
                "MSG - II KOLO PGL VOJVODINE MSG 2021., Novi Sad, 30.10.2021", false);
            // 21
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - UKUPNO I i II kolo KUP-a Vojvodine \"C\" program ŽSG Senta 2020, Senta, 24.10.2020",
                "ZSG - I KOLO KUP-a VOJVODINE \"C program\" ŽSG SOMBOR 2020, SOMBOR, 10.10.2020",
                "ZSG - II kolo KUP-a Vojvodine \"C\" program ŽSG Senta 2020, Senta, 24.10.2020");
            // 22
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - UKUPNO I i II kolo PGL Vojvodine ŽSG 2020, Subotica, 31.10.2020",
                "ZSG - I kolo PGL Vojvodine ŽSG 2020, Novi Sad, 17.10.2020",
                "ZSG - II kolo PGL Vojvodine ŽSG 2020, Subotica, 31.10.2020");
            // 28
            // TODO5: II kolo je prazno u bazi
            // TODO5: Za finale kupa i zbir vise kola, treba negde da se vidi koja su prethodna kola
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "MSG - Ukupno I i II kolo PGL Vojvodine MSG 2022, Novi Sad, 29.10.2022",
                "MSG - I kolo PGL Vojvodine MSG 2022., Subotica, 7.5.2022",
                "MSG - II kolo PGL Vojvodine MSG 2022, Novi Sad, 29.10.2022");
            // 29
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - UKUPNO KUP VOJVODINE \"C program\" ŽSG 2022., Novi Sad, 22.10.2022",
                "ZSG - I KOLO KUP-a VOJVODINE \"C program\" ŽSG - SOMBOR 2022., SOMBOR, 9.4.2022",
                "ZSG - II KOLO KUP-a VOJVODINE \"C program\" ŽSG - SREMSKA MITROVICA 2022., Sremska Mitrovica, 8.5.2022",
                "ZSG - III KOLO KUP-a VOJVODINE \"C program\" ŽSG - SENTA 2022, Senta, 8.10.2022",
                "ZSG - IV KOLO KUP-a VOJVODINE \"C program\" ŽSG - NOVI SAD 2022, Novi Sad, 22.10.2022");
            // 30
            ispraviTakmicenje(TipTakmicenja.ZbirViseKola,
                "ZSG - Ukupno I i II kolo ŽSG \"C program\" GSS - Region I, Novi Sad, 22.10.2022",
                "ZSG - I KOLO KUP-a VOJVODINE \"C program\" ŽSG - SOMBOR 2022., SOMBOR, 9.4.2022",
                "ZSG - IV KOLO KUP-a VOJVODINE \"C program\" ŽSG - NOVI SAD 2022, Novi Sad, 22.10.2022");
        }

        private void btnDumpStandardnaTakmicenja_Click(object sender, EventArgs e)
        {
            dumpTakmicenja(TipTakmicenja.StandardnoTakmicenje, "dump_standardno_takmicenje.txt");
        }
    }
}
