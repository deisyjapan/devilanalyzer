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
    /// DevilUnion.xaml の相互作用ロジック
    /// パラメータは悪魔名と種族名
    /// </summary>
    public partial class DevilUnion : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="DevilName"></param>
        /// <param name="DevileTribe"></param>
        /// <param name="Grade"></param>
        /// <param name="Rare"></param>
        /// <param name="Click">ダブルクリック時に押したボタン</param>
        public DevilUnion(string DevilName ,string DevileTribe,int Grade, int Rare, MouseButton Click)
        {
            InitializeComponent();
            // 画面に選択した悪魔の情報をセット
            lblDevilName.Content = DevileTribe + "  " + DevilName;
            lblGrade.Content = Grade;
            lblReality.Content = Rare;
            if (Click == MouseButton.Left)
            {
                // 左ダブルクリック時はその悪魔を作るための合体を検索
                DataTable dtAnswer = SearchDevilUnion(DevilName, DevileTribe, Grade, Rare);
                dgUnionAnswer.DataContext = dtAnswer;
                lblUnionCount.Content = dtAnswer.Rows.Count.ToString();
                this.Title = "指定悪魔を作成するための組み合わせ";
            }
            else if (Click == MouseButton.Right)
            {
                // 右ダブルクリック時はその悪魔を使った合体を検索
                DataTable dtAnswer = SearchDevilMaterial(DevilName, DevileTribe, Grade, Rare);
                dgUnionAnswer.DataContext = dtAnswer;
                lblUnionCount.Content = dtAnswer.Rows.Count.ToString();
                this.Title = "指定悪魔を素材とするための組み合わせ";
            }
        }


        /// <summary>
        /// その悪魔を作るためにどんな合体で作れるか一覧を作る関数
        /// （結果が選択した悪魔）
        /// </summary>
        public DataTable SearchDevilUnion(string DevilName, string DevileTribe, int Grade, int Rare)
        {
            // 選択した悪魔の種族が作成可能な種族を取得
            string query = QueryCreate.CreateGetUnionTribe(DevileTribe);
            DA.DataAccess da = new DA.DataAccess();
            DataTable dtTribeInfo = new DataTable();
            dtTribeInfo = da.readdata(query);
            DataTable dtBaseInfo = new DataTable();
            DataTable dtAddInfo = new DataTable();
            DataTable dtAfterInfo = new DataTable();
            // この行はいらないかも。
            DevilTable dvlTbl = DevilTable.GetInstance();
            // 画面に表示する悪魔データの格納場所
            DataTable dtUnionList = new DataTable();

            dtUnionList.Columns.Add("グレード1", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("レア度1", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("種族1", Type.GetType("System.String"));
            dtUnionList.Columns.Add("悪魔名1", Type.GetType("System.String"));
            dtUnionList.Columns.Add("ｘ", Type.GetType("System.String"));
            dtUnionList.Columns.Add("グレード2", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("レア度2", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("種族2", Type.GetType("System.String"));
            dtUnionList.Columns.Add("悪魔名2", Type.GetType("System.String"));

            foreach (DataRow inforow in dtTribeInfo.Rows)
            {
                int i, j, k;
                string namebase = inforow["DevilBase"].ToString();
                string nameAdd = inforow["DevilAdd"].ToString();
                int minGrade = 0;
                int maxGrade = 0;

                //dvlTableから対象となる種族の悪魔情報を全件取得する。
                // 素材１のデータ
                query = QueryCreate.CreateGetDevilTableUnionInfo(namebase);
                dtBaseInfo = da.readdata(query);

                // 素材2のデータ
                query = QueryCreate.CreateGetDevilTableUnionInfo(nameAdd);
                dtAddInfo = da.readdata(query);

                // ユーザーが選択した種族のデータ
                //query = QueryCreate.CreateGetDevilTableUnionInfo(DevileTribe);
                query = QueryCreate.CreateGetDevilTableUnionInfoUserSelect(DevileTribe);
                dtAfterInfo = da.readdata(query);


                /*
                 * 計算した結果のグレード値が、対象種族の悪魔のどの悪魔になるのか調べる。
                 * afterInfoのGrad1<渡されたGrade<AfterInfoのgrade2の時、grade2の悪魔になる。
                 */
                for (i = 0; i < dtBaseInfo.Rows.Count; i++)
                {
                    int baseGrade = int.Parse(dtBaseInfo.Rows[i]["Grade"].ToString());
                    for (j = 0; j < dtAddInfo.Rows.Count; j++)
                    {
                        int AddGrade = int.Parse(dtAddInfo.Rows[j]["Grade"].ToString());
                        int calcGrade = (int)(Math.Floor((double)((baseGrade + AddGrade) / 2))) + 1;
                        for (k = 0; k < dtAfterInfo.Rows.Count - 1; k++)
                        {
                            int afterGrade1 = int.Parse(dtAfterInfo.Rows[k]["Grade"].ToString());
                            int afterGrade2 = int.Parse(dtAfterInfo.Rows[k + 1]["Grade"].ToString());

                            if (k == 0)
                            {
                                minGrade = afterGrade1;
                                maxGrade = int.Parse(dtAfterInfo.Rows[dtAfterInfo.Rows.Count-1]["Grade"].ToString()); ;
                            }

                            DataRow addListRow = dtUnionList.NewRow();

                            //}
                            if (afterGrade1 < calcGrade && afterGrade2 >= calcGrade)
                            {
                                // 計算したグレードが対象悪魔リストの現在行の値と次の行の値の間で
                                // 次のレコードのグレードと選択悪魔のグレードが同じなら作成できるパターン 
                                if (afterGrade2 == Grade)
                                {
                                    addListRow["グレード1"] = int.Parse(dtBaseInfo.Rows[i]["Grade"].ToString());
                                    addListRow["レア度1"] = int.Parse(dtBaseInfo.Rows[i]["Reality"].ToString());
                                    addListRow["種族1"] = dtBaseInfo.Rows[i]["TribeName"].ToString();
                                    addListRow["悪魔名1"] = dtBaseInfo.Rows[i]["Name"].ToString();
                                    addListRow["ｘ"] = "ｘ";
                                    addListRow["グレード2"] = int.Parse(dtAddInfo.Rows[j]["Grade"].ToString());
                                    addListRow["レア度2"] = int.Parse(dtAddInfo.Rows[j]["Reality"].ToString());
                                    addListRow["種族2"] = dtAddInfo.Rows[j]["TribeName"].ToString();
                                    addListRow["悪魔名2"] = dtAddInfo.Rows[j]["Name"].ToString();
                                    dtUnionList.Rows.Add(addListRow);
                                    continue;
                                }
                            } else if ((afterGrade1 >= calcGrade) && (Grade >= calcGrade) && (Grade == minGrade))
                            {
                                // 計算したグレードが作成される悪魔のグレードより低く、かつ作成される悪魔の一番低いグレードと一致する場合、作成可能とする。
                                addListRow["グレード1"] = int.Parse(dtBaseInfo.Rows[i]["Grade"].ToString());
                                addListRow["レア度1"] = int.Parse(dtBaseInfo.Rows[i]["Reality"].ToString());
                                addListRow["種族1"] = dtBaseInfo.Rows[i]["TribeName"].ToString();
                                addListRow["悪魔名1"] = dtBaseInfo.Rows[i]["Name"].ToString();
                                addListRow["ｘ"] = "ｘ";
                                addListRow["グレード2"] = int.Parse(dtAddInfo.Rows[j]["Grade"].ToString());
                                addListRow["レア度2"] = int.Parse(dtAddInfo.Rows[j]["Reality"].ToString());
                                addListRow["種族2"] = dtAddInfo.Rows[j]["TribeName"].ToString();
                                addListRow["悪魔名2"] = dtAddInfo.Rows[j]["Name"].ToString();
                                dtUnionList.Rows.Add(addListRow);
                                break;
                            //} else if ((afterGrade2 <= calcGrade) && (maxGrade < calcGrade) && (Grade < calcGrade) && (k == dtAfterInfo.Rows.Count - 1))
                            } else if ((calcGrade > maxGrade) && calcGrade < afterGrade1 && afterGrade2 < calcGrade) 
                            {
                                // 種族最大のグレードをもつ悪魔を計算したグレードが超えていたら、作成可能。
                                // 例、女神で最大グレードのラクシュミ(85)作成にヴィシュヌ(97)×アシェラト(83)でグレード91という計算になるが、作成可能。
                                // 個々を入れると最高ランクを作ったときの結果がおかしくなるっぽい。でも入れないと最小ランクの結果が出ない。どうしようか・・5/21
                                addListRow["グレード1"] = int.Parse(dtBaseInfo.Rows[i]["Grade"].ToString());
                                addListRow["レア度1"] = int.Parse(dtBaseInfo.Rows[i]["Reality"].ToString());
                                addListRow["種族1"] = dtBaseInfo.Rows[i]["TribeName"].ToString();
                                addListRow["悪魔名1"] = dtBaseInfo.Rows[i]["Name"].ToString();
                                addListRow["ｘ"] = "ｘ";
                                addListRow["グレード2"] = int.Parse(dtAddInfo.Rows[j]["Grade"].ToString());
                                addListRow["レア度2"] = int.Parse(dtAddInfo.Rows[j]["Reality"].ToString());
                                addListRow["種族2"] = dtAddInfo.Rows[j]["TribeName"].ToString();
                                addListRow["悪魔名2"] = dtAddInfo.Rows[j]["Name"].ToString();
                                dtUnionList.Rows.Add(addListRow);
                                break;
                            }

                        }
                    }
                }
            }
            return dtUnionList;
        }
        /// <summary>
        /// 指定された悪魔を使用した合体を全パターン検索する。
        /// </summary>
        /// <param name="DevilName"></param>
        /// <param name="DevileTribe"></param>
        /// <param name="Grade"></param>
        /// <param name="Rare"></param>
        /// <param name="Click"></param>
        /// <returns></returns>
        public DataTable SearchDevilMaterial(string DevilName, string DevileTribe, int Grade, int Rare)
        {
            //① deviluniontable.devliaddとdevilAfterに指定種族が含まれるパターンを列挙する。
            //② 列挙したら、指定悪魔と選択対象ではない種族の全悪魔との合体で何ができるか列挙して表示する。

            DataTable dtBaseInfo = new DataTable();     // DevilUnionTableから選択種族ではない方の悪魔情報を格納する
            DataTable dtAddInfo = new DataTable();      // DevilUnionTableのDevilAdd列情報
            DataTable dtAfterInfo = new DataTable();    // DevilUnionTableのDevilAfterに所属する悪魔をすべて格納する。
            int minGrade = 0;
            int maxGrade = 0;

            // 画面に表示する悪魔データの格納場所
            DataTable dtUnionList = new DataTable();

            dtUnionList.Columns.Add("グレード1", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("レア度1", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("種族1", Type.GetType("System.String"));
            dtUnionList.Columns.Add("悪魔名1", Type.GetType("System.String"));
            dtUnionList.Columns.Add("＝", Type.GetType("System.String"));
            dtUnionList.Columns.Add("グレード2", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("レア度2", Type.GetType("System.Int32"));
            dtUnionList.Columns.Add("種族2", Type.GetType("System.String"));
            dtUnionList.Columns.Add("悪魔名2", Type.GetType("System.String"));
            
            // 合体種族テーブルから、指定種族を含む合体パターンを抽出
            string query = QueryCreate.CreateGetUnionTribeMaterial(DevileTribe);
            DA.DataAccess da = new DA.DataAccess();
            DataTable dtTribeInfo = new DataTable();
            dtTribeInfo = da.readdata(query);

            foreach(DataRow inforow in dtTribeInfo.Rows)
            {
                // 抽出したパターンの内、指定種族ではない方の種族の全悪魔を取得
                if (inforow["DevilBase"].ToString() == DevileTribe)
                {
                    query = QueryCreate.CreateGetDevilTableUnionInfo(inforow["DevilAdd"].ToString());
                    dtBaseInfo = da.readdata(query);
                }
                else
                {
                    query = QueryCreate.CreateGetDevilTableUnionInfo(inforow["DevilBase"].ToString());
                    dtBaseInfo = da.readdata(query);
                }
                // 合体結果の悪魔を取得(結果に全書no1001以上の悪魔は含めない)
                query = QueryCreate.CreateGetDevilTableUnionInfoUserSelect(inforow["DevilAfter"].ToString());
                dtAfterInfo = da.readdata(query);

                // 指定悪魔と取得した悪魔を合体させて何ができるか割り出す

                // 対象悪魔の数分ループ
                foreach (DataRow infotargetrow in dtBaseInfo.Rows)
                {
                    // 指定悪魔のグレードと対象種族のグレードを計算して結果のグレード値を算出
                    int calcGrade = (int)(Math.Floor((double)(Grade + int.Parse(infotargetrow["Grade"].ToString())) / 2)) + 1;
                    // 合体結果種族の悪魔情報を全件取得
                    minGrade = int.Parse(dtAfterInfo.Rows[0]["Grade"].ToString());
                    maxGrade = int.Parse(dtAfterInfo.Rows[dtAfterInfo.Rows.Count - 1]["Grade"].ToString()); ;

                    // 対象となるグレードの悪魔を特定
                    for (int i=0; i< dtAfterInfo.Rows.Count - 1; i++)
                    {
                        int afterGrade1 = int.Parse(dtAfterInfo.Rows[i]["Grade"].ToString());
                        int afterGrade2 = int.Parse(dtAfterInfo.Rows[i + 1]["Grade"].ToString());
                        DataRow addListRow = dtUnionList.NewRow();
                        // 計算結果よりも対象種族の最低グレードの方が高ければ1レコード目確定
                        if (calcGrade < minGrade)
                        {
                            addListRow["グレード1"] = int.Parse(infotargetrow["Grade"].ToString());
                            addListRow["レア度1"] = int.Parse(infotargetrow["Reality"].ToString());
                            addListRow["種族1"] = infotargetrow["TribeName"].ToString();
                            addListRow["悪魔名1"] = infotargetrow["Name"].ToString(); 
                            addListRow["＝"] = "＝"; 
                            addListRow["グレード2"] = int.Parse(dtAfterInfo.Rows[i]["Grade"].ToString());
                            addListRow["レア度2"] = int.Parse(dtAfterInfo.Rows[i]["Reality"].ToString());
                            addListRow["種族2"] = dtAfterInfo.Rows[i]["TribeName"].ToString();
                            addListRow["悪魔名2"] = dtAfterInfo.Rows[i]["Name"].ToString();
                            dtUnionList.Rows.Add(addListRow);
                            break;

                        }
                        else if (afterGrade1 < calcGrade && afterGrade2 >= calcGrade)
                        {
                            // 計算したグレードが対象悪魔リストの現在行の値と次の行の値の間なら一つ上のグレードの悪魔が該当
                            addListRow["グレード1"] = int.Parse(infotargetrow["Grade"].ToString());
                            addListRow["レア度1"] = int.Parse(infotargetrow["Reality"].ToString());
                            addListRow["種族1"] = infotargetrow["TribeName"].ToString();
                            addListRow["悪魔名1"] = infotargetrow["Name"].ToString();
                            addListRow["＝"] = "＝";
                            addListRow["グレード2"] = int.Parse(dtAfterInfo.Rows[i+1]["Grade"].ToString());
                            addListRow["レア度2"] = int.Parse(dtAfterInfo.Rows[i+1]["Reality"].ToString());
                            addListRow["種族2"] = dtAfterInfo.Rows[i+1]["TribeName"].ToString();
                            addListRow["悪魔名2"] = dtAfterInfo.Rows[i+1]["Name"].ToString();
                            dtUnionList.Rows.Add(addListRow);
                            break;
                        }
                        else if (calcGrade > maxGrade) 
                        {
                            // 種族最大のグレードをもつ悪魔を計算したグレードが超えていたら、そのパターンは合体不可
                            /*
                            addListRow["グレード1"] = int.Parse(infotargetrow["Grade"].ToString());
                            addListRow["レア度1"] = int.Parse(infotargetrow["Reality"].ToString());
                            addListRow["種族1"] = infotargetrow["TribeName"].ToString();
                            addListRow["悪魔名1"] = infotargetrow["Name"].ToString();
                            addListRow["ｘ"] = "＝";
                            addListRow["グレード2"] = int.Parse(dtAfterInfo.Rows[dtAfterInfo.Rows.Count-1]["Grade"].ToString());
                            addListRow["レア度2"] = int.Parse(dtAfterInfo.Rows[dtAfterInfo.Rows.Count - 1]["Reality"].ToString());
                            addListRow["種族2"] = dtAfterInfo.Rows[dtAfterInfo.Rows.Count - 1]["TribeName"].ToString();
                            addListRow["悪魔名2"] = dtAfterInfo.Rows[dtAfterInfo.Rows.Count - 1]["Name"].ToString();
                            dtUnionList.Rows.Add(addListRow);
                            */
                            break;
                        }
                    }



                }
            }


            return dtUnionList;
        }

        /// <summary>
        /// データグリッドをクリックしたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgUnionAnswer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // データ0件なら処理しない
            if (dgUnionAnswer.Items.Count == 0)
            {
                return ;
            }
            // データの無い部分のクリックなら処理しない
            if (dgUnionAnswer.CurrentColumn == null)
            {
                return;
            }
            string strTitle = dgUnionAnswer.CurrentColumn.Header.ToString();
            string strCellValue;
            string strCellTribe;
            int iCellGrade;
            int iCellRare;

            switch (strTitle)
            {
                case "悪魔名1":
                    // 悪魔1の情報取得
                    strCellValue = ((TextBlock)dgUnionAnswer.CurrentColumn.GetCellContent(dgUnionAnswer.SelectedItem)).Text;
                    strCellTribe = ((TextBlock)dgUnionAnswer.Columns[2].GetCellContent(dgUnionAnswer.SelectedItem)).Text;
                    iCellGrade = int.Parse(((TextBlock)dgUnionAnswer.Columns[0].GetCellContent(dgUnionAnswer.SelectedItem)).Text);
                    iCellRare = int.Parse(((TextBlock)dgUnionAnswer.Columns[1].GetCellContent(dgUnionAnswer.SelectedItem)).Text);
                    // 自身のウィンドウを開く。
                    Window DevilUnionWindowBase = new DevilUnion(strCellValue, strCellTribe, iCellGrade, iCellRare, e.ChangedButton);
                    //DevilUnionWindowBase.ShowDialog();
                    DevilUnionWindowBase.Show();
                    break;
                case "悪魔名2":
                    // 悪魔2の情報取得
                    strCellValue = ((TextBlock)dgUnionAnswer.CurrentColumn.GetCellContent(dgUnionAnswer.SelectedItem)).Text;
                    strCellTribe = ((TextBlock)dgUnionAnswer.Columns[7].GetCellContent(dgUnionAnswer.SelectedItem)).Text;
                    iCellGrade = int.Parse(((TextBlock)dgUnionAnswer.Columns[5].GetCellContent(dgUnionAnswer.SelectedItem)).Text);
                    iCellRare = int.Parse(((TextBlock)dgUnionAnswer.Columns[6].GetCellContent(dgUnionAnswer.SelectedItem)).Text);
                    // 自身のウィンドウを開く。
                    Window DevilUnionWindowAdd = new DevilUnion(strCellValue, strCellTribe, iCellGrade, iCellRare, e.ChangedButton);
                    //DevilUnionWindowBase.ShowDialog();
                    DevilUnionWindowAdd.Show();
                    break;
            }

        }
        /// <summary>
        /// 合体では作成できない特殊な悪魔を判定する。
        /// 2018/10/12時点ではトリック・ランタンのみ。なお合体に使うことは出来る。
        /// ⇒合体の組み合わせを抽出する処理で全書No1001以上ははじくようにしたのでこの処理は不要。
        /// 異なる仕様の特殊な悪魔が出てきたときに使う。
        /// </summary>
        private bool UniqueDevilJedge(string devilName)
        {
            //
            bool bRet = false;
            return bRet;


        }

        private void UnionAnswer_KeyUp(object sender, KeyEventArgs e)
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
