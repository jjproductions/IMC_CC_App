namespace IMC_CC_App.Components
{
    public class RouterBase
    {
        protected ILogger Logger;
        public string UrlFragment;
        public virtual void AddRoutes(WebApplication app) { }
    }
}
