namespace Soenneker.Lepton.Suite;

/// <summary>A parameter descriptor with a compile-time generated, typed setter.</summary>
public sealed class SharedParameterProperty(string name, Action<LeptonComponent, object?> setter, bool cascading, bool direct, bool capture)
{
    public string Name { get; } = name;
    public Action<LeptonComponent, object?> Setter { get; } = setter;
    public bool Cascading { get; } = cascading;
    public bool Direct { get; } = direct;
    public bool Capture { get; } = capture;
}
