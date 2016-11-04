
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
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
using System.Xml;
using System.Xml.Serialization;

namespace PDFMetadataExtractor
{
    public partial class MainForm : Form
    {
        List<MetadataCoordinate> metadataList = new List<MetadataCoordinate>();
        Dictionary<int, PictureBox> pageBitmapList = new Dictionary<int, PictureBox>();

        string selectedMetadataName = string.Empty;
        System.Drawing.Rectangle drawingRectangle;
        string currentPdfFilepath = string.Empty;
        System.Drawing.Rectangle mSelect;
        bool mDragging;
        int currentPdfFilePages = 0;
        int currentPage = 0;
        PictureBox currentPictureBox = null;


        public MainForm()
        {
            InitializeComponent();
        }

        private void openPdfFileToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
            openDialog.Title = "Abrir archivo PDF de ejemplo";
            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Reset();
                currentPdfFilepath = openDialog.FileName;
                toolStripStatusLabel.Text = "Abriendo archivo PDF...";
                ShowPdfPage(1);
                toolStripStatusLabel.Text = "Listo";
            }
        }

        private void Reset()
        {
            currentPage = 0;
            txtCurrentPage.Text = "";
            currentPdfFilePages = 0;
            lblTotalPages.Text = currentPdfFilePages.ToString();

            currentPictureBox = null;
            currentPdfFilepath = string.Empty;

            pageBitmapList.Clear();

            while (pnlDesigner.Controls.Count > 0)
            {
                pnlDesigner.Controls.RemoveAt(0);
            }

            metadataList = new List<MetadataCoordinate>();
            lvwMetadataList.Items.Clear();
            toolStripStatusLabel.Text = "";

        }

        public MemoryStream ConvertPDFToImage(string filepath, int page)
        {
            var converter = new Aspose.Pdf.Facades.PdfConverter();
            converter.BindPdf(filepath);
            converter.StartPage = page;
            converter.EndPage = page;
            converter.Resolution = new Aspose.Pdf.Devices.Resolution(300);
            converter.DoConvert();

            MemoryStream imageStream = new MemoryStream();

            while (converter.HasNextImage())
            {
                converter.GetNextImage(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Position = 0;
            }
            return imageStream;

        }

        private void ShowPdfPage(int pdfPage)
        {
            pdfPage = Math.Max(pdfPage, 1);

            btnPrevPage.Enabled = pdfPage > 1;


            if (currentPictureBox != null && (currentPage != pdfPage))
                currentPictureBox.Hide();

            currentPage = pdfPage;
            txtCurrentPage.Text = currentPage.ToString();

            if (pageBitmapList.ContainsKey(pdfPage))
            {
                currentPictureBox = pageBitmapList[pdfPage];
            }
            else
            {
                var pictureBox = new PictureBox();

                using (PdfReader pdfReader = new PdfReader(currentPdfFilepath))
                {
                    currentPdfFilePages = pdfReader.NumberOfPages;
                    pdfPage = Math.Min(pdfPage, currentPdfFilePages);
                    btnNextPage.Enabled = pdfPage < currentPdfFilePages;
                    lblTotalPages.Text = currentPdfFilePages.ToString();
                    Document doc = new Document(pdfReader.GetPageSizeWithRotation(pdfPage));
                    pictureBox.Size = new Size(Convert.ToInt32(doc.PageSize.Width), Convert.ToInt32(doc.PageSize.Height));
                }

                toolStripStatusLabel.Text = "Cargando página " + pdfPage + "...";

                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Cursor = System.Windows.Forms.Cursors.Cross;
                pictureBox.Paint += PictureBox_Paint;
                pictureBox.MouseUp += PictureBox_MouseUp;
                pictureBox.MouseDown += PictureBox_MouseDown;
                pictureBox.MouseMove += PictureBox_MouseMove;
                pictureBox.Image = System.Drawing.Image.FromStream(ConvertPDFToImage(currentPdfFilepath, pdfPage));

                pageBitmapList.Add(pdfPage, pictureBox);
                pnlDesigner.Controls.Add(pictureBox);
                currentPictureBox = pictureBox;
                toolStripStatusLabel.Text = "Página " + pdfPage + " cargada correctamente.";

            }


            currentPictureBox.Show();
            currentPictureBox.BringToFront();


        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDragging)
            {
                mSelect.Size = new Size(e.X - mSelect.Left, e.Y - mSelect.Top);
                (sender as PictureBox).Invalidate();
            }

            toolStripStatusLabel.Text = string.Format("X: {0}, Y: {1}, W: {2}, H: {3}", e.Location.X, (sender as PictureBox).Size.Height - e.Location.Y, mSelect.Size.Width, mSelect.Height);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mDragging = true;
            mSelect.Location = e.Location;
            mSelect.Size = new Size(0, 0);
            (sender as PictureBox).Invalidate();
        }

        public void RefreshMetadataList()
        {
            lvwMetadataList.Items.Clear();
            foreach (MetadataCoordinate coordinate in metadataList)
            {
                ListViewGroup pageGroup = null;

                foreach (ListViewGroup group in lvwMetadataList.Groups)
                {
                    if (group.Header == ("Página " + coordinate.Page))
                    {
                        pageGroup = group;
                        break;
                    }
                }

                if (pageGroup == null)
                {
                    pageGroup = new ListViewGroup("Página " + coordinate.Page);
                    pageGroup.Tag = coordinate.Page;
                    lvwMetadataList.Groups.Add(pageGroup);
                }

                lvwMetadataList.Items.Add(new ListViewItem(new string[] { coordinate.MetadataName, coordinate.TestString }, pageGroup));
            }
        }


        public string GetTextInArea(string filename, int page, int x, int y, int width, int height)
        {
            string text = string.Empty;

            using (PdfReader pdfReader = new PdfReader(filename))
            {
                RenderFilter filter = new RegionTextRenderFilter(new System.util.RectangleJ(x, y, width, height));
                var strategy = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filter);
                //var strategy = new SimpleTextExtractionStrategy();   

                text = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(
                    Encoding.Default, Encoding.UTF8,
                    Encoding.Default.GetBytes(text)));
            }

            return text;
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            mDragging = false;
            mSelect = drawingRectangle;

            if (mSelect.Width == 0 || mSelect.Height == 0)
                return;

            /*currentTest = GetTextInArea("C:\\Users\\mxi02001166a\\Documents\\Pegaso\\P_123456_autos_70_ZAQ349960101_ZAQ34996_000000_11122012_440_71704_019883_F_11122012_10315754.pdf",
                1, mSelect.X, pictureBox1.Size.Height - mSelect.Bottom, mSelect.Width, mSelect.Height); */

            string readText = GetTextInArea(currentPdfFilepath,
                currentPage, mSelect.X, (sender as PictureBox).Size.Height - mSelect.Bottom, mSelect.Width, mSelect.Height);

            if (string.IsNullOrEmpty(readText))
            {
                mSelect.Size = new Size(0, 0);
                currentPictureBox.Invalidate();
                return;
            }

            MetadataNameInputDialog metaNameInputDialog = new MetadataNameInputDialog();
            metaNameInputDialog.ReadText = readText;
            if (metaNameInputDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string metadataName = metaNameInputDialog.MetadataName;
                string baseMetadataName = metaNameInputDialog.MetadataName;

                int index = 1;

                for (int i = 0; i < metadataList.Count; i++)
                {
                    if (metadataList[i].MetadataName == metadataName)
                    {
                        metadataName = baseMetadataName + " (" + index + ")";
                        index++;
                        i = -1;
                    }
                }

                var coordinate =  new MetadataCoordinate()
                {
                    MetadataName = metadataName,
                    X = mSelect.X,
                    Y = currentPictureBox.Size.Height - mSelect.Bottom,
                    Width = mSelect.Width,
                    Height = mSelect.Height,
                    TestString = readText,
                    Page = currentPage
                };

                AddMetadataTag(coordinate);
                RefreshMetadataList();

            }
            else
            {
                mSelect.Size = new Size(0, 0);
                currentPictureBox.Invalidate();
            }
        }

        public void RemoveMetadataCoordinate(MetadataCoordinate coordinate)
        {
            currentPictureBox.Controls.Remove(coordinate.HoverPanel);
            currentPictureBox.Controls.Remove(coordinate.MetadataTag);
            metadataList.Remove(coordinate);
            RefreshMetadataList();
            currentPictureBox.Invalidate();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(Color.Orange))
            {
                p.Width = 3;

                foreach (MetadataCoordinate coordinate in metadataList)
                {
                    if (coordinate.Page != currentPage) continue;

                    if (coordinate.MetadataName == selectedMetadataName)
                    {
                        p.Width = 5;
                        p.Color = Color.Yellow;
                    }
                    else
                    {
                        p.Width = 3;
                        p.Color = Color.Orange;
                    }

                    e.Graphics.DrawRectangle(p, coordinate.X, (sender as PictureBox).Size.Height - coordinate.Y - coordinate.Height, coordinate.Width, coordinate.Height);
                }
            }

            using (Pen p = new Pen(Color.Black))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                p.Width = 1;

                drawingRectangle = new System.Drawing.Rectangle((mSelect.Width > 0) ? (mSelect.X) : (mSelect.X + mSelect.Width),
                        (mSelect.Height > 0) ? (mSelect.Y) : (mSelect.Y + mSelect.Height),
                        Math.Abs(mSelect.Width),
                        Math.Abs(mSelect.Height));

                e.Graphics.DrawRectangle(p, drawingRectangle);
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (metadataList == null || metadataList.Count == 0)
            {
                MessageBox.Show("No hay nada que guardar. Por favor especifique los metadatos e inténtelo de nuevo.", "Sin metadatos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Archivo XML (*.xml)|*.xml";
            saveDialog.Title = "Guardar plantilla de coordenadas de metadatos";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<MetadataCoordinate>));

                using (StringWriter stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, metadataList);
                    File.WriteAllText(saveDialog.FileName, stringWriter.ToString());
                }

            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            ShowPrevPage();
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            ShowNextPage();
        }

        private void ShowNextPage()
        {
            if (currentPage == currentPdfFilePages) return;
            ShowPdfPage(currentPage + 1);
        }

        private void ShowPrevPage()
        {
            if (currentPage == 1)
                return;
            ShowPdfPage(currentPage - 1);

        }

        private void openXmlTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPdfFilepath))
            {
                MessageBox.Show("Primero debe abrir el archivo PDF molde.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Archivos XML (*.xml)|*.xml";
            openDialog.Title = "Seleccione el archivo XML de coordenadas a cargar";
            openDialog.Multiselect = false;

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadData(openDialog.FileName);
            }
        }

        public void AddMetadataTag(MetadataCoordinate coordinate)
        {
            var tag = new MetadataTag();
            tag.MetadataName = coordinate.MetadataName;
            tag.Tag = coordinate;
            tag.OnNameChanged += ((sender, e) =>
            {
                ((sender as MetadataTag).Tag as MetadataCoordinate).MetadataName = (sender as MetadataTag).MetadataName;
                RefreshMetadataList();
            });

            tag.Location = new Point(coordinate.X, currentPictureBox.Height - coordinate.Y);
            tag.Hide();

            var panel = new TransparentPanel();
            panel.Location = new Point(coordinate.X, currentPictureBox.Height - coordinate.Y - coordinate.Height);
            panel.Size = new System.Drawing.Size(coordinate.Width, coordinate.Height);
            panel.Cursor = System.Windows.Forms.Cursors.Hand;
            panel.MouseHover += ((s, ev) => { ((s as TransparentPanel).Tag as MetadataCoordinate).MetadataTag.Show(); });
            panel.MouseLeave += ((s, ev) =>
            {
                var metadataTag = ((s as TransparentPanel).Tag as MetadataCoordinate).MetadataTag;
                metadataTag.Hide();
                metadataTag.CancelEditing();
            });
            panel.Click += ((s, ev) => { ((s as TransparentPanel).Tag as MetadataCoordinate).MetadataTag.StartEditing(); });

            coordinate.HoverPanel = panel;
       
            MenuItem menuItem = new MenuItem("Eliminar", ((s, ev) =>
            {
                RemoveMetadataCoordinate(((s as MenuItem).Tag as MetadataCoordinate));
            }));
            menuItem.Tag = coordinate;


            panel.ContextMenu = new System.Windows.Forms.ContextMenu(new MenuItem[] { menuItem });
            panel.Tag = coordinate;

            currentPictureBox.Controls.Add(tag);
            currentPictureBox.Controls.Add(panel);

            coordinate.MetadataTag = tag;

            mSelect.Size = new Size(0, 0);
            currentPictureBox.Invalidate();
            metadataList.Add(coordinate);


        }

        public void LoadData(string xmlPath)
        {
            List<MetadataCoordinate> loadedMetadataList = null;

            try
            {
                var xsSubmit = new XmlSerializer(typeof(List<MetadataCoordinate>));
                var sww = new StringReader(File.ReadAllText(xmlPath));
                var reader = XmlReader.Create(sww);
                loadedMetadataList = xsSubmit.Deserialize(reader) as List<MetadataCoordinate>;
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al cargar el archivo. Asegúrese de que es válido.", "Archivo inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (MetadataCoordinate metadataCoordinate in loadedMetadataList)
            {
                metadataCoordinate.TestString = GetTextInArea(currentPdfFilepath, metadataCoordinate.Page, metadataCoordinate.X, metadataCoordinate.Y, metadataCoordinate.Width, metadataCoordinate.Height);
                AddMetadataTag(metadataCoordinate);
            }

            RefreshMetadataList();

        }
    }
}
