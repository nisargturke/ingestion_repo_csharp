using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ArtHandler.Repository
{
    public class ResourceFileManager
    {
        private string CurrentCulture = "en-US";

        public ResourceFileManager(string culture)
        {
            CurrentCulture = culture;
        }
        public string GetColumnNameForCurrentCulture(string columnName)
        {
            if (CurrentCulture != "en-US")
                return columnName + "_" + CurrentCulture;
            else
                return columnName;
        }
    }
}
