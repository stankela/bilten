using System;

namespace Bilten.Data.QueryModel
{
    [Serializable]
    public enum StringMatchMode
    {
        Exact,
        Anywhere,
        End,
        Start
    }
}
