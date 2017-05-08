﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestPaperCreator.DAL.Utility;
using System.Data;

namespace TestPaperCreator.DAL.TestPaperService
{
    public class TestPaperService
    {
        #region 私有成员
        private static string conn = Conn.GetSqlServerConn("ALiConnection");
        #endregion

        #region 增删改筛选条件
        /// <summary>
        /// 增删筛选条件
        /// </summary>
        /// <param name="Condition">筛选条件(数据库表名，例如Course，Section)</param>
        /// <param name="Value">条件名称(显示名称，例如课程，章节)</param>
        /// <param name="Flag">1：生效，0：不生效</param>
        /// <returns></returns>
        public static bool InsertCourse(string Condition, string Value, int Flag)
        {
            string sql = "select Value from " + Condition + " where Value = '" + Value + "'";
            var result = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            if(result!=null)
            {
                sql = "insert into "+Condition+" (Value, Flag) values ('" + Value + "', " + Flag + ")";
            }
            else
            {
                sql = "update " + Condition + " set Flag =" + Flag + "  where Value = '" + Value + "'";
            }
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 查询课程
        public static List<MODEL.TestPaper.Condition> GetCourse()
        {
            List<MODEL.TestPaper.Condition> courselist = new List<MODEL.TestPaper.Condition>();
            MODEL.TestPaper.Condition course = new MODEL.TestPaper.Condition();
            string sql = "select ID, Value from Course where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                course.id = (int)dr[0];
                course.value = (string)dr[1];
                courselist.Add(course);
            }
            return courselist;
        }
        #endregion

        #region 查询难度
        public static List<MODEL.TestPaper.Condition> GetDifficulty()
        {
            List<MODEL.TestPaper.Condition> difficultylist = new List<MODEL.TestPaper.Condition>();
            MODEL.TestPaper.Condition difficulty = new MODEL.TestPaper.Condition();
            string sql = "select ID, Value from Difficulty where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                difficulty.id = (int)dr[0];
                difficulty.value = (string)dr[1];
                difficultylist.Add(difficulty);
            }
            return difficultylist;
        }
        #endregion

