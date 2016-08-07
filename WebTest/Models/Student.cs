using System;
namespace Models
{

    public class Student
    {

        /// <summary>
        /// 说明:- 
        /// 默认:- 
        /// 可空:False 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 说明:- 
        /// 默认:- 
        /// 可空:True 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 说明:- 
        /// 默认:- 
        /// 可空:False 
        /// </summary>
        public int sch_id { get; set; }

        /// <summary>
        /// 说明:- 
        /// 默认:- 
        /// 可空:True 
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 说明:- 
        /// 默认:- 
        /// 可空:False 
        /// </summary>
        public bool isOk { get; set; }

    }




    public class Student2
    {

        public int id { get; set; }

        public string name { get; set; }

        public int sch_id { get; set; }

        public string sex { get; set; }



    }


}