using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfAuthorVerifier.Types
{
    public enum StatusType
    {
        stNew, stError, stDone, stInProgress
    };

    public class ItemClassExtended : Item
    {
        public StatusType Status
        {
            get { return GetValue(() => Status); }
            set
            {
                SetValue(() => Status, value);
                // Yes, we need this
                StatusAsText = value.ToString();
            }
        }

        /// <summary>
        /// </summary>
        public string StatusAsText
        {
            get
            {
                string result = "";
                switch (Status)
                {
                    case StatusType.stNew:
                        {
                            result = "New";
                            break;
                        }
                    case StatusType.stError:
                        {
                            result = "Error during checking";
                            break;
                        }
                    case StatusType.stDone:
                        {
                            result = "Completed";
                            break;
                        }
                    case StatusType.stInProgress:
                        {
                            result = "In progress";
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
                return result;
            }
            set { SetValue(() => StatusAsText, value); }
        }
    }

    public class ItemType : ItemClassExtended { };
}
