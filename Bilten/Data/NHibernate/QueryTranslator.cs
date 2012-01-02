using System;
using System.Collections.Generic;
using System.Text;
using Bilten.Data.QueryModel;
using NHibernate;
using NHibernate.Criterion;

namespace Bilten.Data.NHibernate
{
    internal class QueryTranslator
    {
        private ICriteria _criteria;
        private Query _query;
        
        public QueryTranslator(ICriteria criteria, Query query)
        {
            _criteria = criteria;
            _query = query;
        }
        
        public void Execute()
        {
            Query myQuery = this._query;

            foreach (OrderClause clause in myQuery.OrderClauses)
            {
                _criteria.AddOrder(new Order(clause.PropertyName, clause.Criterion == OrderClause.OrderClauseCriteria.Ascending ? true : false));
            }

            // TODO: Potrebno je proveriti sledece ogranicenje: Za istu asocijaciju
            // (tj AssociationPath) nije moguce kombinovati CreateAlias i eager fetch
            // mode (poziv SetFetchMode sa parametrom FetchMode.Eager)
            foreach (AssociationAlias a in myQuery.Aliases)
            {
                // NOTE: Primetiti da CreateAlias daje inner join
                _criteria.CreateAlias(a.AssociationPath, a.AliasName);
            }

            foreach (Criterion myCriterion in myQuery.Criteria)
            {
                ICriterion criterion = null;
                if (myCriterion.Operator == CriteriaOperator.Equal)
                    criterion = Expression.Eq(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.NotEqual)
                    criterion = Expression.Not(Expression.Eq(myCriterion.PropertyName, myCriterion.Value));
                else if (myCriterion.Operator == CriteriaOperator.GreaterThan)
                    criterion = Expression.Gt(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.GreaterThanOrEqual)
                    criterion = Expression.Ge(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.LesserThan)
                    criterion = Expression.Lt(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.LesserThanOrEqual)
                    criterion = Expression.Le(myCriterion.PropertyName, myCriterion.Value);
                else if (myCriterion.Operator == CriteriaOperator.Like)
                {
                    if (myCriterion.CaseInsensitive)
                        criterion = Expression.InsensitiveLike(
                            myCriterion.PropertyName,
                            myCriterion.Value.ToString(),
                            convertMatchMode(myCriterion.MatchMode));
                    else
                        criterion = Expression.Like(
                            myCriterion.PropertyName,
                            myCriterion.Value.ToString(),
                            convertMatchMode(myCriterion.MatchMode));
                }
                else if (myCriterion.Operator == CriteriaOperator.NotLike)
                {
                    if (myCriterion.CaseInsensitive)
                        criterion = Expression.Not(Expression.InsensitiveLike(
                            myCriterion.PropertyName,
                            myCriterion.Value.ToString(),
                            convertMatchMode(myCriterion.MatchMode)));
                    else
                        criterion = Expression.Not(Expression.Like(
                            myCriterion.PropertyName,
                            myCriterion.Value.ToString(),
                            convertMatchMode(myCriterion.MatchMode)));
                }
                else if (myCriterion.Operator == CriteriaOperator.IsNull)
                    criterion = Expression.IsNull(myCriterion.PropertyName);
                else if (myCriterion.Operator == CriteriaOperator.IsNotNull)
                    criterion = Expression.IsNotNull(myCriterion.PropertyName);
                else
                    throw new ArgumentException("operator", "CriteriaOperator not supported in NHibernate Provider");

                if (_query.Operator == QueryOperator.And)
                    _criteria.Add(Expression.Conjunction().Add(criterion));
                else if (_query.Operator == QueryOperator.Or)
                    _criteria.Add(Expression.Disjunction().Add(criterion));
            }

            foreach (Query subQuery in myQuery.SubQueries)
            {
                QueryTranslator myTranslator = new QueryTranslator(_criteria, _query);
                myTranslator.Execute(); // Recursive Call
            }

            foreach (AssociationFetch f in myQuery.FetchModes)
            {
                FetchMode fetchMode = FetchMode.Default;
                if (f.FetchMode == AssociationFetchMode.Eager)
                    fetchMode = FetchMode.Eager;
                else if (f.FetchMode == AssociationFetchMode.Lazy)
                    fetchMode = FetchMode.Lazy;

                _criteria.SetFetchMode(f.AssociationPath, fetchMode);
            }
        }

        private MatchMode convertMatchMode(StringMatchMode stringMatchMode)
        {
            switch (stringMatchMode)
            { 
                case StringMatchMode.Exact:
                    return MatchMode.Exact;

                case StringMatchMode.Anywhere:
                    return MatchMode.Anywhere;

                case StringMatchMode.Start:
                    return MatchMode.Start;

                case StringMatchMode.End:
                    return MatchMode.End;

                default:
                    throw new ArgumentException("Invalid argument.", "stringMatchMode");
            }
        }
    }
}
