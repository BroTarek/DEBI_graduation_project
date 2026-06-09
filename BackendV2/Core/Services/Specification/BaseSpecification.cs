using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Services.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SoftBridge.Services.Specification
{
    public abstract class BaseSpecification<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public Expression<Func<TEntity, bool>> Criteria { get; protected set; }
        public List<Expression<Func<TEntity, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();
        public List<OrderExpressionInfo<TEntity>> OrderExpressions { get; } = new();
        public Expression<Func<TEntity, object>> OrderByDesc { get; protected set; }
        public Expression<Func<TEntity, object>> OrderBy { get; protected set; }
        public int Take { get; protected set; }
        public int Skip { get; protected set; }
        public bool IsPagingEnabled { get; protected set; }

        protected BaseSpecification() { }
        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            OrderExpressions.Add(new OrderExpressionInfo<TEntity> { Expression = orderByExpression, IsDescending = false });
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
            OrderExpressions.Add(new OrderExpressionInfo<TEntity> { Expression = orderByDescExpression, IsDescending = true });
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}
