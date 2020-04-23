using System;
using System.Globalization;

namespace NU65.Entities.External
{
	/// <summary>
	/// Единица измерения
	/// </summary>
	/// <inheritdoc />
	[Serializable]
	public class Measure : IComparable<Measure>
	{
        /// <summary>
        /// Код единицы измерения
        /// </summary>
		public long Id { get; set; }

        /// <summary>
        /// Код в dbd единицы измерения (в dbf строка)
        /// </summary>
        public int OldDbCode { get; set; }

        /// <summary>
        /// Полное наименование единицы измерения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Красткое наименование единицы измерения
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Код в dbd единицы измерения строковый
        /// </summary>
        public string DisplayOldDbCodeString
        {
	        get
	        {
	            var display = OldDbCode.ToString(CultureInfo.InvariantCulture);
	            while (display.Length < 3)
	            {
	                display = "0" + display;
	            }
	            return display;
	        }
	    }

        protected bool Equals(Measure other)
	    {
	        const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            return Id == other.Id 
	               && OldDbCode == other.OldDbCode 
	               && string.Equals(Name, other.Name, comparisonIgnoreCase) 
	               && string.Equals(ShortName, other.ShortName, comparisonIgnoreCase);
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
	        return Equals((Measure) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = Id.GetHashCode();
	            hashCode = (hashCode * 397) ^ OldDbCode.GetHashCode();
	            hashCode = (hashCode * 397) ^ (Name != null 
	                           ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
	            hashCode = (hashCode * 397) ^ (ShortName != null 
	                           ? StringComparer.OrdinalIgnoreCase.GetHashCode(ShortName) : 0);
	            return hashCode;
	        }
	    }

	    public int CompareTo(Measure other)
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
	        var oldDbCodeComparison = OldDbCode.CompareTo(other.OldDbCode);
	        if (oldDbCodeComparison != 0)
	        {
	            return oldDbCodeComparison;
	        }
            var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
	        if (nameComparison != 0)
	        {
	            return nameComparison;
	        }
	        var shortNameComparison = string.Compare(ShortName, other.ShortName, comparisonIgnoreCase);
	        if (shortNameComparison != 0)
	        {
	            return shortNameComparison;
	        }  
	        return Id.CompareTo(other.Id);
	    }
	}
}
