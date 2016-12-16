
#region 引用命名空间
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
using System.Web.UI.WebControls;
#endregion

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：文件公共类
    /// ** 创始时间：2010-2-28
    /// ** 修改时间：-
    /// ** 修改人：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    internal class FileSugar
    {
        #region 获取文件路并自动创建目录
        /// <summary>
        /// 根据文件目录、编号、文件名生成文件路径，并且创建文件存放目录
        /// 格式为:/directory/code/filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="code"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFiePathAndCreateDirectoryByCode<T>(string directory, T code, string fileName)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("FileSugar.GetCreatePath.directory");
            }
            directory = directory.TrimEnd('/');
            string path = new StringBuilder("{0}//{1}//{2}").AppendFormat(directory, code, fileName).ToString();
            directory = Path.GetDirectoryName(path);
            if (!IsExistDirectory(directory))
            {
                CreateDirectory(directory);
            }
            return path;

        }
        /// <summary>
        /// 根据文件目录、日期、文件名生成文件路径，并且创建文件存放目录
        /// 格式为:/directory/2015/01/01/filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFiePathAndCreateDirectoryByDate<T>(string directory, string fileName)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("FileSugar.GetCreatePath.directory");
            }
            directory = directory.TrimEnd('/');
            string path = new StringBuilder("{0}//{1}//{2}//{3}//{4}").AppendFormat(directory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, fileName).ToString();
            directory = Path.GetDirectoryName(path);
            if (!IsExistDirectory(directory))
            {
                CreateDirectory(directory);
            }
            return path;

        } 
        #endregion

        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            return HttpContext.Current.Server.MapPath(strPath);
        }
        #endregion

        #region 获取缩略图名称
        public static string GetMinPic(string filename, int index)
        {
            string str = "";

            if (string.IsNullOrEmpty(filename))
                return str;

            int nLastDot = filename.LastIndexOf(".");
            if (nLastDot == -1)
                return str;

            str = filename.Substring(0, nLastDot) + "_" + index.ToString() + filename.Substring(nLastDot, filename.Length - nLastDot);
            if (index == -1)
            {
                str = filename.Substring(0, nLastDot) + filename.Substring(nLastDot, filename.Length - nLastDot);
            }
            return str;
        }
        /// <summary>
        /// 获取缩略图片路径
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMinPic(string dir, string filename, int index)
        {
            if (string.IsNullOrEmpty(filename))
                return "";
            if (index < 0)
                index = 0;

            string minPic = string.Empty;
            minPic = string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(filename), index, Path.GetExtension(filename));
            if (!string.IsNullOrEmpty(dir))
                minPic = Path.Combine(dir, minPic);

            return minPic;
        }
        #endregion

        #region 字段定义
        /// <summary>
        /// 同步标识
        /// </summary>
        private static Object sync = new object();
        #endregion

        #region 检测指定目录是否存在
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>        
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 检测指定文件是否存在
        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 检测指定目录是否为空
        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断是否存在文件
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }

                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 检测指定目录中是否存在指定的文件
        /// <summary>
        /// 检测指定目录中是否存在指定的文件,若要搜索子目录请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>        
        public static bool Contains(string directoryPath, string searchPattern)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);

                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测指定目录中是否存在指定的文件
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, true);

                //判断指定文件是否存在
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 创建一个目录
        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion

        #region 创建一个文件

        #region 创建一个文件
        /// <summary>
        /// 创建一个文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);

                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);

                    lock (sync)
                    {
                        //创建文件                    
                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 创建一个文件,并将字节流写入文件
        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);

                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);

                    //创建一个FileInfo对象
                    FileInfo file = new FileInfo(filePath);

                    //创建文件
                    using (FileStream fs = file.Create())
                    {
                        //写入二进制流
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 创建一个文件,并将字符串写入文件

        #region 重载1
        /// <summary>
        /// 创建一个文件,并将字符串写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        public static void CreateFile(string filePath, string text)
        {
            CreateFile(filePath, text, Encoding.UTF8);
        }
        #endregion

        #region 重载2
        /// <summary>
        /// 创建一个文件,并将字符串写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">字符串数据</param>
        /// <param name="encoding">字符编码</param>
        public static void CreateFile(string filePath, string text, Encoding encoding)
        {
            try
            {
                if (IsExistFile(filePath)) {
                    DeleteFile(filePath);
                }
                //如果文件不存在则创建该文件
                if (!IsExistFile(filePath))
                {
                    //获取文件目录路径
                    string directoryPath = GetDirectoryFromFilePath(filePath);

                    //如果文件的目录不存在，则创建目录
                    CreateDirectory(directoryPath);

                    //创建文件
                    FileInfo file = new FileInfo(filePath);
                    using (FileStream stream = file.Create())
                    {
                        using (StreamWriter writer = new StreamWriter(stream, encoding))
                        {
                            //写入字符串     
                            writer.Write(text);

                            //输出
                            writer.Flush();
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #endregion

        #endregion

        #region 打开目录
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void OpenDirectory(string directoryPath)
        {

            //检测目录是否存在
            if (!IsExistDirectory(directoryPath))
            {
                return;
            }

            //打开目录
            GetMapPath(directoryPath);
        }
        #endregion

        #region 打开文件
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void OpenFile(string filePath)
        {
            //检测文件是否存在
            if (!IsExistFile(filePath))
            {
                return;
            }

            //打开目录
            GetMapPath(filePath);
        }
        #endregion

        #region 从文件绝对路径中获取目录路径
        /// <summary>
        /// 从文件绝对路径中获取目录路径
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetDirectoryFromFilePath(string filePath)
        {
            //实例化文件
            FileInfo file = new FileInfo(filePath);

            //获取目录信息
            DirectoryInfo directory = file.Directory;

            //返回目录路径
            return directory.FullName;
        }
        #endregion

        #region 获取文本文件的行数
        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static int GetLineCount(string filePath)
        {
            //创建流读取器
            using (StreamReader reader = new StreamReader(filePath))
            {
                //行数
                int i = 0;

                while (true)
                {
                    //如果读取到内容就把行数加1
                    if (reader.ReadLine() != null)
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                //返回行数
                return i;
            }
        }
        #endregion

        #region 获取一个文件的长度
        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static int GetFileSize(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小
            return (int)fi.Length;
        }

        /// <summary>
        /// 获取一个文件的长度,单位为KB
        /// </summary>
        /// <param name="filePath">文件的路径</param>        
        public static double GetFileSizeByKB(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);

            //获取文件的大小
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024);
        }

        /// <summary>
        /// 获取一个文件的长度,单位为MB
        /// </summary>
        /// <param name="filePath">文件的路径</param>        
        public static double GetFileSizeByMB(string filePath)
        {
            //创建一个文件对象
            FileInfo fi = new FileInfo(filePath);

            //获取文件的大小
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024 / 1024);
        }
        #endregion

        #region 获取指定目录中的文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 获取指定目录中的子目录列表
        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向文本文件写入内容
        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>        
        public static void WriteText(string filePath, string text)
        {
            WriteText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        /// <param name="encoding">编码</param>
        public static void WriteText(string filePath, string text, Encoding encoding)
        {
            //向文件写入内容
            File.WriteAllText(filePath, text, encoding);
        }
        #endregion

        #region 向文本文件的尾部追加内容
        /// <summary>
        /// 向文本文件的尾部追加内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="text">写入的内容</param>
        public static void AppendText(string filePath, string text)
        {
            //======= 追加内容 =======
            try
            {
                lock (sync)
                {
                    //创建流写入器
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(text);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 将现有文件的内容复制到新文件中
        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void CopyTo(string sourceFilePath, string destFilePath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }

            try
            {
                //检测目标文件的目录是否存在，不存在则创建
                string destDirectoryPath = GetDirectoryFromFilePath(destFilePath);
                CreateDirectory(destDirectoryPath);

                //复制文件
                FileInfo file = new FileInfo(sourceFilePath);
                file.CopyTo(destFilePath, true);
            }
            catch
            {
            }
        }
        #endregion

        #region 将文件移动到指定目录( 剪切 )
        /// <summary>
        /// 将文件移动到指定目录( 剪切 )
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void MoveToDirectory(string sourceFilePath, string descDirectoryPath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }

            try
            {
                //获取源文件的名称
                string sourceFileName = GetFileName(sourceFilePath);

                //如果目标目录不存在则创建
                CreateDirectory(descDirectoryPath);

                //如果目标中存在同名文件,则删除
                if (IsExistFile(descDirectoryPath + "\\" + sourceFileName))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceFileName);
                }

                //目标文件路径
                string descFilePath;
                if (!descDirectoryPath.EndsWith(@"\"))
                {
                    descFilePath = descDirectoryPath + "\\" + sourceFileName;
                }
                else
                {
                    descFilePath = descDirectoryPath + sourceFileName;
                }

                //将文件移动到指定目录
                File.Move(sourceFilePath, descFilePath);
            }
            catch
            {
            }
        }
        #endregion

        #region 将文件移动到指定目录，并指定新的文件名( 剪切并改名 )
        /// <summary>
        /// 将文件移动到指定目录，并指定新的文件名( 剪切并改名 )
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descFilePath">目标文件的绝对路径</param>
        public static void Move(string sourceFilePath, string descFilePath)
        {
            //有效性检测
            if (!IsExistFile(sourceFilePath))
            {
                return;
            }

            try
            {
                //获取目标文件目录
                string descDirectoryPath = GetDirectoryFromFilePath(descFilePath);

                //创建目标目录
                CreateDirectory(descDirectoryPath);

                //将文件移动到指定目录
                File.Move(sourceFilePath, descFilePath);
            }
            catch
            {
            }
        }
        #endregion

        #region 将流读取到缓冲区中
        /// <summary>
        /// 将流读取到缓冲区中
        /// </summary>
        /// <param name="stream">原始流</param>
        public static byte[] StreamToBytes(Stream stream)
        {
            try
            {
                //创建缓冲区
                byte[] buffer = new byte[stream.Length];

                //读取流
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));

                //返回流
                return buffer;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //关闭流
                stream.Close();
            }
        }
        #endregion

        #region 将文件读取到缓冲区中
        /// <summary>
        /// 将文件读取到缓冲区中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static byte[] FileToBytes(string filePath)
        {
            //获取文件的大小 
            int fileSize = GetFileSize(filePath);

            //创建一个临时缓冲区
            byte[] buffer = new byte[fileSize];

            //创建一个文件
            FileInfo file = new FileInfo(filePath);

            //创建一个文件流
            using (FileStream fs = file.Open(FileMode.Open))
            {
                //将文件流读入缓冲区
                fs.Read(buffer, 0, fileSize);

                return buffer;
            }
        }
        #endregion

        #region 将文件读取到字符串中
        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string FileToString(string filePath)
        {
            return FileToString(filePath, Encoding.UTF8);
        }
        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static string FileToString(string filePath, Encoding encoding)
        {
            //创建流读取器
            StreamReader reader = new StreamReader(filePath, encoding);
            try
            {
                //读取流
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //关闭流读取器
                reader.Close();
            }
        }
        #endregion

        #region 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileName(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }
        #endregion

        #region 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetFileNameNoExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }
        #endregion

        #region 从文件的绝对路径中获取扩展名
        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static string GetExtension(string filePath)
        {
            //获取文件的名称
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }
        #endregion

        #region 清空指定目录
        /// <summary>
        /// 清空指定目录下所有文件及子目录,但该目录依然保存.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }

                //删除目录中所有的子目录
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }
        #endregion

        #region 清空文件内容
        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void ClearFile(string filePath)
        {
            //删除文件
            File.Delete(filePath);

            //重新创建该文件
            CreateFile(filePath);
        }
        #endregion

        #region 删除指定文件
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endregion

        #region 删除指定目录
        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }
        #endregion

        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strValue"></param>
        public static void WriteFile(string strFilePath, string strValue)
        {
            System.IO.FileInfo oFile = new FileInfo(strFilePath);
            if (!oFile.Directory.Exists)
                oFile.Directory.Create();

            if (!oFile.Exists)
                oFile.Create().Close();

            System.IO.StreamWriter oWrite = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
            oWrite.Write(strValue);
            oWrite.Flush();
            oWrite.Close();
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strValue"></param>
        /// <param name="charset"></param>
        public static void WriteFile(string strFilePath, string strValue, string charset)
        {
            System.IO.FileInfo oFile = new FileInfo(strFilePath);
            if (!oFile.Directory.Exists)
                oFile.Directory.Create();

            if (!oFile.Exists)
                oFile.Create().Close();

            System.IO.StreamWriter oWrite = new StreamWriter(strFilePath, false, System.Text.Encoding.GetEncoding(charset));
            oWrite.Write(strValue);
            oWrite.Flush();
            oWrite.Close();
        }
        #endregion

        #region 网页中显示内容
        public static void ShowPDF(string filePath)
        {
            System.Web.HttpContext.Current.Response.ContentType = "Application/pdf";
            System.Web.HttpContext.Current.Response.WriteFile(filePath);
            System.Web.HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 在网页中显示WORD文档
        /// </summary>
        public static void ShowWord(string filePath)
        {
            System.Web.HttpContext.Current.Response.ContentType = "Application/msword";
            System.Web.HttpContext.Current.Response.WriteFile(filePath);
            System.Web.HttpContext.Current.Response.End();
        }

        public static void ShowExcel(string filePath)
        {
            System.Web.HttpContext.Current.Response.ContentType = "Application/x-msexcel";
            System.Web.HttpContext.Current.Response.WriteFile(filePath);
            System.Web.HttpContext.Current.Response.End();
        }

        public static void ShowHtml(string filePath)
        {
            System.Web.HttpContext.Current.Response.ContentType = "text/HTML";
            System.Web.HttpContext.Current.Response.WriteFile(filePath);
            System.Web.HttpContext.Current.Response.End();
        }

        public static void Show(string filePath)
        {
            System.Web.HttpContext.Current.Response.WriteFile(filePath);
            System.Web.HttpContext.Current.Response.End();
        }
        #endregion

        #region 根据路径得到文件流
        /// <summary>
        /// 根据路径得到文件流
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static byte[] GetFileSream(string Path)
        {
            byte[] buffer = null;
            using (FileStream stream = new FileInfo(Path).OpenRead())
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            }
            return buffer;

        }
        #endregion

    }

}
