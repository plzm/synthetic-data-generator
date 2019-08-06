using System.Xml;
using System.Collections;

namespace pelazem.xmlgen
{
	internal class SubstitutionGroupWrapper
	{
		XmlQualifiedName head;
		ArrayList members = new ArrayList();

		internal XmlQualifiedName Head
		{
			get
			{
				return head;
			}
			set
			{
				head = value;
			}
		}

		internal ArrayList Members
		{
			get
			{
				return members;
			}
		}
	}
}
