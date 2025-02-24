using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TimorINSSBackEnd.DataContracts;

namespace TimorINSSBackEnd.Models
{
    public static class Utils
    {
        public static T2 MappClassToDto<T, T2>(T original) where T2 : new()
        {
            if (original == null)
                return default(T2);

            T2 mapped = new T2();
            var mappedType = original.GetType();

            //here we get all properties of the dto class with the atrribuite MapperAttribute
            IEnumerable<PropertyInfo> originalMapperProperties = typeof(T2).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MapperAttribute)));

            foreach (PropertyInfo prop in originalMapperProperties)
            {
                try
                {
                    prop.SetValue(mapped, mappedType.GetProperty(prop.Name).GetValue(original));
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Erro ao mapear a classe: {0} para classe: {1}", original.GetType().FullName, mapped.GetType().FullName), e);
                }
            }

            return mapped;
        }

        public static T2 MappClassFromDto<T, T2>(T original) where T2 : new()
        {
            if (original == null)
                return default(T2);

            T2 mapped = new T2();
            var mappedType = mapped.GetType();

            //here we get all properties of the dto class with the atrribuite MapperAttribute
            IEnumerable<PropertyInfo> originalMapperProperties = original.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(MapperAttribute)));

            foreach (PropertyInfo prop in originalMapperProperties)
            {
                try
                {
                    var targetProperty = mappedType.GetProperty(prop.Name);
                    var propValue = prop.GetValue(original);
                    if (propValue == null && targetProperty.PropertyType.IsValueType && Nullable.GetUnderlyingType(targetProperty.PropertyType) == null)
                    {
                        throw new ApplicationException(string.Format("Erro ao mapear a classe: {0}! o parametro: {1} não é nullable", original.GetType().FullName, prop.Name));
                    }
                    targetProperty.SetValue(mapped, propValue);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Erro ao mapear a classe: {0}", original.GetType().FullName), e);
                }
            }

            return mapped;
        }

        public static T UpdateClassWithoutVirtuals<T>(T original, T updated)
        {
            var mappedType = updated.GetType();

            //here we get all properties of the class with the atrribuite MapperAttribute
            var originalMapperProperties = original.GetType().GetProperties();
            foreach (PropertyInfo prop in originalMapperProperties)
            {
                try
                {
                    var propValue = prop.GetValue(original);
                    if (prop.GetMethod.IsVirtual && prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        IEnumerable col = propValue as IEnumerable;
                        var enumerator = col.GetEnumerator();
                        if (!enumerator.MoveNext())
                            propValue = null;
                    }

                    if (!prop.GetMethod.IsVirtual || (prop.GetMethod.IsVirtual && propValue != null))
                    {
                        if (propValue == null && prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null)
                        {
                            throw new ApplicationException(string.Format("Erro ao mapear a classe: {0}! o parametro: {1} não é nullable", original.GetType().FullName, prop.Name));
                        }

                        prop.SetValue(updated, propValue);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Erro ao mapear a classe: {0}", original.GetType().FullName), e);
                }
            }
            return updated;
        }

        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName, OrderDirectionEnum? orderDirection = OrderDirectionEnum.ascending)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return query;
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);

            string methodString = "OrderBy";
            if (orderDirection.HasValue)
                if (orderDirection.Value == OrderDirectionEnum.descending)
                    methodString = "OrderByDescending";

            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == methodString && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }
    }

    public class MapperAttribute : Attribute
    {
    }
}