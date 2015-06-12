using System;
using System.Collections;
using System.Data;
using System.IO;
using USC.GISResearchLab.Common.Utils.Files;

namespace USC.GISResearchLab.Common.Utils.Directories
{
	/// <summary>
	/// Summary description for DirectoryUtils.
	/// </summary>
    public class DirectoryUtilsHighSecurity
    {
        public DirectoryUtilsHighSecurity()
        {
        }



        public static DataTable GetFileListAsDataTable(string path)
        {
            return GetFileListAsDataTable(path, null, false);
        }

        public static DataTable GetFileListAsDataTable(string path, string typeFilter, bool useTypeFilter)
        {
            return GetFileListAsDataTable(path, typeFilter, useTypeFilter, SearchOption.TopDirectoryOnly);
        }

        public static DataTable GetFileListAsDataTable(string path, string typeFilter, bool useTypeFilter, SearchOption SearchOption)
        {
            DataTable ret = null;

            ArrayList arrayList = DirectoryUtils.getFileList(path, typeFilter, useTypeFilter, SearchOption);

            if (arrayList != null && arrayList.Count > 0)
            {
                ret = new DataTable();

                ret.Columns.Add(new DataColumn("FilePath"));
                ret.Columns.Add(new DataColumn("FileName"));

                foreach (string filePath in arrayList)
                {
                    DataRow dataRow = ret.NewRow();
                    dataRow["FilePath"] = filePath;
                    dataRow["FileName"] = FileUtils.GetFileName(filePath);
                    ret.Rows.Add(dataRow);
                }

            }

            return ret;
        }


        public static String GetEnvTempDir()
        {
            return Environment.GetEnvironmentVariable("TEMP");
        }

    }


}
