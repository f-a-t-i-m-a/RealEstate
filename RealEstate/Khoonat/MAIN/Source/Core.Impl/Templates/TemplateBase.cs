using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security.AntiXss;

namespace JahanJooy.RealEstate.Core.Impl.Templates
{
	public abstract class TemplateBase
	{
		[Browsable(false)]
		public StringBuilder Buffer { get; set; }

		[Browsable(false)]
		public StringWriter Writer { get; set; }

		protected TemplateBase()
		{
			Buffer = new StringBuilder();
			Writer = new StringWriter(Buffer);
		}

		public virtual void SetModel(object model)
		{
			// Do nothing, overrides should cast and keep the model.
		}

		public abstract void Execute();

		// Writes the results of expressions like: "@foo.Bar"
		public virtual void Write(object value)
		{
		    if (value == null)
		        return;

			if (value is IHtmlString)
				WriteLiteral(value);
			else
				WriteLiteral(AntiXssEncoder.HtmlEncode(value.ToString(), false));
		}

		// Writes literals like markup: "<p>Foo</p>"
		public virtual void WriteLiteral(object value)
		{
			Buffer.Append(value);
		}

		// This method is used to write out attribute values using
		// some funky nested tuple storage.
		// Handles situations like href="@Model.Entry.Id"
		public virtual void WriteAttribute(string name, Tuple<string, int> startTag, Tuple<string, int> endTag, params object[] values)
		{
			var sb = new StringBuilder();

			sb.Append(startTag.Item1);

			var types = new[] { typeof(object), typeof(string), typeof(decimal), typeof(bool), typeof(char), typeof(byte), typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(ushort), typeof(uint), typeof(ulong), typeof(float), typeof(double) };

			// All values must be of type:
			// Tuple<Tuple<string, int>, Tuple<______, int>, bool>
			//       ----- TupleA -----  ----- TupleB -----  bool

			Type genTuple = typeof(Tuple<,>);
			Type genTriple = typeof(Tuple<,,>);

			Type tupleA = genTuple.MakeGenericType(typeof(string), typeof(int));

			foreach (var value in values)
			{
				// Find the type of this value
				foreach (Type type in types)
				{
					Type tupleB = genTuple.MakeGenericType(type, typeof(int));
					Type nonGen = genTriple.MakeGenericType(tupleA, tupleB, typeof(bool));

					// Check if value is this type
					if (!nonGen.IsInstanceOfType(value))
						continue;

					// Found
					// Convert it to this
					dynamic typedObject = Convert.ChangeType(value, nonGen);

					if (typedObject == null)
						continue;

					sb.Append(WriteAttribute(typedObject));
					break;
				}
			}

			sb.Append(endTag.Item1);

			Buffer.Append(sb);
		}

		private static string WriteAttribute<TP>(Tuple<Tuple<string, int>, Tuple<TP, int>, bool> value)
		{
			if (value == null)
				return string.Empty;

			var sb = new StringBuilder();

			sb.Append(value.Item1.Item1);
			sb.Append(value.Item2.Item1);

			return sb.ToString();
		}
	}
}