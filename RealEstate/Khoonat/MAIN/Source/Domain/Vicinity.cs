using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Diagnostics;
using System.Linq;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Domain
{
	[DebuggerDisplay("{Type}: {Name}")]
    public class Vicinity
    {
        public long ID { get; set; }

        public string Name { get; set; }
        public string AlternativeNames { get; set; }
        public string AdditionalSearchText { get; set; }
        public string Description { get; set; }
		public string OfficialLinkUrl { get; set; }
		public string WikiLinkUrl { get; set; }
        public string AdministrativeNotes { get; set; }
        public bool Enabled { get; set; }
        public int Order { get; set; }

        public VicinityType Type { get; set; }
        public VicinityType WellKnownScope { get; set; }
        public bool ShowTypeInTitle { get; set; }
		public bool ShowInHierarchy { get; set; }
        public bool ShowInSummary { get; set; }
        
        public bool CanContainPropertyRecords { get; set; }

        public DbGeography CenterPoint { get; set; }
        public DbGeography Boundary { get; set; }

        //
        // Self-referencing hierarchy relationship

        public Vicinity Parent { get; set; }
        public long? ParentID { get; set; }
        public ICollection<Vicinity> Children { get; set; }

        //
        // Collections of entities related to Vicinity

        public ICollection<PropertyListing> PropertyListings { get; set; }

        #region Initialization

        public Vicinity() 
        {
        }

        public Vicinity(Vicinity source, bool shallow = false)
		{
			ID = source.ID;
			Name = source.Name;
            AlternativeNames = source.AlternativeNames;
            AdditionalSearchText = source.AdditionalSearchText;
            Description = source.Description;
            AdministrativeNotes = source.AdministrativeNotes;
            Enabled = source.Enabled;
            Order = source.Order;
           
            Type = source.Type;
            WellKnownScope = source.WellKnownScope;
            ShowTypeInTitle = source.ShowTypeInTitle;
            ShowInSummary = source.ShowInSummary;

            CanContainPropertyRecords = source.CanContainPropertyRecords;

            CenterPoint = source.CenterPoint;
            Boundary = source.Boundary;

            Parent = source.Parent;
            ParentID = source.ParentID;
           

            PropertyListings = source.PropertyListings;


	        Children = source.Children;
			if (!shallow)
			{
                Children = Copy(source.Children, true);
			}
		}
        public static Vicinity Copy(Vicinity source, bool shallow = false)
        {
            return source == null ? null : new Vicinity(source, shallow);
        }

        public static ICollection<Vicinity> Copy(ICollection<Vicinity> source, bool shallow = false)
        {
            return source?.Select(v => Copy(v, shallow)).ToList();
        }

        #endregion

		#region Utility methods

	    public IEnumerable<long> GetParentIDs()
	    {
		    return GetParentIDsInclusive().Skip(1);
	    }

	    public IEnumerable<long> GetParentIDsInclusive()
	    {
			var vicinity = this;
		    var vicinityId = (long?) ID;

			while (vicinityId.HasValue)
			{
				if (vicinity == null)
					throw new InvalidOperationException("Parent property is not populated. This method should be called on the Vicinity entities that have their Parent property loaded / populated.");

				yield return vicinityId.Value;

				vicinityId = vicinity.ParentID;
				vicinity = vicinity.Parent;
			}
		}

	    public IEnumerable<Vicinity> GetParents()
	    {
		    return GetParentsInclusive().Skip(1);
	    }

	    public IEnumerable<Vicinity> GetParentsInclusive()
	    {
			var vicinity = this;

			while (vicinity != null)
			{
				if (vicinity.ParentID.HasValue && vicinity.Parent == null)
					throw new InvalidOperationException("Parent property" +
					                                    " is not populated. This method should be called on the Vicinity entities that have their Parent property loaded / populated.");

				yield return vicinity;
				vicinity = vicinity.Parent;
			}
		}

	    public IEnumerable<Vicinity> GetChildren()
	    {
		    return Children ?? Enumerable.Empty<Vicinity>();
	    }

	    public IEnumerable<Vicinity> GetDfsChildrenTree()
	    {
		    return GetDfsChildrenTreeInclusive().Skip(1);
	    }

		public IEnumerable<Vicinity> GetDfsChildrenTreeInclusive()
		{
			var stack = new Stack<Vicinity>(this.Yield());
			while (stack.Count > 0)
			{
				var currentNode = stack.Pop();
				stack.PushAll(currentNode.GetChildren());

				yield return currentNode;
			}
		}

	    public IEnumerable<Vicinity> GetBfsChildrenTree()
	    {
		    return GetBfsChildrenTreeInclusive().Skip(1);
	    }

		public IEnumerable<Vicinity> GetBfsChildrenTreeInclusive()
		{
			var queue = new Queue<Vicinity>(this.Yield());
			while (queue.Count > 0)
			{
				var currentNode = queue.Dequeue();
				queue.EnqueueAll(currentNode.GetChildren());

				yield return currentNode;
			}
		}

		#endregion
	}
}