using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DD.Caas.SyncAgent.ApiModels
{	
	/// <summary>
	///     Information about a user account in DD Compute.
	/// </summary>
	[XmlRoot("Account", Namespace = XmlNamespaces.Directory)]
	public class Account
	{
		/// <summary>
		///     The user name (login).
		/// </summary>
		[XmlElement("userName", Namespace = XmlNamespaces.Directory)]
		public string UserName
		{
			get;
			set;
		}

		/// <summary>
		///     The user's full name.
		/// </summary>
		[XmlElement("fullName", Namespace = XmlNamespaces.Directory)]
		public string FullName
		{
			get;
			set;
		}

		/// <summary>
		///     The user's first name.
		/// </summary>
		[XmlElement("firstName", Namespace = XmlNamespaces.Directory)]
		public string FirstName
		{
			get;
			set;
		}

		/// <summary>
		///     The user's last name.
		/// </summary>
		[XmlElement("lastName", Namespace = XmlNamespaces.Directory)]
		public string LastName
		{
			get;
			set;
		}

		/// <summary>
		///     The user's email address.
		/// </summary>
		[XmlElement("emailAddress", Namespace = XmlNamespaces.Directory)]
		public string EmailAddress
		{
			get;
			set;
		}

		/// <summary>
		///     The Id of the organisation to which the user belongs.
		/// </summary>
		[XmlElement("orgId", Namespace = XmlNamespaces.Directory)]
		public Guid OrganizationId
		{
			get;
			set;
		}

		/// <summary>
		///     The roles assigned to the user.
		/// </summary>
		[XmlArray("roles", Namespace = XmlNamespaces.Directory)]
		[XmlArrayItem("role", Namespace = XmlNamespaces.Directory)]
		public List<Role> Roles
		{
			get;
			set;
		} = new List<Role>();
	}
}