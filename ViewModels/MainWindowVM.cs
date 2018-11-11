using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using iTextSharp.text.pdf;
using PdfAuthorVerifier.Core.Major;
using PdfAuthorVerifier.Functions;
using PdfAuthorVerifier.Types;

namespace PdfAuthorVerifier.ViewModels
{
    public partial class MainWindowVM
    {
        #region Commands
        public ICommand SearchForFilesCommand { get; set; }
        public ICommand AnalyzeFilesCommand { get; set; }
        public ICommand ClearListOfFilesCommand { get; set; }
        public ICommand ExportResultCommand { get; set; }
        public ICommand RunSearchCommand { get; set; }
        #endregion Commands

        /// <summary>
        ///     Constructor
        /// </summary>
        public MainWindowVM(DataGrid dataGrid)
        {
            InitData();

            _DataGrid = dataGrid;

            InitCommands();
        }

        private void InitCommands()
        {
            SearchForFilesCommand = new RelayCommand(SearchForFilesAction);
            AnalyzeFilesCommand = new RelayCommand(AnalyzeFilesAction);
            ClearListOfFilesCommand = new RelayCommand(ClearListOfFilesAction);
            ExportResultCommand = new RelayCommand(ExportResultAction);
            RunSearchCommand = new RelayCommand(RunSearchAction);
        }

        private void InitData()
        {
            Items = new ObservableCollection<ItemType>();

            ProgressPosition = 0;
            ProgressMax = 1;

            EnDsCommands(CommandSet.csRootEnabled);

            // Специально после EnDsCommands
            RootFolder = @"G:\Docs\Books\Programming\C Sharp\" + Environment.NewLine + 
                @"G:\Docs\Books\Programming\FoxPro\" + Environment.NewLine +
                @"G:\Docs\Books\Programming\Delphi\" + Environment.NewLine +
                @"G:\Docs\Books\Programming\WPF\" +Environment.NewLine +
                @"G:\Docs\Books\Programming\SQL\";
        }

        #region Commands implementation
        private void ExportResultAction(Object o)
        {

        }

        private void RunSearchAction(Object o)
        {
            DataGridFunctions.ApplyFilterOnDataGrid(SearchQuery, _DataGrid);
        }

        private void SearchForFilesAction(Object o)
        {
            EnDsCommands(CommandSet.csAllDisabled);

            if (String.IsNullOrEmpty(RootFolder))
            {
                return;
            }

            string[] folders = RootFolder.Split(Environment.NewLine.ToCharArray());

            List<Task<string[]>> tasks = new List<Task<string[]>>();

            foreach (string folder in folders)
            {
                string workingFolder = folder.Trim();

                if (String.IsNullOrEmpty(folder))
                {
                    continue;
                }

                if (!workingFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    workingFolder += Path.DirectorySeparatorChar.ToString();
                }

                Task<string[]> task = Task<string[]>.Factory.StartNew(() =>
                {
                    // Find all PDF files starting from location
                    string[] files = Directory.GetFiles(workingFolder, "*.PDF", SearchOption.AllDirectories);

                    return files;
                });
                tasks.Add(task);
            }

            int totalTasks = tasks.Count;
            int completedTasks = 0;
            do
            {
                int z = Task.WaitAny(tasks.ToArray());
                completedTasks++;
                Task<string[]> completedTask = tasks[z];
                tasks.RemoveAt(z);
                foreach (string value in completedTask.Result)
                {
                    if (Items.Where(x => x.FileName == value).Count() == 0)
                    {
                        Items.Add(new ItemType { FileName = value, Status = StatusType.stNew, TypeOfItem = PdfLocation.plFile });
                    }
                }
            }
            while (completedTasks != totalTasks);

            EnDsCommands(CommandSet.csScan);
        }

