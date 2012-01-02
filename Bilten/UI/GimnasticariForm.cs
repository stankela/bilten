using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;
using Bilten.Exceptions;
using Bilten.Data;
using Bilten.Data.QueryModel;

namespace Bilten.UI
{
    public partial class GimnasticariForm : SingleEntityListForm<Gimnasticar>
    {
        public GimnasticariForm()
        {
            // NOTE: Kada form nasledjuje drugi form koji ima genericki parametar,
            // dizajner nece da ga prikaze. Zato sam izbacio fajl
            // GimnasticariForm.Designer.cs (jer je nepotreban) i poziv
            // InitializeComponent(). Ukoliko form treba da dodaje neke kontrole
            // (osim onih koje je nasledio), to treba da se radi programski.
            
            this.Text = "Gimnasticari";
            InitializeGrid();
            InitializeGridColumns();
            FetchModes.Add(new AssociationFetch(
                "Kategorija", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Klub", AssociationFetchMode.Eager));
            FetchModes.Add(new AssociationFetch(
                "Drzava", AssociationFetchMode.Eager));

            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                ShowFirstPage();

        //        dataContext.Commit();
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

        private void InitializeGridColumns()
        {
            AddColumn("Ime", "ImeSrednjeIme", 100);
            AddColumn("Prezime", "Prezime", 100);
            AddColumn("Datum rodjenja", "DatumRodjenja", 100, "{0:d}");
            AddColumn("Gimnastika", "Gimnastika", 70);
            AddColumn("Drzava", "Drzava", 100);
            AddColumn("Registarski broj", "RegistarskiBroj", 100);
            AddColumn("Poslednja registr.", "DatumPoslednjeRegistracije", 100, "{0:d}");
            AddColumn("Kategorija", "Kategorija", 100);
            AddColumn("Klub", "Klub", 100);
        }

        protected override EntityDetailForm createEntityDetailForm(Nullable<int> entityId)
        {
            return new GimnasticarForm(entityId);
        }

        protected override int getEntityId(Gimnasticar entity)
        {
            return entity.Id;
        }

        protected override string DefaultSortingPropertyName
        {
            get { return "Prezime"; }
        }

        protected override void beforeSort()
        {
            // bitno je da brojac ide u nazad zato sto se umece u listu
            for (int i = SortPropertyNames.Count - 1; i >= 0; i--)
            {
                if (SortPropertyNames[i] == "DatumRodjenja")
                {
                    SortPropertyNames[i] = "DatumRodjenja.Dan";
                    SortPropertyNames.Insert(i, "DatumRodjenja.Mesec");
                    SortPropertyNames.Insert(i, "DatumRodjenja.Godina");
                }
                if (SortPropertyNames[i] == "RegistarskiBroj")
                {
                    SortPropertyNames[i] = "RegistarskiBroj.Broj";
                }
                if (SortPropertyNames[i] == "DatumPoslednjeRegistracije")
                {
                    SortPropertyNames[i] = "DatumPoslednjeRegistracije.Dan";
                    SortPropertyNames.Insert(i, "DatumPoslednjeRegistracije.Mesec");
                    SortPropertyNames.Insert(i, "DatumPoslednjeRegistracije.Godina");
                }
            }
        }

        protected override string deleteConfirmationMessage(Gimnasticar gimnasticar)
        {
            return String.Format("Da li zelite da izbrisete gimnasticara \"{0}\"?", gimnasticar);
        }

        protected override string deleteErrorMessage()
        {
            return "Neuspesno brisanje gimnasticara.";
        }

    }
}