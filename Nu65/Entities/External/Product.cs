using System;
using System.Globalization;

namespace NU65.Entities.External
{
    /// <summary>
    /// Изделие
    /// </summary>
    public class Product : IComparable<Product> 
    {
        /// <summary>
        /// Код изделия
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заводской код продукта
        /// </summary>
        public long CodeProduct { get; set; }

        /// <summary>
        /// Название изделия
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Марка изделия
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// Код изделия строковый
        /// </summary>
        public string DisplayCodeString
        {
            get
            {
                var display = CodeProduct.ToString(CultureInfo.InvariantCulture);
                while (display.Length < 14)
                {
                    display = "0" + display;
                }
                return display;
            }
        }

        protected bool Equals(Product other)
        {
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            return Id == other.Id 
                   && CodeProduct == other.CodeProduct 
                   && string.Equals(Name, other.Name, comparisonIgnoreCase) 
                   && string.Equals(Mark, other.Mark, comparisonIgnoreCase);
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
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ CodeProduct.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ (Mark != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Mark) : 0);
                return hashCode;
            }
        }

        public int CompareTo(Product other)
        {
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            var codeProductComparison = CodeProduct.CompareTo(other.CodeProduct);
            if (codeProductComparison != 0)
            {
                return codeProductComparison;
            }
            var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
            if (nameComparison != 0) return nameComparison;
            return string.Compare(Mark, other.Mark, comparisonIgnoreCase);
        }
    }
}
