﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfAuthorVerifier.Core.Major;
using PdfAuthorVerifier.Types;

namespace PdfAuthorVerifier.ViewModels
{
    public partial class MainWindowVM : PropertyChangedNotification
    {
        #region Data
        private int TasksCounter;

        public ObservableCollection<ItemType> Items
        {
            get { return GetValue(() => Items); }
            set { SetValue(() => Items, value); }
        }

        public string RootFolder
        {
            get { return GetValue(() => RootFolder); }
            set { SetValue(() => RootFolder, value); }
        }

        public bool IsEnabled_SearchForFilesCommand
        {
            get { return GetValue(() => IsEnabled_SearchForFilesCommand); }
            set { SetValue(() => IsEnabled_SearchForFilesCommand, value); }
        }

        public bool IsEnabled_ClearCommand
        {
            get { return GetValue(() => IsEnabled_ClearCommand); }
            set { SetValue(() => IsEnabled_ClearCommand, value); }
        }

        public bool IsEnabled_AnalyzeFilesCommand
        {
            get { return GetValue(() => IsEnabled_AnalyzeFilesCommand); }
            set { SetValue(() => IsEnabled_AnalyzeFilesCommand, value); }
        }

        public bool IsEnabled_StopAnalyzeFilesCommand
        {
            get { return GetValue(() => IsEnabled_StopAnalyzeFilesCommand); }
            set { SetValue(() => IsEnabled_StopAnalyzeFilesCommand, value); }
        }

        public int ProgressPosition
        {
            get { return GetValue(() => ProgressPosition); }
            set { SetValue(() => ProgressPosition, value); }
        }
        public int ProgressMax
        {
            get { return GetValue(() => ProgressMax); }
            set { SetValue(() => ProgressMax, value); }
        }
        public string ProgressStatus
        {
            get { return GetValue(() => ProgressStatus); }
            set { SetValue(() => ProgressStatus, value); }
        }
        #endregion Data
    }
}