using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LoadLinks.Model;

namespace LoadLinks.ViewModel
{
    public enum RevitVersion
    {
        [Description("R19")]
        R2019 = 0,
        [Description("R20")]
        R2020 = 1,
        [Description("R21")]
        R2021 = 2,
        [Description("R22")]
        R2022 = 3,
        [Description("R23")]
        R2023 = 4
    }
    public class MainViewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private RevitVersion revitVersion;
        public RevitVersion SelectedRevitVersion
        {
            get => revitVersion;
            set
            {
                revitVersion = value;                
            }
        }

        private string listFilesPath;
        public string ListFilesPath
        {
            get => listFilesPath;
            set
            {
                listFilesPath = value;
                OnPropertyChanged();
            }
        }

        private string listLinksPath;
        public string ListLinksPath
        {
            get => listLinksPath;
            set
            {
                listLinksPath = value;
                OnPropertyChanged();
            }
        }

        
        private ExternalCommandData _commandData;
        public ICommand LoadLinksCommand { get; }
        private void OnLoadLinksCommandExecute(object p)
        {
            UIApplication application = _commandData.Application;
            List<string> pathsToFiles = MyHelpfullTools.MyHelpfullTools.GetPathsFromTXT(ListFilesPath);
            List<string> pathsToLinks;
            if (isFilesListEqualsToLinksList)            
                pathsToLinks = pathsToFiles;
            else
                pathsToLinks = MyHelpfullTools.MyHelpfullTools.GetPathsFromTXT(ListLinksPath);
            string fileError = String.Empty; // список файлов, для которых выполнение программы не удалось
            //TODO добавить список ошибок для каждого файла
            foreach (string pathToFile in pathsToFiles)
            {
                string modelPath = application.ActiveUIDocument.Document.PathName;
                OpenOptions openOptions = new OpenOptions();               
                WorksetConfiguration worksetConfig = new WorksetConfiguration(WorksetConfigurationOption.CloseAllWorksets);                
                openOptions.SetOpenWorksetsConfiguration(worksetConfig);
                ModelPath currentFile = ModelPathUtils.ConvertUserVisiblePathToModelPath(pathToFile);
                UIDocument uIDocument = application.OpenAndActivateDocument(currentFile, openOptions, false);
                Document doc = uIDocument.Document;
                //TODO Добавить возможность загрузки связей в файлы без рабочих наборов. Но нужно ли это вообще?
                View3D view = new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().FirstOrDefault();
                RevitLinkOptions rlo = new RevitLinkOptions(false);                
                Dictionary<ModelPath, Workset> linksWorksetsPairs = LoadLinksModel.GetLinksWorksetsPairs(doc, pathsToLinks, SelectedRevitVersion);
                //TODO добавить опцию создания рабочего набора под связь (а так же указать файлы, для которых был создан рабочий набор).
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Загрузка связей");
                    foreach (ModelPath pathToLink in linksWorksetsPairs.Keys)
                    {
                        LinkLoadResult linkLoadedType = null;
                        try
                        {
                            linkLoadedType = RevitLinkType.Create(doc, pathToLink, rlo);
                        }
                        catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                        {
                            continue;
                        }
                        doc.GetElement(linkLoadedType.ElementId).get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(linksWorksetsPairs[pathToLink].Id.IntegerValue);
                        RevitLinkInstance revitLinkInstance = RevitLinkInstance.Create(doc, linkLoadedType.ElementId, ImportPlacement.Shared);
                        doc.GetElement(revitLinkInstance.Id).get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(linksWorksetsPairs[pathToLink].Id.IntegerValue);
                        revitLinkInstance.Pinned = true;
                    }
                    transaction.Commit();
                }
                TransactWithCentralOptions transactWithCentralOptions = new TransactWithCentralOptions();
                SynchronizeWithCentralOptions synchronizeWithCentralOptions = new SynchronizeWithCentralOptions();
                WorksharingUtils.RelinquishOwnership(doc, new RelinquishOptions(true), transactWithCentralOptions);
                doc.SynchronizeWithCentral(transactWithCentralOptions, synchronizeWithCentralOptions);
                SaveAsOptions saveAsOptions = new SaveAsOptions();
                saveAsOptions.OverwriteExistingFile = true;
                application.OpenAndActivateDocument(modelPath);
                uIDocument.Document.Close(false);
            }
        }
        private bool CanLoadLinksCommandExecuted(object p)
        {
            if (listFilesPath == null)
                return false;
            return true;
        }
        public ICommand ChooseListFilesCommand { get; }
        private void OnChooseListFilesCommandExecute(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            ListFilesPath = openFileDialog.FileName;
        }

        private bool isFilesListEqualsToLinksList;
        public bool IsFilesListEqualsToLinksList
        {
            get => isFilesListEqualsToLinksList;
            set
            {
                isFilesListEqualsToLinksList = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChooseListLinksCommand { get; }
        private void OnChooseListLinksCommandExecute(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            ListLinksPath = openFileDialog.FileName;
        }

        public ICommand ChooseVisibilityCommand { get; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
#if DEBUG
            SelectedRevitVersion = RevitVersion.R2021;
#elif R2019
            SelectedRevitVersion = RevitVersion.R2019;
#elif R2020
            SelectedRevitVersion = RevitVersion.R2020;
#elif R2021
            SelectedRevitVersion = RevitVersion.R2021;
#elif R2022
            SelectedRevitVersion = RevitVersion.R2022;
#elif R2023
            SelectedRevitVersion = RevitVersion.R2023;
#endif
            ChooseListFilesCommand = new RelayCommand(OnChooseListFilesCommandExecute);
            ChooseListLinksCommand = new RelayCommand(OnChooseListLinksCommandExecute);
            LoadLinksCommand = new RelayCommand(OnLoadLinksCommandExecute, CanLoadLinksCommandExecuted);
        }
    }
}
