using System;
using System.IO;
using USC.GISResearchLab.Common.Utils.Directories;
using USC.GISResearchLab.Common.Utils.Strings;

namespace USC.GISResearchLab.Common.Utils.Files
{
    /// <summary>
    /// Summary description for FileUtils.
    /// </summary>
    public class FileUtilsHighSecurity
    {

        public static string GetFirstSubdirectoryPathFromPattern(string basePath, string patternPath, string fileName)
        {
            string subDirectoryPath = DirectoryUtils.GetFirstSubDirectoryPath(basePath);
            string subDirectoryName = DirectoryUtils.GetDirectoryName(subDirectoryPath);
            string[] subsitutionNamesValues = new string[] { "{BASE}=" + basePath, "{DIR}=" + subDirectoryName, "{FILE}=" + fileName };
            return EvaluatePathFromPattern(patternPath, subsitutionNamesValues);
        }

        //public static string EvaluatePathFromTreePattern(string basePath, string subDirectoryName, string patternPath, string fileName)
        //{
        //    if (basePath == null)
        //    {
        //        throw new Exception("EvaluatePathFromTreePattern error: basePath is null");
        //    }
        //    if (subDirectoryName == null)
        //    {
        //        throw new Exception("EvaluatePathFromTreePattern error: subDirectoryName is null");
        //    }
        //    if (patternPath == null)
        //    {
        //        throw new Exception("EvaluatePathFromTreePattern error: patternPath is null");
        //    }
        //    if (fileName == null)
        //    {
        //        throw new Exception("EvaluatePathFromTreePattern error: fileName is null");
        //    }

        //    if (patternPath.IndexOf("{BASE}") != -1)
        //    {
        //        while (patternPath.IndexOf("{BASE}") != -1)
        //        {
        //            patternPath = patternPath.Replace("{BASE}", basePath);
        //        }
        //    }

        //    if (patternPath.IndexOf("{DIR}") != -1)
        //    {
        //        while (patternPath.IndexOf("{DIR}") != -1)
        //        {
        //            patternPath = patternPath.Replace("{DIR}", subDirectoryName);
        //        }
        //    }

        //    if (patternPath.IndexOf("{FILE}") != -1)
        //    {
        //        while (patternPath.IndexOf("{FILE}") != -1)
        //        {
        //            patternPath = patternPath.Replace("{FILE}", fileName);
        //        }
        //    }

        //    patternPath = patternPath.Replace(@"\\", @"\");

        //    return patternPath;
        //}

        public static string EvaluatePathFromPattern(string patternPath, string[] subsitutionNamesValues)
        {
            if (patternPath != null)
            {
                if (subsitutionNamesValues != null)
                {
                    for (int i = 0; i < subsitutionNamesValues.Length; i++)
                    {
                        string[] parts = subsitutionNamesValues[i].Split('=');
                        string sub = parts[0];
                        string val = parts[1];
                        if (sub != null && val != null)
                        {
                            while (patternPath.IndexOf(sub) != -1)
                            {
                                patternPath = patternPath.Replace(sub, val);
                            }
                        }
                    }
                }

                patternPath = patternPath.Replace(@"\\", @"\");
            }

            return patternPath;
        }

        public static byte[] AsBytes(string filePath)
        {
            byte[] ret = null;
            long numBytes = 0;
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                numBytes = fileInfo.Length;
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                ret = binaryReader.ReadBytes((int)numBytes);
                binaryReader.Close();
                fileStream.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading file as bytes: " + filePath + " - " + e.Message, e);
            }
            return ret;
        }

        public static int GetBytesAtOffset(ref byte[] buffer, string filePath, int startPosition, int size)
        {
            int ret = -1;
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                binaryReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
                ret = binaryReader.Read(buffer, 0, size);
                binaryReader.Close();
                fileStream.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading file as bytes: " + filePath + " - " + e.Message, e);
            }
            return ret;
        }

        public static bool SaveBytes(byte[] data, long numBytes, string filePath)
        {
            bool ret = false;
            try
            {
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.CreateNew);
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                    for (int i = 0; i < numBytes; i++)
                    {
                        binaryWriter.Write(data[i]);
                    }

                    binaryWriter.Close();
                    fileStream.Close();
                    ret = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error writing bytes to file: " + filePath, e);
            }

