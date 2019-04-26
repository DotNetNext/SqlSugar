using System;

namespace OrmTest.BugTest
{
    public partial class TTempStock
    {
        public TTempStock()
        { }
        #region Model
        private string _pkid;
        private string _fk_store;
        private string _fstore;
        private string _fmicode;
        private string _fminame;
        private decimal _fkcsl = 0M;
        private string _fbillno;
        private string _fbilltype;
        private string _fremark;
        private DateTime? _fupdatetime;
        private string _fk_materialinfo;
        /// <summary>
        /// 
        /// </summary>
        public string PKID
        {
            set { _pkid = value; }
            get { return _pkid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_Store
        {
            set { _fk_store = value; }
            get { return _fk_store; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FStore
        {
            set { _fstore = value; }
            get { return _fstore; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FMICode
        {
            set { _fmicode = value; }
            get { return _fmicode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FMIName
        {
            set { _fminame = value; }
            get { return _fminame; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal FKCSL
        {
            set { _fkcsl = value; }
            get { return _fkcsl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FBillNo
        {
            set { _fbillno = value; }
            get { return _fbillno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FBillType
        {
            set { _fbilltype = value; }
            get { return _fbilltype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FRemark
        {
            set { _fremark = value; }
            get { return _fremark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FUpdateTime
        {
            set { _fupdatetime = value; }
            get { return _fupdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_Materialinfo
        {
            set { _fk_materialinfo = value; }
            get { return _fk_materialinfo; }
        }
        #endregion Model

    }
}