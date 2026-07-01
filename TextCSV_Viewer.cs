/*
MIT License

Copyright (c) 2026 Sarayut Chaisuriya

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Note on dataset:
The included MalwareBazaar sample CSV has been modified:
- Limited to first 500 rows
- Header format adjusted for teaching purposes
See README.md for full details.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileProcessing
{
    public partial class frmTextView : Form
    {
        public frmTextView()
        {
            InitializeComponent();

            // ตั้งค่าตาราง
            dgvData.AllowUserToAddRows = false;
            dgvData.ReadOnly = true;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// อ่านไฟล์ Text
        /// </summary>
        private void btRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFileName.Text))
            {
                MessageBox.Show(
                    "กรุณาเลือกไฟล์ก่อน",
                    "ไม่พบไฟล์",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!File.Exists(tbFileName.Text))
            {
                MessageBox.Show(
                    "ไม่พบไฟล์ที่เลือก",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                string content = File.ReadAllText(tbFileName.Text);
                rtbShow.Text = content;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "ไม่สามารถอ่านไฟล์ได้\n" + ex.Message,
                    "Read Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// โหลด CSV ตามช่วง m-n และกรองตามประเภทไฟล์
        /// </summary>
        private async void btReadCSV_Click(
            object sender,
            EventArgs e)
        {
            // ตรวจสอบว่าเลือกไฟล์แล้วหรือยัง
            if (string.IsNullOrWhiteSpace(tbFileName.Text))
            {
                MessageBox.Show(
                    "กรุณาเลือกไฟล์ CSV ก่อน",
                    "ไม่พบไฟล์",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // ตรวจสอบว่าไฟล์มีอยู่จริงหรือไม่
            if (!File.Exists(tbFileName.Text))
            {
                MessageBox.Show(
                    "ไม่พบไฟล์ที่เลือก",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            int startRecord = (int)numStartRecord.Value;
            int endRecord = (int)numEndRecord.Value;

            // ตรวจสอบช่วงข้อมูล
            if (endRecord < startRecord)
            {
                MessageBox.Show(
                    "End Record ต้องไม่น้อยกว่า Start Record",
                    "Invalid Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string fileTypeFilter = txtFileType.Text.Trim();

            // ปิดปุ่มชั่วคราว ป้องกันผู้ใช้กดซ้ำ
            btReadCSV.Enabled = false;
            btBrowse.Enabled = false;

            lblResult.Text = "กำลังโหลดข้อมูล...";
            Cursor = Cursors.WaitCursor;

            dgvData.Rows.Clear();
            dgvData.Columns.Clear();

            try
            {
                /*
                 * อ่านไฟล์ใน Background Thread
                 * เพื่อไม่ให้หน้าต่างโปรแกรมค้างขณะอ่านไฟล์ใหญ่
                 */
                CsvLoadResult result = await Task.Run(() =>
                    ReadCsvFile(
                        tbFileName.Text,
                        startRecord,
                        endRecord,
                        fileTypeFilter));

                ShowCsvResult(result);

                if (result.Rows.Count == 0)
                {
                    lblResult.Text =
                        "ไม่พบข้อมูลที่ตรงกับเงื่อนไข";

                    MessageBox.Show(
                        "ไม่พบข้อมูลที่ตรงกับช่วงหรือประเภทไฟล์ที่กำหนด",
                        "No Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    lblResult.Text = "แสดง " +  result.Rows.Count.ToString("N0") + " รายการ จากช่วง " +
                        startRecord.ToString("N0") + " ถึง " + endRecord.ToString("N0");
                }
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "CSV Format Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text = "รูปแบบไฟล์ไม่ถูกต้อง";
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "ไม่สามารถเปิดไฟล์ได้\n" + ex.Message,
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text = "ไม่สามารถเปิดไฟล์ได้";
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "ไม่มีสิทธิ์เข้าถึงไฟล์\n" + ex.Message,
                    "Access Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text = "ไม่มีสิทธิ์เข้าถึงไฟล์";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "เกิดข้อผิดพลาด\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text = "เกิดข้อผิดพลาด";
            }
            finally
            {
                Cursor = Cursors.Default;
                btReadCSV.Enabled = true;
                btBrowse.Enabled = true;
            }
        }

        /// <summary>
        /// อ่าน CSV ทีละบรรทัด
        /// ไม่โหลดไฟล์ทั้งหมดเข้า RAM
        /// </summary>
        private CsvLoadResult ReadCsvFile(
            string filePath,
            int startRecord,
            int endRecord,
            string fileTypeFilter)
        {
            CsvLoadResult result = new CsvLoadResult();

            string[] headers = null;
            int fileTypeIndex = -1;
            int currentRecord = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    /*
                     * ไฟล์ตัวอย่างใช้ Header รูปแบบ:
                     * #HEADER: "first_seen_utc", ...
                     */
                    if (line.StartsWith(
                        "#HEADER:",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string headerText = line.Substring(8);

                        headers = ParseCsvLine(headerText);

                        for (int i = 0; i < headers.Length; i++)
                        {
                            headers[i] = CleanValue(headers[i]);
                        }

                        fileTypeIndex = FindColumnIndex(
                            headers,
                            "file_type_guess");

                        result.Headers = headers;

                        continue;
                    }

                    // ข้าม Comment อื่น ๆ
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] values = ParseCsvLine(line);

                    /*
                     * หากไฟล์ไม่มี #HEADER:
                     * โปรแกรมจะสร้างชื่อคอลัมน์อัตโนมัติ
                     */
                    if (headers == null)
                    {
                        headers = new string[values.Length];

                        for (int i = 0; i < headers.Length; i++)
                        {
                            headers[i] = "Column " + (i + 1);
                        }

                        // MalwareBazaar file_type_guess อยู่ตำแหน่ง 6
                        if (values.Length > 6)
                        {
                            fileTypeIndex = 6;
                        }

                        result.Headers = headers;
                    }

                    // นับเฉพาะบรรทัดข้อมูล ไม่รวม Header และ Comment
                    currentRecord++;

                    // ยังไม่ถึง Start Record
                    if (currentRecord < startRecord)
                    {
                        continue;
                    }

                    // เกิน End Record แล้ว หยุดอ่านทันที
                    if (currentRecord > endRecord)
                    {
                        break;
                    }

                    // กรองประเภทไฟล์
                    if (!string.IsNullOrWhiteSpace(fileTypeFilter))
                    {
                        if (fileTypeIndex < 0)
                        {
                            throw new InvalidDataException(
                                "ไม่พบคอลัมน์ file_type_guess ในไฟล์ CSV");
                        }

                        if (fileTypeIndex >= values.Length)
                        {
                            result.InvalidRowCount++;
                            continue;
                        }

                        string currentFileType =
                            CleanValue(values[fileTypeIndex]);

                        bool isMatched = string.Equals(
                            currentFileType,
                            fileTypeFilter,
                            StringComparison.OrdinalIgnoreCase);

                        if (!isMatched)
                        {
                            continue;
                        }
                    }

                    /*
                     * เพิ่ม Record Number เป็นคอลัมน์แรก
                     * ตามด้วยค่าจาก CSV
                     */
                    object[] rowData =
                        new object[headers.Length + 1];

                    rowData[0] = currentRecord;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (i < values.Length)
                        {
                            rowData[i + 1] =
                                CleanValue(values[i]);
                        }
                        else
                        {
                            rowData[i + 1] = "";
                        }
                    }

                    result.Rows.Add(rowData);
                }
            }

            result.LastRecordRead = currentRecord;

            return result;
        }

        /// <summary>
        /// นำผลลัพธ์ไปแสดงใน DataGridView
        /// </summary>
        private void ShowCsvResult(CsvLoadResult result)
        {
            dgvData.SuspendLayout();

            try
            {
                dgvData.Rows.Clear();
                dgvData.Columns.Clear();

                // คอลัมน์ลำดับข้อมูล
                dgvData.Columns.Add(
                    "colRecordNumber",
                    "Record No.");

                dgvData.Columns[0].Width = 90;

                // เพิ่มคอลัมน์จาก Header
                for (int i = 0; i < result.Headers.Length; i++)
                {
                    string columnName = "colData" + i;
                    string headerText = result.Headers[i];

                    dgvData.Columns.Add(
                        columnName,
                        headerText);

                    dgvData.Columns[i + 1].Width = 150;
                }

                // เพิ่มข้อมูลลงตาราง
                foreach (object[] row in result.Rows)
                {
                    dgvData.Rows.Add(row);
                }
            }
            finally
            {
                dgvData.ResumeLayout();
            }
        }

        /// <summary>
        /// ค้นหาตำแหน่งคอลัมน์จากชื่อ Header
        /// </summary>
        private int FindColumnIndex(
            string[] headers,
            string targetColumn)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (string.Equals(
                    CleanValue(headers[i]),
                    targetColumn,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// ลบช่องว่างและเครื่องหมายคำพูดรอบข้อมูล
        /// </summary>
        private string CleanValue(string value)
        {
            if (value == null)
            {
                return "";
            }

            return value.Trim().Trim('"');
        }

        /// <summary>
        /// แยก CSV โดยรองรับเครื่องหมายคำพูดและคอมมาภายในข้อมูล
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            List<string> values = new List<string>();
            StringBuilder currentValue = new StringBuilder();

            bool insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char currentCharacter = line[i];

                if (currentCharacter == '"')
                {
                    /*
                     * กรณีมีเครื่องหมายคำพูดซ้อน เช่น:
                     * "example ""test"" file"
                     */
                    if (insideQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (currentCharacter == ',' && !insideQuotes)
                {
                    values.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(currentCharacter);
                }
            }

            values.Add(currentValue.ToString().Trim());

            return values.ToArray();
        }

        /// <summary>
        /// เลือกไฟล์
        /// </summary>
        private void btBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog =
                   new OpenFileDialog())
            {
                openFileDialog.Filter =
                    "CSV Files (*.csv)|*.csv|" +
                    "Text Files (*.txt)|*.txt|" +
                    "All Files (*.*)|*.*";

                openFileDialog.Title =
                    "เลือกไฟล์ MalwareBazaar Dataset";

                if (openFileDialog.ShowDialog() ==
                    DialogResult.OK)
                {
                    tbFileName.Text =
                        openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// เก็บผลการอ่าน CSV
        /// </summary>
        private sealed class CsvLoadResult
        {
            public string[] Headers;
            public List<object[]> Rows;
            public int InvalidRowCount;
            public int LastRecordRead;

            public CsvLoadResult()
            {
                Headers = new string[0];
                Rows = new List<object[]>();
                InvalidRowCount = 0;
                LastRecordRead = 0;
            }
        }
    }
}