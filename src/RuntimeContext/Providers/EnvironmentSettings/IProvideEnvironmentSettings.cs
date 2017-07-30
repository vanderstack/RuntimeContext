namespace Vanderstack.RuntimeContext.Providers
{
    public interface IProvideEnvironmentSettings
    {
        /// <summary>
        /// The environment name.
        /// </summary>
        string EnvironmentName { get; }
    }
}
