using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace DevilAnalyzer.DA
{
    /// <summary>
    /// DevilTableのデータ内容を全部保持するクラス。
    /// </summary>
    class DevilTable
    {
        private DataTable m_DevilTableAll;

        private static DevilTable instance = new DevilTable();

        public static DevilTable GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// シングルトンなのでコンストラクタは読み取り専用
        /// </summary>
        private DevilTable()
        {
            string query = "select " +
            "No,Grade,Reality,TribeID,TribeName,Name " +
            "from DevilTable order by No ";
            DA.DataAccess da = new DA.DataAccess();
            m_DevilTableAll = da.readdata(query);
        }
        // DevilTableのデータを全て保持する。
        public DataTable DevilTableAll
        {
            get { return this.m_DevilTableAll; }
        }


    }
}