        #region 查询章节
        public static List<MODEL.TestPaper.Condition> GetSection()
        {
            List<MODEL.TestPaper.Condition> sectionlist = new List<MODEL.TestPaper.Condition>();
            MODEL.TestPaper.Condition section = new MODEL.TestPaper.Condition();
            string sql = "select ID, Value from Section where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                section.id = (int)dr[0];
                section.value = (string)dr[1];
                sectionlist.Add(section);
            }
            return sectionlist;
        }
        #endregion

        #region 查询题型
        public static List<MODEL.TestPaper.Condition> GetQuestionType()
        {
            List<MODEL.TestPaper.Condition> typelist = new List<MODEL.TestPaper.Condition>();
            MODEL.TestPaper.Condition type = new MODEL.TestPaper.Condition();
            string sql = "select ID, Value from Type where Flag = 1";
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                type.id = (int)dr[0];
                type.value = (string)dr[1];
                typelist.Add(type);
            }
            return typelist;
        }
        #endregion

        #region 获取试题ID最大值
        /// <summary>
        /// 获取试题ID最大值
        /// </summary>
        /// <returns>ID</returns>
        public static int GetMaxQuestionID()
        {
            string sql = "SELECT MAX(ID) as maxid FROM Questions";
            var maxid = SqlHelper.ExecuteScalar(conn, CommandType.Text, sql);
            //int maxid = SqlHelper.ExecuteNonQuery(conn, System.Data.CommandType.Text, sql);
            return maxid == null ? -1 : (int)maxid;
        }
        #endregion

        #region 插入试题
        /// <summary>
        /// 插入试题
        /// </summary>
        /// <param name="course">课程</param>
        /// <param name="questiontype">题型</param>
        /// <param name="section">章节</param>
        /// <param name="difficulty">难易度</param>
        /// <returns>成功或失败</returns>
        public static bool InsertQuestion(int course, int questiontype, int section, int difficulty, string content)
        {
            string sql = "insert into Questions (Type, Course, Section, Difficulty, Weight, Time, Flag, Content) values (" + questiontype + "," + course + "," + section + "," + difficulty + ",0,'" + DateTime.Now.ToShortDateString() + "',1,'" + content + "')";
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        #endregion

        #region 根据条件获取试题ID集合
        /// <summary>
        /// 根据条件获取试题ID集合
        /// </summary>
        /// <param name="paperlist">试题条件</param>
        /// <returns>试题ID集合</returns>
        public static List<int> GetQuestionID(List<MODEL.TestPaper.Paper> paperlist)
        {
            List<int> b = new List<int>();
            foreach (MODEL.TestPaper.Paper paper in paperlist)
            {
                int course = paper.papertype.course;
                int section = paper.papertype.section;
                int difficulty = paper.papertype.difficulty;
                int questiontype = paper.papertype.questiontype;
                int count = paper.count;
                string sql = "select ID from Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + " and Weight in(select MIN(Weight) from Questions)  and Flag=1";
                //string sql = "select ID form Questions where Course = " + course + " and Section =" + section + " and Difficulty =" + difficulty + " and Type = " + questiontype + "  and Flag=1";
                DataSet results = SqlHelper.ExecuteDataset(conn,CommandType.Text,sql);
                //results.Tables[0].Rows
                List<int> a = new List<int>();
                foreach(DataRow dr in results.Tables[0].Rows)
                {
                    a.Add((int)dr[0]);
                }
                Random rd = new Random();
                for (int i = 0; i < count; i++)
                {
                    int r = rd.Next(0, a.Count);
                    b.Add(a[r]);
                }
            }

            return b;
        }
        #endregion

        #region 根据条件查询试题
        /// <summary>
        /// 根据条件查询试题
        /// </summary>
        /// <param name="course">课程ID</param>
        /// <param name="section">章节ID</param>
        /// <param name="difficulty">难度ID</param>
        /// <param name="questiontype">题型ID</param>
        /// <param name="keyword">搜索关键字</param>
        /// <returns>试题列表</returns>
        public static List<MODEL.TestPaper.Question> GetQuestionList(int course, int section, int difficulty, int questiontype, string keyword)
        {
            string sql = "select questions.ID,questions.Type, questiontype.Value ,questions.Course, course.Value,questions.Section, section.Value,questions.Difficulty, difficulty.Value, questions.Content from Questions questions, Difficulty difficulty, Course course, Section section, Type questiontype where questions.Flag = 1 ";
            if (course != 0)
            {
                sql += "and Course = " + course;
            }
            if (section != 0)
            {
                sql += "and Section = " + section;
            }
            if (difficulty != 0)
            {
                sql += "and Difficulty = " + difficulty;
            }
            if (questiontype != 0)
            {
                sql += "and Type = " + questiontype;
            }
            if (keyword != null)
            {
                sql += "and Content like %'" + keyword + "'%";
            }
            List<MODEL.TestPaper.Question> questionlist = new List<MODEL.TestPaper.Question>();
            
            DataSet result = SqlHelper.ExecuteDataset(conn, CommandType.Text, sql);
            foreach (DataRow dr in result.Tables[0].Rows)
            {
                MODEL.TestPaper.Question question = new MODEL.TestPaper.Question();
                question.ID = (int)dr[0];
                question.Type = (int)dr[1];
                question.TypeName = (string)dr[2];
                question.Course = (int)dr[3];
                question.CourseName = (string)dr[4];
                question.Section = (int)dr[5];
                question.SectionName = (string)dr[6];
                question.Difficulty = (int)dr[7];
                question.DifficultyName = (string)dr[8];
                if(dr[9] != System.DBNull.Value)
                {
                    question.Content = (string)dr[9];
                }
                else
                {
                    question.Content = "";
                }
                questionlist.Add(question);
            }
            return questionlist;
        }
        #endregion
    }
}