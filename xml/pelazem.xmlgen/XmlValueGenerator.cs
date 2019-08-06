using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using System.Reflection;
using pelazem.rndgen;
using pelazem.util;

namespace pelazem.xmlgen
{
	internal abstract class XmlValueGenerator
	{
		internal static List<string> _ids = new List<string>();

		internal static Generator_ID _g_ID = new Generator_ID();
		internal static Generator_IDREF _g_IDREF = new Generator_IDREF();
		internal static bool _IDRef = false;

		protected ArrayList _allowedValues = null;

		string _prefix = null;
		XmlSchemaDatatype _datatype;

		internal static XmlValueGenerator _anyGenerator = new Generator_anyType();
		internal static XmlValueGenerator _anySimpleTypeGenerator = new Generator_anySimpleType();

		internal static List<string> IDList
		{
			get
			{
				return _ids;
			}
		}

		protected internal virtual string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = value;
			}
		}

		protected internal virtual void AddGenerator(XmlValueGenerator genr)
		{
			return;
		}

		protected internal abstract string GenerateValue();

		internal XmlSchemaDatatype Datatype
		{
			get
			{
				return _datatype;
			}
		}

		internal static XmlValueGenerator CreateGenerator(XmlSchemaDatatype datatype, int listLength)
		{
			XmlTypeCode typeCode = datatype.TypeCode;

			object restriction = datatype.GetType().InvokeMember("Restriction", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, datatype, null);
			CompiledFacets rFacets = new CompiledFacets(restriction);
			XmlValueGenerator generator;

			if (datatype.Variety == XmlSchemaDatatypeVariety.Union)
			{
				generator = CreateUnionGenerator(datatype, rFacets, listLength);
			}
			else if (datatype.Variety == XmlSchemaDatatypeVariety.List)
			{
				generator = CreateListGenerator(datatype, rFacets, listLength);
			}
			else
			{
				switch (typeCode)
				{
					case XmlTypeCode.None:
						generator = _anyGenerator;
						break;
					case XmlTypeCode.Item:
						generator = _anyGenerator;
						break;
					case XmlTypeCode.AnyAtomicType:
						generator = _anySimpleTypeGenerator;
						break;
					case XmlTypeCode.String:
						generator = new Generator_string(rFacets);
						break;
					case XmlTypeCode.Boolean:
						generator = new Generator_boolean();
						break;
					case XmlTypeCode.Float:
						generator = new Generator_float(rFacets);
						break;
					case XmlTypeCode.Double:
						generator = new Generator_double(rFacets);
						break;
					case XmlTypeCode.AnyUri:
						generator = new Generator_anyURI(rFacets);
						break;
					case XmlTypeCode.Integer:
						generator = new Generator_integer(rFacets);
						break;
					case XmlTypeCode.Decimal:
						generator = new Generator_decimal(rFacets);
						break;
					case XmlTypeCode.NonPositiveInteger:
						generator = new Generator_nonPositiveInteger(rFacets);
						break;
					case XmlTypeCode.NegativeInteger:
						generator = new Generator_negativeInteger(rFacets);
						break;
					case XmlTypeCode.Long:
						generator = new Generator_long(rFacets);
						break;
					case XmlTypeCode.Int:
						generator = new Generator_integer(rFacets);
						break;
					case XmlTypeCode.Short:
						generator = new Generator_short(rFacets);
						break;
					case XmlTypeCode.Byte:
						generator = new Generator_byte(rFacets);
						break;
					case XmlTypeCode.NonNegativeInteger:
						generator = new Generator_nonNegativeInteger(rFacets);
						break;
					case XmlTypeCode.UnsignedLong:
						generator = new Generator_unsignedLong(rFacets);
						break;
					case XmlTypeCode.UnsignedInt:
						generator = new Generator_unsignedInt(rFacets);
						break;
					case XmlTypeCode.UnsignedShort:
						generator = new Generator_unsignedShort(rFacets);
						break;
					case XmlTypeCode.UnsignedByte:
						generator = new Generator_unsignedByte(rFacets);
						break;
					case XmlTypeCode.PositiveInteger:
						generator = new Generator_positiveInteger(rFacets);
						break;
					case XmlTypeCode.Duration:
						generator = new Generator_duration(rFacets);
						break;
					case XmlTypeCode.DateTime:
						generator = new Generator_dateTime(rFacets);
						break;
					case XmlTypeCode.Date:
						generator = new Generator_date(rFacets);
						break;
					case XmlTypeCode.GYearMonth:
						generator = new Generator_gYearMonth(rFacets);
						break;
					case XmlTypeCode.GYear:
						generator = new Generator_gYear(rFacets);
						break;
					case XmlTypeCode.GMonthDay:
						generator = new Generator_gMonthDay(rFacets);
						break;
					case XmlTypeCode.GDay:
						generator = new Generator_gDay(rFacets);
						break;
					case XmlTypeCode.GMonth:
						generator = new Generator_gMonth(rFacets);
						break;
					case XmlTypeCode.Time:
						generator = new Generator_time(rFacets);
						break;
					case XmlTypeCode.HexBinary:
						generator = new Generator_hexBinary(rFacets);
						break;
					case XmlTypeCode.Base64Binary:
						generator = new Generator_base64Binary(rFacets);
						break;
					case XmlTypeCode.QName:
						generator = new Generator_QName(rFacets);
						break;
					case XmlTypeCode.Notation:
						generator = new Generator_Notation(rFacets);
						break;
					case XmlTypeCode.NormalizedString:
						generator = new Generator_string(rFacets);
						break;
					case XmlTypeCode.Token:
					case XmlTypeCode.NmToken:
						generator = new Generator_token(rFacets);
						break;
					case XmlTypeCode.Language:
						generator = new Generator_language(rFacets);
						break;
					case XmlTypeCode.Name:
						generator = new Generator_Name(rFacets);
						break;
					case XmlTypeCode.NCName:
						generator = new Generator_NCName(rFacets);
						break;
					case XmlTypeCode.Id:
						_g_ID.CheckFacets(rFacets);
						generator = _g_ID;
						break;
					case XmlTypeCode.Idref:
						_g_IDREF.CheckFacets(rFacets);
						generator = _g_IDREF;
						break;
					default:
						generator = _anyGenerator;
						break;
				}
			}

			generator.SetDatatype(datatype);

			return generator;
		}

		private static XmlValueGenerator CreateUnionGenerator(XmlSchemaDatatype dtype, CompiledFacets facets, int listLength)
		{
			XmlSchemaSimpleType[] memberTypes = (XmlSchemaSimpleType[])dtype.GetType().InvokeMember("types", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, dtype, null);
			Generator_Union union_genr = new Generator_Union(facets);

			foreach (XmlSchemaSimpleType st1 in memberTypes)
			{
				union_genr.AddGenerator(XmlValueGenerator.CreateGenerator(st1.Datatype, listLength));
			}

			return union_genr;
		}

		private static XmlValueGenerator CreateListGenerator(XmlSchemaDatatype dtype, CompiledFacets facets, int listLength)
		{
			XmlSchemaDatatype itemType = (XmlSchemaDatatype)dtype.GetType().InvokeMember("itemType", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, dtype, null);
			Generator_List list_genr = new Generator_List(facets);
			list_genr.ListLength = listLength;
			list_genr.AddGenerator(XmlValueGenerator.CreateGenerator(itemType, listLength));
			return list_genr;
		}

		private void SetDatatype(XmlSchemaDatatype datatype)
		{
			this._datatype = datatype;
		}

		protected internal object GetEnumerationValue()
		{
			return RandomGenerator.Categorical.GetCategorical(_allowedValues);
		}
	}


	internal class Generator_anyType : XmlValueGenerator
	{
		protected internal override string GenerateValue()
		{
			return RandomGenerator.String.GetString();
		}
	}

	internal class Generator_anySimpleType : XmlValueGenerator
	{
		protected internal override string GenerateValue()
		{
			return RandomGenerator.String.GetString();
		}
	}


	internal class Generator_facetBase : XmlValueGenerator
	{
		protected int _length = -1;
		protected int _minLength = -1;
		protected int _maxLength = -1;

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.Length) != 0)
					_length = genFacets.Length;

				if ((flags & RestrictionFlags.MinLength) != 0)
					_minLength = genFacets.MinLength;

				if ((flags & RestrictionFlags.MaxLength) != 0)
					_maxLength = genFacets.MaxLength;

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;
			}
		}

		protected internal override string GenerateValue()
		{
			return string.Empty;
		}
	}

	internal class Generator_string : Generator_facetBase
	{
		internal Generator_string()
		{
		}

		internal Generator_string(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			if (_length > 0)
				return RandomGenerator.String.GetString(_length);

			if (_minLength >= 0 || _maxLength > 0)
				return RandomGenerator.String.GetString(_minLength, _maxLength);

			return RandomGenerator.String.GetString();
		}
	}


	internal class Generator_decimal : XmlValueGenerator
	{
		private decimal _minBound;
		private decimal _maxBound;

		protected internal decimal MinBound
		{
			get { return _minBound; }
			set
			{
				_minBound = value;
			}
		}

		protected internal decimal MaxBound
		{
			get { return _maxBound; }
			set
			{
				_maxBound = value;
			}
		}

		internal Generator_decimal()
		{
			this.MinBound = decimal.MinValue;
			this.MaxBound = decimal.MaxValue;
		}

		internal Generator_decimal(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.MaxInclusive) != 0)
					this.MaxBound = (decimal)Convert.ChangeType(genFacets.MaxInclusive, typeof(decimal));

				if ((flags & RestrictionFlags.MaxExclusive) != 0)
					this.MaxBound = (decimal)Convert.ChangeType(genFacets.MaxExclusive, typeof(decimal)) - 1;

				if ((flags & RestrictionFlags.MinInclusive) != 0)
					this.MinBound = (decimal)Convert.ChangeType(genFacets.MinInclusive, typeof(decimal));

				if ((flags & RestrictionFlags.MinExclusive) != 0)
					this.MinBound = (decimal)Convert.ChangeType(genFacets.MinExclusive, typeof(decimal)) + 1;

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;

				if ((flags & RestrictionFlags.TotalDigits) != 0)
				{
					decimal totalDigitsValue = (decimal)Math.Pow(10, genFacets.TotalDigits) - 1;

					if (totalDigitsValue <= this.MaxBound)
					{ //Use the lower of totalDigits value and maxInc/maxEx
						this.MaxBound = totalDigitsValue;
						this.MinBound = 0 - this.MaxBound;
					}
				}

				if ((flags & RestrictionFlags.FractionDigits) != 0 && genFacets.FractionDigits > 0)
				{
					if ((flags & RestrictionFlags.TotalDigits) != 0)
					{
						//if (T,F) is (6,3) the max value is not 999.999 but 99999.9d but we are going with the smaller range on the integral part to generate more varied fractional part.
						int range = genFacets.TotalDigits - genFacets.FractionDigits;
						double integralPart = Math.Pow(10, range) - 1;
						double fractionPart = integralPart / Math.Pow(10, range);
						this.MaxBound = (decimal)(integralPart + fractionPart);
						this.MinBound = 0 - this.MaxBound;
					}
				}
			}
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			double result = RandomGenerator.Numeric.GetUniform(Converter.GetDouble(this.MinBound), Converter.GetDouble(this.MaxBound));

			return result.ToString(null, NumberFormatInfo.InvariantInfo);
		}
	}

	internal class Generator_double : XmlValueGenerator
	{
		private double _minBound;
		private double _maxBound;

		protected internal double MinBound
		{
			get { return _minBound; }
			set
			{
				_minBound = value;
			}
		}

		protected internal double MaxBound
		{
			get { return _maxBound; }
			set
			{
				_maxBound = value;
			}
		}

		internal Generator_double()
		{
			this.MinBound = double.MinValue;
			this.MaxBound = double.MaxValue;
		}

		internal Generator_double(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.MaxInclusive) != 0)
					this.MaxBound = (double)genFacets.MaxInclusive;

				if ((flags & RestrictionFlags.MaxExclusive) != 0)
					this.MaxBound = (double)genFacets.MaxExclusive - 1;

				if ((flags & RestrictionFlags.MinInclusive) != 0)
					this.MinBound = (double)genFacets.MinInclusive;

				if ((flags & RestrictionFlags.MinExclusive) != 0)
					this.MinBound = (double)genFacets.MinExclusive + 1;

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;
			}
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			double result = RandomGenerator.Numeric.GetUniform(Converter.GetDouble(this.MinBound), Converter.GetDouble(this.MaxBound));

			return XmlConvert.ToString(result);
		}
	}

	internal class Generator_float : XmlValueGenerator
	{
		private float _minBound;
		private float _maxBound;

		protected internal float MinBound
		{
			get { return _minBound; }
			set
			{
				_minBound = value;
			}
		}

		protected internal float MaxBound
		{
			get { return _maxBound; }
			set
			{
				_maxBound = value;
			}
		}

		internal Generator_float()
		{
			this.MinBound = float.MinValue;
			this.MaxBound = float.MaxValue;
		}

		internal Generator_float(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.MaxInclusive) != 0)
					this.MaxBound = (float)genFacets.MaxInclusive;

				if ((flags & RestrictionFlags.MaxExclusive) != 0)
					this.MaxBound = (float)genFacets.MaxExclusive - 1;

				if ((flags & RestrictionFlags.MinInclusive) != 0)
					this.MinBound = (float)genFacets.MinInclusive;

				if ((flags & RestrictionFlags.MinExclusive) != 0)
					this.MinBound = (float)genFacets.MinExclusive + 1;

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;
			}
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			double result = RandomGenerator.Numeric.GetUniform(Converter.GetDouble(this.MinBound), Converter.GetDouble(this.MaxBound));

			return XmlConvert.ToString(result);
		}
	}


	internal class Generator_integer : Generator_decimal
	{
		internal Generator_integer()
		{
			this.MinBound = int.MinValue;
			this.MaxBound = int.MaxValue;
		}

		internal Generator_integer(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_nonPositiveInteger : Generator_decimal
	{
		internal Generator_nonPositiveInteger()
		{
			this.MinBound = int.MinValue;
			this.MaxBound = 0;
		}

		internal Generator_nonPositiveInteger(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_negativeInteger : Generator_decimal
	{
		internal Generator_negativeInteger()
		{
			this.MinBound = int.MinValue;
			this.MaxBound = -1;
		}

		internal Generator_negativeInteger(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_long : Generator_decimal
	{
		internal Generator_long()
		{
			this.MinBound = Int64.MinValue;
			this.MaxBound = Int64.MaxValue;
		}

		internal Generator_long(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_short : Generator_decimal
	{
		internal Generator_short()
		{
			this.MinBound = short.MinValue;
			this.MaxBound = short.MaxValue;
		}

		internal Generator_short(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

	}

	internal class Generator_byte : Generator_decimal
	{
		internal Generator_byte()
		{
			this.MinBound = byte.MinValue;
			this.MaxBound = byte.MaxValue;
		}

		internal Generator_byte(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_nonNegativeInteger : Generator_decimal
	{
		internal Generator_nonNegativeInteger()
		{
			this.MinBound = 0;
			this.MaxBound = int.MaxValue;
		}

		internal Generator_nonNegativeInteger(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_unsignedLong : Generator_decimal
	{
		internal Generator_unsignedLong()
		{
			this.MinBound = ulong.MinValue;
			this.MaxBound = ulong.MaxValue;
		}

		internal Generator_unsignedLong(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_unsignedInt : Generator_decimal
	{
		internal Generator_unsignedInt()
		{
			this.MinBound = uint.MinValue;
			this.MaxBound = uint.MaxValue;
		}

		internal Generator_unsignedInt(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_unsignedShort : Generator_decimal
	{
		internal Generator_unsignedShort()
		{
			this.MinBound = ushort.MinValue;
			this.MaxBound = ushort.MaxValue;
		}

		internal Generator_unsignedShort(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_unsignedByte : Generator_decimal
	{
		internal Generator_unsignedByte()
		{
			this.MinBound = Byte.MinValue;
			this.MaxBound = Byte.MaxValue;
		}

		internal Generator_unsignedByte(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_positiveInteger : Generator_decimal
	{
		internal Generator_positiveInteger()
		{
			this.MinBound = 1;
			this.MaxBound = int.MaxValue;
		}

		internal Generator_positiveInteger(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}


	internal class Generator_boolean : XmlValueGenerator
	{
		protected internal override string GenerateValue()
		{
			return RandomGenerator.Boolean.GetBoolean().ToString();
		}
	}


	internal class Generator_duration : XmlValueGenerator
	{
		private TimeSpan _minBound;
		private TimeSpan _maxBound;

		TimeSpan _endValue = new TimeSpan(0, 0, 1);

		protected internal TimeSpan MinBound
		{
			get { return _minBound; }
			set { _minBound = value; }
		}

		protected internal TimeSpan MaxBound
		{
			get { return _maxBound; }
			set { _maxBound = value; }
		}

		internal Generator_duration()
		{
			this.MinBound = new TimeSpan(0);
			this.MaxBound = new TimeSpan(30, 0, 0, 0, 0);
		}

		internal Generator_duration(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.MaxInclusive) != 0)
					this.MaxBound = (TimeSpan)genFacets.MaxInclusive;

				if ((flags & RestrictionFlags.MaxExclusive) != 0)
					this.MaxBound = ((TimeSpan)genFacets.MaxExclusive).Subtract(_endValue);

				if ((flags & RestrictionFlags.MinInclusive) != 0)
					this.MinBound = (TimeSpan)genFacets.MinInclusive;

				if ((flags & RestrictionFlags.MinExclusive) != 0)
					this.MinBound = ((TimeSpan)genFacets.MinExclusive).Add(_endValue);

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;
			}
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			TimeSpan result = RandomGenerator.DateTime.GetTimeSpan(this.MinBound, this.MaxBound);

			return XmlConvert.ToString(result);
		}
	}

	internal class Generator_dateTime : XmlValueGenerator
	{
		private DateTime _minBound;
		private DateTime _maxBound;
		private TimeSpan _step = new TimeSpan(0, 1, 0);

		protected internal DateTime MinBound
		{
			get { return _minBound; }
			set { _minBound = value; }
		}

		protected internal DateTime MaxBound
		{
			get { return _maxBound; }
			set { _maxBound = value; }
		}

		internal Generator_dateTime()
		{
			this.MinBound = DateTime.UtcNow.AddDays(-30);
			this.MaxBound = DateTime.UtcNow.AddDays(30);
		}

		internal Generator_dateTime(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal void CheckFacets(CompiledFacets genFacets)
		{
			if (genFacets != null)
			{
				RestrictionFlags flags = genFacets.Flags;

				if ((flags & RestrictionFlags.MaxInclusive) != 0)
					this.MaxBound = (DateTime)genFacets.MaxInclusive;

				if ((flags & RestrictionFlags.MaxExclusive) != 0)
					this.MaxBound = ((DateTime)genFacets.MaxExclusive).Subtract(_step);

				if ((flags & RestrictionFlags.MinInclusive) != 0)
					this.MinBound = (DateTime)genFacets.MinInclusive;

				if ((flags & RestrictionFlags.MinExclusive) != 0)
					this.MinBound = ((DateTime)genFacets.MinExclusive).Add(_step);

				if ((flags & RestrictionFlags.Enumeration) != 0)
					_allowedValues = genFacets.Enumeration;
			}
		}

		protected internal override string GenerateValue()
		{
			return XmlConvert.ToString(GenerateWorker(), XmlDateTimeSerializationMode.Utc);
		}

		protected virtual DateTime GenerateWorker()
		{
			DateTime result;

			if (_allowedValues != null && _allowedValues.Count > 0)
				result = (DateTime)RandomGenerator.Categorical.GetCategorical(_allowedValues);
			else
				result = RandomGenerator.DateTime.GetDateTime(this.MinBound, this.MaxBound);

			return result;
		}
	}

	internal class Generator_date : Generator_dateTime
	{
		internal Generator_date() : base() { }

		internal Generator_date(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "yyyy-MM-dd");
		}
	}

	internal class Generator_gYearMonth : Generator_dateTime
	{
		internal Generator_gYearMonth() : base() { }

		internal Generator_gYearMonth(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "yyyy-MM");
		}
	}

	internal class Generator_gYear : Generator_dateTime
	{
		internal Generator_gYear() : base() { }

		internal Generator_gYear(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "yyyy");
		}
	}

	internal class Generator_gMonthDay : Generator_dateTime
	{
		internal Generator_gMonthDay() : base() { }

		internal Generator_gMonthDay(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "--MM-dd");
		}
	}

	internal class Generator_gDay : Generator_dateTime
	{
		internal Generator_gDay() : base() { }

		internal Generator_gDay(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "---dd");
		}
	}

	internal class Generator_gMonth : Generator_dateTime
	{
		internal Generator_gMonth() : base() { }

		internal Generator_gMonth(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result.Date, "--MM--");
		}
	}

	internal class Generator_time : Generator_dateTime
	{
		internal Generator_time() : base() { }

		internal Generator_time(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			DateTime result = GenerateWorker();

			return XmlConvert.ToString(result, "HH:mm:ss");
		}
	}



	internal class Generator_hexBinary : Generator_facetBase
	{
		private Generator_integer _binGen = new Generator_integer();

		internal Generator_hexBinary()
		{
			_binGen.MinBound = 4023;
		}

		internal Generator_hexBinary(CompiledFacets rFacets) : this()
		{
			base.CheckFacets(rFacets);
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();

			int binNo = Converter.GetInt32(_binGen.GenerateValue());

			StringBuilder str = new StringBuilder(binNo.ToString("X4"));

			if (_length == -1 && _minLength == -1 && _maxLength == -1)
			{ // no op
			}
			else if (_length != -1)
				ProcessLengthFacet(ref str);
			else
				ProcessMinMaxLengthFacet(ref str);

			return str.ToString();
		}

		private void ProcessLengthFacet(ref StringBuilder str)
		{
			int pLength = str.Length;

			if (pLength % 2 != 0)
				throw new Exception("Total length of binary data should be even");

			int correctLen = pLength / 2;

			if (correctLen > _length)
			{ //Need to remove (correctLen - length) * 2 chars 
				str.Remove(_length, (correctLen - _length) * 2);
			}
			else if (correctLen < _length)
			{ //Need to add (length - correctLen) * 2 chars
				int addCount = _length - correctLen;

				for (int i = 0; i < addCount; i++)
				{
					str.Append("0A");
				}
			}
		}

		private void ProcessMinMaxLengthFacet(ref StringBuilder str)
		{
			int pLength = str.Length;

			if (pLength % 2 != 0)
				throw new Exception("Total length of binary data should be even");

			int correctLen = pLength / 2;

			if (_minLength != -1)
			{
				if (correctLen < _minLength)
				{
					int addCount = _minLength - correctLen;
					for (int i = 0; i < addCount; i++)
					{
						str.Append("0A");
					}
				}
			}
			else
			{ //if maxLength != -1
				if (correctLen > _maxLength)
				{ //Need to remove (correctLen - maxlength) * 2 chars 
					str.Remove(_maxLength, (correctLen - _maxLength) * 2);
				}
			}
		}
	}

	internal class Generator_base64Binary : Generator_facetBase
	{
		internal Generator_base64Binary() { }

		internal Generator_base64Binary(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		protected internal override string GenerateValue()
		{
			string result;

			if (_allowedValues != null && _allowedValues.Count > 0)
				result = RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();
			else
				result = RandomGenerator.String.GetString();

			byte[] bytes = Encoding.UTF8.GetBytes(result);

			return Convert.ToBase64String(bytes);
		}
	}


	internal class Generator_QName : Generator_string
	{
		internal Generator_QName()
		{
			this.Prefix = "qname";
		}

		internal Generator_QName(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		protected internal override string GenerateValue()
		{
			string result = base.GenerateValue();

			if (result.Length == 1)
			{ //If it is a qname of length 1, then return a char to be sure
				return new string(this.Prefix[0], 1);
			}

			return result;
		}
	}

	internal class Generator_Notation : Generator_QName
	{
		internal Generator_Notation() { }

		internal Generator_Notation(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_token : Generator_string
	{
		internal Generator_token()
		{
			this.Prefix = "Token";
		}

		internal Generator_token(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_language : Generator_string
	{ //A derived type of token
		static List<string> languageList = new List<string>() { "en", "fr", "de", "da", "el", "it", "en-US" };

		internal Generator_language() { }

		internal Generator_language(CompiledFacets rFacets) : base(rFacets)
		{
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return RandomGenerator.Categorical.GetCategorical(_allowedValues).ToString();
			else
				return RandomGenerator.Categorical.GetCategorical<string>(languageList).ToString();
		}
	}

	internal class Generator_Name : Generator_string
	{
		internal Generator_Name()
		{
			this.Prefix = "Name";
		}

		internal Generator_Name(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}

	internal class Generator_NCName : Generator_string
	{
		internal Generator_NCName()
		{
			this.Prefix = "NcName";
		}

		internal Generator_NCName(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}


	internal class Generator_ID : Generator_string
	{
		internal Generator_ID()
		{
			this.Prefix = "ID";
		}

		protected internal override string GenerateValue()
		{
			if (_IDRef)
			{
				_IDRef = false;

				return IDList[IDList.Count - 1];
			}
			else
			{
				string id = base.GenerateValue();

				IDList.Add(id); // Add so that you can retrieve it from there for IDREF

				return id;
			}
		}
	}

	internal class Generator_IDREF : Generator_string
	{
		int refCnt = 0;

		internal Generator_IDREF()
		{
		}

		protected internal override string GenerateValue()
		{
			if (IDList.Count == 0)
			{
				string id = _g_ID.GenerateValue();

				_IDRef = true;
			}

			if (refCnt >= IDList.Count)
			{
				refCnt = refCnt - IDList.Count;
			}

			return IDList[refCnt++];
		}
	}


	internal class Generator_anyURI : Generator_string
	{
		internal Generator_anyURI()
		{
			this.Prefix = "http://tempuri";
		}

		internal Generator_anyURI(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}
	}


	internal class Generator_Union : Generator_facetBase
	{
		ArrayList unionGens = new ArrayList();

		internal Generator_Union()
		{
		}

		internal Generator_Union(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		protected internal override void AddGenerator(XmlValueGenerator genr)
		{
			unionGens.Add(genr);
		}

		internal ArrayList Generators
		{
			get
			{
				return unionGens;
			}
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
			{
				object result = RandomGenerator.Categorical.GetCategorical(_allowedValues);

				//Unpack the union enumeration value into memberType and typedValue
				object typedValue = CompiledFacets.XsdSimpleValueType.InvokeMember("typedValue", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null, result, null);

				return (string)this.Datatype.ChangeType(typedValue, TypeUtil.TypeString);
			}
			else if (unionGens.Count > 0)
			{
				int rnd = Convert.ToInt32(RandomGenerator.Numeric.GetUniform(0, unionGens.Count));

				XmlValueGenerator genr = (XmlValueGenerator)(unionGens[rnd]);

				genr.Prefix = this.Prefix;

				return genr.GenerateValue();
			}

			return string.Empty;
		}
	}

	internal class Generator_List : Generator_facetBase
	{
		private XmlValueGenerator _genr;

		private int _listLength;

		private StringBuilder _resultBuilder;

		internal StringBuilder ResultBuilder
		{
			get
			{
				if (_resultBuilder == null)
					_resultBuilder = new StringBuilder();

				return _resultBuilder;
			}
		}

		internal Generator_List()
		{
		}

		internal Generator_List(CompiledFacets rFacets) : this()
		{
			CheckFacets(rFacets);
		}

		internal int ListLength
		{
			get
			{
				return _listLength;
			}
			set
			{
				_listLength = value;
			}
		}

		protected internal override void AddGenerator(XmlValueGenerator valueGenr)
		{
			_genr = valueGenr;
		}

		protected internal override string GenerateValue()
		{
			if (_allowedValues != null && _allowedValues.Count > 0)
				return (string)this.Datatype.ChangeType(RandomGenerator.Categorical.GetCategorical(_allowedValues), TypeUtil.TypeString);
			else
			{
				this.ResultBuilder.Clear();

				_genr.Prefix = this.Prefix;

				int numOfItems = this.ListLength;

				if (_length != -1)
					numOfItems = _length;
				else if (_minLength != -1)
					numOfItems = _minLength;
				else if (_maxLength != -1)
					numOfItems = _maxLength;

				for (int i = 0; i < numOfItems; i++)
				{
					_resultBuilder.Append(_genr.GenerateValue());
					_resultBuilder.Append(" ");
				}
			}

			return _resultBuilder.ToString();
		}
	}
}
