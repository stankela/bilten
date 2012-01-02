using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Data.QueryModel
{
    public sealed class Query
    {        
        private List<Criterion> criteria = new List<Criterion>();

        // members in the class being queried to be added to the result set.
        // private List<Member> members = new List<Member>;

        private QueryOperator @operator;
        private IList<Query> subQueries = new List<Query>();
        private IList<OrderClause> orderClauses = new List<OrderClause>();
        private IList<AssociationAlias> aliases = new List<AssociationAlias>();
        private IList<AssociationFetch> fetchModes = new List<AssociationFetch>();

        public IList<Criterion> Criteria
        {
            get
            {
                return criteria;
            }
        }

        public QueryOperator Operator
        {
            get
            {
                return @operator;
            }
            set
            {
                @operator = value;
            }
        }

        public IList<Query> SubQueries
        {
            get
            {
                return subQueries;
            }
        }

        public IList<OrderClause> OrderClauses
        {
            get
            {
                return orderClauses;
            }
        }

        public IList<AssociationAlias> Aliases
        {
            get { return aliases; }
        }

        public IList<AssociationFetch> FetchModes
        {
            get { return fetchModes; }
        }
    }
}