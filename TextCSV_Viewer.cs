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
using System.IO;
using System.Windows.Forms;

namespace FileProcessing
{
	public partial class frmTextView : Form
	{
		/// <summary>
		/// Initializes a new instance of the frmTextView class.
		/// </summary>
		public frmTextView()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Handles the Click event of the Read button by loading the contents of the specified file into the display area.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void btRead_Click(object sender, EventArgs e)
		{			
            string content = File.ReadAllText(tbFileName.Text);
            rtbShow.Text = content;
		}
        /// <summary>
        /// Handles the Click event of the btReadCSV button, reading CSV data from the specified file and populating the
        /// DataGridView with its contents.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
		private void btReadCSV_Click(object sender, EventArgs e)
		{
            using (StreamReader srReader = new StreamReader(tbFileName.Text))
            {
                string strLine; // Variable to hold each line read from the file
				bool bHeaderRead = false;   // Flag to indicate whether the header line has been read

				// Main loop: Read the file line by line
				while ((strLine = srReader.ReadLine()) != null)
                {
                    string[] strHeaders_arr = null;
					// Skip comment lines and check for header line
					if (strLine.StartsWith("#")) 
                    { 
                        if (    strLine.Length > 8
                           &&   strLine.Substring(0, 8).Equals("#HEADER") 
                           )
                        {
							// Read the header line and split it into an array of headers
							strHeaders_arr = strLine.Substring(8).Split(',');
						}
                        continue;
                    }
					// Split the current line into an array of values
					string[] strValues_arr = strLine.Split(',');

					// If the header has not been read yet, add the headers to the DataGridView columns
					if (!bHeaderRead)
                    {
						// Add the headers to the DataGridView columns, using the header names from the header line if available
						foreach (string strHeader in strValues_arr)
                        {
                            if ( strHeaders_arr == null )
                                dgvData.Columns.Add(strHeader.Trim(), strHeader.Trim());
                            else
                                dgvData.Columns.Add(strHeader.Trim(), strHeaders_arr[dgvData.Columns.Count].Trim());
						}
                        bHeaderRead = true;
                    }
                    else
                    {
						// Add the values to the DataGridView rows
						dgvData.Rows.Add(strValues_arr);
                    }
				}   // Main loop: Read the file line by line
			}

		}
		/// <summary>
		/// Handles the Click event of the Browse button, allowing the user to select a file and displaying its path in the
		/// file name text box.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void btBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tbFileName.Text = ofd.FileName;
				}
			}
		}
	}   // End of frmTextView class
}
