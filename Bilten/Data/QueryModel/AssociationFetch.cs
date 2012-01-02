using System;

namespace Bilten.Data.QueryModel
{
    [Serializable]
    public enum AssociationFetchMode
    {
        Default = 0,
        Lazy = 1,
        Eager = 2,
    }

    public class AssociationFetch
    {
        private string associationPath;
        public string AssociationPath
        {
            get { return associationPath; }
            set { associationPath = value; }
        }

        private AssociationFetchMode fetchMode;
        public AssociationFetchMode FetchMode
        {
            get { return fetchMode; }
            set { fetchMode = value; }
        }

        public AssociationFetch(string associationPath, AssociationFetchMode fetchMode)
        {
            this.associationPath = associationPath;
            this.fetchMode = fetchMode;
        }
    }
}
