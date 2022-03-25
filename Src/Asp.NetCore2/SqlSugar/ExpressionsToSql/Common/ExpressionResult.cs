using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class ExpressionResult
    {
        public bool IsLockCurrentParameter { get; set; }
        public bool IsUpper { get; set; }
        private ExpressionParameter _CurrentParameter;
        public ExpressionParameter CurrentParameter
        {
            get
            {
                return this._CurrentParameter;
            }
            set
            {
                Check.Exception(value != null && IsLockCurrentParameter, "CurrentParameter is locked.");
                this._CurrentParameter = value;
                this.IsLockCurrentParameter = false;
            }
        }
        #region constructor
        private ExpressionResult()
        {
        }
        public ExpressionResult(ResolveExpressType resolveExpressType)
        {
            this._ResolveExpressType = resolveExpressType;
        }
        #endregion

        #region Fields
        private ResolveExpressType _ResolveExpressType;
        private StringBuilder _Result;
        #endregion

        #region properties
        private StringBuilder Result
        {
            get
            {
                if (_Result == null) _Result = new StringBuilder();
                return _Result;
            }

            set
            {
                _Result = value;
            }
        }
        public bool LastCharIsSpace{
            get {
                if (_Result == null|| _Result.Length==0) return true;
                return _Result.ToString().Last() == UtilConstants.SpaceChar;
            }
        }
        #endregion
        public string GetString()
        {
            if (_Result == null) return null;
            if (IsUpper)
                return _Result.ToString().ToUpper().Replace(UtilConstants.ReplaceCommaKey,",").TrimEnd(',');
            else
                return _Result.ToString().Replace(UtilConstants.ReplaceCommaKey, ",").TrimEnd(',');
        }
        #region functions
        public string[] GetResultArray()
        {
            if (this._Result == null) return null;
            var reslut = new List<string>();

            if (IsUpper)
                reslut= this.Result.ToString().ToUpper().TrimEnd(',').Split(',').ToList();
            else
                reslut= this.Result.ToString().TrimEnd(',').Split(',').ToList();

            if (this.Result.ToString().Contains(UtilConstants.ReplaceCommaKey))
            {
                for (int i = 0; i < reslut.Count; i++)
                {
                    reslut[i] = reslut[i].Replace(UtilConstants.ReplaceCommaKey, ",");
                }
            }
            return reslut.ToArray();
        }

        public string GetResultString()
        {
            if (this._Result == null) return null;
            if (this._ResolveExpressType.IsIn(ResolveExpressType.SelectMultiple, ResolveExpressType.SelectSingle))
            {
                return this.Result.ToString().Replace(UtilConstants.ReplaceCommaKey, ",").TrimEnd(',');
            }
            if (IsUpper)
                return this.Result.ToString().Replace(UtilConstants.ReplaceCommaKey, ",").ToUpper();
            else
                return this.Result.ToString().Replace(UtilConstants.ReplaceCommaKey, ",");
        }

        public void TrimEnd()
        {
            if (this._Result == null) return;
            this.Result = this.Result.Remove(this.Result.Length - 1, 1);
        }

        public bool Contains(string value)
        {
            if (this.Result.Equals(value)) return true;
            return (this.Result.ToString().Contains(value));
        }

        internal void Insert(int index, string value)
        {
            if (this.Result == null) this.Result.Append(value);
            this.Result.Insert(index, value);
        }

        public void Append(object parameter)
        {
            if (this.CurrentParameter.HasValue() && this.CurrentParameter.AppendType.IsIn(ExpressionResultAppendType.AppendTempDate))
            {
                this.CurrentParameter.CommonTempData = parameter;
                return;
            }
            switch (this._ResolveExpressType)
            {
                case ResolveExpressType.ArraySingle:
                case ResolveExpressType.ArrayMultiple:
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                case ResolveExpressType.Update:
                    parameter = parameter + ",";
                    break;
                case ResolveExpressType.WhereSingle:
                    break;
                case ResolveExpressType.WhereMultiple:
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
            this.Result.Append(parameter);
        }

        public void AppendFormat(string parameter, params object[] orgs)
        {
            if (this.CurrentParameter.HasValue() && this.CurrentParameter.AppendType.IsIn(ExpressionResultAppendType.AppendTempDate))
            {
                this.CurrentParameter.CommonTempData = new KeyValuePair<string, object[]>(parameter, orgs);
                return;
            }
            switch (this._ResolveExpressType)
            {
                case ResolveExpressType.SelectSingle:
                case ResolveExpressType.SelectMultiple:
                    parameter = parameter + ",";
                    break;
                case ResolveExpressType.WhereSingle:
                    break;
                case ResolveExpressType.WhereMultiple:
                    break;
                case ResolveExpressType.FieldSingle:
                    break;
                case ResolveExpressType.FieldMultiple:
                    break;
                default:
                    break;
            }
            this.Result.AppendFormat(parameter, orgs);
        }

        public void Replace(string parameter, string newValue)
        {
            this.Result.Replace(parameter, newValue);
        }
        #endregion
    }
}
