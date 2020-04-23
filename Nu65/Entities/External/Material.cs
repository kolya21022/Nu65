using System;
using System.Collections.Generic;
using System.Globalization;

namespace NU65.Entities.External
{
    /// <summary>
    /// Материалы
    /// </summary>
    /// <inheritdoc />
    public class Material : IComparable<Material>
    {
        /// <summary>
        /// Код материала
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заводской код материала
        /// </summary>
        public long CodeMaterial { get; set; }

        /// <summary>
        /// Наименование материала
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Профиль материала
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// ГОСТ
        /// </summary>
        public string Gost { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public Measure Measure { get; set; }

        /// <summary>
        /// Сервисное значение (Warehouse) для переноса из cenmat в prdsetmc.
        /// </summary>
        public decimal ServiceMappedWarehouseId { get; set; }

        /// <summary>
        /// Поле связи ID вложенного объекта (Measure) для связи объектов между собой в сервисном слое.
        /// </summary>
        public long ServiceMappedMeasureId { get; set; }

        //////////////////////////////////////////////////

        public long Nu65TableId { get; set; }

        /// <summary>
        /// Норма расхода вспомогательного продукта в NU65Table
        /// </summary>
        public string AuxiliaryMaterialConsumptionRate { get; set; }

        /// <summary>
        /// Код цеха в NU65Table
        /// </summary>
        public string WorkGuildId { get; set; }

        /// <summary>
        /// Признак материала в NU65Table
        /// </summary>
        public string SignMaterial { get; set; }

        /// <summary>
        /// Код участка в NU65Table
        /// </summary>
        public string ParcelId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string UnitValidation { get; set; }

        /// <summary>
        /// Дата добавления в NU65Table
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Номер извещания в NU65Table
        /// </summary>
        public string FlowRate { get; set; }

        /// <summary>
        /// Код материала строковый
        /// </summary>
        public string DisplayCodeString
        {
            get
            {
                var display = CodeMaterial.ToString(CultureInfo.InvariantCulture);
                while (display.Length < 10)
                {
                    display = "0" + display;
                }
                return display;
            }
        }

        protected bool Equals(Material other)
        {
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            return Id == other.Id 
                   && CodeMaterial == other.CodeMaterial 
                   && string.Equals(Name, other.Name, comparisonIgnoreCase) 
                   && string.Equals(Profile, other.Profile, comparisonIgnoreCase) 
                   && string.Equals(Gost, other.Gost, comparisonIgnoreCase) 
                   && Equals(Measure, other.Measure) 
                   && ServiceMappedMeasureId == other.ServiceMappedMeasureId 
                   && Nu65TableId == other.Nu65TableId 
                   && string.Equals(AuxiliaryMaterialConsumptionRate, other.AuxiliaryMaterialConsumptionRate, comparisonIgnoreCase) 
                   && string.Equals(WorkGuildId, other.WorkGuildId, comparisonIgnoreCase) 
                   && string.Equals(SignMaterial, other.SignMaterial, comparisonIgnoreCase) 
                   && string.Equals(ParcelId, other.ParcelId, comparisonIgnoreCase) 
                   && string.Equals(UnitValidation, other.UnitValidation, comparisonIgnoreCase) 
                   && Date.Equals(other.Date) 
                   && FlowRate == other.FlowRate;
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
            return Equals((Material) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ CodeMaterial.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ (Profile != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Profile) : 0);
                hashCode = (hashCode * 397) ^ (Gost != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Gost) : 0);
                hashCode = (hashCode * 397) ^ (Measure != null ? Measure.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ServiceMappedMeasureId.GetHashCode();
                hashCode = (hashCode * 397) ^ Nu65TableId.GetHashCode();
                hashCode = (hashCode * 397) ^ (AuxiliaryMaterialConsumptionRate != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(AuxiliaryMaterialConsumptionRate) : 0);
                hashCode = (hashCode * 397) ^ (WorkGuildId != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(WorkGuildId) : 0);
                hashCode = (hashCode * 397) ^ (SignMaterial != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(SignMaterial) : 0);
                hashCode = (hashCode * 397) ^ (ParcelId != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ParcelId) : 0);
                hashCode = (hashCode * 397) ^ (UnitValidation != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(UnitValidation) : 0);
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (FlowRate != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(FlowRate) : 0);
                return hashCode;
            }
        }

        public int CompareTo(Material other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var codeMaterialComparison = CodeMaterial.CompareTo(other.CodeMaterial);
            if (codeMaterialComparison != 0)
            {
                return codeMaterialComparison;
            }

            var measureComparison = Comparer<Measure>.Default.Compare(Measure, other.Measure);
            if (measureComparison != 0)
            {
                return measureComparison;
            }

            return Id.CompareTo(other.Id);
        }
    }
}
