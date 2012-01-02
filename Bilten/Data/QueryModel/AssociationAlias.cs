namespace Bilten.Data.QueryModel
{
    public class AssociationAlias
    {
        private string associationPath;
        public string AssociationPath
        {
            get { return associationPath; }
            set { associationPath = value; }
        }

        private string aliasName;
        public string AliasName
        {
            get { return aliasName; }
            set { aliasName = value; }
        }

        public AssociationAlias(string associationPath, string aliasName)
        {
            this.associationPath = associationPath;
            this.aliasName = aliasName;
        }
    }
}
