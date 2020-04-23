using System;
using System.Collections.Generic;

using NU65.Entities.External;

namespace NU65.Entities.Interal
{
    /// <summary>
    /// Основной справочник
    /// </summary>
    public class Nu65 : IComparable<Nu65>
    {
        public Product Product { get; set; }

        public List<Material> Materials { get; set; }

        protected bool Equals(Nu65 other)
        {
            return Equals(Product, other.Product) 
                   && Equals(Materials, other.Materials);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Nu65) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Product != null ? Product.GetHashCode() : 0) * 397) 
                       ^ (Materials != null ? Materials.GetHashCode() : 0);
            }
        }

        public int CompareTo(Nu65 other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            return Comparer<Product>.Default.Compare(Product, other.Product);
        }
    }
}
