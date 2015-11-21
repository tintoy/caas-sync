using System.Xml.Serialization;

namespace DD.Caas.SyncAgent.ApiModels
{
	/// <summary>
	///     Information about a user role in DD Compute.
	/// </summary>
	[XmlRoot("Role", Namespace = XmlNamespaces.Directory)]
	public class Role
	{
		/// <summary>
		///     The role name.
		/// </summary>
		[XmlElement("name", Namespace = XmlNamespaces.Directory)]
		public string Name
		{
			get;
			set;
		}
	}
}