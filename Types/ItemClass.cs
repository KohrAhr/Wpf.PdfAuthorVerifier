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
            set { SetValue(() => Author, value); }
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
            set { SetValue(() => CreationDate, value); }
        }
        /// <summary>
        ///     Value from PDF
        /// </summary>
        public DateTime? ModificationDate
        {
            get { return GetValue(() => ModificationDate); }
            set { SetValue(() => ModificationDate, value); }
        }
    }
}
