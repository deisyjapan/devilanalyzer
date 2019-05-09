using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

using DevilAnalyzer.DA;
namespace DevilAnalyzer
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>   
    public partial class SkillDetail : Window
    {
        public bool bTargetSkill{get; set;}
        public bool btarget;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SkillName">スキル名称</param>
        /// <param name="SkillArcheType">覚醒スキルの場合true</param>
        public SkillDetail(string SkillName, bool SkillAwake)
        {
            InitializeComponent();
            // スキル名称からテーブルを検索し、結果を画面に表示する。
            string query = getSkillTableData(SkillName);

            // 生成したクエリの実行
            DA.DataAccess da = new DA.DataAccess();
            DataTable table = new DataTable();
            table = da.readdata(query);

            // DataGridに紐付け
            // 各ラベルにセット
            if (table.Rows.Count != 0)
            {
                lblSkillName.Content = table.Rows[0]["Name"];
                lblTarget.Content = table.Rows[0]["Target"];
                lblAttribute.Content = table.Rows[0]["AttributeName"];
                lblMaxLV.Content = table.Rows[0]["MaxLV"];
                lblLVUP.Content = table.Rows[0]["LVUP"];
                lblInheritancePoint.Content = table.Rows[0]["InheritancePoint"];
                tbEffect.Text = table.Rows[0]["Effect"].ToString();



                // 以下の3項目は0の場合には「-」を表示する。
                if (SkillAwake == true)
                {
                    lblMP.Content = table.Rows[0]["MP"].ToString() == "0" ? "-" : table.Rows[0]["MP"].ToString() + "-1";
                }
                else
                {
                    lblMP.Content = table.Rows[0]["MP"].ToString() == "0" ? "-" : table.Rows[0]["MP"];
                }
                lblPower.Content = table.Rows[0]["Power"].ToString() == "0" ? "-" : table.Rows[0]["Power"];
                lblCount.Content = table.Rows[0]["Count"].ToString() == "0" ? "-" : table.Rows[0]["Count"];
                bTargetSkill = true;
            }
            else
            {
                MessageBox.Show("対象となるスキルが存在しません。");
                bTargetSkill = false;
            }




        }
        private string getSkillTableData(string SkillName)
        {
            DataTable dtSkill = new DataTable();
            string query = QueryCreate.CreateGetSkillDetail(SkillName);

            return query;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;

            }
        }
    }
}
