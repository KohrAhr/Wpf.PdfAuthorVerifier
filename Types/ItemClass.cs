using PdfAuthorVerifier.Core.Major;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfAuthorVerifier.Types
{
    public enum PdfLocation
    {
        plFile,
        plURL
    };

    public class Item : PropertyChangedNotification
    {
        /// <summary>
        ///     0 -- File
        ///     1 -- URL (reserved for future)
        /// </summary>
        public PdfLocation TypeOfItem
        {
            get { return GetValue(() => TypeOfItem); }
            set { SetValue(() => TypeOfItem, value); }
        }
        public string FileName
        {
            get { return GetValue(() => FileName); }
            set { SetValue(() => FileName, value); }
        }

        /// <summary>
        ///     Value from PDF
        /// </summary>
        public string Author
        {
            get { return GetValue(() => Author); }
            set
            {
                SetValue(() => Author, value);
                NotifyPropertyChanged("AdHock_Item6");
            }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public string Title
        {
            get { return GetValue(() => Title); }
            set { SetValue(() => Title, value); }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public string Producer
        {
            get { return GetValue(() => Producer); }
            set { SetValue(() => Producer, value); }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public string Publisher
        {
            get { return GetValue(() => Publisher); }
            set { SetValue(() => Publisher, value); }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public DateTime? CreationDate
        {
            get { return GetValue(() => CreationDate); }
            set
            {
                SetValue(() => CreationDate, value);
                NotifyPropertyChanged("AdHock_Item1");
                NotifyPropertyChanged("AdHock_Item2");
                NotifyPropertyChanged("AdHock_Item3");
                NotifyPropertyChanged("AdHock_Item4");
                NotifyPropertyChanged("AdHock_Item5");
            }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public DateTime? ModificationDate
        {
            get { return GetValue(() => ModificationDate); }
            set
            {
                SetValue(() => ModificationDate, value);
                NotifyPropertyChanged("AdHock_Item1");
                NotifyPropertyChanged("AdHock_Item2");
                NotifyPropertyChanged("AdHock_Item3");
                NotifyPropertyChanged("AdHock_Item4");
                NotifyPropertyChanged("AdHock_Item5");
            }
        }

        /// <summary>
        ///     Yellow
        /// </summary>
        public bool AdHock_Item1
        {
            get
            {
                return CreationDate != null && ModificationDate != null && CreationDate > ModificationDate;
            }
            set { SetValue(() => AdHock_Item1, value); }
        }

        /// <summary>
        ///     Green
        /// </summary>
        public bool AdHock_Item2
        {
            get
            {
                return CreationDate != null && ModificationDate != null && CreationDate < ModificationDate;
            }
            set { SetValue(() => AdHock_Item2, value); }
        }

        /// <summary>
        ///     Red
        /// </summary>
        public bool AdHock_Item3
        {
            get
            {
                return CreationDate != null && ModificationDate != null && CreationDate > ModificationDate;
            }
            set { SetValue(() => AdHock_Item3, value); }
        }

        /// <summary>
        ///     Light Green
        /// </summary>
        public bool AdHock_Item4
        {
            get
            {
                return CreationDate != null && ModificationDate != null && CreationDate == ModificationDate;
            }
            set { SetValue(() => AdHock_Item4, value); }
        }

        /// <summary>
        ///     Dark Red
        /// </summary>
        public bool AdHock_Item5
        {
            get
            {
                return CreationDate != null && ModificationDate != null && CreationDate > DateTime.Now || ModificationDate > DateTime.Now;
            }
            set { SetValue(() => AdHock_Item5, value); }
        }

        public bool AdHock_Item6
        {
            get
            {
                int c = 0;
                if (!String.IsNullOrEmpty(Author))
                {
                    string result = Author.Replace(".", "").Replace(",", "");

                    string[] results = result.Split(" ".ToCharArray());

                    foreach (string s in results)
                    {
                        if (s.Length > 1)
                        {
                            c++;
                        }
                        else
                        {
                            c--;
                        }
                    }
                }

                return c > 1;
            }
            set { SetValue(() => AdHock_Item6, value); }
        }
    }
}