            return ret;
        }

        public static string GetNextIncrementalFileName(string filePath)
        {
            string ret = filePath;

            while (FileExists(ret))
            {

                string path = GetDirectoryPath(ret);
                string ext = GetExtension(ret);
                string name = GetFileNameWithoutExtension(ret);
                string incrementedName = StringUtils.GetNextIncrementalString(name);

                ret = path + incrementedName + ext;

            }
            return ret;
        }

        public static bool TestPath(string path, bool createIfNeeded, bool throwError)
        {
            bool ret = true;

            if (!FileExists(path))
            {
                ret = false;
                if (throwError)
                {
                    throw new FileNotFoundException("File not found: " + path);
                }
                else if (createIfNeeded)
                {
                    File.CreateText(path);
                    ret = true;
                }
            }

            return ret;
        }

        public static string AsString(string path)
        {
            string ret = null;
            try
            {
                TextReader textReader = new StreamReader(path);
                ret = textReader.ReadToEnd();
                textReader.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading file as string: " + path, e);
            }
            return ret;
        }

        public static void ReplaceContents(string path, string contents)
        {
            OverwriteTextFile(path, contents);
        }

        public static bool FileExists(string path)
        {
            bool ret = false;
            if (!StringUtils.IsEmpty(path))
            {
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    ret = true;
                }
            }
            return ret;
        }

        public static string GetFileNameWithoutExtension(string filePath)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(filePath))
            {
                ret = Path.GetFileNameWithoutExtension(filePath);
            }
            return ret;
        }

        public static string GetFileNameFromPath(string filePath)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(filePath))
            {
                ret = Path.GetFileName(filePath);
            }
            return ret;
        }

        public static string GetDrive(string filePath)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(filePath))
            {
                ret = DirectoryUtils.GetDrive(filePath);
            }
            return ret;
        }

        public static string GetDirectoryPath(string filePath)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(filePath))
            {
                ret = GetDirectoryPath(filePath, true);
            }
            return ret;
        }

        public static string GetDirectoryPathName(string filePath)
        {
            string ret = "";
            if (!String.IsNullOrEmpty(filePath))
            {
                string directoryPath = GetDirectoryPath(filePath, true);
                ret = DirectoryUtils.GetDirectoryName(directoryPath);
            }
            return ret;
        }


        public static string GetDirectoryPath(string filePath, bool includeTrailingSlash)
        {
            string ret = "";
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    ret = (Path.GetDirectoryName(filePath));
                    if (includeTrailingSlash)
                    {
                        ret += "\\";
                    }
                }
                else
                {
                    throw new Exception("FileUtils.GetDirectoryPath() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetDirectoryPath() error: filePath is null");
            }
            return ret;
        }

        public static string GetDirectoryName(string filePath)
        {
            string ret = "";
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    ret = Directory.GetParent(filePath).Name;
                }
                else
                {
                    throw new Exception("FileUtils.GetDirectoryName() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetDirectoryName() error: filePath is null");
            }
            return ret;
        }

        public static string GetFileSizeScaled(string filePath)
        {
            double size = GetFileSize(filePath);
            return GetFileSizeScaled(size);
        }

        public static string GetFileSizeScaled(double size)
        {
            string ret = "";
            string scale;

            if (size > 1000000000.0)
            {
                size = (size / 1000000000.0);
                scale = " GB";
            }
            else if (size > 1000000.0)
            {
                size = (size / 1000000.0);
                scale = " MB";
            }
            else if (size > 1000.0)
            {
                size = (size / 1000.0);
                scale = " KB";
            }
            else
            {
                scale = " B";
            }

            size = System.Math.Round(size, 2);
            ret = size + scale;
            return ret;
        }

        public static double GetFileSizeUnScaled(string sizeString)
        {
            double ret = 0;

            string[] parts = sizeString.Split(' ');
            string sizePart = parts[0];
            string scale = parts[1];

            ret = Convert.ToDouble(sizePart);

            if (String.Compare(scale, "GB", true) == 0)
            {
                ret = ret * 1000000000.0;
            }

            else if (String.Compare(scale, "MB", true) == 0)
            {
                ret = ret * 1000000.0;
            }
            else if (String.Compare(scale, "KB", true) == 0)
            {
                ret = ret * 1000.0;
            }

            return ret;
        }

        public static string GetFileDate(string filePath)
        {
            string ret = "";
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    ret = Convert.ToString(fileInfo.CreationTime);
                }
                else
                {
                    throw new Exception("FileUtils.GetFileDate() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetFileDate() error: filePath is null");
            }
            return ret;
        }

        public static DateTime GetFileModifiedDate(string filePath)
        {
            DateTime ret = new DateTime();
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    ret = fileInfo.LastWriteTime;
                }
                else
                {
                    throw new Exception("FileUtils.GetFileModifiedDate() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetFileModifiedDate() error: filePath is null");
            }
            return ret;
        }

        public static double GetFileSize(string filePath)
        {
            double ret = -1.0;
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    ret = Convert.ToDouble(fileInfo.Length);
                }
                else
                {
                    throw new Exception("FileUtils.GetFileSize() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetFileSize() error: filePath is null");
            }
            return ret;
        }

        public static string GetFileName(string filePath)
        {
            string ret = "";
            if (filePath != null)
            {
                if (!filePath.Equals(""))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    ret = fileInfo.Name;
                }
                else
                {
                    throw new Exception("FileUtils.GetDirectoryName() error: filePath empty");
                }
            }
            else
            {
                throw new Exception("FileUtils.GetDirectoryName() error: filePath is null");
            }
            return ret;
        }

        public static string GetDirectoryParentPath(string filePath)
        {
            string ret = "";
            if (filePath != null && !filePath.Equals(""))
            {
                DirectoryInfo fileDirectory = Directory.GetParent(filePath);
                string fileDirectoryPath = fileDirectory.FullName;
                ret = Directory.GetParent(fileDirectoryPath).FullName + "\\";
            }
            return ret;
        }

        public static string TrimExtension(string fileName, string ext)
        {
            if (fileName.ToLower().IndexOf(ext) != -1)
            {
                fileName = fileName.Substring(0, fileName.ToLower().IndexOf(ext));
            }
            return fileName;
        }

        public static string GetExtension(string filePath, bool includePreceedingPeriod)
        {
            string ret = GetExtension(filePath);
            if (filePath != null)
            {
                if (!includePreceedingPeriod)
                {
                    if (ret.StartsWith("."))
                    {
                        ret = ret.Substring(1);
                    }
                }
                else
                {
                    if (!ret.StartsWith("."))
                    {
                        ret = "." + ret;
                    }
                }
            }
            return ret;
        }

        public static string GetExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public static bool DeleteShapefile(string shapefilePath)
        {
            bool ret = false;
            try
            {

                string baseDirectory = GetDirectoryPath(shapefilePath);
                string baseName = GetFileNameWithoutExtension(shapefilePath);

                DeleteFile(baseDirectory + baseName + ".shp");
                DeleteFile(baseDirectory + baseName + ".dbf");
                DeleteFile(baseDirectory + baseName + ".shx");
                DeleteFile(baseDirectory + baseName + ".prj");

                ret = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error occurred deleting shapefile: " + shapefilePath, e);
            }

            return ret;
        }

        public static bool MoveFile(string path, string destination)
        {
            bool ret = false;
            try
            {
                FileInfo file = new FileInfo(path);

                if (file.Exists)
                {
                    File.Move(path, destination);
                }

                ret = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while trying to move file: " + path, e);
            }

            return ret;
        }

        public static bool DeleteFile(string path)
        {
            bool ret = false;
            try
            {
                FileInfo file = new FileInfo(path);

                if (file.Exists)
                {
                    file.Delete();
                }

                ret = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while trying to delete file: " + path, e);
            }

            return ret;
        }

        public static void AppendText(string fileName, string s)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true);
                writer.Write(s);
                writer.Close();

            }
            catch (Exception e)
            {
                throw new Exception("An error occured appending to file: '" + fileName + "' - '" + s + "'", e);
            }
        }

        public static void AppendLine(string fileName)
        {
            AppendLine(fileName, "");
        }

        public static void AppendLine(string fileName, string s)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true);
                writer.WriteLine(s);
                writer.Close();

            }
            catch (Exception e)
            {
                throw new Exception("An error occured appending to file: '" + fileName + "' - '" + s + "'", e);
            }
        }

        public static void AppendTextFile(string fileName, string s)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true);
                writer.WriteLine(s);
                writer.Close();

            }
            catch (Exception e)
            {
                throw new Exception("An error occured appending to file: '" + fileName + "' - '" + s + "'", e);
            }
        }

        public static void OverwriteTextFile(string fileName, string s)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, false);
                writer.Write(s);
                writer.Close();

            }
            catch (Exception e)
            {
                throw new Exception("An error occured overwriting file: '" + fileName + "' - '" + s + "'", e);
            }
        }

        public static void CreateTextFile(string filePath)
        {
            try
            {
                if (!FileExists(filePath))
                {
                    if (!DirectoryUtils.DirectoryExists(FileUtils.GetDirectoryPath(filePath)))
                    {
                        DirectoryUtils.CreateDirectory(FileUtils.GetDirectoryPath(filePath));
                    }
                    StreamWriter writer = File.CreateText(filePath);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occured creating text file: " + filePath, e);
            }

        }

        public static void RefreshContents(string filePath, string contents)
        {
            StreamWriter writer = File.CreateText(filePath);
            writer.Write(contents);
            writer.Close();

        }


    }
}