        private void AnalyzeFilesAction(Object o)
        {
            EnDsCommands(CommandSet.csStop);

            ProgressPosition = Items.Count();
            ProgressMax = Items.Count();
            ProgressStatus = "Running...";

            foreach (ItemType file in Items)
            {
                Interlocked.Increment(ref TasksCounter);
                Task task = new Task(() =>
                {
                    file.Status = StatusType.stInProgress;

                    PdfFunctions pdfFunctions = new PdfFunctions();
                    DateTimeFunctions dateTimeFunctions = new DateTimeFunctions();

                    PdfReader pdfFile = null;
                    try
                    {
                        string value = "";

                        if (file.TypeOfItem == PdfLocation.plFile)
                        {
                            pdfFile = pdfFunctions.GetPdf(file.FileName);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        if (pdfFile != null)
                        {
                            //
                            file.FileLength = pdfFile.FileLength;

                            file.Encrypted = pdfFile.IsEncrypted();

                            file.NumberOfPages = pdfFile.NumberOfPages;

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "Author");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.Author = value;
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "Title");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.Title = value;
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "Producer");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.Producer = value;
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "EBX_PUBLISHER");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.Publisher = value;
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "CreationDate");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.CreationDate = dateTimeFunctions.FixDateTime(value);
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "ModDate");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.ModificationDate = dateTimeFunctions.FixDateTime(value);
                            }

                            file.Status = StatusType.stDone;
                        }
                        else
                        {
                            file.Status = StatusType.stError;
                        }
                    }
                    finally
                    {
                        pdfFile?.Close();
                        pdfFile?.Dispose();

                        GC.Collect();
                    }

                    Interlocked.Decrement(ref TasksCounter);
                    TaskCompleted();
                });
                task.Start();
            }
        }

        private void TaskCompleted()
        {
            ProgressPosition--;

            if (Interlocked.Equals(TasksCounter, 0))
            {
                ProgressPosition = 0;
                EnDsCommands(CommandSet.csScan);
                ProgressStatus = "Completed " + ProgressMax;
            }
        }

        private void ClearListOfFilesAction(Object o)
        {
            Items.Clear();
            ProgressStatus = "";
            EnDsCommands(CommandSet.csScan);
        }

        #endregion Commands implementation

        private enum CommandSet
        {
            /// <summary>
            ///     All buttons and textbox disabled
            /// </summary>
            csAllDisabled,
            /// <summary>
            ///     All buttons disabled but textbox enabled
            /// </summary>
            csRootEnabled,
            /// <summary>
            ///     Scan button enabled, Clear & Analyze enabled, if Items.Count > 0
            /// </summary>
            csScan,
            /// <summary>
            ///     Scan & Stop disabled, Clear & Analyze enabled, if Items.Count > 0
            /// </summary>
            csRun,
            /// <summary>
            ///     Scan, Clear, Analyze disabled, Stop enabled
            /// </summary>
            csStop
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="way">
        /// </param>
        /// <param name="status"></param>
        private void EnDsCommands(CommandSet way)
        {
            switch (way)
            {
                case CommandSet.csAllDisabled:
                    {
                        IsEnabled_RootFolder =
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_ExportResultCommand = 
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand =
                        IsEnabled_StopAnalyzeFilesCommand = false;

                        break;
                    }
                case CommandSet.csRootEnabled:
                    {
                        IsEnabled_RootFolder = true;
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_ExportResultCommand =
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand =
                        IsEnabled_StopAnalyzeFilesCommand = false;

                        break;
                    }
                case CommandSet.csScan:
                    {
                        IsEnabled_RootFolder =
                        IsEnabled_SearchForFilesCommand = true;
                        IsEnabled_ExportResultCommand =
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand = Items.Count > 0;
                        IsEnabled_StopAnalyzeFilesCommand = false;

                        break;
                    }
                case CommandSet.csRun:
                    {
                        IsEnabled_RootFolder =
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_StopAnalyzeFilesCommand = false;
                        IsEnabled_ExportResultCommand =
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand = Items.Count > 0;

                        break;
                    }
                case CommandSet.csStop:
                    {
                        IsEnabled_RootFolder =
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_ExportResultCommand =
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand = false;
                        IsEnabled_StopAnalyzeFilesCommand = true;

                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
