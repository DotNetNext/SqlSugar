using System;

namespace OrmTest.BugTest
{
 
    public partial class TStock
    {
        public TStock()
        { }
        #region Model
        private string _pkid;
        private string _fk_store;
        private string _fstorename;
        private string _fmicode;
        private string _fminame;
        private decimal _fqty = 0M;
        private string _fremark;
        private DateTime? _fupdatetime = DateTime.Now;
        private string _flaster;
        private string _fk_fsp_id;
        private string _fspname;
        private string _fk_sfsp_id;
        private string _sfspname;
        private decimal? _fprice = 0M;
        private string _fk_materialinfo;
        private decimal? _stocktotalcost = 0M;
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
        public string FStoreName
        {
            set { _fstorename = value; }
            get { return _fstorename; }
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
        public decimal FQty
        {
            set { _fqty = value; }
            get { return _fqty; }
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
        public string FLaster
        {
            set { _flaster = value; }
            get { return _flaster; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_FSP_ID
        {
            set { _fk_fsp_id = value; }
            get { return _fk_fsp_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FSPName
        {
            set { _fspname = value; }
            get { return _fspname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_SFSP_ID
        {
            set { _fk_sfsp_id = value; }
            get { return _fk_sfsp_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SFSPName
        {
            set { _sfspname = value; }
            get { return _sfspname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FPrice
        {
            set { _fprice = value; }
            get { return _fprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_Materialinfo
        {
            set { _fk_materialinfo = value; }
            get { return _fk_materialinfo; }
        }
        /// <summary>
        /// 库存总成本
        /// </summary>
        public decimal? stockTotalCost
        {
            set { _stocktotalcost = value; }
            get { return _stocktotalcost; }
        }
        #endregion Model

    }
}