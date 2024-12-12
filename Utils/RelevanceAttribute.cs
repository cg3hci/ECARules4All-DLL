namespace ECARules4All_DLL.Utils
{
    public class ECARelevanceAttribute : System.Attribute
    {
        public bool isRelevance = false;
        public ushort relevanceLevel = 0;

        public ECARelevanceAttribute(bool isRelevance)
        {
            this.isRelevance = isRelevance;
        }

        public ECARelevanceAttribute(bool isRelevance, ushort relevanceLevel)
        {
            this.isRelevance = isRelevance;
            this.relevanceLevel = relevanceLevel;
        }
    }
}