using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Bilten.Util;

namespace Bilten.Domain
{
    public abstract class DomainObject
    {
        private int id;
        public virtual int Id
        {
            get { return id; }
        }

        protected readonly string NULL = "__NULL__";

        public virtual void validate(Notification notification)
        {
            // TODO: Remove this when you implement IValidatable on DomainObject
            throw new Exception("Derived class should implement this method.");
        }

        private static Dictionary<int, DomainObject> clonedObjects =
            new Dictionary<int, DomainObject>();
        private static bool cloneRegion = false;
        private static List<TypeAssociationPair> region = new List<TypeAssociationPair>();

        // deep copies the whole graph, or only the types in region
        // virtual je zbog proxy podrske za NHibernate
        public virtual DomainObject Clone()
        {
            // gets the unique ID (within an AppDomain) for an object
            int hash = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);

            DomainObject result;
            if (clonedObjects.Count == 0)
            {
                // pocetak kloniranja (koren hijerarhije)
                result = (DomainObject)
                    this.GetType().GetConstructor(new Type[0]/*prazan niz*/).Invoke(null);
                clonedObjects.Add(hash, result);
                result.deepCopy(this);
                // kraj kloniranja

                // priprema za sledece kloniranje
                clonedObjects.Clear();
                cloneRegion = false;
            }
            else
            {
                // kloniranje nekog objekta koji nije koren
                if (!clonedObjects.ContainsKey(hash))
                {
                    // prvi put se nailazi na dati objekt
                    result = (DomainObject)
                        this.GetType().GetConstructor(new Type[0]).Invoke(null);
                    clonedObjects.Add(hash, result);
                    result.deepCopy(this);
                }
                else
                {
                    result = clonedObjects[hash];
                }
            }
            return result;
        }

        // deep copies part of the graph
        // virtual je zbog proxy podrske za NHibernate
        public virtual DomainObject Clone(TypeAssociationPair[] reg)
        {
            if (reg != null && reg.Length > 0)
            {
                region.Clear();
                region.AddRange(reg);
                cloneRegion = true;
            }
            return Clone();
        }

        protected virtual void deepCopy(DomainObject domainObject)
        {
            //id = domainObject.id;
        }

        protected static bool shouldClone(TypeAssociationPair typeAssociation)
        {
            if (!cloneRegion)
                return true;
            foreach (TypeAssociationPair ta in region)
            {
                if (ta.Type == typeAssociation.Type
                && ta.Association == typeAssociation.Association)
                    return true;
            }
            return false;
        }

        // shallow copy
        // virtual je zbog proxy podrske za NHibernate
        public virtual DomainObject shallowCopy()
        {
            return (DomainObject)this.MemberwiseClone();
        }

    }

    public class TypeAssociationPair
    {
        private Type type;
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        private string association;
        public string Association
        {
            get { return association; }
            set { association = value; }
        }

        public TypeAssociationPair(Type type)
        {
            this.type = type;
            this.association = String.Empty;
        }

        public TypeAssociationPair(Type type, string association)
        {
            this.type = type;
            this.association = association;
        }
    }
}
