using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class UpdateBuilder : IDMLBuilder
    {
        public ISqlBuilder Builder { get; internal set; }

        public SqlSugarClient Context
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ILambdaExpressions LambdaExpressions { get; internal set; }

        public List<SugarParameter> Parameters
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public StringBuilder sql
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string SqlTemplate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string ToSqlString()
        {
            throw new NotImplementedException();
        }
    }
}
