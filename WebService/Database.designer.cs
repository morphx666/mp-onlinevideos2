﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:2.0.50727.4927
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineVideos.WebService.Database
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	public partial class OnlineVideosDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertSite(Site instance);
    partial void UpdateSite(Site instance);
    partial void DeleteSite(Site instance);
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    partial void InsertReport(Report instance);
    partial void UpdateReport(Report instance);
    partial void DeleteReport(Report instance);
    partial void InsertDll(Dll instance);
    partial void UpdateDll(Dll instance);
    partial void DeleteDll(Dll instance);
    #endregion
		
		public OnlineVideosDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["OnlineVideosDB"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public OnlineVideosDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OnlineVideosDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OnlineVideosDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public OnlineVideosDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Site> Site
		{
			get
			{
				return this.GetTable<Site>();
			}
		}
		
		public System.Data.Linq.Table<User> User
		{
			get
			{
				return this.GetTable<User>();
			}
		}
		
		public System.Data.Linq.Table<Report> Report
		{
			get
			{
				return this.GetTable<Report>();
			}
		}
		
		public System.Data.Linq.Table<Dll> Dll
		{
			get
			{
				return this.GetTable<Dll>();
			}
		}
	}
	
	[Table(Name="")]
	public partial class Site : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Data.Linq.Link<string> _XML;
		
		private SiteState _State;
		
		private string _Name;
		
		private string _Owner_FK;
		
		private System.DateTime _LastUpdated;
		
		private string _Language;
		
		private string _Description;
		
		private bool _IsAdult;
		
		private string _RequiredDll;
		
		private EntityRef<User> _Owner;
		
		private EntityRef<Dll> _Dll;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnXMLChanging(string value);
    partial void OnXMLChanged();
    partial void OnStateChanging(SiteState value);
    partial void OnStateChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnOwner_FKChanging(string value);
    partial void OnOwner_FKChanged();
    partial void OnLastUpdatedChanging(System.DateTime value);
    partial void OnLastUpdatedChanged();
    partial void OnLanguageChanging(string value);
    partial void OnLanguageChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnIsAdultChanging(bool value);
    partial void OnIsAdultChanged();
    partial void OnRequiredDllChanging(string value);
    partial void OnRequiredDllChanged();
    #endregion
		
		public Site()
		{
			this._Owner = default(EntityRef<User>);
			this._Dll = default(EntityRef<Dll>);
			OnCreated();
		}
		
		[Column(Storage="_XML", DbType="nvarchar(max)", CanBeNull=false)]
		public string XML
		{
			get
			{
				return this._XML.Value;
			}
			set
			{
				if ((this._XML.Value != value))
				{
					this.OnXMLChanging(value);
					this.SendPropertyChanging();
					this._XML.Value = value;
					this.SendPropertyChanged("XML");
					this.OnXMLChanged();
				}
			}
		}
		
		[Column(Storage="_State", DbType="tinyint", CanBeNull=false)]
		public SiteState State
		{
			get
			{
				return this._State;
			}
			set
			{
				if ((this._State != value))
				{
					this.OnStateChanging(value);
					this.SendPropertyChanging();
					this._State = value;
					this.SendPropertyChanged("State");
					this.OnStateChanged();
				}
			}
		}
		
		[Column(Storage="_Name", DbType="nvarchar(200)", CanBeNull=false, IsPrimaryKey=true)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[Column(Storage="_Owner_FK", DbType="nvarchar(200)", CanBeNull=false)]
		public string Owner_FK
		{
			get
			{
				return this._Owner_FK;
			}
			set
			{
				if ((this._Owner_FK != value))
				{
					if (this._Owner.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnOwner_FKChanging(value);
					this.SendPropertyChanging();
					this._Owner_FK = value;
					this.SendPropertyChanged("Owner_FK");
					this.OnOwner_FKChanged();
				}
			}
		}
		
		[Column(Storage="_LastUpdated")]
		public System.DateTime LastUpdated
		{
			get
			{
				return this._LastUpdated;
			}
			set
			{
				if ((this._LastUpdated != value))
				{
					this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
				}
			}
		}
		
		[Column(Storage="_Language", DbType="nvarchar(200)", CanBeNull=false)]
		public string Language
		{
			get
			{
				return this._Language;
			}
			set
			{
				if ((this._Language != value))
				{
					this.OnLanguageChanging(value);
					this.SendPropertyChanging();
					this._Language = value;
					this.SendPropertyChanged("Language");
					this.OnLanguageChanged();
				}
			}
		}
		
		[Column(Storage="_Description", DbType="nvarchar(max)", CanBeNull=false)]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[Column(Storage="_IsAdult")]
		public bool IsAdult
		{
			get
			{
				return this._IsAdult;
			}
			set
			{
				if ((this._IsAdult != value))
				{
					this.OnIsAdultChanging(value);
					this.SendPropertyChanging();
					this._IsAdult = value;
					this.SendPropertyChanged("IsAdult");
					this.OnIsAdultChanged();
				}
			}
		}
		
		[Column(Storage="_RequiredDll", DbType="nvarchar(200)", CanBeNull=false)]
		public string RequiredDll
		{
			get
			{
				return this._RequiredDll;
			}
			set
			{
				if ((this._RequiredDll != value))
				{
					if (this._Dll.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnRequiredDllChanging(value);
					this.SendPropertyChanging();
					this._RequiredDll = value;
					this.SendPropertyChanged("RequiredDll");
					this.OnRequiredDllChanged();
				}
			}
		}
		
		[Association(Name="User_Site", Storage="_Owner", ThisKey="Owner_FK", OtherKey="Email", IsForeignKey=true)]
		public User Owner
		{
			get
			{
				return this._Owner.Entity;
			}
			set
			{
				User previousValue = this._Owner.Entity;
				if (((previousValue != value) 
							|| (this._Owner.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Owner.Entity = null;
						previousValue.Sites.Remove(this);
					}
					this._Owner.Entity = value;
					if ((value != null))
					{
						value.Sites.Add(this);
						this._Owner_FK = value.Email;
					}
					else
					{
						this._Owner_FK = default(string);
					}
					this.SendPropertyChanged("Owner");
				}
			}
		}
		
		[Association(Name="Dll_Site", Storage="_Dll", ThisKey="RequiredDll", OtherKey="Name", IsForeignKey=true)]
		public Dll Dll
		{
			get
			{
				return this._Dll.Entity;
			}
			set
			{
				Dll previousValue = this._Dll.Entity;
				if (((previousValue != value) 
							|| (this._Dll.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Dll.Entity = null;
						previousValue.Sites.Remove(this);
					}
					this._Dll.Entity = value;
					if ((value != null))
					{
						value.Sites.Add(this);
						this._RequiredDll = value.Name;
					}
					else
					{
						this._RequiredDll = default(string);
					}
					this.SendPropertyChanged("Dll");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Email;
		
		private string _Password;
		
		private bool _IsAdmin;
		
		private EntitySet<Site> _Sites;
		
		private EntitySet<Dll> _Dlls;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnEmailChanging(string value);
    partial void OnEmailChanged();
    partial void OnPasswordChanging(string value);
    partial void OnPasswordChanged();
    partial void OnIsAdminChanging(bool value);
    partial void OnIsAdminChanged();
    #endregion
		
		public User()
		{
			this._Sites = new EntitySet<Site>(new Action<Site>(this.attach_Sites), new Action<Site>(this.detach_Sites));
			this._Dlls = new EntitySet<Dll>(new Action<Dll>(this.attach_Dlls), new Action<Dll>(this.detach_Dlls));
			OnCreated();
		}
		
		[Column(Storage="_Email", DbType="nvarchar(200)", CanBeNull=false, IsPrimaryKey=true)]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}
			}
		}
		
		[Column(Storage="_Password", DbType="nvarchar(200)", CanBeNull=false)]
		public string Password
		{
			get
			{
				return this._Password;
			}
			set
			{
				if ((this._Password != value))
				{
					this.OnPasswordChanging(value);
					this.SendPropertyChanging();
					this._Password = value;
					this.SendPropertyChanged("Password");
					this.OnPasswordChanged();
				}
			}
		}
		
		[Column(Storage="_IsAdmin")]
		public bool IsAdmin
		{
			get
			{
				return this._IsAdmin;
			}
			set
			{
				if ((this._IsAdmin != value))
				{
					this.OnIsAdminChanging(value);
					this.SendPropertyChanging();
					this._IsAdmin = value;
					this.SendPropertyChanged("IsAdmin");
					this.OnIsAdminChanged();
				}
			}
		}
		
		[Association(Name="User_Site", Storage="_Sites", ThisKey="Email", OtherKey="Owner_FK")]
		public EntitySet<Site> Sites
		{
			get
			{
				return this._Sites;
			}
			set
			{
				this._Sites.Assign(value);
			}
		}
		
		[Association(Name="User_Dll", Storage="_Dlls", ThisKey="Email", OtherKey="Owner_FK")]
		public EntitySet<Dll> Dlls
		{
			get
			{
				return this._Dlls;
			}
			set
			{
				this._Dlls.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Sites(Site entity)
		{
			this.SendPropertyChanging();
			entity.Owner = this;
		}
		
		private void detach_Sites(Site entity)
		{
			this.SendPropertyChanging();
			entity.Owner = null;
		}
		
		private void attach_Dlls(Dll entity)
		{
			this.SendPropertyChanging();
			entity.Owner = this;
		}
		
		private void detach_Dlls(Dll entity)
		{
			this.SendPropertyChanging();
			entity.Owner = null;
		}
	}
	
	[Table(Name="")]
	public partial class Report : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Site_FK;
		
		private System.DateTime _Date;
		
		private int _Id = default(int);
		
		private string _Message;
		
		private ReportType _Type;
		
		private EntityRef<Site> _Site;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSite_FKChanging(string value);
    partial void OnSite_FKChanged();
    partial void OnDateChanging(System.DateTime value);
    partial void OnDateChanged();
    partial void OnMessageChanging(string value);
    partial void OnMessageChanged();
    partial void OnTypeChanging(ReportType value);
    partial void OnTypeChanged();
    #endregion
		
		public Report()
		{
			this._Site = default(EntityRef<Site>);
			OnCreated();
		}
		
		[Column(Storage="_Site_FK", DbType="nvarchar(200)", CanBeNull=false)]
		public string Site_FK
		{
			get
			{
				return this._Site_FK;
			}
			set
			{
				if ((this._Site_FK != value))
				{
					if (this._Site.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSite_FKChanging(value);
					this.SendPropertyChanging();
					this._Site_FK = value;
					this.SendPropertyChanged("Site_FK");
					this.OnSite_FKChanged();
				}
			}
		}
		
		[Column(Storage="_Date")]
		public System.DateTime Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				if ((this._Date != value))
				{
					this.OnDateChanging(value);
					this.SendPropertyChanging();
					this._Date = value;
					this.SendPropertyChanged("Date");
					this.OnDateChanged();
				}
			}
		}
		
		[Column(Storage="_Id", AutoSync=AutoSync.OnInsert, IsPrimaryKey=true, IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public int Id
		{
			get
			{
				return this._Id;
			}
		}
		
		[Column(Storage="_Message", DbType="nvarchar(max)", CanBeNull=false)]
		public string Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				if ((this._Message != value))
				{
					this.OnMessageChanging(value);
					this.SendPropertyChanging();
					this._Message = value;
					this.SendPropertyChanged("Message");
					this.OnMessageChanged();
				}
			}
		}
		
		[Column(Storage="_Type", DbType="tinyint", CanBeNull=false)]
		public ReportType Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				if ((this._Type != value))
				{
					this.OnTypeChanging(value);
					this.SendPropertyChanging();
					this._Type = value;
					this.SendPropertyChanged("Type");
					this.OnTypeChanged();
				}
			}
		}
		
		[Association(Name="Site_Report", Storage="_Site", ThisKey="Site_FK", OtherKey="Name", IsForeignKey=true)]
		public Site Site
		{
			get
			{
				return this._Site.Entity;
			}
			set
			{
				if ((this._Site.Entity != value))
				{
					this.SendPropertyChanging();
					this._Site.Entity = value;
					this.SendPropertyChanged("Site");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="")]
	public partial class Dll : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _Name;
		
		private System.DateTime _LastUpdated;
		
		private string _Owner_FK;
		
		private string _MD5;
		
		private EntitySet<Site> _Sites;
		
		private EntityRef<User> _Owner;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnLastUpdatedChanging(System.DateTime value);
    partial void OnLastUpdatedChanged();
    partial void OnOwner_FKChanging(string value);
    partial void OnOwner_FKChanged();
    partial void OnMD5Changing(string value);
    partial void OnMD5Changed();
    #endregion
		
		public Dll()
		{
			this._Sites = new EntitySet<Site>(new Action<Site>(this.attach_Sites), new Action<Site>(this.detach_Sites));
			this._Owner = default(EntityRef<User>);
			OnCreated();
		}
		
		[Column(Storage="_Name", DbType="nvarchar(200)", CanBeNull=false, IsPrimaryKey=true)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[Column(Storage="_LastUpdated")]
		public System.DateTime LastUpdated
		{
			get
			{
				return this._LastUpdated;
			}
			set
			{
				if ((this._LastUpdated != value))
				{
					this.OnLastUpdatedChanging(value);
					this.SendPropertyChanging();
					this._LastUpdated = value;
					this.SendPropertyChanged("LastUpdated");
					this.OnLastUpdatedChanged();
				}
			}
		}
		
		[Column(Storage="_Owner_FK", DbType="nvarchar(200)", CanBeNull=false)]
		public string Owner_FK
		{
			get
			{
				return this._Owner_FK;
			}
			set
			{
				if ((this._Owner_FK != value))
				{
					if (this._Owner.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnOwner_FKChanging(value);
					this.SendPropertyChanging();
					this._Owner_FK = value;
					this.SendPropertyChanged("Owner_FK");
					this.OnOwner_FKChanged();
				}
			}
		}
		
		[Column(Storage="_MD5", DbType="nvarchar(200)", CanBeNull=false)]
		public string MD5
		{
			get
			{
				return this._MD5;
			}
			set
			{
				if ((this._MD5 != value))
				{
					this.OnMD5Changing(value);
					this.SendPropertyChanging();
					this._MD5 = value;
					this.SendPropertyChanged("MD5");
					this.OnMD5Changed();
				}
			}
		}
		
		[Association(Name="Dll_Site", Storage="_Sites", ThisKey="Name", OtherKey="RequiredDll")]
		public EntitySet<Site> Sites
		{
			get
			{
				return this._Sites;
			}
			set
			{
				this._Sites.Assign(value);
			}
		}
		
		[Association(Name="User_Dll", Storage="_Owner", ThisKey="Owner_FK", OtherKey="Email", IsForeignKey=true)]
		public User Owner
		{
			get
			{
				return this._Owner.Entity;
			}
			set
			{
				User previousValue = this._Owner.Entity;
				if (((previousValue != value) 
							|| (this._Owner.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Owner.Entity = null;
						previousValue.Dlls.Remove(this);
					}
					this._Owner.Entity = value;
					if ((value != null))
					{
						value.Dlls.Add(this);
						this._Owner_FK = value.Email;
					}
					else
					{
						this._Owner_FK = default(string);
					}
					this.SendPropertyChanged("Owner");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Sites(Site entity)
		{
			this.SendPropertyChanging();
			entity.Dll = this;
		}
		
		private void detach_Sites(Site entity)
		{
			this.SendPropertyChanging();
			entity.Dll = null;
		}
	}
}
#pragma warning restore 1591
