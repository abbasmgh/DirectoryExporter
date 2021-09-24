using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace DirectoryToExcel
{
    public partial class Form1 : Form
    {
        private object missing = Type.Missing;

        public Form1()
        {
            InitializeComponent();

            if (Directory.Exists(path: "Data"))
            {
                var directory = new DirectoryInfo(path: "Data");
                if (directory.GetFiles().Any())
                {
                    var myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();

                    var json = File.ReadAllText(path: "Data\\" + myFile.Name);
                    jsonTreeView.ShowJson(jsonString: json);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var folderDlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            var result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                var path = folderDlg.SelectedPath;

                var folders = Directory.GetDirectories(path: path);
                var files = Directory.GetFiles(path: path);

                var oXL = new Application
                {
                    Visible = false
                };
                var oWB = oXL.Workbooks.Add(Template: missing);

                var index = 1;
                foreach (var folder in folders)
                {
                    Worksheet oSheet;
                    if (index == 1)
                        oSheet = oWB.ActiveSheet as Worksheet;
                    else
                        oSheet = oWB.Sheets.Add(Before: missing, After: missing, Count: 1, Type: missing) as Worksheet;

                    oSheet.Name = Path.GetFileName(path: folder);

                    var items = Directory.GetDirectories(path: folder);
                    var i = 1;
                    foreach (var item in items)
                    {
                        oSheet.Cells[RowIndex: i, ColumnIndex: 1] = Path.GetFileName(path: item);
                        i++;
                    }

                    index++;
                }

                MessageBox.Show(text: "Excel Generated Successfull!");

                var fileName = "d:\\Downloads\\Movie-Excel.xlsx";
                oWB.SaveAs(
                    Filename: fileName,
                    FileFormat: XlFileFormat.xlOpenXMLWorkbook,
                    Password: missing,
                    WriteResPassword: missing,
                    ReadOnlyRecommended: missing,
                    CreateBackup: missing,
                    AccessMode: XlSaveAsAccessMode.xlNoChange,
                    ConflictResolution: missing,
                    AddToMru: missing,
                    TextCodepage: missing,
                    TextVisualLayout: missing,
                    Local: missing);
                oWB.Close(SaveChanges: missing, Filename: missing, RouteWorkbook: missing);
                oXL.UserControl = true;
                oXL.Quit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var folderDlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            var result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                var path = folderDlg.SelectedPath;

                var res = DirSearch_ex3(sDir: path);

                var json = JsonConvert.SerializeObject(value: res);
                jsonTreeView.ShowJson(jsonString: json);

                var filePath = $"Data\\data{DateTime.Now.ToShortDateString()}.json";

                if (!Directory.Exists(path: "Data"))
                    Directory.CreateDirectory(path: "Data");
                if (File.Exists(path: filePath))
                    File.Delete(path: filePath);

                using (var sw = File.AppendText(path: filePath))
                {
                    sw.WriteLine(value: json);
                }
            }
        }

        private static FileDir DirSearch_ex3(string sDir)
        {
            try
            {
                Console.WriteLine(value: sDir);

                var dirItem = new FileDir()
                {
                    Path = Path.GetFileName(path: sDir)
                };
                var fileList = new List<string>();
                foreach (var f in Directory.GetFiles(path: sDir))
                {
                    Console.WriteLine(value: f);

                    fileList.Add(item: Path.GetFileName(path: f));
                }

                dirItem.Files = fileList;
                dirItem.Folders = new List<FileDir>();
                foreach (var d in Directory.GetDirectories(path: sDir))
                {
                    dirItem.Folders.Add(item: DirSearch_ex3(sDir: d));
                }

                return dirItem;
            }
            catch (Exception excpt)
            {
                Console.WriteLine(value: excpt.Message);
                return null;
            }
        }

        private class FileDir
        {
            public string Path { get; set; }

            public List<string> Files { get; set; }

            public List<FileDir> Folders { get; set; }

            public override string ToString()
            {
                return Path;
            }
        }
    }
}
