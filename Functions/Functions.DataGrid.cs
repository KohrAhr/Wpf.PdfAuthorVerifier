using PdfAuthorVerifier.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace PdfAuthorVerifier.Functions
{
    public static class DataGridFunctions
    {
        public static void ApplyFilterOnDataGrid(string query, DataGrid dataGrid)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            if (String.IsNullOrEmpty(query))
            {
                cv.Filter = null;
            }
            else
            {
                cv.Filter = x =>
                {
                    bool match = false;
                    foreach (PropertyInfo propertyInfo in x.GetType().GetProperties())
                    {
                        object value = propertyInfo.GetValue(x, null);

                        if (value == null)
                        {
                            continue;
                        }

                        string valueAsString = value.ToString();

                        match = valueAsString.ToUpper().Contains(query.ToUpper());

                        // "If" for optimization
                        if (match)
                        {
                            break;
                        }
                    }

                    return match;
                };
            }
        }
    }
}
