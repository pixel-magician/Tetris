using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;
using System;
using System.Threading.Tasks;


namespace PM
{
    public class ZipTool
    {
        /// <summary>
        /// 压缩读取文件的速度，每次读取的字节数
        /// </summary>
        static int _speed = 1024;

        public delegate void DelegateNoArg();
        public delegate void DelegateProgress(float progress);

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="path">需压缩的路径</param>
        /// <param name="outPath">压缩后文件的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="progress_CallBack">压缩进度更新的回调</param>
        public static void ToZip(string path, string outPath, string password = null, DelegateNoArg finish_CallBack = null, DelegateProgress progress_CallBack = null)
        {
            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(outPath));
            zipOutputStream.SetLevel(6);
            if (!string.IsNullOrEmpty(password))
                zipOutputStream.Password = password;
            //获取所有文件夹和文件
            List<string> dirs = GetDirectory_All(path);
            List<string> files = GetFiles_All(path);
            int size = dirs.Count + files.Count;
            int count = 0;
            //压缩
            foreach (var item in dirs)
            {
                ToZip_Directory(path, item, zipOutputStream);
                progress_CallBack?.Invoke((float)count / size);
                count++;
            }
            foreach (var item in files)
            {
                ToZip_File(path, item, zipOutputStream);
                progress_CallBack?.Invoke((float)count / size);
                count++;
            }

