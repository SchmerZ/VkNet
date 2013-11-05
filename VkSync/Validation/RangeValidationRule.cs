using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using VkSync.Models;

namespace VkSync.Validation
{
    public class RangeValidationRule : ValidationRule
    {
        private class ValueInfo
        {
            public object Value
            {
                get;
                set;
            }

            public BindingModel Data
            {
                get;
                set;
            }

            public BindingExpression BindingExpression
            {
                get;
                set;
            }
        }

        public int? Min
        {
            get;
            set;
        }

        public int? Max
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            if (owner is BindingExpression)
            {
                var binding = (BindingExpression)owner;

                if (binding.DataItem is BindingModel)
                {
                    var bindingModel = (BindingModel)binding.DataItem;

                    value = new ValueInfo
                        {
                            Value = value,
                            Data = bindingModel,
                            BindingExpression = binding
                        };
                }
            }

            return base.Validate(value, cultureInfo, owner);
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = ValidationResult.ValidResult;

            if (value is ValueInfo)
            {
                var valueInfo = (ValueInfo)value;
                var targetValue = valueInfo.Value;

                var parsed = 0;

                if (string.IsNullOrEmpty(targetValue.ToString()))
                    result = new ValidationResult(false, "Cannot be empty.");

                if (result.IsValid && !int.TryParse(targetValue.ToString(), NumberStyles.Integer, cultureInfo, out parsed))
                    result = new ValidationResult(false, "Invalid characters.");

                if (result.IsValid)
                {
                    if (Min.HasValue && parsed < Min)
                        result = new ValidationResult(false, "Please enter an age in the range: " + Min + " - " + Max + ".");

                    if (Max.HasValue && parsed > Max)
                        result = new ValidationResult(false, "Please enter an age in the range: " + Min + " - " + Max + ".");
                }

                if (!result.IsValid)
                {
                    var property = valueInfo.BindingExpression.ResolvedSourcePropertyName;
                    valueInfo.Data.AddBrokenRuleMessage(property, result.ErrorContent.ToString());
                }
            }

            return result;
        }
    }
}