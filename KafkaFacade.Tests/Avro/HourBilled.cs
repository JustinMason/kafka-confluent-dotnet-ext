// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.9.0.0
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Confluent.Kafka.Examples.AvroSpecific
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Avro;
	using Avro.Specific;
	
	public partial class HourBilled : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""HourBilled"",""namespace"":""Confluent.Kafka.Examples.AvroSpecific"",""fields"":[{""name"":""name"",""type"":""string""},{""name"":""rate_billed"",""type"":{""type"":""bytes"",""logicalType"":""decimal"",""precision"":4,""scale"":2}}]}");
		private string _name;
		private Avro.AvroDecimal _rate_billed;
		public virtual Schema Schema
		{
			get
			{
				return User._SCHEMA;
			}
		}
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}
		public Avro.AvroDecimal rate_billed
		{
			get
			{
				return this._rate_billed;
			}
			set
			{
				this._rate_billed = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.name;
			case 1: return this.rate_billed;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.name = (System.String)fieldValue; break;
			case 3: this.rate_billed = (Avro.AvroDecimal)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}