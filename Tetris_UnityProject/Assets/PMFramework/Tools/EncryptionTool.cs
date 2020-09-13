using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;



namespace PM
{
    /// <summary>
    /// 加密工具
    /// </summary>
    public class EncryptionTool
    {


        /// <summary>
        /// 获取MD5加密后的字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            MD5 mD5 = MD5.Create();
            byte[] result = mD5.ComputeHash(bytes);
            string s = "";
            foreach (var item in result)
            {
                //强制显示两位，默认首位是0的话会自动省略
                s += item.ToString("x2");
            }

            return s;
        }

    }
}