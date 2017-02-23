using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Data;
using Bilten.Exceptions;
using Bilten.Data.QueryModel;
using NHibernate;
using NHibernate.Context;
using Bilten.Dao;

namespace Bilten.UI
{
    public partial class KategorijeGimnasticaraForm : SingleEntityListForm<KategorijaGimnasticara>
    {
        public KategorijeGimnasticaraForm()
        {
            // NOTE: Kada form nasledjuje drugi form koji ima genericki parametar,
            // dizajner nece da ga prikaze. Zato sam izbacio fajl
            // GimnasticariForm.Designer.cs (jer je nepotreban) i poziv
            // InitializeComponent(). Ukoliko form treba da dodaje neke kontrole
            // (osim onih koje je nasledio), to treba da se radi programski.

            this.Text = "Kategorije gimnasticara";
            dataGridViewUserControl1.GridColumnHeaderMouseClick +=
                new EventHandler<GridColumnHeaderMouseClickEventArgs>(DataGridViewUserControl_GridColumnHeaderMouseClick);
            InitializeGridColumns();

            ISession session = null;
            try
            {
                using (session = NHibernateHelper.Instance.OpenSession())
                using (session.BeginTransaction())
                {
                    CurrentSessionContext.Bind(session);
                    IList<KategorijaGimnasticara> kategorije
                        = DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().FindAll();
                    SetItems(kategorije);
                    dataGridViewUserControl1.sort<KategorijaGimnasticara>(
                        new string[] { "Naziv" },
                        new ListSortDirection[] { ListSortDirection.Ascending });
                    updateEntityCount();
                }
            }
            catch (Exception ex)
            {
                if (session != null && session.Transaction != null && session.Transaction.IsActive)
                    session.Transaction.Rollback();
                throw new InfrastructureException(ex.Message, ex);
            }
            finally
            {
                CurrentSessionContext.Unbind(NHibernateHelper.Instance.SessionFactory);
            }
        }

        private void DataGridViewUserControl_GridColumnHeaderMouseClick(object sender,
            GridColumnHeaderMouseClickEventArgs e)
        {
            DataGridViewUserControl dgwuc = sender as DataGridViewUserControl;
            if (dgwuc != null)
                dgwuc.onColumnHeaderMouseClick<KategorijaGimnasticara>(e.DataGridViewCellMouseEventArgs);
        }

        private void InitializeGridColumns()
        {
            AddColumn("Naziv kategorije", "Naziv", 100);
            AddColumn("Gimnastika", "Gimnastika", 70);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new KategorijaGimnasticaraForm(entityId);
        }

        protected override string deleteConfirmationMessage(KategorijaGimnasticara kat)
        {
            return String.Format("Da li zelite da izbrisete kategoriju \"{0}\"?", kat);
        }

        protected override bool refIntegrityDeleteDlg(KategorijaGimnasticara kategorija)
        {
            if (!DAOFactoryFactory.DAOFactory.GetGimnasticarDAO().existsGimnasticar(kategorija))
                return true;
            else
            {
                string msg = "Postoje gimnasticari kategorije '{0}'. Ako " +
                    "je izbrisete, ovi gimnasticari nece imati navedenu kategoriju. " +
                    "Da li zelite da izbrisete kategoriju?";
                return MessageDialogs.queryConfirmation(String.Format(msg, kategorija), this.Text);
            }
        }

        protected override void delete(KategorijaGimnasticara kategorija)
        {
            GimnasticarDAO gimnasticarDAO = DAOFactoryFactory.DAOFactory.GetGimnasticarDAO();
            IList<Gimnasticar> gimnasticari = gimnasticarDAO.FindGimnasticariByKategorija(kategorija);
            foreach (Gimnasticar g in gimnasticari)
            {
                g.Kategorija = null;
                gimnasticarDAO.MakePersistent(g);
            }
            DAOFactoryFactory.DAOFactory.GetKategorijaGimnasticaraDAO().MakeTransient(kategorija);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje kategorije.";
        }

        protected override void updateEntityCount()
        {
            int count = dataGridViewUserControl1.getItems<KategorijaGimnasticara>().Count;
            StatusPanel.Panels[0].Text = count.ToString() + " kategorija";
        }
    }
}