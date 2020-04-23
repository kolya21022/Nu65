using System;
using System.Collections.Generic;

using NU65.Entities.External;

namespace NU65.Entities.Interal
{
    /// <summary>
    /// Класс для отбражения в справочнике (работает быстрее)
    /// </summary>
    public class Nu65View : IComparable<Nu65View>
    {
        /// <summary>
        /// Код Nu65
        /// </summary>
        public long Id { get; set; }

        public Material Material { get; set; }

        public Product Product { get; set; }

        protected bool Equals(Nu65View other)
        {
            return Id == other.Id 
                   && Equals(Material, other.Material) 
                   && Equals(Product, other.Product);
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
            return Equals((Nu65View) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Material != null ? Material.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Product != null ? Product.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int CompareTo(Nu65View other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            var idComparison = Id.CompareTo(other.Id);
            if (idComparison != 0)
            {
                return idComparison;
            }
            var materialComparison = Comparer<Material>.Default.Compare(Material, other.Material);
            if (materialComparison != 0)
            {
                return materialComparison;
            }
            return Comparer<Product>.Default.Compare(Product, other.Product);
        }
    }
}
