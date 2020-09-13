using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM
{
    public interface IRelease
    {
        /// <summary>
        /// 释放资源
        /// </summary>
        void Release();
    }
}