            progress_CallBack?.Invoke((float)count / size);
            zipOutputStream.Finish();
            zipOutputStream.Close();
            //触发完成的回调
            finish_CallBack?.Invoke();
        }
        /// <summary>
        /// 异步压缩文件
        /// </summary>
        /// <param name="path">需压缩的路径</param>
        /// <param name="outPath">压缩后文件的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="progress_CallBack">压缩进度更新的回调</param>
        public static async void ToZipAsync(string path, string outPath, string password = null, DelegateNoArg finish_CallBack = null, DelegateProgress progress_CallBack = null)
        {
            await Task.Run(() =>
            {
                ToZip(path, outPath, password, finish_CallBack, progress_CallBack);
            });
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="path">压缩文件路径</param>
        /// <param name="outPath">解压后的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="progress_CallBack">压缩进度更新的回调</param>
        public static void UnZip(string path, string outPath, string password, DelegateNoArg finish_CallBack = null, DelegateProgress progress_CallBack = null)
        {
            ZipInputStream zipInputStream = new ZipInputStream(File.Open(path, FileMode.Open));
            if (!string.IsNullOrEmpty(password))
                zipInputStream.Password = password;
            ZipEntry entry = null;
            FileInfo info = new FileInfo(path);
            //压缩包总字节数
            long size = info.Length;
            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                UnZip_Entry(outPath, entry, zipInputStream, size, progress_CallBack);
                //Debug.Log(index + "/" + zipInputStream.Length);
            }
            progress_CallBack?.Invoke(1);
            finish_CallBack?.Invoke();
        }
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="path">压缩文件路径</param>
        /// <param name="outPath">解压后的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="projress_CallBack">压缩进度更新的回调</param>
        public static async void UnZipAstnc(string path, string outPath, string password, DelegateNoArg finish_CallBack = null, DelegateProgress projress_CallBack = null)
        {
            await Task.Run(() =>
            {
                UnZip(path, outPath, password, finish_CallBack, projress_CallBack);
            });
        }


        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="bytes">压缩文件流</param>
        /// <param name="outPath">解压后的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="projress_CallBack">压缩进度更新的回调</param>
        public static void UnZip(byte[] bytes, string outPath, string password, DelegateNoArg finish_CallBack = null, DelegateProgress projress_CallBack = null)
        {
            ZipInputStream zipInputStream = new ZipInputStream(new MemoryStream(bytes));
            if (!string.IsNullOrEmpty(password))
                zipInputStream.Password = password;
            ZipEntry entry = null;
            //压缩包总字节数
            long size = bytes.Length;
            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                UnZip_Entry(outPath, entry, zipInputStream, size, projress_CallBack);
                //Debug.Log(index + "/" + zipInputStream.Length);
            }
            projress_CallBack?.Invoke(1);
            finish_CallBack?.Invoke();
        }
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="bytes">压缩文件流</param>
        /// <param name="outPath">解压后的存放路径</param>
        /// <param name="password">密码</param>
        /// <param name="finish_CallBack">压缩完成后的回调</param>
        /// <param name="projress_CallBack">压缩进度更新的回调</param>
        public static async void UnZipAstnc(byte[] bytes, string outPath, string password, DelegateNoArg finish_CallBack = null, DelegateProgress projress_CallBack = null)
        {
            await Task.Run(() =>
            {
                UnZip(bytes, outPath, password, finish_CallBack, projress_CallBack);
            });


        }





        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="fatherPath">父级目录，即选择的压缩根目录</param>
        /// <param name="path">带压缩文件夹的路径</param>
        /// <param name="zip">压缩文件的流对象</param>
        static void ToZip_Directory(string fatherPath, string path, ZipOutputStream zip)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            DirectoryInfo directoryInfo_F = new DirectoryInfo(fatherPath);
            string str = directoryInfo_F.FullName.Replace(directoryInfo_F.Name, "");
            str = directoryInfo.FullName.Replace(str, "");
            //文件夹最后一定要加上“\”
            ZipEntry entry = new ZipEntry(str + "\\");
            //entry.Size = 0;这一步多余，文件夹不需要指定大小
            entry.DateTime = DateTime.Now;

            zip.PutNextEntry(entry);
            //zip.Flush();
            zip.CloseEntry();
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fatherPath">父级目录，即选择的压缩根目录</param>
        /// <param name="path">带压缩文件的路径</param>
        /// <param name="zip">压缩文件的流对象</param>
        static void ToZip_File(string fatherPath, string path, ZipOutputStream zip)
        {
            FileInfo fileInfo = new FileInfo(path);
            DirectoryInfo directoryInfo_F = new DirectoryInfo(fatherPath);
            //string str = directoryInfo_F.FullName.Replace(directoryInfo_F.Name, "");
            string str = directoryInfo_F.Name + "/" + fileInfo.Name;
            //Replace 过滤掉父级目录
            ZipEntry entry = new ZipEntry(str);
            byte[] bytes = File.ReadAllBytes(path);
            entry.Size = bytes.Length;
            zip.PutNextEntry(entry);
            zip.Write(bytes, 0, bytes.Length);
            zip.CloseEntry();
        }

        static void UnZip_Entry(string fatherPath, ZipEntry entry, ZipInputStream zip, long size, DelegateProgress projress_CallBack = null)
        {

            if (entry == null)
            {
                return;
            }
            string path = fatherPath + entry.Name;
            //文件夹直接创建
            if (entry.IsDirectory)
            {
                Directory.CreateDirectory(path);
                return;
            }
            byte[] bytes = new byte[_speed];
            FileInfo info = new FileInfo(path);
            if (!Directory.Exists(info.DirectoryName))
            {
                Directory.CreateDirectory(info.DirectoryName);
            }
            //File.Create(path + ".txt");

            //try
            //{
            using (FileStream file = File.Create(path))
            {
                while (true)
                {
                    int count = zip.Read(bytes, 0, bytes.Length);
                    if (count > 0)
                    {
                        file.Write(bytes, 0, count);
                        projress_CallBack?.Invoke((float)zip.Position / size);
                    }
                    else
                    {

                        break;
                    }
                }
            }
            //}
            //catch
            //{
            //    Debug.Log("文件写入异常");
            //}
            //finally
            //{
            //    Debug.Log("写入结束（不论成功或失败！）");
            //}
            //return ;
        }




        /// <summary>
        /// 递归获取所有文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns></returns>
        public static List<string> GetDirectory_All(string path)
        {
            List<string> paths = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (var item in directoryInfo.GetDirectories())
            {
                paths.Add(item.FullName);
                paths.AddRange(GetDirectory_All(item.FullName));
            }
            return paths;
        }

        /// <summary>
        /// 递归获取所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetFiles_All(string path)
        {
            List<string> paths = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            paths.AddRange(directoryInfo.GetFiles().Select(p => p.FullName));
            foreach (var item in directoryInfo.GetDirectories())
            {
                paths.AddRange(GetFiles_All(item.FullName));
            }
            return paths;
        }

        public static long GetSizeFromFiles(List<string> paths)
        {
            long size = 0;
            foreach (var item in paths)
            {
                FileInfo fileInfo = new FileInfo(item);
                size += fileInfo.Length;
            }
            return size;
        }


        public static long GetSizeFromDir(string path)
        {
            long size = 0;
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                //处理当前文件夹中的文件
                foreach (var item in directoryInfo.GetFiles())
                {
                    size += GetSizeFromDir(item.FullName);
                }
                //继续遍历文件夹
                foreach (var item in directoryInfo.GetDirectories())
                {
                    size += GetSizeFromDir(item.FullName);
                }
            }
            else
            {
                FileInfo fileInfo = new FileInfo(path);
                size += fileInfo.Length;
            }
            return size;
        }

    }
}