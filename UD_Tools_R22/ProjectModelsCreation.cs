using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using AutodeskApplication = Autodesk.Revit.ApplicationServices;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Excel = Microsoft.Office.Interop.Excel;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Events;

namespace UD_Tools_R22
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class ProjectModelsCreation : IExternalDBApplication
    {
        #region Cached Variables

        public static ControlledApplication _cachedCtrlApp;

        #endregion
        string excelPath = @"\\Fs-uni\bim\Managers\Morozov\ИЛ8.xlsx";
        public List<List<string>> GetProjectFromExcel (string excelPath)
        {            
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;
            excelApp.DisplayAlerts = false;
            Excel.Workbook workbook = excelApp.Workbooks.Open(excelPath, Type.Missing, false, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing);
            List<List<string>> ModelsList = new List<List<string>>();
            foreach (Excel.Worksheet sheet in workbook.Sheets)
            {

                // Get the number of rows and columns in the worksheet
                int numRows = sheet.Rows.Count;
                int numCols = sheet.Columns.Count;
                // Loop through each row in the worksheet
                for (int row = 1; row <= numRows; row++)
                {
                    // Create a new list to store the values in the row
                    List<string> rowValues = new List<string>();

                    // Loop through each column in the row
                    for (int col = 1; col <= numCols; col++)
                    {
                        // Get the value of the cell and add it to the list
                        string cellValue = sheet.Cells[row, col].Value?.ToString() ?? "";
                        rowValues.Add(cellValue);
                    }
                    // Add the row to the data list
                    ModelsList.Add(rowValues);
                }
            }
            return ModelsList;
        }

        public void Execute(Document document)
        {
            // Create an automation utility with a hardcoded 
            // stairs configuration number

            
            // Generate the stairs

                        
        }
        //void OnExLaunched(object sender, )
        void OnApplicationInitialized(
      object sender,
      ApplicationInitializedEventArgs e)
        {
            // Sender is an Application instance:

            Application app = sender as Application;
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;
            excelApp.DisplayAlerts = false;
            Excel.Workbook workbook = excelApp.Workbooks.Open(excelPath, Type.Missing, false, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        Type.Missing, Type.Missing);
            List<List<string>> ModelsList = new List<List<string>>();
            
            foreach (Excel.Worksheet sheet in workbook.Sheets)
            {
                if (sheet.Name == "Доп данные")
                {
                    string pathToProjectFamily = sheet.Cells[0][1];
                }
                else
                {
                    int numRows = sheet.Rows.Count;
                    int numCols = sheet.Columns.Count;
                    for (int row = 0; row <= numRows - 1; row++)
                    {
                        List<string> rowValues = new List<string>();
                        for (int col = 0; col <= numCols - 1; col++)
                        {
                            string cellValue = sheet.Cells[row, col].Value?.ToString() ?? "";
                            rowValues.Add(cellValue);
                        }
                        string fileName = rowValues[0];
                        string bookName = rowValues[1];
                        string templateName = rowValues[2];
                        Document newDoc = app.NewProjectDocument(templateName);
                        _cachedCtrlApp.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(_cachedCtrlApp_DocumentCreated);
                        List<string> namesOfWorksets = new List<string>();



                        #region Save Document
                        WorksharingSaveAsOptions worksharingSave = new WorksharingSaveAsOptions()
                        {
                            SaveAsCentral = true,
                            OpenWorksetsDefault = SimpleWorksetConfiguration.AskUserToSpecify
                        };
                        SaveAsOptions saveAsOptions = new SaveAsOptions();
                        saveAsOptions.SetWorksharingOptions(worksharingSave);         
                        newDoc.SaveAs(@"C: \Users\dmorozov\Desktop\" + sheet.Name + @"\" + fileName + ".rvt", saveAsOptions);
                        #endregion
                    }

                }
            }
         
            /*
            Document doc = app.OpenDocumentFile(_model_path);

            if (doc == null)
            {
                throw new InvalidOperationException(
                  "Could not open document.");
            }
            Execute(doc);
            */
        }
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            try
            {
                _cachedCtrlApp = application;

                //TODO: add you code below.
                //MessageBox.Show("ExtDbApp");

                _cachedCtrlApp.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(_cachedCtrlApp_DocumentCreated);

                return ExternalDBApplicationResult.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show(ex.ToString(), ex.Message);

                return ExternalDBApplicationResult.Failed;
            }
        }
        void _cachedCtrlApp_DocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            Document doc = e.Document;
            ElementId id = ElementId.InvalidElementId;

            using (Transaction tr = new Transaction(doc, "Hello from RevitAddinWizard."))
            {
                tr.Start();

                //TextNote text = doc.Create.NewTextNote(
                //    doc.ActiveView,
                //    new XYZ(0, 0, 0),
                //    new XYZ(1, 0, 0),
                //    new XYZ(0, 0, 1),
                //    2,
                //    TextAlignFlags.TEF_ALIGN_CENTER,
                //    "Hello from RevitAddinWizard.");
                ////text.Width = 20;

                tr.Commit();
                //id = text.Id;
            }
        }
    }
}
