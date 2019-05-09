//
// 更新履歴
// 2018/09/14
// 名前、スキル名検索のテキストボックスでEnterを押した場合検索ボタンを押した処理を行うように修正。
//
//
//


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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Data;
using DevilAnalyzer.DA;

namespace DevilAnalyzer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                DevilTable.GetInstance();
            }
            catch (Exception ex)
            {
                MessageBox.Show("起動時にエラーが発生しました。アプリケーションを終了します。再度アプリケーションを起動してみてください。");
                Application.Current.Shutdown();
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // 基本のクエリ作成
                DataTable table = new DataTable();
                bool bInherit = cbInherit.IsChecked == true ? true : false;
                string query = QueryCreate.CreateBaseQuery();

                query = checkSearchWhere(query);
                query = SearchName(query, bInherit);
                if (query == "")
                {
                    return;
                }
                query = SearchSkillAttribute(query, bInherit);
                if (query == "")
                {
                    return;
                }
                query = SearchRegist(query);
                if (query == "")
                {
                    return;
                }

                query = query + " order by grade";
                // 生成したクエリの実行
                DA.DataAccess da = new DA.DataAccess();
                table = da.readdata(query);

                // DataGridに紐付け
                dgSearch.DataContext = table;
            }
            catch (Exception ex)
            {
                string err = string.Format("例外発生　innner={0} Message={1}",ex.InnerException,ex.Message);
                MessageBox.Show(this,err, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// 名称に関する検索処理
        /// </summary>
        /// <param name="query"></param>
        /// <param name="bInherit"></param>
        /// <returns></returns>
        private string SearchName(string query,bool bInherit)
        {
            // 悪魔名称の検索クエリ作成
            if (txtDevilName.Text != "")
            {
                query = query + QueryCreate.CreateDevilNameQuery(txtDevilName.Text);
            }

            // スキル名の検索クエリ作成
            if (txtSkillName.Text != "")
            {
                if (txtDevilName.Text != "")
                {
                    MessageBox.Show(this, "悪魔名とスキル名を同時に入れて検索はできません","エラー",MessageBoxButton.OK,MessageBoxImage.Error);
                    return "";
                }
                else
                {
                    query = query + QueryCreate.CreateSkillNameQuery(txtSkillName.Text, bInherit);
                }
            }
            return query;
        }

        private string SearchSkillAttribute(string query, bool bInherit)
        {
            // 複数条件を組み合わせた時の条件数
            int iChkCnt = 0;

            bool bAttributeCheck = false;
            // スキル属性の検索処理
            // スキルのチェックボックスを配列に
            CheckBox[] skillCbArray = new CheckBox[] { cbSkillAttribute1, cbSkillAttribute2,
                cbSkillAttribute3, cbSkillAttribute4, cbSkillAttribute5, cbSkillAttribute6,
                cbSkillAttribute7, cbSkillAttribute8, cbSkillAttribute9, cbSkillAttribute10,
                cbSkillAttribute11, cbSkillAttribute12, cbSkillAttribute13, cbSkillAttribute14,
                cbSkillAttribute15 };

            // チェックされている属性のクエリを作成
            for (int i = 0; i < skillCbArray.Length; i++)
            {
                if (skillCbArray[i].IsChecked == true)
                {
                    //where句の最初にandをつけないための処理。
                    iChkCnt += 1;
                    if (iChkCnt != 1)
                    {
                        query = query + " and ";
                    }
                    query = query + QueryCreate.CreateSkillTypeQuery(i + 1, bInherit);
                    bAttributeCheck = true;
                }
            }
            // スキル属性チェックのいずれかがチェックされている場合で、名前検索に入力がある場合、検索不可
            if (bAttributeCheck == true && (txtSkillName.Text != "" || txtDevilName.Text != ""))
            {
                MessageBox.Show(this,"悪魔名、スキル名の検索と所持スキル属性を同時に指定することはできません。","エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

            return query;

        }
        private string SearchRegist(string query)
        {
            bool bRegistCheck = false;
            // チェックボックス配列化 すべての要素数は同一にしておくこと。
            // 物理
            CheckBox[] cbPhysicsArray = new CheckBox[] { cbPhysicsWeek, cbPhysics, cbPhysicsRegist, cbPhysicsNone, cbPhysicsSuction, cbPhysicsReflection };
            // 火炎
            CheckBox[] cbFireArray = new CheckBox[] { cbFireWeek, cbFire, cbFireRegist, cbFireNone, cbFireSuction, cbFireReflection };
            // 氷結
            CheckBox[] cbIceArray = new CheckBox[] { cbIceWeek, cbIce, cbIceRegist, cbIceNone, cbIceSuction, cbIceReflection };
            // 電撃
            CheckBox[] cbShockArray = new CheckBox[] { cbShockWeek, cbShock, cbShockRegist, cbShockNone, cbShockSuction, cbShockReflection };
            // 衝撃
            CheckBox[] cbImpactArray = new CheckBox[] { cbImpactWeek, cbImpact, cbImpactRegist, cbImpactNone, cbImpactSuction, cbImpactReflection };
            // 破魔
            CheckBox[] cbHamaArray = new CheckBox[] { cbHamaWeek, cbHama, cbHamaRegist, cbHamaNone, cbHamaSuction, cbHamaReflection };
            // 呪殺
            CheckBox[] cbCurseArray = new CheckBox[] { cbCurseWeek, cbCurse, cbCurseRegist, cbCurseNone, cbCurseSuction, cbCurseReflection };

            // WHERE句が存在する場合のみ処理
            if (query.IndexOf("WHERE ") != -1)
            {
                if (cbPhysicsWeek.IsChecked == true || cbPhysics.IsChecked == true || cbPhysicsRegist.IsChecked == true ||
                cbPhysicsNone.IsChecked == true || cbPhysicsSuction.IsChecked == true || cbPhysicsReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbPhysicsArray, 0);
                    bRegistCheck = true;
                }

                if (cbFireWeek.IsChecked == true || cbFire.IsChecked == true || cbFireRegist.IsChecked == true ||
                 cbFireNone.IsChecked == true || cbFireSuction.IsChecked == true || cbFireReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbFireArray, 1);
                    bRegistCheck = true;
                }
                if (cbIceWeek.IsChecked == true || cbIce.IsChecked == true || cbIceRegist.IsChecked == true ||
                    cbIceNone.IsChecked == true || cbIceSuction.IsChecked == true || cbIceReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbIceArray, 2);
                    bRegistCheck = true;
                }
                if (cbShockWeek.IsChecked == true || cbShock.IsChecked == true || cbShockRegist.IsChecked == true ||
                    cbShockNone.IsChecked == true || cbShockSuction.IsChecked == true || cbShockReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbShockArray, 3);
                    bRegistCheck = true;
                }
                if (cbImpactWeek.IsChecked == true || cbImpact.IsChecked == true || cbImpactRegist.IsChecked == true ||
                    cbImpactNone.IsChecked == true || cbImpactSuction.IsChecked == true || cbImpactReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbImpactArray, 4);
                    bRegistCheck = true;
                }
                if (cbHamaWeek.IsChecked == true || cbHama.IsChecked == true || cbHamaRegist.IsChecked == true ||
                    cbHamaNone.IsChecked == true || cbHamaSuction.IsChecked == true || cbHamaReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbHamaArray, 5);
                    bRegistCheck = true;
                }
                if (cbCurseWeek.IsChecked == true || cbCurse.IsChecked == true || cbCurseRegist.IsChecked == true ||
                    cbCurseNone.IsChecked == true || cbCurseSuction.IsChecked == true || cbCurseReflection.IsChecked == true)
                {
                    query = SearchRegistSub(query, cbCurseArray, 6);
                    bRegistCheck = true;
                }
                // 耐性チェックのいずれかがチェックされている場合で、名前検索に入力がある場合、検索不可
                if (bRegistCheck == true && (txtSkillName.Text != "" || txtDevilName.Text != ""))
                {
                    MessageBox.Show(this,"悪魔名、スキル名の検索と耐性を同時に指定することはできません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }

            }
            return query;
        }

        private string SearchRegistSub(string query, CheckBox[] cbArray,int RegistKind)
        {
            int iChkCnt = 0;

            // where句が存在し、直後でなければほかの条件があるのでANDをつける。
            if (query.IndexOf("WHERE ") != query.Length - 6)
            {
                query = query + " AND ";
            }
            query = query + "(";
            for (int i = 0; i < cbArray.Length; i++)
            {
                if (cbArray[i].IsChecked == true)
                {
                    //最初にorつけないための処理。
                    iChkCnt += 1;
                    if (iChkCnt != 1)
                    {
                        query = query + " or ";
                    }
                    query = query + QueryCreate.CreateRegistQuery(RegistKind, i);
                }
            }
            query = query + ")";
            return query;
        }
        /// <summary>
        /// 検索条件の有無をチェックし、何か条件があればWHEREをつける
        /// </summary>
        private string checkSearchWhere(string query)
        {
            // 検索条件の有無をチェックし、条件があればWHEREを追加
            // 検索条件をここで全部チェックすること。
            if (txtDevilName.Text != "" || txtSkillName.Text != "" || cbSkillAttribute1.IsChecked == true || 
                cbSkillAttribute2.IsChecked == true || cbSkillAttribute3.IsChecked == true || cbSkillAttribute4.IsChecked == true || 
                cbSkillAttribute5.IsChecked == true || cbSkillAttribute6.IsChecked == true || cbSkillAttribute7.IsChecked == true || 
                cbSkillAttribute8.IsChecked == true || cbSkillAttribute9.IsChecked == true || cbSkillAttribute10.IsChecked == true || 
                cbSkillAttribute11.IsChecked == true || cbSkillAttribute12.IsChecked == true || cbSkillAttribute13.IsChecked == true || 
                cbSkillAttribute14.IsChecked == true || cbSkillAttribute15.IsChecked == true ||
                cbPhysicsWeek.IsChecked == true || cbPhysics.IsChecked == true || cbPhysicsRegist.IsChecked == true || 
                cbPhysicsNone.IsChecked == true || cbPhysicsSuction.IsChecked == true || cbPhysicsReflection.IsChecked == true ||
                cbFireWeek.IsChecked == true || cbFire.IsChecked == true || cbFireRegist.IsChecked == true ||
                cbFireNone.IsChecked == true || cbFireSuction.IsChecked == true || cbFireReflection.IsChecked == true ||
                cbIceWeek.IsChecked == true || cbIce.IsChecked == true || cbIceRegist.IsChecked == true ||
                cbIceNone.IsChecked == true || cbIceSuction.IsChecked == true || cbIceReflection.IsChecked == true ||
                cbShockWeek.IsChecked == true || cbShock.IsChecked == true || cbShockRegist.IsChecked == true ||
                cbShockNone.IsChecked == true || cbShockSuction.IsChecked == true || cbShockReflection.IsChecked == true ||
                cbImpactWeek.IsChecked == true || cbImpact.IsChecked == true || cbImpactRegist.IsChecked == true ||
                cbImpactNone.IsChecked == true || cbImpactSuction.IsChecked == true || cbImpactReflection.IsChecked == true ||
                cbHamaWeek.IsChecked == true || cbHama.IsChecked == true || cbHamaRegist.IsChecked == true ||
                cbHamaNone.IsChecked == true || cbHamaSuction.IsChecked == true || cbHamaReflection.IsChecked == true ||
                cbCurseWeek.IsChecked == true || cbCurse.IsChecked == true || cbCurseRegist.IsChecked == true ||
                cbCurseNone.IsChecked == true || cbCurseSuction.IsChecked == true || cbCurseReflection.IsChecked == true
               )
            {
                return query + "WHERE ";
            }

            return query;
        }


        /// <summary>
        /// 検索結果のセルをダブルクリックしたときの処理
        /// </summary>
        private void dgSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 処理の無いセルをダブルクリックし続けていると落ちる現象が発生したため、
            // 処理の無いセルの場合何もせず関数を抜けるよう修正。
            try
            {
                // データ件数0なら処理しない
                if (dgSearch.Items.Count == 0)
                {
                    return;
                }
                // データの無い部分のクリックなら処理しない
                if (dgSearch.CurrentColumn == null)
                {
                    return;
                }
                string strTitle = dgSearch.CurrentColumn.Header.ToString();
                switch (strTitle)
                {
                    case "スキル1":
                    case "スキル2":
                    case "スキル3":
                    case "荒神プレミアム":
                    case "加護プレミアム":
                    case "異能プレミアム":
                    case "防魔プレミアム":
                    case "素体スキル":
                    case "荒神スキル":
                    case "加護スキル":
                    case "異能スキル":
                    case "防魔スキル":
                    case "悪魔名":
                        if (dgSearch.SelectedItem != null)
                        {
                            // 1行目でタイトル取得、2行目でセルの値取得
                            //MessageBox.Show(dgSearch.CurrentColumn.Header.ToString() + ";" +
                            //    ((TextBlock)dgSearch.CurrentColumn.GetCellContent(dgSearch.SelectedItem)).Text);

                            string strCellValue = ((TextBlock)dgSearch.CurrentColumn.GetCellContent(dgSearch.SelectedItem)).Text;
                            // 種族名取得
                            string strCellTribe = ((TextBlock)dgSearch.Columns[3].GetCellContent(dgSearch.SelectedItem)).Text;
                            int iCellGrade = int.Parse(((TextBlock)dgSearch.Columns[1].GetCellContent(dgSearch.SelectedItem)).Text);
                            int iCellRare = int.Parse(((TextBlock)dgSearch.Columns[2].GetCellContent(dgSearch.SelectedItem)).Text);

                            SkillDetail SkillDetailWindow;

                            switch (strTitle)
                            {
                                case "スキル1":
                                case "スキル2":
                                case "スキル3":
                                case "荒神プレミアム":
                                case "加護プレミアム":
                                case "異能プレミアム":
                                case "防魔プレミアム":
                                    // スキル関係の場合。スキル詳細ウィンドウを開く
                                    SkillDetailWindow = new SkillDetail(strCellValue, false);
                                    if (SkillDetailWindow.bTargetSkill == true)
                                    {
                                        SkillDetailWindow.ShowDialog();
                                    }
                                    break;

                                case "素体スキル":
                                case "荒神スキル":
                                case "加護スキル":
                                case "異能スキル":
                                case "防魔スキル":
                                    // 覚醒スキル関係の場合。スキル詳細ウィンドウを開き、MPには-1を表示する。
                                    SkillDetailWindow = new SkillDetail(strCellValue, true);
                                    if (SkillDetailWindow.bTargetSkill == true)
                                    {
                                        SkillDetailWindow.ShowDialog();
                                    }
                                    break;
                                case "悪魔名":
                                    // 悪魔名の場合、その悪魔を作成可能な組み合わせを表示する。
                                    Window DevilUnionWindow = new DevilUnion(strCellValue, strCellTribe, iCellGrade, iCellRare, e.ChangedButton);
                                    //DevilUnionWindow.ShowDialog();
                                    DevilUnionWindow.Show();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("エラーが発生しました。行った操作を添えて開発者に連絡願います。 "));
            }



        }

        private void txtDevilName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                btnSearch_Click(sender, e);
            }
        }

        private void txtSkillName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                btnSearch_Click(sender, e);
            }

        }

        private void DevilAnalyzer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("終了します。よろしいですか？", "終了確認", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {

                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }

        }

        private void DevilAnalyzer_Closed(object sender, EventArgs e)
        {

        }
    }
}
