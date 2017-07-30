namespace Vanderstack.RuntimeContext.Providers
{
    public interface IProvideApplicationSettings
    {
        TRootSettingSection OfType<TRootSettingSection>() where TRootSettingSection : IApplicationSettingRootSection;
    }

    public interface IApplicationSettingRootSection
    {

    }
}
