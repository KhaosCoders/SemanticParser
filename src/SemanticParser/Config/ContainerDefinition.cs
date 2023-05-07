using System.Text.RegularExpressions;

namespace SemanticParser.Config;
internal class ContainerDefinition
{
    public ContainerDefinition(ContainerSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        ArgumentNullException.ThrowIfNull(setting.BeginPattern, nameof(setting.BeginPattern));
        ArgumentNullException.ThrowIfNull(setting.EndPattern, nameof(setting.EndPattern));

        this.BeginPattern = new Regex(setting.BeginPattern, RegexOptions.Compiled);
        this.EndPattern = new Regex(setting.EndPattern, RegexOptions.Compiled);
    }

    public Regex BeginPattern { get; }

    public Regex EndPattern { get; }
}
