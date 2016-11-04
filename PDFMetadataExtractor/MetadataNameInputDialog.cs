using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFMetadataExtractor
{
    public partial class MetadataNameInputDialog : Form
    {

        public string MetadataName
        {
            get
            {
                return txtMetadataName.Text;
            }
        }

        public string ReadText
        {
            get
            {
                return txtReadText.Text;
            }
            set
            {
                txtReadText.Text = value;
            }
        }

        public MetadataNameInputDialog()
        {
            InitializeComponent();
            txtMetadataName.Focus();
        }

        
        private void MetadataNameInputDialog_Load(object sender, EventArgs e)
        {
            txtMetadataName.Text = string.Empty;
            btnOk.Enabled = false;
            txtMetadataName.Focus();
            
        } 

        private void txtMetadataName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMetadataName.Text))
                btnOk.Enabled = true;
            else
                btnOk.Enabled = false;
        }

    }
}
