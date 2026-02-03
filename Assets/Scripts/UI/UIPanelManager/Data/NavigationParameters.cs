namespace Core.UI.Data
{
    public abstract class NavigationParameters
    {
        public bool DontHidePreviousScreen { get; set; }

        public static NavigationParameters CreateDefault(out NavigationParameters parameter)
        {
            parameter = new DefaultNavigationParameter();
            return parameter;
        }

        private class DefaultNavigationParameter : NavigationParameters
        {

        }
    }
}
