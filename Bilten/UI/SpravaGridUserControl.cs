using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Bilten.Domain;

namespace Bilten.UI
{
    public partial class SpravaGridUserControl : UserControl
    {
        public event EventHandler<SpravaGridMouseDownEventArgs> SpravaGridMouseDown;
        public event EventHandler<SpravaGridMouseUpEventArgs> SpravaGridMouseUp;
    
        private Sprava sprava;
        public Sprava Sprava
        {
            get { return sprava; }
        }

        public DataGridViewUserControl DataGridViewUserControl
        {
            get { return dataGridViewUserControl1; }
        }

        public SpravaGridUserControl()
        {
            InitializeComponent();

            dataGridViewUserControl1.DataGridView.MultiSelect = false;

            dataGridViewUserControl1.DataGridView.CellMouseDown += new DataGridViewCellMouseEventHandler(DataGridView_CellMouseDown);
            dataGridViewUserControl1.DataGridView.MouseUp += new MouseEventHandler(DataGridView_MouseUp);
        }

        // NOTE: Ovo nisam mogao da stavim u konstruktor zato sto dizajner nece
        // da kreira kontrolu (moguce da je zbog static metoda getImage)
        public void init(Sprava sprava)
        {
            this.sprava = sprava;
            pictureBoxSprava.Image = SlikeSprava.getImage(sprava);
            centerPicture();
        }

        private void centerPicture()
        {
            int x = (dataGridViewUserControl1.Width - pictureBoxSprava.Width) / 2;
            int y = 5;
            pictureBoxSprava.Location = new Point(
                dataGridViewUserControl1.Location.X + x, y);
            dataGridViewUserControl1.Location = new Point(
                dataGridViewUserControl1.Location.X,
                pictureBoxSprava.Bottom + y);
        }

        void DataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    // selektuj vrstu
                    clearSelection();
                    dataGridViewUserControl1.DataGridView.Rows[e.RowIndex].Selected = true;
                }
            }
            OnSpravaGridMouseDown(new SpravaGridMouseDownEventArgs(sprava));
        }

        protected virtual void OnSpravaGridMouseDown(SpravaGridMouseDownEventArgs e)
        {
            // Save the delegate field in a temporary field for thread safety
            EventHandler<SpravaGridMouseDownEventArgs> temp = SpravaGridMouseDown;

            if (temp != null)
                temp(this, e);
        }

        // NOTE: Nisam koristio CellMouseUp zato sto on ne daje pravilne koordinate
        // misa
        private void DataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            OnSpravaGridMouseUp(new SpravaGridMouseUpEventArgs(sprava, e));
        }

        protected virtual void OnSpravaGridMouseUp(SpravaGridMouseUpEventArgs e)
        {
            // Save the delegate field in a temporary field for thread safety
            EventHandler<SpravaGridMouseUpEventArgs> temp = SpravaGridMouseUp;

            if (temp != null)
                temp(this, e);
        }

        public void clearSelection()
        {
            dataGridViewUserControl1.clearSelection();
        }

        public T getSelectedItem<T>()
        {
            return dataGridViewUserControl1.getSelectedItem<T>();
        }

        public void setSelectedItem<T>(T item)
        {
            dataGridViewUserControl1.setSelectedItem<T>(item);
        }

        // NOTE: Primetiti kako nije potrebno da stavljam genericki parametar T
        // u zaglavlju klase prilikom pozivanja metoda. Takodje primetiti da u ovom
        // konkretnom slucaju nije potrebno ni da navodim ime konkretne klase prilikom 
        // pozivanja metoda ( metod je moguce pozvati sa npr. setItems(raspored) - nije neophodno
        // (mada je moguce) setItems<SudijaNaSpravi>(raspored) ). Ovo drugo vazi samo
        // kada metod ima parametar ciji tip ima genericki parametar T (kao npr. 
        // IList<T>).
        public void setItems<T>(IList<T> items)
        {
            dataGridViewUserControl1.setItems<T>(items);
        }

        public void clearItems()
        {
            dataGridViewUserControl1.clearItems();
        }

        private void SpravaGridUserControl_Resize(object sender, EventArgs e)
        {
            centerPicture();
        }
    }
}
