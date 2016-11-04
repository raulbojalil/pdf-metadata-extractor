using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFMetadataExtractor
{
    public partial class MetadataTag : UserControl
    {

        public event EventHandler OnNameChanged;

        public string MetadataName
        {
            get
            {
                return lblMetadataName.Text;
            }
            set
            {
                lblMetadataName.Text = value;
            }
        }

        public MetadataTag()
        {
            InitializeComponent();
            lblMetadataName.Show();
            txtMetadataName.Hide();
        }

        private void MetadataTag_Click(object sender, EventArgs e)
        {
            StartEditing(); 
        }

        public void StartEditing()
        {
            lblMetadataName.Hide();
            txtMetadataName.Show();
            txtMetadataName.Text = lblMetadataName.Text;
            txtMetadataName.Focus();
        }

        public void CancelEditing()
        {
            lblMetadataName.Show();
            txtMetadataName.Hide();
        }

        private void txtMetadataName_Leave(object sender, EventArgs e)
        {
            CancelEditing();
            lblMetadataName.Text = txtMetadataName.Text;

            if (OnNameChanged != null)
                OnNameChanged(this, null);
        }

        private void txtMetadataName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CancelEditing();
                lblMetadataName.Text = txtMetadataName.Text;

                if (OnNameChanged != null)
                    OnNameChanged(this, null);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelEditing();
            }
        }

        private void lblMetadataName_Click(object sender, EventArgs e)
        {
            StartEditing();
        }

    }
}
