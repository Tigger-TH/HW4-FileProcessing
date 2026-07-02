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
        // หน้า Text แสดงข้อมูลได้ครั้งละไม่เกิน 500 บรรทัด
        private const int MaxTextRows = 500;

        private int textTotalLines = 0;
        private string countedTextFilePath = string.Empty;
        private bool updatingTextRange = false;

        public frmTextView()
        {
            InitializeComponent();

            // ตั้งค่า DataGridView หน้า CSV
            dgvData.AllowUserToAddRows = false;
            dgvData.ReadOnly = true;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvData.ColumnHeadersVisible = true;
            dgvData.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

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

            /*
             * ถ้าผูก ValueChanged ใน Designer ไว้แล้ว
             * บรรทัด -= จะป้องกันไม่ให้ Event ถูกเรียกซ้ำ
             */
            numTextStart.ValueChanged -= numTextStart_ValueChanged;
            numTextStart.ValueChanged += numTextStart_ValueChanged;

            numTextEnd.ValueChanged -= numTextEnd_ValueChanged;
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
                 * กรณีพิมพ์ Path เอง หรือ Path เปลี่ยนจากไฟล์ที่เคยนับ
                 * โปรแกรมจะนับจำนวนบรรทัดใหม่ก่อน
                 */
                if (!string.Equals(
                        countedTextFilePath,
                        filePath,
                        StringComparison.OrdinalIgnoreCase))
                {
                    lblTextResult.Text =
                        "กำลังตรวจสอบจำนวนบรรทัด...";

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

                if (startLine > textTotalLines ||
                    endLine > textTotalLines)
                {
                    MessageBox.Show(
                        "ช่วงที่เลือกเกินจำนวนบรรทัดของไฟล์",
                        "Invalid Range",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int selectedLineCount =
                    endLine - startLine + 1;

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
                    () => ReadTextRange(
                        filePath,
                        startLine,
                        endLine));

                rtbShow.Text = result.Content;

                if (result.LoadedLineCount == 0)
                {
                    lblTextResult.Text =
                        "ไม่พบข้อมูลในช่วงที่กำหนด";

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
                    "ไม่สามารถเปิดหรืออ่านไฟล์ได้\n" +
                    ex.Message,
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text =
                    "ไม่สามารถอ่านไฟล์ได้";
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "ไม่มีสิทธิ์เข้าถึงไฟล์\n" +
                    ex.Message,
                    "Access Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text =
                    "ไม่มีสิทธิ์เข้าถึงไฟล์";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "เกิดข้อผิดพลาด\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblTextResult.Text =
                    "เกิดข้อผิดพลาด";
            }
            finally
            {
                Cursor = Cursors.Default;
                btBrowse.Enabled = true;

                bool hasData = textTotalLines > 0;

                btRead.Enabled = hasData;
                numTextStart.Enabled = hasData;
                numTextEnd.Enabled = hasData;
            }
        }

        /// <summary>
        /// นับจำนวนบรรทัดทั้งหมดแบบไม่โหลดไฟล์ทั้งก้อนเข้า RAM
        /// </summary>
        private int CountTextLines(string filePath)
        {
            int lineCount = 0;

            using (StreamReader reader =
                   new StreamReader(filePath))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }

            return lineCount;
        }

        /// <summary>
        /// กำหนด Maximum ของ Start และ End
        /// ให้เท่ากับจำนวนบรรทัดจริงของไฟล์
        /// </summary>
        private void SetTextFileRange(
            string filePath,
            int totalLines)
        {
            updatingTextRange = true;

            try
            {
                textTotalLines = totalLines;
                countedTextFilePath = filePath;

                int maximumValue =
                    Math.Max(1, totalLines);

                numTextStart.Minimum = 1;
                numTextStart.Maximum = maximumValue;
                numTextStart.Value = 1;

                numTextEnd.Minimum = 1;
                numTextEnd.Maximum = maximumValue;

                if (totalLines > 0)
                {
                    numTextEnd.Value =
                        Math.Min(totalLines, MaxTextRows);

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

                    lblTextResult.Text =
                        "ไฟล์ไม่มีข้อมูล";
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
            TextLoadResult result =
                new TextLoadResult();

            StringBuilder content =
                new StringBuilder();

            int currentLineNumber = 0;

            using (StreamReader reader =
                   new StreamReader(filePath))
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
                    result.LastLineNumber =
                        currentLineNumber;
                }
            }

            result.Content = content.ToString();

            return result;
        }

        /// <summary>
        /// จำกัด End ไม่ให้ช่วงเกิน 500 บรรทัดเมื่อ Start เปลี่ยน
        /// </summary>
        private void numTextStart_ValueChanged(
            object sender,
            EventArgs e)
        {
            if (updatingTextRange ||
                textTotalLines <= 0)
            {
                return;
            }

            updatingTextRange = true;

            try
            {
                int startLine =
                    (int)numTextStart.Value;

                int endLine =
                    (int)numTextEnd.Value;

                int maximumAllowedEnd =
                    Math.Min(
                        textTotalLines,
                        startLine + MaxTextRows - 1);

                if (endLine < startLine)
                {
                    numTextEnd.Value =
                        startLine;
                }
                else if (endLine > maximumAllowedEnd)
                {
                    numTextEnd.Value =
                        maximumAllowedEnd;
                }

                ShowTextRangeInformation();
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        /// <summary>
        /// ป้องกัน End น้อยกว่า Start หรือช่วงเกิน 500 บรรทัด
        /// </summary>
        private void numTextEnd_ValueChanged(
            object sender,
            EventArgs e)
        {
            if (updatingTextRange ||
                textTotalLines <= 0)
            {
                return;
            }

            updatingTextRange = true;

            try
            {
                int startLine =
                    (int)numTextStart.Value;

                int endLine =
                    (int)numTextEnd.Value;

                if (endLine < startLine)
                {
                    numTextEnd.Value =
                        startLine;

                    endLine = startLine;
                }

                int maximumAllowedEnd =
                    Math.Min(
                        textTotalLines,
                        startLine + MaxTextRows - 1);

                if (endLine > maximumAllowedEnd)
                {
                    numTextEnd.Value =
                        maximumAllowedEnd;
                }

                ShowTextRangeInformation();
            }
            finally
            {
                updatingTextRange = false;
            }
        }

        /// <summary>
        /// แสดงจำนวนบรรทัดในช่วงที่เลือก
        /// </summary>
        private void ShowTextRangeInformation()
        {
            if (textTotalLines <= 0)
            {
                return;
            }

            int startLine =
                (int)numTextStart.Value;

            int endLine =
                (int)numTextEnd.Value;

            int selectedCount =
                endLine - startLine + 1;

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

        /// <summary>
        /// ล้างสถานะของหน้า Text
        /// </summary>
        private void ResetTextFileInformation()
        {
            updatingTextRange = true;

            try
            {
                textTotalLines = 0;
                countedTextFilePath =
                    string.Empty;

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
        // CSV FILE
        // =========================================================

        /// <summary>
        /// โหลด CSV ตามช่วง m-n และกรองตามประเภทไฟล์
        /// </summary>
        private async void btReadCSV_Click(
            object sender,
            EventArgs e)
        {
            string filePath =
                tbFileName.Text.Trim();

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

            int startRecord =
                (int)numStartRecord.Value;

            int endRecord =
                (int)numEndRecord.Value;

            if (endRecord < startRecord)
            {
                MessageBox.Show(
                    "End Record ต้องไม่น้อยกว่า Start Record",
                    "Invalid Range",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string fileTypeFilter =
                txtFileType.Text.Trim();

            btReadCSV.Enabled = false;
            btBrowse.Enabled = false;

            lblResult.Text =
                "กำลังโหลดข้อมูล...";

            Cursor = Cursors.WaitCursor;

            dgvData.Rows.Clear();
            dgvData.Columns.Clear();

            try
            {
                CsvLoadResult result =
                    await Task.Run(
                        () => ReadCsvFile(
                            filePath,
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

                lblResult.Text =
                    "รูปแบบไฟล์ไม่ถูกต้อง";
            }
            catch (IOException ex)
            {
                MessageBox.Show(
                    "ไม่สามารถเปิดไฟล์ได้\n" +
                    ex.Message,
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text =
                    "ไม่สามารถเปิดไฟล์ได้";
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "ไม่มีสิทธิ์เข้าถึงไฟล์\n" +
                    ex.Message,
                    "Access Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text =
                    "ไม่มีสิทธิ์เข้าถึงไฟล์";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "เกิดข้อผิดพลาด\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblResult.Text =
                    "เกิดข้อผิดพลาด";
            }
            finally
            {
                Cursor = Cursors.Default;
                btReadCSV.Enabled = true;
                btBrowse.Enabled = true;
            }
        }

        /// <summary>
        /// อ่าน CSV ทีละบรรทัด และใช้ชื่อคอลัมน์จาก Header ในไฟล์
        /// รองรับ Header รูปแบบ:
        /// # "first_seen_utc","sha256_hash",...
        /// #HEADER: "first_seen_utc","sha256_hash",...
        /// "first_seen_utc","sha256_hash",...
        /// </summary>
        private CsvLoadResult ReadCsvFile(
            string filePath,
            int startRecord,
            int endRecord,
            string fileTypeFilter)
        {
            CsvLoadResult result =
                new CsvLoadResult();

            string[] headers = null;
            int fileTypeIndex = -1;
            int currentRecord = 0;

            using (StreamReader reader =
                   new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string normalizedLine =
                        line.TrimStart(
                            '\uFEFF',
                            ' ',
                            '\t');

                    /*
                     * ต้องตรวจ Header ก่อนข้าม Comment
                     * เพราะ Header ของไฟล์จริงขึ้นต้นด้วย #
                     */
                    if (normalizedLine.StartsWith("#"))
                    {
                        string possibleHeaderText =
                            normalizedLine
                                .Substring(1)
                                .Trim();

                        if (possibleHeaderText.StartsWith(
                            "HEADER:",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            possibleHeaderText =
                                possibleHeaderText
                                    .Substring("HEADER:".Length)
                                    .Trim();
                        }

                        string[] possibleHeaders =
                            ParseCsvLine(
                                possibleHeaderText);

                        CleanValues(possibleHeaders);

                        if (IsMalwareBazaarHeader(
                                possibleHeaders))
                        {
                            headers =
                                possibleHeaders;

                            fileTypeIndex =
                                FindColumnIndex(
                                    headers,
                                    "file_type_guess");

                            result.Headers =
                                headers;

                            // Header ไม่นับเป็น Record
                            continue;
                        }

                        // Comment ทั่วไป
                        continue;
                    }

                    string[] values =
                        ParseCsvLine(normalizedLine);

                    /*
                     * รองรับ Header แบบ CSV ปกติที่ไม่ขึ้นต้นด้วย #
                     */
                    if (headers == null)
                    {
                        string[] possibleHeaders =
                            CopyAndCleanValues(values);

                        if (IsMalwareBazaarHeader(
                                possibleHeaders))
                        {
                            headers =
                                possibleHeaders;

                            fileTypeIndex =
                                FindColumnIndex(
                                    headers,
                                    "file_type_guess");

                            result.Headers =
                                headers;

                            continue;
                        }

                        /*
                         * ไม่สร้าง Column 1, Column 2
                         * หากไม่พบ Header ให้แจ้ง Error
                         */
                        throw new InvalidDataException(
                            "ไม่พบ Header ของไฟล์ CSV\n" +
                            "Header ต้องมีคอลัมน์ first_seen_utc, " +
                            "sha256_hash และ file_type_guess");
                    }

                    currentRecord++;

                    if (currentRecord < startRecord)
                    {
                        continue;
                    }

                    if (currentRecord > endRecord)
                    {
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(
                            fileTypeFilter))
                    {
                        if (fileTypeIndex < 0)
                        {
                            throw new InvalidDataException(
                                "ไม่พบคอลัมน์ file_type_guess " +
                                "ในไฟล์ CSV");
                        }

                        if (fileTypeIndex >= values.Length)
                        {
                            result.InvalidRowCount++;
                            continue;
                        }

                        string currentFileType =
                            CleanValue(
                                values[fileTypeIndex]);

                        bool isMatched =
                            string.Equals(
                                currentFileType,
                                fileTypeFilter,
                                StringComparison.OrdinalIgnoreCase);

                        if (!isMatched)
                        {
                            continue;
                        }
                    }

                    object[] rowData =
                        new object[headers.Length + 1];

                    rowData[0] =
                        currentRecord;

                    for (int i = 0;
                         i < headers.Length;
                         i++)
                    {
                        if (i < values.Length)
                        {
                            rowData[i + 1] =
                                CleanValue(values[i]);
                        }
                        else
                        {
                            rowData[i + 1] =
                                string.Empty;
                        }
                    }

                    result.Rows.Add(rowData);
                }
            }

            if (headers == null ||
                headers.Length == 0)
            {
                throw new InvalidDataException(
                    "ไม่พบ Header ภายในไฟล์ CSV");
            }

            result.LastRecordRead =
                currentRecord;

            return result;
        }

        /// <summary>
        /// ตรวจว่าเป็น Header ของ MalwareBazaar หรือไม่
        /// </summary>
        private bool IsMalwareBazaarHeader(
            string[] values)
        {
            if (values == null ||
                values.Length == 0)
            {
                return false;
            }

            return
                FindColumnIndex(
                    values,
                    "first_seen_utc") >= 0 &&
                FindColumnIndex(
                    values,
                    "sha256_hash") >= 0 &&
                FindColumnIndex(
                    values,
                    "file_type_guess") >= 0;
        }

        /// <summary>
        /// ทำความสะอาดค่าทุกช่องใน Array
        /// </summary>
        private void CleanValues(
            string[] values)
        {
            if (values == null)
            {
                return;
            }

            for (int i = 0;
                 i < values.Length;
                 i++)
            {
                values[i] =
                    CleanValue(values[i]);
            }
        }

        /// <summary>
        /// คัดลอกและทำความสะอาดค่าจาก Array
        /// </summary>
        private string[] CopyAndCleanValues(
            string[] values)
        {
            string[] result =
                new string[values.Length];

            for (int i = 0;
                 i < values.Length;
                 i++)
            {
                result[i] =
                    CleanValue(values[i]);
            }

            return result;
        }

        /// <summary>
        /// แสดง CSV ใน DataGridView
        /// โดยใช้ชื่อคอลัมน์จากไฟล์เท่านั้น
        /// </summary>
        private void ShowCsvResult(
            CsvLoadResult result)
        {
            dgvData.SuspendLayout();

            try
            {
                dgvData.Rows.Clear();
                dgvData.Columns.Clear();
                dgvData.ColumnHeadersVisible = true;

                if (result.Headers == null ||
                    result.Headers.Length == 0)
                {
                    throw new InvalidDataException(
                        "ไม่พบชื่อคอลัมน์จากไฟล์ CSV");
                }

                // คอลัมน์นี้โปรแกรมเพิ่มเองเพื่อแสดงลำดับ Record
                DataGridViewTextBoxColumn recordColumn =
                    new DataGridViewTextBoxColumn();

                recordColumn.Name =
                    "colRecordNumber";

                recordColumn.HeaderText =
                    "Record No.";

                recordColumn.Width = 100;

                recordColumn.SortMode =
                    DataGridViewColumnSortMode.NotSortable;

                dgvData.Columns.Add(recordColumn);

                // ใช้ชื่อ Header ที่อ่านจากไฟล์ CSV
                for (int i = 0;
                     i < result.Headers.Length;
                     i++)
                {
                    string headerText =
                        CleanValue(
                            result.Headers[i]);

                    if (string.IsNullOrWhiteSpace(
                            headerText))
                    {
                        throw new InvalidDataException(
                            "พบชื่อคอลัมน์ว่างที่ตำแหน่ง " +
                            (i + 1));
                    }

                    DataGridViewTextBoxColumn column =
                        new DataGridViewTextBoxColumn();

                    column.Name =
                        "colData" + i;

                    column.HeaderText =
                        headerText;

                    column.Width = 160;

                    column.SortMode =
                        DataGridViewColumnSortMode.NotSortable;

                    dgvData.Columns.Add(column);
                }

                foreach (object[] row
                         in result.Rows)
                {
                    dgvData.Rows.Add(row);
                }

                dgvData.AutoResizeColumnHeadersHeight();
            }
            finally
            {
                dgvData.ResumeLayout();
            }
        }

        /// <summary>
        /// ค้นหา Index ของชื่อคอลัมน์
        /// </summary>
        private int FindColumnIndex(
            string[] headers,
            string targetColumn)
        {
            if (headers == null)
            {
                return -1;
            }

            for (int i = 0;
                 i < headers.Length;
                 i++)
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
        /// ลบ BOM ช่องว่าง และเครื่องหมายคำพูดรอบข้อมูล
        /// </summary>
        private string CleanValue(
            string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value
                .Trim()
                .TrimStart('\uFEFF')
                .Trim()
                .Trim('"')
                .Trim();
        }

        /// <summary>
        /// แยก CSV โดยรองรับคอมมาภายในเครื่องหมายคำพูด
        /// </summary>
        private string[] ParseCsvLine(
            string line)
        {
            List<string> values =
                new List<string>();

            StringBuilder currentValue =
                new StringBuilder();

            bool insideQuotes = false;

            for (int i = 0;
                 i < line.Length;
                 i++)
            {
                char currentCharacter =
                    line[i];

                if (currentCharacter == '"')
                {
                    if (insideQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes =
                            !insideQuotes;
                    }
                }
                else if (
                    currentCharacter == ',' &&
                    !insideQuotes)
                {
                    values.Add(
                        currentValue
                            .ToString()
                            .Trim());

                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(
                        currentCharacter);
                }
            }

            values.Add(
                currentValue
                    .ToString()
                    .Trim());

            return values.ToArray();
        }

        // =========================================================
        // BROWSE FILE
        // =========================================================

        /// <summary>
        /// เลือกไฟล์ และนับจำนวนบรรทัดสำหรับหน้า Text
        /// </summary>
        private async void btBrowse_Click(
            object sender,
            EventArgs e)
        {
            using (OpenFileDialog openFileDialog =
                   new OpenFileDialog())
            {
                openFileDialog.Filter =
                    "CSV Files (*.csv)|*.csv|" +
                    "Text Files (*.txt)|*.txt|" +
                    "All Files (*.*)|*.*";

                openFileDialog.Title =
                    "เลือกไฟล์ข้อมูล";

                if (openFileDialog.ShowDialog() !=
                    DialogResult.OK)
                {
                    return;
                }

                string selectedFile =
                    openFileDialog.FileName;

                tbFileName.Text =
                    selectedFile;

                btBrowse.Enabled = false;
                btRead.Enabled = false;
                numTextStart.Enabled = false;
                numTextEnd.Enabled = false;

                lblTextResult.Text =
                    "กำลังตรวจสอบจำนวนบรรทัด...";

                Cursor = Cursors.WaitCursor;

                try
                {
                    int totalLines =
                        await Task.Run(
                            () => CountTextLines(
                                selectedFile));

                    SetTextFileRange(
                        selectedFile,
                        totalLines);
                }
                catch (IOException ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "ไม่สามารถอ่านไฟล์ได้\n" +
                        ex.Message,
                        "File Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text =
                        "ไม่สามารถอ่านไฟล์ได้";
                }
                catch (UnauthorizedAccessException ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "ไม่มีสิทธิ์เข้าถึงไฟล์\n" +
                        ex.Message,
                        "Access Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text =
                        "ไม่มีสิทธิ์เข้าถึงไฟล์";
                }
                catch (Exception ex)
                {
                    ResetTextFileInformation();

                    MessageBox.Show(
                        "เกิดข้อผิดพลาด\n" +
                        ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    lblTextResult.Text =
                        "เกิดข้อผิดพลาด";
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btBrowse.Enabled = true;
                }
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
                Headers =
                    new string[0];

                Rows =
                    new List<object[]>();

                InvalidRowCount = 0;
                LastRecordRead = 0;
            }
        }
    }
}
