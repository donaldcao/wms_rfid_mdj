using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity.Core.Objects;

namespace THOK.Common.Entity
{
    public static class ObjectContextExtension
    {
        public static int ExecuteStoreCommand<T>(this ObjectSet<T> entity, string commandText, params object[] args) where T : class,new()
        {
            return entity.Context.ExecuteStoreCommand(commandText, args);
        }
    }
}
