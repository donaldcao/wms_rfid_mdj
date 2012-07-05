using System.Collections.Generic;
using System.Linq;

namespace THOK.RfidWms.DBModel.Ef.Models.MappingStrategy.Strategies
{
    public class CompositeMappingStrategy<T> : IMappingStrategy<T>
    {
        public IList<IMappingStrategy<T>> Strategies { get; set; }

        public T To(T from)
        {
            return Strategies.Aggregate(from, (current, strategy) => strategy.To(current));
        }
    }
}