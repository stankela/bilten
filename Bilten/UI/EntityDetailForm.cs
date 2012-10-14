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
    public partial class EntityDetailForm : Form
    {
        protected DomainObject entity;
        protected bool editMode;
        protected bool initializing;
        protected IDataContext dataContext;
        private bool persistEntity;
        private bool closedByOK;
        private bool closedByCancel;
        protected bool showWaitCursor;

        public DomainObject Entity
        {
            get { return entity; }
        }

        protected EntityDetailForm()
        {
            InitializeComponent();
        }

        protected void initialize(Nullable<int> entityId, bool persistEntity)
        {
            initializing = true;
            this.persistEntity = persistEntity;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                if (entityId != null)
                {
                    editMode = true;
                    initUpdateMode(entityId.Value);
                }
                else
                {
                    editMode = false;
                    initAddMode();
                }

           //     dataContext.Commit();

                initializing = false;
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

        // NOTE: Radi samo u update modu, kada se umesto id dostavlja sam objekat
        protected void initialize2(DomainObject entity, bool persistEntity)
        {
            initializing = true;
            this.persistEntity = persistEntity;
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                editMode = true;
                initUpdateMode(entity);

                //     dataContext.Commit();

                initializing = false;
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

        protected virtual void initAddMode()
        {
            entity = createNewEntity();
            loadData();
            initUI();
        }

        protected virtual DomainObject createNewEntity()
        {
            throw new Exception("Derived class should implement this method.");
        }

        protected virtual void initUpdateMode(int entityId)
        {
            // Najpre se ucitava objekt, zatim ostali podaci potrebni za UI,
            // i tek nakon toga se inicijalizuje UI. Razlog za ovakav redosled je
            // slucaj kada neko svojstvo objekta utice na UI (npr. koje ce
            // opcije prisutne u combo box-u.)
            entity = getEntityById(entityId);
            saveOriginalData(entity);
            loadData();
            initUI();
            updateUIFromEntity(entity);
        }

        protected void initUpdateMode(DomainObject entity)
        {
            this.entity = entity;
            saveOriginalData(entity);
            loadData();
            initUI();
            updateUIFromEntity(entity);
        }

        protected virtual void loadData()
        {
            // Empty
        }

        protected virtual DomainObject getEntityById(int id)
        {
            throw new Exception("Derived class should implement this method.");
        }

        protected virtual void initUI()
        {

        }

        protected virtual void updateUIFromEntity(DomainObject entity)
        {
            throw new Exception("Derived class should implement this method.");
        }

        protected virtual void saveOriginalData(DomainObject entity)
        {
            // Empty
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            handleOkClick();
        }

        protected virtual void handleOkClick()
        {
            if (showWaitCursor)
            {
                Cursor.Current = Cursors.WaitCursor;
                Cursor.Show();
            }
            try
            {
                DataAccessProviderFactory factory = new DataAccessProviderFactory();
                dataContext = factory.GetDataContext();
                dataContext.BeginTransaction();

                Notification notification = new Notification();
                requiredFieldsAndFormatValidation(notification);
                if (!notification.IsValid())
                    throw new BusinessException(notification);

                if (editMode)
                    update();
                else
                    add();

                if (persistEntity)
                    dataContext.Commit();
                closedByOK = true;
            }
            catch (BusinessException ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                if (ex.Notification != null)
                {
                    NotificationMessage msg = ex.Notification.FirstMessage;
                    MessageDialogs.showMessage(msg.Message, this.Text);
                    setFocus(msg.FieldName);
                }
                else if (!string.IsNullOrEmpty(ex.InvalidProperty))
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                    setFocus(ex.InvalidProperty);
                }
                else
                {
                    MessageDialogs.showMessage(ex.Message, this.Text);
                }
                this.DialogResult = DialogResult.None;
            }
            /*catch (StaleObjectStateException staleEx)
            {
                // TODO: This implementation does not implement optimistic concurrency 
                // control. Your application will not work until you add compensation 
                // actions. Rollback, close everything, possibly compensate for any 
                // permanent changes during the conversation, and finally restart 
                // business conversation. Maybe give the user of the application a 
                // chance to merge some of his work with fresh data... what you do 
                // here depends on your applications design.

                throw staleEx;
            }*/
            catch (Exception ex)
            {
                if (dataContext != null && dataContext.IsInTransaction)
                    dataContext.Rollback();
                MessageDialogs.showError(
                    Strings.getFullDatabaseAccessExceptionMessage(ex), this.Text);
                this.DialogResult = DialogResult.Cancel;
                discardChanges();
                closedByCancel = true;
            }
            finally
            {
                if (dataContext != null)
                    dataContext.Dispose();
                dataContext = null;

                if (showWaitCursor)
                {
                    Cursor.Hide();
                    Cursor.Current = Cursors.Arrow;
                }
            }
        }

        protected virtual void requiredFieldsAndFormatValidation(Notification notification)
        {
            // Empty
        }

        protected virtual void setFocus(string propertyName)
        {
            // Empty
        }

        private void add()
        {
            updateEntityFromUI(entity);
            validateEntity(entity);
            checkBusinessRulesOnAdd(entity);
            if (persistEntity)
                addEntity(entity);
        }

        protected virtual void updateEntityFromUI(DomainObject entity)
        {
            throw new Exception("Derived class should implement this method.");
        }

        protected virtual void validateEntity(DomainObject entity)
        {
            Notification notification = new Notification();
            entity.validate(notification);
            if (!notification.IsValid())
                throw new BusinessException(notification);
        }

        protected virtual void checkBusinessRulesOnAdd(DomainObject entity)
        {
            // Empty
        }

        protected virtual void addEntity(DomainObject entity)
        {
            dataContext.Add(entity);
        }

        private void update()
        {
            updateEntityFromUI(entity);
            validateEntity(entity);
            checkBusinessRulesOnUpdate(entity);
            if (persistEntity)
                updateEntity(entity);
        }

        protected virtual void updateEntity(DomainObject entity)
        {
            dataContext.Save(entity);
        }

        protected virtual void checkBusinessRulesOnUpdate(DomainObject entity)
        {            
            // Empty
        }

        protected virtual void discardChanges()
        {
            // Empty
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            discardChanges();
            closedByCancel = true;
        }

        private void EntityDetailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // CloseReason je UserClosing je kada se pritisne dugme X ili se pozove Close
            // (kada se pritisne OK ili Cancel CloseReason je None)

            // NOTE: Ovde izgleda postoji neki bug. Kada se prvi put pritisne dugme
            // X, CloseReason je UserClosing. Ako tada pritisnem Cancel (u prozoru koji
            // mi kaze da izmene nece biti sacuvane) i ostanem u dijalogu, i zatim
            // pritisnem OK, CloseReason ce ponovo biti UserClosing (kao da daje
            // zaostalu vrednost od proslog puta)

     //       if (e.CloseReason == CloseReason.UserClosing)
            if (!closedByOK && !closedByCancel)
            {
                if (isDirty())
                {
                    bool canClose = MessageBox.Show(
                        "Izmene koje ste uneli nece biti sacuvane?", "Klub",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) ==
                            DialogResult.OK;
                    e.Cancel = !canClose;
                }
            }
        }

        private bool isDirty()
        {
            // TODO
            return true;
        }

        private void EntityDetailForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closedByOK && !closedByCancel)
            {
                discardChanges();
            }
        }

    }
}