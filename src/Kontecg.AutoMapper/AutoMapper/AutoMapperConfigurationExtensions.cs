using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Kontecg.Reflection.Extensions;

namespace Kontecg.AutoMapper
{
    internal static class AutoMapperConfigurationExtensions
    {
        private static readonly object SyncObj = new();

        public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type)
        {
            lock (SyncObj)
            {
                foreach (AutoMapAttributeBase autoMapAttribute in type.GetTypeInfo()
                             .GetCustomAttributes<AutoMapAttributeBase>())
                {
                    autoMapAttribute.CreateMap(configuration, type);
                }
            }
        }

        public static void CreateAutoAttributeMaps(this IMapperConfigurationExpression configuration, Type type,
            Type[] targetTypes, MemberList memberList)
        {
            //Get all the properties in the source that have the AutoMapKeyAttribute
            List<PropertyInfo> sourceKeysPropertyInfo = type.GetProperties()
                .Where(w => w.GetCustomAttribute<AutoMapKeyAttribute>() != null)
                .Select(s => s).ToList();

            foreach (Type targetType in targetTypes)
            {
                if (!sourceKeysPropertyInfo.Any())
                {
                    configuration.CreateMap(type, targetType, memberList);
                    continue;
                }

                BinaryExpression equalityComparer = null;

                //In a lambda expression represent the source example : (source) => ...
                ParameterExpression sourceParameterExpression = Expression.Parameter(type, "source");
                //In a lambda expression represent the target example : (target) => ...
                ParameterExpression targetParameterExpression = Expression.Parameter(targetType, "target");


                //We could use multiple AutoMapKey to compare the determine equality
                foreach (PropertyInfo propertyInfo in sourceKeysPropertyInfo)
                {
                    //In a lambda expression represent a specific property of a parameter example : (source) => source.Id
                    MemberExpression sourcePropertyExpression =
                        Expression.Property(sourceParameterExpression, propertyInfo);

                    //Find the target a property with the same name to compare with
                    //Example if we have in source the attribute AutoMapKey on the Property Id we want to get Id in the target to compare against
                    PropertyInfo targetPropertyInfo = targetType.GetProperty(sourcePropertyExpression.Member.Name);

                    //It happens if the property with AutoMapKeyAttribute does not exist in target
                    if (targetPropertyInfo is null)
                    {
                        continue;
                    }

                    //In a lambda expression represent a specific property of a parameter example : (target) => target.Id
                    MemberExpression targetPropertyExpression =
                        Expression.Property(targetParameterExpression, targetPropertyInfo);

                    //Compare the property defined by AutoMapKey in the source against the same property in the target
                    //Example (source, target) => source.Id == target.Id
                    BinaryExpression equal = Expression.Equal(sourcePropertyExpression, targetPropertyExpression);

                    equalityComparer = equalityComparer is null ? equal : Expression.And(equalityComparer, equal);
                }

                //If there is no match for AutoMapKey in the target
                //In this case we add the default mapping
                if (equalityComparer is null)
                {
                    configuration.CreateMap(type, targetType, memberList);
                    continue;
                }

                //We need to make a generic type of Func<SourceType, TargetType, bool> to invoke later Expression.Lambda
                Type funcGenericType = typeof(Func<,,>).MakeGenericType(type, targetType, typeof(bool));

                //Make a method info of Expression.Lambda<Func<SourceType, TargetType, bool>> to call later
                MethodInfo lambdaMethodInfo =
                    typeof(Expression).GetMethod("Lambda", 2, 1).MakeGenericMethod(funcGenericType);

                //Make the call to Expression.Lambda
                object expressionLambdaResult = lambdaMethodInfo.Invoke(null,
                    new object[] {equalityComparer, new[] {sourceParameterExpression, targetParameterExpression}});

                //Get the method info of IMapperConfigurationExpression.CreateMap<Source, Target>
                MethodInfo createMapMethodInfo = configuration.GetType().GetMethod("CreateMap", 1, 2)
                    .MakeGenericMethod(type, targetType);

                //Make the call to configuration.CreateMap<Source, Target>().
                object createMapResult = createMapMethodInfo.Invoke(configuration, new object[] {memberList});

                Assembly autoMapperCollectionAssembly = Assembly.Load("AutoMapper.Collection");

                Type[] autoMapperCollectionTypes = autoMapperCollectionAssembly.GetTypes();

                MethodInfo equalityComparisonGenericMethodInfo = autoMapperCollectionTypes
                    .Where(w => !w.IsGenericType && !w.IsNested)
                    .SelectMany(s => s.GetMethods()).Where(w => w.Name == "EqualityComparison")
                    .FirstOrDefault()
                    .MakeGenericMethod(type, targetType);

                //Make the call to EqualityComparison
                //Example configuration.CreateMap<Source, Target>().EqualityComparison((source, target) => source.Id == target.Id)
                equalityComparisonGenericMethodInfo.Invoke(createMapResult,
                    new[] {createMapResult, expressionLambdaResult});
            }
        }
    }
}
