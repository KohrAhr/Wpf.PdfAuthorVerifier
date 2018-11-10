using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        #endregion Commands

        /// <summary>
        ///     Constructor
        /// </summary>
        public MainWindowVM()
        {
            InitData();

            InitCommands();
        }

        private void InitCommands()
        {
            SearchForFilesCommand = new RelayCommand(SearchForFilesAction);
            AnalyzeFilesCommand = new RelayCommand(AnalyzeFilesAction);
            ClearListOfFilesCommand = new RelayCommand(ClearListOfFilesAction);
        }


        private void InitData()
        {
            Items = new ObservableCollection<ItemType>();

            RootFolder = @"G:\Docs\Books\Programming\";

            ProgressPosition = 0;
            ProgressMax = 1;

            EnDsCommands(1);
        }

        #region Commands implementation
        private void SearchForFilesAction(Object o)
        {
            EnDsCommands(0);

            string[] files = null;

            if (String.IsNullOrEmpty(RootFolder))
            {
                return;
            }

            if (!RootFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                RootFolder += Path.DirectorySeparatorChar.ToString();
            }

            Task task = new Task(() =>
            {
                // Find all PDF files starting from location
                files = Directory.GetFiles(RootFolder, "*.PDF", SearchOption.AllDirectories);
            });

            task.Start();
            task.ContinueWith(t => 
            {
                try
                {
                    // All to list
                    if (files == null)
                    {
                        return;
                    }

                    foreach (string file in files)
                    {
                        if (Items.Where(x => x.FileName == file).Count() == 0)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                Items.Add(
                                    new ItemType { FileName = file, TypeOfItem = 0, Status = StatusType.stNew }
                                );
                            });
                        }
                    }

                }
                finally
                {
                    EnDsCommands(1);
                }
            });
        }

        private void AnalyzeFilesAction(Object o)
        {
            EnDsCommands(3);

            ProgressPosition = Items.Count();
            ProgressMax = Items.Count();

            foreach (ItemType file in Items)
            {
                Interlocked.Increment(ref TasksCounter);
                Task task = new Task(() =>
                {
                    file.Status = StatusType.stInProgress;

                    PdfFunctions pdfFunctions = new PdfFunctions();

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
                                file.CreationDate = value;
                            }

                            // 
                            value = pdfFunctions.GetValue(pdfFile, "ModDate");

                            if (!String.IsNullOrEmpty(value))
                            {
                                file.ModificationDate = value;
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
                EnDsCommands(1);
            }
        }

        private void ClearListOfFilesAction(Object o)
        {
            Items.Clear();
            EnDsCommands(1);
        }

        #endregion Commands implementation

        /// <summary>
        ///     
        /// </summary>
        /// <param name="way">
        ///     0 -- All buttons disabled
        ///     1 -- Scan button enabled
        ///     2 -- Scan & Stop disabled, Clear & Analyze enabled, if Items.Count > 0
        ///     3 -- Scan, Clear, Analyze disabled, Stop enabled
        /// </param>
        /// <param name="status"></param>
        private void EnDsCommands(int way)
        {
            switch (way)
            {
                case 0:
                    {
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand =
                        IsEnabled_StopAnalyzeFilesCommand = false;

                        break;
                    }
                case 1:
                    {
                        IsEnabled_SearchForFilesCommand = true;
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand = Items.Count > 0;
                        IsEnabled_StopAnalyzeFilesCommand = false;

                        break;
                    }
                case 2:
                    {
                        IsEnabled_SearchForFilesCommand =
                        IsEnabled_StopAnalyzeFilesCommand = false;
                        IsEnabled_ClearCommand =
                        IsEnabled_AnalyzeFilesCommand = Items.Count > 0;

                        break;
                    }
                case 3:
                    {
                        IsEnabled_SearchForFilesCommand =
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
