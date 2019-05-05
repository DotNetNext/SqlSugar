using System;

namespace OrmTest.BugTest
{

  
    public partial class VMaterialInfo
    {
        public VMaterialInfo()
        { }
        #region Model
        private string _pkid;
        private string _fmicode;
        private string _fminame;
        private string _fpy;
        private string _fsiname;
        private string _fgauge;
        private string _foem;
        private string _fk_forigin;
        private string _fselfcode;
        private decimal? _flength;
        private decimal? _fwidth;
        private decimal? _fhigh;
        private decimal? _fweight;
        private bool _fishalf;
        private string _fbiname;
        private string _fuiname;
        private string _flsname;
        private int? _finboxqty;
        private string _fperformancetype;
        private bool _fassembly;
        private string _fptname;
        private decimal? _fretailprice;
        private decimal? _ftradeprice;
        private decimal? _fminimumprice;
        private decimal? _fminbuyqty;
        private string _fmanufacturernum;
        private string _fciname;
        private string _fspname;
        private string _deffspname;
        private DateTime? _fregdate;
        private string _fremark;
        private string _fadder;
        private string _bakspfspname;
        private string _fftname;
        private string _fstoretype;
        private string _fstate;
        private DateTime? _faddtime;
        private string _fk_seriesinfo;
        private string _fk_brandinfo;
        private string _fk_unitinfo;
        private string _fk_lablestyle;
        private string _fk_pricetype;
        private string _fk_clientinfo;
        private string _fk_storeplace;
        private string _fk_baksp;
        private string _fk_adder;
        private string _fk_flowtype;
        private string _lfptname;
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
        public string FPY
        {
            set { _fpy = value; }
            get { return _fpy; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FSIName
        {
            set { _fsiname = value; }
            get { return _fsiname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FGauge
        {
            set { _fgauge = value; }
            get { return _fgauge; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FOEM
        {
            set { _foem = value; }
            get { return _foem; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_FOrigin
        {
            set { _fk_forigin = value; }
            get { return _fk_forigin; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FSelfCode
        {
            set { _fselfcode = value; }
            get { return _fselfcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FLength
        {
            set { _flength = value; }
            get { return _flength; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FWidth
        {
            set { _fwidth = value; }
            get { return _fwidth; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FHigh
        {
            set { _fhigh = value; }
            get { return _fhigh; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FWeight
        {
            set { _fweight = value; }
            get { return _fweight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool FIsHalf
        {
            set { _fishalf = value; }
            get { return _fishalf; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FBIName
        {
            set { _fbiname = value; }
            get { return _fbiname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FUIName
        {
            set { _fuiname = value; }
            get { return _fuiname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FLSName
        {
            set { _flsname = value; }
            get { return _flsname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? FInBoxQty
        {
            set { _finboxqty = value; }
            get { return _finboxqty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FPerformanceType
        {
            set { _fperformancetype = value; }
            get { return _fperformancetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool FAssembly
        {
            set { _fassembly = value; }
            get { return _fassembly; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FPTName
        {
            set { _fptname = value; }
            get { return _fptname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FRetailPrice
        {
            set { _fretailprice = value; }
            get { return _fretailprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FTradePrice
        {
            set { _ftradeprice = value; }
            get { return _ftradeprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FMinimumPrice
        {
            set { _fminimumprice = value; }
            get { return _fminimumprice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? FMinBuyQty
        {
            set { _fminbuyqty = value; }
            get { return _fminbuyqty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FManufacturerNum
        {
            set { _fmanufacturernum = value; }
            get { return _fmanufacturernum; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FCIName
        {
            set { _fciname = value; }
            get { return _fciname; }
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
        public string DefFSPName
        {
            set { _deffspname = value; }
            get { return _deffspname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FRegDate
        {
            set { _fregdate = value; }
            get { return _fregdate; }
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
        public string FAdder
        {
            set { _fadder = value; }
            get { return _fadder; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BakSPFSPName
        {
            set { _bakspfspname = value; }
            get { return _bakspfspname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FFTName
        {
            set { _fftname = value; }
            get { return _fftname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FStoreType
        {
            set { _fstoretype = value; }
            get { return _fstoretype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FState
        {
            set { _fstate = value; }
            get { return _fstate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? FAddTime
        {
            set { _faddtime = value; }
            get { return _faddtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_SeriesInfo
        {
            set { _fk_seriesinfo = value; }
            get { return _fk_seriesinfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_BrandInfo
        {
            set { _fk_brandinfo = value; }
            get { return _fk_brandinfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_UnitInfo
        {
            set { _fk_unitinfo = value; }
            get { return _fk_unitinfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_LableStyle
        {
            set { _fk_lablestyle = value; }
            get { return _fk_lablestyle; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_PriceType
        {
            set { _fk_pricetype = value; }
            get { return _fk_pricetype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_ClientInfo
        {
            set { _fk_clientinfo = value; }
            get { return _fk_clientinfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_StorePlace
        {
            set { _fk_storeplace = value; }
            get { return _fk_storeplace; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_BakSP
        {
            set { _fk_baksp = value; }
            get { return _fk_baksp; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_Adder
        {
            set { _fk_adder = value; }
            get { return _fk_adder; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FK_FlowType
        {
            set { _fk_flowtype = value; }
            get { return _fk_flowtype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LFPTName
        {
            set { _lfptname = value; }
            get { return _lfptname; }
        }
        #endregion Model

    }
}