using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
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

        private void bt_MatrixCreate_Click(object sender, EventArgs e)
        {
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

        // Inverting - black to white and vice versa
        private void bt_Invert_Click(object sender, EventArgs e)
        {
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    if (dataGridView1.Rows[z].Cells[y].InheritedStyle.BackColor != Color.Black)
                        dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.Black;
                    else
                        dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.White;
        }

        // Clear all grid data
        private void bt_Clear_Click(object sender, EventArgs e)
        {
            for (int z = 0; z < dataGridView1.RowCount; z++)
                for (int y = 0; y < dataGridView1.ColumnCount; y++)
                    dataGridView1.Rows[z].Cells[y].Style.BackColor = Color.White;
        }

        // Mirror - on horizontal plane
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
        }

        // Flip  - on vertical plane
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bt_RotataClock.Enabled = false;
            bt_RotateAnticlock.Enabled = false;
            currentFile = FileStatus.New;
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (currentFile == FileStatus.New)
                SaveMatrix();
            else if (currentFile == FileStatus.Open)
                SaveOpenMatrix();
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

        private void SaveMatrix() {
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
            }
        }

        private void OpenMatrix()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open FILE SPECIFCIATIONS
                // File should be in simple TXT format, ',' delimited
                // First Line: ColumnCount, RowCount
                // Second Line and so on: Each line has data of 8 rows in INT format and commas separate the different Columns
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

                this.Text = "MATPaint - " + matrixFileName;
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
        }

        private void NewMatrix()
        {
            dataGridView1.ColumnCount = int.Parse(cmbx_Column.Text);
            dataGridView1.RowCount = int.Parse(cmbx_Row.Text);
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

            currentFile = FileStatus.New;
            isChanged = false;
        }
    }
}
