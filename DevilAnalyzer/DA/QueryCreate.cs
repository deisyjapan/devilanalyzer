using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevilAnalyzer.DA
{
    class QueryCreate
    {
        public static string CreateBaseQuery()
        {
            string query;

            query = "select " +
                "D.No as 全書No," +
                "D.Grade as グレード," +
                "D.Reality as レア度," +
                "D.TribeName as 種族名," +
                "D.Name as 悪魔名," +
                "AI.Name as AI," +
                "D.HP," +
                "D.Power as 力," +
                "D.Magic as 魔," +
                "D.Physical as 体," +
                "D.Speed as 速," +
                "D.Luck as 運," +
                "R1.Name as 物理," +
                "R2.Name as 火炎," +
                "R3.Name as 氷結," +
                "R4.Name as 電撃," +
                "R5.Name as 衝撃," +
                "R6.Name as 破魔," +
                "R7.Name as 呪殺," +
                "D.Skill1 as スキル1," +
                "D.Skill2 as スキル2," +
                "D.Skill3 as スキル3," +
                "D.Skill4 as 素体スキル," +
                "D.Skill4Rough as 荒神スキル," +
                "D.Skill4Protection as 加護スキル," +
                "D.Skill4Unusual as 異能スキル," +
                "D.Skill4Defense as 防魔スキル," +
                "D.Skill5Rough as 荒神プレミアム," +
                "D.Skill5Protection as 加護プレミアム," +
                "D.Skill5Unusual as 異能プレミアム," +
                "D.Skill5Defense as 防魔プレミアム " +
                "from    " +
                "DevilTable AS D left join ResistTable AS R1 on " +
                "D.PhysicsRegist = R1.ID left join ResistTable AS R2 on " +
                "D.FireRegist = R2.ID left join ResistTable AS R3 on " +
                "D.IceRegist = R3.ID left join ResistTable AS R4 on " +
                "D.ShockRegist = R4.ID left join ResistTable AS R5 on " +
                "D.ImpactRegist = R5.ID left join ResistTable AS R6 on " +
                "D.HamaRegist = R6.ID left join ResistTable AS R7 on " +
                "D.CurseRegist = R7.ID left join AITable AS AI on " +
                "D.AI = AI.ID ";
            return query;
        }

        public static string CreateDevilNameQuery(string DevilName)
        {
            string query = string.Format("(D.Name LIKE '%{0}%') ", DevilName);
            return query;
        }
        public static string CreateSkillNameQuery(string SkillName, bool Inherit)
        {
            string query;
            if (Inherit == false)
            {
                // 継承制限なしの時は全スキルフィールドが対象
                query = string.Format(
                    "(D.Skill1 = '{0}' " +
                    "or D.Skill2 = '{0}' " +
                    "or D.Skill3 = '{0}' " +
                    "or D.Skill4 = '{0}' " +
                    "or D.Skill4Rough = '{0}' " +
                    "or D.Skill4Protection = '{0}' " +
                    "or D.Skill4Unusual = '{0}' " +
                    "or D.Skill4Defense = '{0}' " +
                    "or D.Skill5Rough = '{0}' " +
                    "or D.Skill5Protection = '{0}' " +
                    "or D.Skill5Unusual = '{0}' " +
                    "or D.Skill5Defense = '{0}') "
                    , SkillName);
            }
            else
            {
                // 継承制限なしの時はスキル1とプレミアムスキルのみ対象
                query = string.Format(
                    "(D.Skill1 = '{0}' " +
                    "or D.Skill5Rough = '{0}' " +
                    "or D.Skill5Protection = '{0}' " +
                    "or D.Skill5Unusual = '{0}' " +
                    "or D.Skill5Defense = '{0}') "
                    , SkillName);

            }
            return query;
        }

        /// <summary>
        /// 対象属性のスキル持ちかどうかチェック。
        /// </summary>
        /// <param name="SkillName"></param>
        /// <returns></returns>
        public static string CreateSkillTypeQuery(int SkillKind, bool Inherit)
        {
            string query;
            if (Inherit == false)
            {
                query = string.Format("" +
                    "(D.Skill1 IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill2 IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill3 IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill4Rough IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill4Protection IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill4Unusual IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill4Defense IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Rough IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Protection IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Unusual IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Defense IN (SELECT Name from SkillTable where AttributeID = {0}) )"
                    , SkillKind);
            } else {
                query = string.Format("" +
                    "(D.Skill1 IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Rough IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Protection IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Unusual IN (SELECT Name from SkillTable where AttributeID = {0}) " +
                    "or D.Skill5Defense IN (SELECT Name from SkillTable where AttributeID = {0}) )"
                    , SkillKind);
            }
            return query;
        }
        /// <summary>
        /// 物理耐性
        /// </summary>
        /// <param name="RegistID"></param>
        /// <returns></returns>
        public static string CreateRegistQuery(int RegistKind, int RegistID)
        {
            string query = "";
            switch (RegistKind)
            {
                case 0:
                    query = string.Format(" (D.PhysicsRegist == {0} ) ", RegistID);
                    break;
                case 1:
                    query = string.Format(" (D.FireRegist == {0} ) ", RegistID);
                    break;
                case 2:
                    query = string.Format(" (D.IceRegist == {0} ) ", RegistID);
                    break;
                case 3:
                    query = string.Format(" (D.ShockRegist == {0} ) ", RegistID);
                    break;
                case 4:
                    query = string.Format(" (D.ImpactRegist == {0} ) ", RegistID);
                    break;
                case 5:
                    query = string.Format(" (D.HamaRegist == {0} ) ", RegistID);
                    break;
                case 6:
                    query = string.Format(" (D.CurseRegist == {0} ) ", RegistID);
                    break;
            }


            return query;
        }
        /// <summary>
        /// スキル名をキーにスキルの詳細情報を取得する
        /// </summary>
        public static string CreateGetSkillDetail(string SkillName)
        {
            string query = string.Format("select st.Name, " +
                "     st.Target, st.MP , " +
                "     st2.Name as AttributeName, " +
                "     st.Power, st.Count , " +
                "     st.Effect, st.MaxLV, " +
                "     st.LVUP, " +
                "     st.InheritancePoint " +
                "from SkillTable as st left join" +
                "     SkillAttributeTable as st1 on " +
                " 	  st.AttributeID = st1.AttributeID left join " +
                "     SkillAttributeTable as st2 on " +
                "	  st.AttributeID = st2.AttributeID left join " +
                "     SkillAttributeTable as st3 on " +
                " 	  st.AttributeID = st3.AttributeID left join " +
                "     SkillAttributeTable as st4 on " +
                "	  st.AttributeID = st4.AttributeID left join " +
                "     SkillAttributeTable as st5 on " +
                "	  st.AttributeID = st5.AttributeID left join " +
                "     SkillAttributeTable as st6 on " +
                "	  st.AttributeID = st6.AttributeID left join " +
                "     SkillAttributeTable as st7 on " +
                "	  st.AttributeID = st7.AttributeID left join " +
                "     SkillAttributeTable as st8 on " +
                "	  st.AttributeID = st8.AttributeID left join " +
                "     SkillAttributeTable as st9 on " +
                "	  st.AttributeID = st9.AttributeID left join " +
                "     SkillAttributeTable as st10 on " +
                "	  st.AttributeID = st10.AttributeID left join " +
                "     SkillAttributeTable as st11 on " +
                "	  st.AttributeID = st11.AttributeID left join " +
                "     SkillAttributeTable as st12 on " +
                "	  st.AttributeID = st12.AttributeID left join " +
                "     SkillAttributeTable as st13 on " +
                "	  st.AttributeID = st13.AttributeID left join " +
                "     SkillAttributeTable as st14 on " +
                "	  st.AttributeID = st14.AttributeID left join " +
                "     SkillAttributeTable as st15 on " +
                "     st.AttributeID = st15.AttributeID " +
                "where " +
                "     st.Name = '{0}'", SkillName);


            return query;

        }
        /// <summary>
        /// 種族名をキーにしてDevilBase列,DevilAdd列を取得する。
        /// (通常合体用の種族検索)
        /// </summary>
        /// <param name="DevilTribe">種族名</param>
        /// <returns></returns>
        public static string CreateGetUnionTribe(string DevilTribe)
        {
            //            string query = String.Format("select DevilBase,DevilAdd from DevilUnionTable " +
            //                                        "where " +
            //                                        "DevilAfter = '{0}'", DevilTribe);
            string query = String.Format("select DevilBase,DevilAdd from DevilUnionTable " +
                                        "where " +
                                        "DevilAfter = '{0}' ", DevilTribe);

            return query;
        }
        /// <summary>
        /// 種族名がDevilBase、DevilAdd列何れかに含まれるレコードのDevilBase列,DevilAdd列を取得する。
        /// (指定悪魔を素材とした合体パターン用の種族検索)
        /// </summary>
        /// <param name="DevilTribe"></param>
        /// <returns></returns>
        public static string CreateGetUnionTribeMaterial(string DevilTribe)
        {
            string query = String.Format("select DevilBase,DevilAdd,DevilAfter from DevilUnionTable " +
                                        "where " +
                                        "DevilBase = '{0}' or DevilAdd = '{0}' ", DevilTribe);

            return query;
        }

        /// <summary>
        /// 種族名をキーにして悪魔テーブルから合体に必要な情報を返す。
        /// </summary>
        /// <param name="DevilTribe"></param>
        /// <returns></returns>
        public static string CreateGetDevilTableUnionInfo(string DevilTribe)
        {
           string query = string.Format("select No ,Grade ,Reality,TribeName,Name " +
                "from DevilTable " +
                "where TribeName ='{0}' order by Grade", DevilTribe);
            return query;
        }
        /// <summary>
        /// 種族名をキーにして悪魔テーブルから合体に必要な情報を返す。
        /// (ユーザーが選択した悪魔の種族用、全書No1000以上をはじくことで
        /// 合体で作れない悪魔を結果から弾いている。)
        /// </summary>
        /// <param name="DevilTribe"></param>
        /// <returns></returns>
        public static string CreateGetDevilTableUnionInfoUserSelect(string DevilTribe)
        {
            // 種族名一致かつ全書Noが1000以下のものを対象とする。
            // 全書Noが1000を超えるものは合体では作れない悪魔なので対象に含めない。
            // No201 ミシャグジさま
            // No213 パールヴァティ
            string query = string.Format("select No ,Grade ,Reality,TribeName,Name " +
                "from DevilTable " +
                //"where TribeName ='{0}' and No <= 1000 order by Grade", DevilTribe);
                "where TribeName ='{0}' and " +
                "(No <= 1000 and No <> 201 and No <> 213) "+
                " order by Grade", DevilTribe);
            return query;
        }
        /*
        /// <summary>
        /// 種族名をキーにしてDevilAdd列を取得する。
        /// </summary>
        /// <param name="DevilTribe">種族名</param>
        /// <returns></returns>
        public static string CreateGetUnionTribleAdd(string DevilTribe)
        {
            string query = String.Format("select distinct DevilAdd from DevilUnionTable " +
                                        "where " +
                                        "DevilAfter = '{0}'", DevilTribe);
            return query;
        }
        /// <summary>
        /// 種族名をキーにしてDevilAfter列を取得する。
        /// 選択悪魔が何の素材になるかを確認するときに使うクエリ。
        /// 本当にこれでいいかは確認してないので実装時確認すること。
        /// </summary>
        /// <param name="DevilTribe">種族名</param>
        /// <returns></returns>
        public static string CreateGetUnionTribleAfter(string DevilTribe)
        {
            string query = String.Format("select distinct DevilAfter from DevilUnionTable " +
                                        "where " +
                                        "DevilBase = '{0}' or DevilAdd = '{0}'", DevilTribe);
            return query;
        }
*/

    }
}
