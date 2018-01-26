﻿namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinPropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMinProperties(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMinProperties(typed).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var minProperties = GetMinProperties((T) schema);
			return json.Object.Count < minProperties
				       ? new SchemaValidationResults(string.Empty, $"Expected: >= {minProperties} items; Actual: {json.Object.Count} items.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class MinPropertiesSchema04PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMinProperties(JsonSchema04 schema)
		{
			return schema.MinProperties;
		}
	}
	
	internal class MinPropertiesSchema06PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMinProperties(JsonSchema06 schema)
		{
			return schema.MinProperties;
		}
	}
	
	internal class MinPropertiesSchema07PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMinProperties(JsonSchema07 schema)
		{
			return schema.MinProperties;
		}
	}
}
