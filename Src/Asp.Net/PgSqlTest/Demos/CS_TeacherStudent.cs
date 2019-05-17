using System;
using System.Linq;
using System.Text;

namespace  OrmTest.Demo
{
    public class CS_TeacherStudent
    {
        
        /// <summary>
        /// Desc:- 
        /// Default:(newid()) 
        /// Nullable:False 
        /// </summary>
        public Guid tabId {get;set;}

        /// <summary>
        /// Desc:教师Id 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string teacherId {get;set;}

        /// <summary>
        /// Desc:教师课程Id(对应TeacherCourse.tabId) 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string teacherCourseId {get;set;}

        /// <summary>
        /// Desc:教学头内的序号 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? ordInTC {get;set;}

        /// <summary>
        /// Desc:学号 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string stuId {get;set;}

        /// <summary>
        /// Desc:学生姓名 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string stuName {get;set;}

        /// <summary>
        /// Desc:性别 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string stuSex {get;set;}

        /// <summary>
        /// Desc:所属院系Id 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string deptId {get;set;}

        /// <summary>
        /// Desc:所属班级Id 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string classId {get;set;}

        /// <summary>
        /// Desc:不能对应班级代码的班级名称时源班级名称 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string sclassName {get;set;}

        /// <summary>
        /// Desc:- 
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public int? validFlag {get;set;}

    }
}
