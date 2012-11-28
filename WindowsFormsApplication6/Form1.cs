using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MATPaint
{
    public partial class Form1 : Form
    {
        // for Pattern Status SAVE/Open functionality
        // to BE DONE....
        enum FileStatus { New, Open};
        FileStatus currentFile = FileStatus.New;
        bool isChanged = false;
        string matrixFileName = null;

        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string matrixFileNameARG) {
            InitializeComponent();
            matrixFileName = matrixFileNameARG;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.appLocation;
            this.WindowState = Properties.Settings.Default.appState;
            this.Size = Properties.Settings.Default.appSize;
            num_Col.Value = Properties.Settings.Default.MatCols;
            num_Row.Value = Properties.Settings.Default.MatRows;
            recent1toolStripMenuItem.Text = Properties.Settings.Default.Recent1;
            recent2toolStripMenuItem.Text = Properties.Settings.Default.Recent2;
            recent3toolStripMenuItem.Text = Properties.Settings.Default.Recent3;
            bt_RotataClock.Enabled = false;
            bt_RotateAnticlock.Enabled = false;
            if (matrixFileName != null)
            {
                OpenMatrix(matrixFileName);
                currentFile = FileStatus.Open;
                isChanged = false;
            }
            else
            {
                currentFile = FileStatus.New;
                NewMatrix();
            }
            UpdateMatrixCellSize();

            // Tooltip 
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.num_Col, "Select the Width");
            ToolTip1.SetToolTip(this.num_Row, "Select the Height");
            ToolTip1.SetToolTip(this.cmbx_Grid, "Select the size of the block");
            ToolTip1.SetToolTip(this.bt_MatrixCreate, "To create and display the matrix of provided parameters");
            ToolTip1.SetToolTip(this.bt_Clear, "Clear the matrix data");
            ToolTip1.SetToolTip(this.bt_Invert, "Invert the matrix");
            ToolTip1.SetToolTip(this.bt_Mirror, "Create the mirror of the matrix");
            ToolTip1.SetToolTip(this.bt_Flip, "Turn over the matrix");
            ToolTip1.SetToolTip(this.bt_RotateAnticlock, "Rotate anticlockwise by 90 deg");
            ToolTip1.SetToolTip(this.bt_RotataClock, "Rotate clockwise by 90 deg");
            ToolTip1.SetToolTip(this.bt_RotataClock, "Rotate clockwise by 90 deg");
            ToolTip1.SetToolTip(this.bt_RotataClock, "Rotate clockwise by 90 deg");
            ToolTip1.SetToolTip(this.bt_RotataClock, "Rotate clockwise by 90 deg");
            ToolTip1.SetToolTip(this.cbx_GridRoll, "Choose whether to rotate/roll the grid");
            ToolTip1.SetToolTip(this.bt_Up, "Move up in the grid");
            ToolTip1.SetToolTip(this.bt_Down, "Move down in the grid");
            ToolTip1.SetToolTip(this.bt_Left, "Move left in the grid");
            ToolTip1.SetToolTip(this.bt_Right, "Move right in the grid");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isChanged == true && currentFile == FileStatus.New)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                    SaveMatrix();
                else if (z == DialogResult.Cancel)
                    e.Cancel = true;
            }
            else if (isChanged == true && currentFile == FileStatus.Open)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                    SaveOpenMatrix();
                else if (z == DialogResult.Cancel)
                    e.Cancel = true;
            }

            Properties.Settings.Default.appLocation = this.Location;
            Properties.Settings.Default.appState = this.WindowState;
            Properties.Settings.Default.appSize = this.Size;
            Properties.Settings.Default.MatCols = Convert.ToInt16(num_Col.Value);
            Properties.Settings.Default.MatRows = Convert.ToInt16(num_Row.Value);
            Properties.Settings.Default.Recent1 = recent1toolStripMenuItem.Text;
            Properties.Settings.Default.Recent2 = recent2toolStripMenuItem.Text;
            Properties.Settings.Default.Recent3 = recent3toolStripMenuItem.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                this.ShowInTaskbar = false;
            }
            else
            {
                notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
            }
        }
        
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void bt_MatrixCreate_Click(object sender, EventArgs e)
        {
            if (isChanged == true && currentFile == FileStatus.New)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveMatrix();
                    NewMatrix();
                }
                else if (z == DialogResult.No)
                    NewMatrix();
            }
            else if (isChanged == true && currentFile == FileStatus.Open)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveOpenMatrix();
                    NewMatrix();
                }
                else if (z == DialogResult.No)
                    NewMatrix();
            }
            else if (isChanged == false)
                NewMatrix();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.Style.BackColor != Color.Black)
                dataGridView1.CurrentCell.Style.BackColor = Color.Black;
            else
                dataGridView1.CurrentCell.Style.BackColor = Color.White;
            dataGridView1.CurrentCell.Selected = false;
            isChanged = true;
        }

        private void cmbx_Grid_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMatrixCellSize();
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            UpdateMatrixCellSize();
        }

        private void UpdateMatrixCellSize()
        {
            int RowHeight = 10, ColumnWidth = 10;
            switch (cmbx_Grid.SelectedIndex)
            {
                case 0:
                    // Fill Screen
                    if (dataGridView1.RowCount > 0)
                        RowHeight = dataGridView1.Height / dataGridView1.RowCount;
                    if (dataGridView1.ColumnCount > 0)
                        ColumnWidth = dataGridView1.Width / dataGridView1.ColumnCount;
                    break;
                case 1:
                    // Tiny
                    RowHeight = 12;
                    ColumnWidth = 12;
                    break;
                case 2:
                    // Medium
                    RowHeight = 22;
                    ColumnWidth = 22;
                    break;
                case 3:
                    // Large
                    RowHeight = 42;
                    ColumnWidth = 42;
                    break;
                case 4:
                    // HUGE
                    RowHeight = 62;
                    ColumnWidth = 62;
                    break;
                default:
                    break;
            }

            for (int z = 0; z < dataGridView1.RowCount; z++)
                dataGridView1.Rows[z].Height = RowHeight;
            for (int z = 0; z < dataGridView1.ColumnCount; z++)
                dataGridView1.Columns[z].Width = ColumnWidth;
        }

        private void bt_Invert_Click(object sender, EventArgs e)
        {
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    if (dataGridView1.Rows[z].Cells[y].InheritedStyle.BackColor != Color.Black)
                        dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.Black;
                    else
                        dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.White;
            isChanged = true;
        }

        private void bt_Clear_Click(object sender, EventArgs e)
        {
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.White;
        }

        private void bt_Mirror_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.ColumnCount - 1;
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount / 2; y++)
                {
                    Color temp = dataGridView1.Rows[z].Cells[y].Style.BackColor;
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = dataGridView1.Rows[z].Cells[x - y].Style.BackColor;
                    dataGridView1.Rows[z].Cells[x - y].Style.BackColor = temp;                    
                }
            isChanged = true;
        }

        private void bt_Flip_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.RowCount - 1;
            for (int z = 0; z < dataGridView1.RowCount / 2; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                {
                    Color temp = dataGridView1.Rows[z].Cells[y].Style.BackColor;
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = dataGridView1.Rows[x - z].Cells[y].Style.BackColor;
                    dataGridView1.Rows[x - z].Cells[y].Style.BackColor = temp;
                }
            isChanged = true;
        }

        private void bt_RotateAnticlock_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.ColumnCount - 1;
            Color[,] GridArray = new Color[dataGridView1.RowCount, dataGridView1.ColumnCount];

            // Saving grid data in a 2D array
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    GridArray[z,y] = dataGridView1.Rows[z].Cells[y].Style.BackColor;

            // Rotating anticlockwise by 90 deg
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = GridArray[y, x - z];
            isChanged = true;
        }

        private void bt_RotataClock_Click(object sender, EventArgs e)
        {
            int x = dataGridView1.ColumnCount - 1;
            Color[,] GridArray = new Color[dataGridView1.RowCount, dataGridView1.ColumnCount];

            // Saving grid data in a 2D array
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    GridArray[z, y] = dataGridView1.Rows[z].Cells[y].Style.BackColor;

            // Rotating clockwise by 90 deg
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = GridArray[x - y, z];
            isChanged = true;
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (isChanged == true && currentFile == FileStatus.New)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveMatrix();
                    NewMatrix();
                }
                else if (z == DialogResult.No)
                    NewMatrix();
            }
            else if (isChanged == true && currentFile == FileStatus.Open)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveOpenMatrix();
                    NewMatrix();
                }
                else if (z == DialogResult.No)
                    NewMatrix();
            }
            else if (isChanged == false)
                NewMatrix();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (currentFile == FileStatus.New)
                SaveMatrix();
            else if (currentFile == FileStatus.Open)
                SaveOpenMatrix();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMatrix();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (isChanged == true && currentFile == FileStatus.New)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveMatrix();
                    OpenMatrix();
                }
                else if (z == DialogResult.No)
                    OpenMatrix();
            }
            else if (isChanged == true && currentFile == FileStatus.Open)
            {
                DialogResult z = MessageBox.Show("Do you want to SAVE the current Pattern", "Save Pattern", MessageBoxButtons.YesNoCancel);
                if (z == DialogResult.Yes)
                {
                    SaveOpenMatrix();
                    OpenMatrix();
                }
                else if (z == DialogResult.No)
                    OpenMatrix();
            }
            else
                OpenMatrix();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
                    this.Close();
        }

        private void NewMatrix()
        {
            dataGridView1.ColumnCount = Convert.ToInt16(num_Col.Value);
            dataGridView1.RowCount = Convert.ToInt16(num_Row.Value);
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.White;

            dataGridView1.CurrentCell.Selected = false;
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            UpdateMatrixCellSize();
            if (dataGridView1.RowCount == dataGridView1.ColumnCount)
            {
                bt_RotateAnticlock.Enabled = true;
                bt_RotataClock.Enabled = true;
            }
            UpdateMatrixCellSize();
            currentFile = FileStatus.New;
            isChanged = false;
            this.Text = "MATPaint - ";
        }

        private void SaveMatrix()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string matrixCode = "";
                matrixCode = dataGridView1.ColumnCount.ToString() + "," + dataGridView1.RowCount.ToString() + "\r\n";
                int row = 0, col = 0, page = 0, z = 0;
                for (page = 0; page <= dataGridView1.RowCount / 8; page++)
                {
                    for (col = 0; col < dataGridView1.ColumnCount; col++)
                    {
                        for (row = 0; row < 8; row++)
                        {
                            if (dataGridView1.RowCount > (page * 8) + row)
                                if (dataGridView1.Rows[(page * 8) + row].Cells[col].Style.BackColor == Color.Black)
                                    z = z + Convert.ToInt16(Math.Pow(2.0, row));
                            z += 0;
                        }
                        matrixCode = matrixCode + z.ToString() + ",";
                        z = 0;
                    }
                    matrixCode = matrixCode + "\r\n";
                }
                TextWriter tw = new StreamWriter(saveFileDialog1.FileName);
                tw.Write(matrixCode);
                tw.Close();

                matrixFileName = saveFileDialog1.FileName;
                isChanged = false;
                currentFile = FileStatus.Open;
                this.Text = "MATPaint - " + matrixFileName;
                saveFileDialog1.FileName = "";
            }
        }
        
        private void SaveOpenMatrix()
        {
            string matrixCode = "";
            matrixCode = dataGridView1.ColumnCount.ToString() + "," + dataGridView1.RowCount.ToString() + "\r\n";
            int row = 0, col = 0, page = 0, z = 0;
            for (page = 0; page <= dataGridView1.RowCount / 8; page++)
            {
                for (col = 0; col < dataGridView1.ColumnCount; col++)
                {
                    for (row = 0; row < 8; row++)
                    {
                        if (dataGridView1.RowCount > (page * 8) + row)
                            if (dataGridView1.Rows[(page * 8) + row].Cells[col].Style.BackColor == Color.Black)
                                z = z + Convert.ToInt16(Math.Pow(2.0, row));
                        z += 0;
                    }
                    matrixCode = matrixCode + z.ToString() + ",";
                    z = 0;
                }
                matrixCode = matrixCode + "\r\n";
            }
            TextWriter tw = new StreamWriter(matrixFileName);
            tw.Write(matrixCode);
            tw.Close();

            isChanged = false;
            currentFile = FileStatus.Open;
            saveFileDialog1.FileName = "";
        }

        private void OpenMatrix(string matrixFileNameToOpen = null)
        {
            bool readFile = false;
            openFileDialog1.FileName = matrixFileNameToOpen;
            
            if(matrixFileNameToOpen != null)
                readFile = true;
            else if (openFileDialog1.ShowDialog() == DialogResult.OK)
                readFile = true;

            if (readFile == true)
            {
                // Open FILE SPECIFCIATIONS
                // File should be in simple TXT format, ',' delimited
                // First Line: ColumnCount, RowCount
                // Second Line and so on: Each line has data of 8 rows in INT format and commas separate the different Columns
                try
                {
                    TextReader tr = new StreamReader(openFileDialog1.FileName);
                    string matrixCode = tr.ReadToEnd();
                    tr.Close();
                    string[] cellDataS = matrixCode.Split(new char[] { ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    int[] cellData = new int[cellDataS.Length];
                    int count = 0;
                    foreach (string s in cellDataS)
                        cellData[count++] = Convert.ToInt16(s);

                    int row = cellData[1], col = cellData[0], page = 0, z = 0;

                    count = 1;
                    dataGridView1.ColumnCount = col;
                    dataGridView1.RowCount = row;
                    bt_Clear_Click(this, EventArgs.Empty);
                    try
                    {
                        for (page = 0; page <= row / 8; page++)
                            for (col = 0; col < dataGridView1.ColumnCount; col++)
                            {
                                z = cellData[++count];
                                for (row = 0; row < 8; row++)
                                    if (dataGridView1.RowCount > (page * 8) + row)
                                        if ((z & Convert.ToInt16(Math.Pow(2.0, row))) > 0)
                                            dataGridView1.Rows[(page * 8) + row].Cells[col].Style.BackColor = Color.Black;
                            }
                    }
                    catch { }
                    dataGridView1.ClearSelection();
                    matrixFileName = openFileDialog1.FileName;
                    isChanged = false;
                    currentFile = FileStatus.Open;
                    num_Col.Value = dataGridView1.ColumnCount;
                    num_Row.Value = dataGridView1.RowCount;

                    if (dataGridView1.RowCount == dataGridView1.ColumnCount)
                    {
                        bt_RotateAnticlock.Enabled = true;
                        bt_RotataClock.Enabled = true;
                    }
                    else
                    {
                        bt_RotateAnticlock.Enabled = false;
                        bt_RotataClock.Enabled = false;
                    }

                    if (matrixFileName != recent1toolStripMenuItem.Text
                        && matrixFileName != recent2toolStripMenuItem.Text
                        && matrixFileName != recent3toolStripMenuItem.Text)
                    {
                        recent3toolStripMenuItem.Text = recent2toolStripMenuItem.Text;
                        recent2toolStripMenuItem.Text = recent1toolStripMenuItem.Text;
                        recent1toolStripMenuItem.Text = matrixFileName;
                    }
                    this.Text = "MATPaint - " + matrixFileName;
                }
                catch { MessageBox.Show("Cannot Read File", "File Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void bt_ChngMatrixSize_Click(object sender, EventArgs e)
        {
            int tmpCol = Convert.ToInt16(num_Col.Value);
            int tmpRow = Convert.ToInt16(num_Row.Value);
            if ((tmpCol < dataGridView1.ColumnCount) || (tmpRow < dataGridView1.RowCount))
            {
                if (MessageBox.Show("The New Size is Smaller than current Size, some Clipping may occur, \nContinue ?", "Resize Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    dataGridView1.ColumnCount = tmpCol;
                    dataGridView1.RowCount = tmpRow;
                    UpdateMatrixCellSize();
                    isChanged = true;
                }
            }
            else
            {
                dataGridView1.ColumnCount = tmpCol;
                dataGridView1.RowCount = tmpRow;
                UpdateMatrixCellSize();
                isChanged = true;
            }
            if (dataGridView1.RowCount == dataGridView1.ColumnCount)
            {
                bt_RotateAnticlock.Enabled = true;
                bt_RotataClock.Enabled = true;
            }
            else {
                bt_RotateAnticlock.Enabled = false;
                bt_RotataClock.Enabled = false;
            }
        }

        private void bt_Up_Click(object sender, EventArgs e)
        {
            // Storing the data of the critical Row 0 - the first row
            Color[] temp = new Color[dataGridView1.ColumnCount];
            for (int y = 0; y < dataGridView1.ColumnCount; y++)
                temp[y] = dataGridView1.Rows[0].Cells[y].Style.BackColor;

            // Moving the rest of the grid UP
            for (int z = 0; z < dataGridView1.RowCount - 1; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = dataGridView1.Rows[z + 1].Cells[y].Style.BackColor;

            // Checking whether to roll or shift the grid UP
            if (cbx_GridRoll.Checked == true)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[y].Style.BackColor = temp[y];
            else
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[y].Style.BackColor = Color.White;
        }

        private void bt_Down_Click(object sender, EventArgs e)
        {
            // Storing the data of the critical Row - the last row
            Color[] temp = new Color[dataGridView1.ColumnCount];
            for (int y = 0; y < dataGridView1.ColumnCount; y++)
                temp[y] = dataGridView1.Rows[dataGridView1.RowCount - 1].Cells[y].Style.BackColor;

            // Moving the rest of the grid DOWN
            for (int z = dataGridView1.RowCount - 1; z > 0; z--)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = dataGridView1.Rows[z - 1].Cells[y].Style.BackColor;

            // Checking whether to roll or shift the grid DOWN
            if (cbx_GridRoll.Checked == true)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[0].Cells[y].Style.BackColor = temp[y];
            else
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[0].Cells[y].Style.BackColor = Color.White;
        }

        private void bt_Left_Click(object sender, EventArgs e)
        {
            // Storing the data of the critical Column 0 - the first column
            Color[] temp = new Color[dataGridView1.RowCount];
            for (int x = 0; x < dataGridView1.RowCount; x++)
                temp[x] = dataGridView1.Rows[x].Cells[0].Style.BackColor;

            // Moving the rest of the grid to the LEFT
            for (int y = 0; y < dataGridView1.ColumnCount - 1; y++)
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[y].Style.BackColor = dataGridView1.Rows[x].Cells[y + 1].Style.BackColor;

            // Checking whether to roll or shift the grid to the LEFT
            if (cbx_GridRoll.Checked == true)
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[dataGridView1.ColumnCount - 1].Style.BackColor = temp[x];
            else
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[dataGridView1.ColumnCount - 1].Style.BackColor = Color.White;
        }

        private void bt_Right_Click(object sender, EventArgs e)
        {
            // Storing the data of the critical Column 0 - the last column
            Color[] temp = new Color[dataGridView1.RowCount];
            for (int x = 0; x < dataGridView1.RowCount; x++)
                temp[x] = dataGridView1.Rows[x].Cells[dataGridView1.ColumnCount - 1].Style.BackColor;

            // Moving the rest of the grid to the LEFT
            for (int y = dataGridView1.ColumnCount - 1; y > 0; y--)
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[y].Style.BackColor = dataGridView1.Rows[x].Cells[y - 1].Style.BackColor;

            // Checking whether to roll or shift the grid to the RIGHT
            if (cbx_GridRoll.Checked == true)
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[0].Style.BackColor = temp[x];
            else
                for (int x = 0; x < dataGridView1.RowCount; x++)
                    dataGridView1.Rows[x].Cells[0].Style.BackColor = Color.White;


        }

        private void recent1toolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMatrix(recent1toolStripMenuItem.Text);
        }

        private void recent2toolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMatrix(recent2toolStripMenuItem.Text);
        }

        private void recent3toolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMatrix(recent3toolStripMenuItem.Text);
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap matrixPNG = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.DrawToBitmap(matrixPNG, dataGridView1.ClientRectangle);
            e.Graphics.DrawImage(matrixPNG, new Point(0,0));
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" MATPaint - MATrix Paint\n Made by:\n Zaid Pirwani and Maaz Ahmed\n\nas class project for C# Course (OOP)\ntaught by Engr. Sajid Hussain\nat\nIIEE PCSIR","About BOX",MessageBoxButtons.OK,MessageBoxIcon.Information);

        }

        private void imagePNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap matrixPNG = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.DrawToBitmap(matrixPNG, dataGridView1.ClientRectangle);

            saveFileDialog1.Filter = "PNG Image File|*.png";
            saveFileDialog1.Title = "Save PNG Screenshot";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                matrixPNG.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            saveFileDialog1.Filter = "Matrix Pattern Files|*.maz";
            saveFileDialog1.Title = "Save Pattern File";
            saveFileDialog1.FileName = "";
        }

        private void binaryCodeHEXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Binary Code File |*.hex";
            saveFileDialog1.Title = "Save Pattern Hex Code";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                using (FileStream hex = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(hex))
                    {
                        int row = 0, col = 0, page = 0;
                        byte z=0;
                        for (page = 0; page <= dataGridView1.RowCount / 8; page++)
                            for (col = 0; col < dataGridView1.ColumnCount; col++)
                            {
                                for (row = 0; row < 8; row++)
                                {
                                    if (dataGridView1.RowCount > (page * 8) + row)
                                        if (dataGridView1.Rows[(page * 8) + row].Cells[col].Style.BackColor == Color.Black)
                                            z = Convert.ToByte(z + Math.Pow(2.0, row));
                                    z += 0;
                                }
                                writer.Write(z);
                                z = 0;
                            }
                        writer.Close();
                    }
                }
            saveFileDialog1.Filter = "Matrix Pattern Files|*.maz";
            saveFileDialog1.Title = "Save Pattern File";
            saveFileDialog1.FileName = "";
        }
    }
}
