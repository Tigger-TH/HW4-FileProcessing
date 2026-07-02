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
        // หน้า Text แสดงข้อมูลได้ครั้งละไม่เกิน 1000 บรรทัด
        private const int MaxTextRows = 1000;

        // เก็บจำนวนบรรทัดทั้งหมดและไฟล์ที่นับล่าสุด
        private int textTotalLines = 0;
        private string countedTextFilePath = string.Empty;

        // ป้องกัน ValueChanged ทำงานซ้ำระหว่างที่โปรแกรมกำลังปรับค่า
        private bool updatingTextRange = false;

        public frmTextView()
        {
            InitializeComponent();

            // ตั้งค่า DataGridView
            dgvData.AllowUserToAddRows = false;
            dgvData.ReadOnly = true;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // ตั้งค่าหน้า Text ก่อนเลือกไฟล์
            numTextStart.Minimum = 1;
            numTextStart.Maximum = 1;
            numTextStart.Value = 1;
            numTextStart.Enabled = false;

            numTextEnd.Minimum = 1;
            numTextEnd.Maximum = 1;
            numTextEnd.Value = 1;
            numTextEnd.Enabled = false;

            btRead.Enabled = false;
            lblTextResult.Text = "กรุณาเลือกไฟล์";

            // ผูก Event ในโค้ด จึงไม่จำเป็นต้องผูก ValueChanged ใน Designer
            numTextStart.ValueChanged += numTextStart_ValueChanged;
            numTextEnd.ValueChanged += numTextEnd_ValueChanged;
        }

        // =========================================================
        // TEXT FILE
        // =========================================================

        /// <summary>
        /// อ่านไฟล์ Text เฉพาะช่วงบรรทัดที่กำหนด
        /// </summary>
        private async void btRead_Click(object sender, EventArgs e)
        {
            string filePath = tbFileName.Text.Trim();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show(
                    "กรุณาเลือกไฟล์ก่อน",
                    "ไม่พบไฟล์",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!File.Exists(filePath))
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
                /*
                 * หากผู้ใช้พิมพ์ Path เอง หรือเปลี่ยน Path หลังจากกด Browse
                 * ให้โปรแกรมนับจำนวนบรรทัดใหม่ก่อน
                 */
                if (!string.Equals(
                        countedTextFilePath,filePath,StringComparison.OrdinalIgnoreCase))
                {
                    lblTextResult.Text = "กำลังตรวจสอบจำนวนบรรทัด...";
                    btRead.Enabled = false;
                    Cursor = Cursors.WaitCursor;

                    int totalLines = await Task.Run(
                        () => CountTextLines(filePath));

                    SetTextFileRange(filePath, totalLines);
                }

                if (textTotalLines <= 0)
                {
                    MessageBox.Show(
                        "ไฟล์ไม่มีข้อมูล",
                        "Empty File",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                int startLine = (int)numTextStart.Value;
                int endLine = (int)numTextEnd.Value;

                if (endLine < startLine)
                {
                    MessageBox.Show(
                        "End Line ต้องไม่น้อยกว่า Start Line",
                        "Invalid Range",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (startLine > textTotalLines || endLine > textTotalLines)
                {
                    MessageBox.Show(
                        "ช่วงที่เลือกเกินจำนวนบรรทัดของไฟล์",
                        "Invalid Range",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int selectedLineCount = endLine - startLine + 1;

                if (selectedLineCount > MaxTextRows)
                {
                    MessageBox.Show(
                        "สามารถแสดงข้อมูลได้ครั้งละไม่เกิน " +
                        MaxTextRows.ToString("N0") +
                        " บรรทัด",
                        "Range Too Large",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                btRead.Enabled = false;
                btBrowse.Enabled = false;
                numTextStart.Enabled = false;
                numTextEnd.Enabled = false;

                Cursor = Cursors.WaitCursor;
                rtbShow.Clear();
                lblTextResult.Text = "กำลังอ่านข้อมูล...";

                TextLoadResult result = await Task.Run(
                    () => ReadTextRange(filePath, startLine, endLine));

                rtbShow.Text = result.Content;

                if (result.LoadedLineCount == 0)
                {
                    lblTextResult.Text = "ไม่พบข้อมูลในช่วงที่กำหนด";

                    MessageBox.Show(
                        "ไม่พบข้อมูลในช่วงบรรทัดที่กำหนด",
                        "No Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    lblTextResult.Text =
                        "แสดง " +
                        result.LoadedLineCount.ToString("N0") +
                        " บรรทัด จากบรรทัด " +
                        startLine.ToString("N0") +
                        " ถึง " +
                        result.LastLineNumber.ToString("N0") +
                        " | ทั้งไฟล์ " +
                        textTotalLines.ToString("N0") +
                        " บรรทัด";
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "ไม่สามารถเปิดหรืออ่านไฟล์ได้\n" + ex.Message,
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text = "ไม่สามารถอ่านไฟล์ได้";
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "ไม่มีสิทธิ์เข้าถึงไฟล์\n" + ex.Message,
                    "Access Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text = "ไม่มีสิทธิ์เข้าถึงไฟล์";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "เกิดข้อผิดพลาด\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text = "เกิดข้อผิดพลาด";
            }
            finally
            {
                Cursor = Cursors.Default;
                btBrowse.Enabled = true;

                bool hasTextData = textTotalLines > 0;
                btRead.Enabled = hasTextData;
                numTextStart.Enabled = hasTextData;
                numTextEnd.Enabled = hasTextData;
            }
        }

        /// <summary>
        /// นับจำนวนบรรทัดทั้งหมดของไฟล์แบบอ่านทีละบรรทัด
        /// </summary>
        private int CountTextLines(string filePath)
        {
            int lineCount = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }

            return lineCount;
        }

        /// <summary>
        /// ตั้งค่า Maximum ของ NumericUpDown ให้เท่ากับจำนวนบรรทัดของไฟล์
        /// และกำหนดช่วงเริ่มต้นไม่เกิน 500 บรรทัด
        /// </summary>
        private void SetTextFileRange(string filePath, int totalLines)
        {
            updatingTextRange = true;

            try
            {
                textTotalLines = totalLines;
                countedTextFilePath = filePath;

                int maximumValue = Math.Max(1, totalLines);

                numTextStart.Minimum = 1;
                numTextStart.Maximum = maximumValue;
                numTextStart.Value = 1;

                numTextEnd.Minimum = 1;
                numTextEnd.Maximum = maximumValue;

                if (totalLines > 0)
                {
                    numTextEnd.Value = Math.Min(totalLines, MaxTextRows);

                    numTextStart.Enabled = true;
                    numTextEnd.Enabled = true;
                    btRead.Enabled = true;

                    lblTextResult.Text =
                        "ไฟล์มีทั้งหมด " +
                        totalLines.ToString("N0") +
                        " บรรทัด | เลือกได้ครั้งละไม่เกิน " +
                        MaxTextRows.ToString("N0") +
                        " บรรทัด";
                }
                else
                {
                    numTextEnd.Value = 1;

                    numTextStart.Enabled = false;
                    numTextEnd.Enabled = false;
                    btRead.Enabled = false;

                    lblTextResult.Text = "ไฟล์ไม่มีข้อมูล";
                }
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        /// <summary>
        /// อ่านเฉพาะบรรทัด Start Line ถึง End Line
        /// </summary>
        private TextLoadResult ReadTextRange(
            string filePath,
            int startLine,
            int endLine)
        {
            TextLoadResult result = new TextLoadResult();
            StringBuilder content = new StringBuilder();

            int currentLineNumber = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    currentLineNumber++;

                    if (currentLineNumber < startLine)
                    {
                        continue;
                    }

                    if (currentLineNumber > endLine)
                    {
                        break;
                    }

                    content.AppendLine(line);
                    result.LoadedLineCount++;
                    result.LastLineNumber = currentLineNumber;
                }
            }

            result.Content = content.ToString();

            return result;
        }

        /// <summary>
        /// เมื่อ Start Line เปลี่ยน ให้ตรวจว่า End Line ห่างไม่เกิน 500 บรรทัด
        /// </summary>
        private void numTextStart_ValueChanged(object sender, EventArgs e)
        {
            if (updatingTextRange || textTotalLines <= 0)
            {
                return;
            }

            updatingTextRange = true;

            try
            {
                int startLine = (int)numTextStart.Value;
                int endLine = (int)numTextEnd.Value;

                int maximumAllowedEnd = Math.Min(
                    textTotalLines,
                    startLine + MaxTextRows - 1);

                if (endLine < startLine)
                {
                    numTextEnd.Value = startLine;
                }
                else if (endLine > maximumAllowedEnd)
                {
                    numTextEnd.Value = maximumAllowedEnd;
                }

                ShowTextRangeInformation();
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        /// <summary>
        /// เมื่อ End Line เปลี่ยน ให้ตรวจว่าไม่น้อยกว่า Start และช่วงไม่เกิน 500
        /// </summary>
        private void numTextEnd_ValueChanged(object sender, EventArgs e)
        {
            if (updatingTextRange || textTotalLines <= 0)
            {
                return;
            }

            updatingTextRange = true;

            try
            {
                int startLine = (int)numTextStart.Value;
                int endLine = (int)numTextEnd.Value;

                if (endLine < startLine)
                {
                    numTextEnd.Value = startLine;
                    endLine = startLine;
                }

                int maximumAllowedEnd = Math.Min(
                    textTotalLines,
                    startLine + MaxTextRows - 1);

                if (endLine > maximumAllowedEnd)
                {
                    numTextEnd.Value = maximumAllowedEnd;
                }

                ShowTextRangeInformation();
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        /// <summary>
        /// แสดงรายละเอียดช่วงบรรทัดที่เลือก
        /// </summary>
        private void ShowTextRangeInformation()
        {
            if (textTotalLines <= 0)
            {
                return;
            }

            int startLine = (int)numTextStart.Value;
            int endLine = (int)numTextEnd.Value;
            int selectedCount = endLine - startLine + 1;

            lblTextResult.Text =
                "เลือกบรรทัด " +
                startLine.ToString("N0") +
                " ถึง " +
                endLine.ToString("N0") +
                " รวม " +
                selectedCount.ToString("N0") +
                " บรรทัด | ทั้งไฟล์ " +
                textTotalLines.ToString("N0") +
                " บรรทัด";
        }

        // =========================================================
        // CSV FILE
        // =========================================================

        /// <summary>
        /// โหลด CSV ตามช่วง m-n และกรองตามประเภทไฟล์
        /// </summary>
        private async void btReadCSV_Click(object sender, EventArgs e)
        {
            string filePath = tbFileName.Text.Trim();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show(
                    "กรุณาเลือกไฟล์ CSV ก่อน",
                    "ไม่พบไฟล์",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!File.Exists(filePath))
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

            btReadCSV.Enabled = false;
            btBrowse.Enabled = false;

            lblResult.Text = "กำลังโหลดข้อมูล...";
            Cursor = Cursors.WaitCursor;

            dgvData.Rows.Clear();
            dgvData.Columns.Clear();

            try
            {
                CsvLoadResult result = await Task.Run(
                    () => ReadCsvFile(
                        filePath,
                        startRecord,
                        endRecord,
                        fileTypeFilter));

                ShowCsvResult(result);

                if (result.Rows.Count == 0)
                {
                    lblResult.Text = "ไม่พบข้อมูลที่ตรงกับเงื่อนไข";

                    MessageBox.Show(
                        "ไม่พบข้อมูลที่ตรงกับช่วงหรือประเภทไฟล์ที่กำหนด",
                        "No Result",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    lblResult.Text =
                        "แสดง " +
                        result.Rows.Count.ToString("N0") +
                        " รายการ จากช่วง " +
                        startRecord.ToString("N0") +
                        " ถึง " +
                        endRecord.ToString("N0");

                    if (result.InvalidRowCount > 0)
                    {
                        lblResult.Text +=
                            " | ข้ามข้อมูลผิดรูปแบบ " +
                            result.InvalidRowCount.ToString("N0") +
                            " รายการ";
                    }
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
        /// อ่าน CSV ทีละบรรทัด ไม่โหลดไฟล์ทั้งหมดเข้า RAM
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

                    // Header แบบที่ใช้ในไฟล์ตัวอย่างของโปรเจกต์
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

                    // ข้าม Comment
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] values = ParseCsvLine(line);

                    /*
                     * รองรับ CSV ที่ใช้ Header ปกติในบรรทัดแรก
                     * โดยตรวจหาชื่อคอลัมน์ file_type_guess
                     */
                    if (headers == null)
                    {
                        string[] possibleHeaders = new string[values.Length];

                        for (int i = 0; i < values.Length; i++)
                        {
                            possibleHeaders[i] = CleanValue(values[i]);
                        }

                        int possibleFileTypeIndex = FindColumnIndex(
                            possibleHeaders,
                            "file_type_guess");

                        if (possibleFileTypeIndex >= 0)
                        {
                            headers = possibleHeaders;
                            fileTypeIndex = possibleFileTypeIndex;
                            result.Headers = headers;
                            continue;
                        }

                        // หากไม่มี Header ให้สร้างชื่อคอลัมน์อัตโนมัติ
                        headers = new string[values.Length];

                        for (int i = 0; i < headers.Length; i++)
                        {
                            headers[i] = "Column " + (i + 1);
                        }

                        // MalwareBazaar file_type_guess ปกติอยู่ตำแหน่ง Index 6
                        if (values.Length > 6)
                        {
                            fileTypeIndex = 6;
                        }

                        result.Headers = headers;
                    }

                    // นับเฉพาะ Record ข้อมูล ไม่รวม Header, Comment และบรรทัดว่าง
                    currentRecord++;

                    if (currentRecord < startRecord)
                    {
                        continue;
                    }

                    if (currentRecord > endRecord)
                    {
                        break;
                    }

                    if (values.Length == 0)
                    {
                        result.InvalidRowCount++;
                        continue;
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

                    object[] rowData = new object[headers.Length + 1];
                    rowData[0] = currentRecord;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (i < values.Length)
                        {
                            rowData[i + 1] = CleanValue(values[i]);
                        }
                        else
                        {
                            rowData[i + 1] = string.Empty;
                        }
                    }

                    result.Rows.Add(rowData);
                }
            }

            result.LastRecordRead = currentRecord;

            return result;
        }

        /// <summary>
        /// นำผลลัพธ์ CSV ไปแสดงใน DataGridView
        /// </summary>
        private void ShowCsvResult(CsvLoadResult result)
        {
            dgvData.SuspendLayout();

            try
            {
                dgvData.Rows.Clear();
                dgvData.Columns.Clear();

                dgvData.Columns.Add(
                    "colRecordNumber",
                    "Record No.");

                dgvData.Columns[0].Width = 90;

                for (int i = 0; i < result.Headers.Length; i++)
                {
                    string columnName = "colData" + i;
                    string headerText = result.Headers[i];

                    dgvData.Columns.Add(columnName, headerText);
                    dgvData.Columns[i + 1].Width = 150;
                }

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
                return string.Empty;
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
                    // เครื่องหมายคำพูดซ้อน เช่น "example ""test"" file"
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

        // =========================================================
        // FILE BROWSE
        // =========================================================

        /// <summary>
        /// เลือกไฟล์และนับจำนวนบรรทัดสำหรับหน้า Text
        /// </summary>
        private async void btBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter =
                    "CSV Files (*.csv)|*.csv|" +
                    "Text Files (*.txt)|*.txt|" +
                    "All Files (*.*)|*.*";

                openFileDialog.Title = "เลือกไฟล์ข้อมูล";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string selectedFile = openFileDialog.FileName;
                tbFileName.Text = selectedFile;

                btBrowse.Enabled = false;
                btRead.Enabled = false;
                numTextStart.Enabled = false;
                numTextEnd.Enabled = false;

                lblTextResult.Text = "กำลังตรวจสอบจำนวนบรรทัด...";
                Cursor = Cursors.WaitCursor;

                try
                {
                    int totalLines = await Task.Run(
                        () => CountTextLines(selectedFile));

                    SetTextFileRange(selectedFile, totalLines);
                }
                catch (IOException ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "ไม่สามารถอ่านไฟล์ได้\n" + ex.Message,
                        "File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text = "ไม่สามารถอ่านไฟล์ได้";
                }
                catch (UnauthorizedAccessException ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "ไม่มีสิทธิ์เข้าถึงไฟล์\n" + ex.Message,
                        "Access Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text = "ไม่มีสิทธิ์เข้าถึงไฟล์";
                }
                catch (Exception ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "เกิดข้อผิดพลาด\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text = "เกิดข้อผิดพลาด";
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btBrowse.Enabled = true;
                }
            }
        }

        /// <summary>
        /// ล้างข้อมูลการนับบรรทัดเมื่อเลือกไฟล์ไม่สำเร็จ
        /// </summary>
        private void ResetTextFileInformation()
        {
            updatingTextRange = true;

            try
            {
                textTotalLines = 0;
                countedTextFilePath = string.Empty;

                numTextStart.Minimum = 1;
                numTextStart.Maximum = 1;
                numTextStart.Value = 1;
                numTextStart.Enabled = false;

                numTextEnd.Minimum = 1;
                numTextEnd.Maximum = 1;
                numTextEnd.Value = 1;
                numTextEnd.Enabled = false;

                btRead.Enabled = false;
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        // =========================================================
        // RESULT CLASSES
        // =========================================================

        private sealed class TextLoadResult
        {
            public string Content;
            public int LoadedLineCount;
            public int LastLineNumber;

            public TextLoadResult()
            {
                Content = string.Empty;
                LoadedLineCount = 0;
                LastLineNumber = 0;
            }
        }

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
