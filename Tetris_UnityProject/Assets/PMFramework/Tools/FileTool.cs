using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace PM
{
    public class FileTool
    {


        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <returns></returns>
        static List<FileInfo> GetAllFileInfo(DirectoryInfo directoryInfo)
        {
            List<FileInfo> list = new List<FileInfo>();
            list.AddRange(directoryInfo.GetFiles());
            foreach (var item in directoryInfo.GetDirectories())
            {
                list.AddRange(GetAllFileInfo(item));
            }
            return list;
        }


        /// <summary>
        /// 获取文件尺寸
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSizeOfFile(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                return "0KB";
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            List<FileInfo> fileList = GetAllFileInfo(directoryInfo);
            long length = 0;
            foreach (var item in fileList)
            {

                length += item.Length;
            }
            int count = 0;
            float size = length;
            for (int i = 0; i < 3; i++)
            {
                if (size <= 1024)
                {
                    break;
                }
                size /= 1024.0f;
                count++;
            }
            string result = size.ToString("f2") + "";
            switch (count)
            {
                case 0:
                    result += "b";
                    break;
                case 1:
                    result += "KB";
                    break;
                case 2:
                    result += "MB";
                    break;
                case 3:
                    result += "GB";
                    break;
                case 4:
                    result += "TB";
                    break;
                default:
                    break;
            }
            return result;


        }
    }
}